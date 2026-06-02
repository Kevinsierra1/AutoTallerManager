using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        // Número secuencial amigable: PostgreSQL lo genera automáticamente (1, 2, 3…)
        builder.Property(c => c.Numero)
               .UseIdentityAlwaysColumn()   // GENERATED ALWAYS AS IDENTITY en PostgreSQL
               .ValueGeneratedOnAdd();
        builder.HasIndex(c => c.Numero).IsUnique();

        builder.Property(c => c.Nombres).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Apellidos).IsRequired().HasMaxLength(100);
        builder.Property(c => c.NumeroDocumento).IsRequired().HasMaxLength(20);
        builder.HasIndex(c => c.NumeroDocumento).IsUnique();
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Telefono).HasMaxLength(20);
        builder.HasOne(c => c.TipoDocumento).WithMany().HasForeignKey(c => c.TipoDocumentoId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(c => c.Usuario)
               .WithOne(u => u.Cliente)
               .HasForeignKey<Cliente>(c => c.UsuarioId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);
    }
}
