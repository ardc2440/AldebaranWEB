/* =====================================================================================
   RQM Complemento Consulta de Logs - T4: Consulta del log de inactivaciones y refrescos
   de alarmas de inventario (INVENTORY_ALARM_LOG).
   -------------------------------------------------------------------------------------
   SP_GET_INVENTORY_ALARM_LOG(@DateFrom, @DateTo, @AlarmTypeCode, @EventType, @SearchKey, @Skip, @Take)
     - Rango de fechas obligatorio (fecha del evento), sin máximo.
     - @AlarmTypeCode opcional: MINIMUM_QUANTITY / MINIMUM_LOCAL_WAREHOUSE / OUT_OF_STOCK.
     - @EventType opcional: R (refresco) / I (inactivación).
     - Búsqueda por referencia (código interno, artículo, color) u origen.
     - Paginado con TOTAL_ROWS. Lectura sin bloqueos (no afecta la operación).
   Ejecutar después de 04. Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_INVENTORY_ALARM_LOG
    @DateFrom      DATE,
    @DateTo        DATE,
    @AlarmTypeCode VARCHAR(30)   = NULL,
    @EventType     CHAR(1)       = NULL,
    @SearchKey     NVARCHAR(100) = NULL,
    @Skip          INT = 0,
    @Take          INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;

    IF LEN(LTRIM(RTRIM(ISNULL(@SearchKey, N'')))) = 0
        SET @SearchKey = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@AlarmTypeCode, '')))) = 0
        SET @AlarmTypeCode = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@EventType, '')))) = 0
        SET @EventType = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));

    SELECT l.INVENTORY_ALARM_LOG_ID,
           l.EVENT_DATE,
           l.ALARM_TYPE_CODE,
           ISNULL(t.NAME, l.ALARM_TYPE_CODE) AS ALARM_TYPE_NAME,
           l.ALARM_ID,
           l.REFERENCE_ID,
           CONCAT(N'[', i.INTERNAL_REFERENCE, N'] ', i.ITEM_NAME, N' - ', r.REFERENCE_NAME) AS REFERENCE_DESCRIPTION,
           l.EVENT_TYPE,
           l.PREVIOUS_QUANTITY,
           l.NEW_QUANTITY,
           l.PREVIOUS_MINIMUM_QUANTITY,
           l.NEW_MINIMUM_QUANTITY,
           l.PREVIOUS_CREATION_DATE,
           l.NEW_CREATION_DATE,
           l.INACTIVATION_DATE,
           l.VISUALIZATION_COUNT,
           l.VISUALIZATIONS_REMOVED,
           l.SOURCE,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM dbo.INVENTORY_ALARM_LOG l
      JOIN dbo.item_references r ON r.REFERENCE_ID = l.REFERENCE_ID
      JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID
      LEFT JOIN dbo.ALARM_VISUALIZATION_LOG_TYPES t ON t.CODE = l.ALARM_TYPE_CODE
     WHERE l.EVENT_DATE >= @From
       AND l.EVENT_DATE <  @To
       AND (@AlarmTypeCode IS NULL OR l.ALARM_TYPE_CODE = @AlarmTypeCode)
       AND (@EventType IS NULL OR l.EVENT_TYPE = @EventType)
       AND (@SearchKey IS NULL
            OR i.INTERNAL_REFERENCE LIKE N'%' + @SearchKey + N'%'
            OR i.ITEM_NAME          LIKE N'%' + @SearchKey + N'%'
            OR r.REFERENCE_NAME     LIKE N'%' + @SearchKey + N'%'
            OR l.SOURCE             LIKE N'%' + @SearchKey + N'%')
     ORDER BY l.EVENT_DATE DESC, l.INVENTORY_ALARM_LOG_ID DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
    OPTION (RECOMPILE);
END
GO
