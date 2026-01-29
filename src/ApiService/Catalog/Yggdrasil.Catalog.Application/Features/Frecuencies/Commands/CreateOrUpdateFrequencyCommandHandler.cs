using Yggdrasil.Catalog.Application.Interfaces;
using Yggdrasil.Contracts.Messages.Catalog;

namespace Yggdrasil.Catalog.Application.Features.Frecuencies.Commands;

internal class CreateOrUpdateFrequencyCommandHandler : ICommandHandler<CreateOrUpdateFrequencyCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFrequencyRepository _frequencyRepository;
    //private readonly IEventPublisher _eventPublisher;
    private readonly IValidator<CreateOrUpdateFrequencyCommand> _validator;
    private readonly ILogger<CreateOrUpdateFrequencyCommandHandler> _logger;

    public CreateOrUpdateFrequencyCommandHandler(
        IUnitOfWork unitOfWork,
        IFrequencyRepository frequencyRepository,
        //IEventPublisher eventPublisher,
        IValidator<CreateOrUpdateFrequencyCommand> validator,
        ILogger<CreateOrUpdateFrequencyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _frequencyRepository = frequencyRepository;
        //_eventPublisher = eventPublisher;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<int>> HandleAsync(CreateOrUpdateFrequencyCommand command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validación del comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            // Verificar unicidad del código
            var codeExists = await _frequencyRepository
                .AnyAsync(f =>
                    f.Code == command.Code &&
                    (command.FrequencyId == 0 || f.Id != command.FrequencyId),
                    cancellationToken);

            if (codeExists)
            {
                return Result.Error($"Ya existe una frecuencia con el código '{command.Code}'. Por favor, use un código único.");
            }

            // Verificar unicidad del nombre (opcional, según requerimientos)
            var nameExists = await _frequencyRepository
                .AnyAsync(f =>
                    f.Name == command.Name &&
                    (command.FrequencyId == 0 || f.Id != command.FrequencyId),
                    cancellationToken);

            if (nameExists)
            {
                return Result.Error($"Ya existe una frecuencia con el nombre '{command.Name}'. Por favor, use un nombre único.");
            }

            Frequency? frequency;

            // Determinar si es creación o actualización
            if (command.FrequencyId == 0)
            {
                // Creación de nueva frecuencia
                frequency = new Frequency
                {
                    Id = 0, // Asignado por la base de datos
                    Name = command.Name,
                    Code = command.Code,
                    Description = command.Description,
                    DaysInterval = command.DaysInterval,
                    PeriodsPerYear = command.PeriodsPerYear
                };

                await _frequencyRepository.AddAsync(frequency, cancellationToken);


                //await _eventPublisher.PublishAsync(new FrequencyCreatedEvent
                //{
                //    FrequencyId = frequency.Id,
                //    Name = frequency.Name,
                //    Code = frequency.Code,
                //    Description = frequency.Description,
                //    DaysInterval = frequency.DaysInterval,
                //    PeriodsPerYear = frequency.PeriodsPerYear,
                //    IsActive = frequency.IsActive
                //});


                _logger.LogInformation("Frecuencia creada exitosamente con ID: {FrequencyId}, Código: {FrequencyCode}",
                    frequency.Id, frequency.Code);
            }
            else
            {
                // Actualización de frecuencia existente
                frequency = await _frequencyRepository.GetByIdAsync(command.FrequencyId, cancellationToken);

                if (frequency == null)
                {
                    return Result.Error($"No se encontró la frecuencia con ID: {command.FrequencyId}");
                }


                
                frequency.Name = command.Name;
                frequency.Code = command.Code;
                frequency.Description = command.Description;
                frequency.DaysInterval = command.DaysInterval;
                frequency.PeriodsPerYear = command.PeriodsPerYear;

                await _frequencyRepository.UpdateAsync(frequency, cancellationToken);

                //await _eventPublisher.PublishAsync(new FrequencyUpdatedEvent
                //{
                //    FrequencyId = frequency.Id,
                //    NewName = frequency.Name,
                //    Code = frequency.Code,
                //});

                _logger.LogInformation("Frecuencia actualizada exitosamente con ID: {FrequencyId}, Código: {FrequencyCode}",
                    frequency.Id, frequency.Code);
            }

            await _unitOfWork.CommitAsync();

            return Result.Success(frequency.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al procesar la frecuencia. Comando: {@Command}", command);
            return Result.Error($"Error al procesar la frecuencia: {ex.Message}");
        }
    }

}