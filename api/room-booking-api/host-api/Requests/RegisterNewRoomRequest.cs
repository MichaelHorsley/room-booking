namespace host_api.Requests;

public class RegisterNewRoomRequest
{
    public Guid HostId { get; set; }
    public Guid RoomId { get; set; }
}