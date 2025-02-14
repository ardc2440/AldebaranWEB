CREATE OR ALTER   PROCEDURE [dbo].[SP_Automatic_In_Process_Registry]
	@AutomaticInProcessId INT = NULL,
	@PurchaseOrderId INT,
	@CustomerOrderId INT,
	@CustomerOrderInProcessId INT,
	@NewAutomaticInProcessId INT OUT
AS
BEGIN	
	
	IF @AutomaticInProcessId IS NULL
	BEGIN
		INSERT INTO automatic_in_process (PURCHASE_ORDER_ID)
			 VALUES (@PurchaseOrderId)
	
		SET @NewAutomaticInProcessId = SCOPE_IDENTITY()
	END 
	ELSE
		SET @NewAutomaticInProcessId = @AutomaticInProcessId

	INSERT INTO automatic_in_process_orders (AUTOMATIC_IN_PROCESS_ID, CUSTOMER_ORDER_IN_PROCESS_ID, CUSTOMER_ORDER_ID) 
	     VALUES (@NewAutomaticInProcessId, @CustomerOrderInProcessId, @CustomerOrderId)

	DECLARE @AutomaticInProcessOrderId INT = SCOPE_IDENTITY()

	INSERT INTO automatic_in_process_detail (AUTOMATIC_IN_PROCESS_ORDER_ID, CUSTOMER_ORDER_IN_PROCESS_DETAIL_ID, CUSTOMER_ORDER_DETAIL_ID, REFERENCE_ID, WAREHOUSE_ID, ASSIGNED_QUANTITY, BRAND)
		 SELECT @AutomaticInProcessOrderId, a.CUSTOMER_ORDER_IN_PROCESS_DETAIL_ID, b.CUSTOMER_ORDER_DETAIL_ID, b.REFERENCE_ID, a.WAREHOUSE_ID, a.PROCESSED_QUANTITY, a.BRAND
		   FROM customer_order_in_process_details a WITH (NOLOCK) 
		   JOIN customer_order_details b WITH (NOLOCK) ON b.CUSTOMER_ORDER_DETAIL_ID = a.CUSTOMER_ORDER_DETAIL_ID
													  AND b.CUSTOMER_ORDER_ID = @CustomerOrderId
		  WHERE CUSTOMER_ORDER_IN_PROCESS_ID = @CustomerOrderInProcessId		   
END
