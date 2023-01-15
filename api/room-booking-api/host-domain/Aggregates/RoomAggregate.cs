using Events;
using host_domain.Repositories;

namespace host_domain.Aggregates;

public class RoomAggregate : Aggregate
{
    private bool _alreadyCreated = false;

    public RoomAggregate(string id, IEventRepository eventRepository) : base(id, eventRepository)
    {
    }
    
    public void RegisterNewRoom(Guid hostId, string roomId)
    {
        if (!_alreadyCreated)
        {
            Raise(new RoomRegistered());
        }
    }
}