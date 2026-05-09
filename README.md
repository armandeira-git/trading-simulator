# Trading Simulator — Kafka + .NET + Docker

Simulador de ordens de compra e venda de ações em tempo real, inspirado no funcionamento de corretoras e sistemas financeiros como a B3.

## Tecnologias

- .NET 8 (C#)
- Apache Kafka (mensageria e eventos)
- MySQL 8 (persistência)
- Docker + Docker Compose (infraestrutura)

## Arquitetura

```
HTTP POST /orders
    → API .NET
        → Kafka (tópico: orders)
            → Consumer verifica saldo
                ✅ Saldo OK  → debita + salva ordem
                ❌ Insuficiente → rejeita ordem
```

## Funcionalidades

- Envio de ordens via HTTP POST
- Publicação no Kafka em tempo real
- Validação de saldo antes de processar compras
- Débito automático do saldo após aprovação
- Rejeição automática de ordens sem saldo suficiente
- Persistência no MySQL

## Endpoints

### Enviar ordem
```
POST http://localhost:5005/orders
```
```json
{
  "Ticker": "VALE3",
  "Type": "BUY",
  "Quantity": 100,
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

### 4. Envie uma ordem válida
```powershell
Invoke-WebRequest -Uri "http://localhost:5005/orders" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"Ticker":"VALE3","Type":"BUY","Quantity":100,"Price":68.90}'
```

### 5. Envie uma ordem inválida (saldo insuficiente)
```powershell
Invoke-WebRequest -Uri "http://localhost:5005/orders" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"Ticker":"PETR4","Type":"BUY","Quantity":1000,"Price":50.00}'
```

### 6. Verifique no banco
```sql
SELECT * FROM trading.orders;
SELECT * FROM trading.wallets;
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
- [x] Validação de saldo antes de processar ordens de compra
- [ ] Múltiplos tópicos Kafka (orders, processed, rejected)
- [ ] Containerizar API e Consumer no Docker