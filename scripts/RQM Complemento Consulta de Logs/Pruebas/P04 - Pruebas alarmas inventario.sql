/* =====================================================================================
   RQM Complemento Consulta de Logs - T3b: Pruebas de alarmas de inventario + log
   Ejecutar después de "Despliegue\04 - Alarmas inventario estandar y log.sql" (J2/J3 al final: job, script 05).
   Todo corre dentro de una transacción que se REVIERTE al final: no deja cambios.
   Las referencias se eligen solas (alarma activa, con visualizaciones, artículo normal).
   Revisar la columna OK (1 = pasa).
   ===================================================================================== */
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @r TABLE (ESCENARIO VARCHAR(70), RESULTADO NVARCHAR(400), OK BIT);
DECLARE @log0 INT, @pending INT;

/* Datos de prueba elegidos automáticamente */
DECLARE @minRef INT, @minAlarm INT, @locRef INT, @locAlarm INT, @oosRef INT, @oosAlarm INT, @jobRef INT, @oc INT;

SELECT TOP 1 @minRef = f.REFERENCE_ID, @minAlarm = f.MINIMUM_QUANTITY_ALARM_ID
  FROM minimum_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID JOIN items i ON i.ITEM_ID = r.ITEM_ID
 WHERE f.ACTIVE = 1 AND i.IS_DOMESTIC_PRODUCT = 0 AND i.IS_SPECIAL_IMPORT = 0 AND i.IS_SALE_OFF = 0 AND i.DISABLE_ALARMS = 0
   AND EXISTS (SELECT 1 FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID)
   AND EXISTS (SELECT 1 FROM purchase_order_details d WHERE d.REFERENCE_ID = f.REFERENCE_ID)   -- evita que Trg_Set_Minimum_Quantity_Reference cambie el mínimo
 ORDER BY f.MINIMUM_QUANTITY_ALARM_ID;

SELECT TOP 1 @jobRef = f.REFERENCE_ID
  FROM minimum_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID JOIN items i ON i.ITEM_ID = r.ITEM_ID
 WHERE f.ACTIVE = 1 AND f.REFERENCE_ID <> @minRef AND i.DISABLE_ALARMS = 0
   AND EXISTS (SELECT 1 FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID);

SELECT TOP 1 @locRef = f.REFERENCE_ID, @locAlarm = f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID
  FROM minimum_local_warehouse_quantity_alarms f
  JOIN references_warehouse rw ON rw.REFERENCE_ID = f.REFERENCE_ID AND rw.WAREHOUSE_ID = (SELECT WAREHOUSE_ID FROM warehouses WHERE WAREHOUSE_CODE = 1)
  JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID JOIN items i ON i.ITEM_ID = r.ITEM_ID
 WHERE f.ACTIVE = 1 AND r.IS_ACTIVE = 1 AND i.IS_ACTIVE = 1 AND i.IS_DOMESTIC_PRODUCT = 0 AND i.IS_SPECIAL_IMPORT = 0 AND i.DISABLE_ALARMS = 0
   AND rw.QUANTITY > 1 AND rw.QUANTITY - 1 < r.MINIMUM_LOCAL_WAREHOUSE_QUANTITY
   AND EXISTS (SELECT 1 FROM visualized_minimum_local_warehouse_quantity_alarms v WHERE v.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID)
   AND EXISTS (SELECT 1 FROM references_warehouse z WHERE z.REFERENCE_ID = f.REFERENCE_ID AND z.WAREHOUSE_ID <> rw.WAREHOUSE_ID);

