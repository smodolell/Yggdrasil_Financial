using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Yggdrasil.Quotation.Domain.Entities;
using Yggdrasil.Quotation.Domain.Interfaces;

namespace Yggdrasil.Quotation.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{

    public DbSet<AmortizationSchedule> AmortizationSchedules { get; set; } = default!;
    public DbSet<InterestRate> InterestRates { get; set; } = default!;
    public DbSet<PaymentFrequency> PaymentFrequencies { get; set; } = default!;
    public DbSet<PaymentTerm> PaymentTerms { get; set; } = default!;
    public DbSet<Plan> Plans { get; set; } = default!;
    public DbSet<PlanPaymentTerm> PlanPaymentTerms { get; set; } = default!;
    public DbSet<QuoteEngine> QuoteEngines { get; set; } = default!;
    public DbSet<TaxRate> TaxRates { get; set; } = default!;


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configuración automática para entidades auditable (soft delete)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(IAuditable).IsAssignableFrom(e.ClrType)))
        {
            // Aplicar filtro de query global para soft delete
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
            var equalsFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(equalsFalse, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }




    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var auditable = (IAuditable)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                auditable.CreatedAt = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                auditable.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
 

}
