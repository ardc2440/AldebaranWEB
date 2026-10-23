/* =====================================================================================
   RQM Complemento Consulta de Logs - T2: Pruebas de Track (ofuscación + entidades nuevas)
   Ejecutar después de hacer en Aldebaran.Web las operaciones de la lista de pruebas.
   @Desde = hora de inicio de la prueba.
   ===================================================================================== */
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
DECLARE @Desde datetime = DATEADD(hour, -2, GETDATE());   -- ajustar a la hora de inicio de la prueba

/* P1: registros generados por entidad y acción durante la prueba.
       Cada entidad probada debe aparecer con la acción esperada. */
SELECT ENTITY_NAME, ACTION, COUNT(*) AS Registros, MIN(MODIFIED_DATE) AS Primero, MAX(MODIFIED_DATE) AS Ultimo
FROM log.track
WHERE MODIFIED_DATE >= @Desde
GROUP BY ENTITY_NAME, ACTION
ORDER BY ENTITY_NAME, ACTION;

/* P2: entidades con credenciales: NUNCA debe verse la contraseña.
       Password / password = "********" o "******** (modificada)". */
SELECT ID, ENTITY_NAME, ACTION, MODIFIED_DATE,
       JSON_VALUE(CAST(DATA_LOG AS nvarchar(max)), '$.Password') AS Password,
       JSON_VALUE(JSON_VALUE(CAST(DATA_LOG AS nvarchar(max)), '$.Settings'), '$.password') AS SettingsPassword,
       CAST(DATA_LOG AS nvarchar(max)) AS DataLog
FROM log.track
WHERE ENTITY_NAME IN (N'NotificationProviderSetting', N'FtpWritingConnection', N'InventoryAutomationConnection')
  AND MODIFIED_DATE >= @Desde
ORDER BY ID DESC;

/* P3: histórico completo sin contraseñas en claro (debe dar 0). */
SELECT COUNT(*) AS SinOfuscar
FROM log.track t
WHERE (t.ENTITY_NAME = N'NotificationProviderSetting' AND EXISTS (
          SELECT 1 FROM OPENJSON(JSON_VALUE(CAST(t.DATA_LOG AS nvarchar(max)), '$.Settings')) j
          WHERE (j.[key] LIKE N'%password%' OR j.[key] LIKE N'%pwd%' OR j.[key] LIKE N'%secret%' OR j.[key] LIKE N'%token%')
            AND j.[type] = 1 AND j.[value] NOT LIKE N'********%'))
   OR (t.ENTITY_NAME IN (N'FtpWritingConnection', N'InventoryAutomationConnection')
       AND JSON_VALUE(CAST(t.DATA_LOG AS nvarchar(max)), '$.Password') NOT LIKE N'********%');

/* P4: sin errores nuevos de la aplicación al guardar. */
SELECT TimeStamp, Source, LEFT(Message, 300) AS Mensaje
FROM log.Logs
WHERE TimeStamp >= @Desde AND LEVEL_KEY = N'Error'
ORDER BY TimeStamp DESC;
