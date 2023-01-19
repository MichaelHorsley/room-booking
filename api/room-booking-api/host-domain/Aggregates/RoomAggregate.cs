using events;
using host_domain.Repositories;
using host_domain.Services;

namespace host_domain.Aggregates;

public class RoomAggregate : Aggregate
{
    private bool _alreadyCreated = false;

    public RoomAggregate(string id, IEventRepository eventRepository, IEventDispatcher eventDispatcher) : base(id, eventRepository, eventDispatcher)
    {
    }
    
    public void RegisterNewRoom(Guid hostId, string roomId)
    {
        if (!_alreadyCreated)
        {
            Raise(new RoomRegisteredEvent
            {
                HostId = hostId,
                RoomId = roomId
            });
        }
    }

    private void Apply(RoomRegisteredEvent @event)
    {
        _alreadyCreated = true;
    }
}