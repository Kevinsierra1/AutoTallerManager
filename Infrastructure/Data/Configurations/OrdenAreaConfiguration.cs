using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrdenAreaConfiguration : IEntityTypeConfiguration<OrdenArea>
{
    public void Configure(EntityTypeBuilder<OrdenArea> b)
    {
        b.ToTable("OrdenAreas");
        b.HasKey(x => x.Id);
        b.Property(x => x.Observaciones).HasMaxLength(1000);
        b.Property(x => x.TotalMateriales).HasPrecision(18, 2);
        b.Property(x => x.TotalManoObra).HasPrecision(18, 2);

        b.HasOne(x => x.OrdenServicio).WithMany(o => o.OrdenAreas)
            .HasForeignKey(x => x.OrdenServicioId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.AreaTaller).WithMany(a => a.OrdenAreas)
            .HasForeignKey(x => x.AreaTallerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Mecanico).WithMany()
            .HasForeignKey(x => x.MecanicoId).OnDelete(DeleteBehavior.SetNull);
    }
}
