/* =====================================================================================
   RQM Complemento Consulta de Logs - T3b: Estandarización de alarmas de inventario + log
   -------------------------------------------------------------------------------------
   Ciclo de vida (definido por Andrés, 22-oct):
     - La alarma nace cuando se cumple la condición y no hay una activa para la referencia.
     - Vive mientras está activa. Si la condición se vuelve a dar, se conserva la activa con
       su estado más reciente (QUANTITY, CREATION_DATE) y se borran sus visualizaciones para
       que vuelva a salirle a todos (REFRESCO). Con @JOB = 1 no se borran (sin cambios, T3c).
     - Muere al inactivarse (INACTIVACIÓN). La siguiente será una alarma nueva.

   Cambios:
     1. Tabla INVENTORY_ALARM_LOG: registra REFRESCOS (R) e INACTIVACIONES (I) con el estado
        original y el nuevo. La creación NO se registra (la muestra el tablero).
     2. SP_Set_Minimum_Quantity_Alarm y SP_Set_Out_Of_Stock_Alarm:
        - Nuevo parámetro opcional @SOURCE (origen del evento). Llamadas actuales sin cambio.
        - H6: la cantidad mostrada se sigue calculando igual (con OC pendientes), pero una
          alarma ACTIVA solo se inactiva cuando el disponible REAL (sin OC pendientes) ya
          cumple la condición, es decir, cuando la OC se confirma y entra el inventario.
        - INSERT al log vía OUTPUT en los UPDATE existentes.
     3. Trg_Minimum_Local_warehouse_Quantity_Alarm (H1): mismo estándar (refresca y borra
        visualizaciones) + log. Evalúa solo la fila de Bodega Local.
     4. Triggers que llaman los SP: solo agregan @SOURCE.
     5. Proceso programado (job "Verificacion Alarmas", @JOB = 1): el refresco se registra solo si
        cambió la cantidad (mismo comportamiento, sin filas de ruido diarias).

   Reversa: "07b - Reversa alarmas inventario.sql" (definiciones originales).
   Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------- 0. Respaldo de definiciones originales (solo la primera vez) ---------- */
IF OBJECT_ID('dbo.RQM_LOGS_OBJECT_BACKUP') IS NULL
    CREATE TABLE dbo.RQM_LOGS_OBJECT_BACKUP
    (
        OBJECT_NAME  SYSNAME       NOT NULL CONSTRAINT PK_RQM_LOGS_OBJECT_BACKUP PRIMARY KEY,
        OBJECT_TYPE  VARCHAR(10)   NOT NULL,
        DEFINITION   NVARCHAR(MAX) NOT NULL,
        BACKUP_DATE  DATETIME      NOT NULL CONSTRAINT DF_RQM_LOGS_OBJECT_BACKUP_DATE DEFAULT (GETDATE())
    );
GO
INSERT INTO dbo.RQM_LOGS_OBJECT_BACKUP (OBJECT_NAME, OBJECT_TYPE, DEFINITION)
SELECT o.name, o.type, m.definition
  FROM sys.objects o
  JOIN sys.sql_modules m ON m.object_id = o.object_id
 WHERE o.name IN ('SP_Set_Minimum_Quantity_Alarm', 'SP_Set_Out_Of_Stock_Alarm', 'Trg_Minimum_Local_warehouse_Quantity_Alarm',
                  'Trg_Minimum_Quantity_Alarm', 'Trg_Minimum_Quantity_Alarm_Purchase_Detail', 'Trg_Minimum_Quantity_Alarm_Purchase_Order_Cancelled',
                  'Trg_Is_Special_Import', 'Trg_Out_Of_Stock_Alarm', 'Trg_Out_Of_Stock_Alarm_Purchase_Detail', 'Trg_Out_Of_Stock_Alarm_Purchase_Order_Cancelled')
   AND NOT EXISTS (SELECT 1 FROM dbo.RQM_LOGS_OBJECT_BACKUP b WHERE b.OBJECT_NAME = o.name);
GO

