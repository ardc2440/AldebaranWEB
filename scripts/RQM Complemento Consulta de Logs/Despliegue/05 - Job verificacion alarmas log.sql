/* =====================================================================================
   RQM Complemento Consulta de Logs - T3c: Job "Verificacion Alarmas" registra su depuración
   -------------------------------------------------------------------------------------
   El job (diario 6:00 a. m.) recalcula las alarmas con @JOB = 1 y luego inactiva con un UPDATE
   directo las alarmas cuyo artículo/referencia ya no califica. Ese UPDATE no pasaba por los SP y
   no quedaba en INVENTORY_ALARM_LOG.

   Cambio: los pasos "Ejecucion Cantidades Minimas" y "Ejecucion Cantidades sin Inventario"
   conservan exactamente la misma lógica y agregan el registro de esas inactivaciones
   (origen "Proceso programado - depuración").

   - Requiere permisos sobre msdb (sysadmin o SQLAgentOperatorRole + dueño del job).
   - Respalda los comandos originales en RQM_LOGS_OBJECT_BACKUP (OBJECT_TYPE = 'JOBSTEP').
   - Si el job no existe en el servidor, no hace nada.
   - Ejecutar después de 04. Reversa: "Reversa\05 - Reversa job verificacion alarmas.sql".
   Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;

DECLARE @JobName SYSNAME = N'Verificacion Alarmas';
DECLARE @JobId UNIQUEIDENTIFIER = (SELECT job_id FROM msdb.dbo.sysjobs WHERE name = @JobName);

IF @JobId IS NULL
BEGIN
    PRINT 'El job "Verificacion Alarmas" no existe en este servidor. No se hace nada.';
    RETURN;
END

/* Respaldo de los comandos originales (solo la primera vez) */
INSERT INTO dbo.RQM_LOGS_OBJECT_BACKUP (OBJECT_NAME, OBJECT_TYPE, DEFINITION)
SELECT N'JOBSTEP|' + @JobName + N'|' + s.step_name, 'JOBSTEP', s.command
  FROM msdb.dbo.sysjobsteps s
 WHERE s.job_id = @JobId
   AND s.step_name IN (N'Ejecucion Cantidades Minimas', N'Ejecucion Cantidades sin Inventario')
   AND NOT EXISTS (SELECT 1 FROM dbo.RQM_LOGS_OBJECT_BACKUP b WHERE b.OBJECT_NAME = N'JOBSTEP|' + @JobName + N'|' + s.step_name);

DECLARE @StepId INT;

SELECT @StepId = step_id FROM msdb.dbo.sysjobsteps WHERE job_id = @JobId AND step_name = N'Ejecucion Cantidades Minimas';
IF @StepId IS NOT NULL
    EXEC msdb.dbo.sp_update_jobstep @job_id = @JobId, @step_id = @StepId, @command = N'DECLARE @References TABLE (IdReg INT NOT NULL IDENTITY(1,1) PRIMARY KEY, REFERENCE_ID INT)
DECLARE @Reference_Id INT, @IdReg INT, @TotalReg INT

INSERT INTO @References (REFERENCE_ID)
SELECT REFERENCE_ID
  FROM items a WITH (NOLOCK)
  JOIN item_references b WITH (NOLOCK) ON b.ITEM_ID = a.ITEM_ID
 WHERE a.IS_ACTIVE = 1 /* Articulos Activos */
   AND a.IS_SALE_OFF = 0 /* Articulos que No Son Oferta */
   AND a.IS_SPECIAL_IMPORT = 0 /* Articulos que No Son Importacion Especial */
   AND a.IS_DOMESTIC_PRODUCT = 0 /* No es producto nacional */
   AND b.IS_ACTIVE = 1 /* Referencias Activas */
   AND b.ALARM_MINIMUM_QUANTITY > 0
   AND a.DISABLE_ALARMS = 0

SELECT @IdReg = MIN(IdReg), @TotalReg = MAX(IdReg)
  FROM @References

WHILE @IdReg <= @TotalReg
BEGIN
	SELECT @Reference_Id = reference_id from @References where IdReg = @IdReg
	exec SP_Set_Minimum_Quantity_Alarm @Reference_Id, 1
	SET @IdReg = @IdReg + 1
END

/* RQM Complemento Consulta de Logs: la depuración se registra en INVENTORY_ALARM_LOG (misma lógica) */
DECLARE @Inactivated TABLE (ALARM_ID INT, REFERENCE_ID INT, PREVIOUS_QUANTITY INT, NEW_QUANTITY INT, PREVIOUS_MINIMUM_QUANTITY INT, NEW_MINIMUM_QUANTITY INT,
                            PREVIOUS_CREATION_DATE DATETIME, NEW_CREATION_DATE DATETIME, PREVIOUS_ACTIVE BIT, NEW_ACTIVE BIT, INACTIVATION_DATE DATETIME)

UPDATE a
   SET a.ACTIVE = 0,
       a.INACTIVATION_DATE = GETDATE()
OUTPUT inserted.MINIMUM_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
       deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE
  INTO @Inactivated
  FROM minimum_quantity_alarms a
  JOIN item_references b ON b.REFERENCE_ID = a.REFERENCE_ID
  JOIN items c ON c.ITEM_ID = b.ITEM_ID
 WHERE a.ACTIVE = 1
   AND (c.IS_ACTIVE = 0
        OR b.IS_ACTIVE = 0
        OR c.IS_DOMESTIC_PRODUCT = 1
        OR c.IS_SALE_OFF = 1
        OR c.IS_SPECIAL_IMPORT = 1
        OR c.DISABLE_ALARMS = 1
        OR B.ALARM_MINIMUM_QUANTITY <= 0)

