using events;

namespace host_projections;

public interface IHandleEvent<T> where T : Event
{
    public Task Handle(T @event);
}