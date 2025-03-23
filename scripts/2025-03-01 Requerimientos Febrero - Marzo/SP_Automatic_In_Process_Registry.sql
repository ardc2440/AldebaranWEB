CREATE OR ALTER   PROCEDURE [dbo].[SP_AUTOMATIC_IN_PROCESS_REGISTRY]
	@AutomaticInProcessId INT = NULL,
	@DocumentType CHAR(1),
	@DocumentId INT,
	@CustomerOrderId INT,
	@CustomerOrderInProcessId INT,
	@NewAutomaticInProcessId INT OUT
AS
BEGIN	
	DECLARE @DocumentTypeId SMALLINT

	IF @AutomaticInProcessId IS NULL
	BEGIN
		SELECT @DocumentTypeId = DOCUMENT_TYPE_ID
		  FROM document_types 
		 WHERE DOCUMENT_TYPE_CODE = @DocumentType

		INSERT INTO automatic_in_process (DOCUMENT_TYPE_ID, DOCUMENT_ID)
			 VALUES (@DocumentTypeId, @DocumentId)
	
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
