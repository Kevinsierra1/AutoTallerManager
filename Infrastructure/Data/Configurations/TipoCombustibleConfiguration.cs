using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class TipoCombustibleConfiguration : IEntityTypeConfiguration<TipoCombustible>
{
    public void Configure(EntityTypeBuilder<TipoCombustible> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(50);

        builder.HasMany(t => t.Vehiculos)
               .WithOne(v => v.TipoCombustible)
               .HasForeignKey(v => v.TipoCombustibleId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(
            new TipoCombustible { Id = Guid.Parse("66666666-0001-0001-0001-000000000001"), Nombre = "Gasolina",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoCombustible { Id = Guid.Parse("66666666-0001-0001-0001-000000000002"), Nombre = "Diesel",      Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoCombustible { Id = Guid.Parse("66666666-0001-0001-0001-000000000003"), Nombre = "Híbrido",     Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoCombustible { Id = Guid.Parse("66666666-0001-0001-0001-000000000004"), Nombre = "Eléctrico",   Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new TipoCombustible { Id = Guid.Parse("66666666-0001-0001-0001-000000000005"), Nombre = "Gas Natural", Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
