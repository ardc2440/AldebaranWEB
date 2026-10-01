/* =====================================================================================
   RQM  : Inventario del artículo en la alarma de cantidades mínimas - Tarea T1
   Autor: Andrés Ricardo Díaz Cárdenas
   Fecha: 2026-09-30

   Objetos:
     Procedure : SP_GET_ITEM_REFERENCES_INVENTORY
                 A partir de la referencia de una alarma de cantidades mínimas, ubica su artículo
                 y devuelve el inventario de todas sus referencias ACTIVAS para el diálogo
                 "Inventario del artículo" de la bandeja "Sobrepaso de cantidades mínimas".
     Índice    : IX_PURCHASE_ORDER_DETAILS_REFERENCE_ID (apoyo al cálculo de tránsito por referencia)

   Parámetros:
     @ReferenceId : referencia de la alarma. Inexistente o NULL = conjunto vacío (misma estructura).

   Columnas (una fila por referencia activa del artículo, ordenadas por nombre de referencia):
     ReferenceId, ReferenceCode, ReferenceName
     LocalWarehouse   : REFERENCES_WAREHOUSE en la bodega WAREHOUSE_CODE = 1
     FreeZone         : REFERENCES_WAREHOUSE en la bodega WAREHOUSE_CODE = 2
     PhysicalStock    : Stock Físico = LocalWarehouse + FreeZone
     InTransit        : REQUESTED_QUANTITY de OC (DOCUMENT_TYPE_CODE 'O') Pendientes (STATUS_ORDER 1)
                        + En aprobación (STATUS_ORDER 4)
     Committed        : RESERVED_QUANTITY + ORDERED_QUANTITY (Reservas + Pedidos)
     Available        : PhysicalStock + InTransit - Committed                         (decisión D2)
     IsAlarmReference : 1 para la referencia recibida en @ReferenceId

   Notas de diseño:
     - Mismas fuentes y reglas del cálculo de Fase 1 (SP_CSV_EXPORT_INVENTORY), salvo el stock
       físico: aquí es Bodega Local + Zona Franca (D2) y no ITEM_REFERENCES.INVENTORY_QUANTITY.
     - Las agregaciones se limitan a las referencias del artículo (no recorre todo el inventario).
     - No filtra por artículo activo: si la alarma existe, el usuario debe ver su inventario.
     - Siempre devuelve un único conjunto de resultados con la misma estructura (mapeo keyless en EF).
     - ISNULL en las columnas calculadas: el metadato del resultado queda NOT NULL en todas
       (la entidad de EF usa int / bool no anulables). No cambia ninguna cifra.
     - Solo lectura.

   Script idempotente: puede ejecutarse varias veces.
   ===================================================================================== */
USE Aldebaran
GO

