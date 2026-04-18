using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Route("api/users")]
public class UsersController(IUserService service) : ApiController
{
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<UserListDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var users = await service.GetAll(cancellationToken);

		return Ok(users);
	}
	
	[HttpGet("{id:long}")]
	[ProducesResponseType(typeof(UserListDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get([FromRoute] long id, CancellationToken cancellationToken)
	{
		var user = await service.Get(id, cancellationToken);

		return Ok(user);
	}
	
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Create([FromBody] UserCreateDto request, CancellationToken cancellationToken)
	{
		await service.Create(request, cancellationToken);

		return Created();
	}
	
	[HttpPut("{id:long}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UserUpdateDto request, CancellationToken cancellationToken)
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
