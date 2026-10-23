/* =====================================================================================
   RQM Complemento Consulta de Logs - T3: Consulta genérica de visualización de alarmas
   -------------------------------------------------------------------------------------
   Objetos:
     1. Tabla paramétrica  ALARM_VISUALIZATION_LOG_TYPES (tipos de alarma consultables)
     2. Vista              VW_ALARM_LOG                   (alarmas ACTIVAS normalizadas)
     3. Vista              VW_ALARM_LOG_VISUALIZATIONS    (visualizaciones vigentes)
     4. SP genéricos       SP_GET_ALARM_LOG_TYPES / SP_GET_ALARM_LOG / SP_GET_ALARM_LOG_VISUALIZATIONS
     5. Roles nuevos       "Log del sistema" y "Log de inventarios"

   Agregar una alarma nueva en el futuro = 1 fila en la tabla paramétrica
   + 1 rama en cada vista. No requiere SP ni pantalla nuevos.

   Criterio "alarma activa" = el mismo del tablero de notificaciones.
   ALARM_DATE = fecha desde la cual el usuario puede ver la alarma en el tablero.

   Script re-ejecutable (CREATE OR ALTER / IF NOT EXISTS / MERGE).
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------- 1. Tabla paramétrica ---------- */
IF OBJECT_ID('dbo.ALARM_VISUALIZATION_LOG_TYPES') IS NULL
BEGIN
    CREATE TABLE dbo.ALARM_VISUALIZATION_LOG_TYPES
    (
        CODE           VARCHAR(30)   NOT NULL CONSTRAINT PK_ALARM_VISUALIZATION_LOG_TYPES PRIMARY KEY,
        NAME           NVARCHAR(100) NOT NULL,
        CONTEXT        CHAR(1)       NOT NULL CONSTRAINT CK_ALARM_VISUALIZATION_LOG_TYPES_CONTEXT CHECK (CONTEXT IN ('O','P','R','I')),
        ROLE_NAME      NVARCHAR(256) NOT NULL,
        DISPLAY_ORDER  SMALLINT      NOT NULL,
        IS_ACTIVE      BIT           NOT NULL CONSTRAINT DF_ALARM_VISUALIZATION_LOG_TYPES_IS_ACTIVE DEFAULT (1)
    );
END
GO

MERGE dbo.ALARM_VISUALIZATION_LOG_TYPES AS t
USING (VALUES
    ('PURCHASE_ORDER_ALARM',          N'Alarmas de órdenes de compra',                     'O', N'Log de ordenes de compra', 1),
    ('PURCHASE_ORDER_TRANSIT',        N'Órdenes en tránsito modificadas',                  'O', N'Log de ordenes de compra', 2),
    ('CUSTOMER_ORDER_ALARM',          N'Alarmas de pedidos',                               'P', N'Log de pedidos',           1),
    ('AUTOMATIC_IN_PROCESS',          N'Creación automática de traslados a proceso',       'P', N'Log de pedidos',           2),
    ('AUTOMATIC_IN_PROCESS_CHANGE',   N'Modificaciones de traslados a proceso automáticos','P', N'Log de pedidos',           3),
    ('CUSTOMER_RESERVATION_ALARM',    N'Alarmas de reservas',                              'R', N'Log de reservas',          1),
    ('MINIMUM_QUANTITY',              N'Sobrepaso de cantidades mínimas',                  'I', N'Log de inventarios',       1),
    ('MINIMUM_LOCAL_WAREHOUSE',       N'Sobrepaso de cantidades mínimas en bodega local',  'I', N'Log de inventarios',       2),
    ('OUT_OF_STOCK',                  N'Inventario sin stock',                             'I', N'Log de inventarios',       3),
    ('LOCAL_WAREHOUSE_INCOME',        N'Ingreso de mercancía a bodega local',              'I', N'Log de inventarios',       4)
) AS s (CODE, NAME, CONTEXT, ROLE_NAME, DISPLAY_ORDER)
ON t.CODE = s.CODE
WHEN MATCHED THEN UPDATE SET NAME = s.NAME, CONTEXT = s.CONTEXT, ROLE_NAME = s.ROLE_NAME, DISPLAY_ORDER = s.DISPLAY_ORDER
WHEN NOT MATCHED THEN INSERT (CODE, NAME, CONTEXT, ROLE_NAME, DISPLAY_ORDER) VALUES (s.CODE, s.NAME, s.CONTEXT, s.ROLE_NAME, s.DISPLAY_ORDER);
GO

