using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Rol> Roles { get; }
    DbSet<UsuarioRol> UsuarioRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<TipoDocumento> TiposDocumento { get; }
    DbSet<Cliente> Clientes { get; }
    DbSet<Marca> Marcas { get; }
    DbSet<ModeloVehiculo> ModelosVehiculo { get; }
    DbSet<Color> Colores { get; }
    DbSet<Vehiculo> Vehiculos { get; }
    DbSet<VehiculoPropietario> VehiculoPropietarios { get; }
    DbSet<Empleado> Empleados { get; }
    DbSet<Cita> Citas { get; }
    DbSet<TipoServicio> TiposServicio { get; }
    DbSet<OrdenServicio> OrdenesServicio { get; }
    DbSet<HistorialEstadoOrden> HistorialEstadosOrden { get; }
    DbSet<AprobacionOrden> AprobacionesOrden { get; }
    DbSet<CategoriaRepuesto> CategoriasRepuesto { get; }
    DbSet<Proveedor> Proveedores { get; }
    DbSet<Repuesto> Repuestos { get; }
    DbSet<ProveedorRepuesto> ProveedorRepuestos { get; }
    DbSet<MovimientoInventario> MovimientosInventario { get; }
    DbSet<DetalleOrdenServicio> DetallesOrdenServicio { get; }
    DbSet<ManoObra> ManosObra { get; }
    DbSet<MetodoPago> MetodosPago { get; }
    DbSet<Factura> Facturas { get; }
    DbSet<Pago> Pagos { get; }
    DbSet<Auditoria> Auditorias { get; }
    DbSet<LogError> LogsErrores { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
