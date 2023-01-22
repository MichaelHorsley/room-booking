namespace events;

public class HostRegisteredEvent : Event
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
}