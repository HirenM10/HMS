using MediatR;

namespace Clinical.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}