SELECT TOP 1 @oosRef = f.REFERENCE_ID, @oosAlarm = f.OUT_OF_STOCK_INVENTORY_ALARM_ID
  FROM out_of_stock_inventory_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID JOIN items i ON i.ITEM_ID = r.ITEM_ID
 WHERE f.ACTIVE = 1 AND r.ALARM_MINIMUM_QUANTITY <= 0 AND i.IS_DOMESTIC_PRODUCT = 0 AND i.IS_SPECIAL_IMPORT = 0 AND i.IS_SALE_OFF = 0 AND i.DISABLE_ALARMS = 0
   AND EXISTS (SELECT 1 FROM visualized_out_of_stock_inventory_alarms v WHERE v.OUT_OF_STOCK_INVENTORY_ALARM_ID = f.OUT_OF_STOCK_INVENTORY_ALARM_ID);

SELECT TOP 1 @oc = p.PURCHASE_ORDER_ID FROM purchase_orders p JOIN status_document_types s ON s.STATUS_DOCUMENT_TYPE_ID = p.STATUS_DOCUMENT_TYPE_ID WHERE s.STATUS_ORDER = 1 ORDER BY p.PURCHASE_ORDER_ID DESC;

BEGIN TRAN;

/* M1: refresco de cantidades mínimas por un movimiento -> log R, visualizaciones borradas */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE item_references SET ORDERED_QUANTITY = ORDERED_QUANTITY + 1 WHERE REFERENCE_ID = @minRef;
INSERT @r SELECT 'M1 Mínimos: refresco por comprometido +1',
       CONCAT(N'Activa=', f.ACTIVE, N' · Vistas restantes=', (SELECT COUNT(*) FROM visualized_minimum_quantity_alarms WHERE MINIMUM_QUANTITY_ALARM_ID = @minAlarm),
              N' · Log: ', l.EVENT_TYPE, N' ', l.PREVIOUS_QUANTITY, N'→', l.NEW_QUANTITY, N' vistas=', l.VISUALIZATION_COUNT, N' borradas=', l.VISUALIZATIONS_REMOVED, N' (', l.SOURCE, N')'),
       CASE WHEN f.ACTIVE = 1 AND l.EVENT_TYPE = 'R' AND l.NEW_QUANTITY = l.PREVIOUS_QUANTITY - 1 AND l.VISUALIZATIONS_REMOVED = 1
                 AND l.NEW_CREATION_DATE > l.PREVIOUS_CREATION_DATE
                 AND NOT EXISTS (SELECT 1 FROM visualized_minimum_quantity_alarms WHERE MINIMUM_QUANTITY_ALARM_ID = @minAlarm) THEN 1 ELSE 0 END
  FROM minimum_quantity_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @minAlarm AND ALARM_TYPE_CODE = 'MINIMUM_QUANTITY' ORDER BY 1 DESC) l
 WHERE f.MINIMUM_QUANTITY_ALARM_ID = @minAlarm;

/* M2 (H6): agregar la referencia a una OC pendiente NO inactiva la alarma (solo la refresca) */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
SELECT @pending = MINIMUM_QUANTITY * 10 FROM minimum_quantity_alarms WHERE MINIMUM_QUANTITY_ALARM_ID = @minAlarm;
INSERT purchase_order_details (PURCHASE_ORDER_ID, REFERENCE_ID, WAREHOUSE_ID, RECEIVED_QUANTITY, REQUESTED_QUANTITY) VALUES (@oc, @minRef, 1, 0, @pending);
INSERT @r SELECT 'M2 Mínimos: OC pendiente cubre el mínimo (no inactiva)',
       CONCAT(N'Activa=', f.ACTIVE, N' · Cantidad mostrada=', f.QUANTITY, N' / mínimo ', f.MINIMUM_QUANTITY, N' · Log: ', ISNULL(l.EVENT_TYPE, '-'), N' (', ISNULL(l.SOURCE, '-'), N')'),
       CASE WHEN f.ACTIVE = 1 AND f.QUANTITY >= f.MINIMUM_QUANTITY AND l.EVENT_TYPE = 'R' THEN 1 ELSE 0 END
  FROM minimum_quantity_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @minAlarm AND ALARM_TYPE_CODE = 'MINIMUM_QUANTITY' ORDER BY 1 DESC) l
 WHERE f.MINIMUM_QUANTITY_ALARM_ID = @minAlarm;

