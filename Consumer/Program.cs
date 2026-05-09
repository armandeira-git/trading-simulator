using Confluent.Kafka;
using MySql.Data.MySqlClient;
using System.Text.Json;

var kafkaConfig = new ConsumerConfig {
    BootstrapServers = "localhost:9092",
    GroupId = "trading-group-3",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var connStr = "Server=localhost;Database=trading;Uid=root;Pwd=root;";

using var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
consumer.Subscribe("orders");

Console.WriteLine("Consumer aguardando mensagens...");

while (true) {
    var msg = consumer.Consume();
    var order = JsonSerializer.Deserialize<JsonElement>(msg.Message.Value);

    var ticker   = order.GetProperty("Ticker").GetString();
    var type     = order.GetProperty("Type").GetString();
    var quantity = order.GetProperty("Quantity").GetInt32();
    var price    = order.GetProperty("Price").GetDecimal();
    var owner    = "trader1";
    var total    = quantity * price;

    using var conn = new MySqlConnection(connStr);
    conn.Open();

    if (type == "BUY") {
        var balanceCmd = new MySqlCommand(
            "SELECT balance FROM wallets WHERE owner = @owner", conn);
        balanceCmd.Parameters.AddWithValue("@owner", owner);
        var balance = (decimal)balanceCmd.ExecuteScalar();

        if (balance < total) {
            Console.WriteLine($"Saldo insuficiente! Saldo: R$ {balance:F2} | Necessario: R$ {total:F2}");
            continue;
        }

        var debitCmd = new MySqlCommand(
            "UPDATE wallets SET balance = balance - @total WHERE owner = @owner", conn);
        debitCmd.Parameters.AddWithValue("@total", total);
        debitCmd.Parameters.AddWithValue("@owner", owner);
        debitCmd.ExecuteNonQuery();
        Console.WriteLine($"Saldo debitado: R$ {total:F2}");
    }

    var insertCmd = new MySqlCommand(
        "INSERT INTO orders (ticker,type,quantity,price) VALUES (@t,@tp,@q,@p)", conn);
    insertCmd.Parameters.AddWithValue("@t",  ticker);
    insertCmd.Parameters.AddWithValue("@tp", type);
    insertCmd.Parameters.AddWithValue("@q",  quantity);
    insertCmd.Parameters.AddWithValue("@p",  price);
    insertCmd.ExecuteNonQuery();
    Console.WriteLine($"Ordem processada: {ticker} {type} {quantity}x R$ {price:F2}");
}