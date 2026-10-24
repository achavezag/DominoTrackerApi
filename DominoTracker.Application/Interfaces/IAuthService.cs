using DominoTracker.Application.DTOs;

namespace DominoTracker.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
