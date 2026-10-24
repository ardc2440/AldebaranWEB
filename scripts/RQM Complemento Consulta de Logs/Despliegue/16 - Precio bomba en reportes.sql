/* =====================================================================================
   RQM Complemento Consulta de Logs - T20/T21/T22: Precio bomba en reportes
   -------------------------------------------------------------------------------------
   Filtro Precio bomba Sí (1) / No (0) / Todos (NULL, por defecto) como último parámetro opcional
   (las llamadas existentes no cambian). Requiere el script 14 (items.IS_BOMB_PRICE).
   - T20 SP_GET_ITEM_REFERENCES_REPORT: columna IsBombPrice + filtro @BombPrice. Además, estándar de filtros
     Sí/No/Todos: los switches @Only* pasan a condiciones de tres estados (@Is*/@Has*, NULL = Todos).
     Se despliega junto con la versión de la aplicación (cambian los nombres de parámetros).
   - T21 SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT: filtro @BombPrice (se agrega sobre la definición vigente,
     sin reescribir el SP; solo si aún no lo tiene).
   Script re-ejecutable. Reversa: Reversa\16 - Reversa precio bomba en reportes.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Respaldo de las definiciones originales (solo la primera vez) para la reversa */
IF OBJECT_ID('dbo.RQM_LOGS_OBJECT_BACKUP') IS NULL
    CREATE TABLE dbo.RQM_LOGS_OBJECT_BACKUP (OBJECT_NAME SYSNAME NOT NULL CONSTRAINT PK_RQM_LOGS_OBJECT_BACKUP PRIMARY KEY,
        OBJECT_TYPE CHAR(2) NOT NULL, DEFINITION NVARCHAR(MAX) NOT NULL,
        BACKUP_DATE DATETIME NOT NULL CONSTRAINT DF_RQM_LOGS_OBJECT_BACKUP_DATE DEFAULT (GETDATE()));
