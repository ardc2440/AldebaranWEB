/* =====================================================================================
   RQM Complemento Consulta de Logs - T26: Pedido especial en reportes de pedidos
   -------------------------------------------------------------------------------------
   Requiere el script 17 (customer_orders.IS_SPECIAL_ORDER). Se despliega junto con la versión
   de la aplicación (los reportes leen la nueva columna IsSpecialOrder).
   En los 8 SP de reportes de pedidos:
   - Nuevo parámetro @IsSpecialOrder BIT = NULL (Sí = 1 / No = 0 / Todos = NULL). Las llamadas
     existentes no cambian (parámetros nombrados).
   - Condición del filtro sobre customer_orders.IS_SPECIAL_ORDER.
   - Columna IsSpecialOrder en el resultado.
   SP_GET_AUTOMATIC_CUSTOMER_ORDER_INPROCESS_PENDING_REPORT: corrección dentro del alcance; los filtros
   de fecha de pedido, fecha de proceso, fecha estimada de entrega, cliente y estado se recibían
   pero no se aplicaban.
   Cada SP se construye SIEMPRE desde su definición original respaldada en RQM_LOGS_OBJECT_BACKUP
   (re-ejecutable), validando su estructura (THROW si no es la esperada).
   Reversa: Reversa\18 - Reversa pedido especial en reportes.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.customer_orders', 'IS_SPECIAL_ORDER') IS NULL
    THROW 50030, N'Falta customer_orders.IS_SPECIAL_ORDER: ejecute primero el script 17.', 1;
GO

/* Respaldo de las definiciones originales (solo la primera vez) para la reversa */
IF OBJECT_ID('dbo.RQM_LOGS_OBJECT_BACKUP') IS NULL
    CREATE TABLE dbo.RQM_LOGS_OBJECT_BACKUP (OBJECT_NAME SYSNAME NOT NULL CONSTRAINT PK_RQM_LOGS_OBJECT_BACKUP PRIMARY KEY,
        OBJECT_TYPE CHAR(2) NOT NULL, DEFINITION NVARCHAR(MAX) NOT NULL,
        BACKUP_DATE DATETIME NOT NULL CONSTRAINT DF_RQM_LOGS_OBJECT_BACKUP_DATE DEFAULT (GETDATE()));
INSERT INTO dbo.RQM_LOGS_OBJECT_BACKUP (OBJECT_NAME, OBJECT_TYPE, DEFINITION)
SELECT o.name, o.type, m.definition
  FROM sys.objects o JOIN sys.sql_modules m ON m.object_id = o.object_id
 WHERE o.name IN ('SP_GET_CUSTOMER_ORDER_REPORT', 'SP_GET_CUSTOMER_ORDER_EXPORT', 'SP_GET_BACKORDER_REPORT', 'SP_GET_BACKORDER_EXPORT',
                  'SP_GET_CUSTOMER_ORDER_ACTIVITY_REPORT', 'SP_GET_CUSTOMER_SALES_REPORT',
                  'SP_AUTOMATIC_CUSTOMER_ORDER_ASSIGMENT_REPORT', 'SP_GET_AUTOMATIC_CUSTOMER_ORDER_INPROCESS_PENDING_REPORT')
   AND NOT EXISTS (SELECT 1 FROM dbo.RQM_LOGS_OBJECT_BACKUP b WHERE b.OBJECT_NAME = o.name);
GO

/* Procedimiento temporal: aplica hasta dos reemplazos (con número exacto de apariciones esperado)
   sobre la definición respaldada, agrega @IsSpecialOrder como primer parámetro y altera el SP. */
CREATE OR ALTER PROCEDURE #RQM_T26_APPLY
    @Name SYSNAME,
    @Find1 NVARCHAR(4000), @Replace1 NVARCHAR(4000), @Expected1 INT,
    @Find2 NVARCHAR(4000), @Replace2 NVARCHAR(4000), @Expected2 INT
AS
BEGIN
    DECLARE @d NVARCHAR(MAX) = (SELECT DEFINITION FROM dbo.RQM_LOGS_OBJECT_BACKUP WHERE OBJECT_NAME = @Name);
    DECLARE @Count1 INT = (LEN(@d) - LEN(REPLACE(@d, @Find1, ''))) / NULLIF(LEN(@Find1), 0);
    DECLARE @Count2 INT = (LEN(@d) - LEN(REPLACE(@d, @Find2, ''))) / NULLIF(LEN(@Find2), 0);
    DECLARE @Msg NVARCHAR(400) = @Name + N' no tiene la estructura esperada; revisar antes de desplegar.';

    IF @d IS NULL OR CHARINDEX('@IsSpecialOrder', @d) > 0 OR @Count1 <> @Expected1 OR @Count2 <> @Expected2
        THROW 50031, @Msg, 1;

    SET @d = REPLACE(@d, @Find1, @Replace1);
    SET @d = REPLACE(@d, @Find2, @Replace2);
    /* Primer parámetro: T26 Pedido especial Sí (1) / No (0) / Todos (NULL) */
    SET @d = STUFF(@d, CHARINDEX('@', @d, CHARINDEX('PROCEDURE', @d)), 0, '@IsSpecialOrder BIT = NULL,' + CHAR(13) + CHAR(10) + CHAR(9));
    SET @d = STUFF(@d, PATINDEX('%CREATE%PROCEDURE%', @d), CHARINDEX('PROCEDURE', @d) - PATINDEX('%CREATE%PROCEDURE%', @d), 'ALTER ');
    EXEC sys.sp_executesql @d;
    PRINT @Name + ' actualizado';
END
GO

