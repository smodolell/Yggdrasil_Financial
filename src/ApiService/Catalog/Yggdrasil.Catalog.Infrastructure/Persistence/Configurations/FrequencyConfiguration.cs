using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Catalog.Domain.Entities;

namespace Yggdrasil.Catalog.Infrastructure.Persistence.Configurations;

public class FrequencyConfiguration : IEntityTypeConfiguration<PaymentFrequency>
{
    public void Configure(EntityTypeBuilder<PaymentFrequency> builder)
    {
        // Tabla
        builder.ToTable("Frequencies");

        // Clave primaria
        builder.HasKey(f => f.Id);

        // Propiedades
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(f => f.DaysInterval)
            .IsRequired();

        builder.Property(f => f.PeriodsPerYear)
            .IsRequired();

        // Índices
        builder.HasIndex(f => f.Code)
            .IsUnique();

        builder.HasIndex(f => f.DaysInterval);


        // Relaciones
        //builder.HasMany(f => f.QuotationPlanFrequencies)
        //    .WithOne(qpf => qpf.Frequency)
        //    .HasForeignKey(qpf => qpf.FrequencyId)
        //    .OnDelete(DeleteBehavior.Restrict);

        // Seed data para frecuencias comunes
        //SeedData(builder);
    }

    private void SeedData(EntityTypeBuilder<PaymentFrequency> builder)
    {
        builder.HasData(
            new PaymentFrequency
            {
                Id = 1,
                Name = "Diario",
                Code = "DAILY",
                DaysInterval = 1,
                PeriodsPerYear = 365
            },
            new PaymentFrequency
            {
                Id = 2,
                Name = "Semanal",
                Code = "WEEKLY",
                DaysInterval = 7,
                PeriodsPerYear = 52
            },
            new PaymentFrequency
            {
                Id = 3,
                Name = "Quincenal",
                Code = "BIWEEKLY",
                DaysInterval = 15,
                PeriodsPerYear = 24
            },
            new PaymentFrequency
            {
                Id = 4,
                Name = "Mensual",
                Code = "MONTHLY",
                DaysInterval = 30,
                PeriodsPerYear = 12
            },
            new PaymentFrequency
            {
                Id = 5,
                Name = "Bimestral",
                Code = "BIMONTHLY",
                DaysInterval = 60,
                PeriodsPerYear = 6
            },
            new PaymentFrequency
            {
                Id = 6,
                Name = "Trimestral",
                Code = "QUARTERLY",
                DaysInterval = 90,
                PeriodsPerYear = 4
            },
            new PaymentFrequency
            {
                Id = 7,
                Name = "Semestral",
                Code = "SEMIANNUAL",
                DaysInterval = 180,
                PeriodsPerYear = 2
            },
            new PaymentFrequency
            {
                Id = 8,
                Name = "Anual",
                Code = "ANNUAL",
                DaysInterval = 365,
                PeriodsPerYear = 1
            }
        );
    }
}