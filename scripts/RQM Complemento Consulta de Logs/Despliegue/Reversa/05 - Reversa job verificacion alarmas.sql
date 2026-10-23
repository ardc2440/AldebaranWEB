/* =====================================================================================
   RQM Complemento Consulta de Logs - REVERSA de "05 - Job verificacion alarmas log.sql"
   Restaura los comandos originales de los pasos del job desde RQM_LOGS_OBJECT_BACKUP.
   ===================================================================================== */
SET NOCOUNT ON;

DECLARE @JobName SYSNAME = N'Verificacion Alarmas';
DECLARE @JobId UNIQUEIDENTIFIER = (SELECT job_id FROM msdb.dbo.sysjobs WHERE name = @JobName);
IF @JobId IS NULL
BEGIN
    PRINT 'El job no existe en este servidor.';
    RETURN;
END

DECLARE @StepName SYSNAME, @Command NVARCHAR(MAX), @StepId INT;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT SUBSTRING(OBJECT_NAME, LEN(N'JOBSTEP|' + @JobName + N'|') + 1, 200), DEFINITION
      FROM dbo.RQM_LOGS_OBJECT_BACKUP
     WHERE OBJECT_TYPE = 'JOBSTEP' AND OBJECT_NAME LIKE N'JOBSTEP|' + @JobName + N'|%';
OPEN c; FETCH NEXT FROM c INTO @StepName, @Command;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @StepId = NULL;
    SELECT @StepId = step_id FROM msdb.dbo.sysjobsteps WHERE job_id = @JobId AND step_name = @StepName;
    IF @StepId IS NOT NULL
        EXEC msdb.dbo.sp_update_jobstep @job_id = @JobId, @step_id = @StepId, @command = @Command;
    PRINT 'Restaurado: ' + @StepName;
    FETCH NEXT FROM c INTO @StepName, @Command;
END
CLOSE c; DEALLOCATE c;
