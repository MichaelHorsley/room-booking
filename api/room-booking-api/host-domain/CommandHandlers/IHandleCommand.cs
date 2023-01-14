using commands;

namespace host_domain.CommandHandlers
{
    public interface IHandleCommand<T> where T : Command
    {
        public Task Handle(T command);
    }
}