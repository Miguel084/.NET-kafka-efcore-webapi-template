using Confluent.Kafka;

namespace Consumer;

public class WorkerConfig1(ConsumerConfig config)
{
    public ConsumerConfig Config { get; } = config;
}

public class WorkerConfig2(ConsumerConfig config)
{
    public ConsumerConfig Config { get; } = config;
}

