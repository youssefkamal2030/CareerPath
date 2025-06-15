using CareerPath.Domain.Events;

public class UserProfileUpdatedEvent : DomainEvent
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> Skills { get; set; }
    public DateTime UpdatedAt { get; set; }
}