/* =====================================================================================
   RQM Complemento Consulta de Logs - T2a: Ofuscar contraseñas existentes en log.track
   -------------------------------------------------------------------------------------
   - Masivo sobre TODA la tabla log.track (sin IDs ni llaves fijas): aplica a la cantidad
     de registros que tenga cada ambiente.
   - Entidades: NotificationProviderSetting (JSON en "Settings": claves que contengan
     password / pwd / secret / token), FtpWritingConnection y InventoryAutomationConnection
     (propiedad "Password").
   - Por lotes de 5.000 filas para no bloquear la escritura del Track.
   - Re-ejecutable: solo toca valores que aún no están ofuscados.
   - Mismo formato que escribe la aplicación (TrackValueMasker.MaskedValue = "********").
   ===================================================================================== */
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Mask nvarchar(20) = N'********';
DECLARE @Batch int = 5000;
DECLARE @Rows int, @Total int = 0, @Key nvarchar(200), @Path nvarchar(260);

/* ---------- 1. NotificationProviderSetting: claves sensibles dentro de Settings ---------- */
WHILE 1 = 1
BEGIN
    SET @Key = NULL;

    SELECT TOP (1) @Key = j.[key]
    FROM log.track t
    CROSS APPLY (SELECT JSON_VALUE(CAST(t.DATA_LOG AS nvarchar(max)), '$.Settings') AS Settings) s
    CROSS APPLY OPENJSON(s.Settings) j
    WHERE t.ENTITY_NAME = N'NotificationProviderSetting'
      AND ISJSON(s.Settings) = 1
      AND (j.[key] LIKE N'%password%' OR j.[key] LIKE N'%pwd%' OR j.[key] LIKE N'%secret%' OR j.[key] LIKE N'%token%')
      AND j.[type] = 1                                   -- valor de tipo texto
      AND j.[value] NOT LIKE N'********%';

    IF @Key IS NULL BREAK;

    SET @Path = N'$."' + REPLACE(@Key, N'"', N'\"') + N'"';

    WHILE 1 = 1
    BEGIN
        UPDATE TOP (@Batch) t
        -- CONCAT(N'', ...) conserva Settings como TEXTO (igual que lo escribe la aplicación);
        -- sin él, JSON_MODIFY lo insertaría como objeto JSON.
        SET DATA_LOG = CAST(JSON_MODIFY(x.DataLog, '$.Settings', CONCAT(N'', JSON_MODIFY(x.Settings, @Path, @Mask))) AS varchar(max))
        FROM log.track t
        CROSS APPLY (SELECT CAST(t.DATA_LOG AS nvarchar(max)) AS DataLog) d
        CROSS APPLY (SELECT d.DataLog, JSON_VALUE(d.DataLog, '$.Settings') AS Settings) x
        WHERE t.ENTITY_NAME = N'NotificationProviderSetting'
          AND ISJSON(x.Settings) = 1
          AND JSON_VALUE(x.Settings, @Path) IS NOT NULL
          AND JSON_VALUE(x.Settings, @Path) NOT LIKE N'********%';

        SET @Rows = @@ROWCOUNT;
        SET @Total += @Rows;
        IF @Rows < @Batch BREAK;
    END

    PRINT 'NotificationProviderSetting, clave "' + @Key + '" ofuscada';
END

/* ---------- 2. FtpWritingConnection / InventoryAutomationConnection: Password ---------- */
WHILE 1 = 1
BEGIN
    UPDATE TOP (@Batch) t
    SET DATA_LOG = CAST(JSON_MODIFY(d.DataLog, '$.Password', @Mask) AS varchar(max))
    FROM log.track t
    CROSS APPLY (SELECT CAST(t.DATA_LOG AS nvarchar(max)) AS DataLog) d
    WHERE t.ENTITY_NAME IN (N'FtpWritingConnection', N'InventoryAutomationConnection')
      AND ISJSON(d.DataLog) = 1
      AND JSON_VALUE(d.DataLog, '$.Password') IS NOT NULL
      AND JSON_VALUE(d.DataLog, '$.Password') NOT LIKE N'********%';

    SET @Rows = @@ROWCOUNT;
    SET @Total += @Rows;
    IF @Rows < @Batch BREAK;
END

PRINT 'Total de actualizaciones: ' + CAST(@Total AS varchar(20));

/* ---------- 3. Verificación: debe devolver 0 en "SinOfuscar" ---------- */
SELECT t.ENTITY_NAME,
       COUNT(*) AS Registros,
       SUM(s.SIN_OFUSCAR) AS SinOfuscar
FROM log.track t
OUTER APPLY (SELECT CASE WHEN
                   (t.ENTITY_NAME = N'NotificationProviderSetting' AND EXISTS (
                        SELECT 1 FROM OPENJSON(JSON_VALUE(CAST(t.DATA_LOG AS nvarchar(max)), '$.Settings')) j
                        WHERE (j.[key] LIKE N'%password%' OR j.[key] LIKE N'%pwd%' OR j.[key] LIKE N'%secret%' OR j.[key] LIKE N'%token%')
                          AND j.[type] = 1 AND j.[value] NOT LIKE N'********%'))
                OR (t.ENTITY_NAME <> N'NotificationProviderSetting'
                    AND JSON_VALUE(CAST(t.DATA_LOG AS nvarchar(max)), '$.Password') NOT LIKE N'********%')
               THEN 1 ELSE 0 END AS SIN_OFUSCAR) s
WHERE t.ENTITY_NAME IN (N'NotificationProviderSetting', N'FtpWritingConnection', N'InventoryAutomationConnection')
GROUP BY t.ENTITY_NAME;