/* M3: entra el inventario (OC confirmada) -> inactiva con log I */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE item_references SET INVENTORY_QUANTITY = INVENTORY_QUANTITY + @pending + ORDERED_QUANTITY WHERE REFERENCE_ID = @minRef;
INSERT @r SELECT 'M3 Mínimos: ingreso de inventario (inactiva)',
       CONCAT(N'Activa=', f.ACTIVE, N' · Log: ', ISNULL(l.EVENT_TYPE, '-'), N' activa ', l.PREVIOUS_ACTIVE, N'→', l.NEW_ACTIVE, N' inactivación=', CONVERT(VARCHAR(16), l.INACTIVATION_DATE, 120), N' (', l.SOURCE, N')'),
       CASE WHEN f.ACTIVE = 0 AND l.EVENT_TYPE = 'I' AND l.PREVIOUS_ACTIVE = 1 AND l.NEW_ACTIVE = 0 AND l.INACTIVATION_DATE IS NOT NULL THEN 1 ELSE 0 END
  FROM minimum_quantity_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @minAlarm AND ALARM_TYPE_CODE = 'MINIMUM_QUANTITY' ORDER BY 1 DESC) l
 WHERE f.MINIMUM_QUANTITY_ALARM_ID = @minAlarm;

/* J1 (H3): @JOB = 1 refresca sin borrar visualizaciones (cantidad desfasada para que el refresco se registre) */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE minimum_quantity_alarms SET QUANTITY = QUANTITY + 1 WHERE REFERENCE_ID = @jobRef AND ACTIVE = 1;
EXEC dbo.SP_Set_Minimum_Quantity_Alarm @REFERENCE_ID = @jobRef, @JOB = 1;
INSERT @r SELECT 'J1 @JOB = 1: conserva visualizaciones',
       CONCAT(N'Vistas=', (SELECT COUNT(*) FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID),
              N' · Log: ', ISNULL(l.EVENT_TYPE, '-'), N' borradas=', l.VISUALIZATIONS_REMOVED, N' (', l.SOURCE, N')'),
       CASE WHEN l.EVENT_TYPE = 'R' AND l.VISUALIZATIONS_REMOVED = 0 AND l.SOURCE = 'Proceso programado'
                 AND (SELECT COUNT(*) FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID) = l.VISUALIZATION_COUNT THEN 1 ELSE 0 END
  FROM minimum_quantity_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID ORDER BY 1 DESC) l
 WHERE f.REFERENCE_ID = @jobRef AND f.ACTIVE = 1;

/* L1 (H1): movimiento en otra bodega no afecta la alarma de bodega local */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE references_warehouse SET QUANTITY = QUANTITY WHERE REFERENCE_ID = @locRef AND WAREHOUSE_ID <> (SELECT WAREHOUSE_ID FROM warehouses WHERE WAREHOUSE_CODE = 1);
INSERT @r SELECT 'L1 Bodega local: movimiento en Zona Franca (sin efecto)',
       CONCAT(N'Activa=', f.ACTIVE, N' · Eventos=', (SELECT COUNT(*) FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @locAlarm)),
       CASE WHEN f.ACTIVE = 1 AND NOT EXISTS (SELECT 1 FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @locAlarm) THEN 1 ELSE 0 END
  FROM minimum_local_warehouse_quantity_alarms f WHERE f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @locAlarm;

