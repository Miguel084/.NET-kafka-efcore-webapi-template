using Confluent.Kafka;

namespace Shared.Domain.Data.Config;

public static class KafkaConsumerFactory
{
    public static IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config)
    {
        return new ConsumerBuilder<TKey, TValue>(config).Build();
    }
}

