using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
        builder.ToTable("TaxRates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Percentage)
            .IsRequired()
            .HasPrecision(6, 3); // Ejemplo: 99.999%

        builder.Property(x => x.EffectiveDate)
            .IsRequired();

        builder.Property(x => x.ExpirationDate)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_TaxRates_Code");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_TaxRates_Name");

        builder.HasIndex(x => x.EffectiveDate)
            .HasDatabaseName("IX_TaxRates_EffectiveDate");

        builder.HasIndex(x => x.ExpirationDate)
            .HasDatabaseName("IX_TaxRates_ExpirationDate")
            .HasFilter("[ExpirationDate] IS NOT NULL"); // Solo SQL Server

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_TaxRates_IsActive");

        builder.HasIndex(x => x.Percentage)
            .HasDatabaseName("IX_TaxRates_Percentage");

        // Índice compuesto para búsqueda de tasas activas en un período
        builder.HasIndex(x => new { x.IsActive, x.EffectiveDate, x.ExpirationDate })
            .HasDatabaseName("IX_TaxRates_Active_DateRange");

        // Relación con Plans (si existe relación directa)
        // Si Plan tiene TaxRateId, configurar aquí, sino comentar
        // builder.HasMany(x => x.Plans)
        //     .WithOne(p => p.TaxRate) // Si Plan tiene propiedad TaxRate
        //     .HasForeignKey(p => p.TaxRateId) // Si Plan tiene TaxRateId
        //     .OnDelete(DeleteBehavior.Restrict);

        //// Validaciones
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_TaxRates_Percentage",
        //    "[Percentage] >= 0 AND [Percentage] <= 100"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_TaxRates_DateRange",
        //    "[ExpirationDate] IS NULL OR [ExpirationDate] > [EffectiveDate]"));

        // Validación: Solo una tasa activa por código en un momento dado
        // Esto sería mejor manejarlo a nivel de aplicación con una regla de negocio

        // Configuración de propiedades de BaseEntity
        ConfigureBaseEntity(builder);
    }

    private static void ConfigureBaseEntity(EntityTypeBuilder<TaxRate> builder)
    {
        // Si BaseEntity tiene propiedades de auditoría, configúralas aquí
        // builder.Property(x => x.CreatedAt)
        //     .IsRequired()
        //     .HasDefaultValueSql("GETUTCDATE()");
        // 
        // builder.Property(x => x.ModifiedAt)
        //     .IsRequired(false);
    }
}