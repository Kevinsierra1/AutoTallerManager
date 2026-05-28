using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class HistorialAccesoConfiguration : IEntityTypeConfiguration<HistorialAcceso>
{
    public void Configure(EntityTypeBuilder<HistorialAcceso> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.IpAddress).IsRequired().HasMaxLength(50);
        builder.Property(h => h.MotivoFallo).HasMaxLength(200);
        builder.Property(h => h.UserAgent).HasMaxLength(500);

        builder.HasOne(h => h.Usuario)
               .WithMany(u => u.HistorialAccesos)
               .HasForeignKey(h => h.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
