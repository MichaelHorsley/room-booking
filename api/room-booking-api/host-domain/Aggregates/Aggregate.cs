using events;
using host_domain.Repositories;
using host_domain.Services;

namespace host_domain.Aggregates;

public class Aggregate
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventDispatcher _eventDispatcher;

    public string Id { get; }
    public int Version { get; set; }

    public Aggregate(string id, IEventRepository eventRepository, IEventDispatcher eventDispatcher)
    {
        Id = id;
        Version = 0;

        _eventRepository = eventRepository;
        _eventDispatcher = eventDispatcher;
    }

    public void Raise<T>(T @event) where T : Event
    {
        @event.AggregateId = Id;
        @event.AggregateVersion = Version + 1;

        _eventRepository.SaveEvent(@event);
        _eventDispatcher.Dispatch(@event);
    }
}