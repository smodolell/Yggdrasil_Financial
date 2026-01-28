using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class PaymentFrequencyConfiguration : IEntityTypeConfiguration<PaymentFrequency>
{
    public void Configure(EntityTypeBuilder<PaymentFrequency> builder)
    {
        builder.ToTable("PaymentFrequencies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.DaysInterval)
            .IsRequired();

        builder.Property(x => x.PeriodsPerYear)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_PaymentFrequencies_Code");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_PaymentFrequencies_Name");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_PaymentFrequencies_IsActive");

        // Índice compuesto para búsquedas comunes
        builder.HasIndex(x => new { x.IsActive, x.DaysInterval })
            .HasDatabaseName("IX_PaymentFrequencies_Active_DaysInterval");
    }
}