INSERT INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
       PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
       PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
       VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
SELECT ''MINIMUM_QUANTITY'', i.ALARM_ID, i.REFERENCE_ID, ''I'',
       i.PREVIOUS_QUANTITY, i.NEW_QUANTITY, i.PREVIOUS_MINIMUM_QUANTITY, i.NEW_MINIMUM_QUANTITY,
       i.PREVIOUS_CREATION_DATE, i.NEW_CREATION_DATE, i.PREVIOUS_ACTIVE, i.NEW_ACTIVE, i.INACTIVATION_DATE,
       (SELECT COUNT(*) FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = i.ALARM_ID), 0, ''Proceso programado - depuración''
  FROM @Inactivated i';

SET @StepId = NULL;
SELECT @StepId = step_id FROM msdb.dbo.sysjobsteps WHERE job_id = @JobId AND step_name = N'Ejecucion Cantidades sin Inventario';
IF @StepId IS NOT NULL
    EXEC msdb.dbo.sp_update_jobstep @job_id = @JobId, @step_id = @StepId, @command = N'DECLARE @References TABLE (IdReg INT NOT NULL IDENTITY(1,1) PRIMARY KEY, REFERENCE_ID INT)
DECLARE @Reference_Id INT, @IdReg INT, @TotalReg INT

INSERT INTO @References (REFERENCE_ID)
SELECT REFERENCE_ID
  FROM items a WITH (NOLOCK)
  JOIN item_references b WITH (NOLOCK) ON b.ITEM_ID = a.ITEM_ID
 WHERE a.IS_ACTIVE = 1 /* Articulos Activos */
   AND a.IS_SALE_OFF = 0 /* Articulos que No Son Oferta */
   AND a.IS_SPECIAL_IMPORT = 0 /* Articulos que No Son Importacion Especial */
   AND b.IS_ACTIVE = 1 /* Referencias Activas */
   AND a.IS_DOMESTIC_PRODUCT = 0
   AND b.ALARM_MINIMUM_QUANTITY <= 0
   AND a.DISABLE_ALARMS = 0

SELECT @IdReg = MIN(IdReg), @TotalReg = MAX(IdReg)
  FROM @References

WHILE @IdReg <= @TotalReg
BEGIN
	SELECT @Reference_Id = reference_id from @References where IdReg = @IdReg
	exec SP_Set_Out_Of_Stock_Alarm @Reference_Id, 1
	SET @IdReg = @IdReg + 1
END

/* RQM Complemento Consulta de Logs: la depuración se registra en INVENTORY_ALARM_LOG (misma lógica) */
DECLARE @Inactivated TABLE (ALARM_ID INT, REFERENCE_ID INT, PREVIOUS_QUANTITY INT, NEW_QUANTITY INT, PREVIOUS_MINIMUM_QUANTITY INT, NEW_MINIMUM_QUANTITY INT,
                            PREVIOUS_CREATION_DATE DATETIME, NEW_CREATION_DATE DATETIME, PREVIOUS_ACTIVE BIT, NEW_ACTIVE BIT, INACTIVATION_DATE DATETIME)

UPDATE a
   SET a.ACTIVE = 0,
       a.INACTIVATION_DATE = GETDATE()
OUTPUT inserted.OUT_OF_STOCK_INVENTORY_ALARM_ID, inserted.REFERENCE_ID, deleted.QUANTITY, inserted.QUANTITY, NULL, NULL,
       deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE
  INTO @Inactivated
  FROM out_of_stock_inventory_alarms a
  JOIN item_references b ON b.REFERENCE_ID = a.REFERENCE_ID
  JOIN items c ON c.ITEM_ID = b.ITEM_ID
 WHERE a.ACTIVE = 1
   AND (c.IS_ACTIVE = 0
        OR b.IS_ACTIVE = 0
        OR c.IS_DOMESTIC_PRODUCT = 1
        OR c.IS_SALE_OFF = 1
        OR c.IS_SPECIAL_IMPORT = 1
        OR c.DISABLE_ALARMS = 1
        OR B.ALARM_MINIMUM_QUANTITY > 0)

INSERT INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
       PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
       PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
       VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
SELECT ''OUT_OF_STOCK'', i.ALARM_ID, i.REFERENCE_ID, ''I'',
       i.PREVIOUS_QUANTITY, i.NEW_QUANTITY, i.PREVIOUS_MINIMUM_QUANTITY, i.NEW_MINIMUM_QUANTITY,
       i.PREVIOUS_CREATION_DATE, i.NEW_CREATION_DATE, i.PREVIOUS_ACTIVE, i.NEW_ACTIVE, i.INACTIVATION_DATE,
       (SELECT COUNT(*) FROM visualized_out_of_stock_inventory_alarms v WHERE v.OUT_OF_STOCK_INVENTORY_ALARM_ID = i.ALARM_ID), 0, ''Proceso programado - depuración''
  FROM @Inactivated i';

SELECT s.step_id, s.step_name, CASE WHEN s.command LIKE N'%INVENTORY_ALARM_LOG%' THEN 'Actualizado' ELSE 'SIN CAMBIO' END AS ESTADO
  FROM msdb.dbo.sysjobsteps s
 WHERE s.job_id = @JobId
 ORDER BY s.step_id;
