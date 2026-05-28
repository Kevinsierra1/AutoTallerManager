using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class SesionUsuarioConfiguration : IEntityTypeConfiguration<SesionUsuario>
{
    public void Configure(EntityTypeBuilder<SesionUsuario> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.IpAddress).IsRequired().HasMaxLength(50);
        builder.Property(s => s.UserAgent).HasMaxLength(500);
        builder.Property(s => s.TokenSesion).IsRequired().HasMaxLength(500);

        builder.HasOne(s => s.Usuario)
               .WithMany(u => u.Sesiones)
               .HasForeignKey(s => s.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
