using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MiniOrdenDetalleConfiguration : IEntityTypeConfiguration<MiniOrdenDetalle>
{
    public void Configure(EntityTypeBuilder<MiniOrdenDetalle> b)
    {
        b.ToTable("MiniOrdenDetalles");
        b.HasKey(x => x.Id);
        b.Property(x => x.PrecioUnitario).HasPrecision(18, 2);
        b.Property(x => x.Subtotal).HasPrecision(18, 2);
        b.Property(x => x.Observaciones).HasMaxLength(500);

        b.HasOne(x => x.MiniOrden).WithMany(m => m.Detalles)
            .HasForeignKey(x => x.MiniOrdenId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Repuesto).WithMany(r => r.MiniOrdenDetalles)
            .HasForeignKey(x => x.RepuestoId).OnDelete(DeleteBehavior.Restrict);
    }
}
