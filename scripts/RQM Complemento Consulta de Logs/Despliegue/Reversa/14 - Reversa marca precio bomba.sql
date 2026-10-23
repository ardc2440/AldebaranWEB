/* Reversa T8a: restaura SP_GET_MINIMUM_QUANTITY_ALARMS original y elimina items.IS_BOMB_PRICE.
   Antes de ejecutar, publicar la versión de la aplicación sin la marca Precio Bomba. */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_GET_MINIMUM_QUANTITY_ALARMS]
		@EmployeeId INT,
		@SearchKey VARCHAR(100) = ''
AS
BEGIN 

	DECLARE @Alarms TABLE (AlarmId INT, ReferenceId INT, ArticleName VARCHAR(150) , MinimumQuantity INT, AvailableQuantity INT, InTransitQuantity INT, ReservedQuantity INT, OrderedQuantity INT)
	
	IF LEN(TRIM(@SearchKey)) = 0
		SET @SearchKey = NULL

	IF EXISTS(SELECT 1  
			    FROM alarm_types a
			    JOIN users_alarm_type b ON b.ALARM_TYPE_ID = a.ALARM_TYPE_ID
			   WHERE Name = 'Cantidades mínimas'
			     AND b.EMPLOYEE_ID = @EmployeeId)
		INSERT INTO @Alarms
			 SELECT f.MINIMUM_QUANTITY_ALARM_ID,
					b.REFERENCE_ID, 
		 			CONCAT('[', c.INTERNAL_REFERENCE, '] ', c.ITEM_NAME, ' - ', b.REFERENCE_NAME), 
					b.ALARM_MINIMUM_QUANTITY, 
					b.INVENTORY_QUANTITY, 
					ISNULL(e.REQUESTED_QUANTITY,0),
					b.RESERVED_QUANTITY,
					b.ORDERED_QUANTITY
			   FROM item_references b
			   JOIN items c ON c.ITEM_ID = B.ITEM_ID
			   JOIN lines d ON d.LINE_ID = c.LINE_ID
			   JOIN minimum_quantity_alarms f ON f.REFERENCE_ID = b.REFERENCE_ID
											 AND f.ACTIVE = 1 
			   LEFT JOIN (SELECT pod.REFERENCE_ID, SUM(pod.REQUESTED_QUANTITY) REQUESTED_QUANTITY
						    FROM purchase_order_details pod
						    JOIN purchase_orders po ON po.PURCHASE_ORDER_ID = pod.PURCHASE_ORDER_ID				
						    JOIN status_document_types sdt ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
							 						    AND sdt.STATUS_ORDER IN (1, 4)		-- Corrección: Pendientes + En aprobación
						    JOIN document_types dt ON dt.DOCUMENT_TYPE_ID = sdt.DOCUMENT_TYPE_ID
						                          AND dt.DOCUMENT_TYPE_CODE = 'O'			-- Corrección: solo Órdenes de Compra
							GROUP BY pod.REFERENCE_ID) AS e ON e.REFERENCE_ID = b.REFERENCE_ID 
			  WHERE c.IS_ACTIVE = 1
				AND b.IS_ACTIVE = 1
				AND (@SearchKey is NULL OR 
					d.LINE_CODE like '%'+@SearchKey+'%' OR
					d.LINE_NAME like '%'+@SearchKey+'%' OR
					c.INTERNAL_REFERENCE like '%'+@SearchKey+'%' OR
					c.ITEM_NAME like '%'+@SearchKey+'%' OR
					b.REFERENCE_CODE like '%'+@SearchKey+'%' OR
					b.REFERENCE_NAME like '%'+@SearchKey+'%')
				AND NOT EXISTS(SELECT 1 
								 FROM visualized_minimum_quantity_alarms v
								WHERE v.MINIMUM_QUANTITY_ALARM_ID = f.MINIMUM_QUANTITY_ALARM_ID
								  AND v.EMPLOYEE_ID = @EmployeeId)			  

		SELECT AlarmId, ReferenceId, ArticleName, MinimumQuantity, AvailableQuantity, InTransitQuantity, ReservedQuantity, OrderedQuantity  
		  FROM @Alarms

END
GO

DELETE FROM dbo.LOG_TRACK_FIELD_NAMES WHERE FIELD_NAME = 'IsBombPrice';
GO

IF COL_LENGTH('dbo.items', 'IS_BOMB_PRICE') IS NOT NULL
BEGIN
    ALTER TABLE dbo.items DROP CONSTRAINT DF_ITEMS_IS_BOMB_PRICE;
    ALTER TABLE dbo.items DROP COLUMN IS_BOMB_PRICE;
END
GO
