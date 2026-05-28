using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TransferenciaInventarioConfiguration : IEntityTypeConfiguration<TransferenciaInventario>
{
    public void Configure(EntityTypeBuilder<TransferenciaInventario> b)
    {
        b.ToTable("TransferenciasInventario");
        b.HasKey(x => x.Id);
        b.Property(x => x.NumeroTransferencia).HasMaxLength(30).IsRequired();
        b.HasIndex(x => x.NumeroTransferencia).IsUnique();
        b.Property(x => x.Origen).HasMaxLength(200);
        b.Property(x => x.Destino).HasMaxLength(200);
        b.Property(x => x.AprobadoPorNombre).HasMaxLength(200);
        b.Property(x => x.Observaciones).HasMaxLength(2000);
        b.Property(x => x.MotivoRechazo).HasMaxLength(1000);

        b.HasOne(x => x.SolicitadoPor).WithMany()
            .HasForeignKey(x => x.SolicitadoPorId).OnDelete(DeleteBehavior.SetNull);
    }
}
