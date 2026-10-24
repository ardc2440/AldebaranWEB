/* =====================================================================================
   RQM Complemento Consulta de Logs - T23: Marca "Pedido especial" en pedidos
   -------------------------------------------------------------------------------------
   - customer_orders.IS_SPECIAL_ORDER: nueva marca del pedido (default 0). Los pedidos
     existentes quedan como "No". No cambia ninguna lógica actual.
   - LOG_TRACK_FIELD_NAMES: nombre de usuario del campo en Track.
   Script re-ejecutable. Reversa: Reversa\17 - Reversa pedido especial.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.customer_orders', 'IS_SPECIAL_ORDER') IS NULL
BEGIN
    ALTER TABLE dbo.customer_orders ADD IS_SPECIAL_ORDER BIT NOT NULL CONSTRAINT DF_CUSTOMER_ORDERS_IS_SPECIAL_ORDER DEFAULT (0);
    PRINT 'customer_orders.IS_SPECIAL_ORDER creada';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LOG_TRACK_FIELD_NAMES WHERE ENTITY_NAME = '' AND FIELD_NAME = 'IsSpecialOrder')
    INSERT INTO dbo.LOG_TRACK_FIELD_NAMES (ENTITY_NAME, FIELD_NAME, DISPLAY_NAME, IS_TECHNICAL)
    VALUES ('', 'IsSpecialOrder', N'Pedido especial', 0);
GO
