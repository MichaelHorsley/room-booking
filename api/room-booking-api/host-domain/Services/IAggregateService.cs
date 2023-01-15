namespace host_domain.Services;

public interface IAggregateService
{
    public T Get<T>(string id);
}