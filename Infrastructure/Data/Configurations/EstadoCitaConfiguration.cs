using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class EstadoCitaConfiguration : IEntityTypeConfiguration<EstadoCita>
{
    public void Configure(EntityTypeBuilder<EstadoCita> builder)
    {
        builder.ToTable("EstadosCita");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Descripcion).HasMaxLength(200);

        builder.HasData(
            new EstadoCita { Id = Guid.Parse("33333333-0001-0001-0001-000000000001"), Nombre = "Pendiente",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoCita { Id = Guid.Parse("33333333-0001-0001-0001-000000000002"), Nombre = "Confirmada",  Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoCita { Id = Guid.Parse("33333333-0001-0001-0001-000000000003"), Nombre = "Completada",  Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new EstadoCita { Id = Guid.Parse("33333333-0001-0001-0001-000000000004"), Nombre = "Cancelada",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
