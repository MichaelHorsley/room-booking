using events;

namespace host_domain.Repositories;

public interface IEventRepository
{
    void SaveEvent<T>(T @event) where T : Event;
}