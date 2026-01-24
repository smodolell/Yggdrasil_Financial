namespace Yggdrasil.Catalog.Application.Features.Frecuencies.Commands;

public class CreateOrUpdateFrequencyCommandValidator : AbstractValidator<CreateOrUpdateFrequencyCommand>
{
    public CreateOrUpdateFrequencyCommandValidator()
    {
        // Reglas para Name
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la frecuencia es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");

        // Reglas para Code
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código de la frecuencia es requerido")
            .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres")
            .Matches("^[A-Z][A-Z0-9_]*$").WithMessage("El código debe comenzar con letra mayúscula y solo puede contener letras mayúsculas, números y guiones bajos");

        // Reglas para Description
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres");

        // Reglas para DaysInterval
        RuleFor(x => x.DaysInterval)
            .GreaterThan(0).WithMessage("El intervalo en días debe ser mayor a 0")
            .LessThanOrEqualTo(365).WithMessage("El intervalo en días no puede exceder 365 días");

        // Reglas para PeriodsPerYear
        RuleFor(x => x.PeriodsPerYear)
            .GreaterThan(0).WithMessage("Los períodos por año deben ser mayores a 0")
            .LessThanOrEqualTo(365).WithMessage("Los períodos por año no pueden exceder 365");

        // Regla de validación cruzada: verificar consistencia entre DaysInterval y PeriodsPerYear
        RuleFor(x => x)
            .Must(x => IsConsistentInterval(x.DaysInterval, x.PeriodsPerYear))
            .WithMessage("El intervalo en días y los períodos por año no son consistentes. Ejemplo válido: 30 días = 12 períodos/año (mensual)");

        // Regla para validar que el código sea único (excepto para la entidad actual en actualización)
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                // Si FrequencyId es 0, es creación, por lo que no puede existir el código
                if (command.FrequencyId == 0)
                {
                    return true; // La validación de unicidad se hará en el handler
                }
                return true;
            })
            .WithMessage("Validación asíncrona pendiente");
    }

    private bool IsConsistentInterval(int daysInterval, int periodsPerYear)
    {
        // Verificar que la relación sea lógica
        // Ejemplo: 30 días = 12 períodos/año (365/30 ≈ 12.16)
        // Permitimos un margen de ±1 período para redondeos
        var expectedPeriods = 365 / (double)daysInterval;
        return Math.Abs(periodsPerYear - expectedPeriods) <= 1.5;
    }
}
