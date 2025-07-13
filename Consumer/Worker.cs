using System.Net;
using Confluent.Kafka;
using Shared.Domain.Data.Config;
using Consumer;

namespace Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly WorkerConfig1 _workerConfig;

    public Worker(ILogger<Worker> logger, WorkerConfig1 workerConfig)
    {
        _logger = logger;
        _workerConfig = workerConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Worker1 iniciando execução...");
            using var consumer = KafkaConsumerFactory.CreateConsumer<Ignore, string>(_workerConfig.Config);

            var topicPartition = new TopicPartition("users", 0);
            consumer.Assign(new[] { topicPartition });

            _logger.LogInformation("Worker1: Partições atribuídas: users[[{Partition}]]",
                string.Join(", ", consumer.Assignment.Select(p => p.Partition)));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(TimeSpan.FromSeconds(5));
                    if (cr == null)
                    {
                        _logger.LogInformation("Worker1: Nenhuma mensagem recebida");
                        continue;
                    }

                    _logger.LogInformation("Worker1: Mensagem consumida: {Message} em: {Timestamp} da partição: [{Partition}]",
                        cr.Message.Value, cr.Message.Timestamp.UtcDateTime, cr.Partition);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Worker1: Erro ao consumir mensagem");
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Worker1: Operação cancelada");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker1: Erro fatal durante a execução");
            throw;
        }
    }
}