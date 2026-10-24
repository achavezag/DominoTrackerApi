namespace DominoTracker.Application.Exceptions;

public class DuplicateUsernameException : Exception
{
    public DuplicateUsernameException(string username)
        : base($"Ya existe un usuario con username '{username}'") { }
}
