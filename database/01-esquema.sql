-- GameVault - Esquema de base de datos
-- La base de datos se gestiona manualmente ejecutando este script.
-- ATENCIÓN: este script borra las tablas de órdenes, clientes y juegos, y todos sus datos.
-- Las fechas se almacenan como TEXT con formato 'YYYY-MM-DD HH:MM:SS' en UTC.

PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Clients;
DROP TABLE IF EXISTS Games;

CREATE TABLE Games (
    Id TEXT PRIMARY KEY,
    Title TEXT NOT NULL COLLATE NOCASE,
    Genre TEXT NOT NULL,
    Price DECIMAL NOT NULL CHECK (Price >= 0 AND Price <= 500),
    Status INTEGER NOT NULL CHECK (Status IN (0, 1, 2)),
    CreatedAt TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    UpdatedAt TEXT NULL,
    UpdatedBy TEXT NULL,
    CONSTRAINT UQ_Games_Title UNIQUE (Title)
);

CREATE TABLE Clients (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL COLLATE NOCASE,
    Active INTEGER NOT NULL DEFAULT 1 CHECK (Active IN (0, 1)),
    CreatedAt TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    UpdatedAt TEXT NULL,
    UpdatedBy TEXT NULL,
    CONSTRAINT UQ_Clients_Email UNIQUE (Email)
);

CREATE TABLE Orders (
    Id TEXT PRIMARY KEY,
    ClientId TEXT NOT NULL,
    Status INTEGER NOT NULL DEFAULT 0 CHECK (Status IN (0, 1, 2)),
    CreatedAt TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    UpdatedAt TEXT NULL,
    UpdatedBy TEXT NULL,
    CONSTRAINT FK_Orders_Clients FOREIGN KEY (ClientId)
        REFERENCES Clients (Id)
);

CREATE TABLE OrderItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId TEXT NOT NULL,
    GameId TEXT NOT NULL,
    UnitPrice DECIMAL NOT NULL CHECK (UnitPrice >= 0 AND UnitPrice <= 500),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders (Id),
    CONSTRAINT FK_OrderItems_Games FOREIGN KEY (GameId)
        REFERENCES Games (Id),
    CONSTRAINT UQ_OrderItems_Order_Game UNIQUE (OrderId, GameId)
);

CREATE INDEX IX_Orders_ClientId ON Orders (ClientId);
CREATE INDEX IX_OrderItems_OrderId ON OrderItems (OrderId);