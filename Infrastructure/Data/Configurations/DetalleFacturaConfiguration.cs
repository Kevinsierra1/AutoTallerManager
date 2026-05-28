using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class DetalleFacturaConfiguration : IEntityTypeConfiguration<DetalleFactura>
{
    public void Configure(EntityTypeBuilder<DetalleFactura> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Descripcion).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Cantidad).HasPrecision(18, 4);
        builder.Property(d => d.PrecioUnitario).HasPrecision(18, 2);
        builder.Property(d => d.Descuento).HasPrecision(18, 2);
        builder.Property(d => d.Impuesto).HasPrecision(18, 2);
        builder.Property(d => d.Subtotal).HasPrecision(18, 2);

        builder.HasOne(d => d.Factura)
               .WithMany(f => f.Detalles)
               .HasForeignKey(d => d.FacturaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
