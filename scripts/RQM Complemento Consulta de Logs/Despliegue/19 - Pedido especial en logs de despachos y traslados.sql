/* =====================================================================================
   RQM Complemento Consulta de Logs - T27: Pedido especial en logs de despachos y traslados a proceso
   -------------------------------------------------------------------------------------
   SP_GET_ORDER_DOCUMENT_LOG (script 10): agrega la columna IS_SPECIAL_ORDER del pedido al resultado.
   Requiere los scripts 10 y 17. Se modifica sobre la definición vigente (solo si aún no la tiene),
   validando su estructura. Se despliega junto con la aplicación.
   Script re-ejecutable. Reversa: Reversa\19 - Reversa pedido especial en logs de despachos y traslados.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.customer_orders', 'IS_SPECIAL_ORDER') IS NULL
    THROW 50040, N'Falta customer_orders.IS_SPECIAL_ORDER: ejecute primero el script 17.', 1;
GO

DECLARE @d NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.SP_GET_ORDER_DOCUMENT_LOG'));
IF @d IS NULL OR (CHARINDEX('IS_SPECIAL_ORDER', @d) = 0 AND CHARINDEX('o.ORDER_NUMBER, c.CUSTOMER_NAME,', @d) = 0)
    THROW 50041, N'SP_GET_ORDER_DOCUMENT_LOG no tiene la estructura esperada; ejecute el script 10 y revise antes de desplegar.', 1;

IF CHARINDEX('IS_SPECIAL_ORDER', @d) = 0
BEGIN
    SET @d = REPLACE(@d, 'o.ORDER_NUMBER, c.CUSTOMER_NAME,', 'o.ORDER_NUMBER, o.IS_SPECIAL_ORDER, c.CUSTOMER_NAME,');
    SET @d = STUFF(@d, PATINDEX('%CREATE%PROCEDURE%', @d), CHARINDEX('PROCEDURE', @d) - PATINDEX('%CREATE%PROCEDURE%', @d), 'ALTER ');
    EXEC sys.sp_executesql @d;
    PRINT 'SP_GET_ORDER_DOCUMENT_LOG actualizado';
END
GO
