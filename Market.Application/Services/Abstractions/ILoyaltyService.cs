namespace Market.Application.Services.Abstractions;

public interface ILoyaltyService
{
    Task UpdateUserLoyaltyAsync(long userId, CancellationToken cancellationToken);
}