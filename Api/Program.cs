using Confluent.Kafka;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var kafkaConfig = new ProducerConfig {
    BootstrapServers = "localhost:9092"
};

app.MapPost("/orders", async (Order order) =>
{
    using var producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
    var msg = JsonSerializer.Serialize(order);
    await producer.ProduceAsync("orders", new Message<Null, string> { Value = msg });
    return Results.Ok(new { status = "enviado", order });
});

app.MapGet("/", () => "Trading Simulator API rodando!");

app.Run();

record Order(string Ticker, string Type, int Quantity, decimal Price);