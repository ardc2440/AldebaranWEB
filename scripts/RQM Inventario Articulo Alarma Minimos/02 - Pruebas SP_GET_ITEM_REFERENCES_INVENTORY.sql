/* =====================================================================================
   RQM  : Inventario del artículo en la alarma de cantidades mínimas - Pruebas T1
   Objetivo: validar SP_GET_ITEM_REFERENCES_INVENTORY contra un cálculo de control independiente
             (subconsultas por referencia) con la fórmula D2:
                 Stock Físico = Bodega Local + Zona Franca
                 Disponible   = Stock Físico + Tránsito - Comprometido
                 Comprometido = Reservas + Pedidos
   Uso    : ejecutar completo en SSMS. Cada verificación imprime OK / FALLA en la pestaña Mensajes
            y al final el RESULTADO global. Solo lectura: no modifica datos.

   Artículos de prueba (se buscan automáticamente en la BD, sin Ids fijos):
     A1 Varias referencias activas  -> el de más referencias activas (también mide el peor tiempo)
     A2 Con referencias inactivas   -> deben quedar fuera del resultado
     A3 Con tránsito                -> OC Pendientes / En aprobación sobre alguna referencia activa
     En los tres se prefiere un artículo con alguna referencia con cantidad mínima configurada
     (ALARM_MINIMUM_QUANTITY > 0) y esa referencia se usa como "referencia de la alarma".
     Los tres artículos son distintos entre sí.

   Pestaña Resultados: grilla con los tres casos (para compararlos en la aplicación) y la
   ejecución cronometrada de A1 (tiempos en Mensajes).

   Regresión: este script se vuelve a ejecutar en la prueba de cada tarea siguiente (T2..T6).
   ===================================================================================== */
USE Aldebaran
GO
SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#T')      IS NOT NULL DROP TABLE #T;
IF OBJECT_ID('tempdb..#Art')    IS NOT NULL DROP TABLE #Art;
IF OBJECT_ID('tempdb..#Casos')  IS NOT NULL DROP TABLE #Casos;
IF OBJECT_ID('tempdb..#R')      IS NOT NULL DROP TABLE #R;
IF OBJECT_ID('tempdb..#C')      IS NOT NULL DROP TABLE #C;
IF OBJECT_ID('tempdb..#Visual') IS NOT NULL DROP TABLE #Visual;

CREATE TABLE #R (
    ReferenceId INT, ReferenceCode VARCHAR(30), ReferenceName VARCHAR(30),
    LocalWarehouse INT, FreeZone INT, PhysicalStock INT, InTransit INT, [Committed] INT, Available INT,
    IsAlarmReference BIT);
SELECT TOP 0 * INTO #C FROM #R;                                  -- control: misma estructura que el SP
SELECT TOP 0 CAST('' AS VARCHAR(3)) AS Caso, CAST(N'' AS NVARCHAR(200)) AS Articulo, * INTO #Visual FROM #R;

DECLARE @Fallas INT = 0, @Sp INT, @Ctl INT, @Ok INT;

/* =====================================================================================
   0. BÚSQUEDA AUTOMÁTICA DE LOS ARTÍCULOS DE PRUEBA
   ===================================================================================== */
-- Tránsito por referencia (misma regla del SP, calculado una sola vez para toda la BD)
SELECT pod.REFERENCE_ID, SUM(pod.REQUESTED_QUANTITY) AS Transit
  INTO #T
  FROM dbo.purchase_order_details pod
  JOIN dbo.purchase_orders po        ON po.PURCHASE_ORDER_ID = pod.PURCHASE_ORDER_ID
  JOIN dbo.status_document_types sdt ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
  JOIN dbo.document_types dt         ON dt.DOCUMENT_TYPE_ID = sdt.DOCUMENT_TYPE_ID AND dt.DOCUMENT_TYPE_CODE = 'O'
 WHERE sdt.STATUS_ORDER IN (1, 4)
 GROUP BY pod.REFERENCE_ID;