/* ---------- 1. Tabla de log ---------- */
IF OBJECT_ID('dbo.INVENTORY_ALARM_LOG') IS NULL
BEGIN
    CREATE TABLE dbo.INVENTORY_ALARM_LOG
    (
        INVENTORY_ALARM_LOG_ID      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_INVENTORY_ALARM_LOG PRIMARY KEY,
        ALARM_TYPE_CODE             VARCHAR(30)  NOT NULL,   -- MINIMUM_QUANTITY / MINIMUM_LOCAL_WAREHOUSE / OUT_OF_STOCK
        ALARM_ID                    INT          NOT NULL,
        REFERENCE_ID                INT          NOT NULL,
        EVENT_TYPE                  CHAR(1)      NOT NULL,   -- R = Refresco, I = Inactivación (sin CHECK: OUTPUT INTO no lo admite)
        PREVIOUS_QUANTITY           INT          NULL,
        NEW_QUANTITY                INT          NULL,
        PREVIOUS_MINIMUM_QUANTITY   INT          NULL,
        NEW_MINIMUM_QUANTITY        INT          NULL,
        PREVIOUS_CREATION_DATE      DATETIME     NULL,
        NEW_CREATION_DATE           DATETIME     NULL,
        PREVIOUS_ACTIVE             BIT          NULL,
        NEW_ACTIVE                  BIT          NULL,
        INACTIVATION_DATE           DATETIME     NULL,
        VISUALIZATION_COUNT         INT          NOT NULL,   -- visualizaciones que tenía la alarma al momento del evento
        VISUALIZATIONS_REMOVED      BIT          NOT NULL,   -- 1 = el refresco borró sus visualizaciones
        SOURCE                      VARCHAR(60)  NOT NULL,
        EVENT_DATE                  DATETIME     NOT NULL CONSTRAINT DF_INVENTORY_ALARM_LOG_EVENT_DATE DEFAULT (GETDATE())
    );
    CREATE INDEX IX_INVENTORY_ALARM_LOG_EVENT_DATE ON dbo.INVENTORY_ALARM_LOG (EVENT_DATE) INCLUDE (ALARM_TYPE_CODE, REFERENCE_ID, EVENT_TYPE);
    CREATE INDEX IX_INVENTORY_ALARM_LOG_REFERENCE ON dbo.INVENTORY_ALARM_LOG (REFERENCE_ID, EVENT_DATE);
END
GO

/* ---------- 2a. Cantidades mínimas ---------- */
CREATE OR ALTER PROCEDURE [dbo].[SP_Set_Minimum_Quantity_Alarm]
	@REFERENCE_ID INT,
	@JOB BIT = 0,
	@SOURCE VARCHAR(60) = NULL
