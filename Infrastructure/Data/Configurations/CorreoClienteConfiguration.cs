using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class CorreoClienteConfiguration : IEntityTypeConfiguration<CorreoCliente>
{
    public void Configure(EntityTypeBuilder<CorreoCliente> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Tipo).IsRequired().HasMaxLength(50);

        builder.HasOne(c => c.Cliente)
               .WithMany(cl => cl.Correos)
               .HasForeignKey(c => c.ClienteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
