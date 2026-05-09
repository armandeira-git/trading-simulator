using Confluent.Kafka;
using MySql.Data.MySqlClient;
using System.Text.Json;

var consumerConfig = new ConsumerConfig {
    BootstrapServers = "localhost:9092",
    GroupId = "trading-group-4",
    AutoOffsetReset = AutoOffsetReset.Latest
};

var producerConfig = new ProducerConfig {
    BootstrapServers = "localhost:9092"
};

var connStr = "Server=localhost;Database=trading;Uid=root;Pwd=root;";

using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

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
            Console.WriteLine($"❌ Saldo insuficiente! Saldo: R$ {balance:F2} | Necessario: R$ {total:F2}");

            // Publica no tópico rejected
            await producer.ProduceAsync("rejected", new Message<Null, string> {
                Value = JsonSerializer.Serialize(new {
                    Ticker = ticker, Type = type, Quantity = quantity,
                    Price = price, Reason = "Saldo insuficiente",
                    Saldo = balance, Necessario = total
                })
            });

            continue;
        }

        var debitCmd = new MySqlCommand(
            "UPDATE wallets SET balance = balance - @total WHERE owner = @owner", conn);
        debitCmd.Parameters.AddWithValue("@total", total);
        debitCmd.Parameters.AddWithValue("@owner", owner);
        debitCmd.ExecuteNonQuery();
        Console.WriteLine($"💰 Saldo debitado: R$ {total:F2}");
    }

    var insertCmd = new MySqlCommand(
        "INSERT INTO orders (ticker,type,quantity,price) VALUES (@t,@tp,@q,@p)", conn);
    insertCmd.Parameters.AddWithValue("@t",  ticker);
    insertCmd.Parameters.AddWithValue("@tp", type);
    insertCmd.Parameters.AddWithValue("@q",  quantity);
    insertCmd.Parameters.AddWithValue("@p",  price);
    insertCmd.ExecuteNonQuery();

    // Publica no tópico processed
    await producer.ProduceAsync("processed", new Message<Null, string> {
        Value = JsonSerializer.Serialize(new {
            Ticker = ticker, Type = type, Quantity = quantity,
            Price = price, Status = "PROCESSED"
        })
    });

    Console.WriteLine($"✅ Ordem processada: {ticker} {type} {quantity}x R$ {price:F2}");
}