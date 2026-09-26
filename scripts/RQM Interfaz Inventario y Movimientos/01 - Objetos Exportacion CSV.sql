/* =====================================================================================
   RQM  : Interfaz para Consulta de Inventario y Movimientos - FASE 1 (Exportación CSV)
   Autor: Andrés Ricardo Díaz Cárdenas
   Fecha: 2026-09-24

   Objetos:
     Tabla      : SYSTEM_PARAMETERS (clave - valor, reutilizable para otros procesos)
     Función    : FN_CSV_TEXT (limpieza de texto para CSV)
     Procedures : SP_CSV_EXPORT_CUSTOMER_ORDERS  -> Pedidos_yyyyMMdd_HHmmss.csv       (novedades)
                  SP_CSV_EXPORT_INVENTORY        -> Inventario_yyyyMMdd_HHmmss.csv    (fotografía)
                  SP_CSV_EXPORT_PURCHASE_ORDERS  -> OrdenesCompra_yyyyMMdd_HHmmss.csv (novedades)
                  SP_CSV_EXPORT_NOTIFY           -> correo con el resultado de los 3 archivos (paso 4 del Job)
     Índices    : apoyo a la extracción incremental por fechas

   Funcionamiento de cada procedimiento:
     - Llamado sin parámetros (desde el Job): calcula la ventana (última ejecución -> ahora),
       arma y ejecuta el bcp vía xp_cmdshell, valida el resultado y, si fue exitoso,
       actualiza su parámetro de última ejecución. Si falla, lanza error (el paso del Job falla)
       y la marca NO se mueve, así la siguiente corrida recupera el periodo.
     - Llamado con @RETURN_DATA = 1 (lo hace el propio bcp): devuelve las filas del CSV.

   Formato: separador "|", UTF-8 sin BOM (bcp -c -C 65001), primera fila = encabezados.

   Requisitos del servidor:
     - xp_cmdshell habilitado (ver sección 0).
     - Database Mail configurado; el dueño del Job debe ser sysadmin o miembro de DatabaseMailUserRole (msdb).
     - bcp versión 13 (SQL Server 2016) o superior (soporte -C 65001).
     - La cuenta de servicio de SQL Server (o la proxy de xp_cmdshell) debe tener permiso de
       escritura en la ruta de Staging y login en la instancia (bcp usa -T).

   Script idempotente: puede ejecutarse varias veces.
   ===================================================================================== */
USE Aldebaran
GO

/* -------------------------------------------------------------------------------------
   0. HABILITAR xp_cmdshell (requiere sysadmin; ejecutar solo si no está habilitado)
   -------------------------------------------------------------------------------------
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1;           RECONFIGURE;
*/

/* -------------------------------------------------------------------------------------
   1. TABLA DE PARÁMETROS (clave - valor)
   ------------------------------------------------------------------------------------- */
IF OBJECT_ID('dbo.SYSTEM_PARAMETERS') IS NULL
BEGIN
	CREATE TABLE dbo.SYSTEM_PARAMETERS
	(
		PARAMETER_KEY    VARCHAR(100)   NOT NULL CONSTRAINT PK_SYSTEM_PARAMETERS PRIMARY KEY,
		PARAMETER_VALUE  NVARCHAR(4000) NULL,
		DESCRIPTION      NVARCHAR(250)  NULL,
		UPDATE_DATE      DATETIME       NOT NULL CONSTRAINT DF_SYSTEM_PARAMETERS_UPDATE_DATE DEFAULT GETDATE()
	)
END
GO

MERGE dbo.SYSTEM_PARAMETERS AS t
USING (VALUES
	('CSV_EXPORT_STAGING_PATH',                  N'\\ALDEB\Staging\', N'Ruta de Staging de los CSV. Formato C:\ruta\ o \\servidor\ruta\. Debe ser accesible por la cuenta de servicio de SQL Server.'),
	('CSV_EXPORT_LAST_EXECUTION_CUSTOMER_ORDERS', NULL,                    N'Última ejecución exitosa del CSV de Pedidos (yyyy-MM-dd HH:mm:ss.fff). Vacío = exporta todo.'),
	('CSV_EXPORT_LAST_EXECUTION_INVENTORY',       NULL,                    N'Última ejecución exitosa del CSV de Inventario (solo informativo, siempre es fotografía completa).'),
	('CSV_EXPORT_LAST_EXECUTION_PURCHASE_ORDERS', NULL,                    N'Última ejecución exitosa del CSV de Órdenes de Compra (yyyy-MM-dd HH:mm:ss.fff). Vacío = exporta todo.'),
	('CSV_EXPORT_MAIL_PROFILE',                   N'Correos procesos automaticos Aldebaran',            N'Perfil de Database Mail usado para notificar el resultado de la exportación CSV.'),
	('CSV_EXPORT_MAIL_RECIPIENTS',                N'g.ramirez@catalogospromocionales.com;soporte@catalogospromocionales.com',    N'Destinatarios de la notificación de exportación CSV, separados por punto y coma (;).')
) AS s (PARAMETER_KEY, PARAMETER_VALUE, DESCRIPTION)
ON t.PARAMETER_KEY = s.PARAMETER_KEY
WHEN NOT MATCHED THEN
	INSERT (PARAMETER_KEY, PARAMETER_VALUE, DESCRIPTION) VALUES (s.PARAMETER_KEY, s.PARAMETER_VALUE, s.DESCRIPTION);
GO

/* -------------------------------------------------------------------------------------
   2. ÍNDICES DE APOYO (ventana incremental por fechas)
   ------------------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PURCHASE_ORDER_ACTIVITIES_CREATION_DATE' AND object_id = OBJECT_ID('dbo.PURCHASE_ORDER_ACTIVITIES'))
	CREATE NONCLUSTERED INDEX IX_PURCHASE_ORDER_ACTIVITIES_CREATION_DATE ON dbo.PURCHASE_ORDER_ACTIVITIES (CREATION_DATE) INCLUDE (PURCHASE_ORDER_ID)
GO

/* -------------------------------------------------------------------------------------
   3. FUNCIÓN DE LIMPIEZA DE TEXTO: el separador "|" y los saltos de línea romperían el CSV
   ------------------------------------------------------------------------------------- */
/* NVARCHAR(4000) y no MAX a propósito: un valor MAX contamina el CONCAT y convierte cada
   línea del CSV en LOB, lo que dispara lecturas en tempdb al ordenar (ver nota en los SP). */
CREATE OR ALTER FUNCTION dbo.FN_CSV_TEXT (@Value NVARCHAR(4000))
RETURNS NVARCHAR(4000)
WITH SCHEMABINDING
AS
BEGIN
	RETURN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(@Value, N'|', N'/'), CHAR(13), N' '), CHAR(10), N' '), CHAR(9), N' ')))
