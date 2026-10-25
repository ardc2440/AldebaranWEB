/* =====================================================================================
   RQM Complemento Consulta de Logs - T6b: Búsqueda de Track por objetos de negocio
   -------------------------------------------------------------------------------------
   El usuario no conoce los Id: busca por lo que conoce (referencia interna, nombre, número de
   documento, identificación). Los Id de esos objetos están dentro del JSON de log.track.

   - LOG_TRACK_OBJECT_TYPES: tipos de objeto buscables (artículo, referencia, documentos, terceros, empleado).
   - LOG_TRACK_OBJECT_FIELDS: qué campo del JSON apunta a qué tipo de objeto (un objeto nuevo = una fila).
   - FN_TRACK_OBJECTS(@Json): objetos que toca un movimiento (campos directos + derivados:
     referencia -> artículo; detalle de pedido -> referencia, artículo y pedido).
   - LOG_TRACK_OBJECTS: índice (tipo, Id, fecha, movimiento). Se llena con:
       * TRG_LOG_TRACK_INDEX_OBJECTS (AFTER INSERT en log.track) para lo nuevo.
       * SP_BACKFILL_TRACK_OBJECTS para el histórico (masivo, por lotes, re-ejecutable: continúa desde LAST_ID).
   - SP_GET_TRACK_OBJECT_TYPES, SP_SEARCH_TRACK_OBJECTS(@ObjectType, @Text): búsqueda del objeto.
   - SP_GET_TRACK_LOG (reemplaza el de 07): entidad y/o objeto; rango obligatorio.
   - SP_GET_TRACK_OBJECT_DESCRIPTIONS(@Id): descripción de los Id del detalle (ItemId 4160 -> [TE-122] ...;
     Id paramétricos con FN_TRACK_LOOKUP_DESCRIPTION: StatusDocumentTypeId 10 -> Confirmado, motivos, bodegas...).
   Ejecutar después de 07. Script re-ejecutable. Al final ejecuta la carga del histórico.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------- 1. Catálogos ---------- */
IF OBJECT_ID('dbo.LOG_TRACK_OBJECT_TYPES') IS NULL
    CREATE TABLE dbo.LOG_TRACK_OBJECT_TYPES
    (
        OBJECT_TYPE   TINYINT       NOT NULL CONSTRAINT PK_LOG_TRACK_OBJECT_TYPES PRIMARY KEY,
        NAME          NVARCHAR(60)  NOT NULL,
        HINT          NVARCHAR(150) NOT NULL,
        DISPLAY_ORDER TINYINT       NOT NULL
    );
GO
MERGE dbo.LOG_TRACK_OBJECT_TYPES AS t
USING (VALUES
    (1,  N'Artículo',               N'Referencia interna o nombre del artículo', 1),
    (2,  N'Referencia',             N'Referencia interna, nombre del artículo o color', 2),
    (3,  N'Pedido',                 N'Número de pedido o cliente', 3),
    (4,  N'Orden de compra',        N'Número de orden, importación o proveedor', 4),
    (5,  N'Reserva',                N'Número de reserva o cliente', 5),
    (6,  N'Traslado a proceso',     N'Número del traslado o del pedido', 6),
    (7,  N'Despacho',               N'Número del despacho, del pedido o guía', 7),
    (8,  N'Ajuste de inventario',   N'Número del ajuste', 8),
    (9,  N'Traslado entre bodegas', N'Número del traslado', 9),
    (10, N'Cliente',                N'Nombre o identificación', 10),
    (11, N'Proveedor',              N'Nombre, código o identificación', 11),
    (12, N'Empleado',               N'Nombre o identificación', 12)
) AS s (OBJECT_TYPE, NAME, HINT, DISPLAY_ORDER)
   ON t.OBJECT_TYPE = s.OBJECT_TYPE
 WHEN MATCHED THEN UPDATE SET NAME = s.NAME, HINT = s.HINT, DISPLAY_ORDER = s.DISPLAY_ORDER
 WHEN NOT MATCHED THEN INSERT (OBJECT_TYPE, NAME, HINT, DISPLAY_ORDER) VALUES (s.OBJECT_TYPE, s.NAME, s.HINT, s.DISPLAY_ORDER);
GO

