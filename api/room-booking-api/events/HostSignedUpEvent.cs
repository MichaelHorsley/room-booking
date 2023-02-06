namespace events;

public class HostSignedUpEvent : Event
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
}