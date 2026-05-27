using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Data.Configurations;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.NumeroFactura).IsRequired().HasMaxLength(50);
        builder.HasIndex(f => f.NumeroFactura).IsUnique();
        builder.Property(f => f.Subtotal).HasPrecision(18, 2);
        builder.Property(f => f.Impuestos).HasPrecision(18, 2);
        builder.Property(f => f.Descuento).HasPrecision(18, 2);
        builder.Property(f => f.Total).HasPrecision(18, 2);
        builder.HasOne(f => f.Cliente).WithMany(c => c.Facturas).HasForeignKey(f => f.ClienteId);
        builder.HasOne(f => f.OrdenServicio).WithMany().HasForeignKey(f => f.OrdenServicioId);
        builder.HasMany(f => f.Pagos).WithOne(p => p.Factura).HasForeignKey(p => p.FacturaId);
    }
}
