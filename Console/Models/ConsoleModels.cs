using System.Text.Json.Serialization;

namespace AutoTaller.Console.Models;

// ─── API wrappers ────────────────────────────────────────────────────────────

public class ApiResult<T>
{
    public bool Exito { get; set; }
    public string? Mensaje { get; set; }
    public T? Data { get; set; }
    public List<string>? Errores { get; set; }
}

public class PagedData<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

// ─── Auth ────────────────────────────────────────────────────────────────────

public class AuthResponse
{
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime Expiration { get; set; }
    public string[] Roles { get; set; } = [];
    public Guid UserId { get; set; }
    public string Email { get; set; } = "";
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";

    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    public string RolesStr => string.Join(", ", Roles);
}

// ─── Clientes ────────────────────────────────────────────────────────────────

public class ClienteModel
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string TipoDocumento { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime CreadoEn { get; set; }

    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
}

// ─── Vehículos ───────────────────────────────────────────────────────────────

public class VehiculoModel
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = "";
    public string? Vin { get; set; }
    public Guid ModeloVehiculoId { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public Guid? ColorId { get; set; }
    public string? Color { get; set; }
    public int Anio { get; set; }
    public string? NumeroMotor { get; set; }
    public string? NumeroChasis { get; set; }
    public int KilometrajeActual { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }

    public string MarcaModelo => $"{Marca} {Modelo}".Trim();
}

// ─── Catálogos ───────────────────────────────────────────────────────────────

public class CatalogoItem
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
}

public class ModeloItem
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public Guid MarcaId { get; set; }
    public string MarcaNombre { get; set; } = "";
    public string Display => $"{MarcaNombre} {Nombre}";
}

public class ColorItem
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? CodigoHex { get; set; }
}

// ─── Órdenes ─────────────────────────────────────────────────────────────────

public enum EstadoOrden { Pendiente = 0, Aprobada = 1, EnProceso = 2, Finalizada = 3, Cancelada = 4 }

public class OrdenModel
{
    public Guid Id { get; set; }
    public string NumeroOrden { get; set; } = "";
    public Guid ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public Guid VehiculoId { get; set; }
    public string? VehiculoPlaca { get; set; }
    public Guid? MecanicoId { get; set; }
    public string? MecanicoNombre { get; set; }
    public EstadoOrden Estado { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal? Total { get; set; }
    public DateTime CreadoEn { get; set; }

    public string EstadoTexto => Estado switch
    {
        EstadoOrden.Pendiente  => "Pendiente",
        EstadoOrden.Aprobada   => "Aprobada",
        EstadoOrden.EnProceso  => "En Proceso",
        EstadoOrden.Finalizada => "Finalizada",
        EstadoOrden.Cancelada  => "Cancelada",
        _ => Estado.ToString()
    };

    public string EstadoColor => Estado switch
    {
        EstadoOrden.Pendiente  => "yellow",
        EstadoOrden.Aprobada   => "blue",
        EstadoOrden.EnProceso  => "cyan",
        EstadoOrden.Finalizada => "green",
        EstadoOrden.Cancelada  => "red",
        _ => "white"
    };
}

// ─── Repuestos ───────────────────────────────────────────────────────────────

public class RepuestoModel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public Guid CategoriaRepuestoId { get; set; }
    public string? Categoria { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public string? Unidad { get; set; }
    public bool Activo { get; set; }

    public bool StockCritico => StockActual <= StockMinimo;
}

// ─── Inventario ──────────────────────────────────────────────────────────────

public class MovimientoModel
{
    public Guid Id { get; set; }
    public Guid RepuestoId { get; set; }
    public string? RepuestoNombre { get; set; }
    public int Tipo { get; set; }
    public int Cantidad { get; set; }
    public int CantidadAnterior { get; set; }
    public int CantidadNueva { get; set; }
    public string? Motivo { get; set; }
    public DateTime FechaMovimiento { get; set; }

