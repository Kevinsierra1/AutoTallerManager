namespace AutoTallerManager.Application.Features.Dashboard.DTOs;

public record DashboardResumenDto(
    int TotalClientes,
    int TotalVehiculos,
    int OrdenesActivas,
    int OrdenesFinalizadas,
    int RepuestosCriticos,
    decimal FacturacionMensual
);
