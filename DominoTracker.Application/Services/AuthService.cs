using DominoTracker.Application.Constants;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Exceptions;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username)
            ?? throw new InvalidCredentialsException();

        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new InvalidCredentialsException();

        var role = await _userRepository.GetRoleByIdAsync(user.RoleId)
            ?? throw new InvalidCredentialsException();

        if (role.Name != request.Role)
            throw new InvalidCredentialsException();

        string? sessionName = null;

        if (role.Name != RoleNames.SuperAdmin)
        {
            if (string.IsNullOrWhiteSpace(request.SessionName))
                throw new InvalidCredentialsException();

            var sessions = await _sessionRepository.GetAllAsync();
            var session = sessions.FirstOrDefault(s => s.Name == request.SessionName);

            if (session == null || session.Id != user.SessionId)
                throw new InvalidCredentialsException();

            sessionName = session.Name;
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, role.Name, user.SessionId);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = user.Username,
            Role = role.Name,
            SessionId = user.SessionId,
            SessionName = sessionName
        };
    }
}
