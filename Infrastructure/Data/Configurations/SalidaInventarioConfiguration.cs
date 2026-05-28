using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class SalidaInventarioConfiguration : IEntityTypeConfiguration<SalidaInventario>
{
    public void Configure(EntityTypeBuilder<SalidaInventario> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Motivo).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Observaciones).HasMaxLength(500);

        builder.HasOne(s => s.Repuesto)
               .WithMany(r => r.Salidas)
               .HasForeignKey(s => s.RepuestoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.OrdenServicio)
               .WithMany()
               .HasForeignKey(s => s.OrdenServicioId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.MovimientoInventario)
               .WithMany()
               .HasForeignKey(s => s.MovimientoInventarioId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
