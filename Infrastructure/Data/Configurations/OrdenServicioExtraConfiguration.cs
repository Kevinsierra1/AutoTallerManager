using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class OrdenServicioExtraConfiguration : IEntityTypeConfiguration<OrdenServicioExtra>
{
    public void Configure(EntityTypeBuilder<OrdenServicioExtra> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Descripcion).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Costo).HasPrecision(18, 2);
        builder.Property(o => o.Subtotal).HasPrecision(18, 2);

        builder.HasOne(o => o.OrdenServicio)
               .WithMany(os => os.Extras)
               .HasForeignKey(o => o.OrdenServicioId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
