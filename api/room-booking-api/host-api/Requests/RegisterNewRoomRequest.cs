namespace host_api.Requests;

public class RegisterNewRoomRequest : Request
{
    public Guid HostId { get; set; }
    public string RoomId { get; set; }
}