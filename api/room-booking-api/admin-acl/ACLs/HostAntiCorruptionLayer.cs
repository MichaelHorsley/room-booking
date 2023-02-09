using events;

namespace admin_acl.ACLs
{
    public class HostAntiCorruptionLayer
        : IHandleEvent<HostSignedUpEvent>
    {

        public HostAntiCorruptionLayer()
        {
        }

        public Task Handle(HostSignedUpEvent @event)
        {

            return Task.CompletedTask;
        }
    }
}
