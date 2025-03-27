-- Tabla para almacenar los datos del cliente y la orden
CREATE TABLE IF NOT EXISTS Clientes (
    Id_cliente INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    Direccion NVARCHAR(200) NOT NULL,
    Fecha_Creacion DATETIME DEFAULT GETDATE(),
    Fecha_Actualizacion DATETIME DEFAULT GETDATE()
);

-- Tabla para almacenar las órdenes
CREATE TABLE IF NOT EXISTS Ordenes (
    Id_orden INT PRIMARY KEY IDENTITY(1,1),
    Id_cliente INT NOT NULL,
    Fecha_Orden DATETIME DEFAULT GETDATE(),
    Total DECIMAL(10, 2) NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'Pendiente', -- Pendiente, Procesada, Cancelada, etc.
    Fecha_Creacion DATETIME DEFAULT GETDATE(),
    Fecha_Actualizacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Id_cliente) REFERENCES Clientes(Id_cliente)
);

-- Tabla para almacenar los detalles de la orden
CREATE TABLE IF NOT EXISTS DetallesOrden (
    Id_detalle INT PRIMARY KEY IDENTITY(1,1),
    Id_orden INT NOT NULL,
    Id_producto INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio_Unitario DECIMAL(10, 2) NOT NULL,
    Subtotal DECIMAL(10, 2) NOT NULL,
    Fecha_Creacion DATETIME DEFAULT GETDATE(),
    Fecha_Actualizacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Id_orden) REFERENCES Ordenes(Id_orden),
    FOREIGN KEY (Id_producto) REFERENCES Productos(Id_producto)
);

-- Trigger para actualizar la fecha de actualización en Clientes
CREATE TRIGGER IF NOT EXISTS TR_Clientes_Update
ON Clientes
AFTER UPDATE
AS
BEGIN
    UPDATE Clientes
    SET Fecha_Actualizacion = GETDATE()
    FROM Clientes c
    INNER JOIN inserted i ON c.Id_cliente = i.Id_cliente;
END;

-- Trigger para actualizar la fecha de actualización en Ordenes
CREATE TRIGGER IF NOT EXISTS TR_Ordenes_Update
ON Ordenes
AFTER UPDATE
AS
BEGIN
    UPDATE Ordenes
    SET Fecha_Actualizacion = GETDATE()
    FROM Ordenes o
    INNER JOIN inserted i ON o.Id_orden = i.Id_orden;
END;

-- Trigger para actualizar la fecha de actualización en DetallesOrden
CREATE TRIGGER IF NOT EXISTS TR_DetallesOrden_Update
ON DetallesOrden
AFTER UPDATE
AS
BEGIN
    UPDATE DetallesOrden
    SET Fecha_Actualizacion = GETDATE()
    FROM DetallesOrden d
    INNER JOIN inserted i ON d.Id_detalle = i.Id_detalle;
END;

-- Trigger para actualizar el stock de productos después de una orden
CREATE TRIGGER IF NOT EXISTS TR_DetallesOrden_Insert
ON DetallesOrden
AFTER INSERT
AS
BEGIN
    UPDATE Productos
    SET Stock = p.Stock - i.Cantidad,
        Fecha_Actualizacion = GETDATE()
    FROM Productos p
    INNER JOIN inserted i ON p.Id_producto = i.Id_producto;
END;
