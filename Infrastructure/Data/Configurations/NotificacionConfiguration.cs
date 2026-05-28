using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Mensaje).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.Tipo).IsRequired().HasMaxLength(50).HasDefaultValue("Info");
        builder.Property(n => n.Url).HasMaxLength(500);

        builder.HasOne(n => n.Usuario)
               .WithMany(u => u.Notificaciones)
               .HasForeignKey(n => n.UsuarioId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
