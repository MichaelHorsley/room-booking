using commands;
using Serilog;

namespace host_domain.CommandHandlers;

public class RegisterNewRoomCommandHandler : IHandleCommand<RegisterNewRoomCommand>
{
    private readonly ILogger _logger;

    public RegisterNewRoomCommandHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(RegisterNewRoomCommand command)
    {
        _logger.Information("Handling command with CorrelationId:{correlationId}", command.CorrelationId);

        return Task.CompletedTask;
    }
}