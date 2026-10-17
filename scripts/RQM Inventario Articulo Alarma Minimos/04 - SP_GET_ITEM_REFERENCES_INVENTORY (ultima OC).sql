/* =====================================================================================
   RQM  : Histórico de Precios y Proveedor en Inventario de Mínimos - Opción 2 - Tarea T12
   Autor: Andrés Ricardo Díaz Cárdenas
   Fecha: 2026-10-10

   Objetos:
     Procedure : SP_GET_ITEM_REFERENCES_INVENTORY (modificación del script 01)
                 Agrega los datos de la ÚLTIMA ORDEN DE COMPRA CONFIRMADA de cada referencia.

   Columnas nuevas (anulables: NULL si la referencia no tiene OC confirmada - D13):
     ProviderName      : providers.PROVIDER_NAME
     PurchaseOrderDate : purchase_orders.REQUEST_DATE
     RealReceiptDate   : purchase_orders.REAL_RECEIPT_DATE
     RequestedQuantity : purchase_order_details.REQUESTED_QUANTITY
     ReceivedQuantity  : purchase_order_details.RECEIVED_QUANTITY

   Reglas:
     D11 : OC tipo 'O' en estado Confirmada (STATUS_ORDER = 2); la más reciente por
           REAL_RECEIPT_DATE DESC, desempate PURCHASE_ORDER_ID DESC.
     D12 : una OC tiene un solo detalle por referencia: no se suman cantidades.
     Columnas existentes, fórmulas y orden: sin cambios.

   Script idempotente: puede ejecutarse varias veces. Solo lectura.
   ===================================================================================== */
USE Aldebaran
GO

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
           ISNULL(CAST(IIF(i.REFERENCE_ID = @ReferenceId, 1, 0) AS BIT), 0) AS IsAlarmReference,
           lp.PROVIDER_NAME                                   AS ProviderName,
           lp.REQUEST_DATE                                    AS PurchaseOrderDate,
           lp.REAL_RECEIPT_DATE                               AS RealReceiptDate,
           lp.REQUESTED_QUANTITY                              AS RequestedQuantity,
           lp.RECEIVED_QUANTITY                               AS ReceivedQuantity
      FROM Inventory i
      OUTER APPLY
      (
          /* Última OC Confirmada de la referencia (D11): mayor fecha real de recibo,
             desempate por PURCHASE_ORDER_ID. Una OC tiene un solo detalle por referencia (D12). */
          SELECT TOP (1)
                 p.PROVIDER_NAME,
                 po.REQUEST_DATE,
                 po.REAL_RECEIPT_DATE,
                 pod.REQUESTED_QUANTITY,
                 pod.RECEIVED_QUANTITY
            FROM dbo.purchase_order_details pod
            JOIN dbo.purchase_orders po         ON po.PURCHASE_ORDER_ID = pod.PURCHASE_ORDER_ID
            JOIN dbo.status_document_types sdt  ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
            JOIN dbo.document_types dt          ON dt.DOCUMENT_TYPE_ID = sdt.DOCUMENT_TYPE_ID AND dt.DOCUMENT_TYPE_CODE = 'O'
            JOIN dbo.providers p                ON p.PROVIDER_ID = po.PROVIDER_ID
           WHERE pod.REFERENCE_ID = i.REFERENCE_ID
             AND sdt.STATUS_ORDER IN (1,2,4)
           ORDER BY po.REAL_RECEIPT_DATE DESC, po.PURCHASE_ORDER_ID DESC
      ) lp
     ORDER BY i.REFERENCE_NAME;
END
GO
