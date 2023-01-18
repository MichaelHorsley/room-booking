using System.Reflection;
using events;
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

        var events = _eventRepository.GetEvents<Event>(id);

        var methodInfos = instance
            .GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.Name.Contains("Apply"))
            .ToList();

        var methodDictionary = methodInfos
            .ToDictionary(x => x.GetParameters().First().ParameterType.Name, x => x);

        foreach (var @event in events)
        {
            methodDictionary[@event.GetType().Name].Invoke(instance, new[] { @event });

            instance.Version = @event.AggregateVersion;
        }

        return instance;
    }
}