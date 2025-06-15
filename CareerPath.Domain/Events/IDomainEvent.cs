namespace CareerPath.Domain.Events;
using MediatR;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    string EventType { get; }
}