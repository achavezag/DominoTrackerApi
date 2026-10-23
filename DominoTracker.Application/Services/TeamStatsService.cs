using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;
using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.Application.Services;

public class TeamStatsService : ITeamStatsService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IWarRepository _warRepository;
    private readonly ISeasonRepository _seasonRepository;

    public TeamStatsService(
        ITeamRepository teamRepository,
        IMatchRepository matchRepository,
        IWarRepository warRepository,
        ISeasonRepository seasonRepository)
    {
        _teamRepository = teamRepository;
        _matchRepository = matchRepository;
        _warRepository = warRepository;
        _seasonRepository = seasonRepository;
    }

    public async Task<List<TeamStatsResponse>> GetAllStatsAsync(List<int>? seasonIds = null)
    {
        var teams = await _teamRepository.GetAllAsync();
        var matches = await _matchRepository.GetAllWithTeamsAsync();
        var wars = await _warRepository.GetAllTeamWithMatchesAsync();

        var seasons = await _seasonRepository.GetAllAsync();
        var seasonById = seasons.ToDictionary(s => s.Id);

        if (seasonIds != null && seasonIds.Count > 0)
        {
            matches = matches.Where(m => seasonIds.Contains(m.SeasonId)).ToList();
            wars = wars.Where(w => seasonIds.Contains(w.SeasonId)).ToList();
        }

        var result = new List<TeamStatsResponse>();

        foreach (var team in teams)
        {
            var playedMatches = matches
                .Where(m =>
                    m.TeamAId == team.Id ||
                    m.TeamBId == team.Id)
                .ToList();

            int wins = 0;
            int losses = 0;

            int fixedWins = 0;
            int fixedLosses = 0;

            int warWins = 0;
            int warLosses = 0;

            int rotatingWins = 0;
            int rotatingLosses = 0;

            foreach (var match in playedMatches)
            {
                bool isWin = match.WinningTeamId == team.Id;
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

            var teamWars = wars
                .Where(w =>
                    w.WarMatches.Any(wm =>
                        wm.Match.TeamAId == team.Id ||
                        wm.Match.TeamBId == team.Id))
                .ToList();

            foreach (var war in teamWars)
            {
                int winsInWar = 0;
                int lossesInWar = 0;

                foreach (var match in war.WarMatches.Select(wm => wm.Match))
                {
                    if (match.WinningTeamId == team.Id)
                        winsInWar++;
                    else if (match.WinningTeamId != null)
                        lossesInWar++;
                }

                if (winsInWar > lossesInWar)
                    warWins++;
                else if (lossesInWar > winsInWar)
                    warLosses++;
            }

            result.Add(new TeamStatsResponse
            {
                TeamId = team.Id,

                Player1Id = team.Player1Id,
                Player1Nickname = team.Player1.Nickname,

                Player2Id = team.Player2Id,
                Player2Nickname = team.Player2.Nickname,

                MatchesPlayed = playedMatches.Count,
                SeasonsPlayed = playedMatches
                    .Select(m => m.SeasonId)
                    .Distinct()
                    .OrderBy(id => id)
                    .Select(id => new SeasonPlayedDto { Id = id, Alias = seasonById[id].Alias })
                    .ToArray(),
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

                WarsPlayed = teamWars.Count,
                WarWins = warWins,
                WarLosses = warLosses,
                WarWinRate = teamWars.Count > 0
                    ? Math.Round((double)warWins / teamWars.Count, 3)
                    : 0,
            });
        }

        return result;
    }
}