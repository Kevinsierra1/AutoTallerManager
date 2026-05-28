using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class TipoTransmisionConfiguration : IEntityTypeConfiguration<TipoTransmision>
{
    public void Configure(EntityTypeBuilder<TipoTransmision> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(50);

        builder.HasMany(t => t.Vehiculos)
               .WithOne(v => v.TipoTransmision)
               .HasForeignKey(v => v.TipoTransmisionId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(
            new TipoTransmision { Id = Guid.Parse("77777777-0001-0001-0001-000000000001"), Nombre = "Manual",         Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoTransmision { Id = Guid.Parse("77777777-0001-0001-0001-000000000002"), Nombre = "Automática",     Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoTransmision { Id = Guid.Parse("77777777-0001-0001-0001-000000000003"), Nombre = "Semiautomática", Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoTransmision { Id = Guid.Parse("77777777-0001-0001-0001-000000000004"), Nombre = "CVT",            Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
