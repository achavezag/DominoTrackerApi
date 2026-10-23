using DominoTracker.Application.DTOs;

namespace DominoTracker.Application.Interfaces;

public interface ISessionService
{
    Task<List<SessionDto>> GetAllAsync();
    Task<SessionDto> CreateAsync(CreateSessionRequest request);
}
