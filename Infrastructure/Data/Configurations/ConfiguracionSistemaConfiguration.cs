using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class ConfiguracionSistemaConfiguration : IEntityTypeConfiguration<ConfiguracionSistema>
{
    public void Configure(EntityTypeBuilder<ConfiguracionSistema> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Clave).IsRequired().HasMaxLength(100);
        builder.HasIndex(c => c.Clave).IsUnique();
        builder.Property(c => c.Valor).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.Tipo).IsRequired().HasMaxLength(50).HasDefaultValue("string");
        builder.Property(c => c.Descripcion).HasMaxLength(500);
        builder.Property(c => c.Grupo).HasMaxLength(100);

        builder.HasData(
            new ConfiguracionSistema { Id = Guid.Parse("99999999-0001-0001-0001-000000000001"), Clave = "app.nombre",       Valor = "AutoTallerManager",           Tipo = "string", Grupo = "General",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new ConfiguracionSistema { Id = Guid.Parse("99999999-0001-0001-0001-000000000002"), Clave = "app.version",      Valor = "1.0.0",                        Tipo = "string", Grupo = "General",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new ConfiguracionSistema { Id = Guid.Parse("99999999-0001-0001-0001-000000000003"), Clave = "factura.iva",      Valor = "19",                           Tipo = "int",    Grupo = "Facturación", Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new ConfiguracionSistema { Id = Guid.Parse("99999999-0001-0001-0001-000000000004"), Clave = "stock.alertas",    Valor = "true",                         Tipo = "bool",   Grupo = "Inventario", Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new ConfiguracionSistema { Id = Guid.Parse("99999999-0001-0001-0001-000000000005"), Clave = "taller.moneda",    Valor = "COP",                          Tipo = "string", Grupo = "General",    Activo = true, CreadoEn = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );
    }
}
