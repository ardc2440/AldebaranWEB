--CREATE OR ALTER     PROCEDURE SP_Create_Pending_Operational_Request_Notification
--AS
BEGIN 
	SET NOCOUNT ON

	DECLARE @Pending_Cancellation_Request TABLE (DOCUMENT_TYPE_CODE CHAR(1), DOCUMENT_TYPE_NAME VARCHAR(30), DOCUMENT_NUMBER INT)

	INSERT INTO @Pending_Cancellation_Request
		 SELECT c.DOCUMENT_TYPE_CODE, c.DOCUMENT_TYPE_NAME, DOCUMENT_NUMBER 
		   FROM cancellation_requests a	
		   JOIN status_document_types b on b.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		   JOIN document_types c on c.DOCUMENT_TYPE_ID = a.DOCUMENT_TYPE_ID
		  WHERE b.STATUS_ORDER = 2
    
	

	IF NOT EXISTS(SELECT 1 FROM @references)
		return

	DECLARE @transit TABLE (REFERENCE_ID INT NOT NULL PRIMARY KEY, REQUESTED_QUANTITY INT NOT NULL)

	INSERT INTO @transit
	 	 SELECT a.REFERENCE_ID, SUM(a.REQUESTED_QUANTITY) 
		   FROM purchase_order_details a WITH (NOLOCK)
		   JOIN purchase_orders b WITH (NOLOCK) ON b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		   JOIN status_document_types c WITH (NOLOCK) ON c.STATUS_DOCUMENT_TYPE_ID = b.STATUS_DOCUMENT_TYPE_ID
		 											 AND c.STATUS_ORDER = 1 	
		  WHERE EXISTS (SELECT 1 FROM @references d WHERE d.REFERENCE_ID = a.REFERENCE_ID)
	      GROUP BY a.REFERENCE_ID
		  
	DECLARE @ReferencesForNotification TABLE (RegNo INT NOT NULL IDENTITY(1,1), INTERNAL_REFERENCE VARCHAR(10), ITEM_NAME VARCHAR(50), REFERENCE_NAME VARCHAR(50), 
											  LOCAL_WAREHOUSE_QUANTITY INT NOT NULL, ZONA_FRANCA_WAREHOUSE_QUANTITY INT NOT NULL, ORDERED_QUANTITY INT NOT NULL, 
											  REQUESTED_QUANTITY INT NOT NULL)

	INSERT INTO @ReferencesForNotification (INTERNAL_REFERENCE, ITEM_NAME, REFERENCE_NAME, LOCAL_WAREHOUSE_QUANTITY, ZONA_FRANCA_WAREHOUSE_QUANTITY, ORDERED_QUANTITY, REQUESTED_QUANTITY)
		 SELECT f.INTERNAL_REFERENCE, f.ITEM_NAME, a.REFERENCE_NAME, b.QUANTITY, c.QUANTITY, a.ORDERED_QUANTITY, ISNULL(h.REQUESTED_QUANTITY,0) 
		   FROM @references a
		   JOIN references_warehouse b WITH (NOLOCK) ON b.REFERENCE_ID = a.REFERENCE_ID
		   JOIN references_warehouse c WITH (NOLOCK) ON c.REFERENCE_ID = a.REFERENCE_ID
		   JOIN warehouses d WITH (NOLOCK) ON d.WAREHOUSE_ID = b.WAREHOUSE_ID and d.WAREHOUSE_CODE = '1' 
		   JOIN warehouses e WITH (NOLOCK) ON e.WAREHOUSE_ID = c.WAREHOUSE_ID and e.WAREHOUSE_CODE = '2'	  
		   JOIN items f WITH (NOLOCK) ON f.ITEM_ID = a.item_id 
		   LEFT JOIN @transit h ON h.REFERENCE_ID = a.REFERENCE_ID

	IF EXISTS (SELECT 1 FROM @ReferencesForNotification)
	BEGIN 
		DECLARE @StyleTitle NVARCHAR(MAX) = 'border: 1px solid black; font-size:13px;', @StyleCell NVARCHAR(MAX) =  'border: 1px solid black;', @RegNo INT, @TotalReg INT, 
				@INTERNAL_REFERENCE VARCHAR(10), @ITEM_NAME VARCHAR(50), @REFERENCE_NAME VARCHAR(50), @LOCAL_WAREHOUSE_QUANTITY INT, @ZONA_FRANCA_WAREHOUSE_QUANTITY INT, @ORDERED_QUANTITY INT, 
				@REQUESTED_QUANTITY INT, @ReferencesDetail NVARCHAR(MAX)='', @ConsolidadoLineas NVARCHAR(MAX)= '', @LineaMalla NVARCHAR(MAX)= ''

		DECLARE @Head NVARCHAR(MAX) = N'<!DOCTYPE html> <html lang="en"> <head> <meta charset="UTF-8"> <meta name="viewport" content="width=device-width, initial-scale=1.0"> '+
									   '<title>Cantidad Mínima en Bodega Local</title> <style> body {margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4; } '+
									   '.container { width: 100%; max-width: 900px; margin: 0 auto; background-color: #ffffff; border: 1px solid #dddddd; border-radius: 5px; overflow: hidden; } '+
									   '.header { background-color: #f7620b; color: white; padding: 5px; text-align: center; } .body { padding: 20px; line-height: 1.6; color: #333333; } '+
									   '.footer { background-color: #f4f4f4; text-align: center; padding: 10px; font-size: 12px; color: #777777; } /*.th-width { width: calc(70/7%);}*/ </style> </head>'

		DECLARE @Body NVARCHAR(MAX) = N'<body> <div class="container"> <div class="header"><h3>PEDIDO DE REFERENCIAS SIN CANTIDAD MÍNIMA CONFIGURADA PARA BODEGA LOCAL</h3></div><div class="body"> '+
									   '<h4>Hola, </h4><p>Le informamos que se han descargado algunas referencias dentro del pedido <strong>No. {OrderNumber}</strong>, las cuales no tienen configurada la cantidad mínima para bodega local. </p> '+
									   '<p>A continuación, encontrará la información relevante de dichas referencias para su guia.</p><h4>Detalle de las referencias</h4>{ReferencesDetail}<p>'+
									   'Le solicitamos que revise y valide estas referencias para completar su configuración, de forma que se pueda controlar correctamente el inventario de las mismas.</p>'+
									   '<p>Quedamos a su disposición para cualquier duda o aclaración.</p><br><br><p>Atentamente,</p><p>IT, Promos</p></div></div></body></html>'
	
		DECLARE @Titulos NVARCHAR(MAX) = N'<tr><th class="th-width" align="center" style="'+@StyleTitle+'; width: 400px">Artículo</th>'+ /*[INTERNAL_REFERENCE] ITEM_NAME - REFERENCE_NAME  */
										  '	   <th class="th-width" align="center" style="'+@StyleTitle+'; width: 100px">Bodega local</th>'+
										  '	   <th class="th-width" align="center" style="'+@StyleTitle+'; width: 100px">Zona franca</th>'+
										  '	   <th class="th-width" align="center" style="'+@StyleTitle+'; width: 100px">Total pedido </th>'+
										  '	   <th class="th-width" align="center" style="'+@StyleTitle+'; width: 100px">Total tránsito</th>'+
										  '</tr>'
		
		SELECT @RegNo = MIN(RegNo), @TotalReg = MAX(RegNo) 
		  FROM @ReferencesForNotification
		
		WHILE @RegNo <= @TotalReg 
		BEGIN
			SELECT @INTERNAL_REFERENCE = INTERNAL_REFERENCE, @ITEM_NAME = ITEM_NAME, @REFERENCE_NAME = REFERENCE_NAME, @LOCAL_WAREHOUSE_QUANTITY = LOCAL_WAREHOUSE_QUANTITY, 
				   @ZONA_FRANCA_WAREHOUSE_QUANTITY = ZONA_FRANCA_WAREHOUSE_QUANTITY, @ORDERED_QUANTITY = ORDERED_QUANTITY, @REQUESTED_QUANTITY = REQUESTED_QUANTITY
			  FROM @ReferencesForNotification
			 WHERE RegNo = @RegNo

			SET @LineaMalla = N'<td align="center" style="'+@StyleCell+' font-size:12px;">'+TRIM('['+@INTERNAL_REFERENCE+'] '+@ITEM_NAME+' - '+@REFERENCE_NAME)+'</td>'+ 
							   '<td align="center" style="'+@StyleCell+'">'+TRIM(FORMAT(@LOCAL_WAREHOUSE_QUANTITY,'N0'))+'</td>'+ 
							   '<td align="center" style="'+@StyleCell+'">'+TRIM(FORMAT(@ZONA_FRANCA_WAREHOUSE_QUANTITY,'N0'))+'</td>'+
							   '<td align="center" style="'+@StyleCell+'">'+TRIM(FORMAT(@ORDERED_QUANTITY,'N0'))+'</td>'+ 
							   '<td align="center" style="'+@StyleCell+'">'+TRIM(FORMAT(@REQUESTED_QUANTITY,'N0'))+'</td>' 

			SET @ConsolidadoLineas += N'<tr>'+@LineaMalla+'</tr>'

			SET @RegNo += 1;
		END 	
		
		SET @ReferencesDetail = N'<table style="border-collapse: collapse; margin: auto;">'+@Titulos+@ConsolidadoLineas+'</table>'
		
		SET @Body = REPLACE(@Body,'{OrderNumber}',@OrderNumber)
		SET @Body = REPLACE(@Body,'{ReferencesDetail}',@ReferencesDetail)

		DECLARE @FinalBody NVARCHAR(MAX)

		SET @FinalBody = CONCAT(@Head, @Body)	

		--select @FinalBody
	END

	EXEC SP_Send_Pending_Operational_Request_Notification @FinalBody 
END 
