using events;
using host_projections.Repositories;
using view_models;

namespace host_projections.Projections;

public class HostListViewModelProjection
    : IHandleEvent<HostSignedUpEvent>
{
    private readonly IViewModelRepository _viewModelRepository;

    public HostListViewModelProjection(IViewModelRepository viewModelRepository)
    {
        _viewModelRepository = viewModelRepository;
    }

    public Task Handle(HostSignedUpEvent @event)
    {
        _viewModelRepository.Save(new AllHostsViewModel
        {
            Id = @event.AggregateId,
            FirstName = @event.FirstName,
            Surname = @event.Surname,
            Email = @event.Email,
        });

        return Task.CompletedTask;
    }
}