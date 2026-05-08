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