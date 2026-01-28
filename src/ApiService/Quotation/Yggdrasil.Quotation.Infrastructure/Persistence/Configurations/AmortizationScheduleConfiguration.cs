using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class AmortizationScheduleConfiguration : IEntityTypeConfiguration<AmortizationSchedule>
{
    public void Configure(EntityTypeBuilder<AmortizationSchedule> builder)
    {
        // Nombre de tabla sin esquema para compatibilidad con SQLite
        builder.ToTable("AmortizationSchedules");

        // Clave primaria
        builder.HasKey(x => x.Id);

        // Configuración de GUID multi-motor
        // Cada proveedor aplicará su función específica automáticamente
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Propiedades requeridas
        builder.Property(x => x.QuoteEngineId)
            .IsRequired();

        builder.Property(x => x.PeriodNumber)
            .IsRequired();

        // Fecha - cada proveedor usará su tipo nativo
        builder.Property(x => x.DueDate)
            .IsRequired();

        // Configuración de propiedades decimales con precisión
        // HasPrecision() es compatible con los 3 motores (EF Core 5+)
        ConfigureDecimalProperty(builder, x => x.Principal);
        ConfigureDecimalProperty(builder, x => x.Interest);
        ConfigureDecimalProperty(builder, x => x.InterestTax);
        ConfigureDecimalProperty(builder, x => x.TotalDue);
        ConfigureDecimalProperty(builder, x => x.PrincipalBalance);
        ConfigureDecimalProperty(builder, x => x.InterestBalance);
        ConfigureDecimalProperty(builder, x => x.InterestTaxBalance);
        ConfigureDecimalProperty(builder, x => x.TotalBalance);

        // Índices compatibles
        builder.HasIndex(x => x.QuoteEngineId)
            .HasDatabaseName("IX_AmortizationSchedules_QuoteEngineId");

        builder.HasIndex(x => x.DueDate)
            .HasDatabaseName("IX_AmortizationSchedules_DueDate");

        builder.HasIndex(x => x.PeriodNumber)
            .HasDatabaseName("IX_AmortizationSchedules_PeriodNumber");

        // Índice compuesto único
        builder.HasIndex(x => new { x.QuoteEngineId, x.PeriodNumber })
            .HasDatabaseName("IX_AmortizationSchedules_QuoteEngineId_PeriodNumber")
            .IsUnique();

        // Relación
        builder.HasOne(x => x.QuoteEngine)
            .WithMany(q => q.AmortizationSchedules) 
            .HasForeignKey(x => x.QuoteEngineId)
            .OnDelete(DeleteBehavior.Cascade);

    }

    private static void ConfigureDecimalProperty<T>(
        EntityTypeBuilder<T> builder,
        System.Linq.Expressions.Expression<Func<T, decimal>> propertyExpression)
        where T : class
    {
        builder.Property(propertyExpression)
            .IsRequired()
            .HasPrecision(18, 4); // EF Core 5+ convierte esto al tipo adecuado para cada motor
    }


}
