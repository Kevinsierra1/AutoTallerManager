namespace AutoTallerManager.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Acceso denegado.") { }
    public ForbiddenAccessException(string message) : base(message) { }
}
