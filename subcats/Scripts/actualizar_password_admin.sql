-- ============================================
-- PASO 1: Agregar columna NombreCompleto a la tabla Usuarios
-- ============================================
-- Ejecuta esto primero si no tienes la columna NombreCompleto

ALTER TABLE Usuarios ADD NombreCompleto nvarchar(100) NULL;
GO

-- ============================================
-- PASO 2: Actualizar la contraseña del admin con MD5
-- ============================================
-- La contraseña "admin123" encriptada en MD5 es: 0192023a7bbd73250516f069df18b500

UPDATE Usuarios 
SET Password = '0192023a7bbd73250516f069df18b500',  -- MD5 de "admin123"
    NombreCompleto = 'Administrador'
WHERE Username = 'admin';

-- ============================================
-- OPCIÓN ALTERNATIVA: Si el admin no existe, crearlo
-- ============================================
-- Descomenta las siguientes líneas si necesitas crear el admin desde cero

-- INSERT INTO Usuarios (NombreCompleto, Username, Password, Role)
-- VALUES ('Administrador', 'admin@joyeria.com', '0192023a7bbd73250516f069df18b500', 'Admin');

-- ============================================
-- CONTRASEÑAS DE EJEMPLO EN MD5:
-- ============================================
-- "admin123"   -> 0192023a7bbd73250516f069df18b500
-- "admin"      -> 21232f297a57a5a743894a0e4a801fc3
-- "123456"     -> e10adc3949ba59abbe56e057f20f883e
-- "password"   -> 5f4dcc3b5aa765d61d8327deb882cf99
-- ============================================

-- Para generar otros hashes MD5, puedes usar:
-- https://www.md5hashgenerator.com/
