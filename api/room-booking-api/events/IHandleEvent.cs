namespace events;

public interface IHandleEvent<T> where T : Event
{
    public Task Handle(T @event);
}