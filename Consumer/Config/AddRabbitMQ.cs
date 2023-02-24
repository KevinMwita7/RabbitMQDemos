using Microsoft.Extensions.Configuration;
using Messaging;

namespace Microsoft.Extensions.DependencyInjection {
    public static class RabbitMqServiceCollectionExtensions
    {

        public static IServiceCollection AddRabbitMQ(
             this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>()
                    .AddSingleton<IRabbitMQChannel, RabbitMQChannel>();

            return services;
        }
    }
}