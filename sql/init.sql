CREATE DATABASE IF NOT EXISTS trading;
USE trading;

CREATE TABLE orders (
  id          INT AUTO_INCREMENT PRIMARY KEY,
  ticker      VARCHAR(10)    NOT NULL,
  type        ENUM('BUY','SELL') NOT NULL,
  quantity    INT            NOT NULL,
  price       DECIMAL(10,2)  NOT NULL,
  status      VARCHAR(20)    DEFAULT 'PROCESSED',
  created_at  DATETIME       DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS wallets (
  id         INT AUTO_INCREMENT PRIMARY KEY,
  owner      VARCHAR(50)    NOT NULL UNIQUE,
  balance    DECIMAL(10,2)  NOT NULL DEFAULT 0.00
);

-- Cria uma carteira de teste com R$ 10.000
INSERT INTO wallets (owner, balance) VALUES ('trader1', 10000.00);