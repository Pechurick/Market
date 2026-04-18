using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Route("api/users/{userId:long}/orders")]
public class OrdersController(IOrdersService service) : ApiController // Або ControllerBase, залежно від того, від чого ти наслідуєшся
{
    [HttpGet]
    // Змінюємо тип відповіді в документації на PagedResultDto
    [ProducesResponseType(typeof(PagedResultDto<OrderListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromRoute] long userId, 
        [FromQuery] PagedRequestDto pagedRequest, // Параметри ?pageNumber=1&pageSize=10
        CancellationToken cancellationToken)
    {
        // Викликаємо новий метод сервісу
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromRoute] long userId, [FromBody] OrderCreateDto request, CancellationToken cancellationToken)
    {
        await service.Create(userId, request, cancellationToken);
        return Created(); // Або CreatedAtAction, якщо хочеш повертати посилання на створене замовлення
    }
    
    // 👇 НОВІ ЕНДПОІНТИ СТАТУСІВ
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
