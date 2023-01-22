using events;

namespace host_projections.Projections
{
    public class AllRoomsViewModelProjection
        : IHandleEvent<RoomRegisteredEvent>
    {
        public Task Handle(RoomRegisteredEvent command)
        {
            return Task.CompletedTask;
        }
    }
}