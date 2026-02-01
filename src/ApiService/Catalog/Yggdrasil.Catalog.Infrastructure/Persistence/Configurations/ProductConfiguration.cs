using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Catalog.Domain.Entities;

namespace Yggdrasil.Catalog.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Tabla
        builder.ToTable("Products");

        // Clave primaria
        builder.HasKey(f => f.Id);

        // Propiedades
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);




    }
}
