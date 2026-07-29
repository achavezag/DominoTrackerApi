namespace DominoTracker.Application.Exceptions;

public class WarAlreadyExistsException : Exception
{
    public WarAlreadyExistsException()
        : base("Ya existe una guerra hoy") { }
}