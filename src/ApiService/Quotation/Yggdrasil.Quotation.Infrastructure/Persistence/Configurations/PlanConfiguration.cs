using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MinAmount)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.MaxAmount)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.MinAge)
            .IsRequired();

        builder.Property(x => x.MaxAge)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        // Índices
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Plans_Name");

        builder.HasIndex(x => x.StartDate)
            .HasDatabaseName("IX_Plans_StartDate");

        builder.HasIndex(x => x.EndDate)
            .HasDatabaseName("IX_Plans_EndDate");

        // Índice compuesto para búsqueda por rango de fechas
        builder.HasIndex(x => new { x.StartDate, x.EndDate })
            .HasDatabaseName("IX_Plans_DateRange");

        // Índice compuesto para búsqueda por rango de montos
        builder.HasIndex(x => new { x.MinAmount, x.MaxAmount })
            .HasDatabaseName("IX_Plans_AmountRange");

        // Índice compuesto para búsqueda por rango de edades
        builder.HasIndex(x => new { x.MinAge, x.MaxAge })
            .HasDatabaseName("IX_Plans_AgeRange");

        // Relación con PlanPaymentTerms
        builder.HasMany(x => x.PlanPaymentTerms)
            .WithOne(p => p.QuotationPlan)
            .HasForeignKey(p => p.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        //// Validaciones
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_Plans_AmountRange",
        //    "[MaxAmount] >= [MinAmount]"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_Plans_AgeRange",
        //    "[MaxAge] >= [MinAge]"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_Plans_DateRange",
        //    "[EndDate] > [StartDate]"));

        // Validación: Montos deben ser positivos
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_Plans_MinAmount",
        //    "[MinAmount] >= 0"));

        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_Plans_MaxAmount",
        //    "[MaxAmount] >= 0"));
    }
}