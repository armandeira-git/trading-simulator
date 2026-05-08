# Trading Simulator — Kafka + .NET + Docker

Simulador de ordens de compra e venda de ações em tempo real, inspirado no funcionamento de corretoras e sistemas financeiros como a B3.

## Tecnologias

- .NET 8 (C#)
- Apache Kafka (mensageria e eventos)
- MySQL 8 (persistência)
- Docker + Docker Compose (infraestrutura)

## Arquitetura

```
HTTP POST → API .NET → Kafka (tópico: orders) → Consumer .NET → MySQL
```

- **API**: recebe ordens via HTTP REST e publica no Kafka
- **Kafka**: garante entrega e processamento assíncrono das ordens
- **Consumer**: processa cada ordem e persiste no banco de dados

## Endpoints

### Enviar ordem
```
POST http://localhost:5005/orders
```
```json
{
  "Ticker": "VALE3",
  "Type": "BUY",
  "Quantity": 200,
  "Price": 68.90
}
```

### Verificar API
```
GET http://localhost:5005/
```

## Como rodar localmente

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### 1. Suba a infraestrutura
```bash
docker-compose up -d
```

### 2. Rode o Consumer (terminal 1)
```bash
cd Consumer
dotnet run
```

### 3. Rode a API (terminal 2)
```bash
cd Api
dotnet run
```

### 4. Envie uma ordem (terminal 3)
```powershell
Invoke-WebRequest -Uri "http://localhost:5005/orders" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"Ticker":"VALE3","Type":"BUY","Quantity":200,"Price":68.90}'
```

### 5. Verifique no banco
```sql
SELECT * FROM trading.orders;
```

## Estrutura do projeto

```
trading-simulator/
├── docker-compose.yml
├── Api/
│   ├── Api.csproj
│   └── Program.cs
├── Consumer/
│   ├── Consumer.csproj
│   └── Program.cs
└── sql/
    └── init.sql
```

## Próximos passos

- [x] API REST com .NET Minimal API para receber ordens via HTTP
- [ ] Validação de saldo antes de processar ordens
- [ ] Múltiplos tópicos Kafka (orders, processed, rejected)
- [ ] Containerizar Producer e Consumer no Docker