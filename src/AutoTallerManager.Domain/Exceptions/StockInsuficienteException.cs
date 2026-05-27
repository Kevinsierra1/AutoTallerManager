namespace AutoTallerManager.Domain.Exceptions;

public class StockInsuficienteException : DomainException
{
    public StockInsuficienteException(string repuesto, int stockActual, int cantidadSolicitada)
        : base($"Stock insuficiente para '{repuesto}'. Stock actual: {stockActual}, solicitado: {cantidadSolicitada}.") { }
}
