using commands;

namespace host_api.Services
{
    public interface ICommandHandler
    {
        void Dispatch<T>(T command) where T : Command;
    }
}