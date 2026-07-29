namespace DominoTracker.Application.Exceptions;

public class NoMatchesForWarException : Exception
{
    public NoMatchesForWarException()
        : base("No hay partidos 'Fixed' finalizados hoy") { }
}