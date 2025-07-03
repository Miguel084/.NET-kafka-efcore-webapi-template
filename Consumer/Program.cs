using System.Net;
using Confluent.Kafka;
using Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = Dns.GetHostName(),
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,
};


var host = builder.Build();


host.Run();