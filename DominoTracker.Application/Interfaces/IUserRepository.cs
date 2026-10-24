using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<Role?> GetRoleByIdAsync(int id);
    Task<Role?> GetRoleByNameAsync(string name);
    Task<List<Role>> GetAllRolesAsync();
    Task AddAsync(User user);
    Task RemoveAsync(User user);
    Task SaveChangesAsync();
}
