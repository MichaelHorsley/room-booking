namespace host_domain.Aggregates;

public class Aggregate
{
    public string Id { get; }

    public Aggregate(string id)
    {
        Id = id;
    }
}