using RabbitMQ.Client;

namespace Messaging {
    public class QueueConfig {
        public static string Exchange { get; } = "amq.direct";
        
        // Queues
        public static string QueueA { get; } = "QueueA"; // Active queue
        public static string QueueW { get; } = "QueueW"; // Wait queue
        public static string QueueP { get; } = "QueueP"; // Parking queue
        
        // Routing Key
        public static string RoutingKeyA { get; } = "RoutingKeyA";
        public static string RoutingKeyW { get; } = "RoutingKeyW";
        public static string RoutingKeyP { get; } = "RoutingKeyP";
        public static string XDeadLetterExchange { get; } = "x-dead-letter-exchange";
        public static string XDeadLetterRoutingKey { get; } = "x-dead-letter-routing-key";
        public static string XMessageTtl { get; } = "x-message-ttl";

        public static void Init(IModel channel) {
            // Declare and bind queues
            // flexible, can switch between lifecycles(transient, scoped, singleton) without worrying about queues been created every time channel is requested from service container
            channel.QueueDeclare(QueueConfig.QueueA, true, false, false, new Dictionary<string, object> {
                { QueueConfig.XDeadLetterExchange, QueueConfig.Exchange },
                { QueueConfig.XDeadLetterRoutingKey, QueueConfig.RoutingKeyW } // If message is dead, send to QueueW
            });

            channel.QueueDeclare(QueueConfig.QueueW, true, false, false, new Dictionary<string, object> {
                { QueueConfig.XMessageTtl, 10000 },// Message dies after 10 seconds since arriving in this queue
                { QueueConfig.XDeadLetterExchange, QueueConfig.Exchange },
                { QueueConfig.XDeadLetterRoutingKey, QueueConfig.RoutingKeyA } // After dying message is rerouted to active QueueA for retry
            });

            // Message could not be processed after some number of retries. 
            // Will be sent to this queue and wait for manual intervention
            channel.QueueDeclare(QueueConfig.QueueP, true, false, false, null);

            // Bind queues to exchange
            channel.QueueBind(QueueConfig.QueueA, QueueConfig.Exchange, QueueConfig.RoutingKeyA, null);
            channel.QueueBind(QueueConfig.QueueW, QueueConfig.Exchange, QueueConfig.RoutingKeyW, null);
            channel.QueueBind(QueueConfig.QueueP, QueueConfig.Exchange, QueueConfig.RoutingKeyP, null);            
        }
    }
}