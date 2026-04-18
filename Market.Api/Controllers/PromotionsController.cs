using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers; // Або Market.Api.Controllers, залежно від твого неймспейсу

[Route("api/promotions")]
public class PromotionsController(IPromotionsService service) : ApiController 
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromotionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var promotions = await service.GetAll(cancellationToken);
        return Ok(promotions);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] PromotionCreateDto request, CancellationToken cancellationToken)
    {
        var promotion = await service.Create(request, cancellationToken);
        return Ok(promotion); 
    }
}