IF OBJECT_ID('dbo.LOG_TRACK_OBJECT_FIELDS') IS NULL
    CREATE TABLE dbo.LOG_TRACK_OBJECT_FIELDS
    (
        FIELD_NAME  NVARCHAR(100) NOT NULL CONSTRAINT PK_LOG_TRACK_OBJECT_FIELDS PRIMARY KEY,
        OBJECT_TYPE TINYINT       NOT NULL CONSTRAINT FK_LOG_TRACK_OBJECT_FIELDS_TYPE REFERENCES dbo.LOG_TRACK_OBJECT_TYPES (OBJECT_TYPE)
    );
GO
MERGE dbo.LOG_TRACK_OBJECT_FIELDS AS t
USING (VALUES
    (N'ItemId', 1), (N'ReferenceId', 2), (N'CustomerOrderId', 3), (N'PurchaseOrderId', 4), (N'CustomerReservationId', 5),
    (N'CustomerOrderInProcessId', 6), (N'CustomerOrderShipmentId', 7), (N'AdjustmentId', 8), (N'WarehouseTransferId', 9),
    (N'CustomerId', 10), (N'ProviderId', 11),
    (N'EmployeeId', 12), (N'ActivityEmployeeId', 12), (N'RequestEmployeeId', 12), (N'ResponseEmployeeId', 12),
    (N'EmployeeRecipientId', 12), (N'ChangedByEmployeeId', 12), (N'Employee_Id', 12)
) AS s (FIELD_NAME, OBJECT_TYPE)
   ON t.FIELD_NAME = s.FIELD_NAME
 WHEN MATCHED THEN UPDATE SET OBJECT_TYPE = s.OBJECT_TYPE
 WHEN NOT MATCHED THEN INSERT (FIELD_NAME, OBJECT_TYPE) VALUES (s.FIELD_NAME, s.OBJECT_TYPE);
GO

/* ---------- 2. Objetos que toca un movimiento ---------- */
CREATE OR ALTER FUNCTION dbo.FN_TRACK_OBJECTS (@Json NVARCHAR(MAX))
RETURNS TABLE
AS
RETURN
    WITH j AS (SELECT CASE WHEN ISJSON(@Json) = 1 THEN @Json ELSE N'{}' END AS DOC),
    direct AS (
        /* Solo valores cortos: TRY_CONVERT de un texto de más de 4000 caracteres falla (Msg 8152) */
        SELECT f.OBJECT_TYPE, TRY_CONVERT(INT, CASE WHEN LEN(x.[value]) <= 20 THEN x.[value] END) AS OBJECT_ID
          FROM j CROSS APPLY OPENJSON(j.DOC) x
          JOIN dbo.LOG_TRACK_OBJECT_FIELDS f ON f.FIELD_NAME = x.[key] COLLATE DATABASE_DEFAULT),
    orderDetail AS (
        SELECT d.REFERENCE_ID, d.CUSTOMER_ORDER_ID
          FROM j JOIN dbo.customer_order_details d WITH (NOLOCK)
            ON d.CUSTOMER_ORDER_DETAIL_ID = TRY_CONVERT(INT, JSON_VALUE(j.DOC, '$.CustomerOrderDetailId'))),
    refs AS (
        SELECT OBJECT_ID AS REFERENCE_ID FROM direct WHERE OBJECT_TYPE = 2
        UNION SELECT REFERENCE_ID FROM orderDetail)
    SELECT DISTINCT o.OBJECT_TYPE, o.OBJECT_ID
      FROM (SELECT OBJECT_TYPE, OBJECT_ID FROM direct
            UNION ALL SELECT CAST(2 AS TINYINT), REFERENCE_ID FROM orderDetail
            UNION ALL SELECT CAST(3 AS TINYINT), CUSTOMER_ORDER_ID FROM orderDetail
            UNION ALL SELECT CAST(1 AS TINYINT), r.ITEM_ID FROM refs JOIN dbo.item_references r WITH (NOLOCK) ON r.REFERENCE_ID = refs.REFERENCE_ID) o
     WHERE o.OBJECT_ID > 0;
GO

/* ---------- 3. Índice de objetos ---------- */
IF OBJECT_ID('dbo.LOG_TRACK_OBJECTS') IS NULL
    CREATE TABLE dbo.LOG_TRACK_OBJECTS
    (
        OBJECT_TYPE   TINYINT  NOT NULL,
        OBJECT_ID     INT      NOT NULL,
        MODIFIED_DATE DATETIME NOT NULL,
        TRACK_ID      BIGINT   NOT NULL,
        CONSTRAINT PK_LOG_TRACK_OBJECTS PRIMARY KEY CLUSTERED (OBJECT_TYPE, OBJECT_ID, MODIFIED_DATE, TRACK_ID)
    );
