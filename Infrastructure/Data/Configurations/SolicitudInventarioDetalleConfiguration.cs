using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SolicitudInventarioDetalleConfiguration : IEntityTypeConfiguration<SolicitudInventarioDetalle>
{
    public void Configure(EntityTypeBuilder<SolicitudInventarioDetalle> b)
    {
        b.ToTable("SolicitudInventarioDetalles");
        b.HasKey(x => x.Id);
        b.Property(x => x.Observaciones).HasMaxLength(500);

        b.HasOne(x => x.SolicitudInventario).WithMany(s => s.Detalles)
            .HasForeignKey(x => x.SolicitudInventarioId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Repuesto).WithMany(r => r.SolicitudDetalles)
            .HasForeignKey(x => x.RepuestoId).OnDelete(DeleteBehavior.Restrict);
    }
}
