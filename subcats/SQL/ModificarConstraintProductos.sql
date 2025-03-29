-- Primero, identificar el nombre exacto de la restricción
-- (aunque ya lo tenemos del mensaje de error: FK__DetallesO__Id_pr__7E37BEF6)

-- Eliminar la restricción existente
ALTER TABLE DetallesOrden
DROP CONSTRAINT FK__DetallesO__Id_pr__7E37BEF6;

-- Crear la restricción nuevamente con CASCADE DELETE
ALTER TABLE DetallesOrden
ADD CONSTRAINT FK_DetallesOrden_Productos
FOREIGN KEY (Id_producto) REFERENCES productos(id_producto)
ON DELETE CASCADE;
