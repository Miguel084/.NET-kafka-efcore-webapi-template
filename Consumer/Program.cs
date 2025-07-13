using System.Net;
using Confluent.Kafka;
using Consumer;
using Shared.Domain.Data.Config;

var builder = Host.CreateApplicationBuilder(args);

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "users-consumer-group",    // GroupId fixo para o Worker1
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,
};

var config2 = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "credits-consumer-group", // GroupId fixo para evitar conflitos
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,
};

// Remover os registros duplicados de ConsumerConfig
// builder.Services.AddSingleton<ConsumerConfig>(config);
// builder.Services.AddSingleton<ConsumerConfig>(sp => config2);
builder.Services.AddSingleton(new WorkerConfig1(config));
builder.Services.AddSingleton(new WorkerConfig2(config2));
builder.Services.AddHostedService<Worker2>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();


host.Run();