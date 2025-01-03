USE [Aldebaran]
GO

INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
     VALUES ('9E122CE3-483E-43C0-97AC-D842F14C2259','EC77211D-66F0-41ED-A9CD-1E1353EEFB77','Log de ordenes de compra','LOG DE ORDENES DE COMPRA')
INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
     VALUES ('F0BC59AC-ACE6-405F-AD22-3285D1623578','D0FF8CC2-8B50-409A-AC5B-05C6F476B5DE','Log de pedidos','LOG DE PEDIDOS')
INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
     VALUES ('97BEC0A1-4134-479F-914D-68BFB21FC210','D50B8187-483E-4408-8F2B-8DE079B181AE','Log de reservas','LOG DE RESERVAS')

GO

ALTER   PROCEDURE [dbo].[SP_GET_REFERENCE_MOVEMENT_REPORT]
	@ReferenceIds VARCHAR(MAX) = '',
	@InitialMovementDate DATETIME = null,
	@FinalMovementDate DATETIME = null, 
	@InactiveItems BIT = 0, 
	@InactiveReferences BIT = 0
AS
BEGIN	

	DECLARE @FilterReferences TABLE (ReferenceId INT)

	/* Obtener el filtro de referencias a consultar. si no hay filtro inserta todas las referencias en la tabla temporal */
	IF LEN(RTRIM(@ReferenceIds)) > 0
		INSERT INTO @FilterReferences
			 SELECT value FROM STRING_SPLIT(@ReferenceIds,',')
	ELSE
		/* Si se activo el filtro de Items inactivos solo inserta referencias asociadas a Items Inactivos */
		IF @InactiveItems = 1
			INSERT INTO @FilterReferences
				 SELECT REFERENCE_ID 
				   FROM item_references a
				   JOIN items b ON b.ITEM_ID = a.ITEM_ID
				  WHERE b.IS_ACTIVE = 0
		ELSE
			/* Si se activo el filtro de referencias inactivas solo inserta referencias Inactivas */
			IF @InactiveReferences = 1
				INSERT INTO @FilterReferences
					 SELECT REFERENCE_ID 
					   FROM item_references
					  WHERE IS_ACTIVE = 0
			ELSE
				/* Si no activo ningun filtro de inactivos, solo inserta referencias activas asociads a items Activos */
				INSERT INTO @FilterReferences
					 SELECT REFERENCE_ID 
					   FROM item_references a
					   JOIN items b ON b.ITEM_ID = a.ITEM_ID
					  WHERE b.IS_ACTIVE = 1 
					    AND a.IS_ACTIVE = 1
	
	/* Obtener los movimientos de Ordenes Pedidos, Reservas y Ajustes */

	DECLARE @Movements TABLE
	(	Reference_Id INT, 
		Title_Id SMALLINT,	/* 1 Ajustes de Inventario; 2 Ordenes de Compra; 3 Reservas; 4	Pedidos*/
		Title VARCHAR(150),        
		Code VARCHAR(10),
		Movement_Date DATETIME,
		Movement_Owner VARCHAR(70),
		Movement_Amount INT,
		Movement_Status varchar(30),
		Operator smallint
	)

	INSERT INTO @Movements
		 SELECT b.REFERENCE_ID, 1, 'Ajustes de inventario realizados en el periodo', a.ADJUSTMENT_ID, a.ADJUSTMENT_DATE, c.ADJUSTMENT_TYPE_NAME + ' '+ f.WAREHOUSE_NAME +' por ' + d.ADJUSTMENT_REASON_NAME, b.QUANTITY, e.STATUS_DOCUMENT_TYPE_NAME, c.OPERATOR
		   FROM adjustments a
		   JOIN adjustment_types c ON c.ADJUSTMENT_TYPE_ID = a.ADJUSTMENT_TYPE_ID
		   JOIN adjustment_reasons d ON d.ADJUSTMENT_REASON_ID = a.ADJUSTMENT_REASON_ID
		   JOIN adjustment_details b ON b.ADJUSTMENT_ID = a.ADJUSTMENT_ID
		   JOIN warehouses f ON f.WAREHOUSE_ID = b.WAREHOUSE_ID 
		   JOIN status_document_types e ON e.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		  WHERE (a.ADJUSTMENT_DATE BETWEEN @InitialMovementDate AND @FinalMovementDate OR @InitialMovementDate IS NULL)
		    AND e.STATUS_ORDER = 1
		    AND EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = b.REFERENCE_ID)
	 	  ORDER BY b.REFERENCE_ID, a.ADJUSTMENT_DATE, a.ADJUSTMENT_ID

	INSERT INTO @Movements
		 SELECT b.REFERENCE_ID, 2, 'Traslados entre bodegas realizados en el periodo', a.WAREHOUSE_TRANSFER_ID, a.TRANSFER_DATE, 'De '+c.WAREHOUSE_NAME + ' a ' + d.WAREHOUSE_NAME, b.QUANTITY, e.STATUS_DOCUMENT_TYPE_NAME, 1
		   FROM warehouse_transfers a
		   JOIN warehouse_transfer_details b ON b.WAREHOUSE_TRANSFER_ID = a.WAREHOUSE_TRANSFER_ID
		   JOIN warehouses c ON c.WAREHOUSE_ID = a.ORIGIN_WAREHOUSE_ID
		   JOIN warehouses d ON d.WAREHOUSE_ID = a.DESTINATION_WAREHOUSE_ID
		   JOIN status_document_types e ON e.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		  WHERE (a.TRANSFER_DATE BETWEEN @InitialMovementDate AND @FinalMovementDate OR @InitialMovementDate IS NULL)
		    AND e.STATUS_ORDER = 1 
		    AND EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = b.REFERENCE_ID)
	 	  ORDER BY b.REFERENCE_ID, a.TRANSFER_DATE, a.WAREHOUSE_TRANSFER_ID

  	INSERT INTO @Movements
		 SELECT b.REFERENCE_ID, 3, 'Ordenes de compra realizadas en el periodo', a.ORDER_NUMBER, a.REQUEST_DATE, c.PROVIDER_NAME, CASE WHEN d.STATUS_ORDER = 1 THEN b.REQUESTED_QUANTITY ELSE b.RECEIVED_QUANTITY END, d.STATUS_DOCUMENT_TYPE_NAME, 1
		   FROM purchase_orders a
		   JOIN purchase_order_details b on b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		   JOIN providers c on c.PROVIDER_ID = a.PROVIDER_ID
		   JOIN status_document_types d ON d.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		  WHERE (((a.REQUEST_DATE BETWEEN @InitialMovementDate AND @FinalMovementDate OR @InitialMovementDate IS NULL) AND d.STATUS_ORDER = 1)
				OR
				((a.REAL_RECEIPT_DATE BETWEEN @InitialMovementDate AND @FinalMovementDate OR @InitialMovementDate IS NULL) AND d.STATUS_ORDER = 2))
		    AND EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = b.REFERENCE_ID)
		  ORDER BY b.REFERENCE_ID, a.REQUEST_DATE, a.ORDER_NUMBER

  	INSERT INTO @Movements
		 SELECT b.REFERENCE_ID, 4, 'Reservas realizadas en el periodo', a.RESERVATION_NUMBER, a.RESERVATION_DATE, c.CUSTOMER_NAME, b.RESERVED_QUANTITY, d.STATUS_DOCUMENT_TYPE_NAME, 1
		   FROM customer_reservations a
		   JOIN customer_reservation_details b on b.CUSTOMER_RESERVATION_ID = a.CUSTOMER_RESERVATION_ID
		   JOIN customers c on c.CUSTOMER_ID = a.CUSTOMER_ID
		   JOIN status_document_types d ON d.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		  WHERE (a.RESERVATION_DATE BETWEEN @InitialMovementDate AND @FinalMovementDate OR @InitialMovementDate IS NULL)
		    AND d.STATUS_ORDER IN (1, 2)
		    AND EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = b.REFERENCE_ID)
		  ORDER BY b.REFERENCE_ID, a.RESERVATION_DATE, a.RESERVATION_NUMBER

	INSERT INTO @Movements
		 SELECT b.REFERENCE_ID, 5, 'Pedidos realizados en el periodo', a.ORDER_NUMBER, a.ORDER_DATE, c.CUSTOMER_NAME, b.REQUESTED_QUANTITY, 
				CASE 
					WHEN d.STATUS_ORDER = 5 THEN 'Cerrado' 
					WHEN b.DELIVERED_QUANTITY = b.REQUESTED_QUANTITY THEN 'Totalmente Atendido' 
					WHEN b.DELIVERED_QUANTITY < b.REQUESTED_QUANTITY AND b.DELIVERED_QUANTITY > 0 THEN 'Parcialmente Atendido'
					WHEN b.DELIVERED_QUANTITY = 0 AND b.PROCESSED_QUANTITY > 0 THEN 'En Proceso'
					WHEN b.DELIVERED_QUANTITY = 0 AND b.PROCESSED_QUANTITY = 0 AND d.STATUS_ORDER IN (1,2,3) THEN 'Pendiente'
				END, 1
		   FROM customer_orders a
		   JOIN customer_order_details b on b.CUSTOMER_ORDER_ID = a.CUSTOMER_ORDER_ID
		   JOIN customers c on c.CUSTOMER_ID = a.CUSTOMER_ID
		   JOIN status_document_types d ON d.STATUS_DOCUMENT_TYPE_ID = a.STATUS_DOCUMENT_TYPE_ID
		  WHERE (a.ORDER_DATE BETWEEN @InitialMovementDate AND @FinalMovementDate OR @InitialMovementDate IS NULL)
		    AND d.STATUS_ORDER IN (1, 2, 3, 4, 5)
		    AND EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = b.REFERENCE_ID)
		  ORDER BY b.REFERENCE_ID, a.ORDER_DATE, a.ORDER_NUMBER
		  
	/*  Generacion del Set de datos final */			   
	SELECT c.LINE_ID LineId, c.LINE_CODE LineCode, c.LINE_NAME LineName, b.ITEM_ID ItemId, b.INTERNAL_REFERENCE InternalReference, b.ITEM_NAME ItemName, a.REFERENCE_ID ReferenceId, 
		   a.REFERENCE_CODE ReferenceCode, a.REFERENCE_NAME ReferenceName, a.RESERVED_QUANTITY ReservedQuantity, a.ORDERED_QUANTITY RequestedQuantity, e.WAREHOUSE_ID WarehouseId,
		   e.WAREHOUSE_NAME WarehouseName, d.QUANTITY Amount, Title_Id TitleId, Title,	Code, Movement_Date [Date],  Movement_Owner [Owner], Movement_Amount MovementAmount, Movement_Status [Status], Operator [Operator]
	  FROM item_references a
	  JOIN items b ON b.ITEM_ID = a.ITEM_ID 
	  JOIN lines c ON c.LINE_ID = b.LINE_ID
	  JOIN references_warehouse d ON d.REFERENCE_ID = a.REFERENCE_ID
	  JOIN warehouses e ON e.WAREHOUSE_ID = d.WAREHOUSE_ID
	  LEFT JOIN @Movements m ON m.Reference_Id = a.REFERENCE_ID 
	 WHERE EXISTS (SELECT 1 FROM @FilterReferences fr WHERE fr.ReferenceId = a.REFERENCE_ID)
END 
GO


