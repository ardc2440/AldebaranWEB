CREATE OR ALTER PROCEDURE SP_GET_CONFIRMED_PURCHASES_WITH_AUTOMATIC_ASSIGMENT
	@EmployeeId INT = NULL,
	@SearchKey VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @PurchaseOrderDocumentTypeId SMALLINT

	SELECT @PurchaseOrderDocumentTypeId = DOCUMENT_TYPE_ID 
	  FROM document_types 
	 WHERE DOCUMENT_TYPE_CODE = 'O'

	SELECT a.AUTOMATIC_IN_PROCESS_ID, CAST ('O' AS VARCHAR(1)) DOCUMENT_TYPE, b.PURCHASE_ORDER_ID DOCUMENT_ID, CAST (b.ORDER_NUMBER AS VARCHAR(10)) ORDER_NUMBER, c.IDENTITY_NUMBER, c.PROVIDER_NAME, b.PROFORMA_NUMBER,
		   b.IMPORT_NUMBER, b.REAL_RECEIPT_DATE, CAST (a.Creation_Date AS date) CREATION_DATE
	  FROM automatic_in_process a WITH (NOLOCK)
	  JOIN purchase_orders b WITH (NOLOCK) ON b.PURCHASE_ORDER_ID = a.DOCUMENT_ID
	  JOIN providers c WITH (NOLOCK) ON c.PROVIDER_ID = b.PROVIDER_ID	  
	 WHERE a.DOCUMENT_TYPE_ID = @PurchaseOrderDocumentTypeId 
	   AND NOT EXISTS (SELECT 1 
						 FROM visualized_automatic_in_process va WITH (NOLOCK)
						WHERE va.AUTOMATIC_IN_PROCESS_ID = a.AUTOMATIC_IN_PROCESS_ID
						  AND (va.EMPLOYEE_ID = @EmployeeId or @EmployeeId IS NULL)) 
	   AND (@SearchKey IS NULL 
	    OR b.ORDER_NUMBER LIKE '%'+@SearchKey+'%'
		OR c.IDENTITY_NUMBER LIKE '%'+@SearchKey+'%'
		OR c.PROVIDER_NAME LIKE '%'+@SearchKey+'%'
		OR b.PROFORMA_NUMBER LIKE '%'+@SearchKey+'%'
		OR b.IMPORT_NUMBER LIKE '%'+@SearchKey+'%'
		OR REPLACE(CONVERT(VARCHAR(10), b.REAL_RECEIPT_DATE, 111),'/','\') LIKE '%'+@SearchKey+'%'
		OR REPLACE(CONVERT(VARCHAR(10), CAST (a.Creation_Date AS date), 111),'/','\') LIKE '%'+@SearchKey+'%')
	UNION 
	SELECT a.AUTOMATIC_IN_PROCESS_ID, 'B', b.WAREHOUSE_TRANSFER_ID, CAST(b.WAREHOUSE_TRANSFER_ID AS VARCHAR(10)), 'N/A', 'Bodega Zona Franca', 'N/A',
		   b.NATIONALIZATION, b.TRANSFER_DATE, CAST (a.Creation_Date AS date) CREATION_DATE
	  FROM automatic_in_process a WITH (NOLOCK)
	  JOIN warehouse_transfers  b WITH (NOLOCK) ON b.WAREHOUSE_TRANSFER_ID = a.DOCUMENT_ID	  
	 WHERE a.DOCUMENT_TYPE_ID <> @PurchaseOrderDocumentTypeId 
	   AND NOT EXISTS (SELECT 1 
						 FROM visualized_automatic_in_process va WITH (NOLOCK)
						WHERE va.AUTOMATIC_IN_PROCESS_ID = a.AUTOMATIC_IN_PROCESS_ID
						  AND (va.EMPLOYEE_ID = @EmployeeId or @EmployeeId IS NULL)) 
	   AND (@SearchKey IS NULL 
	    OR b.NATIONALIZATION  LIKE '%'+@SearchKey+'%'
	    OR REPLACE(CONVERT(VARCHAR(10), b.TRANSFER_DATE, 111),'/','\') LIKE '%'+@SearchKey+'%'
		OR REPLACE(CONVERT(VARCHAR(10), CAST (a.Creation_Date AS date), 111),'/','\') LIKE '%'+@SearchKey+'%')
END
GO


