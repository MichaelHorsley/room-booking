using events;
using host_domain.Repositories;
using host_domain.Services;

namespace host_domain.Aggregates;

public class HostAggregate : Aggregate
{
    private bool _alreadyCreated = false;

    public HostAggregate(string id, IEventRepository eventRepository, IEventDispatcher eventDispatcher) : base(id, eventRepository, eventDispatcher)
    {
    }
    
    public void RegisterNewHost(string email, string firstName, string surname)
    {
        if (!_alreadyCreated)
        {
            Raise(new HostRegisteredEvent
            {
                Email = email,
                FirstName = firstName,
                Surname = surname
            });
        }
    }

    private void Apply(HostRegisteredEvent @event)
    {
        _alreadyCreated = true;
    }
}