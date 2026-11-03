-- GameVault - Datos iniciales
-- Ejecutar manualmente después de 01-esquema.sql.

INSERT OR IGNORE INTO Games
    (Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES
    ('11111111-1111-4111-8111-111111111111', 'Aventura Estelar', 'Aventura', 59.99, 0, '2026-01-15 10:00:00', 'sistema', NULL, NULL),
    ('22222222-2222-4222-8222-222222222222', 'Reinos del Norte', 'Rol', 49.99, 1, '2026-01-16 11:30:00', 'sistema', '2026-02-01 08:00:00', 'sistema'),
    ('33333333-3333-4333-8333-333333333333', 'Circuito Infinito', 'Carreras', 39.50, 2, '2026-01-17 14:15:00', 'sistema', '2026-02-02 09:00:00', 'sistema'),
    ('44444444-4444-4444-8444-444444444444', 'Misterio en la Isla', 'Aventura', 0.00, 0, '2026-01-18 16:45:00', 'sistema', NULL, NULL),
    ('55555555-5555-4555-8555-555555555555', 'Combate Táctico', 'Estrategia', 500.00, 1, '2026-01-19 12:20:00', 'sistema', '2026-02-03 10:00:00', 'sistema');

INSERT OR IGNORE INTO Clients
    (Id, Name, Email, Active, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES
    ('aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa', 'Ana García', 'ana.garcia@example.com', 1, '2026-01-20 09:00:00', 'sistema', NULL, NULL),
    ('bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb', 'Bruno López', 'bruno.lopez@example.com', 0, '2026-01-21 10:30:00', 'sistema', '2026-02-04 11:00:00', 'sistema'),
    ('cccccccc-cccc-4ccc-8ccc-cccccccccccc', 'Carla Ruiz', 'carla.ruiz@example.com', 1, '2026-01-22 14:15:00', 'sistema', NULL, NULL);