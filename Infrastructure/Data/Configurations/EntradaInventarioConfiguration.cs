using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class EntradaInventarioConfiguration : IEntityTypeConfiguration<EntradaInventario>
{
    public void Configure(EntityTypeBuilder<EntradaInventario> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PrecioUnitario).HasPrecision(18, 2);
        builder.Property(e => e.Total).HasPrecision(18, 2);
        builder.Property(e => e.NumeroFacturaProveedor).HasMaxLength(50);
        builder.Property(e => e.Observaciones).HasMaxLength(500);

        builder.HasOne(e => e.Repuesto)
               .WithMany(r => r.Entradas)
               .HasForeignKey(e => e.RepuestoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Proveedor)
               .WithMany()
               .HasForeignKey(e => e.ProveedorId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.MovimientoInventario)
               .WithMany()
               .HasForeignKey(e => e.MovimientoInventarioId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
