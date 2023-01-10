namespace host_api.Controllers;

public class RegisterNewRoomCommand : Command
{
    public Guid RoomId { get; set; }
    public Guid HostId { get; set; }
}