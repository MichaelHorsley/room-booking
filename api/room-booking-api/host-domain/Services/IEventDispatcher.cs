using events;

namespace host_domain.Services;

public interface IEventDispatcher
{
    void Dispatch<T>(T @event) where T : Event;
}