END
GO

/* =====================================================================================
   4. LISTADO DE PEDIDOS (incremental)
      Pedidos creados (CREATION_DATE) o modificados (MODIFIED_CUSTOMER_ORDERS) en la ventana.
      Se exporta el pedido completo (todos sus detalles).
      Se excluyen los Pedidos Cancelados (STATUS_DOCUMENT_TYPE_CODE = 'A').
   ===================================================================================== */
CREATE OR ALTER PROCEDURE dbo.SP_CSV_EXPORT_CUSTOMER_ORDERS
	@RETURN_DATA BIT      = 0,     /* uso interno: 1 = devolver filas (lo invoca el bcp) */
	@DATE_FROM   DATETIME = NULL,  /* uso interno: exclusivo  */
	@DATE_TO     DATETIME = NULL   /* uso interno: inclusivo  */
AS
BEGIN
	SET NOCOUNT ON;

	/* ---------- Modo datos: invocado por bcp ---------- */
	IF @RETURN_DATA = 1
	BEGIN
		/* 1. Pedidos a exportar: se materializan UNA vez (con PK) para no recalcular el UNION
		      dentro de los JOIN; con CTE el optimizador lo reevaluaba por fila (costo cuadrático). */
		DECLARE @Orders TABLE (CUSTOMER_ORDER_ID INT NOT NULL PRIMARY KEY, LAST_MODIFICATION_DATE DATETIME NULL);

		INSERT INTO @Orders (CUSTOMER_ORDER_ID)
		SELECT co.CUSTOMER_ORDER_ID
		  FROM dbo.CUSTOMER_ORDERS co
		 WHERE co.CREATION_DATE <= @DATE_TO AND (@DATE_FROM IS NULL OR co.CREATION_DATE > @DATE_FROM)
		UNION
		SELECT m.CUSTOMER_ORDER_ID
		  FROM dbo.MODIFIED_CUSTOMER_ORDERS m
		 WHERE m.MODIFICATION_DATE <= @DATE_TO AND (@DATE_FROM IS NULL OR m.MODIFICATION_DATE > @DATE_FROM)
		OPTION (RECOMPILE);

		/* 2. Fecha de última modificación de cada pedido */
		UPDATE o
		   SET LAST_MODIFICATION_DATE = lm.LAST_MODIFICATION_DATE
		  FROM @Orders o
		  JOIN (SELECT m.CUSTOMER_ORDER_ID, MAX(m.MODIFICATION_DATE) AS LAST_MODIFICATION_DATE
		          FROM dbo.MODIFIED_CUSTOMER_ORDERS m
		         WHERE m.MODIFICATION_DATE <= @DATE_TO
		         GROUP BY m.CUSTOMER_ORDER_ID) lm ON lm.CUSTOMER_ORDER_ID = o.CUSTOMER_ORDER_ID
		OPTION (RECOMPILE);

		/* 3. Filas del CSV (encabezado + detalle) */
		WITH Lines AS
		(
			SELECT 0 AS SORT_GROUP, 0 AS ORDER_ID, 0 AS DETAIL_ID,
			       CAST(N'NumeroPedido|IdentificacionCliente|Cliente|FechaPedido|FechaCreacion|FechaUltimaModificacion|EstadoPedido|ReferenciaInterna|NombreArticulo|CodigoSubreferencia|NombreSubreferencia|CantidadSolicitada|CantidadEnProceso|CantidadEntregada' AS NVARCHAR(4000)) AS LINE  /* 4000, no MAX: evita LOB por fila al ordenar */
			UNION ALL
			SELECT 1, co.CUSTOMER_ORDER_ID, cod.CUSTOMER_ORDER_DETAIL_ID,
			       CONCAT(
			           dbo.FN_CSV_TEXT(co.ORDER_NUMBER),                 N'|',
			           dbo.FN_CSV_TEXT(cu.IDENTITY_NUMBER),              N'|',
			           dbo.FN_CSV_TEXT(cu.CUSTOMER_NAME),                N'|',
			           CONVERT(CHAR(10), co.ORDER_DATE, 23),             N'|',
			           CONVERT(CHAR(19), co.CREATION_DATE, 120),         N'|',
			           CONVERT(CHAR(19), o.LAST_MODIFICATION_DATE, 120), N'|',
			           dbo.FN_CSV_TEXT(sdt.STATUS_DOCUMENT_TYPE_NAME),   N'|',
			           dbo.FN_CSV_TEXT(i.INTERNAL_REFERENCE),            N'|',
			           dbo.FN_CSV_TEXT(i.ITEM_NAME),                     N'|',
			           dbo.FN_CSV_TEXT(ir.REFERENCE_CODE),               N'|',
			           dbo.FN_CSV_TEXT(ir.REFERENCE_NAME),               N'|',
			           cod.REQUESTED_QUANTITY,                           N'|',
			           cod.PROCESSED_QUANTITY,                           N'|',
			           cod.DELIVERED_QUANTITY)
			  FROM @Orders o
			  JOIN dbo.CUSTOMER_ORDERS co         ON co.CUSTOMER_ORDER_ID = o.CUSTOMER_ORDER_ID
			  JOIN dbo.CUSTOMERS cu               ON cu.CUSTOMER_ID = co.CUSTOMER_ID
			  JOIN dbo.STATUS_DOCUMENT_TYPES sdt  ON sdt.STATUS_DOCUMENT_TYPE_ID = co.STATUS_DOCUMENT_TYPE_ID
			  JOIN dbo.CUSTOMER_ORDER_DETAILS cod ON cod.CUSTOMER_ORDER_ID = co.CUSTOMER_ORDER_ID
			  JOIN dbo.ITEM_REFERENCES ir         ON ir.REFERENCE_ID = cod.REFERENCE_ID
			  JOIN dbo.ITEMS i                    ON i.ITEM_ID = ir.ITEM_ID
			 WHERE sdt.STATUS_DOCUMENT_TYPE_CODE <> 'A'   /* se descartan los Pedidos Cancelados */
		)
		SELECT CAST(LINE AS NVARCHAR(4000)) AS LINE
		  FROM Lines
		 ORDER BY SORT_GROUP, ORDER_ID, DETAIL_ID
		OPTION (RECOMPILE);

		RETURN;
	END

	/* ---------- Modo exportación: invocado por el Job ---------- */
	DECLARE @LastKey VARCHAR(100) = 'CSV_EXPORT_LAST_EXECUTION_CUSTOMER_ORDERS',
	        @Path NVARCHAR(4000), @From DATETIME, @To DATETIME = GETDATE(),
	        @Command VARCHAR(8000), @Result INT, @Output NVARCHAR(MAX);
	DECLARE @CmdOutput TABLE (ID INT IDENTITY(1,1), LINE NVARCHAR(4000));

	SELECT @Path = PARAMETER_VALUE FROM dbo.SYSTEM_PARAMETERS WHERE PARAMETER_KEY = 'CSV_EXPORT_STAGING_PATH';
	SELECT @From = TRY_CONVERT(DATETIME, NULLIF(PARAMETER_VALUE, N''), 121) FROM dbo.SYSTEM_PARAMETERS WHERE PARAMETER_KEY = @LastKey;

	IF @Path IS NULL OR NOT (@Path LIKE N'[A-Za-z]:\%' OR @Path LIKE N'\\[^\]%') OR CHARINDEX(N'"', @Path) > 0
		THROW 50001, N'Parámetro CSV_EXPORT_STAGING_PATH inválido. Formato esperado: C:\ruta\ o \\servidor\ruta\', 1;
	IF RIGHT(@Path, 1) <> N'\' SET @Path += N'\';

	SET @Command = CONCAT(
		'bcp "EXEC dbo.SP_CSV_EXPORT_CUSTOMER_ORDERS @RETURN_DATA = 1',
		', @DATE_FROM = ', ISNULL('''' + CONVERT(VARCHAR(23), @From, 126) + '''', 'NULL'),
		', @DATE_TO = ''', CONVERT(VARCHAR(23), @To, 126), '''',
		' WITH RESULT SETS ((LINE NVARCHAR(4000)))"',
		' queryout "', @Path, 'Pedidos_', FORMAT(@To, 'yyyyMMdd_HHmmss'), '.csv"',
		' -S "', CAST(SERVERPROPERTY('ServerName') AS VARCHAR(128)), '" -d "', DB_NAME(), '" -T -c -C 65001');

	INSERT INTO @CmdOutput (LINE) EXEC @Result = master.dbo.xp_cmdshell @Command;

	IF @Result <> 0 OR EXISTS (SELECT 1 FROM @CmdOutput WHERE LINE LIKE N'%Error%')
	BEGIN
		SELECT @Output = STRING_AGG(LINE, N' ') WITHIN GROUP (ORDER BY ID) FROM @CmdOutput WHERE LINE IS NOT NULL;
		SET @Output = LEFT(CONCAT(N'Error generando CSV de Pedidos: ', @Output), 2047);
		THROW 50002, @Output, 1;
	END

	UPDATE dbo.SYSTEM_PARAMETERS
	   SET PARAMETER_VALUE = CONVERT(NVARCHAR(23), @To, 121), UPDATE_DATE = GETDATE()
	 WHERE PARAMETER_KEY = @LastKey;
END
GO

/* =====================================================================================
   5. LISTADO DE INVENTARIO (fotografía completa, artículos y referencias activos)
        Stock Físico          = INVENTORY_QUANTITY (debe ser = Bodega Local + Zona Franca)
        Cantidad Comprometida = Pedidos Pendientes (ORDERED) + Reservas (RESERVED)
        Cantidad en Tránsito  = Compras Pendientes (STATUS_ORDER 1) + Compras En Aprobación (STATUS_ORDER 4)
        Stock Disponible      = Stock Físico + Cantidad en Tránsito - Cantidad Comprometida
   ===================================================================================== */
CREATE OR ALTER PROCEDURE dbo.SP_CSV_EXPORT_INVENTORY
	@RETURN_DATA BIT      = 0,     /* uso interno: 1 = devolver filas (lo invoca el bcp) */
	@DATE_TO     DATETIME = NULL   /* uso interno: fecha de corte */
AS
BEGIN
	SET NOCOUNT ON;

	/* ---------- Modo datos: invocado por bcp ---------- */
	IF @RETURN_DATA = 1
	BEGIN
		WITH WarehouseStock AS
		(
			SELECT rw.REFERENCE_ID,
			       SUM(CASE WHEN w.WAREHOUSE_CODE = 1 THEN rw.QUANTITY ELSE 0 END) AS LOCAL_QUANTITY,
			       SUM(CASE WHEN w.WAREHOUSE_CODE = 2 THEN rw.QUANTITY ELSE 0 END) AS FREE_ZONE_QUANTITY
			  FROM dbo.REFERENCES_WAREHOUSE rw
			  JOIN dbo.WAREHOUSES w ON w.WAREHOUSE_ID = rw.WAREHOUSE_ID
			 GROUP BY rw.REFERENCE_ID
		),
		Transit AS
		(
			SELECT pod.REFERENCE_ID,
			       SUM(CASE WHEN sdt.STATUS_ORDER = 1 THEN pod.REQUESTED_QUANTITY ELSE 0 END) AS PENDING_QUANTITY,
			       SUM(CASE WHEN sdt.STATUS_ORDER = 4 THEN pod.REQUESTED_QUANTITY ELSE 0 END) AS APPROVAL_QUANTITY
			  FROM dbo.PURCHASE_ORDER_DETAILS pod
			  JOIN dbo.PURCHASE_ORDERS po        ON po.PURCHASE_ORDER_ID = pod.PURCHASE_ORDER_ID
			  JOIN dbo.STATUS_DOCUMENT_TYPES sdt ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
			  JOIN dbo.DOCUMENT_TYPES dt         ON dt.DOCUMENT_TYPE_ID = sdt.DOCUMENT_TYPE_ID AND dt.DOCUMENT_TYPE_CODE = 'O'
			 WHERE sdt.STATUS_ORDER IN (1, 4)
			 GROUP BY pod.REFERENCE_ID
		),
		Stock AS
		(
			SELECT i.INTERNAL_REFERENCE, i.ITEM_NAME, ir.REFERENCE_CODE, ir.REFERENCE_NAME,
			       i.IS_DOMESTIC_PRODUCT, i.IS_CATALOG_VISIBLE, i.IS_SALE_OFF, i.IS_SPECIAL_IMPORT,
			       ir.INVENTORY_QUANTITY                      AS PHYSICAL_QUANTITY,
			       ISNULL(ws.LOCAL_QUANTITY, 0)               AS LOCAL_QUANTITY,
			       ISNULL(ws.FREE_ZONE_QUANTITY, 0)           AS FREE_ZONE_QUANTITY,
			       ir.ORDERED_QUANTITY,
			       ir.RESERVED_QUANTITY,
			       ir.ORDERED_QUANTITY + ir.RESERVED_QUANTITY AS COMMITTED_QUANTITY,
			       ISNULL(t.PENDING_QUANTITY, 0)              AS PENDING_QUANTITY,
			       ISNULL(t.APPROVAL_QUANTITY, 0)             AS APPROVAL_QUANTITY,
			       ISNULL(t.PENDING_QUANTITY, 0) + ISNULL(t.APPROVAL_QUANTITY, 0) AS TRANSIT_QUANTITY
			  FROM dbo.ITEM_REFERENCES ir
			  JOIN dbo.ITEMS i            ON i.ITEM_ID = ir.ITEM_ID
			  LEFT JOIN WarehouseStock ws ON ws.REFERENCE_ID = ir.REFERENCE_ID
			  LEFT JOIN Transit t         ON t.REFERENCE_ID = ir.REFERENCE_ID
			 WHERE i.IS_ACTIVE = 1
			   AND ir.IS_ACTIVE = 1
		),
		Lines AS
		(
			SELECT 0 AS SORT_GROUP, CAST(N'' AS NVARCHAR(30)) AS SORT_ITEM, CAST(N'' AS NVARCHAR(30)) AS SORT_REFERENCE,
			       CAST(N'ReferenciaInterna|NombreArticulo|CodigoSubreferencia|NombreSubreferencia|StockFisico|StockBodegaLocal|StockZonaFranca|PedidosPendientes|Reservas|CantidadComprometida|ComprasPendientes|ComprasEnAprobacion|CantidadEnTransito|StockDisponible|FechaCorte|ProductoNacional|VisibleCatalogo|Oferta|ImportacionEspecial' AS NVARCHAR(4000)) AS LINE  /* 4000, no MAX: evita LOB por fila al ordenar */
			UNION ALL
			SELECT 1, s.INTERNAL_REFERENCE, s.REFERENCE_CODE,
			       CONCAT(
			           dbo.FN_CSV_TEXT(s.INTERNAL_REFERENCE), N'|',
			           dbo.FN_CSV_TEXT(s.ITEM_NAME),          N'|',
			           dbo.FN_CSV_TEXT(s.REFERENCE_CODE),     N'|',
			           dbo.FN_CSV_TEXT(s.REFERENCE_NAME),     N'|',
			           s.PHYSICAL_QUANTITY,                   N'|',
			           s.LOCAL_QUANTITY,                      N'|',
			           s.FREE_ZONE_QUANTITY,                  N'|',
			           s.ORDERED_QUANTITY,                    N'|',
			           s.RESERVED_QUANTITY,                   N'|',
			           s.COMMITTED_QUANTITY,                  N'|',
			           s.PENDING_QUANTITY,                    N'|',
			           s.APPROVAL_QUANTITY,                   N'|',
			           s.TRANSIT_QUANTITY,                    N'|',
			           s.PHYSICAL_QUANTITY + s.TRANSIT_QUANTITY - s.COMMITTED_QUANTITY, N'|',
			           CONVERT(CHAR(19), @DATE_TO, 120),      N'|',
			           IIF(s.IS_DOMESTIC_PRODUCT = 1, N'Sí', N'No'), N'|',
			           IIF(s.IS_CATALOG_VISIBLE  = 1, N'Sí', N'No'), N'|',
			           IIF(s.IS_SALE_OFF         = 1, N'Sí', N'No'), N'|',
			           IIF(s.IS_SPECIAL_IMPORT   = 1, N'Sí', N'No'))
			  FROM Stock s
		)
		SELECT CAST(LINE AS NVARCHAR(4000)) AS LINE
		  FROM Lines
		 ORDER BY SORT_GROUP, SORT_ITEM, SORT_REFERENCE;

		RETURN;
	END

	/* ---------- Modo exportación: invocado por el Job ---------- */
	DECLARE @LastKey VARCHAR(100) = 'CSV_EXPORT_LAST_EXECUTION_INVENTORY',
	        @Path NVARCHAR(4000), @To DATETIME = GETDATE(),
	        @Command VARCHAR(8000), @Result INT, @Output NVARCHAR(MAX);
	DECLARE @CmdOutput TABLE (ID INT IDENTITY(1,1), LINE NVARCHAR(4000));

	SELECT @Path = PARAMETER_VALUE FROM dbo.SYSTEM_PARAMETERS WHERE PARAMETER_KEY = 'CSV_EXPORT_STAGING_PATH';

	IF @Path IS NULL OR NOT (@Path LIKE N'[A-Za-z]:\%' OR @Path LIKE N'\\[^\]%') OR CHARINDEX(N'"', @Path) > 0
		THROW 50001, N'Parámetro CSV_EXPORT_STAGING_PATH inválido. Formato esperado: C:\ruta\ o \\servidor\ruta\', 1;
	IF RIGHT(@Path, 1) <> N'\' SET @Path += N'\';

	SET @Command = CONCAT(
		'bcp "EXEC dbo.SP_CSV_EXPORT_INVENTORY @RETURN_DATA = 1',
		', @DATE_TO = ''', CONVERT(VARCHAR(23), @To, 126), '''',
		' WITH RESULT SETS ((LINE NVARCHAR(4000)))"',
		' queryout "', @Path, 'Inventario_', FORMAT(@To, 'yyyyMMdd_HHmmss'), '.csv"',
		' -S "', CAST(SERVERPROPERTY('ServerName') AS VARCHAR(128)), '" -d "', DB_NAME(), '" -T -c -C 65001');

	INSERT INTO @CmdOutput (LINE) EXEC @Result = master.dbo.xp_cmdshell @Command;

	IF @Result <> 0 OR EXISTS (SELECT 1 FROM @CmdOutput WHERE LINE LIKE N'%Error%')
	BEGIN
		SELECT @Output = STRING_AGG(LINE, N' ') WITHIN GROUP (ORDER BY ID) FROM @CmdOutput WHERE LINE IS NOT NULL;
		SET @Output = LEFT(CONCAT(N'Error generando CSV de Inventario: ', @Output), 2047);
		THROW 50002, @Output, 1;
	END

	UPDATE dbo.SYSTEM_PARAMETERS
	   SET PARAMETER_VALUE = CONVERT(NVARCHAR(23), @To, 121), UPDATE_DATE = GETDATE()
	 WHERE PARAMETER_KEY = @LastKey;
END
GO

/* =====================================================================================
   6. LISTADO DE ÓRDENES DE COMPRA (incremental)
      OC creadas, modificadas (MODIFIED_PURCHASE_ORDERS) o con anotaciones nuevas
      (PURCHASE_ORDER_ACTIVITIES.CREATION_DATE) en la ventana.
      Se exporta la OC completa: una fila por detalle x anotación (todas sus anotaciones).
      Cantidad Recibida solo cuando la OC está Confirmada (STATUS_ORDER 2).
      Se excluyen las OC Canceladas (STATUS_DOCUMENT_TYPE_CODE = 'A').
   ===================================================================================== */
CREATE OR ALTER PROCEDURE dbo.SP_CSV_EXPORT_PURCHASE_ORDERS
	@RETURN_DATA BIT      = 0,     /* uso interno: 1 = devolver filas (lo invoca el bcp) */
	@DATE_FROM   DATETIME = NULL,  /* uso interno: exclusivo  */
	@DATE_TO     DATETIME = NULL   /* uso interno: inclusivo  */
AS
BEGIN
	SET NOCOUNT ON;

	/* ---------- Modo datos: invocado por bcp ---------- */
	IF @RETURN_DATA = 1
	BEGIN
		/* 1. OC a exportar: se materializan UNA vez (con PK) para no recalcular el UNION
		      dentro de los JOIN; con CTE el optimizador lo reevaluaba por fila (costo cuadrático). */
		DECLARE @Orders TABLE (PURCHASE_ORDER_ID INT NOT NULL PRIMARY KEY, LAST_MODIFICATION_DATE DATETIME NULL);

		INSERT INTO @Orders (PURCHASE_ORDER_ID)
		SELECT po.PURCHASE_ORDER_ID
		  FROM dbo.PURCHASE_ORDERS po
		 WHERE po.CREATION_DATE <= @DATE_TO AND (@DATE_FROM IS NULL OR po.CREATION_DATE > @DATE_FROM)
		UNION
		SELECT m.PURCHASE_ORDER_ID
		  FROM dbo.MODIFIED_PURCHASE_ORDERS m
		 WHERE m.MODIFICATION_DATE <= @DATE_TO AND (@DATE_FROM IS NULL OR m.MODIFICATION_DATE > @DATE_FROM)
		UNION
		SELECT a.PURCHASE_ORDER_ID
		  FROM dbo.PURCHASE_ORDER_ACTIVITIES a
		 WHERE a.CREATION_DATE <= @DATE_TO AND (@DATE_FROM IS NULL OR a.CREATION_DATE > @DATE_FROM)
		OPTION (RECOMPILE);

		/* 2. Fecha de última modificación de cada OC */
		UPDATE o
		   SET LAST_MODIFICATION_DATE = lm.LAST_MODIFICATION_DATE
		  FROM @Orders o
		  JOIN (SELECT m.PURCHASE_ORDER_ID, MAX(m.MODIFICATION_DATE) AS LAST_MODIFICATION_DATE
		          FROM dbo.MODIFIED_PURCHASE_ORDERS m
		         WHERE m.MODIFICATION_DATE <= @DATE_TO
		         GROUP BY m.PURCHASE_ORDER_ID) lm ON lm.PURCHASE_ORDER_ID = o.PURCHASE_ORDER_ID
		OPTION (RECOMPILE);

		/* 3. Filas del CSV (encabezado + detalle x anotación) */
		WITH Lines AS
		(
			SELECT 0 AS SORT_GROUP, 0 AS ORDER_ID, 0 AS DETAIL_ID, CAST(NULL AS DATETIME) AS ACTIVITY_DATE, 0 AS ACTIVITY_ID,
			       CAST(N'NumeroOrden|IdentificacionProveedor|Proveedor|EstadoOrden|FechaCompra|FechaEstimadaEntrega|FechaRealEntrega|FechaCreacion|FechaUltimaModificacion|ReferenciaInterna|NombreArticulo|CodigoSubreferencia|NombreSubreferencia|BodegaDestino|CantidadSolicitada|CantidadRecibida|FechaAnotacion|Anotacion' AS NVARCHAR(4000)) AS LINE  /* 4000, no MAX: evita LOB por fila al ordenar */
			UNION ALL
			SELECT 1, po.PURCHASE_ORDER_ID, pod.PURCHASE_ORDER_DETAIL_ID, a.EXECUTION_DATE, ISNULL(a.PURCHASE_ORDER_ACTIVITY_ID, 0),
			       CONCAT(
			           dbo.FN_CSV_TEXT(po.ORDER_NUMBER),                 N'|',
			           dbo.FN_CSV_TEXT(p.IDENTITY_NUMBER),               N'|',
			           dbo.FN_CSV_TEXT(p.PROVIDER_NAME),                 N'|',
			           dbo.FN_CSV_TEXT(sdt.STATUS_DOCUMENT_TYPE_NAME),   N'|',
			           CONVERT(CHAR(10), po.REQUEST_DATE, 23),           N'|',
			           CONVERT(CHAR(10), po.EXPECTED_RECEIPT_DATE, 23),  N'|',
			           CONVERT(CHAR(10), po.REAL_RECEIPT_DATE, 23),      N'|',
			           CONVERT(CHAR(19), po.CREATION_DATE, 120),         N'|',
			           CONVERT(CHAR(19), o.LAST_MODIFICATION_DATE, 120), N'|',
			           dbo.FN_CSV_TEXT(i.INTERNAL_REFERENCE),            N'|',
			           dbo.FN_CSV_TEXT(i.ITEM_NAME),                     N'|',
			           dbo.FN_CSV_TEXT(ir.REFERENCE_CODE),               N'|',
			           dbo.FN_CSV_TEXT(ir.REFERENCE_NAME),               N'|',
			           dbo.FN_CSV_TEXT(w.WAREHOUSE_NAME),                N'|',
			           pod.REQUESTED_QUANTITY,                           N'|',
			           CASE WHEN sdt.STATUS_ORDER = 2 THEN pod.RECEIVED_QUANTITY END, N'|',
			           CONVERT(CHAR(10), a.EXECUTION_DATE, 23),          N'|',
			           dbo.FN_CSV_TEXT(a.ACTIVITY_DESCRIPTION))
			  FROM @Orders o
			  JOIN dbo.PURCHASE_ORDERS po         ON po.PURCHASE_ORDER_ID = o.PURCHASE_ORDER_ID
			  JOIN dbo.PROVIDERS p                ON p.PROVIDER_ID = po.PROVIDER_ID
			  JOIN dbo.STATUS_DOCUMENT_TYPES sdt  ON sdt.STATUS_DOCUMENT_TYPE_ID = po.STATUS_DOCUMENT_TYPE_ID
			  JOIN dbo.PURCHASE_ORDER_DETAILS pod ON pod.PURCHASE_ORDER_ID = po.PURCHASE_ORDER_ID
			  JOIN dbo.ITEM_REFERENCES ir         ON ir.REFERENCE_ID = pod.REFERENCE_ID
			  JOIN dbo.ITEMS i                    ON i.ITEM_ID = ir.ITEM_ID
			  JOIN dbo.WAREHOUSES w               ON w.WAREHOUSE_ID = pod.WAREHOUSE_ID
			  LEFT JOIN dbo.PURCHASE_ORDER_ACTIVITIES a ON a.PURCHASE_ORDER_ID = po.PURCHASE_ORDER_ID AND a.CREATION_DATE <= @DATE_TO
			 WHERE sdt.STATUS_DOCUMENT_TYPE_CODE <> 'A'   /* se descartan las OC Canceladas */
		)
		SELECT CAST(LINE AS NVARCHAR(4000)) AS LINE
		  FROM Lines
		 ORDER BY SORT_GROUP, ORDER_ID, DETAIL_ID, ACTIVITY_DATE, ACTIVITY_ID
		OPTION (RECOMPILE);

		RETURN;
	END

	/* ---------- Modo exportación: invocado por el Job ---------- */
	DECLARE @LastKey VARCHAR(100) = 'CSV_EXPORT_LAST_EXECUTION_PURCHASE_ORDERS',
	        @Path NVARCHAR(4000), @From DATETIME, @To DATETIME = GETDATE(),
	        @Command VARCHAR(8000), @Result INT, @Output NVARCHAR(MAX);
	DECLARE @CmdOutput TABLE (ID INT IDENTITY(1,1), LINE NVARCHAR(4000));

	SELECT @Path = PARAMETER_VALUE FROM dbo.SYSTEM_PARAMETERS WHERE PARAMETER_KEY = 'CSV_EXPORT_STAGING_PATH';
	SELECT @From = TRY_CONVERT(DATETIME, NULLIF(PARAMETER_VALUE, N''), 121) FROM dbo.SYSTEM_PARAMETERS WHERE PARAMETER_KEY = @LastKey;

	IF @Path IS NULL OR NOT (@Path LIKE N'[A-Za-z]:\%' OR @Path LIKE N'\\[^\]%') OR CHARINDEX(N'"', @Path) > 0
		THROW 50001, N'Parámetro CSV_EXPORT_STAGING_PATH inválido. Formato esperado: C:\ruta\ o \\servidor\ruta\', 1;
	IF RIGHT(@Path, 1) <> N'\' SET @Path += N'\';

	SET @Command = CONCAT(
		'bcp "EXEC dbo.SP_CSV_EXPORT_PURCHASE_ORDERS @RETURN_DATA = 1',
		', @DATE_FROM = ', ISNULL('''' + CONVERT(VARCHAR(23), @From, 126) + '''', 'NULL'),
		', @DATE_TO = ''', CONVERT(VARCHAR(23), @To, 126), '''',
		' WITH RESULT SETS ((LINE NVARCHAR(4000)))"',
		' queryout "', @Path, 'OrdenesCompra_', FORMAT(@To, 'yyyyMMdd_HHmmss'), '.csv"',
		' -S "', CAST(SERVERPROPERTY('ServerName') AS VARCHAR(128)), '" -d "', DB_NAME(), '" -T -c -C 65001');

	INSERT INTO @CmdOutput (LINE) EXEC @Result = master.dbo.xp_cmdshell @Command;

	IF @Result <> 0 OR EXISTS (SELECT 1 FROM @CmdOutput WHERE LINE LIKE N'%Error%')
	BEGIN
		SELECT @Output = STRING_AGG(LINE, N' ') WITHIN GROUP (ORDER BY ID) FROM @CmdOutput WHERE LINE IS NOT NULL;
		SET @Output = LEFT(CONCAT(N'Error generando CSV de Órdenes de Compra: ', @Output), 2047);
		THROW 50002, @Output, 1;
	END

	UPDATE dbo.SYSTEM_PARAMETERS
	   SET PARAMETER_VALUE = CONVERT(NVARCHAR(23), @To, 121), UPDATE_DATE = GETDATE()
	 WHERE PARAMETER_KEY = @LastKey;
END
GO

/* =====================================================================================
   7. NOTIFICACIÓN POR CORREO (paso 4 del Job)
      Lee del historial del Job el resultado de los pasos 1-3 de la ejecución actual y envía
      un correo con el estado de cada archivo (nombre generado o error con el que falló).
      Si algún archivo falló, al final lanza error para que el Job quede registrado como fallido.
   ===================================================================================== */
CREATE OR ALTER PROCEDURE dbo.SP_CSV_EXPORT_NOTIFY
	@JOB_ID     UNIQUEIDENTIFIER,  /* token del Agent: $(ESCAPE_NONE(JOBID))  */
	@START_DATE INT,               /* token del Agent: $(ESCAPE_NONE(STRTDT)) yyyyMMdd */
	@START_TIME INT                /* token del Agent: $(ESCAPE_NONE(STRTTM)) HHmmss   */
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Profile NVARCHAR(4000), @Recipients NVARCHAR(4000), @Subject NVARCHAR(255),
	        @Body NVARCHAR(MAX), @Failures INT, @JobStart DATETIME = msdb.dbo.agent_datetime(@START_DATE, @START_TIME);

	SELECT @Profile    = MAX(CASE WHEN PARAMETER_KEY = 'CSV_EXPORT_MAIL_PROFILE'    THEN PARAMETER_VALUE END),
	       @Recipients = MAX(CASE WHEN PARAMETER_KEY = 'CSV_EXPORT_MAIL_RECIPIENTS' THEN PARAMETER_VALUE END)
	  FROM dbo.SYSTEM_PARAMETERS
	 WHERE PARAMETER_KEY IN ('CSV_EXPORT_MAIL_PROFILE', 'CSV_EXPORT_MAIL_RECIPIENTS');

	IF NULLIF(@Profile, N'') IS NULL OR NULLIF(@Recipients, N'') IS NULL
		THROW 50011, N'Parámetros CSV_EXPORT_MAIL_PROFILE / CSV_EXPORT_MAIL_RECIPIENTS sin configurar.', 1;

	DECLARE @Result TABLE (STEP_ID INT, LIST_NAME NVARCHAR(50), FILE_PREFIX NVARCHAR(50), LAST_KEY VARCHAR(100),
	                       RUN_STATUS INT NULL, MESSAGE NVARCHAR(4000) NULL, FILE_NAME NVARCHAR(260) NULL);

	INSERT INTO @Result (STEP_ID, LIST_NAME, FILE_PREFIX, LAST_KEY)
	VALUES (1, N'Pedidos',            N'Pedidos',       'CSV_EXPORT_LAST_EXECUTION_CUSTOMER_ORDERS'),
	       (2, N'Inventario',         N'Inventario',    'CSV_EXPORT_LAST_EXECUTION_INVENTORY'),
	       (3, N'Órdenes de Compra',  N'OrdenesCompra', 'CSV_EXPORT_LAST_EXECUTION_PURCHASE_ORDERS');

	/* Último resultado de cada paso dentro de la ejecución actual del Job */
	WITH History AS
	(
		SELECT h.step_id, h.run_status, h.message,
		       ROW_NUMBER() OVER (PARTITION BY h.step_id ORDER BY h.instance_id DESC) AS RN
		  FROM msdb.dbo.sysjobhistory h
		 WHERE h.job_id = @JOB_ID
		   AND h.step_id BETWEEN 1 AND 3
		   AND msdb.dbo.agent_datetime(h.run_date, h.run_time) >= @JobStart
	)
	UPDATE r
	   SET RUN_STATUS = h.run_status,
	       MESSAGE    = h.message
	  FROM @Result r
	  JOIN History h ON h.step_id = r.STEP_ID AND h.RN = 1;

	/* Archivo generado: el nombre se reconstruye con la marca de última ejecución (misma fecha usada en el bcp) */
	UPDATE r
	   SET FILE_NAME = CONCAT(r.FILE_PREFIX, N'_', FORMAT(TRY_CONVERT(DATETIME, p.PARAMETER_VALUE, 121), 'yyyyMMdd_HHmmss'), N'.csv')
	  FROM @Result r
	  JOIN dbo.SYSTEM_PARAMETERS p ON p.PARAMETER_KEY = r.LAST_KEY
	 WHERE r.RUN_STATUS = 1;

	SELECT @Failures = COUNT(*) FROM @Result WHERE ISNULL(RUN_STATUS, 0) <> 1;

	SET @Subject = CONCAT(N'Aldebaran - Exportación CSV ', CONVERT(CHAR(10), @JobStart, 23), N': ',
	                      CASE WHEN @Failures = 0 THEN N'archivos generados correctamente'
	                           ELSE CONCAT(@Failures, N' de 3 archivo(s) NO generado(s)') END);

	SELECT @Body = CONCAT(
		N'<p>Resultado de la exportación CSV del ', CONVERT(CHAR(19), @JobStart, 120), N'.</p>',
		N'<p>Ruta de Staging: <b>', (SELECT PARAMETER_VALUE FROM dbo.SYSTEM_PARAMETERS WHERE PARAMETER_KEY = 'CSV_EXPORT_STAGING_PATH'), N'</b></p>',
		N'<table border="1" cellpadding="6" cellspacing="0" style="border-collapse:collapse;font-family:Segoe UI,Arial;font-size:13px">',
		N'<tr style="background:#1f3864;color:#fff"><th>Listado</th><th>Estado</th><th>Archivo / Error</th></tr>',
		STRING_AGG(CAST(CONCAT(
			N'<tr><td>', LIST_NAME, N'</td>',
			CASE WHEN RUN_STATUS = 1 THEN N'<td style="color:#2e7d32"><b>Generado</b></td><td>' + FILE_NAME
			     WHEN RUN_STATUS IS NULL THEN N'<td style="color:#c62828"><b>No ejecutado</b></td><td>El paso no registró resultado en el historial del Job.'
			     ELSE N'<td style="color:#c62828"><b>Falló</b></td><td>No se pudo generar el archivo del día. Error: '
			          + REPLACE(REPLACE(REPLACE(ISNULL(MESSAGE, N''), N'&', N'&amp;'), N'<', N'&lt;'), N'>', N'&gt;') END,
			N'</td></tr>') AS NVARCHAR(MAX)), N'') WITHIN GROUP (ORDER BY STEP_ID),
		N'</table>',
		CASE WHEN @Failures > 0 THEN N'<p>Los archivos fallidos se recuperan automáticamente en la siguiente ejecución: la marca de última ejecución no avanzó para esos listados.</p>' ELSE N'' END)
	  FROM @Result;

	EXEC msdb.dbo.sp_send_dbmail
		@profile_name = @Profile,
		@recipients   = @Recipients,
		@subject      = @Subject,
		@body         = @Body,
		@body_format  = 'HTML';

	/* Deja el Job como fallido en el historial si algún archivo no se generó */
	IF @Failures > 0
	BEGIN
		DECLARE @Msg NVARCHAR(2047) = CONCAT(N'Exportación CSV con ', @Failures, N' archivo(s) no generado(s). Notificación enviada.');
		THROW 50012, @Msg, 1;
	END
END
GO
