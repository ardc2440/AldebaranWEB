INSERT INTO employees (AREA_ID, IDENTITY_TYPE_ID, IDENTITY_NUMBER, FULL_NAME, POSITION)
     VALUES (17, 3, '000000001','Asignaci�n Autom�tica de Ordenes Confirmadas', 'Servicios Autom�ticos')
GO
INSERT INTO process_satellites (PROCESS_SATELLITE_NAME, PROCESS_SATELLITE_ADDRESS, IDENTITY_TYPE_ID, IDENTITY_NUMBER, PHONE, CITY_ID, 
			LEGAL_REPRESENTATIVE, IS_ACTIVE)
	 VALUES ('Asignaci�n Autom�tica de Ordenes Confirmadas','PROMOS LTDA.', 3, '830.018.350-3', '', 9,'',1)
GO
INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
     VALUES ('E414C60B-2037-4BBA-83A3-9B63771E8C5F', '5B6AFF1D-8B46-42B5-9618-443DB7F29D53', 'Consulta de notificaciones por asignaci�n autom�tica de pedidos', 'CONSULTA DE NOTIFICACIONES POR ASIGNACI�N AUTOM�TICA DE PEDIDOS')
GO