/* ---------- 2. Vista de alarmas activas ---------- */
CREATE OR ALTER VIEW dbo.VW_ALARM_LOG
AS
    /* Alarmas de documentos (alarms): activas y ya ejecutadas. 1 = Orden, 2 = Pedido, 3 = Reserva */
    SELECT CAST('PURCHASE_ORDER_ALARM' AS VARCHAR(30)) AS TYPE_CODE,
           CAST(a.ALARM_ID AS VARCHAR(50))            AS ALARM_KEY,
           a.EXECUTION_DATE                           AS ALARM_DATE,
           CAST(CONCAT(N'OC ', p.ORDER_NUMBER, N' - ', pr.PROVIDER_NAME) AS NVARCHAR(300)) AS DOCUMENT,
           CAST(m.ALARM_MESSAGE AS NVARCHAR(500))     AS DESCRIPTION
      FROM dbo.alarms a
      JOIN dbo.alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID AND m.ALARM_TYPE_ID = 1
      JOIN dbo.purchase_orders p ON p.PURCHASE_ORDER_ID = a.DOCUMENT_ID
      JOIN dbo.providers pr ON pr.PROVIDER_ID = p.PROVIDER_ID
     WHERE a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE()
    UNION ALL
    SELECT 'CUSTOMER_ORDER_ALARM', CAST(a.ALARM_ID AS VARCHAR(50)), a.EXECUTION_DATE,
           CONCAT(N'Pedido ', o.ORDER_NUMBER, N' - ', c.CUSTOMER_NAME),
           m.ALARM_MESSAGE
      FROM dbo.alarms a
      JOIN dbo.alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID AND m.ALARM_TYPE_ID = 2
      JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = a.DOCUMENT_ID
      JOIN dbo.customers c ON c.CUSTOMER_ID = o.CUSTOMER_ID
     WHERE a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE()
    UNION ALL
    SELECT 'CUSTOMER_RESERVATION_ALARM', CAST(a.ALARM_ID AS VARCHAR(50)), a.EXECUTION_DATE,
           CONCAT(N'Reserva ', r.RESERVATION_NUMBER, N' - ', c.CUSTOMER_NAME),
           m.ALARM_MESSAGE
      FROM dbo.alarms a
      JOIN dbo.alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID AND m.ALARM_TYPE_ID = 3
      JOIN dbo.customer_reservations r ON r.CUSTOMER_RESERVATION_ID = a.DOCUMENT_ID
      JOIN dbo.customers c ON c.CUSTOMER_ID = r.CUSTOMER_ID
     WHERE a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE()
    UNION ALL
    /* Órdenes en tránsito modificadas: OC pendiente (1) o en aprobación de ajuste (4) */
    SELECT 'PURCHASE_ORDER_TRANSIT', CAST(t.PURCHASE_ORDER_TRANSIT_ALARM_ID AS VARCHAR(50)), mp.MODIFICATION_DATE,
           CONCAT(N'OC ', p.ORDER_NUMBER, N' - ', pr.PROVIDER_NAME),
           CONCAT(N'Fecha esperada anterior: ', CONVERT(VARCHAR(10), t.OLD_EXPECTED_RECEIPT_DATE, 103),
                  N' · Fecha esperada actual: ', CONVERT(VARCHAR(10), p.EXPECTED_RECEIPT_DATE, 103),
                  N' · Motivo: ', mr.MODIFICATION_REASON_NAME)
      FROM dbo.purchase_order_transit_alarms t
      JOIN dbo.modified_purchase_orders mp ON mp.MODIFIED_PURCHASE_ORDER_ID = t.MODIFIED_PURCHASE_ORDER_ID
      JOIN dbo.modification_reasons mr ON mr.MODIFICATION_REASON_ID = mp.MODIFICATION_REASON_ID
      JOIN dbo.purchase_orders p ON p.PURCHASE_ORDER_ID = mp.PURCHASE_ORDER_ID
      JOIN dbo.providers pr ON pr.PROVIDER_ID = p.PROVIDER_ID
      JOIN dbo.status_document_types s ON s.STATUS_DOCUMENT_TYPE_ID = p.STATUS_DOCUMENT_TYPE_ID AND s.STATUS_ORDER IN (1, 4)
    UNION ALL
    /* Creación automática de traslados a proceso (documento origen: OC o traslado entre bodegas) */
    SELECT 'AUTOMATIC_IN_PROCESS', CAST(ai.AUTOMATIC_IN_PROCESS_ID AS VARCHAR(50)), ai.CREATION_DATE,
           CASE dt.DOCUMENT_TYPE_CODE
                WHEN 'O' THEN CONCAT(N'OC ', p.ORDER_NUMBER, N' - ', pr.PROVIDER_NAME)
                ELSE CONCAT(N'Traslado entre bodegas ', ai.DOCUMENT_ID)
           END,
           CONCAT((SELECT COUNT(DISTINCT x.CUSTOMER_ORDER_ID) FROM dbo.automatic_in_process_orders x WHERE x.AUTOMATIC_IN_PROCESS_ID = ai.AUTOMATIC_IN_PROCESS_ID),
                  N' pedido(s) atendido(s) automáticamente')
      FROM dbo.automatic_in_process ai
      JOIN dbo.document_types dt ON dt.DOCUMENT_TYPE_ID = ai.DOCUMENT_TYPE_ID
      LEFT JOIN dbo.purchase_orders p ON dt.DOCUMENT_TYPE_CODE = 'O' AND p.PURCHASE_ORDER_ID = ai.DOCUMENT_ID
      LEFT JOIN dbo.providers pr ON pr.PROVIDER_ID = p.PROVIDER_ID
    UNION ALL
    /* Cancelaciones de traslados a proceso generados automáticamente (llave: ID|C) */
    SELECT 'AUTOMATIC_IN_PROCESS_CHANGE', CONCAT(c.CUSTOMER_ORDER_IN_PROCESS_ID, '|C'), c.CANCELLATION_DATE,
           CONCAT(N'Pedido ', o.ORDER_NUMBER, N' - ', cu.CUSTOMER_NAME),
           CONCAT(N'Cancelación del traslado ', c.CUSTOMER_ORDER_IN_PROCESS_ID, N' · Motivo: ', cr.CANCELLATION_REASON_NAME, N' · Por: ', e.FULL_NAME)
      FROM dbo.canceled_orders_in_process c
      JOIN dbo.cancellation_reasons cr ON cr.CANCELLATION_REASON_ID = c.CANCELLATION_REASON_ID
      JOIN dbo.employees e ON e.EMPLOYEE_ID = c.EMPLOYEE_ID
      JOIN dbo.customer_orders_in_process ip ON ip.CUSTOMER_ORDER_IN_PROCESS_ID = c.CUSTOMER_ORDER_IN_PROCESS_ID
      JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = ip.CUSTOMER_ORDER_ID
      JOIN dbo.customers cu ON cu.CUSTOMER_ID = o.CUSTOMER_ID
     WHERE EXISTS (SELECT 1 FROM dbo.automatic_in_process_orders x WHERE x.CUSTOMER_ORDER_IN_PROCESS_ID = c.CUSTOMER_ORDER_IN_PROCESS_ID)
    UNION ALL
    /* Modificaciones de traslados automáticos no cancelados (llave: ID|M) */
    SELECT 'AUTOMATIC_IN_PROCESS_CHANGE', CONCAT(m.MODIFIED_CUSTOMER_ORDER_IN_PROCESS_ID, '|M'), m.MODIFICATION_DATE,
           CONCAT(N'Pedido ', o.ORDER_NUMBER, N' - ', cu.CUSTOMER_NAME),
           CONCAT(N'Modificación del traslado ', m.CUSTOMER_ORDER_IN_PROCESS_ID, N' · Motivo: ', mr.MODIFICATION_REASON_NAME, N' · Por: ', e.FULL_NAME)
      FROM dbo.modified_orders_in_process m
      JOIN dbo.modification_reasons mr ON mr.MODIFICATION_REASON_ID = m.MODIFICATION_REASON_ID
      JOIN dbo.employees e ON e.EMPLOYEE_ID = m.EMPLOYEE_ID
      JOIN dbo.customer_orders_in_process ip ON ip.CUSTOMER_ORDER_IN_PROCESS_ID = m.CUSTOMER_ORDER_IN_PROCESS_ID
      JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = ip.CUSTOMER_ORDER_ID
      JOIN dbo.customers cu ON cu.CUSTOMER_ID = o.CUSTOMER_ID
     WHERE EXISTS (SELECT 1 FROM dbo.automatic_in_process_orders x WHERE x.CUSTOMER_ORDER_IN_PROCESS_ID = m.CUSTOMER_ORDER_IN_PROCESS_ID)
       AND NOT EXISTS (SELECT 1 FROM dbo.canceled_orders_in_process c WHERE c.CUSTOMER_ORDER_IN_PROCESS_ID = m.CUSTOMER_ORDER_IN_PROCESS_ID)
    UNION ALL
    /* Alarmas de inventario activas */
    SELECT 'MINIMUM_QUANTITY', CAST(f.MINIMUM_QUANTITY_ALARM_ID AS VARCHAR(50)), f.CREATION_DATE,
           CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME, N' - ', r.REFERENCE_NAME),
           CONCAT(N'Disponible: ', f.QUANTITY, N' · Cantidad mínima: ', f.MINIMUM_QUANTITY)
      FROM dbo.minimum_quantity_alarms f
      JOIN dbo.item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1
      JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1
     WHERE f.ACTIVE = 1
    UNION ALL
    SELECT 'MINIMUM_LOCAL_WAREHOUSE', CAST(f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID AS VARCHAR(50)), f.CREATION_DATE,
           CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME, N' - ', r.REFERENCE_NAME),
           CONCAT(N'Cantidad en bodega local: ', f.QUANTITY, N' · Cantidad mínima: ', f.MINIMUM_QUANTITY)
      FROM dbo.minimum_local_warehouse_quantity_alarms f
      JOIN dbo.item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1
      JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1
     WHERE f.ACTIVE = 1
    UNION ALL
    SELECT 'OUT_OF_STOCK', CAST(f.OUT_OF_STOCK_INVENTORY_ALARM_ID AS VARCHAR(50)), f.CREATION_DATE,
           CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME, N' - ', r.REFERENCE_NAME),
           CONCAT(N'Disponible: ', f.QUANTITY)
      FROM dbo.out_of_stock_inventory_alarms f
      JOIN dbo.item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1
      JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1
     WHERE f.ACTIVE = 1
    UNION ALL
    /* Ingreso de mercancía a bodega local (documento: OC o traslado entre bodegas) */
    SELECT 'LOCAL_WAREHOUSE_INCOME', CAST(l.LOCAL_WAREHOUSE_ALARM_ID AS VARCHAR(50)), l.ALARM_DATE,
           CASE dt.DOCUMENT_TYPE_CODE
                WHEN 'O' THEN CONCAT(N'OC ', p.ORDER_NUMBER, N' - ', pr.PROVIDER_NAME)
                ELSE CONCAT(N'Traslado entre bodegas ', l.DOCUMENT_NUMBER)
           END,
           CONCAT(CASE WHEN ISJSON(l.REFERENCE_LIST) = 1 THEN (SELECT COUNT(*) FROM OPENJSON(l.REFERENCE_LIST)) ELSE 0 END, N' referencia(s) · ',
                  CASE WHEN ISJSON(l.CUSTOMER_ORDER_LIST) = 1 THEN (SELECT COUNT(*) FROM OPENJSON(l.CUSTOMER_ORDER_LIST)) ELSE 0 END, N' pedido(s) con pendiente')
      FROM dbo.local_warehouse_alarms l
      JOIN dbo.document_types dt ON dt.DOCUMENT_TYPE_ID = l.DOCUMENT_TYPE_ID
      LEFT JOIN dbo.purchase_orders p ON dt.DOCUMENT_TYPE_CODE = 'O' AND p.PURCHASE_ORDER_ID = l.DOCUMENT_NUMBER
      LEFT JOIN dbo.providers pr ON pr.PROVIDER_ID = p.PROVIDER_ID;
