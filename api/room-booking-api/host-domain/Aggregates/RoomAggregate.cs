namespace host_domain.Aggregates;

public class RoomAggregate : Aggregate
{
    public RoomAggregate(string id) : base(id)
    {
    }

    public void RegisterNewRoom(Guid hostId, string roomId)
    {

    }
}