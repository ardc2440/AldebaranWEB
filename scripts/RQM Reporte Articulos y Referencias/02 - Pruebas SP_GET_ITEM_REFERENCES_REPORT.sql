/* =====================================================================================
   RQM  : Reporte de Artículos y Referencias - FASE 2 / Pruebas T1
   Objetivo: validar SP_GET_ITEM_REFERENCES_REPORT contra consultas directas (control).
   Uso    : ejecutar completo en SSMS. Cada caso imprime OK o FALLA con ambos conteos.
            Solo lectura: no modifica datos.
   Regresión: este script se vuelve a ejecutar en la prueba de cada tarea siguiente (T2..T9).
   ===================================================================================== */
USE Aldebaran
GO
SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#R') IS NOT NULL DROP TABLE #R;
CREATE TABLE #R (
    LineId SMALLINT, LineName VARCHAR(30), ItemId INT, ItemName VARCHAR(50), InternalReference VARCHAR(10),
    ProviderItemName VARCHAR(50), ProviderReference VARCHAR(27), IsItemActive BIT, IsCatalogVisible BIT,
    IsDomesticProduct BIT, IsSpecialImport BIT, IsSaleOff BIT, ReferenceId INT, ReferenceName VARCHAR(30),
    ReferenceCode VARCHAR(30), ProviderReferenceName VARCHAR(30), ProviderReferenceCode VARCHAR(10),
    IsReferenceActive BIT, IsSoldOut BIT, AlarmMinimumQuantity INT, MinimumLocalWarehouseQuantity INT,
    PurchaseOrderVariation INT);

DECLARE @Sp INT, @Ctl INT, @Caso VARCHAR(120);

-- Datos de muestra tomados de la BD (2 líneas, 2 artículos de la 1a línea, 2 referencias del 1er artículo)
DECLARE @L1 SMALLINT, @L2 SMALLINT, @I1 INT, @I2 INT, @R1 INT, @R2 INT;
SELECT TOP 1 @L1 = LINE_ID FROM lines ORDER BY LINE_ID;
SELECT TOP 1 @L2 = LINE_ID FROM lines WHERE LINE_ID > @L1 ORDER BY LINE_ID;
SELECT TOP 1 @I1 = ITEM_ID FROM items i WHERE LINE_ID = @L1 AND EXISTS (SELECT 1 FROM item_references r WHERE r.ITEM_ID = i.ITEM_ID) ORDER BY ITEM_ID;
SELECT TOP 1 @I2 = ITEM_ID FROM items WHERE LINE_ID = @L1 AND ITEM_ID > @I1 ORDER BY ITEM_ID;
SELECT TOP 1 @R1 = REFERENCE_ID FROM item_references WHERE ITEM_ID = @I1 ORDER BY REFERENCE_ID;
SELECT TOP 1 @R2 = REFERENCE_ID FROM item_references WHERE ITEM_ID = @I1 AND REFERENCE_ID > @R1 ORDER BY REFERENCE_ID;
DECLARE @sL VARCHAR(50) = CONCAT(@L1, ',', @L2), @sI VARCHAR(50) = CONCAT(@I1, ',', @I2), @sR VARCHAR(50) = CONCAT(@R1, ',', @R2);
PRINT CONCAT('Muestra -> Líneas: ', @sL, ' | Artículos: ', @sI, ' | Referencias: ', @sR);
PRINT '-------------------------------------------------------------------------------';

-- C01 Sin filtros = catálogo completo
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT;
SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C01 Sin filtros (catálogo completo)    SP=', @Sp, ' Control=', @Ctl);

-- C02 Por líneas
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @LineIds = @sL;
SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE i.LINE_ID IN (@L1, @L2);
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C02 Por líneas                         SP=', @Sp, ' Control=', @Ctl);

