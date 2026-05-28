using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

public interface IApplicationDbContext
{
    // ── Catálogos existentes ────────────────────────────────────────────────────
    DbSet<TipoDocumento> TiposDocumento { get; }
    DbSet<Marca> Marcas { get; }
    DbSet<ModeloVehiculo> ModelosVehiculo { get; }
    DbSet<Color> Colores { get; }
    DbSet<TipoServicio> TiposServicio { get; }
    DbSet<CategoriaRepuesto> CategoriasRepuesto { get; }
    DbSet<MetodoPago> MetodosPago { get; }

    // ── Catálogos nuevos ────────────────────────────────────────────────────────
    DbSet<EstadoOrden> EstadosOrden { get; }
    DbSet<TipoMovInventario> TiposMovimientoInventario { get; }
    DbSet<EstadoCita> EstadosCita { get; }
    DbSet<EstadoFactura> EstadosFactura { get; }
    DbSet<PrioridadOrden> PrioridadesOrden { get; }
    DbSet<TipoCombustible> TiposCombustible { get; }
    DbSet<TipoTransmision> TiposTransmision { get; }

    // ── Seguridad existente ─────────────────────────────────────────────────────
    DbSet<Usuario> Usuarios { get; }
    DbSet<Rol> Roles { get; }
    DbSet<UsuarioRol> UsuarioRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Empleado> Empleados { get; }

    // ── Seguridad nueva ─────────────────────────────────────────────────────────
    DbSet<Permiso> Permisos { get; }
    DbSet<RolPermiso> RolPermisos { get; }
    DbSet<SesionUsuario> SesionesUsuarios { get; }
    DbSet<HistorialAcceso> HistorialAccesos { get; }

    // ── Clientes ────────────────────────────────────────────────────────────────
    DbSet<Cliente> Clientes { get; }
    DbSet<DireccionCliente> DireccionesClientes { get; }
    DbSet<TelefonoCliente> TelefonosClientes { get; }
    DbSet<CorreoCliente> CorreosClientes { get; }

    // ── Vehículos ───────────────────────────────────────────────────────────────
    DbSet<Vehiculo> Vehiculos { get; }
    DbSet<VehiculoPropietario> VehiculoPropietarios { get; }
    DbSet<VehiculoKilometraje> VehiculoKilometrajes { get; }
    DbSet<VehiculoFoto> VehiculoFotos { get; }
    DbSet<VehiculoMantenimiento> VehiculoMantenimientos { get; }
    DbSet<VehiculoDocumento> VehiculoDocumentos { get; }

    // ── Agenda ──────────────────────────────────────────────────────────────────
    DbSet<Cita> Citas { get; }
    DbSet<HistorialCita> HistorialCitas { get; }

    // ── Inventario ──────────────────────────────────────────────────────────────
    DbSet<Repuesto> Repuestos { get; }
    DbSet<Proveedor> Proveedores { get; }
    DbSet<ProveedorRepuesto> ProveedorRepuestos { get; }
    DbSet<MovimientoInventario> MovimientosInventario { get; }
    DbSet<HistorialPrecioRepuesto> HistorialPreciosRepuestos { get; }
    DbSet<EntradaInventario> EntradasInventario { get; }
    DbSet<SalidaInventario> SalidasInventario { get; }

    // ── Órdenes de servicio ─────────────────────────────────────────────────────
    DbSet<OrdenServicio> OrdenesServicio { get; }
    DbSet<HistorialEstadoOrden> HistorialEstadosOrden { get; }
    DbSet<AprobacionOrden> AprobacionesOrden { get; }
    DbSet<DetalleOrdenServicio> DetallesOrdenServicio { get; }
    DbSet<ManoObra> ManosObra { get; }
    DbSet<OrdenServicioExtra> OrdenServiciosExtras { get; }

    // ── Facturación ─────────────────────────────────────────────────────────────
    DbSet<Factura> Facturas { get; }
    DbSet<DetalleFactura> DetalleFacturas { get; }
    DbSet<Pago> Pagos { get; }
    DbSet<ImpuestoFactura> ImpuestosFacturas { get; }

    // ── Auditoría y configuración ───────────────────────────────────────────────
    DbSet<Auditoria> Auditorias { get; }
    DbSet<LogError> LogsErrores { get; }
    DbSet<Notificacion> Notificaciones { get; }
    DbSet<ConfiguracionSistema> ConfiguracionesSistema { get; }

    // ── Gestión por áreas ───────────────────────────────────────────────────────
    DbSet<AreaTaller> AreasTaller { get; }
    DbSet<OrdenArea> OrdenAreas { get; }
    DbSet<OrdenAreaDetalle> OrdenAreaDetalles { get; }
    DbSet<OrdenAreaManoObra> OrdenAreaManosObra { get; }

    // ── Mini-órdenes (Flujo M-J-C) ─────────────────────────────────────────────
    DbSet<MiniOrden> MiniOrdenes { get; }
    DbSet<MiniOrdenDetalle> MiniOrdenDetalles { get; }
    DbSet<MiniOrdenManoObra> MiniOrdenManosObra { get; }
    DbSet<MiniOrdenHistorial> MiniOrdenHistoriales { get; }
    DbSet<MiniOrdenAprobacion> MiniOrdenAprobaciones { get; }

    // ── Inventario empresarial ──────────────────────────────────────────────────
    DbSet<SolicitudInventario> SolicitudesInventario { get; }
    DbSet<SolicitudInventarioDetalle> SolicitudInventarioDetalles { get; }
    DbSet<TransferenciaInventario> TransferenciasInventario { get; }
    DbSet<TransferenciaInventarioDetalle> TransferenciaInventarioDetalles { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
