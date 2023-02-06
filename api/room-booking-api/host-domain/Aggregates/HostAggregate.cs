using events;
using host_domain.Repositories;
using host_domain.Services;

namespace host_domain.Aggregates;

public class HostAggregate : Aggregate
{
    private bool _alreadySignedUp = false;

    public HostAggregate(string id, IEventRepository eventRepository, IEventDispatcher eventDispatcher) : base(id, eventRepository, eventDispatcher)
    {
    }
    
    public void SignUp(string email, string firstName, string surname)
    {
        if (!_alreadySignedUp)
        {
            Raise(new HostSignedUpEvent
            {
                Email = email,
                FirstName = firstName,
                Surname = surname
            });
        }
    }

    private void Apply(HostSignedUpEvent @event)
    {
        _alreadySignedUp = true;
    }
}