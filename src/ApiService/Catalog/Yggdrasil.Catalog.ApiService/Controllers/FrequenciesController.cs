using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Yggdrasil.Catalog.Application.Features.Frecuencies.Commands;
using Yggdrasil.Catalog.Application.Features.Frecuencies.DTOs;
using Yggdrasil.Catalog.Application.Features.Frecuencies.Queries;

namespace Yggdrasil.Catalog.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FrequenciesController(IQueryMediator queryMediator,ICommandMediator commandMediator) : ControllerBase
{
    private readonly IQueryMediator _queryMediator = queryMediator;
    private readonly ICommandMediator _commandMediator = commandMediator;

    [HttpGet]
    //public async Task<IActionResult<PagedResult<List<FrequencyListItemDto>>>> GetFrequencies(
    public async Task<PagedResult<List<FrequencyListItemDto>>> GetFrequencies(
        [FromQuery] GetFrequenciesQuery query,
        CancellationToken cancellationToken)
    {
        //var result = await _queryMediator.QueryAsync(query, cancellationToken);
        return await _queryMediator.QueryAsync(query, cancellationToken);
        //return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> CreateFrequency(
    [FromBody] CreateOrUpdateFrequencyCommand command,
    CancellationToken cancellationToken)
    {
        // Forzar que FrequencyId sea 0 para creación
        command.FrequencyId = 0;

        var result = await _commandMediator.SendAsync(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetFrequencyById), new { id = result.Value }, result);

        if (result.Status == ResultStatus.Invalid)
            return BadRequest(result.ValidationErrors);

        return BadRequest(result.Errors);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Result<int>>> UpdateFrequency(
        int id,
        [FromBody] CreateOrUpdateFrequencyCommand command,CancellationToken cancellationToken)
    {
        command.FrequencyId = id;
        
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result.Status == ResultStatus.NotFound)
            return NotFound(result.Errors);

        if (result.Status == ResultStatus.Invalid)
            return BadRequest(result.ValidationErrors);

        return BadRequest(result.Errors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FrequencyListItemDto>> GetFrequencyById(
       int id,
       CancellationToken cancellationToken)
    {
        var query = new GetFrequencyByIdQuery(id);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);


        if (result.Status == ResultStatus.NotFound)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }
}