INSERT INTO dbo.RQM_LOGS_OBJECT_BACKUP (OBJECT_NAME, OBJECT_TYPE, DEFINITION)
SELECT o.name, o.type, m.definition
  FROM sys.objects o JOIN sys.sql_modules m ON m.object_id = o.object_id
 WHERE o.name IN ('SP_GET_ITEM_REFERENCES_REPORT', 'SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT')
   AND NOT EXISTS (SELECT 1 FROM dbo.RQM_LOGS_OBJECT_BACKUP b WHERE b.OBJECT_NAME = o.name);
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_ITEM_REFERENCES_REPORT
    @LineIds                                VARCHAR(MAX) = NULL,
    @ItemIds                                VARCHAR(MAX) = NULL,
    @ReferenceIds                           VARCHAR(MAX) = NULL,
    @IsItemActive                           BIT = NULL,   -- Condiciones Sí (1) / No (0) / Todos (NULL)
    @IsCatalogVisible                       BIT = NULL,
    @IsDomesticProduct                      BIT = NULL,
    @IsSpecialImport                        BIT = NULL,
    @IsSaleOff                              BIT = NULL,
    @BombPrice                              BIT = NULL,
    @IsReferenceActive                      BIT = NULL,
    @IsSoldOut                              BIT = NULL,
    @HasAlarmMinimumQuantity                BIT = NULL,
    @HasMinimumLocalWarehouseQuantity       BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Lines      TABLE (LINE_ID      SMALLINT PRIMARY KEY);
    DECLARE @Items      TABLE (ITEM_ID      INT      PRIMARY KEY);
    DECLARE @References TABLE (REFERENCE_ID INT      PRIMARY KEY);

    DECLARE @FilterLevel TINYINT = 0;   -- 0 = sin jerarquía, 1 = líneas, 2 = artículos, 3 = referencias

    IF LEN(LTRIM(RTRIM(ISNULL(@ReferenceIds, '')))) > 0
    BEGIN
        INSERT INTO @References (REFERENCE_ID)
             SELECT DISTINCT TRY_CAST(value AS INT)
               FROM STRING_SPLIT(@ReferenceIds, ',')
              WHERE TRY_CAST(value AS INT) IS NOT NULL;
        SET @FilterLevel = 3;
    END
    ELSE IF LEN(LTRIM(RTRIM(ISNULL(@ItemIds, '')))) > 0
    BEGIN
        INSERT INTO @Items (ITEM_ID)
             SELECT DISTINCT TRY_CAST(value AS INT)
               FROM STRING_SPLIT(@ItemIds, ',')
              WHERE TRY_CAST(value AS INT) IS NOT NULL;
        SET @FilterLevel = 2;
    END
    ELSE IF LEN(LTRIM(RTRIM(ISNULL(@LineIds, '')))) > 0
    BEGIN
        INSERT INTO @Lines (LINE_ID)
             SELECT DISTINCT TRY_CAST(value AS SMALLINT)
               FROM STRING_SPLIT(@LineIds, ',')
              WHERE TRY_CAST(value AS SMALLINT) IS NOT NULL;
        SET @FilterLevel = 1;
    END

    SELECT l.LINE_ID                            AS LineId,
           l.LINE_NAME                          AS LineName,
           i.ITEM_ID                            AS ItemId,
           i.ITEM_NAME                          AS ItemName,
           i.INTERNAL_REFERENCE                 AS InternalReference,
           i.PROVIDER_ITEM_NAME                 AS ProviderItemName,
           i.PROVIDER_REFERENCE                 AS ProviderReference,
           i.IS_ACTIVE                          AS IsItemActive,
           i.IS_CATALOG_VISIBLE                 AS IsCatalogVisible,
           i.IS_DOMESTIC_PRODUCT                AS IsDomesticProduct,
           i.IS_SPECIAL_IMPORT                  AS IsSpecialImport,
           i.IS_SALE_OFF                        AS IsSaleOff,
           i.IS_BOMB_PRICE                      AS IsBombPrice,
           r.REFERENCE_ID                       AS ReferenceId,
           r.REFERENCE_NAME                     AS ReferenceName,
           r.REFERENCE_CODE                     AS ReferenceCode,
           r.PROVIDER_REFERENCE_NAME            AS ProviderReferenceName,
           r.PROVIDER_REFERENCE_CODE            AS ProviderReferenceCode,
           r.IS_ACTIVE                          AS IsReferenceActive,
           r.IS_SOLD_OUT                        AS IsSoldOut,
           r.ALARM_MINIMUM_QUANTITY             AS AlarmMinimumQuantity,
           r.MINIMUM_LOCAL_WAREHOUSE_QUANTITY   AS MinimumLocalWarehouseQuantity,
           r.PURCHASE_ORDER_VARIATION           AS PurchaseOrderVariation
      FROM dbo.item_references r
      JOIN dbo.items           i ON i.ITEM_ID = r.ITEM_ID
      JOIN dbo.lines           l ON l.LINE_ID = i.LINE_ID
     WHERE (@FilterLevel <> 3 OR EXISTS (SELECT 1 FROM @References f WHERE f.REFERENCE_ID = r.REFERENCE_ID))
       AND (@FilterLevel <> 2 OR EXISTS (SELECT 1 FROM @Items      f WHERE f.ITEM_ID      = i.ITEM_ID))
       AND (@FilterLevel <> 1 OR EXISTS (SELECT 1 FROM @Lines      f WHERE f.LINE_ID      = l.LINE_ID))
       -- Condiciones de Artículo (NULL = Todos)
       AND (@IsItemActive      IS NULL OR i.IS_ACTIVE           = @IsItemActive)
       AND (@IsCatalogVisible  IS NULL OR i.IS_CATALOG_VISIBLE  = @IsCatalogVisible)
       AND (@IsDomesticProduct IS NULL OR i.IS_DOMESTIC_PRODUCT = @IsDomesticProduct)
       AND (@IsSpecialImport   IS NULL OR i.IS_SPECIAL_IMPORT   = @IsSpecialImport)
       AND (@IsSaleOff         IS NULL OR i.IS_SALE_OFF         = @IsSaleOff)
       AND (@BombPrice         IS NULL OR i.IS_BOMB_PRICE       = @BombPrice)
       -- Condiciones de Referencia (NULL = Todos)
       AND (@IsReferenceActive IS NULL OR r.IS_ACTIVE   = @IsReferenceActive)
       AND (@IsSoldOut         IS NULL OR r.IS_SOLD_OUT = @IsSoldOut)
       AND (@HasAlarmMinimumQuantity          IS NULL OR CAST(CASE WHEN r.ALARM_MINIMUM_QUANTITY           > 0 THEN 1 ELSE 0 END AS BIT) = @HasAlarmMinimumQuantity)
       AND (@HasMinimumLocalWarehouseQuantity IS NULL OR CAST(CASE WHEN r.MINIMUM_LOCAL_WAREHOUSE_QUANTITY > 0 THEN 1 ELSE 0 END AS BIT) = @HasMinimumLocalWarehouseQuantity)
     ORDER BY l.LINE_NAME, i.ITEM_NAME, r.REFERENCE_NAME
    OPTION (RECOMPILE);
END
GO

/* T21: SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT - parámetro y condición @BombPrice sobre la definición vigente */
DECLARE @d NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT'));
IF @d IS NOT NULL AND CHARINDEX('@BombPrice', @d) = 0
BEGIN
    IF CHARINDEX('@OutOfStockBIT BIT = NULL', @d) = 0 OR CHARINDEX('AND (@Offert IS NULL OR i.IS_SALE_OFF = @Offert)', @d) = 0
        THROW 50020, N'SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT no tiene la estructura esperada; revisar antes de desplegar.', 1;

    SET @d = REPLACE(@d, '@OutOfStockBIT BIT = NULL', '@OutOfStockBIT BIT = NULL,' + CHAR(13) + CHAR(10) + CHAR(9) + '@BombPrice BIT = NULL  -- T21: Precio bomba Sí (1) / No (0) / Todos (NULL)');
    SET @d = REPLACE(@d, 'AND (@Offert IS NULL OR i.IS_SALE_OFF = @Offert)', 'AND (@Offert IS NULL OR i.IS_SALE_OFF = @Offert)' + CHAR(13) + CHAR(10) + CHAR(9) + CHAR(9) + 'AND (@BombPrice IS NULL OR i.IS_BOMB_PRICE = @BombPrice)');
    SET @d = STUFF(@d, PATINDEX('%CREATE%PROCEDURE%', @d), CHARINDEX('PROCEDURE', @d) - PATINDEX('%CREATE%PROCEDURE%', @d), 'ALTER ');
    EXEC sys.sp_executesql @d;
    PRINT 'SP_GET_MINIMUM_WAREHOUSE_STOCK_REPORT actualizado';
END
GO
