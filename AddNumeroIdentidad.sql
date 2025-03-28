-- Script para agregar la columna NumeroIdentidad a la tabla Clientes
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Clientes') AND name = 'NumeroIdentidad')
    BEGIN
        -- Agregar la columna NumeroIdentidad si no existe
        ALTER TABLE Clientes ADD NumeroIdentidad NVARCHAR(20) NULL;
        PRINT 'Columna NumeroIdentidad agregada correctamente a la tabla Clientes';
    END
    ELSE
    BEGIN
        PRINT 'La columna NumeroIdentidad ya existe en la tabla Clientes';
    END
END
ELSE
BEGIN
    PRINT 'La tabla Clientes no existe en la base de datos';
END
