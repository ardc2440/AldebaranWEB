/* =====================================================================================
   RQM Complemento Consulta de Logs - T1: Pruebas de índices log.Logs y log.track
   Ejecutar DESPUÉS de "01 - Indices Log y Track.sql".
   Para cada bloque: activar "Incluir plan de ejecución real" (Ctrl+M) y verificar
   que aparezca "Index Seek" sobre el índice indicado (no "Clustered Index Scan").
   Anotar el tiempo que muestra cada PRINT.
   ===================================================================================== */
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
DECLARE @t datetime2, @n int;
DECLARE @Hasta datetime = (SELECT MAX(TimeStamp) FROM log.Logs);

-- P1: Log general, 1 día, agrupado por mensaje  -> Seek IX_Logs_TimeStamp
SET @t = SYSDATETIME();
SELECT @n = COUNT(*) FROM (
    SELECT TEMPLATE_HASH, LEVEL_KEY, Source, COUNT(*) c, MIN(TimeStamp) d1, MAX(TimeStamp) d2
    FROM log.Logs WHERE TimeStamp >= DATEADD(day, -1, @Hasta) AND TimeStamp <= @Hasta
    GROUP BY TEMPLATE_HASH, LEVEL_KEY, Source) g
OPTION (RECOMPILE);
PRINT 'P1 Logs 1 día agrupado: ' + CAST(@n AS varchar) + ' grupos, ' + CAST(DATEDIFF(ms, @t, SYSDATETIME()) AS varchar) + ' ms';

-- P2: Log general, 1 año, agrupado, solo errores  -> Seek IX_Logs_TimeStamp
SET @t = SYSDATETIME();
SELECT @n = COUNT(*) FROM (
    SELECT TEMPLATE_HASH, Source, COUNT(*) c
    FROM log.Logs WHERE TimeStamp >= DATEADD(year, -1, @Hasta) AND TimeStamp <= @Hasta AND LEVEL_KEY = N'Error'
    GROUP BY TEMPLATE_HASH, Source) g
OPTION (RECOMPILE);
PRINT 'P2 Logs 1 año errores agrupado: ' + CAST(@n AS varchar) + ' grupos, ' + CAST(DATEDIFF(ms, @t, SYSDATETIME()) AS varchar) + ' ms';

-- P3: Ocurrencias de un grupo (página de 20)  -> Seek IX_Logs_Template_TimeStamp
DECLARE @h binary(32) = (SELECT TOP 1 TEMPLATE_HASH FROM log.Logs WHERE TimeStamp >= DATEADD(day, -1, @Hasta) ORDER BY TimeStamp DESC);
SET @t = SYSDATETIME();
SELECT Id, TimeStamp, LEVEL_KEY, Source FROM log.Logs
WHERE TEMPLATE_HASH = @h AND TimeStamp >= DATEADD(year, -1, @Hasta) AND TimeStamp <= @Hasta
ORDER BY TimeStamp DESC OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY OPTION (RECOMPILE);
PRINT 'P3 Ocurrencias de un grupo: ' + CAST(DATEDIFF(ms, @t, SYSDATETIME()) AS varchar) + ' ms';

-- P4: Track, entidad + 1 mes (página de 20)  -> Seek IX_track_Entity_Date
DECLARE @Ent nvarchar(200) = N'ItemReference';
DECLARE @HastaT datetime = (SELECT MAX(MODIFIED_DATE) FROM log.track WHERE ENTITY_NAME = @Ent);
SET @t = SYSDATETIME();
SELECT ID, MODIFIED_DATE, ACTION, MODIFIER_NAME FROM log.track
WHERE ENTITY_NAME = @Ent AND MODIFIED_DATE >= DATEADD(month, -1, @HastaT) AND MODIFIED_DATE <= @HastaT
ORDER BY MODIFIED_DATE DESC OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY OPTION (RECOMPILE);
PRINT 'P4 Track entidad 1 mes: ' + CAST(DATEDIFF(ms, @t, SYSDATETIME()) AS varchar) + ' ms';

-- P5: Track, registro anterior de la misma llave (diff)  -> Seek IX_track_Entity_Key
DECLARE @Id bigint, @Kh binary(32), @Fecha datetime;
SELECT TOP 1 @Id = ID, @Kh = ENTITY_KEY_HASH, @Fecha = MODIFIED_DATE
FROM log.track WHERE ENTITY_NAME = @Ent AND ACTION = 'Modified' ORDER BY ID DESC;
SET @t = SYSDATETIME();
SELECT TOP 1 ID, MODIFIED_DATE FROM log.track
WHERE ENTITY_NAME = @Ent AND ENTITY_KEY_HASH = @Kh AND (MODIFIED_DATE < @Fecha OR (MODIFIED_DATE = @Fecha AND ID < @Id))
ORDER BY MODIFIED_DATE DESC, ID DESC;
PRINT 'P5 Track registro anterior: ' + CAST(DATEDIFF(ms, @t, SYSDATETIME()) AS varchar) + ' ms';
GO

/* P6 (en la aplicación, no en SSMS): crear y modificar un pedido en Aldebaran.Web y luego ejecutar:
   - Debe haber filas nuevas en track y Logs con la hora de la prueba (las inserciones no fallan).
   - El servicio NotificationProcessor y FileWritingService siguen escribiendo en log.Logs. */
SELECT TOP 5 ID, ENTITY_NAME, ACTION, MODIFIED_DATE, ENTITY_KEY_HASH FROM log.track ORDER BY ID DESC;
SELECT Source, MAX(TimeStamp) UltimoRegistro FROM log.Logs GROUP BY Source;
GO
