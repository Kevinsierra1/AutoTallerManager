using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MiniOrdenHistorialConfiguration : IEntityTypeConfiguration<MiniOrdenHistorial>
{
    public void Configure(EntityTypeBuilder<MiniOrdenHistorial> b)
    {
        b.ToTable("MiniOrdenHistoriales");
        b.HasKey(x => x.Id);
        b.Property(x => x.Observacion).HasMaxLength(1000);
        b.Property(x => x.CambiadoPor).HasMaxLength(200);

        b.HasOne(x => x.MiniOrden).WithMany(m => m.Historial)
            .HasForeignKey(x => x.MiniOrdenId).OnDelete(DeleteBehavior.Cascade);
    }
}
