CREATE TABLE IF NOT EXISTS Games (
    Id TEXT PRIMARY KEY,
    Title TEXT NOT NULL,
    Genre TEXT NOT NULL,
    Price DECIMAL NOT NULL,
    Status INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    UpdatedAt TEXT NULL,
    UpdatedBy TEXT NULL
);