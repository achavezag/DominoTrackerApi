namespace DominoTracker.Application.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid id)
        : base($"No existe un usuario con id {id}") { }
}
