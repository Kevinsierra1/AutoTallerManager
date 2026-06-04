using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Domain.Entities;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ── Catálogos existentes ────────────────────────────────────────────────────
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<ModeloVehiculo> ModelosVehiculo => Set<ModeloVehiculo>();
    public DbSet<Color> Colores => Set<Color>();
    public DbSet<TipoServicio> TiposServicio => Set<TipoServicio>();
    public DbSet<CategoriaRepuesto> CategoriasRepuesto => Set<CategoriaRepuesto>();
    public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();

    // ── Catálogos nuevos ────────────────────────────────────────────────────────
    public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();
    public DbSet<TipoMovInventario> TiposMovimientoInventario => Set<TipoMovInventario>();
    public DbSet<EstadoCita> EstadosCita => Set<EstadoCita>();
    public DbSet<EstadoFactura> EstadosFactura => Set<EstadoFactura>();
    public DbSet<PrioridadOrden> PrioridadesOrden => Set<PrioridadOrden>();
    public DbSet<TipoCombustible> TiposCombustible => Set<TipoCombustible>();
    public DbSet<TipoTransmision> TiposTransmision => Set<TipoTransmision>();

    // ── Seguridad existente ─────────────────────────────────────────────────────
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Empleado> Empleados => Set<Empleado>();

    // ── Seguridad nueva ─────────────────────────────────────────────────────────
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
    public DbSet<SesionUsuario> SesionesUsuarios => Set<SesionUsuario>();
    public DbSet<HistorialAcceso> HistorialAccesos => Set<HistorialAcceso>();

    // ── Clientes ────────────────────────────────────────────────────────────────
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<DireccionCliente> DireccionesClientes => Set<DireccionCliente>();
    public DbSet<TelefonoCliente> TelefonosClientes => Set<TelefonoCliente>();
    public DbSet<CorreoCliente> CorreosClientes => Set<CorreoCliente>();

    // ── Vehículos ───────────────────────────────────────────────────────────────
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<VehiculoPropietario> VehiculoPropietarios => Set<VehiculoPropietario>();
    public DbSet<VehiculoKilometraje> VehiculoKilometrajes => Set<VehiculoKilometraje>();
    public DbSet<VehiculoFoto> VehiculoFotos => Set<VehiculoFoto>();
    public DbSet<VehiculoMantenimiento> VehiculoMantenimientos => Set<VehiculoMantenimiento>();
    public DbSet<VehiculoDocumento> VehiculoDocumentos => Set<VehiculoDocumento>();

    // ── Agenda ──────────────────────────────────────────────────────────────────
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<HistorialCita> HistorialCitas => Set<HistorialCita>();

    // ── Inventario ──────────────────────────────────────────────────────────────
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<ProveedorRepuesto> ProveedorRepuestos => Set<ProveedorRepuesto>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<HistorialPrecioRepuesto> HistorialPreciosRepuestos => Set<HistorialPrecioRepuesto>();
    public DbSet<EntradaInventario> EntradasInventario => Set<EntradaInventario>();
    public DbSet<SalidaInventario> SalidasInventario => Set<SalidaInventario>();

    // ── Órdenes de servicio ─────────────────────────────────────────────────────
    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<HistorialEstadoOrden> HistorialEstadosOrden => Set<HistorialEstadoOrden>();
    public DbSet<AprobacionOrden> AprobacionesOrden => Set<AprobacionOrden>();
    public DbSet<DetalleOrdenServicio> DetallesOrdenServicio => Set<DetalleOrdenServicio>();
    public DbSet<ManoObra> ManosObra => Set<ManoObra>();
    public DbSet<OrdenServicioExtra> OrdenServiciosExtras => Set<OrdenServicioExtra>();

    // ── Facturación ─────────────────────────────────────────────────────────────
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<DetalleFactura> DetalleFacturas => Set<DetalleFactura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<SolicitudPago> SolicitudesPago => Set<SolicitudPago>();
    public DbSet<ImpuestoFactura> ImpuestosFacturas => Set<ImpuestoFactura>();

    // ── Auditoría y configuración ───────────────────────────────────────────────
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<LogError> LogsErrores => Set<LogError>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<ConfiguracionSistema> ConfiguracionesSistema => Set<ConfiguracionSistema>();

    // ── Gestión por áreas ───────────────────────────────────────────────────────
    public DbSet<AreaTaller> AreasTaller => Set<AreaTaller>();
    public DbSet<OrdenArea> OrdenAreas => Set<OrdenArea>();
    public DbSet<OrdenAreaDetalle> OrdenAreaDetalles => Set<OrdenAreaDetalle>();
    public DbSet<OrdenAreaManoObra> OrdenAreaManosObra => Set<OrdenAreaManoObra>();

    // ── Mini-órdenes (Flujo M-J-C) ─────────────────────────────────────────────
    public DbSet<MiniOrden> MiniOrdenes => Set<MiniOrden>();
    public DbSet<MiniOrdenDetalle> MiniOrdenDetalles => Set<MiniOrdenDetalle>();
    public DbSet<MiniOrdenManoObra> MiniOrdenManosObra => Set<MiniOrdenManoObra>();
    public DbSet<MiniOrdenHistorial> MiniOrdenHistoriales => Set<MiniOrdenHistorial>();
    public DbSet<MiniOrdenAprobacion> MiniOrdenAprobaciones => Set<MiniOrdenAprobacion>();

    // ── Inventario empresarial ──────────────────────────────────────────────────
    public DbSet<SolicitudInventario> SolicitudesInventario => Set<SolicitudInventario>();
    public DbSet<SolicitudInventarioDetalle> SolicitudInventarioDetalles => Set<SolicitudInventarioDetalle>();
    public DbSet<TransferenciaInventario> TransferenciasInventario => Set<TransferenciaInventario>();
    public DbSet<TransferenciaInventarioDetalle> TransferenciaInventarioDetalles => Set<TransferenciaInventarioDetalle>();

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Entities.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.ActualizadoEn = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // ── Global soft delete filters ──────────────────────────────────────────
        modelBuilder.Entity<Cliente>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Vehiculo>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Empleado>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Cita>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<OrdenServicio>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Repuesto>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Factura>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Usuario>().HasQueryFilter(e => !e.Eliminado);

        // ── Composite keys ──────────────────────────────────────────────────────
        modelBuilder.Entity<UsuarioRol>().HasKey(ur => new { ur.UsuarioId, ur.RolId });
        modelBuilder.Entity<RolPermiso>().HasKey(rp => new { rp.RolId, rp.PermisoId });

        // ── Regular keys ────────────────────────────────────────────────────────
        modelBuilder.Entity<ProveedorRepuesto>().HasKey(pr => pr.Id);

        // ── Soft delete filters para nuevas entidades ──────────────────────────
        modelBuilder.Entity<MiniOrden>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<SolicitudInventario>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<TransferenciaInventario>().HasQueryFilter(e => !e.Eliminado);
    }
}
