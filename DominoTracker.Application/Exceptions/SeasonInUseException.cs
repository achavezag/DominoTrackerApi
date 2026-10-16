namespace DominoTracker.Application.Exceptions;

public class SeasonInUseException : Exception
{
    public SeasonInUseException()
        : base("No se puede eliminar la temporada porque tiene partidos o guerras asociadas") { }
}
