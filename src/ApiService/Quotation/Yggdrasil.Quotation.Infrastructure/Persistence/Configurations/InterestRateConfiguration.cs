using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class InterestRateConfiguration : IEntityTypeConfiguration<InterestRate>
{
    public void Configure(EntityTypeBuilder<InterestRate> builder)
    {
        // Nombre de tabla
        builder.ToTable("InterestRates");

        // Clave primaria (int - autoincremental)
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Configuración de propiedades
        builder.Property(x => x.RateName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        // AnnualPercentage con precisión para porcentajes
        builder.Property(x => x.AnnualPercentage)
            .IsRequired()
            .HasPrecision(8, 4) // Ejemplo: 99.9999%
            .HasColumnType("decimal(8,4)");

        // MonthlyPercentage es una propiedad calculada (no se almacena)
        builder.Ignore(x => x.MonthlyPercentage);

        // Fechas
        builder.Property(x => x.EffectiveDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(x => x.ExpirationDate)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(x => x.RateName)
            .HasDatabaseName("IX_InterestRates_RateName");

        builder.HasIndex(x => x.EffectiveDate)
            .HasDatabaseName("IX_InterestRates_EffectiveDate");

        builder.HasIndex(x => x.ExpirationDate)
            .HasDatabaseName("IX_InterestRates_ExpirationDate");
            //.HasFilter("[ExpirationDate] IS NOT NULL"); // Solo para SQL Server

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_InterestRates_IsActive");

        // Índice compuesto para búsquedas comunes
        builder.HasIndex(x => new { x.IsActive, x.EffectiveDate, x.ExpirationDate })
            .HasDatabaseName("IX_InterestRates_Active_DateRange");

        // Relación con PlanPaymentTerms
        builder.HasMany(x => x.PlanPaymentTerms)
            .WithOne() // Especifica si PlanPaymentTerm tiene navegación a InterestRate
            .HasForeignKey("InterestRateId") // O la propiedad de clave foránea si existe
            .OnDelete(DeleteBehavior.Restrict); // O Cascade según tu lógica

        // Validación de rango de fechas (opcional, a nivel de base de datos)
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_InterestRates_DateRange",
        //    "[ExpirationDate] IS NULL OR [ExpirationDate] > [EffectiveDate]"));

        // Configuración de propiedades de BaseEntity
        ConfigureBaseEntity(builder);
    }

    private static void ConfigureBaseEntity(EntityTypeBuilder<InterestRate> builder)
    {
        // Si BaseEntity tiene propiedades de auditoría, configúralas aquí
        // Ejemplo para CreatedAt y ModifiedAt:
        // builder.Property(x => x.CreatedAt)
        //     .IsRequired()
        //     .HasDefaultValueSql("GETUTCDATE()"); // Para SQL Server
        //     // .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Para compatibilidad multi-motor

        // builder.Property(x => x.ModifiedAt)
        //     .IsRequired(false);

        // builder.Property(x => x.CreatedBy)
        //     .HasMaxLength(255)
        //     .IsRequired(false);

        // builder.Property(x => x.ModifiedBy)
        //     .HasMaxLength(255)
        //     .IsRequired(false);

        // Si tienes IsDeleted en BaseEntity:
        // builder.Property(x => x.IsDeleted)
        //     .IsRequired()
        //     .HasDefaultValue(false);

        // builder.HasQueryFilter(x => !x.IsDeleted); // Filtro global para soft delete
    }
}