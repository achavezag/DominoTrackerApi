using DominoTracker.Application.Constants;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Exceptions;
using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, ISessionRepository sessionRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var roles = await _userRepository.GetAllRolesAsync();
        var sessions = await _sessionRepository.GetAllAsync();

        return users.Select(u => ToDto(u, roles, sessions)).ToList();
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new UserNotFoundException(id);

        var roles = await _userRepository.GetAllRolesAsync();
        var sessions = await _sessionRepository.GetAllAsync();

        return ToDto(user, roles, sessions);
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _userRepository.GetAllRolesAsync();

        return roles
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new DuplicateUsernameException(request.Username);

        var role = await _userRepository.GetRoleByIdAsync(request.RoleId)
            ?? throw new Exception($"No existe un rol con id {request.RoleId}");

        await ValidateSessionForRoleAsync(role, request.SessionId);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = _passwordHasher.Hash(request.Password),
            RoleId = role.Id,
            SessionId = role.Name == RoleNames.SuperAdmin ? null : request.SessionId
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return await GetByIdAsync(user.Id);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new UserNotFoundException(id);

        if (!string.Equals(user.Username, request.Username, StringComparison.Ordinal)
            && await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new DuplicateUsernameException(request.Username);

        var role = await _userRepository.GetRoleByIdAsync(request.RoleId)
            ?? throw new Exception($"No existe un rol con id {request.RoleId}");

        await ValidateSessionForRoleAsync(role, request.SessionId);

        user.Username = request.Username;
        user.RoleId = role.Id;
        user.SessionId = role.Name == RoleNames.SuperAdmin ? null : request.SessionId;

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = _passwordHasher.Hash(request.Password);

        await _userRepository.SaveChangesAsync();

        return await GetByIdAsync(user.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new UserNotFoundException(id);

        await _userRepository.RemoveAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    private async Task ValidateSessionForRoleAsync(Role role, int? sessionId)
    {
        if (role.Name == RoleNames.SuperAdmin)
            return;

        if (sessionId == null)
            throw new Exception("Los usuarios con rol distinto de 'super_admin' deben tener una sesión asignada");

        _ = await _sessionRepository.GetByIdAsync(sessionId.Value)
            ?? throw new Exception($"No existe una sesión con id {sessionId.Value}");
    }

    private static UserDto ToDto(User user, List<Role> roles, List<Session> sessions)
    {
        var role = roles.First(r => r.Id == user.RoleId);
        var session = user.SessionId.HasValue
            ? sessions.FirstOrDefault(s => s.Id == user.SessionId.Value)
            : null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            RoleId = role.Id,
            RoleName = role.Name,
            SessionId = user.SessionId,
            SessionName = session?.Name
        };
    }
}
