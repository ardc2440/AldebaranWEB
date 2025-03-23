INSERT INTO employees (AREA_ID, IDENTITY_TYPE_ID, IDENTITY_NUMBER, FULL_NAME, POSITION)
     VALUES (17, 3, '000000001','Creación Automática de Traslados a Proceso', 'Servicios Automáticos')
GO
INSERT INTO process_satellites (PROCESS_SATELLITE_NAME, PROCESS_SATELLITE_ADDRESS, IDENTITY_TYPE_ID, IDENTITY_NUMBER, PHONE, CITY_ID, 
			LEGAL_REPRESENTATIVE, IS_ACTIVE)
	 VALUES ('Creación Automática de Traslados a Proceso','PROMOS LTDA.', 3, '830.018.350-3', '', 9,'',1)
GO
/*  
	Ojo no olvidar meter el parametro y el valor en el appSettings.json de la aplicacion, despues de correr este insert
	en la seccion "AppSettings" agregar la siguente llave reemplazando el 11 con el niuevo ID generaado en el inser anterior
	"ProcessSatelliteId": 11
*/
INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
     VALUES ('E414C60B-2037-4BBA-83A3-9B63771E8C5F', '5B6AFF1D-8B46-42B5-9618-443DB7F29D53', 'Consulta de notificaciones por creación automática de traslados a proceso', 'CONSULTA DE NOTIFICACIONES POR CREACIÓN AUTOMÁTICA DE TRASLADOS A PROCESO')
GO
INSERT INTO modification_reasons (MODIFICATION_REASON_NAME, DOCUMENT_TYPE_ID, NOTES)
	 VALUES('Cambio de Satelite',4,'Cambio de satélite por creación automática de traslado a proceso')
GO