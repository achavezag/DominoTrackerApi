using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Services;

public class MatchService
{
    private readonly IMatchRepository _repository;
    private readonly ITeamRepository _teamRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICurrentSessionAccessor _currentSessionAccessor;

    public MatchService(
        IMatchRepository repository,
        ITeamRepository teamRepository,
        ISeasonRepository seasonRepository,
        ICurrentSessionAccessor currentSessionAccessor)
    {
        _repository = repository;
        _teamRepository = teamRepository;
        _seasonRepository = seasonRepository;
        _currentSessionAccessor = currentSessionAccessor;
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
            SeasonId = match.SeasonId,

            TeamA = new TeamDto
            {
                Id = match.TeamA.Id,
                Player1 = new PlayerDto
                {
                    Id = match.TeamA.Player1.Id,
                    Name = match.TeamA.Player1.Name,
                    Nickname = match.TeamA.Player1.Nickname,
                    Image = match.TeamA.Player1.Image
                },
                Player2 = new PlayerDto
                {
                    Id = match.TeamA.Player2.Id,
                    Name = match.TeamA.Player2.Name,
                    Nickname = match.TeamA.Player2.Nickname,
                    Image = match.TeamA.Player2.Image
                }
            },

            TeamB = new TeamDto
            {
                Id = match.TeamB.Id,
                Player1 = new PlayerDto
                {
                    Id = match.TeamB.Player1.Id,
                    Name = match.TeamB.Player1.Name,
                    Nickname = match.TeamB.Player1.Nickname,
                    Image = match.TeamB.Player1.Image
                },
                Player2 = new PlayerDto
                {
                    Id = match.TeamB.Player2.Id,
                    Name = match.TeamB.Player2.Name,
                    Nickname = match.TeamB.Player2.Nickname,
                    Image = match.TeamB.Player2.Image
                }
            },
        };
    }

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

        var matchDate = request.Date.Date;
        var today = DateTime.UtcNow.Date;

        if (matchDate >= today)
            throw new Exception("La fecha del partido debe ser anterior a la fecha actual");

        var season = await _seasonRepository.GetByDateAsync(matchDate)
            ?? throw new Exception($"No hay una temporada definida para la fecha {matchDate:yyyy-MM-dd}");

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

            // Se conserva el día/mes/año elegido por el usuario, pero se le agrega
            // la hora exacta de guardado para preservar el orden de inserción entre
            // partidos de un mismo día (el historial del día se ordena por FinishedAt).
            FinishedAt = matchDate + DateTime.UtcNow.TimeOfDay,

            SeasonId = season.Id,
            SessionId = _currentSessionAccessor.SessionId
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
            FinishedAt = m.FinishedAt,
            SeasonId = m.SeasonId
        }).ToList();
    }

    public async Task<List<MatchListItemResponse>> GetFilteredAsync(
        DateTime? fromDate,
        DateTime? toDate,
        string? type,
        Guid? teamAId,
        Guid? teamBId,
        List<int>? seasonIds = null
    )
    {
        var matches = await _repository.GetAllWithTeamsAsync();

        var query = matches.AsQueryable();

        // Filtro por temporada(s); sin filtro = todas las temporadas combinadas
        if (seasonIds != null && seasonIds.Count > 0)
        {
            query = query.Where(m => seasonIds.Contains(m.SeasonId));
        }

        // Filtro por fechas (por día, sin importar la hora de guardado)
        if (fromDate.HasValue)
        {
            query = query.Where(m => m.FinishedAt!.Value.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(m => m.FinishedAt!.Value.Date <= toDate.Value.Date);
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
            .OrderByDescending(m => m.FinishedAt)
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
                FinishedAt = m.FinishedAt,
                SeasonId = m.SeasonId
            })
            .DistinctBy(m => m.Id)
            .ToList();
    }
}