/* L2 (H1): refresco de bodega local -> log R, visualizaciones borradas */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE references_warehouse SET QUANTITY = QUANTITY - 1 WHERE REFERENCE_ID = @locRef AND WAREHOUSE_ID = (SELECT WAREHOUSE_ID FROM warehouses WHERE WAREHOUSE_CODE = 1);
INSERT @r SELECT 'L2 Bodega local: refresco (-1)',
       CONCAT(N'Activa=', f.ACTIVE, N' · Vistas restantes=', (SELECT COUNT(*) FROM visualized_minimum_local_warehouse_quantity_alarms WHERE MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @locAlarm),
              N' · Log: ', l.EVENT_TYPE, N' ', l.PREVIOUS_QUANTITY, N'→', l.NEW_QUANTITY, N' fecha ', CONVERT(VARCHAR(16), l.PREVIOUS_CREATION_DATE, 120), N'→', CONVERT(VARCHAR(16), l.NEW_CREATION_DATE, 120), N' borradas=', l.VISUALIZATIONS_REMOVED),
       CASE WHEN f.ACTIVE = 1 AND l.EVENT_TYPE = 'R' AND l.VISUALIZATIONS_REMOVED = 1
                 AND NOT EXISTS (SELECT 1 FROM visualized_minimum_local_warehouse_quantity_alarms WHERE MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @locAlarm) THEN 1 ELSE 0 END
  FROM minimum_local_warehouse_quantity_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @locAlarm AND ALARM_TYPE_CODE = 'MINIMUM_LOCAL_WAREHOUSE' ORDER BY 1 DESC) l
 WHERE f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @locAlarm;

/* L3: bodega local alcanza el mínimo -> inactiva con log I */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE rw SET QUANTITY = r.MINIMUM_LOCAL_WAREHOUSE_QUANTITY + 1
  FROM references_warehouse rw JOIN item_references r ON r.REFERENCE_ID = rw.REFERENCE_ID
 WHERE rw.REFERENCE_ID = @locRef AND rw.WAREHOUSE_ID = (SELECT WAREHOUSE_ID FROM warehouses WHERE WAREHOUSE_CODE = 1);
INSERT @r SELECT 'L3 Bodega local: alcanza el mínimo (inactiva)',
       CONCAT(N'Activa=', f.ACTIVE, N' · Log: ', ISNULL(l.EVENT_TYPE, '-'), N' (', l.SOURCE, N')'),
       CASE WHEN f.ACTIVE = 0 AND l.EVENT_TYPE = 'I' THEN 1 ELSE 0 END
  FROM minimum_local_warehouse_quantity_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @locAlarm AND ALARM_TYPE_CODE = 'MINIMUM_LOCAL_WAREHOUSE' ORDER BY 1 DESC) l
 WHERE f.MINIMUM_LOCAL_WAREHOUSE_QUANTITY_ALARM_ID = @locAlarm;

/* O1: sin stock, refresco */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE item_references SET ORDERED_QUANTITY = ORDERED_QUANTITY + 1 WHERE REFERENCE_ID = @oosRef;
INSERT @r SELECT 'O1 Sin stock: refresco por comprometido +1',
       CONCAT(N'Activa=', f.ACTIVE, N' · Log: ', ISNULL(l.EVENT_TYPE, '-'), N' ', l.PREVIOUS_QUANTITY, N'→', l.NEW_QUANTITY, N' borradas=', l.VISUALIZATIONS_REMOVED),
       CASE WHEN f.ACTIVE = 1 AND l.EVENT_TYPE = 'R' AND l.VISUALIZATIONS_REMOVED = 1 THEN 1 ELSE 0 END
  FROM out_of_stock_inventory_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @oosAlarm AND ALARM_TYPE_CODE = 'OUT_OF_STOCK' ORDER BY 1 DESC) l
 WHERE f.OUT_OF_STOCK_INVENTORY_ALARM_ID = @oosAlarm;