GO

/* ---------- 3. Vista de visualizaciones vigentes ---------- */
CREATE OR ALTER VIEW dbo.VW_ALARM_LOG_VISUALIZATIONS
AS
    SELECT CAST(CASE m.ALARM_TYPE_ID WHEN 1 THEN 'PURCHASE_ORDER_ALARM'
                                    WHEN 2 THEN 'CUSTOMER_ORDER_ALARM'
                                    WHEN 3 THEN 'CUSTOMER_RESERVATION_ALARM' END AS VARCHAR(30)) AS TYPE_CODE,
           CAST(v.ALARM_ID AS VARCHAR(50)) AS ALARM_KEY, v.EMPLOYEE_ID, v.VISUALIZED_DATE
      FROM dbo.visualized_alarms v
      JOIN dbo.alarms a ON a.ALARM_ID = v.ALARM_ID
      JOIN dbo.alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID
     WHERE m.ALARM_TYPE_ID IN (1, 2, 3)
    UNION ALL
    SELECT 'PURCHASE_ORDER_TRANSIT', CAST(PURCHASE_ORDER_TRANSIT_ALARM_ID AS VARCHAR(50)), EMPLOYEE_ID, VISUALIZED_DATE
      FROM dbo.visualized_purchase_order_transit_alarms
    UNION ALL
    SELECT 'AUTOMATIC_IN_PROCESS', CAST(AUTOMATIC_IN_PROCESS_ID AS VARCHAR(50)), EMPLOYEE_ID, VISUALIZED_DATE
      FROM dbo.visualized_automatic_in_process
    UNION ALL
    SELECT 'AUTOMATIC_IN_PROCESS_CHANGE', CONCAT(Id, '|', ActionType), Employee_Id, Visualized_Date
      FROM dbo.visualized_automatic_customer_in_process_modifications
    UNION ALL
    SELECT 'MINIMUM_QUANTITY', CAST(MINIMUM_QUANTITY_ALARM_ID AS VARCHAR(50)), EMPLOYEE_ID, VIZUALIZED_DATE
      FROM dbo.visualized_minimum_quantity_alarms
    UNION ALL
    SELECT 'MINIMUM_LOCAL_WAREHOUSE', CAST(MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID AS VARCHAR(50)), EMPLOYEE_ID, VIZUALIZED_DATE
      FROM dbo.visualized_minimum_local_warehouse_quantity_alarms
    UNION ALL
    SELECT 'OUT_OF_STOCK', CAST(OUT_OF_STOCK_INVENTORY_ALARM_ID AS VARCHAR(50)), EMPLOYEE_ID, VIZUALIZED_DATE
      FROM dbo.visualized_out_of_stock_inventory_alarms
    UNION ALL
    SELECT 'LOCAL_WAREHOUSE_INCOME', CAST(LOCAL_WAREHOUSE_ALARM_ID AS VARCHAR(50)), EMPLOYEE_ID, VISUALIZED_DATE
      FROM dbo.visualized_local_warehouse_alarms;
