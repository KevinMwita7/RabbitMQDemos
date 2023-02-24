using Messaging;

namespace Publisher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRabbitMQChannel _channel;

    public Worker(ILogger<Worker> logger, IRabbitMQChannel channel)
    {
        _logger = logger;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world!");
            _channel.BasicPublish("amq.direct", "RoutingKeyA", true, messageBodyBytes);

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