GO
IF OBJECT_ID('dbo.LOG_TRACK_OBJECTS_BACKFILL') IS NULL
    CREATE TABLE dbo.LOG_TRACK_OBJECTS_BACKFILL
    (
        ID          TINYINT  NOT NULL CONSTRAINT PK_LOG_TRACK_OBJECTS_BACKFILL PRIMARY KEY DEFAULT (1),
        START_ID    BIGINT   NOT NULL,   -- movimientos <= START_ID los carga el proceso masivo; los mayores, el trigger
        LAST_ID     BIGINT   NOT NULL,   -- último movimiento procesado por el proceso masivo
        FINISHED    BIT      NOT NULL DEFAULT (0),
        UPDATED_AT  DATETIME NOT NULL DEFAULT (GETDATE())
    );
GO

/* El trigger se crea antes de la carga masiva: desde este punto todo movimiento nuevo queda indexado. */
CREATE OR ALTER TRIGGER log.TRG_LOG_TRACK_INDEX_OBJECTS
   ON log.track AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    /* DATA_LOG es TEXT: no está disponible en "inserted"; se lee de la fila ya insertada. */
    INSERT INTO dbo.LOG_TRACK_OBJECTS (OBJECT_TYPE, OBJECT_ID, MODIFIED_DATE, TRACK_ID)
    SELECT DISTINCT o.OBJECT_TYPE, o.OBJECT_ID, t.MODIFIED_DATE, t.ID
      FROM inserted i
      JOIN log.track t ON t.ID = i.ID
     CROSS APPLY dbo.FN_TRACK_OBJECTS(CAST(t.DATA_LOG AS NVARCHAR(MAX))) o;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LOG_TRACK_OBJECTS_BACKFILL)
    INSERT INTO dbo.LOG_TRACK_OBJECTS_BACKFILL (ID, START_ID, LAST_ID)
    SELECT 1, ISNULL(MAX(ID), 0), 0 FROM log.track;
GO

CREATE OR ALTER PROCEDURE dbo.SP_BACKFILL_TRACK_OBJECTS
    @BatchSize  INT = 20000,
    @MaxBatches INT = NULL      -- NULL = hasta terminar
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;   -- un error detiene la carga sin marcar el lote como procesado
    DECLARE @StartId BIGINT, @LastId BIGINT, @To BIGINT, @Batches INT = 0;
    SELECT @StartId = START_ID, @LastId = LAST_ID FROM dbo.LOG_TRACK_OBJECTS_BACKFILL WHERE ID = 1;

    WHILE @LastId < @StartId AND (@MaxBatches IS NULL OR @Batches < @MaxBatches)
    BEGIN
        SET @To = CASE WHEN @LastId + @BatchSize > @StartId THEN @StartId ELSE @LastId + @BatchSize END;

        BEGIN TRAN;
            INSERT INTO dbo.LOG_TRACK_OBJECTS (OBJECT_TYPE, OBJECT_ID, MODIFIED_DATE, TRACK_ID)
            SELECT DISTINCT o.OBJECT_TYPE, o.OBJECT_ID, t.MODIFIED_DATE, t.ID
              FROM log.track t WITH (NOLOCK)
             CROSS APPLY dbo.FN_TRACK_OBJECTS(CAST(t.DATA_LOG AS NVARCHAR(MAX))) o
             WHERE t.ID > @LastId AND t.ID <= @To;

            UPDATE dbo.LOG_TRACK_OBJECTS_BACKFILL SET LAST_ID = @To, FINISHED = CASE WHEN @To >= @StartId THEN 1 ELSE 0 END, UPDATED_AT = GETDATE() WHERE ID = 1;
        COMMIT;

        SET @LastId = @To;
        SET @Batches += 1;
    END

    SELECT START_ID, LAST_ID, FINISHED, UPDATED_AT FROM dbo.LOG_TRACK_OBJECTS_BACKFILL WHERE ID = 1;
END
GO

