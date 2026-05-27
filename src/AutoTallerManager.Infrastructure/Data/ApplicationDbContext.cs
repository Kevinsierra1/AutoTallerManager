using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<ModeloVehiculo> ModelosVehiculo => Set<ModeloVehiculo>();
    public DbSet<Color> Colores => Set<Color>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<VehiculoPropietario> VehiculoPropietarios => Set<VehiculoPropietario>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<TipoServicio> TiposServicio => Set<TipoServicio>();
    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<HistorialEstadoOrden> HistorialEstadosOrden => Set<HistorialEstadoOrden>();
    public DbSet<AprobacionOrden> AprobacionesOrden => Set<AprobacionOrden>();
    public DbSet<CategoriaRepuesto> CategoriasRepuesto => Set<CategoriaRepuesto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<ProveedorRepuesto> ProveedorRepuestos => Set<ProveedorRepuesto>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<DetalleOrdenServicio> DetallesOrdenServicio => Set<DetalleOrdenServicio>();
    public DbSet<ManoObra> ManosObra => Set<ManoObra>();
    public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<LogError> LogsErrores => Set<LogError>();

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Auto-update audit fields
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

        // Global soft delete filter
        modelBuilder.Entity<Cliente>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Vehiculo>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Empleado>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Cita>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<OrdenServicio>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Repuesto>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Factura>().HasQueryFilter(e => !e.Eliminado);
        modelBuilder.Entity<Usuario>().HasQueryFilter(e => !e.Eliminado);

        // UsuarioRol composite key
        modelBuilder.Entity<UsuarioRol>().HasKey(ur => new { ur.UsuarioId, ur.RolId });

        // ProveedorRepuesto composite key support
        modelBuilder.Entity<ProveedorRepuesto>().HasKey(pr => pr.Id);
    }
}
