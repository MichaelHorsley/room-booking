namespace host_api.Requests;

public class Request
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}