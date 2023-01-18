using events;

namespace host_domain.Repositories;

public interface IEventRepository
{
    void SaveEvent<T>(T @event) where T : Event;
    List<T> GetEvents<T>(string aggregateId) where T : Event;
}