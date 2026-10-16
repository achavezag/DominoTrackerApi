using DominoTracker.Application.DTOs;
using DominoTracker.Application.Exceptions;
using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;
using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.Application.Services;

public class WarService : IWarService
{
    private readonly IWarRepository _repository;

    public WarService(IWarRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WarListItemResponse>> GetAllAsync(List<int>? seasonIds = null)
    {
        var wars = await _repository.GetAllAsync();

        if (seasonIds != null && seasonIds.Count > 0)
        {
            wars = wars.Where(w => seasonIds.Contains(w.SeasonId)).ToList();
        }

        return wars.Select(w => new WarListItemResponse
        {
            Id = w.Id,
            Date = w.Date,

            TeamAName = $"{w.TeamA.Player1.Nickname} / {w.TeamA.Player2.Nickname}",

            TeamBName = $"{w.TeamB.Player1.Nickname} / {w.TeamB.Player2.Nickname}"
        })
       .ToList();
    }

    public async Task<WarDetailResponse?> GetByIdAsync(Guid id)
    {
        var war = await _repository.GetByIdAsync(id);

        if (war == null) return null;

        var firstMatch = war.WarMatches.FirstOrDefault()?.Match;
        if (firstMatch == null) return null;

        return new WarDetailResponse
        {
            Id = war.Id,
            Date = war.Date,
            TeamAName = $"{firstMatch.TeamA.Player1.Nickname} / {firstMatch.TeamA.Player2.Nickname}",
            TeamBName = $"{firstMatch.TeamB.Player1.Nickname} / {firstMatch.TeamB.Player2.Nickname}",
            Matches = war.WarMatches
                .OrderBy(wm => wm.Order)
                .Select(wm => new MatchListItemResponse
                {
                    Id = wm.Match.Id,
                    TeamAName = $"{wm.Match.TeamA.Player1.Nickname} / {wm.Match.TeamA.Player2.Nickname}",
                    TeamBName = $"{wm.Match.TeamB.Player1.Nickname} / {wm.Match.TeamB.Player2.Nickname}",
                    TeamAScore = wm.Match.TeamAScore,
                    TeamBScore = wm.Match.TeamBScore,
                    WinningTeamId = wm.Match.WinningTeamId,
                    Status = wm.Match.Status.ToString(),
                    Type = wm.Match.Type.ToString(),
                    FinishedAt = wm.Match.FinishedAt
                })
                .ToList()
        };
    }

    public async Task<List<MatchHistoryItemResponse>> GetMatchHistoryAsync(Guid teamAId, Guid teamBId, MatchType type)
    {
        var today = DateTime.UtcNow.Date;

        var matches = await _repository.GetMatchHistoryAsync(teamAId, teamBId, type, today);

        return matches.Select(m => new MatchHistoryItemResponse
        {
            Id = m.Id,
            TeamAId = m.TeamAId,
            TeamBId = m.TeamBId,
            TeamAScore = m.TeamAScore,
            TeamBScore = m.TeamBScore,
            FinishedAt = m.FinishedAt
        }).ToList();
    }

    public async Task CreateWarAsync(List<Guid> matchIds)
    {
        if (matchIds == null || !matchIds.Any())
            throw new Exception("No se enviaron partidos");

        var normalizedIds = matchIds
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var matches = await _repository.GetMatchesByIdsAsync(matchIds);

        if (matches.Count != matchIds.Count)
            throw new Exception("Algunos partidos no existen");

        if (matches.Any(m => m.Type != MatchType.Fixed))
            throw new Exception("Todos los partidos deben ser 'Fixed'");

        if (matches.Any(m => !m.FinishedAt.HasValue))
            throw new Exception("Todos los partidos deben estar finalizados");

        var first = matches.First();

        var teamAId = first.TeamAId;
        var teamBId = first.TeamBId;

        if (matches.Any(m => m.SeasonId != first.SeasonId))
            throw new Exception("Todos los partidos deben pertenecer a la misma temporada");

        // Validar consistencia de equipos (incluyendo invertidos)
        var invalid = matches.Any(m =>
            !(m.TeamAId == teamAId && m.TeamBId == teamBId) &&
            !(m.TeamAId == teamBId && m.TeamBId == teamAId)
        );

        if (invalid)
            throw new Exception("Los partidos no pertenecen a la misma guerra");

        var existingWars = await _repository.GetWarMatchIdsAsync();

        var alreadyExists = existingWars.Any(existing =>
            existing.SequenceEqual(normalizedIds)
        );

        if (alreadyExists)
            throw new DuplicateWarException("Ya existe una guerra con esos mismos partidos");

        // Ordenar
        matches = matches
            .OrderBy(m => m.FinishedAt)
            .ToList();

        // Calcular ganador
        int winsA = matches.Count(m => m.WinningTeamId == teamAId);
        int winsB = matches.Count(m => m.WinningTeamId == teamBId);

        Guid? winningTeamId = null;

        if (winsA > winsB) winningTeamId = teamAId;
        else if (winsB > winsA) winningTeamId = teamBId;

        var war = new War
        {
            Id = Guid.NewGuid(),
            Date = matches.First().FinishedAt!.Value.Date,
            TeamAId = teamAId,
            TeamBId = teamBId,
            WinningTeamId = winningTeamId,
            SeasonId = first.SeasonId
        };

        await _repository.AddAsync(war);

        int order = 0;

        var warMatches = matches.Select(m => new WarMatch
        {
            Id = Guid.NewGuid(),
            WarId = war.Id,
            MatchId = m.Id,
            Order = order++
        }).ToList();

        await _repository.AddWarMatchesAsync(warMatches);
        await _repository.SaveChangesAsync();
    }
}