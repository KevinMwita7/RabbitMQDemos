using RabbitMQ.Client;

namespace Messaging {
    public interface IRabbitMQConnection {
        public IModel CreateModel();
    }

    public class RabbitMQConnection : IRabbitMQConnection {
        private readonly IConnection _connection;

        public RabbitMQConnection(IConfiguration configuration) {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.UserName = configuration["Rabbit:User"];
            connectionFactory.Password = configuration["Rabbit:Password"];

            // connection that will recover automatically. Read more https://www.rabbitmq.com/dotnet-api-guide.html#recovery
            connectionFactory.AutomaticRecoveryEnabled = true;

            // See https://www.rabbitmq.com/dotnet-api-guide.html#client-provided-names
            connectionFactory.ClientProvidedName = "demo:publisher";

            _connection = connectionFactory.CreateConnection();
        }

        public IModel CreateModel() {
            return _connection.CreateModel();
        }
    }
}