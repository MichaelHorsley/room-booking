namespace host_api.Requests;

public class RegisterNewRoomRequest
{
    public Guid HostId { get; set; }
    public string RoomId { get; set; }
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}