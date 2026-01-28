using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class QuoteEngineConfiguration : IEntityTypeConfiguration<QuoteEngine>
{
    public void Configure(EntityTypeBuilder<QuoteEngine> builder)
    {
        builder.ToTable("QuoteEngines");

        builder.HasKey(x => x.Id);

        // Configuración de GUID
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()"); // Para SQL Server
                                                      // Para compatibilidad multi-motor: solo .ValueGeneratedOnAdd()

        builder.Property(x => x.FrequencyId)
            .IsRequired();

        builder.Property(x => x.Term)
            .IsRequired();

        // Configuración de propiedades decimales
        ConfigureDecimalProperty(builder, x => x.Rate);
        ConfigureDecimalProperty(builder, x => x.TaxRate);
        ConfigureDecimalProperty(builder, x => x.InsuranceRate);
        ConfigureDecimalProperty(builder, x => x.InsurancePercentage);
        ConfigureDecimalProperty(builder, x => x.Amount);

        // Configuración del enum
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>() // Almacena como string para legibilidad
            .HasMaxLength(50);

        // Índices
        builder.HasIndex(x => x.FrequencyId)
            .HasDatabaseName("IX_QuoteEngines_FrequencyId");

        builder.HasIndex(x => x.Term)
            .HasDatabaseName("IX_QuoteEngines_Term");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_QuoteEngines_Status");

        builder.HasIndex(x => x.Amount)
            .HasDatabaseName("IX_QuoteEngines_Amount");

        // Índice compuesto para búsquedas comunes
        builder.HasIndex(x => new { x.Status, x.FrequencyId, x.Term })
            .HasDatabaseName("IX_QuoteEngines_Status_Frequency_Term");

        // Relación con AmortizationSchedules
        builder.HasMany(x => x.AmortizationSchedules)
            .WithOne(a => a.QuoteEngine)
            .HasForeignKey(a => a.QuoteEngineId)
            .OnDelete(DeleteBehavior.Cascade);

        //// Validaciones
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_QuoteEngines_Term",
        //    "[Term] > 0"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_QuoteEngines_Amount",
        //    "[Amount] >= 0"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_QuoteEngines_Rate",
        //    "[Rate] >= 0"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_QuoteEngines_TaxRate",
        //    "[TaxRate] >= 0"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_QuoteEngines_InsuranceRate",
        //    "[InsuranceRate] >= 0"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_QuoteEngines_InsurancePercentage",
        //    "[InsurancePercentage] >= 0 AND [InsurancePercentage] <= 100"));
    }

    private static void ConfigureDecimalProperty<T>(
        EntityTypeBuilder<T> builder,
        System.Linq.Expressions.Expression<Func<T, decimal>> propertyExpression)
        where T : class
    {
        builder.Property(propertyExpression)
            .IsRequired()
            .HasPrecision(18, 4);
    }
}