using Messaging;
using Publisher;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>()
            .AddRabbitMQ();
    })
    .Build();

// Initialize queues
QueueConfig.Init(host.Services.GetRequiredService<IRabbitMQChannel>().Model);

await host.RunAsync();
