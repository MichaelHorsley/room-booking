namespace events;

public class RoomRegisteredEvent : Event
{
    public string RoomId { get; set; }
    public Guid HostId { get; set; }
}