using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class EstadoFacturaConfiguration : IEntityTypeConfiguration<EstadoFactura>
{
    public void Configure(EntityTypeBuilder<EstadoFactura> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Descripcion).HasMaxLength(200);

        builder.HasData(
            new EstadoFactura { Id = Guid.Parse("44444444-0001-0001-0001-000000000001"), Nombre = "Pendiente", Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoFactura { Id = Guid.Parse("44444444-0001-0001-0001-000000000002"), Nombre = "Parcial",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoFactura { Id = Guid.Parse("44444444-0001-0001-0001-000000000003"), Nombre = "Pagada",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoFactura { Id = Guid.Parse("44444444-0001-0001-0001-000000000004"), Nombre = "Anulada",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoFactura { Id = Guid.Parse("44444444-0001-0001-0001-000000000005"), Nombre = "Vencida",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
