//using Ardalis.Result.AspNetCore;
//using LiteBus.Commands.Abstractions;
//using LiteBus.Queries.Abstractions;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Yggdrasil.Quotation.ApiService.Infrastructure;
//using Yggdrasil.Quotation.Application.Features.Catalogs.Commands;
//using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
//using Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

//namespace Yggdrasil.Quotation.ApiService.Endpoints;
//public class PaymentTerm : EndpointGroupBase
//{
//    public override void Map(RouteGroupBuilder groupBuilder)
//    {
//        groupBuilder.MapPost("/", CreatePaymentTerm);
      

//        groupBuilder.MapPost("{id}/toggle", TogglePaymentTermStatus);
//        groupBuilder.MapGet(GetAllPaymentTerm);
//    }

//    //public async Task<Results<Created<int>, BadRequest>> CreatePaymentTerm(ICommandMediator commandMediator, CreatePaymentTermCommand command)
//    //{
//    //    var result = await commandMediator.SendAsync(command);
//    //    if (result.IsSuccess)
//    //    {
//    //        return TypedResults.Created($"/{nameof(PaymentTerm)}/{result.Value}", result.Value);
//    //    }

//    //    return TypedResults.BadRequest();
//    //}

//    [EndpointSummary("Crea un nuevo término de pago")]
//    [EndpointDescription("Permite registrar términos de pago validados por el negocio")]
//    public async Task<IResult> CreatePaymentTerm(
//        ICommandMediator commandMediator,
//        CreatePaymentTermCommand command
//    )
//    {
//        var result = await commandMediator.SendAsync(command);
//        return result.ToMinimalApiResult();
//    }
//    public async Task<Ok<List<PaymentTermDto>>> GetAllPaymentTerm(IQueryMediator queryMediator, [AsParameters] GetAllPaymentTermQuery query)
//    {
//        var result = await queryMediator.QueryAsync(query);
//        return TypedResults.Ok(result);
//    }

//    public async Task<Results<NoContent, BadRequest>> TogglePaymentTermStatus(ICommandMediator commandMediator, int id, TogglePaymentTermStatusCommand command)
//    {
//        if (id != command.PaymentTermId) return TypedResults.BadRequest();

//        await commandMediator.SendAsync(command);

//        return TypedResults.NoContent();
//    }
  
 
//}