/* ---------- 4. Consultas ---------- */
CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_OBJECT_TYPES
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OBJECT_TYPE, NAME, HINT FROM dbo.LOG_TRACK_OBJECT_TYPES ORDER BY DISPLAY_ORDER;
END
GO

/* Descripción legible de un objeto (también la usa el detalle). */
CREATE OR ALTER FUNCTION dbo.FN_TRACK_OBJECT_DESCRIPTION (@ObjectType TINYINT, @ObjectId INT)
RETURNS TABLE
AS
RETURN
    SELECT TOP 1 DESCRIPTION FROM (
        SELECT CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME) AS DESCRIPTION FROM dbo.items i WHERE @ObjectType = 1 AND i.ITEM_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME, N' - ', r.REFERENCE_NAME) FROM dbo.item_references r JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID WHERE @ObjectType = 2 AND r.REFERENCE_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'Pedido ', o.ORDER_NUMBER, N' - ', c.CUSTOMER_NAME) FROM dbo.customer_orders o JOIN dbo.customers c ON c.CUSTOMER_ID = o.CUSTOMER_ID WHERE @ObjectType = 3 AND o.CUSTOMER_ORDER_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'OC ', p.ORDER_NUMBER, N' - ', v.PROVIDER_NAME) FROM dbo.purchase_orders p JOIN dbo.providers v ON v.PROVIDER_ID = p.PROVIDER_ID WHERE @ObjectType = 4 AND p.PURCHASE_ORDER_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'Reserva ', r.RESERVATION_NUMBER, N' - ', c.CUSTOMER_NAME) FROM dbo.customer_reservations r JOIN dbo.customers c ON c.CUSTOMER_ID = r.CUSTOMER_ID WHERE @ObjectType = 5 AND r.CUSTOMER_RESERVATION_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'Traslado a proceso ', p.CUSTOMER_ORDER_IN_PROCESS_ID, N' - Pedido ', o.ORDER_NUMBER) FROM dbo.customer_orders_in_process p JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = p.CUSTOMER_ORDER_ID WHERE @ObjectType = 6 AND p.CUSTOMER_ORDER_IN_PROCESS_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'Despacho ', s.CUSTOMER_ORDER_SHIPMENT_ID, N' - Pedido ', o.ORDER_NUMBER) FROM dbo.customer_order_shipments s JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = s.CUSTOMER_ORDER_ID WHERE @ObjectType = 7 AND s.CUSTOMER_ORDER_SHIPMENT_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'Ajuste ', a.ADJUSTMENT_ID, N' - ', CONVERT(VARCHAR(10), a.ADJUSTMENT_DATE, 103)) FROM dbo.adjustments a WHERE @ObjectType = 8 AND a.ADJUSTMENT_ID = @ObjectId
        UNION ALL SELECT CONCAT(N'Traslado ', w.WAREHOUSE_TRANSFER_ID, N' - ', CONVERT(VARCHAR(10), w.TRANSFER_DATE, 103)) FROM dbo.warehouse_transfers w WHERE @ObjectType = 9 AND w.WAREHOUSE_TRANSFER_ID = @ObjectId
        UNION ALL SELECT CONCAT(c.CUSTOMER_NAME, N' (', c.IDENTITY_NUMBER, N')') FROM dbo.customers c WHERE @ObjectType = 10 AND c.CUSTOMER_ID = @ObjectId
        UNION ALL SELECT CONCAT(v.PROVIDER_NAME, N' (', v.PROVIDER_CODE, N')') FROM dbo.providers v WHERE @ObjectType = 11 AND v.PROVIDER_ID = @ObjectId
        UNION ALL SELECT CONCAT(e.FULL_NAME, N' (', e.IDENTITY_NUMBER, N')') FROM dbo.employees e WHERE @ObjectType = 12 AND e.EMPLOYEE_ID = @ObjectId
    ) x;
GO

