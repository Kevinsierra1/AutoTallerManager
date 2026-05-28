using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AreaTallerConfiguration : IEntityTypeConfiguration<AreaTaller>
{
    public void Configure(EntityTypeBuilder<AreaTaller> b)
    {
        b.ToTable("AreasTaller");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        b.Property(x => x.Descripcion).HasMaxLength(500);
        b.HasIndex(x => x.Tipo);
    }
}
