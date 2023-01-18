using events;
using host_domain.Repositories;

namespace host_domain.Aggregates;

public class Aggregate
{
    private readonly IEventRepository _eventRepository;
    public string Id { get; }
    public int Version { get; set; }

    public Aggregate(string id, IEventRepository eventRepository)
    {
        Id = id;
        Version = 0;

        _eventRepository = eventRepository;
    }

    public void Raise<T>(T @event) where T : Event
    {
        @event.AggregateId = Id;
        @event.AggregateVersion = Version + 1;

        _eventRepository.SaveEvent(@event);
    }
}