/* =====================================================================================
   RQM  : Reporte de Artículos y Referencias - FASE 2 / Tarea T1
   Autor: Andrés Ricardo Díaz Cárdenas
   Fecha: 2026-09-25

   Objeto:
     Procedure : SP_GET_ITEM_REFERENCES_REPORT
                 Devuelve el árbol Línea -> Artículo -> Referencias (sin cantidades de inventario)
                 para el reporte "Artículos y Referencias" de Aldebaran.Web.

   Parámetros:
     Jerarquía (listas de Ids separadas por coma; NULL o '' = sin filtro).
     Prevalece el nivel más específico que venga informado:
       @ReferenceIds  >  @ItemIds  >  @LineIds
     Switches (BIT; 1 = aplica el filtro, 0 o NULL = trae todo):
       Artículo   : @OnlyActiveItems, @OnlyCatalogVisible, @OnlyDomesticProduct,
                    @OnlySpecialImport, @OnlySaleOff
       Referencia : @OnlyActiveReferences, @OnlySoldOut,
                    @OnlyWithAlarmMinimumQuantity (ALARM_MINIMUM_QUANTITY > 0),
                    @OnlyWithMinimumLocalWarehouseQuantity (MINIMUM_LOCAL_WAREHOUSE_QUANTITY > 0)

   Notas de diseño:
     - Las listas se materializan en tablas variables con PK (búsqueda por índice).
     - TRY_CAST descarta valores no numéricos de las listas (entrada defensiva).
     - OPTION (RECOMPILE): los filtros opcionales generan un plan acorde a cada combinación.
     - Una fila por referencia; los datos del artículo y la línea se repiten
       (la aplicación arma el árbol y el Excel plano usa la misma forma).

   Script idempotente: puede ejecutarse varias veces.
   ===================================================================================== */
USE Aldebaran
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_ITEM_REFERENCES_REPORT
    @LineIds                                VARCHAR(MAX) = NULL,
    @ItemIds                                VARCHAR(MAX) = NULL,
    @ReferenceIds                           VARCHAR(MAX) = NULL,
    @OnlyActiveItems                        BIT = 0,
    @OnlyCatalogVisible                     BIT = 0,
    @OnlyDomesticProduct                    BIT = 0,
    @OnlySpecialImport                      BIT = 0,
    @OnlySaleOff                            BIT = 0,
    @OnlyActiveReferences                   BIT = 0,
    @OnlySoldOut                            BIT = 0,
    @OnlyWithAlarmMinimumQuantity           BIT = 0,
    @OnlyWithMinimumLocalWarehouseQuantity  BIT = 0
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
       -- Switches de Artículo
       AND (ISNULL(@OnlyActiveItems, 0)     = 0 OR i.IS_ACTIVE           = 1)
       AND (ISNULL(@OnlyCatalogVisible, 0)  = 0 OR i.IS_CATALOG_VISIBLE  = 1)
       AND (ISNULL(@OnlyDomesticProduct, 0) = 0 OR i.IS_DOMESTIC_PRODUCT = 1)
       AND (ISNULL(@OnlySpecialImport, 0)   = 0 OR i.IS_SPECIAL_IMPORT   = 1)
       AND (ISNULL(@OnlySaleOff, 0)         = 0 OR i.IS_SALE_OFF         = 1)
       -- Switches de Referencia
       AND (ISNULL(@OnlyActiveReferences, 0)                  = 0 OR r.IS_ACTIVE   = 1)
       AND (ISNULL(@OnlySoldOut, 0)                           = 0 OR r.IS_SOLD_OUT = 1)
       AND (ISNULL(@OnlyWithAlarmMinimumQuantity, 0)          = 0 OR r.ALARM_MINIMUM_QUANTITY           > 0)
       AND (ISNULL(@OnlyWithMinimumLocalWarehouseQuantity, 0) = 0 OR r.MINIMUM_LOCAL_WAREHOUSE_QUANTITY > 0)
     ORDER BY l.LINE_NAME, i.ITEM_NAME, r.REFERENCE_NAME
    OPTION (RECOMPILE);
END
GO
