-- Crear tabla para almacenar las temáticas
CREATE TABLE IF NOT EXISTS tematica (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500),
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    Activa BOOLEAN NOT NULL DEFAULT FALSE,
    Imagen LONGBLOB
);
