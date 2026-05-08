using Confluent.Kafka;
using MySql.Data.MySqlClient;
using System.Text.Json;

var kafkaConfig = new ConsumerConfig {
    BootstrapServers = "localhost:9092",
    GroupId = "trading-group-2",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var connStr = "Server=localhost;Database=trading;Uid=root;Pwd=root;";

using var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
consumer.Subscribe("orders");

Console.WriteLine("Consumer aguardando mensagens...");

while (true) {
    var msg = consumer.Consume();
    Console.WriteLine($"Mensagem recebida: {msg.Message.Value}");

    var order = JsonSerializer.Deserialize<JsonElement>(msg.Message.Value);

    using var conn = new MySqlConnection(connStr);
    conn.Open();
    var cmd = new MySqlCommand(
        "INSERT INTO orders (ticker,type,quantity,price) VALUES (@t,@tp,@q,@p)", conn);
    cmd.Parameters.AddWithValue("@t",  order.GetProperty("Ticker").GetString());
    cmd.Parameters.AddWithValue("@tp", order.GetProperty("Type").GetString());
    cmd.Parameters.AddWithValue("@q",  order.GetProperty("Quantity").GetInt32());
    cmd.Parameters.AddWithValue("@p",  order.GetProperty("Price").GetDecimal());
    cmd.ExecuteNonQuery();
    Console.WriteLine($"Ordem salva no banco: {msg.Message.Value}");
}