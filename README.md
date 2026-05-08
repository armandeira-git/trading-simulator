# Trading Simulator — Kafka + .NET + Docker

Simulador de ordens de compra e venda de ações em tempo real, inspirado no funcionamento de corretoras e sistemas financeiros como a B3.

## Tecnologias

- .NET 8 (C#)
- Apache Kafka (mensageria e eventos)
- MySQL 8 (persistência)
- Docker + Docker Compose (infraestrutura)

## Arquitetura

```
Producer (.NET) → Kafka (tópico: orders) → Consumer (.NET) → MySQL
```

- **Producer**: simula um trader enviando ordens de compra/venda
- **Kafka**: garante entrega e processamento assíncrono das ordens
- **Consumer**: processa cada ordem e persiste no banco de dados

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

### 3. Rode o Producer (terminal 2)
```bash
cd Producer
dotnet run
```

### 4. Verifique no banco
```sql
SELECT * FROM trading.orders;
```

## Estrutura do projeto

```
trading-simulator/
├── docker-compose.yml
├── Producer/
│   ├── Producer.csproj
│   └── Program.cs
├── Consumer/
│   ├── Consumer.csproj
│   └── Program.cs
└── sql/
    └── init.sql
```

## Próximos passos

- [ ] API REST com .NET Minimal API para receber ordens via HTTP
- [ ] Validação de saldo antes de processar ordens
- [ ] Múltiplos tópicos Kafka (orders, processed, rejected)
- [ ] Containerizar Producer e Consumer no Docker
