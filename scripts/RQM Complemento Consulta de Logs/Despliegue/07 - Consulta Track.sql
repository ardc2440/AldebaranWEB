/* =====================================================================================
   RQM Complemento Consulta de Logs - T6: Consulta de Track (log.track)
   -------------------------------------------------------------------------------------
   - LOG_TRACK_ENTITY_NAMES: catálogo de entidades trazadas con su nombre en español
     (es el filtro principal de la consulta; no se recorre log.track para armarlo).
   - SP_GET_TRACK_ENTITIES: entidades activas del catálogo.
   - SP_GET_TRACK_LOG: movimientos de una entidad en un rango de fechas (obligatorios),
     con filtros opcionales de acción, usuario y llave; paginado con TOTAL_ROWS.
     Usa IX_track_Entity_Date (T1).
   - SP_GET_TRACK_DETAIL: datos del movimiento y del movimiento anterior de la misma entidad
     y llave (para comparar campo / anterior / nuevo). Usa IX_track_Entity_Key (T1).
   Las credenciales ya quedan ofuscadas en log.track (T2): la consulta nunca las muestra en claro.
   Lectura sin bloqueos. Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.LOG_TRACK_ENTITY_NAMES') IS NULL
    CREATE TABLE dbo.LOG_TRACK_ENTITY_NAMES
    (
        ENTITY_NAME  NVARCHAR(200) NOT NULL CONSTRAINT PK_LOG_TRACK_ENTITY_NAMES PRIMARY KEY,
        DISPLAY_NAME NVARCHAR(150) NOT NULL,
        IS_ACTIVE    BIT NOT NULL CONSTRAINT DF_LOG_TRACK_ENTITY_NAMES_ACTIVE DEFAULT (1)
    );
GO

MERGE dbo.LOG_TRACK_ENTITY_NAMES AS t
USING (VALUES
        (N'PurchaseOrderActivity', N'Actividad de orden de compra'),
        (N'CustomerOrderActivity', N'Actividad de pedido'),
        (N'ForwarderAgent', N'Agente de transportadora'),
        (N'Adjustment', N'Ajuste de inventario'),
        (N'PurchaseOrderAdjustmentLog', N'Ajuste de orden de compra'),
        (N'Alarm', N'Alarma de documento'),
        (N'LocalWarehouseAlarm', N'Alarma de ingreso a bodega local'),
        (N'PurchaseOrderTransitAlarm', N'Alarma de orden en tránsito'),
        (N'Item', N'Artículo'),
        (N'ItemsArea', N'Artículo por área'),
        (N'Customer', N'Cliente'),
        (N'FtpWritingConnection', N'Conexión FTP de escritura'),
        (N'InventoryAutomationConnection', N'Conexión del autómata de inventario'),
        (N'NotificationProviderSetting', N'Configuración del proveedor de correo'),
        (N'CustomerContact', N'Contacto de cliente'),
        (N'NotificationDefinition', N'Definición de notificación'),
        (N'CanceledOrderShipment', N'Despacho cancelado'),
        (N'CustomerOrderShipment', N'Despacho de pedido'),
        (N'ModifiedOrderShipment', N'Despacho modificado'),
        (N'AutomataNotificationRecipient', N'Destinatario de notificaciones del autómata'),
        (N'CustomerOrderActivityDetail', N'Detalle de actividad de pedido'),
        (N'AdjustmentDetail', N'Detalle de ajuste de inventario'),
        (N'CustomerOrderShipmentDetail', N'Detalle de despacho de pedido'),
        (N'PurchaseOrderDetail', N'Detalle de orden de compra'),
        (N'CustomerOrderDetail', N'Detalle de pedido'),
        (N'CustomerReservationDetail', N'Detalle de reserva'),
        (N'CustomerOrderInProcessDetail', N'Detalle de traslado a proceso'),
        (N'WarehouseTransferDetail', N'Detalle de traslado entre bodegas'),
        (N'Packaging', N'Empaque'),
        (N'Employee', N'Empleado'),
        (N'ReferencesWarehouse', N'Existencia por bodega'),
        (N'PurchaseOrderApprovalRangeLog', N'Historial de tolerancias de ajustes'),
        (N'ShipmentForwarderAgentMethod', N'Método de envío del agente'),
        (N'PurchaseOrderNotification', N'Notificación de orden de compra'),
        (N'CustomerOrderNotification', N'Notificación de pedido'),
        (N'CustomerReservationNotification', N'Notificación de reserva'),
        (N'PurchaseOrder', N'Orden de compra'),
        (N'CanceledPurchaseOrder', N'Orden de compra cancelada'),
        (N'ModifiedPurchaseOrder', N'Orden de compra modificada'),
        (N'AutomataConnectivityErrorPattern', N'Patrón de error de conectividad del autómata'),
        (N'CustomerOrder', N'Pedido'),
        (N'CanceledCustomerOrder', N'Pedido cancelado'),
        (N'ClosedCustomerOrder', N'Pedido cerrado'),
        (N'ModifiedCustomerOrder', N'Pedido modificado'),
        (N'NotificationTemplate', N'Plantilla de notificación'),
        (N'EmployeePreference', N'Preferencia de empleado'),
        (N'Provider', N'Proveedor'),
        (N'ItemReference', N'Referencia'),
        (N'ProviderReference', N'Referencia del proveedor'),
        (N'CustomerReservation', N'Reserva'),
        (N'CanceledCustomerReservation', N'Reserva cancelada'),
        (N'ModifiedCustomerReservation', N'Reserva modificada'),
        (N'CancellationRequest', N'Solicitud de cancelación'),
        (N'UsersAlarmType', N'Tipo de alarma por usuario'),
        (N'PurchaseOrderApprovalRange', N'Tolerancia de ajustes de orden de compra'),
        (N'Forwarder', N'Transportadora'),
        (N'CanceledOrdersInProcess', N'Traslado a proceso cancelado'),
        (N'ModifiedOrdersInProcess', N'Traslado a proceso modificado'),
        (N'CustomerOrdersInProcess', N'Traslado de pedido a proceso'),
        (N'WarehouseTransfer', N'Traslado entre bodegas'),
        (N'VisualizedAlarm', N'Visualización de alarma de documento'),
        (N'VisualizedMinimumQuantityAlarm', N'Visualización de cantidad mínima'),
        (N'VisualizedLocalWarehouseAlarm', N'Visualización de ingreso a bodega local'),
        (N'VisualizedOutOfStockInventoryAlarm', N'Visualización de inventario sin stock'),
        (N'VisualizedAutomaticCustomerOrderInProcessModification', N'Visualización de modificación automática de traslado'),
        (N'VisualizedMinimumLocalWarehouseQuantityAlarm', N'Visualización de mínimo en bodega local'),
        (N'VisualizedPurchaseOrderTransitAlarm', N'Visualización de orden en tránsito'),
        (N'VisualizedAutomaticInProcess', N'Visualización de traslado automático')
) AS s (ENTITY_NAME, DISPLAY_NAME)
   ON t.ENTITY_NAME = s.ENTITY_NAME
 WHEN MATCHED AND t.DISPLAY_NAME <> s.DISPLAY_NAME THEN UPDATE SET DISPLAY_NAME = s.DISPLAY_NAME
 WHEN NOT MATCHED THEN INSERT (ENTITY_NAME, DISPLAY_NAME) VALUES (s.ENTITY_NAME, s.DISPLAY_NAME);
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_ENTITIES
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ENTITY_NAME, DISPLAY_NAME
      FROM dbo.LOG_TRACK_ENTITY_NAMES
     WHERE IS_ACTIVE = 1
     ORDER BY DISPLAY_NAME;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_LOG
    @EntityName   NVARCHAR(200),
    @DateFrom     DATE,
    @DateTo       DATE,
    @Action       VARCHAR(10)   = NULL,   -- Added / Modified / Deleted
    @ModifierName NVARCHAR(100) = NULL,
    @EntityKey    NVARCHAR(100) = NULL,   -- valor de la llave (p. ej. 6161)
    @Skip         INT = 0,
    @Take         INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF LEN(LTRIM(RTRIM(ISNULL(@EntityName, N'')))) = 0
        THROW 50001, N'La entidad es obligatoria.', 1;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;

    IF LEN(LTRIM(RTRIM(ISNULL(@Action, '')))) = 0        SET @Action = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@ModifierName, N'')))) = 0 SET @ModifierName = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@EntityKey, N'')))) = 0    SET @EntityKey = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));
    DECLARE @KeyPattern NVARCHAR(110) = CASE WHEN @EntityKey IS NULL THEN NULL ELSE N'%"' + @EntityKey + N'"%' END;

    SELECT t.ID,
           t.ENTITY_NAME,
           CAST(t.ENTITY_KEY AS NVARCHAR(4000)) AS ENTITY_KEY,
           t.ACTION,
           t.MODIFIED_DATE,
           t.MODIFIER_NAME,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM log.track t
     WHERE t.ENTITY_NAME = @EntityName
       AND t.MODIFIED_DATE >= @From
       AND t.MODIFIED_DATE <  @To
       AND (@Action IS NULL OR t.ACTION = @Action)
       AND (@ModifierName IS NULL OR t.MODIFIER_NAME LIKE N'%' + @ModifierName + N'%')
       AND (@KeyPattern IS NULL OR CAST(t.ENTITY_KEY AS NVARCHAR(4000)) LIKE @KeyPattern)
     ORDER BY t.MODIFIED_DATE DESC, t.ID DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
    OPTION (RECOMPILE);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_DETAIL
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    DECLARE @EntityName NVARCHAR(200), @KeyHash BINARY(32), @ModifiedDate DATETIME;

    SELECT @EntityName = ENTITY_NAME, @KeyHash = ENTITY_KEY_HASH, @ModifiedDate = MODIFIED_DATE
      FROM log.track
     WHERE ID = @Id;

    SELECT c.ID,
           c.ACTION,
           CAST(c.DATA_LOG AS NVARCHAR(MAX)) AS DATA_LOG,
           p.ID AS PREVIOUS_ID,
           CAST(p.DATA_LOG AS NVARCHAR(MAX)) AS PREVIOUS_DATA_LOG
      FROM log.track c
      OUTER APPLY (SELECT TOP 1 x.ID, x.DATA_LOG
                     FROM log.track x
                    WHERE x.ENTITY_NAME = @EntityName
                      AND x.ENTITY_KEY_HASH = @KeyHash
                      AND x.MODIFIED_DATE <= @ModifiedDate
                      AND x.ID < @Id
                    ORDER BY x.MODIFIED_DATE DESC, x.ID DESC) p
     WHERE c.ID = @Id;
END
GO