AS
BEGIN
	DECLARE @AvailableQuantity INT, @RealAvailableQuantity INT, @MinimumQuantity INT, @IsSpecialImport BIT, @IsSaleOff BIT, @IsDomesticProduct BIT, @DisableAlarms BIT
	DECLARE @ActiveAlarmId INT, @VisualizationCount INT = 0

	SET @SOURCE = ISNULL(@SOURCE, CASE WHEN @JOB = 1 THEN 'Proceso programado' ELSE 'No identificado' END)

	SELECT @IsSpecialImport = IS_SPECIAL_IMPORT, @IsSaleOff = IS_SALE_OFF, @IsDomesticProduct = a.IS_DOMESTIC_PRODUCT, @DisableAlarms = a.DISABLE_ALARMS
	  FROM items a WITH (NOLOCK)
	  JOIN item_references b WITH (NOLOCK) ON b.ITEM_ID = a.ITEM_ID
	 WHERE b.REFERENCE_ID = @REFERENCE_ID

	IF @DisableAlarms = 0
	BEGIN
		SELECT @AvailableQuantity = CASE WHEN @IsDomesticProduct = 0 AND @IsSpecialImport = 0 AND @IsSaleOff = 0 THEN a.INVENTORY_QUANTITY - ORDERED_QUANTITY ELSE ALARM_MINIMUM_QUANTITY + 1 END,
			   @MinimumQuantity = ALARM_MINIMUM_QUANTITY
		  FROM item_references a WITH (NOLOCK)
		 WHERE a.REFERENCE_ID = @REFERENCE_ID

		/* H6: disponible real, sin OC pendientes (decide si una alarma activa sigue viva) */
		SET @RealAvailableQuantity = @AvailableQuantity

		/* Disponible mostrado: igual que siempre, suma OC pendientes */
		SELECT @AvailableQuantity = @AvailableQuantity + ISNULL(SUM(a.REQUESTED_QUANTITY), 0)
		  FROM purchase_order_details a WITH (NOLOCK)
		  JOIN purchase_orders b WITH (NOLOCK) ON b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		  JOIN status_document_types c WITH (NOLOCK) ON c.STATUS_DOCUMENT_TYPE_ID = b.STATUS_DOCUMENT_TYPE_ID
		 WHERE a.REFERENCE_ID = @REFERENCE_ID
		   AND c.STATUS_ORDER IN (1, 4)	/* Pendientes + Ajuste en aprobación (igual que producción, RQM Aprobación Ajustes OC) */
	END

	/* La referencia es de cantidad mínima: una alarma de "sin stock" activa queda sin quién la evalúe -> se inactiva */
	DECLARE @OtherAlarmId INT, @OtherVisualizationCount INT = 0
	SELECT @OtherAlarmId = OUT_OF_STOCK_INVENTORY_ALARM_ID FROM out_of_stock_inventory_alarms WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID
	   AND EXISTS (SELECT 1 FROM item_references WHERE REFERENCE_ID = @REFERENCE_ID AND ALARM_MINIMUM_QUANTITY > 0)
	IF @OtherAlarmId IS NOT NULL
	BEGIN
		SELECT @OtherVisualizationCount = COUNT(*) FROM visualized_out_of_stock_inventory_alarms WHERE OUT_OF_STOCK_INVENTORY_ALARM_ID = @OtherAlarmId
		UPDATE a
		   SET ACTIVE = 0, INACTIVATION_DATE = GETDATE()
		OUTPUT 'OUT_OF_STOCK', inserted.OUT_OF_STOCK_INVENTORY_ALARM_ID, inserted.REFERENCE_ID, 'I',
			   deleted.QUANTITY, inserted.QUANTITY, NULL, NULL,
			   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
			   @OtherVisualizationCount, 0, 'Referencia pasó a cantidad mínima'
		  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
			   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
			   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
			   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
		  FROM out_of_stock_inventory_alarms a
		 WHERE a.OUT_OF_STOCK_INVENTORY_ALARM_ID = @OtherAlarmId
	END

	SELECT @ActiveAlarmId = MINIMUM_QUANTITY_ALARM_ID
	  FROM minimum_quantity_alarms WITH (NOLOCK)
	 WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID

	IF @ActiveAlarmId IS NOT NULL
		SELECT @VisualizationCount = COUNT(*) FROM visualized_minimum_quantity_alarms WHERE MINIMUM_QUANTITY_ALARM_ID = @ActiveAlarmId

	IF @DisableAlarms = 0 AND (@AvailableQuantity < @MinimumQuantity OR (@ActiveAlarmId IS NOT NULL AND @RealAvailableQuantity < @MinimumQuantity))
	BEGIN
		/* T18-1 (R5): evaluación de la aplicación sin cambio de cantidad ni de mínimo -> la alarma no se toca
		   (no se refresca, no se borran visualizaciones, no se registra en el log). */
		IF @JOB = 0 AND EXISTS (SELECT 1 FROM minimum_quantity_alarms WITH (NOLOCK) WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID
		                          AND QUANTITY = @AvailableQuantity AND MINIMUM_QUANTITY = @MinimumQuantity)
			RETURN

		DECLARE @Rows INT
		/* Proceso programado (@JOB = 1) sin cambio de cantidad: mismo refresco, sin fila en el log */
		IF @JOB = 1 AND EXISTS (SELECT 1 FROM minimum_quantity_alarms WITH (NOLOCK) WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID AND QUANTITY = @AvailableQuantity)
		BEGIN
			UPDATE a
			   SET QUANTITY = @AvailableQuantity, MINIMUM_QUANTITY = @MinimumQuantity, CREATION_DATE = GETDATE()
			  FROM minimum_quantity_alarms a WITH (NOLOCK)
			 WHERE a.ACTIVE = 1
			   AND a.REFERENCE_ID = @REFERENCE_ID
			SET @Rows = @@ROWCOUNT
		END
		ELSE
		BEGIN
			UPDATE a
				   SET QUANTITY = @AvailableQuantity, MINIMUM_QUANTITY = @MinimumQuantity, CREATION_DATE = GETDATE()
			OUTPUT 'MINIMUM_QUANTITY', inserted.MINIMUM_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, 'R',
				   deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
				   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
				   @VisualizationCount, CASE WHEN @JOB = 0 AND @VisualizationCount > 0 THEN 1 ELSE 0 END, @SOURCE
			  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
				   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
				   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
				   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
				  FROM minimum_quantity_alarms a WITH (NOLOCK)
				 WHERE a.ACTIVE = 1
				   AND a.REFERENCE_ID = @REFERENCE_ID
			SET @Rows = @@ROWCOUNT
		END

		IF @Rows = 0
			INSERT INTO minimum_quantity_alarms (REFERENCE_ID, QUANTITY, MINIMUM_QUANTITY)
	 			 SELECT REFERENCE_ID, @AvailableQuantity, ALARM_MINIMUM_QUANTITY
		  		   FROM item_references WITH (NOLOCK)
				  WHERE REFERENCE_ID = @REFERENCE_ID

		IF @JOB = 0
			DELETE a
			  FROM visualized_minimum_quantity_alarms a
			  WHERE EXISTS (SELECT 1 FROM minimum_quantity_alarms b WITH (NOLOCK)
									WHERE b.MINIMUM_QUANTITY_ALARM_ID = a.MINIMUM_QUANTITY_ALARM_ID
									  AND b.ACTIVE = 1
									  AND b.REFERENCE_ID = @REFERENCE_ID)
	END
	ELSE
		UPDATE a
		   SET ACTIVE = 0, INACTIVATION_DATE = GETDATE()
		OUTPUT 'MINIMUM_QUANTITY', inserted.MINIMUM_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, 'I',
			   deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
			   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
			   @VisualizationCount, 0, @SOURCE
		  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
			   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
			   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
			   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
		  FROM minimum_quantity_alarms a WITH (NOLOCK)
		 WHERE a.ACTIVE = 1
		   AND a.REFERENCE_ID = @REFERENCE_ID
