using System.Net;
using Confluent.Kafka;

namespace Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = Dns.GetHostName(),
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("users");


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(stoppingToken);
                _logger.LogInformation("Consumed message: {Message} at: {Timestamp}", cr.Value, cr.Message.Timestamp.UtcDateTime);

            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "Error occurred while consuming message");
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }
}