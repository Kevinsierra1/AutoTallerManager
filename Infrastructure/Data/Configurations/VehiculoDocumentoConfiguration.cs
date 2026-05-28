using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class VehiculoDocumentoConfiguration : IEntityTypeConfiguration<VehiculoDocumento>
{
    public void Configure(EntityTypeBuilder<VehiculoDocumento> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Tipo).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(v => v.UrlDocumento).IsRequired().HasMaxLength(500);

        builder.HasOne(v => v.Vehiculo)
               .WithMany(ve => ve.Documentos)
               .HasForeignKey(v => v.VehiculoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
