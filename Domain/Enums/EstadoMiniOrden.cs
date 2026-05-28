namespace Domain.Enums;

public enum EstadoMiniOrden
{
    Borrador = 0,
    EnRevisionJefe = 1,
    AprobadaJefe = 2,
    EnRevisionCliente = 3,
    AprobadaCliente = 4,
    EnProceso = 5,
    Completada = 6,
    RechazadaJefe = 7,
    RechazadaCliente = 8,
    Cancelada = 9
}
