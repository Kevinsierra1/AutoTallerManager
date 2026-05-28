using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TransferenciaInventarioDetalleConfiguration : IEntityTypeConfiguration<TransferenciaInventarioDetalle>
{
    public void Configure(EntityTypeBuilder<TransferenciaInventarioDetalle> b)
    {
        b.ToTable("TransferenciaInventarioDetalles");
        b.HasKey(x => x.Id);
        b.Property(x => x.Observaciones).HasMaxLength(500);

        b.HasOne(x => x.TransferenciaInventario).WithMany(t => t.Detalles)
            .HasForeignKey(x => x.TransferenciaInventarioId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Repuesto).WithMany(r => r.TransferenciaDetalles)
            .HasForeignKey(x => x.RepuestoId).OnDelete(DeleteBehavior.Restrict);
    }
}
