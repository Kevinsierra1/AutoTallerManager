using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

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
        // OrdenServicioId ahora es nullable (factura puede agrupar varias órdenes)
        builder.HasOne(f => f.OrdenServicio).WithMany().HasForeignKey(f => f.OrdenServicioId).IsRequired(false);
        // Relación 1:N Factura → OrdenesServicio (a través de OrdenServicio.FacturaId)
        builder.HasMany(f => f.Ordenes).WithOne(o => o.Factura).HasForeignKey(o => o.FacturaId).IsRequired(false);
        builder.HasMany(f => f.Pagos).WithOne(p => p.Factura).HasForeignKey(p => p.FacturaId);
    }
}