/* O2: sin stock, ingreso de inventario -> inactiva con log I */
SET @log0 = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
UPDATE item_references SET INVENTORY_QUANTITY = INVENTORY_QUANTITY + ORDERED_QUANTITY + 20 WHERE REFERENCE_ID = @oosRef;
INSERT @r SELECT 'O2 Sin stock: ingreso de inventario (inactiva)',
       CONCAT(N'Activa=', f.ACTIVE, N' · Log: ', ISNULL(l.EVENT_TYPE, '-'), N' (', l.SOURCE, N')'),
       CASE WHEN f.ACTIVE = 0 AND l.EVENT_TYPE = 'I' THEN 1 ELSE 0 END
  FROM out_of_stock_inventory_alarms f
  OUTER APPLY (SELECT TOP 1 * FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @log0 AND ALARM_ID = @oosAlarm AND ALARM_TYPE_CODE = 'OUT_OF_STOCK' ORDER BY 1 DESC) l
 WHERE f.OUT_OF_STOCK_INVENTORY_ALARM_ID = @oosAlarm;

ROLLBACK;

SELECT CONCAT('Referencias usadas: mínimos=', @minRef, ' job=', @jobRef, ' bodega local=', @locRef, ' sin stock=', @oosRef, ' OC=', @oc) AS ESCENARIO, NULL AS RESULTADO, NULL AS OK
UNION ALL
SELECT ESCENARIO, RESULTADO, OK FROM @r;

/* =====================================================================================
   J2/J3 - Job "Verificacion Alarmas" (T3c). Transacción revertida.
   J2: el recálculo diario (@JOB = 1) sin cambio de cantidad NO deja filas R en el log.
   J3: la depuración (UPDATE directo del job, script 05) deja una fila I por alarma inactivada.
   Simula el paso "Ejecucion Cantidades Minimas" con un artículo marcado DISABLE_ALARMS = 1.
   ===================================================================================== */
SET NOCOUNT ON;
DECLARE @LogIni INT = ISNULL((SELECT MAX(INVENTORY_ALARM_LOG_ID) FROM INVENTORY_ALARM_LOG), 0);
DECLARE @Item INT = (SELECT TOP 1 r.ITEM_ID FROM minimum_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID JOIN items i ON i.ITEM_ID = r.ITEM_ID
                      WHERE f.ACTIVE = 1 AND i.IS_ACTIVE = 1 AND r.IS_ACTIVE = 1 AND i.DISABLE_ALARMS = 0 AND i.IS_SALE_OFF = 0 AND i.IS_SPECIAL_IMPORT = 0 AND i.IS_DOMESTIC_PRODUCT = 0);
DECLARE @Esperadas INT = (SELECT COUNT(*) FROM minimum_quantity_alarms f JOIN item_references r ON r.REFERENCE_ID = f.REFERENCE_ID WHERE f.ACTIVE = 1 AND r.ITEM_ID = @Item);
BEGIN TRAN;
UPDATE items SET DISABLE_ALARMS = 1 WHERE ITEM_ID = @Item;

DECLARE @References TABLE (IdReg INT NOT NULL IDENTITY(1,1) PRIMARY KEY, REFERENCE_ID INT), @Reference_Id INT, @IdReg INT, @TotalReg INT;
INSERT INTO @References (REFERENCE_ID)
SELECT REFERENCE_ID FROM items a WITH (NOLOCK) JOIN item_references b WITH (NOLOCK) ON b.ITEM_ID = a.ITEM_ID
 WHERE a.IS_ACTIVE = 1 AND a.IS_SALE_OFF = 0 AND a.IS_SPECIAL_IMPORT = 0 AND a.IS_DOMESTIC_PRODUCT = 0 AND b.IS_ACTIVE = 1 AND b.ALARM_MINIMUM_QUANTITY > 0 AND a.DISABLE_ALARMS = 0;
SELECT @IdReg = MIN(IdReg), @TotalReg = MAX(IdReg) FROM @References;
WHILE @IdReg <= @TotalReg
BEGIN
	SELECT @Reference_Id = REFERENCE_ID FROM @References WHERE IdReg = @IdReg;
	EXEC SP_Set_Minimum_Quantity_Alarm @Reference_Id, 1;
	SET @IdReg += 1;
