using Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer;

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
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel.Model);

        // Message received handler
        consumer.Received += async (ch, ea) =>
        {
            try {
                var body = ea.Body.ToArray();

                if (handleExceededRetryCount(ea)) {
                    // Reset headers to reset retry count then send message to parking queue
                    ea.BasicProperties.ClearHeaders();

                    _channel.BasicPublish(QueueConfig.Exchange, QueueConfig.RoutingKeyP, true, ea.Body);
                } else {
                    // copy or deserialise the payload
                    // and process the message
                    // ...                        
                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(body));
                }

                // Acknowledge message delivery. See https://www.rabbitmq.com/confirms.html#acknowledgement-modes
                _channel.Model.BasicAck(ea.DeliveryTag, false);
            } catch(Exception ex) {
                // TODO: Log exception
                Console.WriteLine(ex);
                
                // See https://www.rabbitmq.com/confirms.html#consumer-nacks-requeue
                _channel.Model.BasicReject(ea.DeliveryTag, false);
            }
        };

        // this consumer tag identifies the subscription
        // when it has to be cancelled
        string consumerTag = _channel.Model.BasicConsume(QueueConfig.QueueA, false, consumer);

        // You can cancel an active consumer with IModel.BasicCancel:
        // channel.BasicCancel(consumerTag)        
    }

    // Check if message has been retried more than twice
    private bool handleExceededRetryCount(BasicDeliverEventArgs ea) {
        if (ea.BasicProperties.IsHeadersPresent()) {
            List<Object> xDeathHeaders = (List<Object>) ea.BasicProperties.Headers["x-death"];
            Dictionary<String, Object> header = (Dictionary<String, Object>) xDeathHeaders[0];

            return ((long) header["count"]) > 2;
        }

        return false;
    }    
}
