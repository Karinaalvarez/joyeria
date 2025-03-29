-- Agregar campo Activo a la tabla productos si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('productos') AND name = 'Activo')
BEGIN
    ALTER TABLE productos ADD Activo BIT NOT NULL DEFAULT 1;
END
