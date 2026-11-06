/* =====================================================================================
   RQM Homologados Competencia y Ajuste Reporte Referencias x Proveedor - T1
   SP_GET_PROVIDER_REFERENCE_REPORT: filtros jerárquicos, condiciones Sí/No/Todos e inventario opcional
   -------------------------------------------------------------------------------------
   Construido sobre la definición de PRODUCCIÓN (traída a desarrollo el 2026-11-05).
   - Jerarquía: prevalece el nivel más específico informado (@ReferenceIds > @ItemIds > @LineIds).
     Listas de Ids separados por coma; valores no numéricos se descartan (TRY_CAST).
   - Condiciones Sí (1) / No (0) / Todos (NULL), igual que SP_GET_ITEM_REFERENCES_REPORT, sin cantidades mínimas:
     artículo activo, visible en página, producto nacional, importación especial, en oferta, precio bomba;
     referencia activa, referencia agotada.
   - @IncludeInventory (1 por defecto): 1 = una fila por referencia y bodega (como hoy);
     0 = una fila por referencia, sin bodega (WarehouseId/WarehouseName/Amount en NULL).
   - Compatible con la llamada actual (@ProviderId, @ReferenceIds): sin parámetros nuevos el resultado no cambia.
   - Respaldo de la definición anterior en RQM_HOMOLOGADOS_OBJECT_BACKUP (solo la primera vez) para la reversa.
   Script re-ejecutable. Reversa: Reversa\01 - Reversa reporte referencias del proveedor.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP') IS NULL
    CREATE TABLE dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP (OBJECT_NAME SYSNAME NOT NULL CONSTRAINT PK_RQM_HOMOLOGADOS_OBJECT_BACKUP PRIMARY KEY,
        OBJECT_TYPE CHAR(2) NOT NULL, DEFINITION NVARCHAR(MAX) NOT NULL,
        BACKUP_DATE DATETIME NOT NULL CONSTRAINT DF_RQM_HOMOLOGADOS_OBJECT_BACKUP_DATE DEFAULT (GETDATE()));
INSERT INTO dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP (OBJECT_NAME, OBJECT_TYPE, DEFINITION)
SELECT o.name, o.type, m.definition
  FROM sys.objects o JOIN sys.sql_modules m ON m.object_id = o.object_id
 WHERE o.name = 'SP_GET_PROVIDER_REFERENCE_REPORT'
   AND NOT EXISTS (SELECT 1 FROM dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP b WHERE b.OBJECT_NAME = o.name);
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_PROVIDER_REFERENCE_REPORT
    @ProviderId         INT          = NULL,
    @ReferenceIds       VARCHAR(MAX) = NULL,
    @LineIds            VARCHAR(MAX) = NULL,
    @ItemIds            VARCHAR(MAX) = NULL,
    @IsItemActive       BIT = NULL,   -- Condiciones Sí (1) / No (0) / Todos (NULL)
    @IsCatalogVisible   BIT = NULL,
    @IsDomesticProduct  BIT = NULL,
    @IsSpecialImport    BIT = NULL,
    @IsSaleOff          BIT = NULL,
    @BombPrice          BIT = NULL,
    @IsReferenceActive  BIT = NULL,
    @IsSoldOut          BIT = NULL,
    @IncludeInventory   BIT = 1       -- 1 = con bodegas (como hoy); 0 = solo referencias
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
             SELECT DISTINCT TRY_CAST(value AS INT) FROM STRING_SPLIT(@ReferenceIds, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL;
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

    SELECT a.PROVIDER_ID ProviderId, a.PROVIDER_CODE ProviderCode, a.PROVIDER_NAME ProviderName, a.PROVIDER_ADDRESS ProviderAddress,
           a.PHONE Phone, a.FAX Fax, a.EMAIL Email, a.CONTACT_PERSON ContactPerson,
           e.LINE_ID LineId, e.LINE_CODE LineCode, e.LINE_NAME LineName,
           d.ITEM_ID ItemId, d.INTERNAL_REFERENCE InternalReference, d.ITEM_NAME ItemName,
           c.REFERENCE_ID ReferenceId, c.REFERENCE_CODE ReferenceCode, c.REFERENCE_NAME ReferenceName, c.PROVIDER_REFERENCE_NAME ProviderReferenceName,
           c.ORDERED_QUANTITY ConfirmedAmount, c.RESERVED_QUANTITY ReservedAmount, c.INVENTORY_QUANTITY AvailableAmount,
           f.QUANTITY Amount, g.WAREHOUSE_ID WarehouseId, g.WAREHOUSE_NAME WarehouseName
      FROM dbo.providers a
      JOIN dbo.provider_references b ON b.PROVIDER_ID = a.PROVIDER_ID
      JOIN dbo.item_references c ON c.REFERENCE_ID = b.REFERENCE_ID
      JOIN dbo.items d ON d.ITEM_ID = c.ITEM_ID
      JOIN dbo.lines e ON e.LINE_ID = d.LINE_ID
      LEFT JOIN dbo.references_warehouse f ON @IncludeInventory = 1 AND f.REFERENCE_ID = c.REFERENCE_ID
      LEFT JOIN dbo.warehouses g ON g.WAREHOUSE_ID = f.WAREHOUSE_ID
     WHERE (@ProviderId IS NULL OR a.PROVIDER_ID = @ProviderId)
       AND (@FilterLevel <> 3 OR EXISTS (SELECT 1 FROM @References x WHERE x.REFERENCE_ID = c.REFERENCE_ID))
       AND (@FilterLevel <> 2 OR EXISTS (SELECT 1 FROM @Items x WHERE x.ITEM_ID = d.ITEM_ID))
       AND (@FilterLevel <> 1 OR EXISTS (SELECT 1 FROM @Lines x WHERE x.LINE_ID = e.LINE_ID))
       AND (@IsItemActive      IS NULL OR d.IS_ACTIVE           = @IsItemActive)
       AND (@IsCatalogVisible  IS NULL OR d.IS_CATALOG_VISIBLE  = @IsCatalogVisible)
       AND (@IsDomesticProduct IS NULL OR d.IS_DOMESTIC_PRODUCT = @IsDomesticProduct)
       AND (@IsSpecialImport   IS NULL OR d.IS_SPECIAL_IMPORT   = @IsSpecialImport)
       AND (@IsSaleOff         IS NULL OR d.IS_SALE_OFF         = @IsSaleOff)
       AND (@BombPrice         IS NULL OR d.IS_BOMB_PRICE       = @BombPrice)
       AND (@IsReferenceActive IS NULL OR c.IS_ACTIVE           = @IsReferenceActive)
       AND (@IsSoldOut         IS NULL OR c.IS_SOLD_OUT         = @IsSoldOut)
       AND (@IncludeInventory = 0 OR f.REFERENCE_ID IS NOT NULL)   -- con inventario: igual que el JOIN original
     ORDER BY a.PROVIDER_NAME, e.LINE_NAME, d.ITEM_NAME, c.REFERENCE_NAME, g.WAREHOUSE_NAME
    OPTION (RECOMPILE);
END
GO

/* ---------- Verificación ---------- */
SELECT OBJECT_NAME, BACKUP_DATE FROM dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP WHERE OBJECT_NAME = 'SP_GET_PROVIDER_REFERENCE_REPORT';
SELECT p.name AS Parametro FROM sys.parameters p WHERE p.object_id = OBJECT_ID('dbo.SP_GET_PROVIDER_REFERENCE_REPORT') ORDER BY p.parameter_id;
GO
