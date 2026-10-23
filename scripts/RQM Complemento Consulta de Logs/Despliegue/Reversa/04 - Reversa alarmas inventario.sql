/* =====================================================================================
   RQM Complemento Consulta de Logs - T3b: REVERSA de "04 - Alarmas inventario estandar y log.sql"
   -------------------------------------------------------------------------------------
   Restaura las definiciones originales de los 2 SP y 8 triggers desde la tabla de respaldo
   RQM_LOGS_OBJECT_BACKUP (llenada automáticamente por el script 04 la primera vez que corre).
   NO borra INVENTORY_ALARM_LOG (conserva el historial); el SP original simplemente deja de escribir.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID('dbo.RQM_LOGS_OBJECT_BACKUP') IS NULL
    THROW 50010, N'No existe el respaldo RQM_LOGS_OBJECT_BACKUP. No se puede revertir.', 1;

DECLARE @name SYSNAME, @type VARCHAR(10), @def NVARCHAR(MAX), @drop NVARCHAR(400);

DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT OBJECT_NAME, LTRIM(RTRIM(OBJECT_TYPE)), DEFINITION FROM dbo.RQM_LOGS_OBJECT_BACKUP WHERE OBJECT_TYPE <> 'JOBSTEP';
OPEN c; FETCH NEXT FROM c INTO @name, @type, @def;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @drop = CASE @type WHEN 'P' THEN N'DROP PROCEDURE IF EXISTS dbo.' ELSE N'DROP TRIGGER IF EXISTS dbo.' END + QUOTENAME(@name);
    EXEC sys.sp_executesql @drop;
    EXEC sys.sp_executesql @def;
    PRINT 'Restaurado: ' + @name;
    FETCH NEXT FROM c INTO @name, @type, @def;
END
CLOSE c; DEALLOCATE c;

SELECT b.OBJECT_NAME, CASE WHEN m.definition = b.DEFINITION THEN 'Restaurado' ELSE 'DIFERENTE' END AS ESTADO
  FROM dbo.RQM_LOGS_OBJECT_BACKUP b
  LEFT JOIN sys.sql_modules m ON m.object_id = OBJECT_ID('dbo.' + b.OBJECT_NAME)
 WHERE b.OBJECT_TYPE <> 'JOBSTEP';
