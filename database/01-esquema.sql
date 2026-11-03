-- GameVault - Esquema de base de datos
-- La base de datos se gestiona manualmente ejecutando este script.
-- ATENCIÓN: este script borra la tabla Games y todos sus datos.
-- Las fechas se almacenan como TEXT con formato 'YYYY-MM-DD HH:MM:SS' en UTC.

PRAGMA foreign_keys = ON;

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