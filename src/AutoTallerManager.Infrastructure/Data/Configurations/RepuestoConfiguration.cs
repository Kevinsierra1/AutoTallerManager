using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Data.Configurations;

public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
{
    public void Configure(EntityTypeBuilder<Repuesto> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Codigo).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => r.Codigo).IsUnique();
        builder.Property(r => r.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(r => r.PrecioCompra).HasPrecision(18, 2);
        builder.Property(r => r.PrecioVenta).HasPrecision(18, 2);
        builder.HasOne(r => r.CategoriaRepuesto).WithMany(c => c.Repuestos).HasForeignKey(r => r.CategoriaRepuestoId);
    }
}
