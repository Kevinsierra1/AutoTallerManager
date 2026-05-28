using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SolicitudInventarioConfiguration : IEntityTypeConfiguration<SolicitudInventario>
{
    public void Configure(EntityTypeBuilder<SolicitudInventario> b)
    {
        b.ToTable("SolicitudesInventario");
        b.HasKey(x => x.Id);
        b.Property(x => x.NumeroSolicitud).HasMaxLength(30).IsRequired();
        b.HasIndex(x => x.NumeroSolicitud).IsUnique();
        b.Property(x => x.AprobadoPorNombre).HasMaxLength(200);
        b.Property(x => x.Observaciones).HasMaxLength(2000);
        b.Property(x => x.MotivoRechazo).HasMaxLength(1000);

        b.HasOne(x => x.Solicitante).WithMany()
            .HasForeignKey(x => x.SolicitanteId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.OrdenServicio).WithMany(o => o.Solicitudes)
            .HasForeignKey(x => x.OrdenServicioId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.MiniOrden).WithMany(m => m.Solicitudes)
            .HasForeignKey(x => x.MiniOrdenId).OnDelete(DeleteBehavior.SetNull);
    }
}
