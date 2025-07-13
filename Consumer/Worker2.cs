using System.Net;
using Confluent.Kafka;
using Shared.Domain.Data.Config;

namespace Consumer;

public class Worker2 : BackgroundService
{
    private readonly ILogger<Worker2> _logger;
    private readonly WorkerConfig2 _workerConfig;

    public Worker2(ILogger<Worker2> logger, WorkerConfig2 workerConfig)
    {
        _logger = logger;
        _workerConfig = workerConfig;
        _logger.LogInformation("Worker2 construtor chamado!");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Worker2 iniciando execução...");
            using var consumer = KafkaConsumerFactory.CreateConsumer<Ignore, string>(_workerConfig.Config);

            // Assina o tópico usersCredits
            var topicPartition = new TopicPartition("usersCredits", 0);
            consumer.Assign(new[] { topicPartition });

            var assignment = consumer.Assignment;
            _logger.LogInformation("Worker2: Partições atribuídas: {Partitions}",
                string.Join(", ", assignment.Select(p => $"{p.Topic}[{p.Partition}]")));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume();
                    if (consumeResult == null)
                    {
                        _logger.LogInformation("Worker2: Nenhuma mensagem recebida no timeout");
                        continue;
                    }

                    _logger.LogInformation("Worker2: Mensagem consumida: {Message} em: {Timestamp} da partição: {Partition}",
                        consumeResult.Value, consumeResult.Message.Timestamp.UtcDateTime, consumeResult.Partition);

                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal ao iniciar Worker2");
        }
    }
}