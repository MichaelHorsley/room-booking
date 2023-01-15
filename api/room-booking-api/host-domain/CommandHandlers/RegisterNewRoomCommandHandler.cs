using commands;
using host_domain.Aggregates;
using host_domain.Services;
using Serilog;

namespace host_domain.CommandHandlers;

public class RegisterNewRoomCommandHandler : IHandleCommand<RegisterNewRoomCommand>
{
    private readonly ILogger _logger;
    private readonly IAggregateService _aggregateService;

    public RegisterNewRoomCommandHandler(ILogger logger, IAggregateService aggregateService)
    {
        _logger = logger;
        _aggregateService = aggregateService;
    }

    public Task Handle(RegisterNewRoomCommand command)
    {
        _logger.Information("Handling command with CorrelationId:{correlationId}", command.CorrelationId);

        var roomAggregate = _aggregateService.Get<RoomAggregate>($"{command.HostId}|{command.RoomId}");

        roomAggregate.RegisterNewRoom(command.HostId, command.RoomId);

        return Task.CompletedTask;
    }
}