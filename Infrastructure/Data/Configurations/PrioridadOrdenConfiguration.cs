using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class PrioridadOrdenConfiguration : IEntityTypeConfiguration<PrioridadOrden>
{
    public void Configure(EntityTypeBuilder<PrioridadOrden> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Descripcion).HasMaxLength(200);
        builder.HasIndex(p => p.Nivel).IsUnique();

        builder.HasMany(p => p.OrdenesServicio)
               .WithOne(o => o.PrioridadOrden)
               .HasForeignKey(o => o.PrioridadOrdenId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(
            new PrioridadOrden { Id = Guid.Parse("55555555-0001-0001-0001-000000000001"), Nombre = "Alta",   Nivel = 1, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new PrioridadOrden { Id = Guid.Parse("55555555-0001-0001-0001-000000000002"), Nombre = "Media",  Nivel = 2, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new PrioridadOrden { Id = Guid.Parse("55555555-0001-0001-0001-000000000003"), Nombre = "Baja",   Nivel = 3, Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
