namespace host_api.Requests;

public class RegisterNewHostRequest : Request
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
}