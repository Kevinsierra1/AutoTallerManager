using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class TelefonoClienteConfiguration : IEntityTypeConfiguration<TelefonoCliente>
{
    public void Configure(EntityTypeBuilder<TelefonoCliente> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Tipo).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Numero).IsRequired().HasMaxLength(20);

        builder.HasOne(t => t.Cliente)
               .WithMany(c => c.Telefonos)
               .HasForeignKey(t => t.ClienteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