CREATE OR ALTER PROCEDURE dbo.SP_SEARCH_TRACK_OBJECTS
    @ObjectType TINYINT,
    @Text       NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SET @Text = LTRIM(RTRIM(ISNULL(@Text, N'')));
    IF LEN(@Text) < 2
    BEGIN
        SELECT CAST(NULL AS INT) AS OBJECT_ID, CAST(NULL AS NVARCHAR(400)) AS DESCRIPTION WHERE 1 = 0;
        RETURN;
    END

    DECLARE @Like NVARCHAR(110) = N'%' + @Text + N'%';
    DECLARE @Number INT = TRY_CONVERT(INT, @Text);

    SELECT TOP 30 OBJECT_ID, DESCRIPTION FROM (
        SELECT i.ITEM_ID AS OBJECT_ID, CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME) AS DESCRIPTION
          FROM dbo.items i WHERE @ObjectType = 1 AND (i.INTERNAL_REFERENCE LIKE @Like OR i.ITEM_NAME LIKE @Like)
        UNION ALL
        SELECT r.REFERENCE_ID, CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME, N' - ', r.REFERENCE_NAME)
          FROM dbo.item_references r JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID
         WHERE @ObjectType = 2 AND (i.INTERNAL_REFERENCE LIKE @Like OR i.ITEM_NAME LIKE @Like OR r.REFERENCE_NAME LIKE @Like)
        UNION ALL
        SELECT o.CUSTOMER_ORDER_ID, CONCAT(N'Pedido ', o.ORDER_NUMBER, N' - ', c.CUSTOMER_NAME)
          FROM dbo.customer_orders o JOIN dbo.customers c ON c.CUSTOMER_ID = o.CUSTOMER_ID
         WHERE @ObjectType = 3 AND (o.ORDER_NUMBER LIKE @Like OR c.CUSTOMER_NAME LIKE @Like OR c.IDENTITY_NUMBER LIKE @Like)
        UNION ALL
        SELECT p.PURCHASE_ORDER_ID, CONCAT(N'OC ', p.ORDER_NUMBER, N' - ', v.PROVIDER_NAME)
          FROM dbo.purchase_orders p JOIN dbo.providers v ON v.PROVIDER_ID = p.PROVIDER_ID
         WHERE @ObjectType = 4 AND (p.ORDER_NUMBER LIKE @Like OR p.IMPORT_NUMBER LIKE @Like OR v.PROVIDER_NAME LIKE @Like)
        UNION ALL
        SELECT r.CUSTOMER_RESERVATION_ID, CONCAT(N'Reserva ', r.RESERVATION_NUMBER, N' - ', c.CUSTOMER_NAME)
          FROM dbo.customer_reservations r JOIN dbo.customers c ON c.CUSTOMER_ID = r.CUSTOMER_ID
         WHERE @ObjectType = 5 AND (r.RESERVATION_NUMBER LIKE @Like OR c.CUSTOMER_NAME LIKE @Like OR c.IDENTITY_NUMBER LIKE @Like)
        UNION ALL
        SELECT p.CUSTOMER_ORDER_IN_PROCESS_ID, CONCAT(N'Traslado a proceso ', p.CUSTOMER_ORDER_IN_PROCESS_ID, N' - Pedido ', o.ORDER_NUMBER)
          FROM dbo.customer_orders_in_process p JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = p.CUSTOMER_ORDER_ID
         WHERE @ObjectType = 6 AND (p.CUSTOMER_ORDER_IN_PROCESS_ID = @Number OR o.ORDER_NUMBER LIKE @Like)
        UNION ALL
        SELECT s.CUSTOMER_ORDER_SHIPMENT_ID, CONCAT(N'Despacho ', s.CUSTOMER_ORDER_SHIPMENT_ID, N' - Pedido ', o.ORDER_NUMBER)
          FROM dbo.customer_order_shipments s JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = s.CUSTOMER_ORDER_ID
         WHERE @ObjectType = 7 AND (s.CUSTOMER_ORDER_SHIPMENT_ID = @Number OR o.ORDER_NUMBER LIKE @Like OR s.TRACKING_NUMBER LIKE @Like)
        UNION ALL
        SELECT a.ADJUSTMENT_ID, CONCAT(N'Ajuste ', a.ADJUSTMENT_ID, N' - ', CONVERT(VARCHAR(10), a.ADJUSTMENT_DATE, 103))
          FROM dbo.adjustments a WHERE @ObjectType = 8 AND a.ADJUSTMENT_ID = @Number
        UNION ALL
        SELECT w.WAREHOUSE_TRANSFER_ID, CONCAT(N'Traslado ', w.WAREHOUSE_TRANSFER_ID, N' - ', CONVERT(VARCHAR(10), w.TRANSFER_DATE, 103))
          FROM dbo.warehouse_transfers w WHERE @ObjectType = 9 AND w.WAREHOUSE_TRANSFER_ID = @Number
        UNION ALL
        SELECT c.CUSTOMER_ID, CONCAT(c.CUSTOMER_NAME, N' (', c.IDENTITY_NUMBER, N')')
          FROM dbo.customers c WHERE @ObjectType = 10 AND (c.CUSTOMER_NAME LIKE @Like OR c.IDENTITY_NUMBER LIKE @Like)
        UNION ALL
        SELECT v.PROVIDER_ID, CONCAT(v.PROVIDER_NAME, N' (', v.PROVIDER_CODE, N')')
          FROM dbo.providers v WHERE @ObjectType = 11 AND (v.PROVIDER_NAME LIKE @Like OR v.PROVIDER_CODE LIKE @Like OR v.IDENTITY_NUMBER LIKE @Like)
        UNION ALL
        SELECT e.EMPLOYEE_ID, CONCAT(e.FULL_NAME, N' (', e.IDENTITY_NUMBER, N')')
          FROM dbo.employees e WHERE @ObjectType = 12 AND (e.FULL_NAME LIKE @Like OR e.DISPLAY_NAME LIKE @Like OR e.IDENTITY_NUMBER LIKE @Like)
    ) x
    ORDER BY DESCRIPTION
    OPTION (RECOMPILE);
