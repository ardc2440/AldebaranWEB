/* =====================================================================================
   RQM Complemento Consulta de Logs - T10..T13: Logs de modificación y cancelación de
   despachos y de traslados a proceso.
   -------------------------------------------------------------------------------------
   SP_GET_ORDER_DOCUMENT_LOG(@LogType, @DateFrom, @DateTo, @SearchKey, @Skip, @Take)
     @LogType: SHIPMENT_MODIFICATION / SHIPMENT_CANCELLATION /
               IN_PROCESS_MODIFICATION / IN_PROCESS_CANCELLATION
     - Rango de fechas obligatorio (fecha de la modificación/cancelación), sin máximo.
     - Búsqueda por número de pedido, cliente, identificación, motivo, empleado o número del documento.
     - Paginado con TOTAL_ROWS. Lectura sin bloqueos.
   Mismo patrón que los logs existentes de pedidos (rol "Log de pedidos").
   Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_ORDER_DOCUMENT_LOG
    @LogType   VARCHAR(30),
    @DateFrom  DATE,
    @DateTo    DATE,
    @SearchKey NVARCHAR(100) = NULL,
    @Skip      INT = 0,
    @Take      INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF @LogType NOT IN ('SHIPMENT_MODIFICATION', 'SHIPMENT_CANCELLATION', 'IN_PROCESS_MODIFICATION', 'IN_PROCESS_CANCELLATION')
        THROW 50001, N'El tipo de log no es válido.', 1;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;

    IF LEN(LTRIM(RTRIM(ISNULL(@SearchKey, N'')))) = 0
        SET @SearchKey = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));
    DECLARE @Like NVARCHAR(110) = N'%' + @SearchKey + N'%';

    ;WITH logs AS (
        SELECT CONCAT('M', m.MODIFIED_CUSTOMER_SHIPMENT_ID) AS LOG_KEY, m.MODIFICATION_DATE AS EVENT_DATE,
               s.CUSTOMER_ORDER_SHIPMENT_ID AS DOCUMENT_ID, s.CUSTOMER_ORDER_ID, s.SHIPPING_DATE AS DOCUMENT_DATE,
               r.MODIFICATION_REASON_NAME AS REASON, m.EMPLOYEE_ID
          FROM dbo.modified_order_shipment m
          JOIN dbo.customer_order_shipments s ON s.CUSTOMER_ORDER_SHIPMENT_ID = m.CUSTOMER_ORDER_SHIPMENT_ID
          JOIN dbo.MODIFICATION_REASONS r ON r.MODIFICATION_REASON_ID = m.MODIFICATION_REASON_ID
         WHERE @LogType = 'SHIPMENT_MODIFICATION' AND m.MODIFICATION_DATE >= @From AND m.MODIFICATION_DATE < @To
        UNION ALL
        SELECT CONCAT('C', c.CUSTOMER_ORDER_SHIPMENT_ID), c.CANCELLATION_DATE,
               s.CUSTOMER_ORDER_SHIPMENT_ID, s.CUSTOMER_ORDER_ID, s.SHIPPING_DATE,
               r.CANCELLATION_REASON_NAME, c.EMPLOYEE_ID
          FROM dbo.canceled_order_shipment c
          JOIN dbo.customer_order_shipments s ON s.CUSTOMER_ORDER_SHIPMENT_ID = c.CUSTOMER_ORDER_SHIPMENT_ID
          JOIN dbo.cancellation_reasons r ON r.CANCELLATION_REASON_ID = c.CANCELLATION_REASON_ID
         WHERE @LogType = 'SHIPMENT_CANCELLATION' AND c.CANCELLATION_DATE >= @From AND c.CANCELLATION_DATE < @To
        UNION ALL
        SELECT CONCAT('M', m.MODIFIED_CUSTOMER_ORDER_IN_PROCESS_ID), m.MODIFICATION_DATE,
               p.CUSTOMER_ORDER_IN_PROCESS_ID, p.CUSTOMER_ORDER_ID, p.TRANSFER_DATETIME,
               r.MODIFICATION_REASON_NAME, m.EMPLOYEE_ID
          FROM dbo.modified_orders_in_process m
          JOIN dbo.customer_orders_in_process p ON p.CUSTOMER_ORDER_IN_PROCESS_ID = m.CUSTOMER_ORDER_IN_PROCESS_ID
          JOIN dbo.MODIFICATION_REASONS r ON r.MODIFICATION_REASON_ID = m.MODIFICATION_REASON_ID
         WHERE @LogType = 'IN_PROCESS_MODIFICATION' AND m.MODIFICATION_DATE >= @From AND m.MODIFICATION_DATE < @To
        UNION ALL
        SELECT CONCAT('C', c.CUSTOMER_ORDER_IN_PROCESS_ID), c.CANCELLATION_DATE,
               p.CUSTOMER_ORDER_IN_PROCESS_ID, p.CUSTOMER_ORDER_ID, p.TRANSFER_DATETIME,
               r.CANCELLATION_REASON_NAME, c.EMPLOYEE_ID
          FROM dbo.canceled_orders_in_process c
          JOIN dbo.customer_orders_in_process p ON p.CUSTOMER_ORDER_IN_PROCESS_ID = c.CUSTOMER_ORDER_IN_PROCESS_ID
          JOIN dbo.cancellation_reasons r ON r.CANCELLATION_REASON_ID = c.CANCELLATION_REASON_ID
         WHERE @LogType = 'IN_PROCESS_CANCELLATION' AND c.CANCELLATION_DATE >= @From AND c.CANCELLATION_DATE < @To
    )
    SELECT l.LOG_KEY, l.EVENT_DATE, l.DOCUMENT_ID, l.DOCUMENT_DATE, l.CUSTOMER_ORDER_ID,
           o.ORDER_NUMBER, c.CUSTOMER_NAME, l.REASON, e.FULL_NAME AS EMPLOYEE_NAME,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM logs l
      JOIN dbo.customer_orders o ON o.CUSTOMER_ORDER_ID = l.CUSTOMER_ORDER_ID
      JOIN dbo.customers c ON c.CUSTOMER_ID = o.CUSTOMER_ID
      JOIN dbo.employees e ON e.EMPLOYEE_ID = l.EMPLOYEE_ID
     WHERE @SearchKey IS NULL
        OR o.ORDER_NUMBER LIKE @Like OR c.CUSTOMER_NAME LIKE @Like OR c.IDENTITY_NUMBER LIKE @Like
        OR l.REASON LIKE @Like OR e.FULL_NAME LIKE @Like OR CAST(l.DOCUMENT_ID AS VARCHAR(12)) LIKE @Like
     ORDER BY l.EVENT_DATE DESC, l.LOG_KEY DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
    OPTION (RECOMPILE);
END
GO
