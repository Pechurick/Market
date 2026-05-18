using MediatR; 

namespace Market.Domain.Events;

public record OrderCreatedEvent(long OrderId, long UserId, DateTime OccurredOn) : INotification;