-- C03 Por artículos (con líneas informadas: prevalece artículos)
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @LineIds = @sL, @ItemIds = @sI;
SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references WHERE ITEM_ID IN (@I1, @I2);
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C03 Por artículos                      SP=', @Sp, ' Control=', @Ctl);

-- C04 Por referencias (con líneas y artículos informados: prevalece referencias)
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @LineIds = @sL, @ItemIds = @sI, @ReferenceIds = @sR;
SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references WHERE REFERENCE_ID IN (@R1, @R2);
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C04 Por referencias                    SP=', @Sp, ' Control=', @Ctl);

-- C05 Lista con basura (' ', 'abc', vacíos) no rompe y se ignora
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @ReferenceIds = @sR; SELECT @Ctl = COUNT(*) FROM #R;
DECLARE @sBad VARCHAR(80) = CONCAT(@R1, ',abc,, ,', @R2, ',');
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @ReferenceIds = @sBad; SELECT @Sp = COUNT(*) FROM #R;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C05 Lista con valores inválidos        SP=', @Sp, ' Control=', @Ctl);

-- C06..C14 Cada switch por separado
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyActiveItems = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE i.IS_ACTIVE = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C06 Solo artículos activos             SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyCatalogVisible = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE i.IS_CATALOG_VISIBLE = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C07 Solo visibles en página            SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyDomesticProduct = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE i.IS_DOMESTIC_PRODUCT = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C08 Solo producto nacional             SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlySpecialImport = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE i.IS_SPECIAL_IMPORT = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C09 Solo importación especial          SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlySaleOff = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE i.IS_SALE_OFF = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C10 Solo en promoción                  SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyActiveReferences = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE r.IS_ACTIVE = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C11 Solo referencias activas           SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlySoldOut = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE r.IS_SOLD_OUT = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C12 Solo referencias agotadas          SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyWithAlarmMinimumQuantity = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE r.ALARM_MINIMUM_QUANTITY > 0;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C13 Solo con cantidad mínima general   SP=', @Sp, ' Control=', @Ctl);

TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyWithMinimumLocalWarehouseQuantity = 1; SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID WHERE r.MINIMUM_LOCAL_WAREHOUSE_QUANTITY > 0;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C14 Solo con mínima en Bodega Local    SP=', @Sp, ' Control=', @Ctl);

-- C15 Combinación: líneas + activos (artículo y referencia) + visibles
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @LineIds = @sL, @OnlyActiveItems = 1, @OnlyActiveReferences = 1, @OnlyCatalogVisible = 1;
SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID
 WHERE i.LINE_ID IN (@L1, @L2) AND i.IS_ACTIVE = 1 AND r.IS_ACTIVE = 1 AND i.IS_CATALOG_VISIBLE = 1;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C15 Líneas + activos + visibles        SP=', @Sp, ' Control=', @Ctl);

-- C16 Switch en 0 o NULL = trae todo (igual a C01)
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT @OnlyActiveItems = 0, @OnlySoldOut = NULL;
SELECT @Sp = COUNT(*) FROM #R;
SELECT @Ctl = COUNT(*) FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID;
PRINT CONCAT(IIF(@Sp = @Ctl, 'OK    ', 'FALLA '), 'C16 Switches en 0 / NULL = todo       SP=', @Sp, ' Control=', @Ctl);

-- C17 Sin duplicados: una fila por referencia
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT;
SELECT @Sp = COUNT(*) - COUNT(DISTINCT ReferenceId) FROM #R;
PRINT CONCAT(IIF(@Sp = 0, 'OK    ', 'FALLA '), 'C17 Sin referencias duplicadas         Duplicados=', @Sp);

PRINT '-------------------------------------------------------------------------------';

-- Tiempo sin filtros (catálogo completo): revisar pestaña Mensajes
SET STATISTICS TIME ON;
EXEC dbo.SP_GET_ITEM_REFERENCES_REPORT;
SET STATISTICS TIME OFF;

DROP TABLE #R;
