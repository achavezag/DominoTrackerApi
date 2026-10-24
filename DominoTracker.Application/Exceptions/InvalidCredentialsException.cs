namespace DominoTracker.Application.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Usuario, contraseña, rol o sesión inválidos") { }
}
