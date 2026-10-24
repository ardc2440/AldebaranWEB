/* =====================================================================================
   RQM Complemento Consulta de Logs - T3: Pruebas de la consulta de visualización de alarmas
   Ejecutar después de "05 - Consulta visualizacion de alarmas.sql".
   Cada bloque devuelve un resultado; la columna OK debe ser 1 en todas las filas.
   ===================================================================================== */
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @r TABLE (PRUEBA VARCHAR(80), DETALLE NVARCHAR(400), OK BIT);
DECLARE @t TABLE (ALARM_KEY VARCHAR(50), ALARM_DATE DATETIME, DOCUMENT NVARCHAR(300), DESCRIPTION NVARCHAR(500), VISUALIZATION_COUNT INT, TOTAL_ROWS INT);
DECLARE @v TABLE (EMPLOYEE_NAME NVARCHAR(200), AREA_NAME NVARCHAR(200), VISUALIZED_DATE DATETIME);
DECLARE @ty TABLE (CODE VARCHAR(30), NAME NVARCHAR(100));
DECLARE @code VARCHAR(30), @spTotal INT, @spVis INT, @direct INT, @directVis INT, @viewAll INT, @viewVisualized INT;

/* P1: por cada tipo:
       - la vista de alarmas activas = consulta directa a la fuente;
       - el SP solo lista las alarmas activas que al menos un empleado visualizó;
       - visualizaciones sumadas del SP = visualizaciones de esas alarmas en su tabla visualized_* */
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT CODE FROM dbo.ALARM_VISUALIZATION_LOG_TYPES WHERE IS_ACTIVE = 1;
OPEN c; FETCH NEXT FROM c INTO @code;
WHILE @@FETCH_STATUS = 0
BEGIN
    DELETE @t;
    INSERT @t EXEC dbo.SP_GET_ALARM_LOG @TypeCode = @code, @DateFrom = '2000-01-01', @DateTo = '2100-12-31', @Skip = 0, @Take = 1000000;
    SELECT @spTotal = ISNULL(MAX(TOTAL_ROWS), 0), @spVis = ISNULL(SUM(VISUALIZATION_COUNT), 0) FROM @t;

    SELECT @direct = CASE @code
        WHEN 'PURCHASE_ORDER_ALARM'       THEN (SELECT COUNT(*) FROM alarms a JOIN alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID WHERE m.ALARM_TYPE_ID = 1 AND a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE())
        WHEN 'CUSTOMER_ORDER_ALARM'       THEN (SELECT COUNT(*) FROM alarms a JOIN alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID WHERE m.ALARM_TYPE_ID = 2 AND a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE())
        WHEN 'CUSTOMER_RESERVATION_ALARM' THEN (SELECT COUNT(*) FROM alarms a JOIN alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID WHERE m.ALARM_TYPE_ID = 3 AND a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE())
        WHEN 'PURCHASE_ORDER_TRANSIT'     THEN (SELECT COUNT(*) FROM purchase_order_transit_alarms t JOIN modified_purchase_orders mp ON mp.MODIFIED_PURCHASE_ORDER_ID = t.MODIFIED_PURCHASE_ORDER_ID JOIN purchase_orders p ON p.PURCHASE_ORDER_ID = mp.PURCHASE_ORDER_ID JOIN status_document_types s ON s.STATUS_DOCUMENT_TYPE_ID = p.STATUS_DOCUMENT_TYPE_ID WHERE s.STATUS_ORDER IN (1, 4))
        WHEN 'AUTOMATIC_IN_PROCESS'       THEN (SELECT COUNT(*) FROM automatic_in_process)
        WHEN 'AUTOMATIC_IN_PROCESS_CHANGE' THEN (SELECT COUNT(*) FROM canceled_orders_in_process c2 WHERE EXISTS (SELECT 1 FROM automatic_in_process_orders x WHERE x.CUSTOMER_ORDER_IN_PROCESS_ID = c2.CUSTOMER_ORDER_IN_PROCESS_ID))
                                             + (SELECT COUNT(*) FROM modified_orders_in_process m WHERE EXISTS (SELECT 1 FROM automatic_in_process_orders x WHERE x.CUSTOMER_ORDER_IN_PROCESS_ID = m.CUSTOMER_ORDER_IN_PROCESS_ID) AND NOT EXISTS (SELECT 1 FROM canceled_orders_in_process c3 WHERE c3.CUSTOMER_ORDER_IN_PROCESS_ID = m.CUSTOMER_ORDER_IN_PROCESS_ID))
        WHEN 'MINIMUM_QUANTITY'           THEN (SELECT COUNT(*) FROM minimum_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1 WHERE f.ACTIVE = 1)
        WHEN 'MINIMUM_LOCAL_WAREHOUSE'    THEN (SELECT COUNT(*) FROM minimum_local_warehouse_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1 WHERE f.ACTIVE = 1)
        WHEN 'OUT_OF_STOCK'               THEN (SELECT COUNT(*) FROM out_of_stock_inventory_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1 WHERE f.ACTIVE = 1)
        WHEN 'LOCAL_WAREHOUSE_INCOME'     THEN (SELECT COUNT(*) FROM local_warehouse_alarms)
    END;

    SELECT @directVis = CASE @code
        WHEN 'PURCHASE_ORDER_ALARM'       THEN (SELECT COUNT(*) FROM visualized_alarms v JOIN alarms a ON a.ALARM_ID = v.ALARM_ID JOIN alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID WHERE m.ALARM_TYPE_ID = 1 AND a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE())
        WHEN 'CUSTOMER_ORDER_ALARM'       THEN (SELECT COUNT(*) FROM visualized_alarms v JOIN alarms a ON a.ALARM_ID = v.ALARM_ID JOIN alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID WHERE m.ALARM_TYPE_ID = 2 AND a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE())
        WHEN 'CUSTOMER_RESERVATION_ALARM' THEN (SELECT COUNT(*) FROM visualized_alarms v JOIN alarms a ON a.ALARM_ID = v.ALARM_ID JOIN alarm_messages m ON m.ALARM_MESSAGE_ID = a.ALARM_MESSAGE_ID WHERE m.ALARM_TYPE_ID = 3 AND a.IS_ACTIVE = 1 AND a.EXECUTION_DATE <= GETDATE())
        WHEN 'PURCHASE_ORDER_TRANSIT'     THEN (SELECT COUNT(*) FROM visualized_purchase_order_transit_alarms v JOIN purchase_order_transit_alarms t ON t.PURCHASE_ORDER_TRANSIT_ALARM_ID = v.PURCHASE_ORDER_TRANSIT_ALARM_ID JOIN modified_purchase_orders mp ON mp.MODIFIED_PURCHASE_ORDER_ID = t.MODIFIED_PURCHASE_ORDER_ID JOIN purchase_orders p ON p.PURCHASE_ORDER_ID = mp.PURCHASE_ORDER_ID JOIN status_document_types s ON s.STATUS_DOCUMENT_TYPE_ID = p.STATUS_DOCUMENT_TYPE_ID WHERE s.STATUS_ORDER IN (1, 4))
        WHEN 'AUTOMATIC_IN_PROCESS'       THEN (SELECT COUNT(*) FROM visualized_automatic_in_process v WHERE EXISTS (SELECT 1 FROM automatic_in_process a WHERE a.AUTOMATIC_IN_PROCESS_ID = v.AUTOMATIC_IN_PROCESS_ID))
        WHEN 'MINIMUM_QUANTITY'           THEN (SELECT COUNT(*) FROM visualized_minimum_quantity_alarms v JOIN minimum_quantity_alarms f ON f.MINIMUM_QUANTITY_ALARM_ID = v.MINIMUM_QUANTITY_ALARM_ID JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1 WHERE f.ACTIVE = 1)
        WHEN 'MINIMUM_LOCAL_WAREHOUSE'    THEN (SELECT COUNT(*) FROM visualized_minimum_local_warehouse_quantity_alarms v JOIN minimum_local_warehouse_quantity_alarms f ON f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = v.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1 WHERE f.ACTIVE = 1)
        WHEN 'OUT_OF_STOCK'               THEN (SELECT COUNT(*) FROM visualized_out_of_stock_inventory_alarms v JOIN out_of_stock_inventory_alarms f ON f.OUT_OF_STOCK_INVENTORY_ALARM_ID = v.OUT_OF_STOCK_INVENTORY_ALARM_ID JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1 WHERE f.ACTIVE = 1)
        WHEN 'LOCAL_WAREHOUSE_INCOME'     THEN (SELECT COUNT(*) FROM visualized_local_warehouse_alarms)
        ELSE @spVis   -- AUTOMATIC_IN_PROCESS_CHANGE: llave compuesta, se valida en P8
    END;

    SELECT @viewAll = COUNT(*) FROM dbo.VW_ALARM_LOG a WHERE a.TYPE_CODE = @code;
    SELECT @viewVisualized = COUNT(*) FROM dbo.VW_ALARM_LOG a
     WHERE a.TYPE_CODE = @code AND EXISTS (SELECT 1 FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v WHERE v.TYPE_CODE = a.TYPE_CODE AND v.ALARM_KEY = a.ALARM_KEY);
    INSERT @r VALUES ('P1 ' + @code,
                      CONCAT(N'Activas vista=', @viewAll, N' directo=', @direct, N' · Visualizadas SP=', @spTotal, N' esperado=', ISNULL(@viewVisualized, 0),
                             N' · Visualizaciones SP=', @spVis, N' directo=', @directVis),
                      CASE WHEN @viewAll = @direct AND @spTotal = ISNULL(@viewVisualized, 0) AND @spVis = @directVis THEN 1 ELSE 0 END);
    FETCH NEXT FROM c INTO @code;