END
GO

/* ---------- 2b. Inventario sin stock ---------- */
CREATE OR ALTER PROCEDURE [dbo].[SP_Set_Out_Of_Stock_Alarm]
	@REFERENCE_ID INT,
	@Job BIT = 0,
	@SOURCE VARCHAR(60) = NULL
AS
BEGIN
	DECLARE @AvailableQuantity INT, @RealAvailableQuantity INT, @IsDomesticProduct BIT, @IsSpecialImport BIT, @IsSaleOff BIT, @DisableAlarms BIT
	DECLARE @ActiveAlarmId INT, @VisualizationCount INT = 0

	SET @SOURCE = ISNULL(@SOURCE, CASE WHEN @Job = 1 THEN 'Proceso programado' ELSE 'No identificado' END)

	SELECT @IsDomesticProduct = IS_DOMESTIC_PRODUCT, @IsSpecialImport = IS_SPECIAL_IMPORT, @IsSaleOff = IS_SALE_OFF, @DisableAlarms = a.DISABLE_ALARMS
	  FROM items a WITH (NOLOCK)
	  JOIN item_references b WITH (NOLOCK) ON b.ITEM_ID = a.ITEM_ID
	 WHERE b.REFERENCE_ID = @REFERENCE_ID

	IF @DisableAlarms = 0
	BEGIN
		SELECT @AvailableQuantity = CASE WHEN @IsDomesticProduct = 0 AND @IsSpecialImport = 0 AND @IsSaleOff = 0 THEN a.INVENTORY_QUANTITY - ORDERED_QUANTITY ELSE 1 END
		  FROM item_references a WITH (NOLOCK)
		 WHERE a.REFERENCE_ID = @REFERENCE_ID

		/* H6: disponible real, sin OC pendientes */
		SET @RealAvailableQuantity = @AvailableQuantity

		SELECT @AvailableQuantity = @AvailableQuantity + ISNULL(SUM(a.REQUESTED_QUANTITY), 0)
		  FROM purchase_order_details a WITH (NOLOCK)
		  JOIN purchase_orders b WITH (NOLOCK) ON b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		  JOIN status_document_types c WITH (NOLOCK) ON c.STATUS_DOCUMENT_TYPE_ID = b.STATUS_DOCUMENT_TYPE_ID
		 WHERE a.REFERENCE_ID = @REFERENCE_ID
		   AND c.STATUS_ORDER IN (1, 4)	/* Pendientes + Ajuste en aprobación (igual que producción, RQM Aprobación Ajustes OC) */
	END

	/* La referencia es de sin stock (mínimo <= 0): una alarma de cantidad mínima activa queda sin quién la evalúe -> se inactiva */
	DECLARE @OtherAlarmId INT, @OtherVisualizationCount INT = 0
	SELECT @OtherAlarmId = MINIMUM_QUANTITY_ALARM_ID FROM minimum_quantity_alarms WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID
	   AND EXISTS (SELECT 1 FROM item_references WHERE REFERENCE_ID = @REFERENCE_ID AND ALARM_MINIMUM_QUANTITY <= 0)
	IF @OtherAlarmId IS NOT NULL
	BEGIN
		SELECT @OtherVisualizationCount = COUNT(*) FROM visualized_minimum_quantity_alarms WHERE MINIMUM_QUANTITY_ALARM_ID = @OtherAlarmId
		UPDATE a
		   SET ACTIVE = 0, INACTIVATION_DATE = GETDATE()
		OUTPUT 'MINIMUM_QUANTITY', inserted.MINIMUM_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, 'I',
			   deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
			   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
			   @OtherVisualizationCount, 0, 'Referencia pasó a sin stock'
		  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
			   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
			   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
			   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
		  FROM minimum_quantity_alarms a
		 WHERE a.MINIMUM_QUANTITY_ALARM_ID = @OtherAlarmId
	END

	SELECT @ActiveAlarmId = OUT_OF_STOCK_INVENTORY_ALARM_ID
	  FROM out_of_stock_inventory_alarms WITH (NOLOCK)
	 WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID

	IF @ActiveAlarmId IS NOT NULL
		SELECT @VisualizationCount = COUNT(*) FROM visualized_out_of_stock_inventory_alarms WHERE OUT_OF_STOCK_INVENTORY_ALARM_ID = @ActiveAlarmId

	IF @DisableAlarms = 0 AND (@AvailableQuantity <= 0 OR (@ActiveAlarmId IS NOT NULL AND @RealAvailableQuantity <= 0))
	BEGIN
		/* T18-1 (R5): evaluación de la aplicación sin cambio de cantidad -> la alarma no se toca. */
		IF @Job = 0 AND EXISTS (SELECT 1 FROM out_of_stock_inventory_alarms WITH (NOLOCK) WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID
		                          AND QUANTITY = @AvailableQuantity)
			RETURN

		DECLARE @Rows INT
		/* Proceso programado (@JOB = 1) sin cambio de cantidad: mismo refresco, sin fila en el log */
		IF @Job = 1 AND EXISTS (SELECT 1 FROM out_of_stock_inventory_alarms WITH (NOLOCK) WHERE ACTIVE = 1 AND REFERENCE_ID = @REFERENCE_ID AND QUANTITY = @AvailableQuantity)
		BEGIN
			UPDATE a
			   SET QUANTITY = @AvailableQuantity, CREATION_DATE = GETDATE()
			  FROM out_of_stock_inventory_alarms a WITH (NOLOCK)
			 WHERE a.ACTIVE = 1
			   AND a.REFERENCE_ID = @REFERENCE_ID
			SET @Rows = @@ROWCOUNT
		END
		ELSE
		BEGIN
			UPDATE a
				   SET QUANTITY = @AvailableQuantity, CREATION_DATE = GETDATE()
			OUTPUT 'OUT_OF_STOCK', inserted.OUT_OF_STOCK_INVENTORY_ALARM_ID, inserted.REFERENCE_ID, 'R',
				   deleted.QUANTITY, inserted.QUANTITY, NULL, NULL,
				   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
				   @VisualizationCount, CASE WHEN @Job = 0 AND @VisualizationCount > 0 THEN 1 ELSE 0 END, @SOURCE
			  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
				   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
				   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
				   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
				  FROM out_of_stock_inventory_alarms a WITH (NOLOCK)
				 WHERE a.ACTIVE = 1
				   AND a.REFERENCE_ID = @REFERENCE_ID
			SET @Rows = @@ROWCOUNT
		END

		IF @Rows = 0
			INSERT INTO out_of_stock_inventory_alarms (REFERENCE_ID, QUANTITY)
	 			 SELECT REFERENCE_ID, @AvailableQuantity
		  		   FROM item_references WITH (NOLOCK)
				  WHERE REFERENCE_ID = @REFERENCE_ID

		IF @Job = 0
			DELETE a
			  FROM visualized_out_of_stock_inventory_alarms a
			  WHERE EXISTS (SELECT 1 FROM out_of_stock_inventory_alarms b WITH (NOLOCK)
									WHERE b.OUT_OF_STOCK_INVENTORY_ALARM_ID = a.OUT_OF_STOCK_INVENTORY_ALARM_ID
									  AND b.ACTIVE = 1
									  AND b.REFERENCE_ID = @REFERENCE_ID)
	END
	ELSE
		UPDATE a
		   SET ACTIVE = 0, INACTIVATION_DATE = GETDATE()
		OUTPUT 'OUT_OF_STOCK', inserted.OUT_OF_STOCK_INVENTORY_ALARM_ID, inserted.REFERENCE_ID, 'I',
			   deleted.QUANTITY, inserted.QUANTITY, NULL, NULL,
			   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
			   @VisualizationCount, 0, @SOURCE
		  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
			   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
			   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
			   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
		  FROM out_of_stock_inventory_alarms a WITH (NOLOCK)
		 WHERE a.ACTIVE = 1
		   AND a.REFERENCE_ID = @REFERENCE_ID
