using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface IOrdersService
{
    Task<IEnumerable<OrderListDto>> GetAll(long userId, CancellationToken cancellationToken);
    Task<OrderDto> Get(long id, long userId, CancellationToken cancellationToken);
    Task Create(long userId, OrderCreateDto request, CancellationToken cancellationToken);
    
    // 👇 Нові методи для Workflow з Лаби №2
    Task Confirm(long id, long userId, CancellationToken cancellationToken);
    Task Ship(long id, long userId, CancellationToken cancellationToken);
    Task Deliver(long id, long userId, CancellationToken cancellationToken);
    Task Cancel(long id, long userId, CancellationToken cancellationToken);
    Task<PagedResultDto<OrderListDto>> GetPaged(long userId, PagedRequestDto pagedRequest, CancellationToken cancellationToken);
}
