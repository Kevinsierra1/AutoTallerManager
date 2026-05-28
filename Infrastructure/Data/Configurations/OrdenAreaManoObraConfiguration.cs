using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrdenAreaManoObraConfiguration : IEntityTypeConfiguration<OrdenAreaManoObra>
{
    public void Configure(EntityTypeBuilder<OrdenAreaManoObra> b)
    {
        b.ToTable("OrdenAreaManosObra");
        b.HasKey(x => x.Id);
        b.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        b.Property(x => x.HorasTrabajo).HasPrecision(10, 2);
        b.Property(x => x.TarifaHora).HasPrecision(18, 2);
        b.Property(x => x.Total).HasPrecision(18, 2);

        b.HasOne(x => x.OrdenArea).WithMany(o => o.ManosObra)
            .HasForeignKey(x => x.OrdenAreaId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Tecnico).WithMany()
            .HasForeignKey(x => x.TecnicoId).OnDelete(DeleteBehavior.SetNull);
    }
}
