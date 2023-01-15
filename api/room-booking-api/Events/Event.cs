namespace Events;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AggregateId { get; set; }
}