using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class TipoMovInventarioConfiguration : IEntityTypeConfiguration<TipoMovInventario>
{
    public void Configure(EntityTypeBuilder<TipoMovInventario> builder)
    {
        builder.ToTable("TiposMovimientoInventario");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Descripcion).HasMaxLength(200);

        builder.HasData(
            new TipoMovInventario { Id = Guid.Parse("22222222-0001-0001-0001-000000000001"), Nombre = "Entrada",  Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoMovInventario { Id = Guid.Parse("22222222-0001-0001-0001-000000000002"), Nombre = "Salida",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoMovInventario { Id = Guid.Parse("22222222-0001-0001-0001-000000000003"), Nombre = "Ajuste",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
