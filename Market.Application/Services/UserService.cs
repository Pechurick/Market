using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using FluentValidation;

namespace Market.Application.Services;

public class UserService(IUsersRepository repository, IValidator<UserCreateDto> createValidator, IValidator<UserUpdateDto> updateValidator) : IUserService
{
	public async Task<IEnumerable<UserListDto>> GetAll(CancellationToken cancellationToken)
	{
		var users = await repository.GetAll(cancellationToken);

		return users.Select(x => new UserListDto
		{
			Id = x.Id,
			FirstName = x.FirstName,
			LastName = x.LastName,
			MiddleName = x.MiddleName,
			BirthDate = x.BirthDate
		});
	}
	
	public async Task<UserListDto> Get(long id, CancellationToken cancellationToken)
	{
		var user = await repository.Get(id, cancellationToken);

		if (user is null)
		{
			throw new ArgumentNullException(nameof(user));
		}

		return new UserListDto
		{
			Id = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName,
			MiddleName = user.MiddleName,
			BirthDate = user.BirthDate
		};
	}

	public async Task Create(UserCreateDto request, CancellationToken cancellationToken)
	{
		await createValidator.ValidateAndThrowAsync(request, cancellationToken);
		var user = new User
		{
			FirstName = request.FirstName,
			LastName = request.LastName,
			MiddleName = request.MiddleName,
			BirthDate = request.BirthDate
		};
		
		await repository.Add(user, cancellationToken);
	}
	
	public async Task Update(long id, UserUpdateDto request, CancellationToken cancellationToken)
	{
		await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
		var user = await repository.Get(id, cancellationToken);
		
		if (user is null)
		{
			throw new ArgumentNullException(nameof(user));
		}

		user.FirstName = request.FirstName;
		user.LastName = request.LastName;
		user.MiddleName = request.MiddleName;
		user.BirthDate = request.BirthDate;
		
		await repository.Update(user, cancellationToken);
	}
	
	public async Task Delete(long id, CancellationToken cancellationToken)
	{
		var user = await repository.Get(id, cancellationToken);
		
		if (user is null)
		{
			throw new ArgumentNullException(nameof(user));
		}
		
		await repository.Delete(user, cancellationToken);
	}
}
