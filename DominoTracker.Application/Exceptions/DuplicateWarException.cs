namespace DominoTracker.Application.Exceptions;

public class DuplicateWarException : Exception
{
    public DuplicateWarException(string message) : base(message) { }
}