GO

/* ---------- 4. SP genéricos ---------- */

/* Tipos de alarma de un contexto que el usuario puede consultar.
   @Roles: roles del usuario separados por coma. "Administrador" ve todos. */
CREATE OR ALTER PROCEDURE dbo.SP_GET_ALARM_LOG_TYPES
    @Context CHAR(1),
    @Roles   NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserRoles TABLE (ROLE_NAME NVARCHAR(256) PRIMARY KEY);
    INSERT INTO @UserRoles (ROLE_NAME)
         SELECT DISTINCT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@Roles, ',') WHERE LEN(LTRIM(RTRIM(value))) > 0;

    SELECT t.CODE, t.NAME
      FROM dbo.ALARM_VISUALIZATION_LOG_TYPES t
     WHERE t.IS_ACTIVE = 1
       AND t.CONTEXT = @Context
       AND (EXISTS (SELECT 1 FROM @UserRoles r WHERE r.ROLE_NAME = t.ROLE_NAME)
            OR EXISTS (SELECT 1 FROM @UserRoles r WHERE r.ROLE_NAME = N'Administrador'))
     ORDER BY t.DISPLAY_ORDER;
END
GO

/* Página de alarmas activas de un tipo, con su número de visualizaciones vigentes. */
CREATE OR ALTER PROCEDURE dbo.SP_GET_ALARM_LOG
    @TypeCode  VARCHAR(30),
    @DateFrom  DATE,
    @DateTo    DATE,
    @SearchKey NVARCHAR(100) = NULL,
    @Skip      INT = 0,
    @Take      INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF NOT EXISTS (SELECT 1 FROM dbo.ALARM_VISUALIZATION_LOG_TYPES WHERE CODE = @TypeCode AND IS_ACTIVE = 1)
        THROW 50001, N'El tipo de alarma no está registrado o está inactivo.', 1;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;

    IF LEN(LTRIM(RTRIM(ISNULL(@SearchKey, N'')))) = 0
        SET @SearchKey = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));

    SELECT a.ALARM_KEY,
           a.ALARM_DATE,
           a.DOCUMENT,
           a.DESCRIPTION,
           (SELECT COUNT(*) FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v
             WHERE v.TYPE_CODE = @TypeCode AND v.ALARM_KEY = a.ALARM_KEY) AS VISUALIZATION_COUNT,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM dbo.VW_ALARM_LOG a
     WHERE a.TYPE_CODE = @TypeCode
       AND a.ALARM_DATE >= @From
       AND a.ALARM_DATE <  @To
       AND (@SearchKey IS NULL
            OR a.DOCUMENT    LIKE N'%' + @SearchKey + N'%'
            OR a.DESCRIPTION LIKE N'%' + @SearchKey + N'%')
     ORDER BY a.ALARM_DATE DESC, a.ALARM_KEY
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
    OPTION (RECOMPILE);
