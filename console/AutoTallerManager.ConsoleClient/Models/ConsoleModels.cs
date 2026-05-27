namespace AutoTallerManager.ConsoleClient.Models;

public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string RefreshToken, DateTime Expiration, string[] Roles);
public record DashboardResumen(int TotalClientes, int TotalVehiculos, int OrdenesActivas, int OrdenesFinalizadas, int RepuestosCriticos, decimal FacturacionMensual);
public record ClienteResumen(Guid Id, string Nombres, string Apellidos, string NumeroDocumento, string? Telefono);
public record VehiculoResumen(Guid Id, string Placa, string? Vin, string? Marca, string? Modelo, string? Color);
public record OrdenResumen(Guid Id, string NumeroOrden, string Estado, string? ClienteNombre, string? VehiculoPlaca, DateTime FechaIngreso);
public record RepuestoResumen(Guid Id, string Codigo, string Nombre, int StockActual, int StockMinimo, decimal PrecioVenta);
public record FacturaResumen(Guid Id, string NumeroFactura, string? ClienteNombre, decimal Total, DateTime FechaEmision);
public record PagedResponse<T>(List<T> Items, int TotalCount, int PageNumber, int PageSize);
