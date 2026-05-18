using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController(IBrandsService brandsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll(CancellationToken cancellationToken)
    {
        var brands = await brandsService.GetAll(cancellationToken);
        return Ok(brands);
    }

    [HttpPost]
    public async Task<ActionResult> Create(BrandCreateDto request, CancellationToken cancellationToken)
    {
        await brandsService.Create(request, cancellationToken);
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BrandDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var brand = await brandsService.GetById(id, cancellationToken);

        if (brand is null)
        {
            return NotFound(new { error = "Бренд не знайдено" }); 
        }

        return Ok(brand);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, BrandUpdateDto request, CancellationToken cancellationToken)
    {
        await brandsService.Update(id, request, cancellationToken);
        return NoContent(); 
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await brandsService.Delete(id, cancellationToken);
        return NoContent(); 
    }
}
