using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class VehiculoFotoConfiguration : IEntityTypeConfiguration<VehiculoFoto>
{
    public void Configure(EntityTypeBuilder<VehiculoFoto> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.UrlFoto).IsRequired().HasMaxLength(500);
        builder.Property(v => v.Descripcion).HasMaxLength(200);

        builder.HasOne(v => v.Vehiculo)
               .WithMany(ve => ve.Fotos)
               .HasForeignKey(v => v.VehiculoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