-- Perfil de cada artículo
SELECT r.ITEM_ID,
       SUM(IIF(r.IS_ACTIVE = 1, 1, 0))                                  AS ActiveRefs,
       SUM(IIF(r.IS_ACTIVE = 0, 1, 0))                                  AS InactiveRefs,
       MAX(IIF(r.IS_ACTIVE = 1 AND r.ALARM_MINIMUM_QUANTITY > 0, 1, 0)) AS HasAlarm,
       MAX(IIF(r.IS_ACTIVE = 1 AND ISNULL(t.Transit, 0) > 0, 1, 0))     AS HasTransit
  INTO #Art
  FROM dbo.item_references r
  LEFT JOIN #T t ON t.REFERENCE_ID = r.REFERENCE_ID
 GROUP BY r.ITEM_ID;

DECLARE @A1 INT, @A2 INT, @A3 INT, @R1 INT, @R2 INT, @R3 INT, @RInactiva INT;

SELECT TOP 1 @A1 = ITEM_ID FROM #Art
 WHERE ActiveRefs >= 2
 ORDER BY HasAlarm DESC, ActiveRefs DESC, ITEM_ID;

SELECT TOP 1 @A2 = ITEM_ID FROM #Art
 WHERE ActiveRefs >= 1 AND InactiveRefs >= 1
   AND ITEM_ID <> ISNULL(@A1, -1)
 ORDER BY HasAlarm DESC, InactiveRefs DESC, ITEM_ID;

SELECT TOP 1 @A3 = ITEM_ID FROM #Art
 WHERE HasTransit = 1
   AND ITEM_ID NOT IN (ISNULL(@A1, -1), ISNULL(@A2, -1))
 ORDER BY HasAlarm DESC, ITEM_ID;

-- Referencia "de la alarma" de cada artículo: activa, preferiblemente con cantidad mínima configurada
SELECT TOP 1 @R1 = REFERENCE_ID FROM dbo.item_references
 WHERE ITEM_ID = @A1 AND IS_ACTIVE = 1
 ORDER BY IIF(ALARM_MINIMUM_QUANTITY > 0, 0, 1), REFERENCE_ID;

SELECT TOP 1 @R2 = REFERENCE_ID FROM dbo.item_references
 WHERE ITEM_ID = @A2 AND IS_ACTIVE = 1
 ORDER BY IIF(ALARM_MINIMUM_QUANTITY > 0, 0, 1), REFERENCE_ID;

SELECT TOP 1 @R3 = r.REFERENCE_ID FROM dbo.item_references r
  LEFT JOIN #T t ON t.REFERENCE_ID = r.REFERENCE_ID
 WHERE r.ITEM_ID = @A3 AND r.IS_ACTIVE = 1
 ORDER BY IIF(ISNULL(t.Transit, 0) > 0, 0, 1), IIF(r.ALARM_MINIMUM_QUANTITY > 0, 0, 1), r.REFERENCE_ID;

SELECT TOP 1 @RInactiva = REFERENCE_ID FROM dbo.item_references
 WHERE ITEM_ID = @A2 AND IS_ACTIVE = 0
 ORDER BY REFERENCE_ID;

CREATE TABLE #Casos (Orden TINYINT PRIMARY KEY, Caso VARCHAR(3), Descripcion VARCHAR(40), ItemId INT, ReferenceId INT);
INSERT INTO #Casos VALUES
    (1, 'A1', 'Varias referencias activas', @A1, @R1),
    (2, 'A2', 'Con referencias inactivas',  @A2, @R2),
    (3, 'A3', 'Con transito',               @A3, @R3);

PRINT '===============================================================================';
PRINT ' PRUEBAS T1 - SP_GET_ITEM_REFERENCES_INVENTORY';
PRINT '===============================================================================';

/* =====================================================================================
   1. CASOS POR ARTÍCULO (A1, A2, A3): SP vs control
   ===================================================================================== */
DECLARE @i TINYINT = 1, @Caso VARCHAR(3), @Desc VARCHAR(40), @Item INT, @Ref INT, @Etiqueta NVARCHAR(200);