END
GO

/* ---------- 3. Mínimo en bodega local (H1) ---------- */
CREATE OR ALTER TRIGGER [dbo].[Trg_Minimum_Local_warehouse_Quantity_Alarm]
   ON  [dbo].[references_warehouse] AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ReferenceId INT, @IsDomesticProduct INT, @IsSpecialImport INT, @MinimumQuantity INT, @Quantity INT, @DisableAlarms BIT
	DECLARE @ActiveAlarmId INT, @VisualizationCount INT = 0

	/* Solo la fila de Bodega Local (antes se tomaba cualquier bodega) */
	SELECT @ReferenceId = a.REFERENCE_ID, @Quantity = a.QUANTITY
	  FROM inserted a
	  JOIN warehouses w ON w.WAREHOUSE_ID = a.WAREHOUSE_ID
	 WHERE w.WAREHOUSE_CODE = 1

	IF @ReferenceId IS NULL
		RETURN

	SELECT @IsDomesticProduct = b.IS_DOMESTIC_PRODUCT, @IsSpecialImport = b.IS_SPECIAL_IMPORT, @MinimumQuantity = a.MINIMUM_LOCAL_WAREHOUSE_QUANTITY,
		   @DisableAlarms = b.DISABLE_ALARMS
	  FROM item_references a WITH (NOLOCK)
	  JOIN items b WITH (NOLOCK) ON b.ITEM_ID = a.ITEM_ID
	 WHERE a.REFERENCE_ID = @ReferenceId
	   AND b.IS_ACTIVE = 1
	   AND a.IS_ACTIVE = 1

	SELECT @ActiveAlarmId = MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID
	  FROM minimum_local_warehouse_quantity_alarms
	 WHERE ACTIVE = 1 AND REFERENCE_ID = @ReferenceId

	IF @ActiveAlarmId IS NOT NULL
		SELECT @VisualizationCount = COUNT(*) FROM visualized_minimum_local_warehouse_quantity_alarms WHERE MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @ActiveAlarmId

	IF (@DisableAlarms = 0 AND @IsDomesticProduct = 0 AND @IsSpecialImport = 0 AND @MinimumQuantity > @Quantity)
	BEGIN
		IF @ActiveAlarmId IS NULL
			/* Nace la alarma */
			INSERT INTO minimum_local_warehouse_quantity_alarms (REFERENCE_ID, QUANTITY, MINIMUM_QUANTITY)
			     VALUES (@ReferenceId, @Quantity, @MinimumQuantity)
		ELSE
		BEGIN
			/* Refresco: estado más reciente y vuelve a salirle a todos */
			UPDATE a
			   SET QUANTITY = @Quantity, MINIMUM_QUANTITY = @MinimumQuantity, CREATION_DATE = GETDATE()
			OUTPUT 'MINIMUM_LOCAL_WAREHOUSE', inserted.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, 'R',
				   deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
				   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
				   @VisualizationCount, CASE WHEN @VisualizationCount > 0 THEN 1 ELSE 0 END, 'Movimiento en bodega local'
			  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
				   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
				   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
				   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
			  FROM minimum_local_warehouse_quantity_alarms a
			 WHERE a.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @ActiveAlarmId

			DELETE FROM visualized_minimum_local_warehouse_quantity_alarms
			 WHERE MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @ActiveAlarmId
		END
	END
	ELSE
		/* Muere la alarma */
		UPDATE a
		   SET ACTIVE = 0, INACTIVATION_DATE = GETDATE()
		OUTPUT 'MINIMUM_LOCAL_WAREHOUSE', inserted.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, 'I',
			   deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
			   deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE,
			   @VisualizationCount, 0, 'Movimiento en bodega local'
		  INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE,
			   PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
			   PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE,
			   VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
		  FROM minimum_local_warehouse_quantity_alarms a
		 WHERE a.ACTIVE = 1
		   AND a.REFERENCE_ID = @ReferenceId
