namespace commands;

public class Command
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}