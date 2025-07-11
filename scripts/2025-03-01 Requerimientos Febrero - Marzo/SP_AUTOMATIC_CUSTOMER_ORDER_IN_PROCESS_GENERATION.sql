CREATE OR ALTER PROCEDURE [dbo].[SP_AUTOMATIC_CUSTOMER_ORDER_IN_PROCESS_GENERATION]
	@DocumentType CHAR(1),
	@DocumentId INT 	
AS
BEGIN
	/* Declaracion de Tablas Temporales para el proceso */
	DECLARE @References TABLE (ReferenceId INT, WarehouseId INT, Quantity INT, Assigned INT, Available AS (Quantity-Assigned))
	DECLARE @CustomerOrderDetails TABLE (CustomerOrderId INT, CreationDate DATETIME, CustomerOrderDetailId INT, ReferenceId INT, Pending INT, Brand VARCHAR(250))
	DECLARE @CustomerOrders TABLE (OrderNo INT NOT NULL IDENTITY(1,1), CustomerOrderId INT)
	DECLARE @Details TABLE (DetailNo INT NOT NULL IDENTITY(1,1), CustomerOrderDetailId INT, ReferenceId INT, Pending INT, Assigned INT, NewPending AS (Pending-Assigned), Brand VARCHAR(250))
	DECLARE @Available TABLE (AvailableNo INT NOT NULL IDENTITY(1,1), WarehouseId INT, Quantity INT)
				
	/* Declaracion de variables */
	DECLARE @OrderNo INT, @TotalOrders INT, @CustomerOrderId INT, @DocumentNumber VARCHAR(10), @EmployeeId INT, @DetailNo INT, @TotalDetails INT, @NewStatus INT,
	        @ProcessSatelliteId INT, @StatusDocumentTypeId INT, @DocumentTypeId INT, @CustomerOrderinProcessId INT, @CustomerOrderDetailId INT, 
			@ReferenceId INT, @Pending INT, @AvailableNo INT, @TotalAvailable INT, @AvailableQuantity INT = 0, @WarehouseId INT = 0, @Brand VARCHAR(250), @AutomaticInProcessId INT = NULL,
			@LocalWarehouseId INT 

	/* Obtener tipo de documento para traslados */
	SELECT @DocumentTypeId = DOCUMENT_TYPE_ID
	  FROM document_types WITH (NOLOCK)
	 WHERE DOCUMENT_TYPE_CODE = 'T'

	/* Obtener estado de traslado realizado */  
	SELECT @StatusDocumentTypeId = STATUS_DOCUMENT_TYPE_ID
	  FROM status_document_types WITH (NOLOCK)
	 WHERE DOCUMENT_TYPE_ID = @DocumentTypeId
	   AND STATUS_ORDER = 1

	/* Obtener texto para las notas del Traslado */
	SELECT @DocumentNumber = 'traslado entre bodegas No. '+CAST(@DocumentId AS VARCHAR(10))
		
	IF (@DocumentType = 'P')
		SELECT @DocumentNumber = 'confirmación de orden de compra No. '+ORDER_NUMBER 
		  FROM purchase_orders WITH (NOLOCK)
		 WHERE PURCHASE_ORDER_ID = @DocumentId
	
	
	/* Obtenere Empleado para soporte de movimientos de Traslado a proceso */	
	SELECT @EmployeeId = EMPLOYEE_ID
	  FROM employees WITH (NOLOCK)
	 WHERE IDENTITY_NUMBER = '000000001' /*Asignación Automática de Ordenes Confirmadas*/	

	/* Obtener Satelite temporal de asignacion para el traslado a proceso */
	SELECT @ProcessSatelliteId = PROCESS_SATELLITE_ID 
	  FROM process_satellites WITH (NOLOCK)
     WHERE PROCESS_SATELLITE_NAME = 'Creación Automática de Traslados a Proceso'	

	/* Obtener el Id de la Bodega Local */
	SELECT @LocalWarehouseId = WAREHOUSE_ID 
	  FROM Warehouses 
	 WHERE Warehouse_Code = '1'
	 
	IF (@DocumentType = 'O')
		/* Obtener referencias incluidas en la orden de compra Que tengan como llegada la bodega Local */
		INSERT INTO @References
			 SELECT REFERENCE_ID, WAREHOUSE_ID, RECEIVED_QUANTITY, 0 
			   FROM purchase_order_details WITH (NOLOCK)
			  WHERE PURCHASE_ORDER_ID = @DocumentId
				AND RECEIVED_QUANTITY > 0 
				AND WAREHOUSE_ID = @LocalWarehouseId
	ELSE
		/* Obtener referencias incluidas en el traslado entre bodegas */
		INSERT INTO @References
			 SELECT REFERENCE_ID, @LocalWarehouseId, QUANTITY, 0 
			   FROM warehouse_transfer_details
			  WHERE WAREHOUSE_TRANSFER_ID = @DocumentId 

	/* Obtener pedidos que tienene pendientes de las referencias incluidas en la orden de compra */	
	INSERT INTO @CustomerOrderDetails (CustomerOrderId, CreationDate, CustomerOrderDetailId, ReferenceId, Pending)
		 SELECT b.CUSTOMER_ORDER_ID, c.CREATION_DATE, b.CUSTOMER_ORDER_DETAIL_ID, b.REFERENCE_ID, b.REQUESTED_QUANTITY - b.PROCESSED_QUANTITY - b.DELIVERED_QUANTITY Pending  
		   FROM customer_order_details b WITH (NOLOCK)
		   JOIN customer_orders c WITH (NOLOCK) ON c.CUSTOMER_ORDER_ID = b.CUSTOMER_ORDER_ID				     
		   JOIN status_document_types d WITH (NOLOCK) ON d.STATUS_DOCUMENT_TYPE_ID = c.STATUS_DOCUMENT_TYPE_ID
													 AND d.STATUS_ORDER IN (1,2,3)	
		  WHERE b.REQUESTED_QUANTITY -  b.PROCESSED_QUANTITY - b.DELIVERED_QUANTITY >0 
		    AND EXISTS (SELECT 1 FROM @References ref WHERE ref.ReferenceId = b.REFERENCE_ID)
		  ORDER BY c.CREATION_DATE ASC

	IF NOT EXISTS (SELECT 1 FROM @CustomerOrderDetails)
		RETURN

	/* Obtener las diferentes ordenes de compra a las que se les debe crear un traslado a proceso */	
	INSERT INTO @CustomerOrders (CustomerOrderId)
		 SELECT DISTINCT CustomerOrderId
		   FROM @CustomerOrderDetails		 
		  
	SELECT @OrderNo = MIN(OrderNo), @TotalOrders = MAX(OrderNo)
	  FROM @CustomerOrders	

	/* Por cada orden de compra se crea un traslado a proceso */
	WHILE @OrderNo <= @TotalOrders 
	BEGIN
		SELECT @CustomerOrderId  = NULL, @DetailNo = 1, @TotalDetails = 0 

		SELECT @CustomerOrderId = CustomerOrderId
		  FROM @CustomerOrders
		 WHERE OrderNo = @OrderNo
		 
		DELETE FROM @Details 

		SET @OrderNo += 1

		IF (@CustomerOrderId IS NULL) 
			CONTINUE 

		/* Obtener el detalle del pedido que será atendido con el traslado a proceso */
		INSERT INTO @Details (CustomerOrderDetailId, ReferenceId, Pending, Assigned, Brand)
			 SELECT CustomerOrderDetailId, ReferenceId, Pending, 0, Brand 
			   FROM @CustomerOrderDetails
			  WHERE CustomerOrderId = @CustomerOrderId 
		
		/* Se valida si en @references, aún hay disponible para atender algún item del pedido actual */
		IF NOT EXISTS (SELECT 1 FROM @Details a WHERE EXISTS (SELECT 1 FROM @References b WHERE b.ReferenceId = a.ReferenceId AND b.Available > 0))
			CONTINUE
		
		/* Crear el encabezado del traslado a proceso */
		INSERT INTO customer_orders_in_process (CUSTOMER_ORDER_ID, PROCESS_DATE, NOTES, 
											    PROCESS_SATELLITE_ID, EMPLOYEE_RECIPIENT_ID, 
												TRANSFER_DATETIME, CREATION_DATE, STATUS_DOCUMENT_TYPE_ID, EMPLOYEE_ID)
			 VALUES(@CustomerOrderId, GETDATE(), 'Asignación automática de inventario por '+ @DocumentNumber, 
					@ProcessSatelliteId, @EmployeeId, 
					GETDATE(), GETDATE(), @StatusDocumentTypeId, @EmployeeId)
		
		/* Se obtiene el ultimo encabezado de traslado a proceso creado, para asociar ssus detalles */
		SET @CustomerOrderinProcessId = SCOPE_IDENTITY()

		SELECT @NewStatus = STATUS_DOCUMENT_TYPE_ID 
		  FROM status_document_types a WITH (NOLOCK) 
		  JOIN document_types b WITH (NOLOCK) on b.DOCUMENT_TYPE_ID = a.DOCUMENT_TYPE_ID
		 WHERE DOCUMENT_TYPE_CODE = 'P'
		   AND STATUS_ORDER = 2

		UPDATE a
		   SET STATUS_DOCUMENT_TYPE_ID = @NewStatus 
		  FROM customer_orders a WITH (NOLOCK) 
		  JOIN status_document_types b WITH (NOLOCK) ON b.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
													AND b.STATUS_ORDER = 1		   
		 WHERE CUSTOMER_ORDER_ID = @CustomerOrderId
		   		
		SELECT @DetailNo = MIN(DetailNo), @TotalDetails = MAX(DetailNo)
		  FROM @Details 
		  
		/* procesar los detalles que se pueda */
		WHILE @DetailNo <= @TotalDetails 
		BEGIN 
			SELECT @CustomerOrderDetailId = NULL, @ReferenceId = NULL, @Pending = NULL, @Brand = NULL
			
			/* Obtener el detalle a procesar */
			SELECT @CustomerOrderDetailId = CustomerOrderDetailId, @ReferenceId = ReferenceId, @Pending = NewPending, @Brand = Brand 
			  FROM @Details	
			 WHERE DetailNo = @DetailNo
	
			DELETE FROM @Available

			SET @DetailNo += 1

			IF @CustomerOrderDetailId IS NULL 
				CONTINUE

			/* Obtener el disponible de esa referencia */
			INSERT INTO @Available (WarehouseId, Quantity)
				 SELECT WarehouseId, Available
				   FROM @References
				  WHERE ReferenceId = @ReferenceId 
				    AND Available >0 
				  ORDER BY Available
			
			SELECT @AvailableNo = MIN(AvailableNo), @TotalAvailable = MAX(AvailableNo)
 			  FROM @Available

			/* Recorrer el disponible hasta que se acabe o hasta que se cubra el pendiente del traslado */
			WHILE @AvailableNo <= @TotalAvailable AND @Pending > 0  
			BEGIN 
				
				SELECT @AvailableQuantity = NULL, @WarehouseId = NULL

				SELECT @AvailableQuantity = Quantity, @WarehouseId = WarehouseId
				  FROM @Available 
				 WHERE AvailableNo = @AvailableNo
				
				SET @AvailableNo += 1

				IF ISNULL(@AvailableQuantity, 0) = 0
					CONTINUE

				/* Si existe el disponible para atender algo del pedido pendiente de traslado */

				DECLARE @QuantityToAssign INT = CASE WHEN @Pending >= @AvailableQuantity THEN @AvailableQuantity ELSE @Pending END

				--SELECT @CustomerOrderinProcessId, @QuantityToAssign, @AvailableQuantity, @WarehouseId, @ReferenceId, @Pending, @CustomerOrderDetailId, @CustomerOrderId, @Brand

				EXEC SP_AUTOMATIC_CUSTOMER_ORDER_IN_PROCESS_DETAIL_GENERATION @CustomerOrderinProcessId, @QuantityToAssign, @WarehouseId, @CustomerOrderDetailId, @ReferenceId, @Brand
				
				UPDATE @References
				   SET Assigned = @QuantityToAssign
				 WHERE ReferenceId = @ReferenceId 
				   AND WarehouseId = @WarehouseId

			END 			
		END 		
	
		/* Despues de finalizado el Pedido, se crea el log de la Order in Process Generada, para que pueda ser consultada en las alarmas y en las consultas */
		EXEC SP_AUTOMATIC_IN_PROCESS_REGISTRY @AutomaticInProcessId, @DocumentType, @DocumentId, @CustomerOrderId, @CustomerOrderinProcessId, @AutomaticInProcessId OUT
	END
END
