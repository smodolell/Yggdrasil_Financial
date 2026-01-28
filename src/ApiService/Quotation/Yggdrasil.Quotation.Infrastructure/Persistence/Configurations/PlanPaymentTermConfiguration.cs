using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Configurations;

public class PlanPaymentTermConfiguration : IEntityTypeConfiguration<PlanPaymentTerm>
{
    public void Configure(EntityTypeBuilder<PlanPaymentTerm> builder)
    {
        builder.ToTable("PlanPaymentTerms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PlanId)
            .IsRequired();

        builder.Property(x => x.PaymentTermId)
            .IsRequired();

        builder.Property(x => x.InterestRateId)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();

        // Índices
        builder.HasIndex(x => x.PlanId)
            .HasDatabaseName("IX_PlanPaymentTerms_PlanId");

        builder.HasIndex(x => x.PaymentTermId)
            .HasDatabaseName("IX_PlanPaymentTerms_PaymentTermId");

        builder.HasIndex(x => x.InterestRateId)
            .HasDatabaseName("IX_PlanPaymentTerms_InterestRateId");

        builder.HasIndex(x => x.Order)
            .HasDatabaseName("IX_PlanPaymentTerms_Order");

        // Índice único para evitar duplicados en la combinación Plan-PaymentTerm
        builder.HasIndex(x => new { x.PlanId, x.PaymentTermId })
            .IsUnique()
            .HasDatabaseName("IX_PlanPaymentTerms_PlanId_PaymentTermId");

        // Índice compuesto para ordenamiento
        builder.HasIndex(x => new { x.PlanId, x.Order })
            .HasDatabaseName("IX_PlanPaymentTerms_PlanId_Order");

        // Relaciones
        builder.HasOne(x => x.QuotationPlan)
            .WithMany(p => p.PlanPaymentTerms)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PaymentTerm)
            .WithMany() // Si PaymentTerm tiene colección, especifícala
            .HasForeignKey(x => x.PaymentTermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InterestRate)
            .WithMany(i => i.PlanPaymentTerms)
            .HasForeignKey(x => x.InterestRateId)
            .OnDelete(DeleteBehavior.Restrict);

        //// Validación: Order debe ser positivo
        //builder.ToTable(t => t.HasCheckConstraint(
        //    "CK_PlanPaymentTerms_Order",
        //    "[Order] > 0"));
    }
}
