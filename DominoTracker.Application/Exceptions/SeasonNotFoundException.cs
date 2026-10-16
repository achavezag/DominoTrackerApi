namespace DominoTracker.Application.Exceptions;

public class SeasonNotFoundException : Exception
{
    public SeasonNotFoundException(int id)
        : base($"No existe una temporada con id {id}") { }

    public SeasonNotFoundException(DateTime date)
        : base($"No hay una temporada definida para la fecha {date:yyyy-MM-dd}") { }
}
