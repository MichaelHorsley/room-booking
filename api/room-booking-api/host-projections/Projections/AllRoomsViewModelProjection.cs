using events;

namespace host_projections.Projections
{
    public class AllRoomsViewModelProjection
        : IHandleEvent<RoomRegisteredEvent>
    {
        public Task Handle(RoomRegisteredEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}