using host_domain.Aggregates;
using host_domain.Repositories;

namespace host_domain.Services;

public class AggregateService : IAggregateService
{
    private readonly IEventRepository _eventRepository;

    public AggregateService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public T Get<T>(string id) where T : Aggregate
    {
        var instance = (T)Activator.CreateInstance(typeof(T), id, _eventRepository);

        return instance;
    }
}