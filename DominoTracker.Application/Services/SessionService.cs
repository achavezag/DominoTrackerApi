using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;

    public SessionService(ISessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SessionDto>> GetAllAsync()
    {
        var sessions = await _repository.GetAllAsync();

        return sessions.Select(ToDto).ToList();
    }

    public async Task<SessionDto> CreateAsync(CreateSessionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("El nombre de la sesión es obligatorio");

        var session = new Session
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(session);
        await _repository.SaveChangesAsync();

        return ToDto(session);
    }

    private static SessionDto ToDto(Session session) => new()
    {
        Id = session.Id,
        Name = session.Name,
        CreatedAt = session.CreatedAt
    };
}
