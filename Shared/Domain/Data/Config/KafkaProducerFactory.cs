using Confluent.Kafka;

namespace Shared.Domain.Data.Config;

public static class KafkaProducerFactory
{
    public static IProducer<TKey, TValue> CreateProducer<TKey, TValue>(ProducerConfig config)
    {
        return new ProducerBuilder<TKey, TValue>(config).Build();
    }
}