END
CLOSE c; DEALLOCATE c;

/* P2: paginación — la página 2 no repite filas de la página 1 */
DECLARE @p1 TABLE (ALARM_KEY VARCHAR(50));
DELETE @t; INSERT @t EXEC dbo.SP_GET_ALARM_LOG 'MINIMUM_LOCAL_WAREHOUSE', '2000-01-01', '2100-12-31', NULL, 0, 20;
INSERT @p1 SELECT ALARM_KEY FROM @t;
DELETE @t; INSERT @t EXEC dbo.SP_GET_ALARM_LOG 'MINIMUM_LOCAL_WAREHOUSE', '2000-01-01', '2100-12-31', NULL, 20, 20;
INSERT @r SELECT 'P2 Paginación', CONCAT(COUNT(*), N' filas en página 2; repetidas: ', COUNT(p.ALARM_KEY)),
                 CASE WHEN COUNT(*) > 0 AND COUNT(p.ALARM_KEY) = 0 THEN 1 ELSE 0 END
            FROM @t t LEFT JOIN @p1 p ON p.ALARM_KEY = t.ALARM_KEY;

/* P3: búsqueda por texto del documento */
DECLARE @term NVARCHAR(100) = (SELECT TOP 1 i.INTERNAL_REFERENCE FROM minimum_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID AND r.IS_ACTIVE = 1 JOIN items i ON i.ITEM_ID = r.ITEM_ID AND i.IS_ACTIVE = 1
                                 WHERE f.ACTIVE = 1 AND EXISTS (SELECT 1 FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID));
