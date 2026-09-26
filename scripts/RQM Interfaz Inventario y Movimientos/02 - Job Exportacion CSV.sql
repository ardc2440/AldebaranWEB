/* =====================================================================================
   RQM  : Interfaz para Consulta de Inventario y Movimientos - FASE 1 (Exportación CSV)
   Job  : Aldebaran - Exportacion CSV Inventario y Movimientos
   Pasos: 1. Pedidos -> 2. Inventario -> 3. Ordenes de Compra -> 4. Notificación por correo
          Si un paso de generación falla, el Job continúa con el siguiente (cada listado tiene su
          propia marca de última ejecución) y el paso 4 siempre notifica el resultado de los 3.
   Programación por defecto: diario 02:00 a.m. (ajustable en las propiedades del Job).
   ===================================================================================== */
USE msdb
GO

DECLARE @JobName  SYSNAME = N'Aldebaran - Exportacion CSV Inventario y Movimientos';
DECLARE @Database SYSNAME = N'Aldebaran';
DECLARE @JobId    UNIQUEIDENTIFIER;

BEGIN TRANSACTION;

IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = @JobName)
	EXEC msdb.dbo.sp_delete_job @job_name = @JobName, @delete_unused_schedule = 1;

EXEC msdb.dbo.sp_add_job
	@job_name = @JobName,
	@enabled = 1,
	@description = N'Genera los CSV de Pedidos (novedades), Inventario (fotografía) y Órdenes de Compra (novedades) en la ruta CSV_EXPORT_STAGING_PATH de SYSTEM_PARAMETERS.',
	@job_id = @JobId OUTPUT;

EXEC msdb.dbo.sp_add_jobstep @job_id = @JobId, @step_id = 1, @step_name = N'CSV Pedidos',
	@subsystem = N'TSQL', @database_name = @Database,
	@command = N'EXEC dbo.SP_CSV_EXPORT_CUSTOMER_ORDERS',
	@on_success_action = 3, @on_fail_action = 3;

EXEC msdb.dbo.sp_add_jobstep @job_id = @JobId, @step_id = 2, @step_name = N'CSV Inventario',
	@subsystem = N'TSQL', @database_name = @Database,
	@command = N'EXEC dbo.SP_CSV_EXPORT_INVENTORY',
	@on_success_action = 3, @on_fail_action = 3;

EXEC msdb.dbo.sp_add_jobstep @job_id = @JobId, @step_id = 3, @step_name = N'CSV Ordenes de Compra',
	@subsystem = N'TSQL', @database_name = @Database,
	@command = N'EXEC dbo.SP_CSV_EXPORT_PURCHASE_ORDERS',
	@on_success_action = 3, @on_fail_action = 3;

EXEC msdb.dbo.sp_add_jobstep @job_id = @JobId, @step_id = 4, @step_name = N'Notificacion por correo',
	@subsystem = N'TSQL', @database_name = @Database,
	@command = N'EXEC dbo.SP_CSV_EXPORT_NOTIFY
	@JOB_ID     = $(ESCAPE_NONE(JOBID)),
	@START_DATE = $(ESCAPE_NONE(STRTDT)),
	@START_TIME = $(ESCAPE_NONE(STRTTM));',
	@on_success_action = 1, @on_fail_action = 2;

EXEC msdb.dbo.sp_update_job @job_id = @JobId, @start_step_id = 1;

EXEC msdb.dbo.sp_add_jobschedule @job_id = @JobId,
	@name = N'Diario 02:00',
	@enabled = 1,
	@freq_type = 4, @freq_interval = 1,
	@active_start_time = 20000;

EXEC msdb.dbo.sp_add_jobserver @job_id = @JobId, @server_name = N'(local)';

COMMIT TRANSACTION;
GO
