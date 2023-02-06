using commands;
using host_domain.Services;
using Serilog;
using System.Text.RegularExpressions;
using host_domain.Aggregates;

namespace host_domain.CommandHandlers;

public class RegisterNewHostCommandHandler : IHandleCommand<SignUpNewHostCommand>
{
    private readonly ILogger _logger;
    private readonly IAggregateService _aggregateService;

    public RegisterNewHostCommandHandler(ILogger logger, IAggregateService aggregateService)
    {
        _logger = logger;
        _aggregateService = aggregateService;
    }

    public Task Handle(SignUpNewHostCommand command)
    {
        _logger.Information("Handling command with CorrelationId:{correlationId}", command.CorrelationId);

        var aggregateId = Regex.Replace((command.FirstName + command.Surname + command.Email).ToLower(), "[^0-9a-zA-Z]+", "");

        var aggregateGuid = GuidUtility.Create(aggregateId);

        var roomAggregate = _aggregateService.Get<HostAggregate>(aggregateGuid.ToString());

        roomAggregate.SignUp(command.Email, command.FirstName, command.Surname);

        return Task.CompletedTask;
    }
}