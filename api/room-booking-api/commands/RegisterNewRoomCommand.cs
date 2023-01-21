namespace commands;

public class RegisterNewRoomCommand : Command
{
    public string RoomId { get; set; }
    public Guid HostId { get; set; }
}