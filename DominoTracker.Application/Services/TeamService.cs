using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Services;

public class TeamService
{
    private readonly ITeamRepository _teamRepository;

    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    private (Guid, Guid) NormalizePlayers(Guid player1Id, Guid player2Id)
    {
        return player1Id.CompareTo(player2Id) < 0
            ? (player1Id, player2Id)
            : (player2Id, player1Id);
    }

    public async Task<Team> GetOrCreateTeamAsync(Guid player1Id, Guid player2Id)
    {
        if (player1Id == player2Id)
            throw new Exception("A team must have two different players");

        var (p1, p2) = NormalizePlayers(player1Id, player2Id);

        var existing = await _teamRepository.GetByPlayersAsync(p1, p2);

        if (existing != null)
            return existing;

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Player1Id = p1,
            Player2Id = p2
        };

        await _teamRepository.AddAsync(team);
        await _teamRepository.SaveChangesAsync();

        return team;
    }

}