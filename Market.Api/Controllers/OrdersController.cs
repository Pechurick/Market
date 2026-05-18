using Market.Application.Commands; 
using MediatR; 
using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Route("api/users/{userId:long}/orders")]

public class OrdersController(IOrdersService service, IMediator mediator) : ApiController 
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<OrderListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromRoute] long userId, 
        [FromQuery] PagedRequestDto pagedRequest, 
        CancellationToken cancellationToken)
    {
        var result = await service.GetPaged(userId, pagedRequest, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
    {
        var order = await service.Get(id, userId, cancellationToken);
        return Ok(order);
    }
    
 
    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromRoute] long userId, [FromBody] OrderCreateDto request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(userId, request);
        var orderId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { userId = userId, id = orderId }, new { OrderId = orderId });
    }
    
    [HttpPut("{id:long}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirm([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
    {
        await service.Confirm(id, userId, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("{id:long}/ship")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Ship([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
    {
        await service.Ship(id, userId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:long}/deliver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deliver([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
    {
        await service.Deliver(id, userId, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
    {
        await service.Cancel(id, userId, cancellationToken);
        return NoContent();
    }
}