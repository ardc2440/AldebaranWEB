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
	@VARIATIONMONTHNUMBER INT,
	@PURCHASE_ORDER_ID INT = -1 
AS
BEGIN 
	DECLARE @Variation INT = 0
	DECLARE @Average INT = 0
	DECLARE @Is_Valid_Variation bit = 1   

	SELECT @Variation = PURCHASE_ORDER_VARIATION 
	  FROM item_references 
	 WHERE REFERENCE_ID = @REFERENCE_ID
	
	/*No tiene configurada validacion de orden de compra*/
	if @Variation > 0 
	BEGIN	
		/* Validar si el proveedor tiene ordenes con la misma referencia */
		SELECT @Average = AVG(a.REQUESTED_QUANTITY) FROM purchase_order_details a 
		  JOIN purchase_orders b on b.PURCHASE_ORDER_ID = a.PURCHASE_ORDER_ID
		  JOIN status_document_types c on c.STATUS_DOCUMENT_TYPE_ID = b.STATUS_DOCUMENT_TYPE_ID 
									  AND c.STATUS_ORDER IN (1,2)	
		 WHERE a.PURCHASE_ORDER_ID <> @PURCHASE_ORDER_ID
		   AND a.REFERENCE_ID = @REFERENCE_ID
		   AND b.PROVIDER_ID = @PROVIDER_ID
		   AND b.REQUEST_DATE >= DATEADD(MONTH,-@VARIATIONMONTHNUMBER,GETDATE())
		   			
		IF NOT(@QUANTITY BETWEEN FLOOR(@Average - (@Average*@Variation)/100) AND FLOOR(@Average + (@Average*@Variation)/100))
			SET @Is_Valid_Variation = 0
	END 

	SELECT @Is_Valid_Variation IS_VALID_VARIATION, @Average AVERAGE, FLOOR(@Average - (@Average*@Variation)/100) MINIMUM_RANGE, FLOOR(@Average + (@Average*@Variation)/100) MAXIMUM_RANGE
END 
GO

