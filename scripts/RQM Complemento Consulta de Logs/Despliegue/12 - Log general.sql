/* =====================================================================================
   RQM Complemento Consulta de Logs - T15: Log general (log.Logs)
   -------------------------------------------------------------------------------------
   - SP_GET_GENERAL_LOG_GROUPS: mensajes agrupados (plantilla + nivel + origen) con número
     de ocurrencias (GROUP BY directo), primera y última fecha. Filtros: rango de fechas
     obligatorio, nivel, origen y texto.
   - SP_GET_GENERAL_LOG_OCCURRENCES: ocurrencias de un grupo, paginadas.
   - SP_GET_GENERAL_LOG_DETAIL: detalle técnico de una ocurrencia (excepción y propiedades).
   - SP_GET_GENERAL_LOG_SOURCES: orígenes registrados (filtro).
   Requiere el script 01 (TEMPLATE_HASH, LEVEL_KEY e índices de log.Logs).
   Lectura sin bloqueos. Rol "Log del sistema". Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_GENERAL_LOG_GROUPS
    @DateFrom  DATE,
    @DateTo    DATE,
    @Level     NVARCHAR(20) = NULL,
    @Source    NVARCHAR(200) = NULL,
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
    IF LEN(LTRIM(RTRIM(ISNULL(@Level, N'')))) = 0 SET @Level = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@Source, N'')))) = 0 SET @Source = NULL;

    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));
    DECLARE @Like NVARCHAR(110) = N'%' + @SearchKey + N'%';

    /* Agrupación directa (GROUP BY) sobre IX_Logs_TimeStamp; el texto se lee solo de una fila por grupo. */
    CREATE TABLE #groups (TEMPLATE_HASH BINARY(32) NULL, LEVEL_KEY NVARCHAR(20) NULL, SOURCE NVARCHAR(200) NULL,
                          OCCURRENCES INT NOT NULL, FIRST_DATE DATETIME NOT NULL, LAST_DATE DATETIME NOT NULL, LAST_ID BIGINT NOT NULL);

    INSERT INTO #groups
    SELECT l.TEMPLATE_HASH, l.LEVEL_KEY, l.Source, COUNT(*), MIN(l.TimeStamp), MAX(l.TimeStamp), MAX(l.Id)
      FROM log.Logs l
     WHERE l.TimeStamp >= @From AND l.TimeStamp < @To
       AND (@Level IS NULL OR l.LEVEL_KEY = @Level)
       AND (@Source IS NULL OR l.Source = @Source)
     GROUP BY l.TEMPLATE_HASH, l.LEVEL_KEY, l.Source
    OPTION (RECOMPILE);

    SELECT CONVERT(VARCHAR(64), g.TEMPLATE_HASH, 2) AS GROUP_KEY, g.LEVEL_KEY AS LOG_LEVEL, g.SOURCE,
           s.MessageTemplate AS MESSAGE_TEMPLATE, g.OCCURRENCES, g.FIRST_DATE, g.LAST_DATE, g.LAST_ID,
           COUNT(*) OVER () AS TOTAL_ROWS
      FROM #groups g
      JOIN log.Logs s ON s.Id = g.LAST_ID
     WHERE @SearchKey IS NULL OR s.MessageTemplate LIKE @Like
     ORDER BY g.LAST_DATE DESC, g.LAST_ID DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_GENERAL_LOG_OCCURRENCES
    @GroupKey  VARCHAR(64),
    @DateFrom  DATE,
    @DateTo    DATE,
    @Level     NVARCHAR(20) = NULL,
    @Source    NVARCHAR(200) = NULL,
    @Skip      INT = 0,
    @Take      INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF @DateFrom IS NULL OR @DateTo IS NULL OR @DateFrom > @DateTo
        THROW 50002, N'El rango de fechas es obligatorio y la fecha inicial no puede ser mayor que la final.', 1;
    IF LEN(LTRIM(RTRIM(ISNULL(@Level, N'')))) = 0 SET @Level = NULL;
    IF LEN(LTRIM(RTRIM(ISNULL(@Source, N'')))) = 0 SET @Source = NULL;

    DECLARE @Hash BINARY(32) = CONVERT(BINARY(32), NULLIF(@GroupKey, ''), 2);
    DECLARE @From DATETIME = CAST(@DateFrom AS DATETIME);
    DECLARE @To   DATETIME = DATEADD(DAY, 1, CAST(@DateTo AS DATETIME));

    /* Ocurrencias de un grupo (IX_Logs_Template_TimeStamp); el texto se lee solo para la página. */
    ;WITH page AS (
        SELECT l.Id, COUNT(*) OVER () AS TOTAL_ROWS
          FROM log.Logs l
         WHERE ((@Hash IS NULL AND l.TEMPLATE_HASH IS NULL) OR l.TEMPLATE_HASH = @Hash)
           AND l.TimeStamp >= @From AND l.TimeStamp < @To
           AND (@Level IS NULL OR l.LEVEL_KEY = @Level)
           AND (@Source IS NULL OR l.Source = @Source)
         ORDER BY l.TimeStamp DESC, l.Id DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY)
    SELECT l.Id AS LOG_ID, l.TimeStamp AS EVENT_DATE, l.LEVEL_KEY AS LOG_LEVEL, l.Source AS SOURCE,
           l.Message AS MESSAGE, CAST(CASE WHEN l.Exception IS NULL THEN 0 ELSE 1 END AS BIT) AS HAS_EXCEPTION,
           p.TOTAL_ROWS
      FROM page p JOIN log.Logs l ON l.Id = p.Id
     ORDER BY l.TimeStamp DESC, l.Id DESC
    OPTION (RECOMPILE);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_GENERAL_LOG_DETAIL
    @LogId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT l.Id AS LOG_ID, l.TimeStamp AS EVENT_DATE, l.LEVEL_KEY AS LOG_LEVEL, l.Source AS SOURCE,
           l.Message AS MESSAGE, l.MessageTemplate AS MESSAGE_TEMPLATE, l.Exception AS EXCEPTION, l.Properties AS PROPERTIES
      FROM log.Logs l
     WHERE l.Id = @LogId;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_GENERAL_LOG_SOURCES
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    /* Orígenes registrados (índice IX_Logs_Template_TimeStamp incluye Source). */
    SELECT DISTINCT l.Source AS SOURCE FROM log.Logs l WHERE l.Source IS NOT NULL ORDER BY l.Source;
END
GO