DECLARE @Filter_a NVARCHAR(200) = '@CustomerId IS NULL)' + CHAR(13) + CHAR(10) + CHAR(9) + 'AND (a.IS_SPECIAL_ORDER = @IsSpecialOrder OR @IsSpecialOrder IS NULL)';
DECLARE @Column_a NVARCHAR(200) = 'a.CustomerId, (SELECT co.IS_SPECIAL_ORDER FROM customer_orders co WHERE co.CUSTOMER_ORDER_ID = a.OrderId) IsSpecialOrder,';

/* T26a: Pedidos (reporte y exportación) */
EXEC #RQM_T26_APPLY 'SP_GET_CUSTOMER_ORDER_REPORT', '@CustomerId IS NULL)', @Filter_a, 1, 'a.CustomerId,', @Column_a, 1;
EXEC #RQM_T26_APPLY 'SP_GET_CUSTOMER_ORDER_EXPORT', '@CustomerId IS NULL)', @Filter_a, 1, 'a.ORDER_NUMBER OrderNumber,', 'a.ORDER_NUMBER OrderNumber, a.IS_SPECIAL_ORDER IsSpecialOrder,', 1;

/* T26b: Back order (reporte y exportación) */
EXEC #RQM_T26_APPLY 'SP_GET_BACKORDER_REPORT', '@CustomerId IS NULL)', @Filter_a, 1, 'a.CustomerId,', @Column_a, 1;
EXEC #RQM_T26_APPLY 'SP_GET_BACKORDER_EXPORT', '@CustomerId IS NULL)', @Filter_a, 1, 'a.ORDER_NUMBER OrderNumber,', 'a.ORDER_NUMBER OrderNumber, a.IS_SPECIAL_ORDER IsSpecialOrder,', 1;

/* T26c: Actividades (alias b = customer_orders) */
EXEC #RQM_T26_APPLY 'SP_GET_CUSTOMER_ORDER_ACTIVITY_REPORT',
     '@CustomerId IS NULL)', '@CustomerId IS NULL)' + CHAR(13) + CHAR(10) + CHAR(9) + 'AND (b.IS_SPECIAL_ORDER = @IsSpecialOrder OR @IsSpecialOrder IS NULL)', 1,
     'a.CustomerId,', @Column_a, 1;

/* T26d: Ventas */
EXEC #RQM_T26_APPLY 'SP_GET_CUSTOMER_SALES_REPORT', '@CustomerId IS NULL)', @Filter_a, 1, 'a.CustomerId,', @Column_a, 1;

/* T26e: Asignación automática (alias e = customer_orders; dos bloques del UNION) */
EXEC #RQM_T26_APPLY 'SP_AUTOMATIC_CUSTOMER_ORDER_ASSIGMENT_REPORT',
     '@CustomerId IS NULL)', '@CustomerId IS NULL)' + CHAR(13) + CHAR(10) + CHAR(9) + 'AND (e.IS_SPECIAL_ORDER = @IsSpecialOrder OR @IsSpecialOrder IS NULL)', 2,
     'e.ORDER_NUMBER CustomerOrderNumber,', 'e.ORDER_NUMBER CustomerOrderNumber, e.IS_SPECIAL_ORDER IsSpecialOrder,', 2;

/* T26f: Pendientes de traslado automático (b = customer_orders, a = detalle del traslado).
   Corrección dentro del alcance: se aplican los filtros que el SP recibía y no usaba. */
EXEC #RQM_T26_APPLY 'SP_GET_AUTOMATIC_CUSTOMER_ORDER_INPROCESS_PENDING_REPORT',
     'WHERE ORDER_NUMBER = @OrderNumber OR @OrderNumber IS NULL',
     'WHERE (ORDER_NUMBER = @OrderNumber OR @OrderNumber IS NULL)' + CHAR(13) + CHAR(10)
     + CHAR(9) + 'AND (b.IS_SPECIAL_ORDER = @IsSpecialOrder OR @IsSpecialOrder IS NULL)' + CHAR(13) + CHAR(10)
     + CHAR(9) + 'AND (@OrderDateFrom IS NULL OR b.ORDER_DATE >= @OrderDateFrom) AND (@OrderDateTo IS NULL OR b.ORDER_DATE <= @OrderDateTo)' + CHAR(13) + CHAR(10)
     + CHAR(9) + 'AND (@ProcessDateFrom IS NULL OR CAST(a.PROCESS_DATE AS DATE) >= @ProcessDateFrom) AND (@ProcessDateTo IS NULL OR CAST(a.PROCESS_DATE AS DATE) <= @ProcessDateTo)' + CHAR(13) + CHAR(10)
     + CHAR(9) + 'AND (@EstimatedDeliveryDateFrom IS NULL OR b.ESTIMATED_DELIVERY_DATE >= @EstimatedDeliveryDateFrom) AND (@EstimatedDeliveryDateTo IS NULL OR b.ESTIMATED_DELIVERY_DATE <= @EstimatedDeliveryDateTo)' + CHAR(13) + CHAR(10)
     + CHAR(9) + 'AND (b.CUSTOMER_ID = @CustomerId OR @CustomerId IS NULL)' + CHAR(13) + CHAR(10)
     + CHAR(9) + 'AND (b.STATUS_DOCUMENT_TYPE_ID = @StatusDocumentTypeId OR @StatusDocumentTypeId IS NULL)', 1,
     'a.CUSTOMER_ORDER_ID CustomerOrderId,', 'a.CUSTOMER_ORDER_ID CustomerOrderId, (SELECT co.IS_SPECIAL_ORDER FROM customer_orders co WHERE co.CUSTOMER_ORDER_ID = a.CUSTOMER_ORDER_ID) IsSpecialOrder,', 1;
GO

DROP PROCEDURE IF EXISTS #RQM_T26_APPLY;
GO
