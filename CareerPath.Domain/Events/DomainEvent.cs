namespace CareerPath.Domain.Events;
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; private set; }

    public string EventType => throw new NotImplementedException();

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}