using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Catalog.Domain.Entities;

namespace Yggdrasil.Catalog.Infrastructure.Persitence.Configurations;

public class FrequencyConfiguration : IEntityTypeConfiguration<Frequency>
{
    public void Configure(EntityTypeBuilder<Frequency> builder)
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

    private void SeedData(EntityTypeBuilder<Frequency> builder)
    {
        builder.HasData(
            new Frequency
            {
                Id = 1,
                Name = "Diario",
                Code = "DAILY",
                DaysInterval = 1,
                PeriodsPerYear = 365
            },
            new Frequency
            {
                Id = 2,
                Name = "Semanal",
                Code = "WEEKLY",
                DaysInterval = 7,
                PeriodsPerYear = 52
            },
            new Frequency
            {
                Id = 3,
                Name = "Quincenal",
                Code = "BIWEEKLY",
                DaysInterval = 15,
                PeriodsPerYear = 24
            },
            new Frequency
            {
                Id = 4,
                Name = "Mensual",
                Code = "MONTHLY",
                DaysInterval = 30,
                PeriodsPerYear = 12
            },
            new Frequency
            {
                Id = 5,
                Name = "Bimestral",
                Code = "BIMONTHLY",
                DaysInterval = 60,
                PeriodsPerYear = 6
            },
            new Frequency
            {
                Id = 6,
                Name = "Trimestral",
                Code = "QUARTERLY",
                DaysInterval = 90,
                PeriodsPerYear = 4
            },
            new Frequency
            {
                Id = 7,
                Name = "Semestral",
                Code = "SEMIANNUAL",
                DaysInterval = 180,
                PeriodsPerYear = 2
            },
            new Frequency
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
