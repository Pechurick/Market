using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Enums;

namespace Market.Application.Services;

public class LoyaltyService(
    IUsersRepository usersRepository,
    IOrdersRepository ordersRepository) : ILoyaltyService
{
    public async Task UpdateUserLoyaltyAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await usersRepository.Get(userId, cancellationToken);
        if (user == null) return;

        // Звертаємося до бази, щоб порахувати загальну суму всіх доставлених замовлень
        var calculatedTotal = await ordersRepository.GetTotalSpentByUser(userId, cancellationToken);

        var newTier = calculatedTotal switch
        {
            >= 15000 => LoyaltyTier.Gold,   
            >= 5000  => LoyaltyTier.Silver, 
            >= 1000  => LoyaltyTier.Bronze, 
            _        => LoyaltyTier.Standard   
        };

        // Оновлюємо юзера тільки якщо дані реально змінилися (економимо запити до БД)
        if (user.LoyaltyTier != newTier || user.TotalSpent != calculatedTotal)
        {
            user.LoyaltyTier = newTier;
            user.TotalSpent = calculatedTotal; 
        
            await usersRepository.Update(user, cancellationToken);
        }
    }
}