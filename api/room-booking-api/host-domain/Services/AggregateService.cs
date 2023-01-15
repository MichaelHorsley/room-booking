namespace host_domain.Services;

public class AggregateService : IAggregateService
{
    public T Get<T>(string id)
    {
        return (T)Activator.CreateInstance(typeof(T), id);
    }
}