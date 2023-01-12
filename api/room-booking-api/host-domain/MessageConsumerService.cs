using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace host_domain;

public class MessageConsumerService : IHostedService
{
    private readonly ILogger<MessageConsumerService> _logger;

    public MessageConsumerService(ILogger<MessageConsumerService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is starting up");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is shutting down");

        return Task.CompletedTask;
    }
}