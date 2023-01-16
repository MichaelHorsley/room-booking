using events;

namespace host_domain.Repositories;

public interface IEventRepository
{
    void SaveEvent(Event @event);
}