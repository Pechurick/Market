using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Route("api/products")]
public class ProductsController(IProductsService service) : ApiController
{
	[HttpGet]
	public async Task<ActionResult<PagedResultDto<ProductListDto>>> GetAll(
    	[FromQuery] Guid? categoryId, 
    	[FromQuery] PagedRequestDto pagedRequest, 
    	CancellationToken cancellationToken)
		{
    	var result = await service.GetPaged(categoryId, pagedRequest, cancellationToken);
    
    	return Ok(result);
	}
	
	[HttpGet("{id:long}")]
	[ProducesResponseType(typeof(ProductListDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get([FromRoute] long id, CancellationToken cancellationToken)
	{
		var product = await service.Get(id, cancellationToken);

		return Ok(product);
	}
	
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Create([FromBody] ProductCreateDto request, CancellationToken cancellationToken)
	{
		await service.Create(request, cancellationToken);

		return Created();
	}
	
	[HttpPut("{id:long}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Update([FromRoute] long id, [FromBody] ProductUpdateDto request, CancellationToken cancellationToken)
	{
		await service.Update(id, request, cancellationToken);

		return NoContent();
	}
	
	[HttpDelete("{id:long}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
	{
		await service.Delete(id, cancellationToken);

		return NoContent();
	}
}