WHILE @i <= 3
BEGIN
    SELECT @Caso = Caso, @Desc = Descripcion, @Item = ItemId, @Ref = ReferenceId
      FROM #Casos WHERE Orden = @i;
    SET @i += 1;

    PRINT '';
    IF @Ref IS NULL
    BEGIN
        PRINT CONCAT('SIN DATOS ', @Caso, ' ', @Desc, ': no hay en la BD un artículo con estas características');
        CONTINUE;
    END

    SELECT @Etiqueta = CONCAT(i.INTERNAL_REFERENCE, N' - ', i.ITEM_NAME,
                              N' | Ref. alarma: ', r.REFERENCE_CODE, N' ', r.REFERENCE_NAME, N' (Id ', r.REFERENCE_ID, N')')
      FROM dbo.item_references r
      JOIN dbo.items i ON i.ITEM_ID = r.ITEM_ID
     WHERE r.REFERENCE_ID = @Ref;
    PRINT CONCAT(@Caso, ' ', @Desc, ' -> ', @Etiqueta);

    -- Resultado del SP
    TRUNCATE TABLE #R;
    INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId = @Ref;

    -- Control independiente: subconsultas por referencia
    TRUNCATE TABLE #C;
    INSERT INTO #C (ReferenceId, ReferenceCode, ReferenceName, LocalWarehouse, FreeZone, PhysicalStock, InTransit, [Committed], Available, IsAlarmReference)
    SELECT r.REFERENCE_ID, r.REFERENCE_CODE, r.REFERENCE_NAME,
           x.Loc, x.Fz, x.Loc + x.Fz, x.Tr,
           r.RESERVED_QUANTITY + r.ORDERED_QUANTITY,
           x.Loc + x.Fz + x.Tr - (r.RESERVED_QUANTITY + r.ORDERED_QUANTITY),
           IIF(r.REFERENCE_ID = @Ref, 1, 0)
      FROM dbo.item_references r
     CROSS APPLY (SELECT
            Loc = (SELECT ISNULL(SUM(rw.QUANTITY), 0)
                     FROM dbo.references_warehouse rw
                     JOIN dbo.warehouses w ON w.WAREHOUSE_ID = rw.WAREHOUSE_ID
                    WHERE rw.REFERENCE_ID = r.REFERENCE_ID AND w.WAREHOUSE_CODE = 1),
            Fz  = (SELECT ISNULL(SUM(rw.QUANTITY), 0)
                     FROM dbo.references_warehouse rw
                     JOIN dbo.warehouses w ON w.WAREHOUSE_ID = rw.WAREHOUSE_ID
                    WHERE rw.REFERENCE_ID = r.REFERENCE_ID AND w.WAREHOUSE_CODE = 2),
            Tr  = (SELECT ISNULL(SUM(pod.REQUESTED_QUANTITY), 0)
                     FROM dbo.purchase_order_details pod
                     JOIN dbo.purchase_orders po        ON po.PURCHASE_ORDER_ID = pod.PURCHASE_ORDER_ID
                     JOIN dbo.status_document_types sdt ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
                     JOIN dbo.document_types dt         ON dt.DOCUMENT_TYPE_ID = sdt.DOCUMENT_TYPE_ID
                    WHERE pod.REFERENCE_ID = r.REFERENCE_ID
                      AND dt.DOCUMENT_TYPE_CODE = 'O'
                      AND sdt.STATUS_ORDER IN (1, 4))
     ) x
     WHERE r.ITEM_ID = @Item
       AND r.IS_ACTIVE = 1;

    -- .1 Una fila por referencia activa del artículo
    SELECT @Sp = COUNT(*) FROM #R;
    SELECT @Ctl = COUNT(*) FROM dbo.item_references WHERE ITEM_ID = @Item AND IS_ACTIVE = 1;
    SET @Ok = IIF(@Sp = @Ctl AND @Sp > 0, 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.1 Filas = referencias activas del artículo        SP=', @Sp, ' Control=', @Ctl);

    -- .2 Cifras idénticas al control, fila por fila (EXCEPT en ambos sentidos)
    SELECT @Sp = (SELECT COUNT(*) FROM (SELECT * FROM #R EXCEPT SELECT * FROM #C) d)
               + (SELECT COUNT(*) FROM (SELECT * FROM #C EXCEPT SELECT * FROM #R) d);
    SET @Ok = IIF(@Sp = 0, 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.2 Cifras = control (Local, ZF, Físico, Tráns., Comp., Disp.) Dif.=', @Sp);

    -- .3 Sin referencias inactivas ni de otro artículo (en A2 además deben existir inactivas en BD)
    SELECT @Sp = COUNT(*) FROM #R x JOIN dbo.item_references r ON r.REFERENCE_ID = x.ReferenceId
     WHERE r.IS_ACTIVE = 0 OR r.ITEM_ID <> @Item;
    SELECT @Ctl = COUNT(*) FROM dbo.item_references WHERE ITEM_ID = @Item AND IS_ACTIVE = 0;
    SET @Ok = IIF(@Sp = 0 AND (@Caso <> 'A2' OR @Ctl > 0), 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.3 Sin inactivas ni ajenas                      En SP=', @Sp, ' (inactivas del artículo en BD=', @Ctl, ')');

    -- .4 Solo la referencia de la alarma está marcada
    SELECT @Sp  = COUNT(*) FROM #R WHERE IsAlarmReference = 1;
    SELECT @Ctl = COUNT(*) FROM #R WHERE IsAlarmReference = 1 AND ReferenceId = @Ref;
    SET @Ok = IIF(@Sp = 1 AND @Ctl = 1, 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.4 Solo la referencia de la alarma marcada       Marcadas=', @Sp);

    -- .5 Fórmula D2 en cada fila
    SELECT @Sp = COUNT(*) FROM #R WHERE Available <> PhysicalStock + InTransit - [Committed];
    SET @Ok = IIF(@Sp = 0, 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.5 Disponible = Físico + Tránsito - Comp.       Filas con error=', @Sp);

    -- .6 Stock Físico = Bodega Local + Zona Franca en cada fila
    SELECT @Sp = COUNT(*) FROM #R WHERE PhysicalStock <> LocalWarehouse + FreeZone;
    SET @Ok = IIF(@Sp = 0, 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.6 Stock Físico = Local + ZF                    Filas con error=', @Sp);

    -- .7 (solo A3) el artículo efectivamente tiene tránsito
    IF @Caso = 'A3'
    BEGIN
        SELECT @Sp = ISNULL(SUM(InTransit), 0) FROM #R;
        SET @Ok = IIF(@Sp > 0, 1, 0); SET @Fallas += 1 - @Ok;
        PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), @Caso, '.7 El artículo tiene tránsito                    Tránsito total=', @Sp);
    END

    INSERT INTO #Visual SELECT @Caso, @Etiqueta, * FROM #R;
END

/* =====================================================================================
   2. CASOS GENERALES
   ===================================================================================== */
PRINT '';
PRINT 'Casos generales';

-- C01 Referencia inexistente: conjunto vacío, sin error
SELECT @Ctl = ISNULL(MAX(REFERENCE_ID), 0) + 1000 FROM dbo.item_references;
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId = @Ctl;
SELECT @Sp = COUNT(*) FROM #R;
SET @Ok = IIF(@Sp = 0, 1, 0); SET @Fallas += 1 - @Ok;
PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), 'C01 Referencia inexistente -> vacío             Filas=', @Sp);

-- C02 Referencia NULL: conjunto vacío, sin error
TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId = NULL;
SELECT @Sp = COUNT(*) FROM #R;
SET @Ok = IIF(@Sp = 0, 1, 0); SET @Fallas += 1 - @Ok;
PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), 'C02 Referencia NULL -> vacío                    Filas=', @Sp);

-- C03 Se envía una referencia INACTIVA (de A2): devuelve las activas del artículo y ninguna marcada
IF @RInactiva IS NULL
    PRINT 'SIN DATOS C03 Referencia inactiva: no se encontró una en la BD'
ELSE
BEGIN
    TRUNCATE TABLE #R; INSERT INTO #R EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId = @RInactiva;
    SELECT @Sp  = COUNT(*) FROM #R;
    SELECT @Ctl = COUNT(*) FROM dbo.item_references WHERE ITEM_ID = @A2 AND IS_ACTIVE = 1;
    SET @Ok = IIF(@Sp = @Ctl AND NOT EXISTS (SELECT 1 FROM #R WHERE IsAlarmReference = 1), 1, 0); SET @Fallas += 1 - @Ok;
    PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), 'C03 Ref. inactiva -> activas, ninguna marcada   SP=', @Sp, ' Control=', @Ctl);
END

-- C04 Estructura del resultado (nombres y orden de columnas que mapeará la entidad de T2)
DECLARE @Cols NVARCHAR(4000), @Tipos NVARCHAR(4000),
        @ColsEsperadas NVARCHAR(4000) = N'ReferenceId,ReferenceCode,ReferenceName,LocalWarehouse,FreeZone,PhysicalStock,InTransit,Committed,Available,IsAlarmReference';
SELECT @Cols  = STRING_AGG(CAST(name AS NVARCHAR(4000)), N',') WITHIN GROUP (ORDER BY column_ordinal),
       @Tipos = STRING_AGG(CAST(CONCAT(name, N' ', system_type_name, IIF(is_nullable = 1, N' null', N'')) AS NVARCHAR(4000)), N', ') WITHIN GROUP (ORDER BY column_ordinal)
  FROM sys.dm_exec_describe_first_result_set(N'EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId = 0', NULL, 0);
SET @Ok = IIF(@Cols = @ColsEsperadas, 1, 0); SET @Fallas += 1 - @Ok;
PRINT CONCAT(IIF(@Ok = 1, 'OK    ', 'FALLA '), 'C04 Estructura: ', @Cols);
PRINT CONCAT('INFO  C04 Tipos: ', @Tipos);

/* =====================================================================================
   3. INFORMATIVOS (no suman fallas; sirven para validar D2 con el cliente)
   ===================================================================================== */
PRINT '';
PRINT 'Informativos';

SELECT @Sp = COUNT(*) FROM dbo.item_references WHERE IS_ACTIVE = 0 AND ALARM_MINIMUM_QUANTITY > 0;
PRINT CONCAT('INFO  I01 Referencias INACTIVAS con cantidad mínima configurada: ', @Sp,
             ' (si alguna genera alarma, su fila no aparece en el diálogo)');

SELECT @Sp = COUNT(*)
  FROM dbo.item_references r
 WHERE r.IS_ACTIVE = 1
   AND r.INVENTORY_QUANTITY <> (SELECT ISNULL(SUM(rw.QUANTITY), 0)
                                  FROM dbo.references_warehouse rw
                                  JOIN dbo.warehouses w ON w.WAREHOUSE_ID = rw.WAREHOUSE_ID
                                 WHERE rw.REFERENCE_ID = r.REFERENCE_ID AND w.WAREHOUSE_CODE IN (1, 2));
PRINT CONCAT('INFO  I02 Referencias activas con Local + ZF <> INVENTORY_QUANTITY: ', @Sp,
             ' (el CSV de Fase 1 usa INVENTORY_QUANTITY como stock físico; D2 usa Local + ZF)');

SELECT @Etiqueta = STRING_AGG(CAST(i.name AS NVARCHAR(200)), N', ')
  FROM sys.indexes i
  JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.key_ordinal = 1
  JOIN sys.columns c        ON c.object_id = ic.object_id AND c.column_id = ic.column_id
 WHERE i.object_id = OBJECT_ID('dbo.purchase_order_details') AND c.name = 'REFERENCE_ID';
PRINT CONCAT('INFO  I03 Índice por REFERENCE_ID en purchase_order_details: ', ISNULL(@Etiqueta, N'NO EXISTE'));

/* =====================================================================================
   4. RESULTADO GLOBAL, GRILLA DE COMPARACIÓN Y TIEMPO
   ===================================================================================== */
PRINT '';
PRINT '===============================================================================';
PRINT CONCAT(' RESULTADO: ', IIF(@Fallas = 0, 'TODAS LAS PRUEBAS OK', CONCAT(@Fallas, ' FALLA(S)')));
PRINT '===============================================================================';

-- Grilla para comparar en la aplicación (Tablero / Reporte de referencias)
SELECT Caso, Articulo, ReferenceId, ReferenceCode, ReferenceName,
       LocalWarehouse, FreeZone, PhysicalStock, InTransit, [Committed], Available, IsAlarmReference
  FROM #Visual
 ORDER BY Caso, ReferenceName;

-- Tiempo del caso más pesado (A1 = artículo con más referencias activas): revisar pestaña Mensajes
IF @R1 IS NOT NULL
BEGIN
    SET STATISTICS IO ON;
    SET STATISTICS TIME ON;
    EXEC dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId = @R1;
    SET STATISTICS TIME OFF;
    SET STATISTICS IO OFF;
END

DROP TABLE #T, #Art, #Casos, #R, #C, #Visual;