END
GO

/* Reemplaza la versión de 07: la entidad es obligatoria solo si no se busca por objeto. */
CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_LOG
    @EntityName   NVARCHAR(200) = NULL,
    @DateFrom     DATE,
    @DateTo       DATE,
    @Action       VARCHAR(10)   = NULL,   -- Added / Modified / Deleted
    @ModifierName NVARCHAR(100) = NULL,
    @ObjectType   TINYINT       = NULL,
    @ObjectId     INT           = NULL,
    @Skip         INT = 0,
    @Take         INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF LEN(LTRIM(RTRIM(ISNULL(@EntityName, N'')))) = 0 SET @EntityName = NULL;
    IF @ObjectType IS NULL OR @ObjectId IS NULL BEGIN SET @ObjectType = NULL; SET @ObjectId = NULL; END

    IF @EntityName IS NULL AND @ObjectId IS NULL
        THROW 50001, N'Seleccione la entidad o el objeto a consultar.', 1;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;

    IF LEN(LTRIM(RTRIM(ISNULL(@Action, '')))) = 0        SET @Action = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@ModifierName, N'')))) = 0 SET @ModifierName = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));

    IF @ObjectId IS NULL
        SELECT t.ID, t.ENTITY_NAME, ISNULL(n.DISPLAY_NAME, t.ENTITY_NAME) AS ENTITY_DISPLAY_NAME,
               CAST(t.ENTITY_KEY AS NVARCHAR(4000)) AS ENTITY_KEY, t.ACTION, t.MODIFIED_DATE, t.MODIFIER_NAME,
               COUNT(*) OVER () AS TOTAL_ROWS
          FROM log.track t
          LEFT JOIN dbo.LOG_TRACK_ENTITY_NAMES n ON n.ENTITY_NAME = t.ENTITY_NAME
         WHERE t.ENTITY_NAME = @EntityName
           AND t.MODIFIED_DATE >= @From AND t.MODIFIED_DATE < @To
           AND (@Action IS NULL OR t.ACTION = @Action)
           AND (@ModifierName IS NULL OR t.MODIFIER_NAME LIKE N'%' + @ModifierName + N'%')
         ORDER BY t.MODIFIED_DATE DESC, t.ID DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
        OPTION (RECOMPILE);
    ELSE
        SELECT t.ID, t.ENTITY_NAME, ISNULL(n.DISPLAY_NAME, t.ENTITY_NAME) AS ENTITY_DISPLAY_NAME,
               CAST(t.ENTITY_KEY AS NVARCHAR(4000)) AS ENTITY_KEY, t.ACTION, t.MODIFIED_DATE, t.MODIFIER_NAME,
               COUNT(*) OVER () AS TOTAL_ROWS
          FROM dbo.LOG_TRACK_OBJECTS o
          JOIN log.track t ON t.ID = o.TRACK_ID
          LEFT JOIN dbo.LOG_TRACK_ENTITY_NAMES n ON n.ENTITY_NAME = t.ENTITY_NAME
         WHERE o.OBJECT_TYPE = @ObjectType AND o.OBJECT_ID = @ObjectId
           AND o.MODIFIED_DATE >= @From AND o.MODIFIED_DATE < @To
           AND (@EntityName IS NULL OR t.ENTITY_NAME = @EntityName)
           AND (@Action IS NULL OR t.ACTION = @Action)
           AND (@ModifierName IS NULL OR t.MODIFIER_NAME LIKE N'%' + @ModifierName + N'%')
         ORDER BY t.MODIFIED_DATE DESC, t.ID DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
        OPTION (RECOMPILE);
