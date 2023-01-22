using events;

namespace host_projections.Projections;

public class HostListViewModelProjection
    : IHandleEvent<HostRegisteredEvent>
{
    public Task Handle(HostRegisteredEvent command)
    {
        return Task.CompletedTask;
    }
}