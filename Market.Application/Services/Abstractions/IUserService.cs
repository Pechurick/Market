using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface IUserService
{
	Task<IEnumerable<UserListDto>> GetAll(CancellationToken cancellationToken);
	Task<UserListDto> Get(long id, CancellationToken cancellationToken);
	Task Create(UserCreateDto request, CancellationToken cancellationToken);
	Task Update(long id, UserUpdateDto request, CancellationToken cancellationToken);
	Task Delete(long id, CancellationToken cancellationToken);
}
