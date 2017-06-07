using System;
using Lykke.RabbitMqBroker.Subscriber;
using RabbitMQ.Client;

namespace Lykke.MarketProfileService.Services.RabbitMq
{
    public class MessageReadWithTransientAutodeleteQueueStrategy : IMessageReadStrategy
    {
        private readonly string _routingKey;

        public MessageReadWithTransientAutodeleteQueueStrategy(string routingKey = "")
        {
            _routingKey = routingKey;
        }

        public string Configure(RabbitMqSubscriberSettings settings, IModel channel)
        {
            var queue = string.IsNullOrEmpty(settings.QueueName) ? settings.ExchangeName + "." + Guid.NewGuid() : settings.QueueName;

            settings.QueueName = channel.QueueDeclare(queue, false, false, true).QueueName;
            channel.QueueBind(settings.QueueName, settings.ExchangeName, _routingKey);

            return settings.QueueName;
        }
    }
}