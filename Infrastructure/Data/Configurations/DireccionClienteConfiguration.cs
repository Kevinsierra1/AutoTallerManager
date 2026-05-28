using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class DireccionClienteConfiguration : IEntityTypeConfiguration<DireccionCliente>
{
    public void Configure(EntityTypeBuilder<DireccionCliente> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Tipo).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Linea1).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Linea2).HasMaxLength(200);
        builder.Property(d => d.Ciudad).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Departamento).HasMaxLength(100);
        builder.Property(d => d.Pais).IsRequired().HasMaxLength(100).HasDefaultValue("Colombia");
        builder.Property(d => d.CodigoPostal).HasMaxLength(10);

        builder.HasOne(d => d.Cliente)
               .WithMany(c => c.Direcciones)
               .HasForeignKey(d => d.ClienteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
