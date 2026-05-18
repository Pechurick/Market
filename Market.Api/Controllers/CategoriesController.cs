using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Route("api/categories")]
public class CategoriesController(ICategoriesService service) : ApiController
{
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<CategoryListDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var categories = await service.GetAll(cancellationToken);

		return Ok(categories);
	}

	[HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryListDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await service.GetById(id, cancellationToken);

        if (category is null)
        {
            return NotFound(new { Message = $"Category with ID {id} not found." }); 
        }

        return Ok(category); 
    }
	
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Create([FromBody] CategoryCreateDto request, CancellationToken cancellationToken)
	{
		await service.Create(request, cancellationToken);

		return Created();
	}
	
	[HttpPut("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CategoryUpdateDto request, CancellationToken cancellationToken)
	{
		await service.Update(id, request, cancellationToken);

		return NoContent();
	}
	
	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		await service.Delete(id, cancellationToken);

		return NoContent();
	}
}