    public string TipoTexto => Tipo switch { 1 => "Entrada", 2 => "Salida", 3 => "Ajuste", _ => "?" };
    public string TipoColor => Tipo switch { 1 => "green", 2 => "red", _ => "yellow" };
}

// ─── Facturas ────────────────────────────────────────────────────────────────

public class FacturaModel
{
    public Guid Id { get; set; }
    public string NumeroFactura { get; set; } = "";
    public Guid OrdenServicioId { get; set; }
    public Guid ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public bool Pagada { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime CreadoEn { get; set; }
}

// ─── Dashboard ───────────────────────────────────────────────────────────────

public class DashboardResumen
{
    public int TotalClientes { get; set; }
    public int TotalVehiculos { get; set; }
    public int OrdenesActivas { get; set; }
    public int OrdenesFinalizadas { get; set; }
    public int RepuestosCriticos { get; set; }
    public decimal FacturacionMensual { get; set; }
}

public class OrdenEstadistica
{
    public string Estado { get; set; } = "";
    public int Total { get; set; }
}

public class FacturacionMensual
{
    public string Mes { get; set; } = "";
    public decimal Total { get; set; }
}

// ─── Catalogos ───────────────────────────────────────────────────────────────

public class MarcaModel { public Guid Id { get; set; } public string Nombre { get; set; } = ""; }
public class ModeloModel { public Guid Id { get; set; } public string Nombre { get; set; } = ""; public Guid MarcaId { get; set; } }
public class ColorModel { public Guid Id { get; set; } public string Nombre { get; set; } = ""; }
public class CategoriaRepuestoModel { public Guid Id { get; set; } public string Nombre { get; set; } = ""; }
public class EmpleadoModel
{
    public Guid Id { get; set; }
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public int TipoEmpleado { get; set; }
    public string? Especialidad { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; }
}

// ─── Mini-Órdenes ─────────────────────────────────────────────────────────────

public class MiniOrdenModel
{
    public Guid Id { get; set; }
    public string NumeroMiniOrden { get; set; } = "";
    public Guid OrdenServicioId { get; set; }
    public string? NumeroOrden { get; set; }
    public Guid? OrdenAreaId { get; set; }
    public string? AreaNombre { get; set; }
    public string Descripcion { get; set; } = "";
    public int Estado { get; set; }
    public string? EstadoNombre { get; set; }
    public Guid? MecanicoId { get; set; }
    public string? MecanicoNombre { get; set; }
    public Guid? JefeTallerId { get; set; }
    public string? JefeTallerNombre { get; set; }
    public DateTime? FechaAprobacionJefe { get; set; }
    public DateTime? FechaAprobacionCliente { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal TotalMateriales { get; set; }
    public decimal TotalManoObra { get; set; }
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public DateTime CreadoEn { get; set; }

    public string EstadoColor => Estado switch
    {
        0 => "grey",
        1 => "yellow",
        2 => "blue",
        3 => "yellow",
        4 => "cyan",
        5 => "cyan",
        6 => "green",
        7 => "red",
        8 => "red",
        9 => "red",
        _ => "white"
    };
}

// ─── Usuarios / Roles ────────────────────────────────────────────────────────

public class UsuarioModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public bool Activo { get; set; }
    public List<string> Roles { get; set; } = [];
    public DateTime CreadoEn { get; set; }
    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    public string RolesStr => string.Join(", ", Roles);
}

public class RolModel
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
}

// ─── Helpers de roles ─────────────────────────────────────────────────────────

public static class RolHelper
{
    public static bool EsAdmin(this AuthResponse u) => u.Roles.Contains("Admin");
    public static bool EsJefeTaller(this AuthResponse u) => u.Roles.Contains("JefeTaller") || u.EsAdmin();
    public static bool EsMecanico(this AuthResponse u) =>
        u.Roles.Any(r => r is "Mecánico" or "MecanicoDiagnostico" or "MecanicoArea") || u.EsAdmin();
    public static bool EsRecepcionista(this AuthResponse u) => u.Roles.Contains("Recepcionista") || u.EsAdmin();
    public static bool EsCliente(this AuthResponse u) => u.Roles.Contains("Cliente");
    public static bool EsAlmacen(this AuthResponse u) =>
        u.Roles.Any(r => r is "JefeAlmacen" or "JefeBodega") || u.EsAdmin();
    public static bool PuedeVerInventario(this AuthResponse u) =>
        u.EsAlmacen() || u.EsJefeTaller() || u.EsAdmin();
    public static bool PuedeGestionarMiniOrdenes(this AuthResponse u) =>
        u.EsMecanico() || u.EsJefeTaller() || u.EsCliente() || u.EsAdmin();
}
