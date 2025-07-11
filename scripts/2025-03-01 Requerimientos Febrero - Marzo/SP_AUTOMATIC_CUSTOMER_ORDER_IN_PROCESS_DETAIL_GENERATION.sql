CREATE OR ALTER PROCEDURE [dbo].[SP_AUTOMATIC_CUSTOMER_ORDER_IN_PROCESS_DETAIL_GENERATION] 
	@CustomerOrderinProcessId INT, 
	@QuantityToAssign INT, 
	@WarehouseId INT, 
	@CustomerOrderDetailId INT, 
	@ReferenceId INT,
	@Brand VARCHAR(250)
AS 
BEGIN
	/* Se crea el detalle del traslado a proceso */
	INSERT INTO customer_order_in_process_details (CUSTOMER_ORDER_IN_PROCESS_ID, CUSTOMER_ORDER_DETAIL_ID, WAREHOUSE_ID, PROCESSED_QUANTITY, BRAND)
		 VALUES (@CustomerOrderinProcessId, @CustomerOrderDetailId, @WarehouseId, @QuantityToAssign, @Brand)		

	/* Descontar de InventoryQuantity y OrderedQuantity, y Sumar a WORK_IN_PROCESS_QUANTITY en el Inventario (Item_Reference)*/
	UPDATE item_references 
	   SET INVENTORY_QUANTITY -= @QuantityToAssign,
		   ORDERED_QUANTITY -= @QuantityToAssign,
		   WORK_IN_PROCESS_QUANTITY += @QuantityToAssign
	 WHERE REFERENCE_ID = @ReferenceId
		   
	/* Descontar de la Bodea */
	UPDATE references_warehouse
	   SET QUANTITY -= @QuantityToAssign
	 WHERE REFERENCE_ID = @ReferenceId
	   AND WAREHOUSE_ID = @WarehouseId
			
	/* Descontar (Sumar A Processed_Quantity en el Pedido (CUSTIOMER_ORDER_DETAIL) */
	UPDATE customer_order_details
	   SET PROCESSED_QUANTITY += @QuantityToAssign
     WHERE CUSTOMER_ORDER_DETAIL_ID = @CustomerOrderDetailId
END