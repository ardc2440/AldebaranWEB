/* =====================================================================================
   RQM Complemento Consulta de Logs - T14a / T14b: Logs de órdenes de compra
   -------------------------------------------------------------------------------------
   - SP_GET_PURCHASE_ORDER_ADJUSTMENT_LOG: solicitudes de aprobación de ajustes de OC
     (quién y cuándo la creó, si se aprobó o devolvió, quién, cuándo y motivo).
   - SP_GET_APPROVAL_RANGE_LOG: cambios en las tolerancias de ajuste de OC
     (PURCHASE_ORDER_APPROVAL_RANGE_LOG), valor anterior vs. nuevo.
   Rango de fechas obligatorio, búsqueda, paginado con TOTAL_ROWS, lectura sin bloqueos.
   Rol "Log de ordenes de compra". Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_PURCHASE_ORDER_ADJUSTMENT_LOG
    @DateFrom  DATE,
    @DateTo    DATE,
    @SearchKey NVARCHAR(100) = NULL,
    @Skip      INT = 0,
    @Take      INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;
    IF LEN(LTRIM(RTRIM(ISNULL(@SearchKey, N'')))) = 0 SET @SearchKey = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));
    DECLARE @Like NVARCHAR(110) = N'%' + @SearchKey + N'%';

    /* Una fila por solicitud de aprobación de ajustes (intento de confirmación que pasó la OC
       a "Ajuste en aprobación", motivo 'Solicitud aprobación ajustes'), con su resolución:
       el primer registro de PURCHASE_ORDER_ADJUSTMENT_LOG de la OC posterior a la solicitud.
       Resolución con estado Pendiente = Devuelta; cualquier otro = Aprobada; sin registro = En aprobación. */
    ;WITH requests AS (
        SELECT m.MODIFIED_PURCHASE_ORDER_ID AS REQUEST_ID, m.PURCHASE_ORDER_ID,
               m.MODIFICATION_DATE AS REQUEST_DATE, m.EMPLOYEE_ID AS REQUESTED_BY_ID
          FROM dbo.modified_purchase_orders m
          JOIN dbo.modification_reasons r ON r.MODIFICATION_REASON_ID = m.MODIFICATION_REASON_ID
         WHERE r.MODIFICATION_REASON_NAME = N'Solicitud aprobación ajustes' AND r.DOCUMENT_TYPE_ID = 1
           AND m.MODIFICATION_DATE >= @From AND m.MODIFICATION_DATE < @To),
    resolved AS (
        SELECT q.*, a.CREATED_DATE AS RESOLUTION_DATE, a.EMPLOYE_ID AS RESOLVED_BY_ID, a.REASON,
               CASE WHEN a.PURCHASE_ORDER_ADJUSTMENT_LOG_ID IS NULL THEN N'En aprobación'
                    WHEN a.NEW_STATUS_DOCUMENT_TYPE_ID = 1 THEN N'Devuelta'
                    ELSE N'Aprobada' END AS RESULT
          FROM requests q
          OUTER APPLY (SELECT TOP (1) l.PURCHASE_ORDER_ADJUSTMENT_LOG_ID, l.CREATED_DATE, l.EMPLOYE_ID, l.REASON, l.NEW_STATUS_DOCUMENT_TYPE_ID
                         FROM dbo.PURCHASE_ORDER_ADJUSTMENT_LOG l
                        WHERE l.PURCHASE_ORDER_ID = q.PURCHASE_ORDER_ID AND l.CREATED_DATE >= q.REQUEST_DATE
                        ORDER BY l.CREATED_DATE, l.PURCHASE_ORDER_ADJUSTMENT_LOG_ID) a)
    SELECT x.REQUEST_ID, x.REQUEST_DATE, p.PURCHASE_ORDER_ID, p.ORDER_NUMBER, v.PROVIDER_NAME,
           rq.FULL_NAME AS REQUESTED_BY, x.RESULT, x.RESOLUTION_DATE, rs.FULL_NAME AS RESOLVED_BY, x.REASON,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM resolved x
      JOIN dbo.purchase_orders p ON p.PURCHASE_ORDER_ID = x.PURCHASE_ORDER_ID
      JOIN dbo.providers v ON v.PROVIDER_ID = p.PROVIDER_ID
      JOIN dbo.employees rq ON rq.EMPLOYEE_ID = x.REQUESTED_BY_ID
      LEFT JOIN dbo.employees rs ON rs.EMPLOYEE_ID = x.RESOLVED_BY_ID
     WHERE @SearchKey IS NULL OR p.ORDER_NUMBER LIKE @Like OR v.PROVIDER_NAME LIKE @Like OR rq.FULL_NAME LIKE @Like
           OR rs.FULL_NAME LIKE @Like OR x.RESULT LIKE @Like OR x.REASON LIKE @Like
     ORDER BY x.REQUEST_DATE DESC, x.REQUEST_ID DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
    OPTION (RECOMPILE);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_APPROVAL_RANGE_LOG
    @DateFrom  DATE,
    @DateTo    DATE,
    @SearchKey NVARCHAR(100) = NULL,
    @Skip      INT = 0,
    @Take      INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;
    IF LEN(LTRIM(RTRIM(ISNULL(@SearchKey, N'')))) = 0 SET @SearchKey = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));
    DECLARE @Like NVARCHAR(110) = N'%' + @SearchKey + N'%';

    /* El valor "nuevo" de un cambio es el "anterior" del cambio siguiente del mismo rango;
       para el último cambio, el valor actual del rango. */
    ;WITH changes AS (
        SELECT l.*,
               LEAD(l.PREVIOUS_REQUESTED_QUANTITY_FROM)     OVER (PARTITION BY l.PURCHASE_ORDER_APPROVAL_RANGE_ID ORDER BY l.CHANGED_DATE, l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID) AS NEXT_FROM,
               LEAD(l.PREVIOUS_REQUESTED_QUANTITY_TO)       OVER (PARTITION BY l.PURCHASE_ORDER_APPROVAL_RANGE_ID ORDER BY l.CHANGED_DATE, l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID) AS NEXT_TO,
               LEAD(l.PREVIOUS_ALLOWED_DIFFERENCE_PERCENT)  OVER (PARTITION BY l.PURCHASE_ORDER_APPROVAL_RANGE_ID ORDER BY l.CHANGED_DATE, l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID) AS NEXT_PERCENT,
               LEAD(l.PREVIOUS_IS_ACTIVE)                   OVER (PARTITION BY l.PURCHASE_ORDER_APPROVAL_RANGE_ID ORDER BY l.CHANGED_DATE, l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID) AS NEXT_ACTIVE,
               LEAD(l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID) OVER (PARTITION BY l.PURCHASE_ORDER_APPROVAL_RANGE_ID ORDER BY l.CHANGED_DATE, l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID) AS NEXT_LOG_ID
          FROM dbo.PURCHASE_ORDER_APPROVAL_RANGE_LOG l)
    SELECT l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID AS LOG_ID, l.CHANGED_DATE AS EVENT_DATE,
           l.PREVIOUS_REQUESTED_QUANTITY_FROM, l.PREVIOUS_REQUESTED_QUANTITY_TO,
           l.PREVIOUS_ALLOWED_DIFFERENCE_PERCENT, l.PREVIOUS_IS_ACTIVE,
           CASE WHEN l.NEXT_LOG_ID IS NULL THEN r.REQUESTED_QUANTITY_FROM    ELSE l.NEXT_FROM    END AS REQUESTED_QUANTITY_FROM,
           CASE WHEN l.NEXT_LOG_ID IS NULL THEN r.REQUESTED_QUANTITY_TO      ELSE l.NEXT_TO      END AS REQUESTED_QUANTITY_TO,
           CASE WHEN l.NEXT_LOG_ID IS NULL THEN r.ALLOWED_DIFFERENCE_PERCENT ELSE l.NEXT_PERCENT END AS ALLOWED_DIFFERENCE_PERCENT,
           CASE WHEN l.NEXT_LOG_ID IS NULL THEN r.IS_ACTIVE                  ELSE l.NEXT_ACTIVE  END AS IS_ACTIVE,
           l.CHANGE_REASON, e.FULL_NAME AS EMPLOYEE_NAME,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM changes l
      JOIN dbo.PURCHASE_ORDER_APPROVAL_RANGES r ON r.PURCHASE_ORDER_APPROVAL_RANGE_ID = l.PURCHASE_ORDER_APPROVAL_RANGE_ID
      JOIN dbo.employees e ON e.EMPLOYEE_ID = l.CHANGED_BY_EMPLOYEE_ID
     WHERE l.CHANGED_DATE >= @From AND l.CHANGED_DATE < @To
       AND (@SearchKey IS NULL OR l.CHANGE_REASON LIKE @Like OR e.FULL_NAME LIKE @Like)
     ORDER BY l.CHANGED_DATE DESC, l.PURCHASE_ORDER_APPROVAL_RANGE_LOG_ID DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
    OPTION (RECOMPILE);
END
GO