END

DECLARE @Inactivated TABLE (ALARM_ID INT, REFERENCE_ID INT, PREVIOUS_QUANTITY INT, NEW_QUANTITY INT, PREVIOUS_MINIMUM_QUANTITY INT, NEW_MINIMUM_QUANTITY INT,
                            PREVIOUS_CREATION_DATE DATETIME, NEW_CREATION_DATE DATETIME, PREVIOUS_ACTIVE BIT, NEW_ACTIVE BIT, INACTIVATION_DATE DATETIME);
UPDATE a SET a.ACTIVE = 0, a.INACTIVATION_DATE = GETDATE()
OUTPUT inserted.MINIMUM_QUANTITY_ALARM_ID, inserted.REFERENCE_ID, deleted.QUANTITY, inserted.QUANTITY, deleted.MINIMUM_QUANTITY, inserted.MINIMUM_QUANTITY,
       deleted.CREATION_DATE, inserted.CREATION_DATE, deleted.ACTIVE, inserted.ACTIVE, inserted.INACTIVATION_DATE INTO @Inactivated
  FROM minimum_quantity_alarms a JOIN item_references b ON b.REFERENCE_ID = a.REFERENCE_ID JOIN items c ON c.ITEM_ID = b.ITEM_ID
 WHERE a.ACTIVE = 1 AND (c.IS_ACTIVE = 0 OR b.IS_ACTIVE = 0 OR c.IS_DOMESTIC_PRODUCT = 1 OR c.IS_SALE_OFF = 1 OR c.IS_SPECIAL_IMPORT = 1 OR c.DISABLE_ALARMS = 1 OR b.ALARM_MINIMUM_QUANTITY <= 0);
INSERT INTO dbo.INVENTORY_ALARM_LOG (ALARM_TYPE_CODE, ALARM_ID, REFERENCE_ID, EVENT_TYPE, PREVIOUS_QUANTITY, NEW_QUANTITY, PREVIOUS_MINIMUM_QUANTITY, NEW_MINIMUM_QUANTITY,
       PREVIOUS_CREATION_DATE, NEW_CREATION_DATE, PREVIOUS_ACTIVE, NEW_ACTIVE, INACTIVATION_DATE, VISUALIZATION_COUNT, VISUALIZATIONS_REMOVED, SOURCE)
SELECT 'MINIMUM_QUANTITY', i.ALARM_ID, i.REFERENCE_ID, 'I', i.PREVIOUS_QUANTITY, i.NEW_QUANTITY, i.PREVIOUS_MINIMUM_QUANTITY, i.NEW_MINIMUM_QUANTITY,
       i.PREVIOUS_CREATION_DATE, i.NEW_CREATION_DATE, i.PREVIOUS_ACTIVE, i.NEW_ACTIVE, i.INACTIVATION_DATE,
       (SELECT COUNT(*) FROM visualized_minimum_quantity_alarms v WHERE v.MINIMUM_QUANTITY_ALARM_ID = i.ALARM_ID), 0, 'Proceso programado - depuración'
  FROM @Inactivated i;

SELECT 'J2' AS PRUEBA, CASE WHEN NOT EXISTS (SELECT 1 FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @LogIni AND EVENT_TYPE = 'R' AND PREVIOUS_QUANTITY = NEW_QUANTITY) THEN 'OK' ELSE 'FALLA' END AS RESULTADO
UNION ALL
SELECT 'J3', CASE WHEN (SELECT COUNT(*) FROM INVENTORY_ALARM_LOG WHERE INVENTORY_ALARM_LOG_ID > @LogIni AND EVENT_TYPE = 'I' AND SOURCE = 'Proceso programado - depuración') = @Esperadas AND @Esperadas > 0 THEN 'OK' ELSE 'FALLA' END;
ROLLBACK;
