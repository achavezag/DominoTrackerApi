using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Services;

public class MatchService
{
    private readonly IMatchRepository _repository;
    private readonly ITeamRepository _teamRepository;

    public MatchService(IMatchRepository repository, ITeamRepository teamRepository)
    {
        _repository = repository;
        _teamRepository = teamRepository;
    }

    public async Task<MatchDetailResponse?> GetMatchDetailAsync(Guid matchId)
    {
        var match = await _repository.GetByIdWithDetailsAsync(matchId);

        if (match == null)
            return null;

        return new MatchDetailResponse
        {
            Id = match.Id,
            Type = match.Type.ToString(),

            TeamAScore = match.TeamAScore,
            TeamBScore = match.TeamBScore,

            WinningTeamId = match.WinningTeamId,
            Status = match.Status.ToString(),

            TeamA = new TeamDto
            {
                Id = match.TeamA.Id,
                Player1 = new PlayerDto
                {
                    Id = match.TeamA.Player1.Id,
                    Name = match.TeamA.Player1.Name,
                    Nickname = match.TeamA.Player1.Nickname
                },
                Player2 = new PlayerDto
                {
                    Id = match.TeamA.Player2.Id,
                    Name = match.TeamA.Player2.Name,
                    Nickname = match.TeamA.Player2.Nickname
                }
            },

            TeamB = new TeamDto
            {
                Id = match.TeamB.Id,
                Player1 = new PlayerDto
                {
                    Id = match.TeamB.Player1.Id,
                    Name = match.TeamB.Player1.Name,
                    Nickname = match.TeamB.Player1.Nickname
                },
                Player2 = new PlayerDto
                {
                    Id = match.TeamB.Player2.Id,
                    Name = match.TeamB.Player2.Name,
                    Nickname = match.TeamB.Player2.Nickname
                }
            },
        };
    }

    /* public async Task<PlayerStatsResponse?> GetPlayerStatsAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);

        if (player == null)
            return null;

        var matches = await _repository.GetAllWithTeamsAsync();

        var playedMatches = matches
            .Where(m =>
                m.TeamA.Player1Id == playerId || m.TeamA.Player2Id == playerId ||
                m.TeamB.Player1Id == playerId || m.TeamB.Player2Id == playerId)
            .ToList();

        int wins = 0;
        int losses = 0;
        int fixedMatches = 0;
        int rotatingMatches = 0;

        foreach (var match in playedMatches)
        {
            bool isInTeamA =
                match.TeamA.Player1Id == playerId || match.TeamA.Player2Id == playerId;

            var playerTeamId = isInTeamA ? match.TeamAId : match.TeamBId;

            if (match.WinningTeamId == playerTeamId) wins++;
            else if (match.WinningTeamId != null) losses++;

            if (match.Type == MatchType.Fixed) fixedMatches++;
            else if (match.Type == MatchType.Rotating) rotatingMatches++;
        }

        return new PlayerStatsResponse
        {
            PlayerId = player.Id,
            Name = player.Name,
            Nickname = player.Nickname,

            MatchesPlayed = playedMatches.Count,
            Wins = wins,
            Losses = losses,

            FixedMatches = fixedMatches,
            RotatingMatches = rotatingMatches
        };
    } */

    public async Task<MatchDetailResponse> CreateMatchAsync(CreateMatchRequest request)
    {
        // Validar que los equipos sean distintos
        if (request.TeamAId == request.TeamBId)
            throw new Exception("Teams must be different");

        if (request.TeamAScore < 0 || request.TeamBScore < 0)
            throw new Exception("Scores must be positive");

        if (request.TeamAScore == request.TeamBScore)
            throw new Exception("A match cannot end in a tie");

        bool validScore =
            (request.TeamAScore == 15 && request.TeamBScore < 15) ||
            (request.TeamBScore == 15 && request.TeamAScore < 15) ||
            (request.TeamAScore == 11 && request.TeamBScore == 0) ||
            (request.TeamBScore == 11 && request.TeamAScore == 0);

        if (!validScore)
            throw new Exception("Invalid final score");

        // Obtener equipos
        var teamA = await _teamRepository.GetByIdAsync(request.TeamAId);
        var teamB = await _teamRepository.GetByIdAsync(request.TeamBId);

        if (teamA == null || teamB == null)
            throw new Exception("One or both teams do not exist");

        var winningTeamId =
        request.TeamAScore > request.TeamBScore
            ? request.TeamAId
            : request.TeamBId;

        // Crear match
        var match = new Match
        {
            Id = Guid.NewGuid(),

            TeamAId = request.TeamAId,
            TeamBId = request.TeamBId,

            Type = request.Type,

            TeamAScore = request.TeamAScore,
            TeamBScore = request.TeamBScore,

            WinningTeamId = winningTeamId,

            Status = MatchStatus.Finished,

            CreatedAt = DateTime.UtcNow,
            FinishedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(match);
        await _repository.SaveChangesAsync();

        return await GetMatchDetailAsync(match.Id)
            ?? throw new Exception("Error retrieving created match");
    }

    public async Task<List<MatchListItemResponse>> GetAllAsync()
    {
        var matches = await _repository.GetAllWithTeamsAsync();

        return matches.Select(m => new MatchListItemResponse
        {
            Id = m.Id,
            TeamAName = $"{m.TeamA.Player1.Name} / {m.TeamA.Player2.Name}",
            TeamBName = $"{m.TeamB.Player1.Name} / {m.TeamB.Player2.Name}",
            TeamAScore = m.TeamAScore,
            TeamBScore = m.TeamBScore,
            Status = m.Status.ToString(),
            Type = m.Type.ToString(),
            CreatedAt = m.CreatedAt,
            FinishedAt = m.FinishedAt
        }).ToList();
    }

    public async Task<List<MatchListItemResponse>> GetFilteredAsync(
        DateTime? fromDate,
        DateTime? toDate,
        string? type,
        Guid? teamAId,
        Guid? teamBId
    )
    {
        var matches = await _repository.GetAllWithTeamsAsync();

        var query = matches.AsQueryable();

        // Filtro por fechas
        if (fromDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt <= toDate.Value);
        }

        // Filtro por tipo
        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(m => m.Type.ToString() == type);
        }

        // Filtros por teams
        if (teamAId.HasValue && teamBId.HasValue)
        {
            // VS exacto entre dos equipos (sin importar orden)
            query = query.Where(m =>
                (m.TeamAId == teamAId && m.TeamBId == teamBId) ||
                (m.TeamAId == teamBId && m.TeamBId == teamAId)
            );
        }
        else if (teamAId.HasValue)
        {
            // Participa teamA
            query = query.Where(m =>
                m.TeamAId == teamAId || m.TeamBId == teamAId
            );
        }
        else if (teamBId.HasValue)
        {
            // Participa teamB
            query = query.Where(m =>
                m.TeamAId == teamBId || m.TeamBId == teamBId
            );
        }

        // Mapping
        return query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MatchListItemResponse
            {
                Id = m.Id,
                TeamAName = $"{m.TeamA.Player1.Nickname} / {m.TeamA.Player2.Nickname}",
                TeamBName = $"{m.TeamB.Player1.Nickname} / {m.TeamB.Player2.Nickname}",
                TeamAScore = m.TeamAScore,
                TeamBScore = m.TeamBScore,
                WinningTeamId = m.WinningTeamId,
                Status = m.Status.ToString(),
                Type = m.Type.ToString(),
                FinishedAt = m.FinishedAt
            })
            .DistinctBy(m => m.Id)
            .ToList();
    }
}