namespace DominoTracker.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string username, string roleName, int? sessionId);
}
