using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class HistorialPrecioRepuestoConfiguration : IEntityTypeConfiguration<HistorialPrecioRepuesto>
{
    public void Configure(EntityTypeBuilder<HistorialPrecioRepuesto> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.PrecioCompraAnterior).HasPrecision(18, 2);
        builder.Property(h => h.PrecioCompraNuevo).HasPrecision(18, 2);
        builder.Property(h => h.PrecioVentaAnterior).HasPrecision(18, 2);
        builder.Property(h => h.PrecioVentaNuevo).HasPrecision(18, 2);
        builder.Property(h => h.CambiadoPor).HasMaxLength(100);
        builder.Property(h => h.Motivo).HasMaxLength(500);

        builder.HasOne(h => h.Repuesto)
               .WithMany(r => r.HistorialPrecios)
               .HasForeignKey(h => h.RepuestoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
