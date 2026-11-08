/* =====================================================================================
   RQM Homologados Competencia y Ajuste Reporte Referencias x Proveedor - T13
   SP_GET_COMPETITOR_HOMOLOGATION_REPORT: reporte de homologaciones con la competencia
   -------------------------------------------------------------------------------------
   Una fila por homologación con datos de compañía, línea, artículo y referencia PROMOS.
   - Jerarquía: prevalece el nivel más específico informado (@ReferenceIds > @ItemIds > @LineIds).
     Con referencias, incluye las homologaciones de esa referencia Y las de nivel artículo de su artículo
     (una homologación de artículo aplica a todas sus referencias).
   - Competencia: @CompetitorCompanyIds (lista), @CompetitorReference y @CompetitorProductName (contiene).
   - @IncludeInactive = 0 (por defecto): solo homologaciones activas de compañías activas.
   Requiere el script 02. Script re-ejecutable. Reversa: Reversa\04 - Reversa reporte homologaciones.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_COMPETITOR_HOMOLOGATION_REPORT
    @LineIds               VARCHAR(MAX) = NULL,
    @ItemIds               VARCHAR(MAX) = NULL,
    @ReferenceIds          VARCHAR(MAX) = NULL,
    @CompetitorCompanyIds  VARCHAR(MAX) = NULL,
    @CompetitorReference   VARCHAR(50)  = NULL,
    @CompetitorProductName VARCHAR(150) = NULL,
    @IncludeInactive       BIT          = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Lines      TABLE (LINE_ID      SMALLINT PRIMARY KEY);
    DECLARE @Items      TABLE (ITEM_ID      INT      PRIMARY KEY);
    DECLARE @References TABLE (REFERENCE_ID INT      PRIMARY KEY, ITEM_ID INT NOT NULL);
    DECLARE @Companies  TABLE (COMPETITOR_COMPANY_ID INT PRIMARY KEY);
    DECLARE @FilterLevel TINYINT = 0;   -- 0 = sin jerarquía, 1 = líneas, 2 = artículos, 3 = referencias
    DECLARE @FilterCompanies BIT = 0;

    IF LEN(LTRIM(RTRIM(ISNULL(@ReferenceIds, '')))) > 0
    BEGIN
        INSERT INTO @References (REFERENCE_ID, ITEM_ID)
             SELECT r.REFERENCE_ID, r.ITEM_ID
               FROM dbo.item_references r
              WHERE r.REFERENCE_ID IN (SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT(@ReferenceIds, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL);
        SET @FilterLevel = 3;
    END
    ELSE IF LEN(LTRIM(RTRIM(ISNULL(@ItemIds, '')))) > 0
    BEGIN
        INSERT INTO @Items (ITEM_ID)
             SELECT DISTINCT TRY_CAST(value AS INT) FROM STRING_SPLIT(@ItemIds, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        SET @FilterLevel = 2;
    END
    ELSE IF LEN(LTRIM(RTRIM(ISNULL(@LineIds, '')))) > 0
    BEGIN
        INSERT INTO @Lines (LINE_ID)
             SELECT DISTINCT TRY_CAST(value AS SMALLINT) FROM STRING_SPLIT(@LineIds, ',') WHERE TRY_CAST(value AS SMALLINT) IS NOT NULL;
        SET @FilterLevel = 1;
    END

    IF LEN(LTRIM(RTRIM(ISNULL(@CompetitorCompanyIds, '')))) > 0
    BEGIN
        INSERT INTO @Companies (COMPETITOR_COMPANY_ID)
             SELECT DISTINCT TRY_CAST(value AS INT) FROM STRING_SPLIT(@CompetitorCompanyIds, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
        SET @FilterCompanies = 1;
    END

    DECLARE @Reference VARCHAR(50)  = NULLIF(LTRIM(RTRIM(@CompetitorReference)), '');
    DECLARE @Product   VARCHAR(150) = NULLIF(LTRIM(RTRIM(@CompetitorProductName)), '');

    SELECT c.COMPETITOR_COMPANY_ID AS CompetitorCompanyId, c.COMPANY_NAME AS CompanyName, c.WEBSITE AS Website, c.IS_ACTIVE AS IsCompanyActive,
           l.LINE_ID AS LineId, l.LINE_NAME AS LineName,
           i.ITEM_ID AS ItemId, i.INTERNAL_REFERENCE AS InternalReference, i.ITEM_NAME AS ItemName,
           r.REFERENCE_ID AS ReferenceId, r.REFERENCE_CODE AS ReferenceCode, r.REFERENCE_NAME AS ReferenceName,
           h.COMPETITOR_HOMOLOGATION_ID AS CompetitorHomologationId, h.COMPETITOR_REFERENCE AS CompetitorReference,
           h.COMPETITOR_PRODUCT_NAME AS CompetitorProductName, h.PRODUCT_URL AS ProductUrl, h.COMPETITOR_PRICE AS CompetitorPrice,
           h.IMAGE_FILE_NAME AS ImageFileName, h.IS_ACTIVE AS IsActive
      FROM dbo.COMPETITOR_HOMOLOGATIONS h
      JOIN dbo.COMPETITOR_COMPANIES c ON c.COMPETITOR_COMPANY_ID = h.COMPETITOR_COMPANY_ID
      JOIN dbo.items i ON i.ITEM_ID = h.ITEM_ID
      JOIN dbo.lines l ON l.LINE_ID = i.LINE_ID
      LEFT JOIN dbo.item_references r ON r.REFERENCE_ID = h.REFERENCE_ID
     WHERE (@IncludeInactive = 1 OR (h.IS_ACTIVE = 1 AND c.IS_ACTIVE = 1))
       AND (@FilterLevel <> 3 OR EXISTS (SELECT 1 FROM @References f
                                          WHERE f.REFERENCE_ID = h.REFERENCE_ID
                                             OR (h.REFERENCE_ID IS NULL AND f.ITEM_ID = h.ITEM_ID)))
       AND (@FilterLevel <> 2 OR EXISTS (SELECT 1 FROM @Items f WHERE f.ITEM_ID = h.ITEM_ID))
       AND (@FilterLevel <> 1 OR EXISTS (SELECT 1 FROM @Lines f WHERE f.LINE_ID = l.LINE_ID))
       AND (@FilterCompanies = 0 OR EXISTS (SELECT 1 FROM @Companies f WHERE f.COMPETITOR_COMPANY_ID = h.COMPETITOR_COMPANY_ID))
       AND (@Reference IS NULL OR CHARINDEX(@Reference, h.COMPETITOR_REFERENCE) > 0) -- "contiene" literal: %, _ y [ no actúan como comodines
       AND (@Product   IS NULL OR CHARINDEX(@Product, h.COMPETITOR_PRODUCT_NAME) > 0)
     ORDER BY c.COMPANY_NAME, l.LINE_NAME, i.ITEM_NAME, ISNULL(r.REFERENCE_NAME, ''), h.COMPETITOR_REFERENCE
    OPTION (RECOMPILE);
END
GO

/* ---------- Verificación ---------- */
SELECT p.name AS Parametro FROM sys.parameters p WHERE p.object_id = OBJECT_ID('dbo.SP_GET_COMPETITOR_HOMOLOGATION_REPORT') ORDER BY p.parameter_id;
GO