END
GO

/* ---------- 4. Triggers que invocan los SP: solo agregan @SOURCE ---------- */
CREATE OR ALTER TRIGGER [dbo].[Trg_Minimum_Quantity_Alarm]
   ON  [dbo].[item_references] AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ReferenceId INT

	IF (UPDATE(INVENTORY_QUANTITY) AND NOT UPDATE(WORK_IN_PROCESS_QUANTITY)) OR
	   (UPDATE(ORDERED_QUANTITY) AND NOT UPDATE(WORK_IN_PROCESS_QUANTITY)) OR
	   UPDATE(RESERVED_QUANTITY) OR
	   UPDATE(IS_ACTIVE) OR
	   UPDATE(ALARM_MINIMUM_QUANTITY)
	BEGIN
		IF EXISTS (SELECT 1 FROM inserted WHERE ALARM_MINIMUM_QUANTITY > 0)
		BEGIN
			SELECT @ReferenceId = REFERENCE_ID FROM inserted

			EXEC SP_Set_Minimum_Quantity_Alarm @ReferenceId, @SOURCE = 'Movimiento de la referencia'
		END
	END
END
GO

CREATE OR ALTER TRIGGER [dbo].[Trg_Minimum_Quantity_Alarm_Purchase_Detail]
   ON  [dbo].[purchase_order_details] AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReferenceId INT = NULL

	SELECT @ReferenceId = REFERENCE_ID FROM inserted

	IF @ReferenceId IS NULL
		SELECT @ReferenceId = REFERENCE_ID FROM deleted

	IF EXISTS (SELECT 1 FROM item_references WHERE REFERENCE_ID = @ReferenceId AND ALARM_MINIMUM_QUANTITY > 0)
		EXEC SP_Set_Minimum_Quantity_Alarm @ReferenceId, @SOURCE = 'Detalle de orden de compra'
