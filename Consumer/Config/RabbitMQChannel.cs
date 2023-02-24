using RabbitMQ.Client;

namespace Messaging {
    public interface IRabbitMQChannel {
        public IModel Model { get; }
        public void BasicPublish(string exchange, string routingKey, bool mandatory, ReadOnlyMemory<byte> body);
    }

    public class RabbitMQChannel : IRabbitMQChannel {
        public IModel Model { get; }

        public RabbitMQChannel(IRabbitMQConnection connection) {
            Model = connection.CreateModel();
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, ReadOnlyMemory<byte> body)
        {
            // Durable messages. See https://www.rabbitmq.com/dotnet-api-guide.html#publishing and
            // https://www.rabbitmq.com/dotnet-api-guide.html#automatic-recovery-limitations
            IBasicProperties props = Model.CreateBasicProperties();
            props.DeliveryMode = 2; // 1 for transient(stored in RAM and will be lost on server restart) and 2 for persistent (saved in disk and survives server restart)

            Model.BasicPublish(exchange, routingKey, mandatory, props, body);
        }
    }
}