/* -------------------------------------------------------------------------------------
   1. ÍNDICE DE APOYO
      PURCHASE_ORDER_DETAILS solo tiene índices que inician por PURCHASE_ORDER_ID, por lo que
      sumar el tránsito de una referencia recorrería todos los detalles de OC en cada consulta.
      Se crea únicamente si no existe ya un índice cuya primera columna sea REFERENCE_ID.
   ------------------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1
                 FROM sys.index_columns ic
                 JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                WHERE ic.object_id   = OBJECT_ID('dbo.purchase_order_details')
                  AND ic.key_ordinal = 1
                  AND c.name         = 'REFERENCE_ID')
    CREATE NONCLUSTERED INDEX IX_PURCHASE_ORDER_DETAILS_REFERENCE_ID
        ON dbo.purchase_order_details (REFERENCE_ID)
        INCLUDE (PURCHASE_ORDER_ID, REQUESTED_QUANTITY);
GO

/* -------------------------------------------------------------------------------------
   2. PROCEDIMIENTO
   ------------------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.SP_GET_ITEM_REFERENCES_INVENTORY
    @ReferenceId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ItemId INT;

    SELECT @ItemId = r.ITEM_ID
      FROM dbo.item_references r
     WHERE r.REFERENCE_ID = @ReferenceId;

    WITH ItemReferences AS
    (
        SELECT r.REFERENCE_ID,
               r.REFERENCE_CODE,
               r.REFERENCE_NAME,
               r.RESERVED_QUANTITY + r.ORDERED_QUANTITY AS COMMITTED_QUANTITY
          FROM dbo.item_references r
         WHERE r.ITEM_ID   = @ItemId
           AND r.IS_ACTIVE = 1
    ),
    WarehouseStock AS
    (
        SELECT rw.REFERENCE_ID,
               SUM(CASE WHEN w.WAREHOUSE_CODE = 1 THEN rw.QUANTITY ELSE 0 END) AS LOCAL_QUANTITY,
               SUM(CASE WHEN w.WAREHOUSE_CODE = 2 THEN rw.QUANTITY ELSE 0 END) AS FREE_ZONE_QUANTITY
          FROM ItemReferences ir
          JOIN dbo.references_warehouse rw ON rw.REFERENCE_ID = ir.REFERENCE_ID
          JOIN dbo.warehouses w            ON w.WAREHOUSE_ID  = rw.WAREHOUSE_ID
         GROUP BY rw.REFERENCE_ID
    ),
    Transit AS
    (
        SELECT pod.REFERENCE_ID,
               SUM(pod.REQUESTED_QUANTITY) AS TRANSIT_QUANTITY
          FROM ItemReferences ir
          JOIN dbo.purchase_order_details pod ON pod.REFERENCE_ID = ir.REFERENCE_ID
          JOIN dbo.purchase_orders po         ON po.PURCHASE_ORDER_ID = pod.PURCHASE_ORDER_ID
          JOIN dbo.status_document_types sdt  ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
          JOIN dbo.document_types dt          ON dt.DOCUMENT_TYPE_ID = sdt.DOCUMENT_TYPE_ID AND dt.DOCUMENT_TYPE_CODE = 'O'
         WHERE sdt.STATUS_ORDER IN (1, 4)
         GROUP BY pod.REFERENCE_ID
    ),
    Inventory AS
    (
        SELECT ir.REFERENCE_ID,
               ir.REFERENCE_CODE,
               ir.REFERENCE_NAME,
               ISNULL(ws.LOCAL_QUANTITY, 0)     AS LOCAL_QUANTITY,
               ISNULL(ws.FREE_ZONE_QUANTITY, 0) AS FREE_ZONE_QUANTITY,
               ISNULL(t.TRANSIT_QUANTITY, 0)    AS TRANSIT_QUANTITY,
               ir.COMMITTED_QUANTITY
          FROM ItemReferences ir
          LEFT JOIN WarehouseStock ws ON ws.REFERENCE_ID = ir.REFERENCE_ID
          LEFT JOIN Transit t         ON t.REFERENCE_ID  = ir.REFERENCE_ID
    )
    SELECT i.REFERENCE_ID                                     AS ReferenceId,
           i.REFERENCE_CODE                                   AS ReferenceCode,
           i.REFERENCE_NAME                                   AS ReferenceName,
           i.LOCAL_QUANTITY                                   AS LocalWarehouse,
           i.FREE_ZONE_QUANTITY                               AS FreeZone,
           ISNULL(i.LOCAL_QUANTITY + i.FREE_ZONE_QUANTITY, 0) AS PhysicalStock,
           i.TRANSIT_QUANTITY                                 AS InTransit,
           ISNULL(i.COMMITTED_QUANTITY, 0)                    AS [Committed],
           ISNULL(i.LOCAL_QUANTITY + i.FREE_ZONE_QUANTITY
             + i.TRANSIT_QUANTITY - i.COMMITTED_QUANTITY, 0)  AS Available,
           ISNULL(CAST(IIF(i.REFERENCE_ID = @ReferenceId, 1, 0) AS BIT), 0) AS IsAlarmReference
      FROM Inventory i
     ORDER BY i.REFERENCE_NAME;
END
GO
