namespace AutoTallerManager.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"La entidad '{name}' con id '{key}' no fue encontrada.") { }
    
    public NotFoundException(string message) : base(message) { }
}
