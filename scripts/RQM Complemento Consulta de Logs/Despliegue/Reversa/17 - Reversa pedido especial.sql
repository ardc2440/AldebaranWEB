/* Reversa T23: elimina customer_orders.IS_SPECIAL_ORDER y su nombre en Track.
   Antes de ejecutar, publicar la versión de la aplicación sin la marca Pedido especial. */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DELETE FROM dbo.LOG_TRACK_FIELD_NAMES WHERE ENTITY_NAME = '' AND FIELD_NAME = 'IsSpecialOrder';
GO

IF COL_LENGTH('dbo.customer_orders', 'IS_SPECIAL_ORDER') IS NOT NULL
BEGIN
    ALTER TABLE dbo.customer_orders DROP CONSTRAINT DF_CUSTOMER_ORDERS_IS_SPECIAL_ORDER;
    ALTER TABLE dbo.customer_orders DROP COLUMN IS_SPECIAL_ORDER;
END
GO
