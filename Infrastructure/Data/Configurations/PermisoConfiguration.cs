using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Clave).IsRequired().HasMaxLength(100);
        builder.HasIndex(p => p.Clave).IsUnique();
        builder.Property(p => p.Modulo).HasMaxLength(50);
        builder.Property(p => p.Descripcion).HasMaxLength(500);

        builder.HasData(
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000001"), Nombre = "Ver Clientes",     Clave = "clientes:ver",     Modulo = "Clientes",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000002"), Nombre = "Crear Clientes",   Clave = "clientes:crear",   Modulo = "Clientes",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000003"), Nombre = "Ver Vehículos",    Clave = "vehiculos:ver",    Modulo = "Vehículos",  Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000004"), Nombre = "Ver Órdenes",      Clave = "ordenes:ver",      Modulo = "Órdenes",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000005"), Nombre = "Ver Inventario",   Clave = "inventario:ver",   Modulo = "Inventario", Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000006"), Nombre = "Ver Facturas",     Clave = "facturas:ver",     Modulo = "Facturación",Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new Permiso { Id = Guid.Parse("88888888-0001-0001-0001-000000000007"), Nombre = "Administrar Todo", Clave = "sistema:admin",    Modulo = "Sistema",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
