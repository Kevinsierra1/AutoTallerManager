using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class HistorialCitaConfiguration : IEntityTypeConfiguration<HistorialCita>
{
    public void Configure(EntityTypeBuilder<HistorialCita> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Observaciones).HasMaxLength(500);
        builder.Property(h => h.CambiadoPor).HasMaxLength(100);

        builder.HasOne(h => h.Cita)
               .WithMany(c => c.Historial)
               .HasForeignKey(h => h.CitaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
