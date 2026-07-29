namespace DominoTracker.Application.Exceptions;

public class InvalidWarMatchesException : Exception
{
    public InvalidWarMatchesException()
        : base("Los matches no pertenecen a la misma guerra") { }
}