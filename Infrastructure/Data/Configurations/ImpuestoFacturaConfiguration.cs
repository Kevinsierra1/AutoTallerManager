using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class ImpuestoFacturaConfiguration : IEntityTypeConfiguration<ImpuestoFactura>
{
    public void Configure(EntityTypeBuilder<ImpuestoFactura> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Porcentaje).HasPrecision(5, 2);
        builder.Property(i => i.Base).HasPrecision(18, 2);
        builder.Property(i => i.Monto).HasPrecision(18, 2);

        builder.HasOne(i => i.Factura)
               .WithMany(f => f.FacturaImpuestos)
               .HasForeignKey(i => i.FacturaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
