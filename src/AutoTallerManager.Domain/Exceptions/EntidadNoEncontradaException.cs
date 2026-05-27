namespace AutoTallerManager.Domain.Exceptions;

public class EntidadNoEncontradaException : DomainException
{
    public EntidadNoEncontradaException(string entidad, object id)
        : base($"La entidad '{entidad}' con id '{id}' no fue encontrada.") { }
}
