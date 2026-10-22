/* =====================================================================================
   RQM Complemento Consulta de Logs - T1: Índices para log.Logs y log.track
   Rama: RQM-ComplementoLogs+SaleOff
   -------------------------------------------------------------------------------------
   Objetivo: que las consultas de Log general y Track (filtradas por rango de fechas,
   entidad, nivel o grupo de mensaje) usen seek y nunca recorran las tablas completas.

   Diseño (no bloquear a los que escriben log):
   - Columnas calculadas NO persistidas: agregarlas es solo metadata (instantáneo, no
     reescribe la tabla). El valor se materializa únicamente dentro de los índices.
     Serilog y AldebaranDbContext.TrackEntities no las conocen ni las escriben.
   - Índices ONLINE cuando la edición lo permite (Enterprise/Developer/Azure); en otra
     edición se crean OFFLINE -> en producción ejecutar fuera de horario.
   - Script re-ejecutable: cada objeto se crea solo si no existe.

   Requisito de las sesiones que insertan (ya se cumple con .NET/SqlClient):
   ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL,
   QUOTED_IDENTIFIER = ON; NUMERIC_ROUNDABORT = OFF.
   ===================================================================================== */
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET NOCOUNT ON;
GO

DECLARE @Online nvarchar(20) =
    CASE WHEN CAST(SERVERPROPERTY('EngineEdition') AS int) IN (3, 5, 8) THEN N'ONLINE = ON, ' ELSE N'' END;
DECLARE @Options nvarchar(200) = N' WITH (' + @Online + N'SORT_IN_TEMPDB = ON);';
DECLARE @Sql nvarchar(max);
DECLARE @Start datetime2;

PRINT 'Modo de creación de índices: ' + CASE WHEN @Online = N'' THEN 'OFFLINE (ejecutar fuera de horario)' ELSE 'ONLINE' END;

/* ---------- log.Logs: columnas calculadas ---------- */
IF COL_LENGTH('log.Logs', 'TEMPLATE_HASH') IS NULL
BEGIN
    ALTER TABLE log.Logs ADD TEMPLATE_HASH AS CAST(HASHBYTES('SHA2_256', MessageTemplate) AS binary(32));
    PRINT 'log.Logs.TEMPLATE_HASH creada';
END

IF COL_LENGTH('log.Logs', 'LEVEL_KEY') IS NULL
BEGIN
    -- Level es nvarchar(max): se expone una versión corta para poder incluirla en índices.
    ALTER TABLE log.Logs ADD LEVEL_KEY AS CAST(LEFT(Level, 20) AS nvarchar(20));
    PRINT 'log.Logs.LEVEL_KEY creada';
END

/* ---------- log.track: columna calculada ---------- */
IF COL_LENGTH('log.track', 'ENTITY_KEY_HASH') IS NULL
BEGIN
    ALTER TABLE log.track ADD ENTITY_KEY_HASH AS CAST(HASHBYTES('SHA2_256', CAST(ENTITY_KEY AS varchar(max))) AS binary(32));
    PRINT 'log.track.ENTITY_KEY_HASH creada';
END

/* ---------- log.Logs: índices ---------- */
-- Consulta principal: rango de fechas (+ nivel, origen) y agrupación por mensaje.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('log.Logs') AND name = 'IX_Logs_TimeStamp')
BEGIN
    SET @Start = SYSDATETIME();
    SET @Sql = N'CREATE NONCLUSTERED INDEX IX_Logs_TimeStamp ON log.Logs (TimeStamp) INCLUDE (LEVEL_KEY, Source, TEMPLATE_HASH)' + @Options;
    EXEC sys.sp_executesql @Sql;
    PRINT 'IX_Logs_TimeStamp creado en ' + CAST(DATEDIFF(second, @Start, SYSDATETIME()) AS varchar(10)) + ' s';
END

-- Detalle de un grupo: ocurrencias de un mismo mensaje dentro del rango.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('log.Logs') AND name = 'IX_Logs_Template_TimeStamp')
BEGIN
    SET @Start = SYSDATETIME();
    SET @Sql = N'CREATE NONCLUSTERED INDEX IX_Logs_Template_TimeStamp ON log.Logs (TEMPLATE_HASH, TimeStamp) INCLUDE (LEVEL_KEY, Source)' + @Options;
    EXEC sys.sp_executesql @Sql;
    PRINT 'IX_Logs_Template_TimeStamp creado en ' + CAST(DATEDIFF(second, @Start, SYSDATETIME()) AS varchar(10)) + ' s';
END

/* ---------- log.track: índices ---------- */
-- Consulta principal: entidad + rango de fechas (+ acción, usuario).
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('log.track') AND name = 'IX_track_Entity_Date')
BEGIN
    SET @Start = SYSDATETIME();
    SET @Sql = N'CREATE NONCLUSTERED INDEX IX_track_Entity_Date ON log.track (ENTITY_NAME, MODIFIED_DATE) INCLUDE (ACTION, MODIFIER_NAME, ENTITY_KEY_HASH)' + @Options;
    EXEC sys.sp_executesql @Sql;
    PRINT 'IX_track_Entity_Date creado en ' + CAST(DATEDIFF(second, @Start, SYSDATETIME()) AS varchar(10)) + ' s';
END

-- Diff: registro anterior de la misma entidad y llave.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('log.track') AND name = 'IX_track_Entity_Key')
BEGIN
    SET @Start = SYSDATETIME();
    SET @Sql = N'CREATE NONCLUSTERED INDEX IX_track_Entity_Key ON log.track (ENTITY_NAME, ENTITY_KEY_HASH, MODIFIED_DATE)' + @Options;
    EXEC sys.sp_executesql @Sql;
    PRINT 'IX_track_Entity_Key creado en ' + CAST(DATEDIFF(second, @Start, SYSDATETIME()) AS varchar(10)) + ' s';
END
GO

/* ---------- Verificación ---------- */
SELECT OBJECT_SCHEMA_NAME(i.object_id) + '.' + OBJECT_NAME(i.object_id) AS Tabla, i.name AS Indice, i.type_desc
FROM sys.indexes i
WHERE i.object_id IN (OBJECT_ID('log.Logs'), OBJECT_ID('log.track'))
ORDER BY Tabla, i.index_id;
GO
