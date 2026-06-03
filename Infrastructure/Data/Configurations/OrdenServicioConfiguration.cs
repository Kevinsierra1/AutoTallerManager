using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
    public void Configure(EntityTypeBuilder<OrdenServicio> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.NumeroOrden).IsRequired().HasMaxLength(50);
        builder.HasIndex(o => o.NumeroOrden).IsUnique();
        builder.Property(o => o.Total).HasPrecision(18, 2);
        builder.HasOne(o => o.Cliente).WithMany(c => c.Ordenes).HasForeignKey(o => o.ClienteId);
        builder.HasOne(o => o.Vehiculo).WithMany(v => v.Ordenes).HasForeignKey(o => o.VehiculoId);
        builder.HasOne(o => o.Mecanico).WithMany(e => e.OrdenesAsignadas).HasForeignKey(o => o.MecanicoId).IsRequired(false);
        builder.HasOne(o => o.TipoServicio).WithMany().HasForeignKey(o => o.TipoServicioId).IsRequired(false);
        builder.HasMany(o => o.DetallesOrdenServicio).WithOne(d => d.OrdenServicio).HasForeignKey(d => d.OrdenServicioId);
        builder.HasMany(o => o.ManosObra).WithOne(m => m.OrdenServicio).HasForeignKey(m => m.OrdenServicioId);
        builder.HasMany(o => o.HistorialEstados).WithOne(h => h.OrdenServicio).HasForeignKey(h => h.OrdenServicioId);
    }
}
