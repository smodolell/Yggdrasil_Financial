//namespace Yggdrasil.Quotation.Application.Common.Behaviours;

////using LiteBus.Commands.Abstractions;
////using FluentValidation;
//////public sealed class PreHandler : ICommandPreHandler<ICommand>
//////{


//////    public Task PreHandleAsync(ICommand command, CancellationToken cancellationToken = default)
//////    {

//////        return Task.CompletedTask;
//////    }
//////}


////public interface IValidatableCommand : ICommand
////{
////    // Puedes agregar propiedades comunes si lo deseas
////}

////public sealed class ValidationPreHandler : ICommandPreHandler<ICommand>
////{
////    private readonly IServiceProvider _serviceProvider;

////    public ValidationPreHandler(IServiceProvider serviceProvider)
////    {
////        _serviceProvider = serviceProvider;
////    }

////    public async Task PreHandleAsync(ICommand command, CancellationToken cancellationToken = default)
////    {
////        // Resuelve todos los validadores registrados para el tipo concreto
////        var validators = (IEnumerable<IValidator<IValidatableCommand>>)
////            _serviceProvider.GetService(typeof(IEnumerable<IValidator<IValidatableCommand>>));

////        if (validators != null && validators.Any())
////        {
////            var validationResults = await Task.WhenAll(
////                validators.Select(v =>
////                    v.ValidateAsync(new ValidationContext<ICommand>(command), cancellationToken)));

////            var failures = validationResults
////                .Where(r => r.Errors.Any())
////                .SelectMany(r => r.Errors)
////                .ToList();

////            if (failures.Count != 0)
////                throw new ValidationException(failures);
////        }
////    }
////}

//using FluentValidation;
//using LiteBus.Messaging.Abstractions;
//using LiteBus.Queries.Abstractions;

//public class QueryValidationPreHandler<TQuery, TResult> : IQueryPreHandler<TQuery>
//    where TQuery : IQuery<TResult>
//{
//    private readonly IEnumerable<IValidator<TQuery>> _validators;

//    public QueryValidationPreHandler(IEnumerable<IValidator<TQuery>> validators)
//    {
//        _validators = validators;
//    }

//    public async Task PreHandleAsync(TQuery message, CancellationToken cancellationToken = default)
//    {
//        // Si entra aquí, logramos el vínculo
//        if (!_validators.Any()) return;

//        var context = new ValidationContext<TQuery>(message);
//        var validationResults = await Task.WhenAll(
//            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
//        );

//        var failures = validationResults
//            .SelectMany(r => r.Errors)
//            .Where(f => f != null)
//            .ToList();

//        if (failures.Count != 0)
//        {
//            var invalidResult = Result.Invalid(failures.AsErrors());

//            // Abortamos con el tipo de resultado esperado
//            AmbientExecutionContext.Current.Abort(invalidResult);
//        }
//    }
//}