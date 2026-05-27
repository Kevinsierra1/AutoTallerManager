namespace Application.UseCase.Dashboard;

public record DashboardResumenDto(
    int TotalClientes,
    int TotalVehiculos,
    int OrdenesActivas,
    int OrdenesFinalizadas,
    int RepuestosCriticos,
    decimal FacturacionMensual
);
