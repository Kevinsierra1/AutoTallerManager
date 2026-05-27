using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Entidad).IsRequired().HasMaxLength(100);
        builder.Property(a => a.RegistroId).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Accion).IsRequired().HasMaxLength(50);
        builder.Property(a => a.UsuarioId).HasMaxLength(50);
        builder.HasQueryFilter(a => !a.Eliminado);
    }
}
