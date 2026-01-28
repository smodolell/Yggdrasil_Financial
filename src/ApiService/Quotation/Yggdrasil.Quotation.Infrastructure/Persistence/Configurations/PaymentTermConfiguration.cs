using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class PaymentTermConfiguration : IEntityTypeConfiguration<PaymentTerm>
{
    public void Configure(EntityTypeBuilder<PaymentTerm> builder)
    {
        builder.ToTable("PaymentTerms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.NumberOfPayments)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_PaymentTerms_Code");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_PaymentTerms_Name");

        builder.HasIndex(x => x.NumberOfPayments)
            .IsUnique()
            .HasDatabaseName("IX_PaymentTerms_NumberOfPayments");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_PaymentTerms_IsActive");

        // Validación: NumberOfPayments debe ser positivo
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_PaymentTerms_NumberOfPayments",
        //    "[NumberOfPayments] > 0"));
    }
}