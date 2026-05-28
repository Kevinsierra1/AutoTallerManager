using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class VehiculoKilometrajeConfiguration : IEntityTypeConfiguration<VehiculoKilometraje>
{
    public void Configure(EntityTypeBuilder<VehiculoKilometraje> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Observaciones).HasMaxLength(500);
        builder.Property(v => v.RegistradoPor).HasMaxLength(100);

        builder.HasOne(v => v.Vehiculo)
               .WithMany(ve => ve.Kilometrajes)
               .HasForeignKey(v => v.VehiculoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
