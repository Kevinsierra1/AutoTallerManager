using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrdenAreaDetalleConfiguration : IEntityTypeConfiguration<OrdenAreaDetalle>
{
    public void Configure(EntityTypeBuilder<OrdenAreaDetalle> b)
    {
        b.ToTable("OrdenAreaDetalles");
        b.HasKey(x => x.Id);
        b.Property(x => x.PrecioUnitario).HasPrecision(18, 2);
        b.Property(x => x.Subtotal).HasPrecision(18, 2);
        b.Property(x => x.Observaciones).HasMaxLength(500);

        b.HasOne(x => x.OrdenArea).WithMany(o => o.Detalles)
            .HasForeignKey(x => x.OrdenAreaId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Repuesto).WithMany(r => r.OrdenAreaDetalles)
            .HasForeignKey(x => x.RepuestoId).OnDelete(DeleteBehavior.Restrict);
    }
}