END
GO

CREATE OR ALTER TRIGGER [dbo].[Trg_Minimum_Quantity_Alarm_Purchase_Order_Cancelled]
   ON  [dbo].[purchase_orders] AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @References TABLE (IdReg INT NOT NULL IDENTITY(1,1), Reference_Id INT NOT NULL)

	DECLARE @IdReg INT, @TotalReg INT, @ReferenceId INT

	INSERT INTO @References(Reference_Id)
		 SELECT c.REFERENCE_ID
		   FROM inserted a
		   JOIN status_document_types b ON b.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		   JOIN purchase_order_details c ON c.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		   JOIN item_references d ON d.REFERENCE_ID = c.REFERENCE_ID
		  WHERE b.STATUS_ORDER in (3) /* cancelando la Orden */
		    AND d.ALARM_MINIMUM_QUANTITY > 0

	SELECT @IdReg = MIN(IdReg), @TotalReg = MAX(IdReg) FROM @References

	WHILE @IdReg <= @TotalReg
	BEGIN
		SET @ReferenceId = NULL

		SELECT @ReferenceId = Reference_Id FROM @References WHERE IdReg = @IdReg

		IF NOT @ReferenceId IS NULL
			EXEC SP_Set_Minimum_Quantity_Alarm @ReferenceId, @SOURCE = 'Cancelación de orden de compra'

		SET @IdReg = @IdReg+1
	END
END
GO