DELETE @t; INSERT @t EXEC dbo.SP_GET_ALARM_LOG 'MINIMUM_QUANTITY', '2000-01-01', '2100-12-31', @term, 0, 1000;
INSERT @r SELECT 'P3 Búsqueda "' + @term + '"', CONCAT(COUNT(*), N' filas; todas contienen el término: ', MIN(CASE WHEN DOCUMENT LIKE N'%' + @term + N'%' THEN 1 ELSE 0 END)),
                 CASE WHEN COUNT(*) > 0 AND MIN(CASE WHEN DOCUMENT LIKE N'%' + @term + N'%' THEN 1 ELSE 0 END) = 1 THEN 1 ELSE 0 END FROM @t;

/* P4-P6: validaciones controladas */
BEGIN TRY EXEC dbo.SP_GET_ALARM_LOG 'NO_EXISTE', '2026-01-01', '2026-12-31'; INSERT @r VALUES ('P4 Tipo no registrado', N'No falló', 0); END TRY
BEGIN CATCH INSERT @r VALUES ('P4 Tipo no registrado', ERROR_MESSAGE(), CASE WHEN ERROR_NUMBER() = 50001 THEN 1 ELSE 0 END); END CATCH;
BEGIN TRY EXEC dbo.SP_GET_ALARM_LOG 'MINIMUM_QUANTITY', NULL, '2026-12-31'; INSERT @r VALUES ('P5 Sin fecha inicial', N'No falló', 0); END TRY
BEGIN CATCH INSERT @r VALUES ('P5 Sin fecha inicial', ERROR_MESSAGE(), CASE WHEN ERROR_NUMBER() = 50002 THEN 1 ELSE 0 END); END CATCH;
BEGIN TRY EXEC dbo.SP_GET_ALARM_LOG 'MINIMUM_QUANTITY', '2026-12-31', '2026-01-01'; INSERT @r VALUES ('P6 Rango invertido', N'No falló', 0); END TRY
BEGIN CATCH INSERT @r VALUES ('P6 Rango invertido', ERROR_MESSAGE(), CASE WHEN ERROR_NUMBER() = 50002 THEN 1 ELSE 0 END); END CATCH;