END
GO

/* Quién visualizó una alarma (visualizaciones vigentes). */
CREATE OR ALTER PROCEDURE dbo.SP_GET_ALARM_LOG_VISUALIZATIONS
    @TypeCode VARCHAR(30),
    @AlarmKey VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF NOT EXISTS (SELECT 1 FROM dbo.ALARM_VISUALIZATION_LOG_TYPES WHERE CODE = @TypeCode AND IS_ACTIVE = 1)
        THROW 50001, N'El tipo de alarma no está registrado o está inactivo.', 1;

    SELECT e.FULL_NAME AS EMPLOYEE_NAME,
           ar.AREA_NAME,
           v.VISUALIZED_DATE
      FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v
      JOIN dbo.employees e ON e.EMPLOYEE_ID = v.EMPLOYEE_ID
      LEFT JOIN dbo.areas ar ON ar.AREA_ID = e.AREA_ID
     WHERE v.TYPE_CODE = @TypeCode
       AND v.ALARM_KEY = @AlarmKey
     ORDER BY v.VISUALIZED_DATE DESC
    OPTION (RECOMPILE);
END
GO

/* ---------- 5. Roles nuevos ---------- */
INSERT INTO dbo.AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
SELECT CAST(NEWID() AS NVARCHAR(450)), CAST(NEWID() AS NVARCHAR(MAX)), r.Name, UPPER(r.Name)
  FROM (VALUES (N'Log del sistema'), (N'Log de inventarios')) r(Name)
 WHERE NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles x WHERE x.NormalizedName = UPPER(r.Name));
GO

/* ---------- Verificación ---------- */
SELECT CODE, NAME, CONTEXT, ROLE_NAME, DISPLAY_ORDER, IS_ACTIVE FROM dbo.ALARM_VISUALIZATION_LOG_TYPES ORDER BY CONTEXT, DISPLAY_ORDER;
SELECT Name FROM dbo.AspNetRoles WHERE Name IN (N'Log del sistema', N'Log de inventarios');
GO