CREATE OR ALTER TRIGGER [dbo].[Trg_Is_Special_Import]
   ON  [dbo].[items]
   AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ReferenceId INT, @RowNum INT, @TotalRow INT, @MinimumQuantity INT
	DECLARE @References TABLE (RowNum INT NOT NULL IDENTITY(1,1), ReferenceId INT, MinimumQuantity INT)

	IF UPDATE(IS_SPECIAL_IMPORT) OR UPDATE(IS_DOMESTIC_PRODUCT) OR UPDATE(IS_SALE_OFF)
	BEGIN
		INSERT INTO @References (ReferenceId, MinimumQuantity)
		     SELECT REFERENCE_ID, ALARM_MINIMUM_QUANTITY
			   FROM item_references a
			   JOIN inserted b ON b.ITEM_ID = a.ITEM_ID

		SELECT @RowNum = MIN(RowNum), @TotalRow = MAX(RowNum)
		  FROM @References

		WHILE @RowNum <= @TotalRow
		BEGIN
			SELECT @ReferenceId = ReferenceId, @MinimumQuantity = MinimumQuantity
			  FROM @References
			 WHERE RowNum = @RowNum

			IF @MinimumQuantity > 0
				EXEC SP_Set_Minimum_Quantity_Alarm @ReferenceId, @SOURCE = 'Cambio en el artículo'
			IF @MinimumQuantity <= 0
				EXEC SP_Set_Out_Of_Stock_Alarm @ReferenceId, @SOURCE = 'Cambio en el artículo'

			SET @RowNum = @RowNum+1
		END
	END
END
GO

CREATE OR ALTER TRIGGER [dbo].[Trg_Out_Of_Stock_Alarm]
   ON  [dbo].[item_references] AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ReferenceId INT

	IF (UPDATE(INVENTORY_QUANTITY) AND NOT UPDATE(WORK_IN_PROCESS_QUANTITY)) OR
	   (UPDATE(ORDERED_QUANTITY) AND NOT UPDATE(WORK_IN_PROCESS_QUANTITY)) OR
	   UPDATE(RESERVED_QUANTITY) OR
	   UPDATE(IS_ACTIVE) OR
	   UPDATE(ALARM_MINIMUM_QUANTITY)
	BEGIN
		IF EXISTS (SELECT 1 FROM inserted WHERE ALARM_MINIMUM_QUANTITY <= 0)
		BEGIN
			SELECT @ReferenceId = REFERENCE_ID FROM inserted

			EXEC SP_Set_Out_Of_Stock_Alarm @ReferenceId, @SOURCE = 'Movimiento de la referencia'
		END
	END
END
GO

CREATE OR ALTER TRIGGER [dbo].[Trg_Out_Of_Stock_Alarm_Purchase_Detail]
   ON  [dbo].[purchase_order_details] AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReferenceId INT = NULL

	SELECT @ReferenceId = REFERENCE_ID FROM inserted

	IF @ReferenceId IS NULL
		SELECT @ReferenceId = REFERENCE_ID FROM deleted

	IF EXISTS (SELECT 1 FROM item_references WHERE REFERENCE_ID = @ReferenceId AND ALARM_MINIMUM_QUANTITY <= 0)
		EXEC SP_Set_Out_Of_Stock_Alarm @ReferenceId, @SOURCE = 'Detalle de orden de compra'
END
GO

CREATE OR ALTER TRIGGER [dbo].[Trg_Out_Of_Stock_Alarm_Purchase_Order_Cancelled]
   ON  [dbo].[purchase_orders] AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @References TABLE (IdReg INT NOT NULL IDENTITY(1,1), Reference_Id INT NOT NULL)

	DECLARE @IdReg INT, @TotalReg INT, @ReferenceId INT

	INSERT INTO @References(Reference_Id)
		 SELECT c.REFERENCE_ID
		   FROM inserted a
		   JOIN status_document_types b ON b.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		   JOIN purchase_order_details c ON c.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		   JOIN item_references d ON d.REFERENCE_ID = c.REFERENCE_ID
		  WHERE b.STATUS_ORDER in (3) /* cancelando la Orden */
		    AND d.ALARM_MINIMUM_QUANTITY <= 0

	SELECT @IdReg = MIN(IdReg), @TotalReg = MAX(IdReg) FROM @References

	WHILE @IdReg <= @TotalReg
	BEGIN
		SET @ReferenceId = NULL

		SELECT @ReferenceId = Reference_Id FROM @References WHERE IdReg = @IdReg

		IF @ReferenceId IS NOT NULL
			EXEC SP_Set_Out_Of_Stock_Alarm @ReferenceId, @SOURCE = 'Cancelación de orden de compra'

		SET @IdReg = @IdReg+1
	END
END
GO
