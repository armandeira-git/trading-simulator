using Confluent.Kafka;
using System.Text.Json;

var config = new ProducerConfig {
    BootstrapServers = "localhost:9092"
};

var order = new {
    Ticker = "PETR4",
    Type = "BUY",
    Quantity = 100,
    Price = 38.50
};

using var producer = new ProducerBuilder<Null, string>(config).Build();

var msg = JsonSerializer.Serialize(order);
await producer.ProduceAsync("orders", new Message<Null, string> { Value = msg });
Console.WriteLine($"Ordem enviada: {msg}");