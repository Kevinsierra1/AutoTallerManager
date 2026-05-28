using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class EstadoOrdenConfiguration : IEntityTypeConfiguration<EstadoOrden>
{
    public void Configure(EntityTypeBuilder<EstadoOrden> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Descripcion).HasMaxLength(200);
        builder.HasIndex(e => e.Codigo).IsUnique();

        builder.HasData(
            new EstadoOrden { Id = Guid.Parse("11111111-0001-0001-0001-000000000001"), Nombre = "Pendiente",   Codigo = 0, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoOrden { Id = Guid.Parse("11111111-0001-0001-0001-000000000002"), Nombre = "Aprobada",    Codigo = 1, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoOrden { Id = Guid.Parse("11111111-0001-0001-0001-000000000003"), Nombre = "En Proceso",  Codigo = 2, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoOrden { Id = Guid.Parse("11111111-0001-0001-0001-000000000004"), Nombre = "Finalizada",  Codigo = 3, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoOrden { Id = Guid.Parse("11111111-0001-0001-0001-000000000005"), Nombre = "Cancelada",   Codigo = 4, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