/* P7: detalle de visualizaciones de una alarma = su tabla visualized_* */
DECLARE @k VARCHAR(50) = (SELECT TOP 1 CAST(v.MINIMUM_QUANTITY_ALARM_ID AS VARCHAR(50)) FROM visualized_minimum_quantity_alarms v JOIN minimum_quantity_alarms f ON f.MINIMUM_QUANTITY_ALARM_ID = v.MINIMUM_QUANTITY_ALARM_ID WHERE f.ACTIVE = 1 GROUP BY v.MINIMUM_QUANTITY_ALARM_ID ORDER BY COUNT(*) DESC);
DELETE @v; INSERT @v EXEC dbo.SP_GET_ALARM_LOG_VISUALIZATIONS 'MINIMUM_QUANTITY', @k;
SELECT @directVis = COUNT(*) FROM visualized_minimum_quantity_alarms WHERE MINIMUM_QUANTITY_ALARM_ID = @k;
INSERT @r SELECT 'P7 Visualizaciones alarma ' + @k, CONCAT(COUNT(*), N' (directo ', @directVis, N')'), CASE WHEN COUNT(*) = @directVis THEN 1 ELSE 0 END FROM @v;

/* P8: llave compuesta de modificaciones automáticas (ID|C / ID|M) */
DECLARE @k2 VARCHAR(50) = (SELECT TOP 1 CONCAT(Id, '|', ActionType) FROM visualized_automatic_customer_in_process_modifications);
DELETE @v; INSERT @v EXEC dbo.SP_GET_ALARM_LOG_VISUALIZATIONS 'AUTOMATIC_IN_PROCESS_CHANGE', @k2;
SELECT @directVis = COUNT(*) FROM visualized_automatic_customer_in_process_modifications WHERE CONCAT(Id, '|', ActionType) = @k2;
INSERT @r SELECT 'P8 Llave compuesta ' + ISNULL(@k2, '-'), CONCAT(COUNT(*), N' (directo ', @directVis, N')'), CASE WHEN COUNT(*) = @directVis THEN 1 ELSE 0 END FROM @v;

