using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Data.Configurations;

public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Placa).IsRequired().HasMaxLength(20);
        builder.HasIndex(v => v.Placa).IsUnique();
        builder.Property(v => v.Vin).HasMaxLength(17);
        builder.HasOne(v => v.ModeloVehiculo).WithMany(m => m.Vehiculos).HasForeignKey(v => v.ModeloVehiculoId);
        builder.HasOne(v => v.Color).WithMany().HasForeignKey(v => v.ColorId).OnDelete(DeleteBehavior.SetNull);
    }
}
