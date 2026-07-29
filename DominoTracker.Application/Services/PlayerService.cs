using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;
using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.Application.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IWarRepository _warRepository;

    public PlayerService(IPlayerRepository playerRepository, IMatchRepository matchRepository, IWarRepository warRepository)
    {
        _playerRepository = playerRepository;
        _matchRepository = matchRepository;
        _warRepository = warRepository;
    }

    public async Task<List<PlayerStatsResponse>> GetAllStatsAsync()
    {
        var players = await _playerRepository.GetAllAsync();
        var matches = await _matchRepository.GetAllWithTeamsAsync();
        var wars = await _warRepository.GetAllWithMatchesAsync();

        var result = new List<PlayerStatsResponse>();

        foreach (var player in players)
        {
            var playedMatches = matches
                .Where(m =>
                    m.TeamA.Player1Id == player.Id ||
                    m.TeamA.Player2Id == player.Id ||
                    m.TeamB.Player1Id == player.Id ||
                    m.TeamB.Player2Id == player.Id)
                .ToList();

            int wins = 0;
            int losses = 0;
            int warWins = 0;
            int warLosses = 0;
            int fixedWins = 0;
            int fixedLosses = 0;
            int rotatingWins = 0;
            int rotatingLosses = 0;

            foreach (var match in playedMatches)
            {
                bool isInTeamA =
                    match.TeamA.Player1Id == player.Id ||
                    match.TeamA.Player2Id == player.Id;

                var playerTeamId = isInTeamA ? match.TeamAId : match.TeamBId;

                bool isWin = match.WinningTeamId == playerTeamId;
                bool isLoss = match.WinningTeamId != null && !isWin;

                if (isWin)
                    wins++;

                if (isLoss)
                    losses++;

                if (match.Type == MatchType.Fixed)
                {
                    if (isWin)
                        fixedWins++;

                    if (isLoss)
                        fixedLosses++;
                }
                else if (match.Type == MatchType.Rotating)
                {
                    if (isWin)
                        rotatingWins++;

                    if (isLoss)
                        rotatingLosses++;
                }
            }

            var playedWars = wars
                .Where(w =>
                    w.WarMatches.Any(wm =>
                        wm.Match.TeamA.Player1Id == player.Id ||
                        wm.Match.TeamA.Player2Id == player.Id ||
                        wm.Match.TeamB.Player1Id == player.Id ||
                        wm.Match.TeamB.Player2Id == player.Id))
                .ToList();

            foreach (var war in playedWars)
            {
                int winsInWar = 0;
                int lossesInWar = 0;

                foreach (var match in war.WarMatches.Select(wm => wm.Match))
                {
                    bool isInTeamA =
                        match.TeamA.Player1Id == player.Id ||
                        match.TeamA.Player2Id == player.Id;

                    var playerTeamId =
                        isInTeamA ? match.TeamAId : match.TeamBId;

                    if (match.WinningTeamId == playerTeamId)
                        winsInWar++;
                    else if (match.WinningTeamId != null)
                        lossesInWar++;
                }

                if (winsInWar > lossesInWar)
                    warWins++;
                else if (lossesInWar > winsInWar)
                    warLosses++;
            }

            result.Add(new PlayerStatsResponse
            {
                PlayerId = player.Id,
                Name = player.Name,
                Nickname = player.Nickname,

                MatchesPlayed = playedMatches.Count,
                Wins = wins,
                Losses = losses,
                WinRate = playedMatches.Count > 0
                    ? Math.Round((double)wins / playedMatches.Count, 3)
                    : 0,

                FixedMatches = playedMatches.Count(m => m.Type == MatchType.Fixed),
                FixedWins = fixedWins,
                FixedLosses = fixedLosses,
                FixedMatchesWinRate = (fixedWins + fixedLosses) > 0
                    ? Math.Round((double)fixedWins / (fixedWins + fixedLosses), 3)
                    : 0,

                RotatingMatches = playedMatches.Count(m => m.Type == MatchType.Rotating),
                RotatingWins = rotatingWins,
                RotatingLosses = rotatingLosses,
                RotatingMatchesWinRate = (rotatingWins + rotatingLosses) > 0
                    ? Math.Round((double)rotatingWins / (rotatingWins + rotatingLosses), 3)
                    : 0,

                WarsPlayed = playedWars.Count,
                WarWins = warWins,
                WarLosses = warLosses,
                WarWinRate = playedWars.Count > 0
                    ? Math.Round((double)warWins / playedWars.Count, 3)
                    : 0,
            });
        }

        return result;
    }

    public async Task<HeadToHeadResponse> GetHeadToHeadAsync(Guid player1Id, Guid player2Id)
    {
        if (player1Id == player2Id)
            throw new Exception("Players must be different");

        var matches = await _matchRepository.GetAllWithTeamsAsync();

        var wars = await _warRepository.GetAllWithMatchesAsync();

        // HEAD TO HEAD MATCHES
        var headToHeadMatches = matches
            .Where(m =>
            {
                bool player1InTeamA =
                    m.TeamA.Player1Id == player1Id ||
                    m.TeamA.Player2Id == player1Id;

                bool player1InTeamB =
                    m.TeamB.Player1Id == player1Id ||
                    m.TeamB.Player2Id == player1Id;

                bool player2InTeamA =
                    m.TeamA.Player1Id == player2Id ||
                    m.TeamA.Player2Id == player2Id;

                bool player2InTeamB =
                    m.TeamB.Player1Id == player2Id ||
                    m.TeamB.Player2Id == player2Id;

                return
                    (player1InTeamA && player2InTeamB) ||
                    (player1InTeamB && player2InTeamA);
            })
            .ToList();

        int player1Wins = 0;
        int player2Wins = 0;

        foreach (var match in headToHeadMatches)
        {
            bool player1InWinningTeam =
                (match.WinningTeamId == match.TeamAId &&
                    (match.TeamA.Player1Id == player1Id ||
                    match.TeamA.Player2Id == player1Id))
                ||
                (match.WinningTeamId == match.TeamBId &&
                    (match.TeamB.Player1Id == player1Id ||
                    match.TeamB.Player2Id == player1Id));

            if (player1InWinningTeam)
                player1Wins++;
            else
                player2Wins++;
        }

        // WAR MATCHES
        var warMatchIds = wars
            .SelectMany(w => w.WarMatches)
            .Select(wm => wm.MatchId)
            .ToHashSet();

        var warMatches = headToHeadMatches
            .Where(m => warMatchIds.Contains(m.Id))
            .ToList();

        int player1WarMatchWins = 0;
        int player2WarMatchWins = 0;

        foreach (var match in warMatches)
        {
            bool player1Won =
                (match.TeamAId == match.WinningTeamId &&
                    (match.TeamA.Player1Id == player1Id ||
                    match.TeamA.Player2Id == player1Id))
                ||
                (match.TeamBId == match.WinningTeamId &&
                    (match.TeamB.Player1Id == player1Id ||
                    match.TeamB.Player2Id == player1Id));

            if (player1Won)
                player1WarMatchWins++;
            else
                player2WarMatchWins++;
        }

        // WARS
        var relevantWars = wars
            .Where(w =>
            {
                var firstMatch = w.WarMatches.FirstOrDefault()?.Match;

                if (firstMatch == null)
                    return false;

                bool player1InTeamA =
                    firstMatch.TeamA.Player1Id == player1Id ||
                    firstMatch.TeamA.Player2Id == player1Id;

                bool player1InTeamB =
                    firstMatch.TeamB.Player1Id == player1Id ||
                    firstMatch.TeamB.Player2Id == player1Id;

                bool player2InTeamA =
                    firstMatch.TeamA.Player1Id == player2Id ||
                    firstMatch.TeamA.Player2Id == player2Id;

                bool player2InTeamB =
                    firstMatch.TeamB.Player1Id == player2Id ||
                    firstMatch.TeamB.Player2Id == player2Id;

                return
                    (player1InTeamA && player2InTeamB) ||
                    (player1InTeamB && player2InTeamA);
            })
            .ToList();

        int player1WarWins = 0;
        int player2WarWins = 0;

        foreach (var war in relevantWars)
        {
            bool player1WonWar =
                (war.TeamAId == war.WinningTeamId && war.TeamA != null &&
                    (war.TeamA.Player1Id == player1Id || war.TeamA.Player2Id == player1Id))
                ||
                war.TeamBId == war.WinningTeamId && war.TeamB != null &&
                    (war.TeamB.Player1Id == player1Id || war.TeamB.Player2Id == player1Id);

            if (war.WinningTeamId == null)
                continue;

            if (player1WonWar)
                player1WarWins++;
            else
                player2WarWins++;
        }

        int total = headToHeadMatches.Count;

        return new HeadToHeadResponse
        {
            Player1Id = player1Id,
            Player2Id = player2Id,

            MatchesPlayed = total,

            Player1Wins = player1Wins,
            Player2Wins = player2Wins,

            Player1WarMatchWins = player1WarMatchWins,
            Player2WarMatchWins = player2WarMatchWins,

            Player1WarWins = player1WarWins,
            Player2WarWins = player2WarWins
        };
    }
}