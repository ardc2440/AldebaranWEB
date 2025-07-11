CREATE OR ALTER PROCEDURE SP_GET_CUSTOMER_ORDER_DETAILS_WITH_AUTOMATIC_ASSIGMENT
	@ProcessOrderId INT = NULL
AS
BEGIN
	SELECT b.REFERENCE_ID, CONCAT('[', c.INTERNAL_REFERENCE, '] ', c.ITEM_NAME, ' - ', b.REFERENCE_NAME) ARTICLE_NAME, e.REQUESTED_QUANTITY, a.ASSIGNED_QUANTITY,
		   e.REQUESTED_QUANTITY - e.DELIVERED_QUANTITY - e.PROCESSED_QUANTITY PENDING_QUANTITY 
	  FROM automatic_in_process_detail a WITH (NOLOCK)	  
	  JOIN item_references b WITH (NOLOCK) ON b.REFERENCE_ID = a.REFERENCE_ID
	  JOIN items c WITH (NOLOCK) ON c.ITEM_ID = b.ITEM_ID
	  JOIN customer_order_details e WITH (NOLOCK) ON e.CUSTOMER_ORDER_DETAIL_ID = a.CUSTOMER_ORDER_DETAIL_ID
	 WHERE a.AUTOMATIC_IN_PROCESS_ORDER_ID = @ProcessOrderId 
END
GO