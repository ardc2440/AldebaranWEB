/* =====================================================================================
   RQM Complemento Consulta de Logs - T6c: Nombres de campo del Track para el usuario final
   -------------------------------------------------------------------------------------
   - LOG_TRACK_FIELD_NAMES: nombre técnico del campo (JSON de log.track) -> nombre en español.
       ENTITY_NAME = ''  : traducción general (aplica a todas las entidades).
       ENTITY_NAME = 'X' : traducción específica para la entidad X (manda sobre la general).
       IS_TECHNICAL = 1  : llave interna sin valor para el usuario; se muestra solo con "Ver campos técnicos".
     Un campo que no esté en el catálogo se muestra con su nombre técnico (nada se rompe).
   - SP_GET_TRACK_FIELD_NAMES(@EntityName): nombres resueltos para una entidad.
   Ejecutar después de 08. Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.LOG_TRACK_FIELD_NAMES') IS NULL
    CREATE TABLE dbo.LOG_TRACK_FIELD_NAMES
    (
        ENTITY_NAME  NVARCHAR(200) NOT NULL CONSTRAINT DF_LOG_TRACK_FIELD_NAMES_ENTITY DEFAULT (N''),
        FIELD_NAME   NVARCHAR(100) NOT NULL,
        DISPLAY_NAME NVARCHAR(150) NOT NULL,
        IS_TECHNICAL BIT NOT NULL CONSTRAINT DF_LOG_TRACK_FIELD_NAMES_TECH DEFAULT (0),
        CONSTRAINT PK_LOG_TRACK_FIELD_NAMES PRIMARY KEY (ENTITY_NAME, FIELD_NAME)
    );
GO

MERGE dbo.LOG_TRACK_FIELD_NAMES AS t
USING (VALUES
    (N'', N'ActionType', N'Tipo de acción', 0),
    (N'', N'Active', N'Activa', 0),
    (N'', N'ActivityDate', N'Fecha de la actividad', 0),
    (N'', N'ActivityDescription', N'Descripción de la actividad', 0),
    (N'', N'ActivityEmployeeId', N'Empleado de la actividad', 0),
    (N'', N'ActivityTypeId', N'Tipo de actividad', 0),
    (N'', N'AdjustmentDate', N'Fecha del ajuste', 0),
    (N'', N'AdjustmentDetailId', N'Id del detalle de ajuste', 1),
    (N'', N'AdjustmentId', N'Ajuste', 0),
    (N'', N'AdjustmentReasonId', N'Motivo del ajuste', 0),
    (N'', N'AdjustmentTypeId', N'Tipo de ajuste', 0),
    (N'', N'AlarmDate', N'Fecha de la alarma', 0),
    (N'', N'AlarmId', N'Alarma', 0),
    (N'', N'AlarmMessageId', N'Mensaje de la alarma', 0),
    (N'', N'AlarmMinimumQuantity', N'Cantidad mínima de alarma', 0),
    (N'', N'AlarmTypeId', N'Tipo de alarma', 0),
    (N'', N'AllowedDifferencePercent', N'Porcentaje de diferencia permitido', 0),
    (N'', N'ApplyPreorder', N'Aplica preorden', 0),
    (N'', N'AreaId', N'Área', 0),
    (N'', N'Brand', N'Marca', 0),
    (N'', N'CancellationDate', N'Fecha de cancelación', 0),
    (N'', N'CancellationReasonId', N'Motivo de cancelación', 0),
    (N'', N'CellPhone', N'Celular', 0),
    (N'', N'ChangedByEmployeeId', N'Modificado por', 0),
    (N'', N'ChangedDate', N'Fecha del cambio', 0),
    (N'', N'ChangeReason', N'Motivo del cambio', 0),
    (N'', N'CifCost', N'Costo CIF', 0),
    (N'', N'CifMeasureUnitId', N'Unidad de medida CIF', 0),
    (N'', N'CityId', N'Ciudad', 0),
    (N'', N'CloseCustomerOrderReasonId', N'Motivo de cierre', 0),
    (N'', N'CloseDate', N'Fecha de cierre', 0),
    (N'', N'ClosedCustomerOrderId', N'Id del cierre de pedido', 1),
    (N'', N'ContactPerson', N'Persona de contacto', 0),
    (N'', N'CreatedAt', N'Fecha de creación', 0),
    (N'', N'CreatedDate', N'Fecha de creación', 0),
    (N'', N'CreationDate', N'Fecha de creación', 0),
    (N'', N'CurrencyId', N'Moneda', 0),
    (N'', N'CustomerAddress', N'Dirección del cliente', 0),
    (N'', N'CustomerId', N'Cliente', 0),
    (N'', N'CustomerName', N'Nombre del cliente', 0),
    (N'', N'CustomerNotes', N'Observaciones del cliente', 0),
    (N'', N'CustomerOrderActivityDetailId', N'Id del detalle de actividad', 1),
    (N'', N'CustomerOrderActivityId', N'Actividad del pedido', 0),
    (N'', N'CustomerOrderDetailId', N'Id del detalle de pedido', 1),
    (N'', N'CustomerOrderId', N'Pedido', 0),
    (N'', N'CustomerOrderInProcessDetailId', N'Id del detalle de traslado a proceso', 1),
    (N'', N'CustomerOrderInProcessId', N'Traslado a proceso', 0),
    (N'', N'CustomerOrderList', N'Pedidos', 0),
    (N'', N'CustomerOrderNotificationId', N'Id de la notificación de pedido', 1),
    (N'', N'CustomerOrderShipmentDetailId', N'Id del detalle de despacho', 1),
    (N'', N'CustomerOrderShipmentId', N'Despacho', 0),
    (N'', N'CustomerReservationDetailId', N'Id del detalle de reserva', 1),
    (N'', N'CustomerReservationId', N'Reserva', 0),
    (N'', N'CustomerReservationNotificationId', N'Id de la notificación de reserva', 1),
    (N'', N'DatabaseName', N'Base de datos', 0),
    (N'', N'Deactivates', N'Desactiva', 0),
    (N'', N'DeliveredQuantity', N'Cantidad entregada', 0),
    (N'', N'DeliveryNote', N'Remisión', 0),
    (N'', N'Description', N'Descripción', 0),
    (N'', N'DestinationWarehouseId', N'Bodega destino', 0),
    (N'', N'DisableAlarms', N'Alarmas deshabilitadas', 0),
    (N'', N'DisplayName', N'Nombre para mostrar', 0),
    (N'', N'DocumentId', N'Documento', 0),
    (N'', N'DocumentNumber', N'Número de documento', 0),
    (N'', N'DocumentTypeId', N'Tipo de documento', 0),
    (N'', N'Email', N'Correo electrónico', 0),
    (N'', N'EmbarkationPort', N'Puerto de embarque', 0),
    (N'', N'Employee_Id', N'Empleado', 0),
    (N'', N'EmployeeId', N'Empleado', 0),
    (N'', N'EmployeeRecipientId', N'Empleado que recibe', 0),
    (N'', N'EnableNotifications', N'Notificaciones habilitadas', 0),
    (N'', N'EstimatedDeliveryDate', N'Fecha estimada de entrega', 0),
    (N'', N'ExecutionDate', N'Fecha de ejecución', 0),
    (N'', N'ExpectedReceiptDate', N'Fecha esperada de recibo', 0),
    (N'', N'ExpirationDate', N'Fecha de vencimiento', 0),
    (N'', N'Fax', N'Fax', 0),
    (N'', N'FobCost', N'Costo FOB', 0),
    (N'', N'FobMeasureUnitId', N'Unidad de medida FOB', 0),
    (N'', N'ForwarderAgentId', N'Agente de transportadora', 0),
    (N'', N'FtpWritingConnectionId', N'Id de la conexión FTP', 1),
    (N'', N'FullName', N'Nombre completo', 0),
    (N'', N'Height', N'Alto', 0),
    (N'', N'HostName', N'Servidor', 0),
    (N'', N'Id', N'Id', 1),
    (N'', N'IdentityNumber', N'Número de identificación', 0),
    (N'', N'IdentityTypeId', N'Tipo de identificación', 0),
    (N'', N'ImportNumber', N'Número de importación', 0),
    (N'', N'InternalNotes', N'Observaciones internas', 0),
    (N'', N'InternalReference', N'Referencia interna', 0),
    (N'', N'InventoryAutomationConnectionId', N'Id de la conexión del autómata', 1),
    (N'', N'InventoryQuantity', N'Cantidad en inventario', 0),
    (N'', N'IsActive', N'Activo', 0),
    (N'', N'IsCatalogVisible', N'Visible en catálogo', 0),
    (N'', N'IsDomesticProduct', N'Producto nacional', 0),
    (N'', N'IsExternalInventory', N'Inventario externo', 0),
    (N'', N'IsSaleOff', N'En oferta', 0),
    (N'', N'IsSoldOut', N'Agotado', 0),
    (N'', N'IsSpecialImport', N'Importación especial', 0),
    (N'', N'ItemId', N'Artículo', 0),
    (N'', N'ItemName', N'Nombre del artículo', 0),
    (N'', N'LastValidationDate', N'Fecha de la última validación', 0),
    (N'', N'Length', N'Largo', 0),
    (N'', N'LineId', N'Línea', 0),
    (N'', N'LocalWarehouseAlarmId', N'Alarma de ingreso a bodega local', 0),
    (N'', N'LoginUserId', N'Usuario de acceso', 1),
    (N'', N'Message', N'Mensaje', 0),
    (N'', N'MinimumLocalWarehouseQuantity', N'Cantidad mínima en bodega local', 0),
    (N'', N'MinimumQuantityAlarmId', N'Alarma de cantidad mínima', 0),
    (N'', N'MinimumQuantityPercent', N'Porcentaje de cantidad mínima', 0),
    (N'', N'ModificationDate', N'Fecha de modificación', 0),
    (N'', N'ModificationReasonId', N'Motivo de modificación', 0),
    (N'', N'ModifiedCustomerOrderId', N'Id de la modificación de pedido', 1),
    (N'', N'ModifiedCustomerOrderInProcessId', N'Id de la modificación de traslado', 1),
    (N'', N'ModifiedCustomerReservationId', N'Id de la modificación de reserva', 1),
    (N'', N'ModifiedCustomerShipmentId', N'Id de la modificación de despacho', 1),
    (N'', N'ModifiedPurchaseOrderId', N'Id de la modificación de OC', 1),
    (N'', N'Name', N'Nombre', 0),
    (N'', N'Nationalization', N'Nacionalización', 0),
    (N'', N'NewStatusDocumentTypeId', N'Nuevo estado', 0),
    (N'', N'Notes', N'Observaciones', 0),
    (N'', N'NotificationDate', N'Fecha de notificación', 0),
    (N'', N'NotificationDefinitionId', N'Id de la definición de notificación', 1),
    (N'', N'NotificationId', N'Id de la notificación', 1),
    (N'', N'NotificationMessage', N'Mensaje de la notificación', 0),
    (N'', N'NotificationProviderSettingId', N'Id de la configuración de correo', 1),
    (N'', N'NotificationSendingErrorMessage', N'Error de envío de la notificación', 0),
    (N'', N'NotificationState', N'Estado de la notificación', 0),
    (N'', N'NotificationTemplateId', N'Plantilla de notificación', 0),
    (N'', N'NotificationType', N'Tipo de notificación', 0),
    (N'', N'NotifiedMailList', N'Correos notificados', 0),
    (N'', N'OldExpectedReceiptDate', N'Fecha esperada de recibo anterior', 0),
    (N'', N'OrderDate', N'Fecha del pedido', 0),
    (N'', N'OrderedQuantity', N'Cantidad pedida', 0),
    (N'', N'OrderNumber', N'Número de documento', 0),
    (N'', N'OriginWarehouseId', N'Bodega origen', 0),
    (N'', N'OutOfStockInventoryAlarmId', N'Alarma de inventario sin stock', 0),
    (N'', N'PackagingId', N'Id del empaque', 1),
    (N'', N'Password', N'Contraseña', 0),
    (N'', N'Pattern', N'Patrón', 0),
    (N'', N'Phone', N'Teléfono', 0),
    (N'', N'Phone1', N'Teléfono 1', 0),
    (N'', N'Phone2', N'Teléfono 2', 0),
    (N'', N'PortNumber', N'Puerto', 0),
    (N'', N'Position', N'Cargo', 0),
    (N'', N'PreviousAllowedDifferencePercent', N'Porcentaje de diferencia anterior', 0),
    (N'', N'PreviousIsActive', N'Activo anterior', 0),
    (N'', N'PreviousRequestedQuantityFrom', N'Cantidad desde anterior', 0),
    (N'', N'PreviousRequestedQuantityTo', N'Cantidad hasta anterior', 0),
    (N'', N'ProcessDate', N'Fecha de proceso', 0),
    (N'', N'ProcessedQuantity', N'Cantidad en proceso', 0),
    (N'', N'ProcessSatelliteId', N'Satélite de proceso', 0),
    (N'', N'ProformaNumber', N'Número de proforma', 0),
    (N'', N'ProviderAddress', N'Dirección del proveedor', 0),
    (N'', N'ProviderCode', N'Código del proveedor', 0),
    (N'', N'ProviderId', N'Proveedor', 0),
    (N'', N'ProviderItemName', N'Nombre del artículo del proveedor', 0),
    (N'', N'ProviderName', N'Nombre del proveedor', 0),
    (N'', N'ProviderReference', N'Referencia del proveedor', 0),
    (N'', N'ProviderReferenceCode', N'Código de referencia del proveedor', 0),
    (N'', N'ProviderReferenceName', N'Nombre de referencia del proveedor', 0),
    (N'', N'PurchaseOrderActivityId', N'Id de la actividad de OC', 1),
    (N'', N'PurchaseOrderAdjustmentLogId', N'Id del ajuste de OC', 1),
    (N'', N'PurchaseOrderApprovalRangeId', N'Rango de tolerancia', 0),
    (N'', N'PurchaseOrderApprovalRangeLogId', N'Id del historial de tolerancia', 1),
    (N'', N'PurchaseOrderDetailId', N'Id del detalle de OC', 1),
    (N'', N'PurchaseOrderId', N'Orden de compra', 0),
    (N'', N'PurchaseOrderNotificationId', N'Id de la notificación de OC', 1),
    (N'', N'PurchaseOrderTransitAlarmId', N'Alarma de orden en tránsito', 0),
    (N'', N'PurchaseOrderVariation', N'Variación de orden de compra', 0),
    (N'', N'Quantity', N'Cantidad', 0),
    (N'', N'QueryParameters', N'Parámetros de consulta', 0),
    (N'', N'RealReceiptDate', N'Fecha real de recibo', 0),
    (N'', N'Reason', N'Motivo', 0),
    (N'', N'ReceivedQuantity', N'Cantidad recibida', 0),
    (N'', N'ReferenceCode', N'Código de la referencia', 0),
    (N'', N'ReferenceId', N'Referencia', 0),
    (N'', N'ReferenceList', N'Referencias', 0),
    (N'', N'ReferenceName', N'Nombre de la referencia', 0),
    (N'', N'RequestDate', N'Fecha de solicitud', 0),
    (N'', N'RequestedQuantity', N'Cantidad solicitada', 0),
    (N'', N'RequestedQuantityFrom', N'Cantidad desde', 0),
    (N'', N'RequestedQuantityTo', N'Cantidad hasta', 0),
    (N'', N'RequestEmployeeId', N'Empleado que solicita', 0),
    (N'', N'RequestId', N'Id de la solicitud', 1),
    (N'', N'ReservationDate', N'Fecha de la reserva', 0),
    (N'', N'ReservationNumber', N'Número de reserva', 0),
    (N'', N'ReservedQuantity', N'Cantidad reservada', 0),
    (N'', N'ResponseDate', N'Fecha de respuesta', 0),
    (N'', N'ResponseEmployeeId', N'Empleado que responde', 0),
    (N'', N'ResponseReason', N'Motivo de la respuesta', 0),
    (N'', N'RewriteFile', N'Reescribir archivo', 0),
    (N'', N'SendToCustomerOrder', N'Enviar al pedido', 0),
    (N'', N'ServerName', N'Servidor', 0),
    (N'', N'Settings', N'Configuración', 0),
    (N'', N'ShipmentForwarderAgentMethodId', N'Método de envío', 0),
    (N'', N'ShippingDate', N'Fecha de despacho', 0),
    (N'', N'ShippingMethodId', N'Método de envío', 0),
    (N'', N'StatusDocumentTypeId', N'Estado', 0),
    (N'', N'Subject', N'Asunto', 0),
    (N'', N'Target', N'Destino', 0),
    (N'', N'TrackingNumber', N'Número de guía', 0),
    (N'', N'TransferDate', N'Fecha del traslado', 0),
    (N'', N'TransferDatetime', N'Fecha y hora del traslado', 0),
    (N'', N'UpdatedAt', N'Fecha de actualización', 0),
    (N'', N'UpdatedDate', N'Fecha de actualización', 0),
    (N'', N'UserId', N'Usuario', 0),
    (N'', N'UserName', N'Nombre de usuario', 0),
    (N'', N'ValidationIntervalMinutes', N'Intervalo de validación (minutos)', 0),
    (N'', N'ValidationQuery', N'Consulta de validación', 0),
    (N'', N'Visualize', N'Visualizar', 0),
    (N'', N'Visualized_Date', N'Fecha de visualización', 0),
    (N'', N'VisualizedDate', N'Fecha de visualización', 0),
    (N'', N'Volume', N'Volumen', 0),
    (N'', N'WarehouseId', N'Bodega', 0),
    (N'', N'WarehouseTransferDetailId', N'Id del detalle de traslado', 1),
    (N'', N'WarehouseTransferId', N'Traslado entre bodegas', 0),
    (N'', N'Weight', N'Peso', 0),
    (N'', N'Width', N'Ancho', 0),
    (N'', N'WorkInProcessQuantity', N'Cantidad en proceso de traslado', 0),
    (N'', N'CustomerContactId', N'Id del contacto', 1),
    (N'', N'CustomerContactName', N'Nombre del contacto', 0),
    (N'', N'Email1', N'Correo electrónico 1', 0),
    (N'', N'Email2', N'Correo electrónico 2', 0),
    (N'', N'Mail1', N'Correo electrónico 1', 0),
    (N'', N'Mail2', N'Correo electrónico 2', 0),
    (N'', N'ForwarderId', N'Transportadora', 0),
    (N'', N'ForwarderName', N'Nombre de la transportadora', 0),
    (N'', N'ForwarderAddress', N'Dirección de la transportadora', 0),
    (N'', N'ForwarderAgentName', N'Nombre del agente', 0),
    (N'', N'ForwarderAgentAddress', N'Dirección del agente', 0),
    (N'', N'ShipmentMethodId', N'Método de envío', 0),
    (N'', N'Title', N'Cargo', 0),
    (N'', N'Employee_EmployeeId', N'Empleado', 0),
    (N'CustomerOrder', N'Notes', N'Observaciones del pedido', 0),
    (N'PurchaseOrder', N'OrderNumber', N'Número de orden de compra', 0),
    (N'CustomerOrder', N'OrderNumber', N'Número de pedido', 0),
    (N'PurchaseOrder', N'RequestDate', N'Fecha de solicitud de la OC', 0),
    (N'ItemReference', N'IsActive', N'Referencia activa', 0),
    (N'Item', N'IsActive', N'Artículo activo', 0),
    (N'ReferencesWarehouse', N'Quantity', N'Cantidad en la bodega', 0),
    (N'AdjustmentDetail', N'Quantity', N'Cantidad ajustada', 0),
    (N'WarehouseTransferDetail', N'Quantity', N'Cantidad trasladada', 0),
    (N'CustomerOrderShipmentDetail', N'DeliveredQuantity', N'Cantidad despachada', 0),
    (N'CustomerOrderInProcessDetail', N'ProcessedQuantity', N'Cantidad trasladada a proceso', 0)
) AS s (ENTITY_NAME, FIELD_NAME, DISPLAY_NAME, IS_TECHNICAL)
   ON t.ENTITY_NAME = s.ENTITY_NAME AND t.FIELD_NAME = s.FIELD_NAME
 WHEN MATCHED THEN UPDATE SET DISPLAY_NAME = s.DISPLAY_NAME, IS_TECHNICAL = s.IS_TECHNICAL
 WHEN NOT MATCHED THEN INSERT (ENTITY_NAME, FIELD_NAME, DISPLAY_NAME, IS_TECHNICAL) VALUES (s.ENTITY_NAME, s.FIELD_NAME, s.DISPLAY_NAME, s.IS_TECHNICAL);
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_TRACK_FIELD_NAMES
    @EntityName NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT g.FIELD_NAME,
           ISNULL(e.DISPLAY_NAME, g.DISPLAY_NAME) AS DISPLAY_NAME,
           ISNULL(e.IS_TECHNICAL, g.IS_TECHNICAL) AS IS_TECHNICAL
      FROM dbo.LOG_TRACK_FIELD_NAMES g
      LEFT JOIN dbo.LOG_TRACK_FIELD_NAMES e ON e.ENTITY_NAME = @EntityName AND e.FIELD_NAME = g.FIELD_NAME
     WHERE g.ENTITY_NAME = N''
    UNION ALL
    SELECT e.FIELD_NAME, e.DISPLAY_NAME, e.IS_TECHNICAL
      FROM dbo.LOG_TRACK_FIELD_NAMES e
     WHERE e.ENTITY_NAME = @EntityName
       AND NOT EXISTS (SELECT 1 FROM dbo.LOG_TRACK_FIELD_NAMES g WHERE g.ENTITY_NAME = N'' AND g.FIELD_NAME = e.FIELD_NAME);
END
GO

/* Reemplaza la versión de 08: agrega RECORD_DESCRIPTION (registro legible para el usuario).
   - Si la llave es un objeto conocido: su descripción (p. ej. "Pedido 0000039469 - CLIENTE").
   - Si no: nombre de la entidad + valor de la llave (p. ej. "Detalle de pedido 95364").
   Se calcula solo para la página que se devuelve. */
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
    DECLARE @Page TABLE (ID BIGINT PRIMARY KEY, TOTAL_ROWS INT);

    IF @ObjectId IS NULL
        INSERT INTO @Page (ID, TOTAL_ROWS)
        SELECT t.ID, COUNT(*) OVER ()
          FROM log.track t
         WHERE t.ENTITY_NAME = @EntityName
           AND t.MODIFIED_DATE >= @From AND t.MODIFIED_DATE < @To
           AND (@Action IS NULL OR t.ACTION = @Action)
           AND (@ModifierName IS NULL OR t.MODIFIER_NAME LIKE N'%' + @ModifierName + N'%')
         ORDER BY t.MODIFIED_DATE DESC, t.ID DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
        OPTION (RECOMPILE);
    ELSE
        INSERT INTO @Page (ID, TOTAL_ROWS)
        SELECT t.ID, COUNT(*) OVER ()
          FROM dbo.LOG_TRACK_OBJECTS o
          JOIN log.track t ON t.ID = o.TRACK_ID
         WHERE o.OBJECT_TYPE = @ObjectType AND o.OBJECT_ID = @ObjectId
           AND o.MODIFIED_DATE >= @From AND o.MODIFIED_DATE < @To
           AND (@EntityName IS NULL OR t.ENTITY_NAME = @EntityName)
           AND (@Action IS NULL OR t.ACTION = @Action)
           AND (@ModifierName IS NULL OR t.MODIFIER_NAME LIKE N'%' + @ModifierName + N'%')
         ORDER BY t.MODIFIED_DATE DESC, t.ID DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
        OPTION (RECOMPILE);

    SELECT t.ID, t.ENTITY_NAME, ISNULL(n.DISPLAY_NAME, t.ENTITY_NAME) AS ENTITY_DISPLAY_NAME,
           CAST(t.ENTITY_KEY AS NVARCHAR(4000)) AS ENTITY_KEY,
           CASE WHEN r.KNOWN = 1 THEN r.PARTS
                ELSE CONCAT(ISNULL(n.DISPLAY_NAME, t.ENTITY_NAME), N' ', r.PARTS) END AS RECORD_DESCRIPTION,
           t.ACTION, t.MODIFIED_DATE, t.MODIFIER_NAME, p.TOTAL_ROWS
      FROM @Page p
      JOIN log.track t ON t.ID = p.ID
      LEFT JOIN dbo.LOG_TRACK_ENTITY_NAMES n ON n.ENTITY_NAME = t.ENTITY_NAME
     OUTER APPLY (
        SELECT STRING_AGG(ISNULL(d.DESCRIPTION, CASE WHEN fn.IS_TECHNICAL = 0 THEN CONCAT(fn.DISPLAY_NAME, N' ', CAST(k.[value] AS NVARCHAR(400))) ELSE CAST(k.[value] AS NVARCHAR(400)) END), N' / ') AS PARTS,
               MAX(CASE WHEN d.DESCRIPTION IS NULL THEN 0 ELSE 1 END) AS KNOWN
          FROM OPENJSON(CASE WHEN ISJSON(CAST(t.ENTITY_KEY AS NVARCHAR(4000))) = 1 THEN CAST(t.ENTITY_KEY AS NVARCHAR(4000)) ELSE N'{}' END) k
          LEFT JOIN dbo.LOG_TRACK_OBJECT_FIELDS f ON f.FIELD_NAME = k.[key] COLLATE DATABASE_DEFAULT
          LEFT JOIN dbo.LOG_TRACK_FIELD_NAMES fn ON fn.ENTITY_NAME = N'' AND fn.FIELD_NAME = k.[key] COLLATE DATABASE_DEFAULT
         OUTER APPLY dbo.FN_TRACK_OBJECT_DESCRIPTION(f.OBJECT_TYPE, TRY_CONVERT(INT, k.[value])) d) r
     ORDER BY t.MODIFIED_DATE DESC, t.ID DESC;
END
GO