END
GO

/* Descripción de los Id que aparecen en el movimiento y en el anterior (para el detalle). */
/* Descripción de los Id paramétricos del detalle (estados, motivos, bodegas, ciudades...). Solo para mostrar:
   no se indexan como objetos de búsqueda. */
CREATE OR ALTER FUNCTION dbo.FN_TRACK_LOOKUP_DESCRIPTION (@FieldName NVARCHAR(100), @Id INT)
RETURNS TABLE
AS
RETURN
    SELECT TOP 1 DESCRIPTION FROM (
        SELECT s.STATUS_DOCUMENT_TYPE_NAME AS DESCRIPTION FROM dbo.status_document_types s WHERE @FieldName IN (N'StatusDocumentTypeId', N'NewStatusDocumentTypeId', N'PreviousStatusDocumentTypeId') AND s.STATUS_DOCUMENT_TYPE_ID = @Id
        UNION ALL SELECT d.DOCUMENT_TYPE_NAME FROM dbo.document_types d WHERE @FieldName = N'DocumentTypeId' AND d.DOCUMENT_TYPE_ID = @Id
        UNION ALL SELECT r.CANCELLATION_REASON_NAME FROM dbo.cancellation_reasons r WHERE @FieldName = N'CancellationReasonId' AND r.CANCELLATION_REASON_ID = @Id
        UNION ALL SELECT r.MODIFICATION_REASON_NAME FROM dbo.modification_reasons r WHERE @FieldName = N'ModificationReasonId' AND r.MODIFICATION_REASON_ID = @Id
        UNION ALL SELECT r.CLOSE_REASON_NAME FROM dbo.close_customer_order_reasons r WHERE @FieldName = N'CloseCustomerOrderReasonId' AND r.CLOSE_CUSTOMER_ORDER_REASON_ID = @Id
        UNION ALL SELECT r.ADJUSTMENT_REASON_NAME FROM dbo.adjustment_reasons r WHERE @FieldName = N'AdjustmentReasonId' AND r.ADJUSTMENT_REASON_ID = @Id
        UNION ALL SELECT t.ADJUSTMENT_TYPE_NAME FROM dbo.adjustment_types t WHERE @FieldName = N'AdjustmentTypeId' AND t.ADJUSTMENT_TYPE_ID = @Id
        UNION ALL SELECT w.WAREHOUSE_NAME FROM dbo.warehouses w WHERE @FieldName IN (N'WarehouseId', N'OriginWarehouseId', N'DestinationWarehouseId') AND w.WAREHOUSE_ID = @Id
        UNION ALL SELECT c.CITY_NAME FROM dbo.cities c WHERE @FieldName = N'CityId' AND c.CITY_ID = @Id
        UNION ALL SELECT i.IDENTITY_TYPE_NAME FROM dbo.identity_types i WHERE @FieldName = N'IdentityTypeId' AND i.IDENTITY_TYPE_ID = @Id
        UNION ALL SELECT m.SHIPPING_METHOD_NAME FROM dbo.shipping_methods m WHERE @FieldName = N'ShippingMethodId' AND m.SHIPPING_METHOD_ID = @Id
        UNION ALL SELECT p.PROCESS_SATELLITE_NAME FROM dbo.process_satellites p WHERE @FieldName = N'ProcessSatelliteId' AND p.PROCESS_SATELLITE_ID = @Id
        UNION ALL SELECT CONCAT(l.LINE_CODE, N' - ', l.LINE_NAME) FROM dbo.lines l WHERE @FieldName = N'LineId' AND l.LINE_ID = @Id
        UNION ALL SELECT c.CURRENCY_NAME FROM dbo.currencies c WHERE @FieldName = N'CurrencyId' AND c.CURRENCY_ID = @Id
        UNION ALL SELECT u.MEASURE_UNIT_NAME FROM dbo.measure_units u WHERE @FieldName IN (N'FobMeasureUnitId', N'CifMeasureUnitId', N'MeasureUnitId') AND u.MEASURE_UNIT_ID = @Id
        UNION ALL SELECT a.FORWARDER_AGENT_NAME FROM dbo.forwarder_agents a WHERE @FieldName = N'ForwarderAgentId' AND a.FORWARDER_AGENT_ID = @Id
        UNION ALL SELECT CONCAT(m.SHIPMENT_METHOD_NAME, N' - ', a.FORWARDER_AGENT_NAME) FROM dbo.shipment_forwarder_agent_methods x JOIN dbo.shipment_methods m ON m.SHIPMENT_METHOD_ID = x.SHIPMENT_METHOD_ID JOIN dbo.forwarder_agents a ON a.FORWARDER_AGENT_ID = x.FORWARDER_AGENT_ID WHERE @FieldName = N'ShipmentForwarderAgentMethodId' AND x.SHIPMENT_FORWARDER_AGENT_METHOD_ID = @Id
        UNION ALL SELECT t.NAME FROM dbo.alarm_types t WHERE @FieldName = N'AlarmTypeId' AND t.ALARM_TYPE_ID = @Id
        UNION ALL SELECT t.NAME FROM dbo.notification_templates t WHERE @FieldName = N'NotificationTemplateId' AND t.NOTIFICATION_TEMPLATE_ID = @Id
    ) x;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_OBJECT_DESCRIPTIONS
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    DECLARE @Current NVARCHAR(MAX), @Previous NVARCHAR(MAX), @EntityName NVARCHAR(200), @KeyHash BINARY(32), @ModifiedDate DATETIME;
    SELECT @Current = CAST(DATA_LOG AS NVARCHAR(MAX)), @EntityName = ENTITY_NAME, @KeyHash = ENTITY_KEY_HASH, @ModifiedDate = MODIFIED_DATE
      FROM log.track WHERE ID = @Id;
    SELECT TOP 1 @Previous = CAST(x.DATA_LOG AS NVARCHAR(MAX))
      FROM log.track x
     WHERE x.ENTITY_NAME = @EntityName AND x.ENTITY_KEY_HASH = @KeyHash AND x.MODIFIED_DATE <= @ModifiedDate AND x.ID < @Id
     ORDER BY x.MODIFIED_DATE DESC, x.ID DESC;

    ;WITH v AS (
        /* Solo valores cortos: TRY_CONVERT de un texto de más de 4000 caracteres falla (Msg 8152) */
        SELECT x.[key] COLLATE DATABASE_DEFAULT AS FIELD_NAME, TRY_CONVERT(INT, CASE WHEN LEN(x.[value]) <= 20 THEN x.[value] END) AS OBJECT_ID FROM OPENJSON(CASE WHEN ISJSON(@Current) = 1 THEN @Current ELSE N'{}' END) x
        UNION
        SELECT x.[key] COLLATE DATABASE_DEFAULT, TRY_CONVERT(INT, CASE WHEN LEN(x.[value]) <= 20 THEN x.[value] END) FROM OPENJSON(CASE WHEN ISJSON(@Previous) = 1 THEN @Previous ELSE N'{}' END) x)
    SELECT v.FIELD_NAME, v.OBJECT_ID, d.DESCRIPTION
      FROM v
      JOIN dbo.LOG_TRACK_OBJECT_FIELDS f ON f.FIELD_NAME = v.FIELD_NAME
     CROSS APPLY dbo.FN_TRACK_OBJECT_DESCRIPTION(f.OBJECT_TYPE, v.OBJECT_ID) d
     WHERE v.OBJECT_ID > 0
    UNION ALL
    /* Id paramétricos (estado, motivos, bodegas...) */
    SELECT v.FIELD_NAME, v.OBJECT_ID, d.DESCRIPTION
      FROM v
     CROSS APPLY dbo.FN_TRACK_LOOKUP_DESCRIPTION(v.FIELD_NAME, v.OBJECT_ID) d
     WHERE v.OBJECT_ID > 0;
END
GO

/* ---------- 5. Carga del histórico (masiva, por lotes; se puede interrumpir y volver a ejecutar) ---------- */
EXEC dbo.SP_BACKFILL_TRACK_OBJECTS;
GO