/* P9-P11: tipos visibles por rol */
DELETE @ty; INSERT @ty EXEC dbo.SP_GET_ALARM_LOG_TYPES 'I', N'Log de pedidos';
INSERT @r SELECT 'P9 Contexto I con rol Log de pedidos', CONCAT(COUNT(*), N' tipos (esperado 0)'), CASE WHEN COUNT(*) = 0 THEN 1 ELSE 0 END FROM @ty;
DELETE @ty; INSERT @ty EXEC dbo.SP_GET_ALARM_LOG_TYPES 'I', N'Log de inventarios';
INSERT @r SELECT 'P10 Contexto I con rol Log de inventarios', CONCAT(COUNT(*), N' tipos (esperado 4)'), CASE WHEN COUNT(*) = 4 THEN 1 ELSE 0 END FROM @ty;
DELETE @ty; INSERT @ty EXEC dbo.SP_GET_ALARM_LOG_TYPES 'P', N'Consulta de reportes,Administrador';
INSERT @r SELECT 'P11 Contexto P con Administrador', CONCAT(COUNT(*), N' tipos (esperado 3)'), CASE WHEN COUNT(*) = 3 THEN 1 ELSE 0 END FROM @ty;

/* P12: roles nuevos creados */
INSERT @r SELECT 'P12 Roles nuevos', CONCAT(COUNT(*), N' de 2'), CASE WHEN COUNT(*) = 2 THEN 1 ELSE 0 END
  FROM AspNetRoles WHERE Name IN (N'Log del sistema', N'Log de inventarios');

/* P13: filtro por empleado — solo alarmas que ese empleado visualizó, y todas las que visualizó */
DECLARE @emp INT = (SELECT TOP 1 v.EMPLOYEE_ID FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v JOIN dbo.VW_ALARM_LOG a ON a.TYPE_CODE = v.TYPE_CODE AND a.ALARM_KEY = v.ALARM_KEY
                     WHERE v.TYPE_CODE = 'MINIMUM_QUANTITY' GROUP BY v.EMPLOYEE_ID ORDER BY COUNT(*) DESC);
DELETE @t; INSERT @t EXEC dbo.SP_GET_ALARM_LOG @TypeCode = 'MINIMUM_QUANTITY', @DateFrom = '2000-01-01', @DateTo = '2100-12-31', @Skip = 0, @Take = 1000000, @EmployeeId = @emp;
SELECT @direct = COUNT(DISTINCT v.ALARM_KEY) FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v JOIN dbo.VW_ALARM_LOG a ON a.TYPE_CODE = v.TYPE_CODE AND a.ALARM_KEY = v.ALARM_KEY
 WHERE v.TYPE_CODE = 'MINIMUM_QUANTITY' AND v.EMPLOYEE_ID = @emp;
DECLARE @others INT = (SELECT COUNT(*) FROM @t t WHERE NOT EXISTS (SELECT 1 FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v WHERE v.TYPE_CODE = 'MINIMUM_QUANTITY' AND v.ALARM_KEY = t.ALARM_KEY AND v.EMPLOYEE_ID = @emp));
INSERT @r SELECT 'P13 Filtro por empleado ' + CAST(@emp AS VARCHAR(10)),
                 CONCAT(COUNT(*), N' alarmas (directo ', @direct, N'); no visualizadas por el empleado: ', @others),
                 CASE WHEN COUNT(*) = @direct AND COUNT(*) > 0 AND @others = 0 THEN 1 ELSE 0 END FROM @t t;

/* P14: opciones del filtro Empleado = empleados que visualizaron alarmas activas del tipo */
DECLARE @emps TABLE (EMPLOYEE_ID INT, EMPLOYEE_NAME NVARCHAR(200));
INSERT @emps EXEC dbo.SP_GET_ALARM_LOG_VISUALIZERS 'MINIMUM_QUANTITY';
SELECT @direct = COUNT(DISTINCT v.EMPLOYEE_ID) FROM dbo.VW_ALARM_LOG_VISUALIZATIONS v JOIN dbo.VW_ALARM_LOG a ON a.TYPE_CODE = v.TYPE_CODE AND a.ALARM_KEY = v.ALARM_KEY WHERE v.TYPE_CODE = 'MINIMUM_QUANTITY';
INSERT @r SELECT 'P14 Empleados del filtro', CONCAT(COUNT(*), N' (directo ', @direct, N')'), CASE WHEN COUNT(*) = @direct AND COUNT(*) > 0 THEN 1 ELSE 0 END FROM @emps;

SELECT PRUEBA, DETALLE, OK FROM @r;
