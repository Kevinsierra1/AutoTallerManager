using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MiniOrdenAprobacionConfiguration : IEntityTypeConfiguration<MiniOrdenAprobacion>
{
    public void Configure(EntityTypeBuilder<MiniOrdenAprobacion> b)
    {
        b.ToTable("MiniOrdenAprobaciones");
        b.HasKey(x => x.Id);
        b.Property(x => x.AprobadoPorNombre).HasMaxLength(200);
        b.Property(x => x.Observacion).HasMaxLength(1000);

        b.HasOne(x => x.MiniOrden).WithMany(m => m.Aprobaciones)
            .HasForeignKey(x => x.MiniOrdenId).OnDelete(DeleteBehavior.Cascade);
    }
}
