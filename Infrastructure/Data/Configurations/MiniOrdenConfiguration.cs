using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MiniOrdenConfiguration : IEntityTypeConfiguration<MiniOrden>
{
    public void Configure(EntityTypeBuilder<MiniOrden> b)
    {
        b.ToTable("MiniOrdenes");
        b.HasKey(x => x.Id);
        b.Property(x => x.NumeroMiniOrden).HasMaxLength(30).IsRequired();
        b.HasIndex(x => x.NumeroMiniOrden).IsUnique();
        b.Property(x => x.Descripcion).HasMaxLength(1000).IsRequired();
        b.Property(x => x.TotalMateriales).HasPrecision(18, 2);
        b.Property(x => x.TotalManoObra).HasPrecision(18, 2);
        b.Property(x => x.Total).HasPrecision(18, 2);
        b.Property(x => x.Observaciones).HasMaxLength(2000);
        b.Property(x => x.MotivoRechazo).HasMaxLength(1000);

        b.HasOne(x => x.Cliente).WithMany()
            .HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Vehiculo).WithMany()
            .HasForeignKey(x => x.VehiculoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.OrdenServicio).WithMany(o => o.MiniOrdenes)
            .HasForeignKey(x => x.OrdenServicioId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.OrdenArea).WithMany(a => a.MiniOrdenes)
            .HasForeignKey(x => x.OrdenAreaId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Mecanico).WithMany()
            .HasForeignKey(x => x.MecanicoId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.JefeTaller).WithMany()
            .HasForeignKey(x => x.JefeTallerId).OnDelete(DeleteBehavior.SetNull);
    }
}
