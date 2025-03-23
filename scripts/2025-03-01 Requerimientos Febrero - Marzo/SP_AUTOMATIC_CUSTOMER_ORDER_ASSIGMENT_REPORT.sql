CREATE OR ALTER   PROCEDURE [dbo].[SP_AUTOMATIC_CUSTOMER_ORDER_ASSIGMENT_REPORT]
	@PurchaseOrderNumber VARCHAR(10) = NULL, 
	@ProviderId INT = NULL, 
	@ProformaNumber VARCHAR(50) = NULL, 
	@ImportNumber VARCHAR(20) = NULL, 
	@ReceiptDateFrom DATE = NULL, @ReceiptDateTo DATE = NULL, 
	@ConfirmedDateFrom DATE = NULL, @ConfirmedDateTo DATE = NULL, 
	@CustomerOrderNumber VARCHAR(10) = NULL, 
	@CustomerId INT = NULL, 
	@OrderDateFrom DATE = NULL, @OrderDateTo DATE = NULL, 
	@EstimatedDeliveryDateFrom DATE = NULL, @EstimatedDeliveryDateTo DATE = NULL,
	@StatusDocumentTypeId INT = NULL, 
	@ReferenceIds VARCHAR(MAX) = ''
AS
BEGIN
	DECLARE @FilterReferences TABLE (ReferenceId INT NOT NULL PRIMARY KEY)
	
	IF LEN(RTRIM(@ReferenceIds)) > 0
		INSERT INTO @FilterReferences
			 SELECT value FROM STRING_SPLIT(@ReferenceIds,',')
	ELSE
		INSERT INTO @FilterReferences
			 SELECT REFERENCE_ID 
			   FROM item_references

	SELECT b.PURCHASE_ORDER_ID PurchaseOrderId, b.ORDER_NUMBER PurchaseOrderNumber, c.IDENTITY_NUMBER ProviderIdentity, c.PROVIDER_NAME ProviderName, 
		   b.PROFORMA_NUMBER ProformaNumber, b.IMPORT_NUMBER ImportNumber, b.REAL_RECEIPT_DATE ReceipDate, CAST (a.Creation_Date AS date) ConfirmationDate, 
		   e.CUSTOMER_ORDER_ID CustomerOrderId, e.ORDER_NUMBER CustomerOrderNumber, f.IDENTITY_NUMBER CustomerIdentity, f.CUSTOMER_NAME CustomerName, 
		   e.ORDER_DATE OrderDate, e.ESTIMATED_DELIVERY_DATE EstimatedDeliveryDate, g.STATUS_DOCUMENT_TYPE_NAME StatusOrderName, j.ITEM_ID ItemId, 
		   j.INTERNAL_REFERENCE InternalReference, j.ITEM_NAME ItemName, i.REFERENCE_NAME ReferenceName, k.REQUESTED_QUANTITY Requested, 
		   h.ASSIGNED_QUANTITY Assigned, (k.REQUESTED_QUANTITY - k.DELIVERED_QUANTITY - k.PROCESSED_QUANTITY) Pending 
	  FROM automatic_in_process a
	  JOIN purchase_orders b ON b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
	  JOIN providers c ON c.PROVIDER_ID = b.PROVIDER_ID	  	 

	  JOIN automatic_in_process_orders d ON d.AUTOMATIC_IN_PROCESS_ID = a.AUTOMATIC_IN_PROCESS_ID 	  
	  JOIN customer_orders e ON e.CUSTOMER_ORDER_ID = d.CUSTOMER_ORDER_ID
	  JOIN customers f ON f.CUSTOMER_ID = e.CUSTOMER_ID 
	  JOIN status_document_types g ON g.STATUS_DOCUMENT_TYPE_ID = e.STATUS_DOCUMENT_TYPE_ID		 

	  JOIN automatic_in_process_detail h ON h.AUTOMATIC_IN_PROCESS_ORDER_ID = d.AUTOMATIC_IN_PROCESS_ORDER_ID	  
	  JOIN item_references i ON i.REFERENCE_ID = h.REFERENCE_ID
	  JOIN items j ON j.ITEM_ID = i.ITEM_ID
	  JOIN customer_order_details k on k.CUSTOMER_ORDER_DETAIL_ID = h.CUSTOMER_ORDER_DETAIL_ID
	 WHERE EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = i.REFERENCE_ID)
	   AND (b.ORDER_NUMBER like '%'+@PurchaseOrderNumber+'%' OR @PurchaseOrderNumber IS NULL)
	   AND (c.PROVIDER_ID = @ProviderId OR @ProviderId IS NULL)
	   AND (b.PROFORMA_NUMBER like '%'+@ProformaNumber+'%' OR @ProformaNumber IS NULL)
	   AND (b.IMPORT_NUMBER like '%'+@ImportNumber+'%' OR @ImportNumber IS NULL)
	   AND (b.REAL_RECEIPT_DATE BETWEEN @ReceiptDateFrom AND @ReceiptDateTo OR @ReceiptDateFrom IS NULL)
	   AND (a.CREATION_DATE BETWEEN @ConfirmedDateFrom AND @ConfirmedDateTo OR @ConfirmedDateFrom IS NULL)
	   AND (e.ORDER_NUMBER like '%'+@CustomerOrderNumber+'%' OR @CustomerOrderNumber IS NULL)
	   AND (f.CUSTOMER_ID = @CustomerId OR @CustomerId IS NULL)
	   AND (e.ORDER_DATE BETWEEN @OrderDateFrom AND @OrderDateTo OR @OrderDateFrom IS NULL)
	   AND (e.ESTIMATED_DELIVERY_DATE BETWEEN @EstimatedDeliveryDateFrom AND @EstimatedDeliveryDateTo OR @EstimatedDeliveryDateFrom IS NULL)
	   AND (g.STATUS_DOCUMENT_TYPE_ID = @StatusDocumentTypeId OR @StatusDocumentTypeId IS NULL)	   
END
	 
GO


