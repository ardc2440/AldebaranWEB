USE Aldebaran
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PURCHASE_ORDER_VARIATION' AND object_id = OBJECT_ID('ITEM_REFERENCES')) 
	ALTER TABLE [item_references] DROP COLUMN PURCHASE_ORDER_VARIATION 
GO

ALTER TABLE [item_references] ADD PURCHASE_ORDER_VARIATION INT NOT NULL CONSTRAINT DF_PURCHASE_ORDER_VARIATION DEFAULT 0
GO

CREATE OR ALTER PROCEDURE SP_IS_VALID_PURCHASE_ORDER_VARIATION
	@PROVIDER_ID INT, 
	@REFERENCE_ID INT, 
	@QUANTITY INT, 
	@PURCHASE_ORDER_ID INT = -1, 
	@IS_VALID_VARIATION BIT OUT 
AS
BEGIN 
	DECLARE @Variation INT
	DECLARE @Average INT
	   
	SET @IS_VALID_VARIATION = 1

	SELECT @Variation = PURCHASE_ORDER_VARIATION 
	  FROM item_references 
	 WHERE REFERENCE_ID = @REFERENCE_ID
	
	/*No tiene configurada validacion de orden de compra*/
	if @Variation <= 0 
		RETURN 
	
	/* Validar si el proveedor tiene ordenes con la misma referencia */
	SELECT @Average = AVG(a.REQUESTED_QUANTITY) FROM purchase_order_details a 
	  JOIN purchase_orders b on b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
	  JOIN status_document_types c on c.STATUS_DOCUMENT_TYPE_ID = b.STATUS_DOCUMENT_TYPE_ID 
								  AND c.STATUS_ORDER IN (1,2)	
     WHERE a.PURCHASE_ORDER_ID <> @PURCHASE_ORDER_ID
	   AND a.REFERENCE_ID = @REFERENCE_ID
	   AND b.PROVIDER_ID = @PROVIDER_ID
	
	IF @Average = 0 
		RETURN 

	IF (@QUANTITY BETWEEN FLOOR(@Average - (@Average*@Variation)/100) AND FLOOR(@Average + (@Average*@Variation)/100)+1)
		RETURN 
	
	--SELECT @QUANTITY, @Variation, @Average, (@Average*@Variation)/100, FLOOR(@Average - (@Average*@Variation)/100), FLOOR(@Average + (@Average*@Variation)/100)+1

	SET @IS_VALID_VARIATION = 0
END 
GO
