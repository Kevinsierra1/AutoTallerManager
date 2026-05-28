using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class VehiculoMantenimientoConfiguration : IEntityTypeConfiguration<VehiculoMantenimiento>
{
    public void Configure(EntityTypeBuilder<VehiculoMantenimiento> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Tipo).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Descripcion).IsRequired().HasMaxLength(500);
        builder.Property(v => v.Costo).HasPrecision(18, 2);

        builder.HasOne(v => v.Vehiculo)
               .WithMany(ve => ve.Mantenimientos)
               .HasForeignKey(v => v.VehiculoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.OrdenServicio)
               .WithMany()
               .HasForeignKey(v => v.OrdenServicioId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
