using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nombres).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Apellidos).IsRequired().HasMaxLength(100);
        builder.Property(c => c.NumeroDocumento).IsRequired().HasMaxLength(20);
        builder.HasIndex(c => c.NumeroDocumento).IsUnique();
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Telefono).HasMaxLength(20);
        builder.HasOne(c => c.TipoDocumento).WithMany().HasForeignKey(c => c.TipoDocumentoId).OnDelete(DeleteBehavior.SetNull);
    }
}