CREATE OR ALTER PROCEDURE SP_GET_NOTFICATIONS_WITH_SEND_ERROR
	@SEARCHKEY VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @Notifications TABLE (
		EMAIL_TYPE SMALLINT,
		EMAIL_ID INT,
		Description VARCHAR(MAX), 
		CUSTOMER_NAME VARCHAR(50),
		Reason VARCHAR(100),	
		NOTIFIED_MAIL_LIST	VARCHAR(MAX),
		NOTIFICATION_DATE DATETIME,
		NOTIFICATION_SENDING_ERROR_MESSAGE VARCHAR(MAX))

	INSERT INTO @Notifications
		SELECT 0, a.PURCHASE_ORDER_NOTIFICATION_ID, 'Notificación para el pedido no. '+d.ORDER_NUMBER+' del '+CONVERT(varchar,d.ORDER_DATE,3) + ' por modificación de la orden de compra No. '+
			   c.ORDER_NUMBER Description, f.CUSTOMER_NAME, e.MODIFICATION_REASON_NAME Reason, a.NOTIFIED_MAIL_LIST, a.NOTIFICATION_DATE, a.NOTIFICATION_SENDING_ERROR_MESSAGE
		  FROM purchase_order_notifications a 
		  JOIN modified_purchase_orders b ON b.MODIFIED_PURCHASE_ORDER_ID = a.MODIFIED_PURCHASE_ORDER_ID
		  JOIN purchase_orders c ON c.PURCHASE_ORDER_ID = b.PURCHASE_ORDER_ID
		  JOIN customer_orders d ON d.CUSTOMER_ORDER_ID = a.CUSTOMER_ORDER_ID  
		  JOIN modification_reasons e ON e.MODIFICATION_REASON_ID = b.MODIFICATION_REASON_ID
		  JOIN customers f ON f.CUSTOMER_ID = d.CUSTOMER_ID
		 WHERE NOTIFICATION_STATE = -1
		UNION 
		SELECT 1, a.CUSTOMER_ORDER_NOTIFICATION_ID, 'Notificación para el pedido no. '+b.ORDER_NUMBER+' del '+CONVERT(varchar,b.ORDER_DATE,3) Description, 
			   c.CUSTOMER_NAME, d.SUBJECT Reason, a.NOTIFIED_MAIL_LIST, a.NOTIFICATION_DATE, a.NOTIFICATION_SENDING_ERROR_MESSAGE
		  FROM customer_order_notifications a
		  JOIN customer_orders b ON b.CUSTOMER_ORDER_ID = a.CUSTOMER_ORDER_ID
		  JOIN customers c ON c.CUSTOMER_ID = b.CUSTOMER_ID
		  JOIN notification_templates d ON d.NOTIFICATION_TEMPLATE_ID = a.NOTIFICATION_TEMPLATE_ID
		 WHERE NOTIFICATION_STATE = -1
		UNION
		SELECT 2, a.CUSTOMER_RESERVATION_NOTIFICATION_ID, 'Notificación para la reserva no. '+b.RESERVATION_NUMBER+' del '+CONVERT(varchar,b.RESERVATION_DATE,3) Description, 
			   c.CUSTOMER_NAME, d.SUBJECT Reason, a.NOTIFIED_MAIL_LIST, a.NOTIFICATION_DATE, a.NOTIFICATION_SENDING_ERROR_MESSAGE
		  FROM customer_reservation_notifications a
		  JOIN customer_reservations b ON b.CUSTOMER_RESERVATION_ID = a.CUSTOMER_RESERVATION_ID
		  JOIN customers c ON c.CUSTOMER_ID = b.CUSTOMER_ID
		  JOIN notification_templates d ON d.NOTIFICATION_TEMPLATE_ID = a.NOTIFICATION_TEMPLATE_ID
		 WHERE NOTIFICATION_STATE = -1

	SELECT * 
	  FROM @Notifications
	 WHERE @SearchKey IS NULL 
	    OR Description LIKE '%'+@SearchKey+'%' 
		OR CUSTOMER_NAME LIKE '%'+@SearchKey+'%' 
		OR Reason LIKE '%'+@SearchKey+'%' 
		OR NOTIFIED_MAIL_LIST LIKE '%'+@SearchKey+'%' 
		OR CONVERT(VARCHAR, NOTIFICATION_DATE, 3) LIKE '%'+@SearchKey+'%' 
		OR NOTIFICATION_SENDING_ERROR_MESSAGE LIKE '%'+@SearchKey+'%' 
 
END 
GO

DELETE FROM AspNetRoles WHERE Id=N'D9326438-F865-407F-AECB-88835E5684DF'
DELETE FROM AspNetRoles WHERE Id=N'6237D7AE-B500-4BC3-A3F0-C481C39C7537'
DELETE FROM AspNetRoles WHERE Id=N'CEFCF7BA-8051-4834-A71E-9EDF6D00994F'
DELETE FROM AspNetRoles WHERE Id=N'C04E9671-031F-412D-90C1-39FC449475C0'
DELETE FROM AspNetRoles WHERE Id=N'CF36761A-287D-4510-BAD5-D17073C125EA'

INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'91691696-9306-4B13-8092-87860C032D73', N'49F3AD59-8050-45B1-AE01-232C3FA00E0A', N'Consulta de notificaciones por cantidades mínimas', N'CONSULTA DE NOTIFICACIONES POR CANTIDADES MÍNIMAS')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'B8986382-DACC-4E65-A20A-726312BFB656', N'00950DDF-FD40-46B0-9EA0-712F6F091C25', N'Consulta de notificaciones por artículos sin disponible', N'CONSULTA DE NOTIFICACIONES POR ARTÍCULOS SIN DISPONIBLE')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'F03670C4-F79C-405E-AE7B-6FAD489668BE', N'1D3D399D-7DDE-446A-A2DA-3C7DE2C00FC6', N'Consulta de notificaciones por reservas vencidas', N'CONSULTA DE NOTIFICACIONES POR RESERVAS VENCIDAS')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'444E388D-A9FA-46E8-A3C9-FE14ABC05DC2', N'FE761A84-68D7-4D95-8515-572D43F1254D', N'Consulta de notificaciones por alarmas del día', N'CONSULTA DE NOTIFICACIONES POR ALARMAS DEL DÍA')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'C3B6C037-DF62-467A-B4D3-9B374104DC66', N'50FE4921-1F22-465B-999F-BB9DF5DCDA71', N'Consulta de notificaciones por alarmas de órdenes modificadas con afectación en pedido', N'CONSULTA DE NOTIFICACIONES POR ALARMAS DE ÓRDENES MODIFICADAS CON AFECTACIÓN EN PEDIDO')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'8DC9A444-A36A-447E-BF36-EF17B576E45D', N'6D765C29-3580-4BA4-AB6C-A5F7EA2A3DBF', N'Consulta de notificaciones por órdenes próximas a su vencimiento', N'CONSULTA DE NOTIFICACIONES POR ÓRDENES PRÓXIMAS A SU VENCIMIENTO')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'B370C85A-8407-478B-8CAC-9D8DE2E6F171', N'6870FA88-034A-4E88-98BA-3F45C5E08872', N'Consulta de notificaciones por pedidos vencidos', N'CONSULTA DE NOTIFICACIONES POR PEDIDOS VENCIDOS')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'5573C87B-1F0A-49EE-8125-CFAFFB940D88', N'05953D91-D898-4647-82AA-2EEFCE9F7AFB', N'Consulta de notificaciones por envio de correo con error', N'CONSULTA DE NOTIFICACIONES POR ENVIO DE CORREO CON ERROR')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'3D78286C-E66D-4336-AE3C-5366CEAFE790', N'13CD8934-6C97-41C3-984D-7A8BDC82786B', N'Creación de órdenes de compra','CREACIÓN DE ÓRDENES DE COMPRA')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'9B10B24F-70F3-41C7-9FEE-32113289C774', N'1A3E4DFA-8DE9-4AD1-B9C7-62C6549314B3', N'Modificación de órdenes de compra','MODIFICACIÓN DE ÓRDENES DE COMPRA')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'9E6647C7-D68F-40CA-97C6-76F94F082C10', N'1A3E4DFA-8DE9-4AD1-B9C7-62C6549314B3', N'Confirmación de órdenes de compra','CONFIRMACIÓN DE ÓRDENES DE COMPRA')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'65C45C3F-0005-4133-9C7C-6FC96AB52EA1', N'1A3E4DFA-8DE9-4AD1-B9C7-62C6549314B3', N'Cancelación de órdenes de compra','CANCELACIÓN DE ÓRDENES DE COMPRA')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'3E55B995-B331-4911-A850-01E165ED23AC', N'7E4F8093-DDBF-423C-A224-53D4DD78B77B', N'Modificación de pedidos','MODIFICACIÓN DE PEDIDOS')  
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'22009B3D-D6CC-4BF1-A3CD-344A5E282032', N'68603CF6-63D5-4C82-9768-695A5D0E4C1F', N'Creación de pedidos','CREACIÓN DE PEDIDOS')  
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'A6ABDEE5-DE7A-43AC-AD4F-08CCB80A7A15', N'A6ABDEE5-DE7A-43AC-AD4F-08CCB80A7A15', N'Cierre de pedidos','CIERRE DE PEDIDOS')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'AB0D1257-B491-45C0-9DCA-0EFF6DD56DD2', N'68355298-258D-42BF-88EF-DC259365DB50', N'Cancelción de pedidos','CANCELACIÓN DE PEDIDOS')
INSERT [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'302D116A-7596-4500-A8A1-5D9357646196', N'CC5AEFB7-3373-47C4-BF6B-2A2B7A0570DD', N'Descarga de formato de pedidos','DESCARGA DE FORMATO DE PEDIDOS')
GO
/*
SELECT NEWID()
SELECT NEWID()
*/