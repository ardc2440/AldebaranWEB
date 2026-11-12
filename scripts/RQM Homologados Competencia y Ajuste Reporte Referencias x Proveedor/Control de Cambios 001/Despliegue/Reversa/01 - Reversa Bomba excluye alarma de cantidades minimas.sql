/* Control de Cambios 001 - Reversa Tarea 1
   Quita la exclusion de Precio Bomba y deja el SP generando alarma como antes.
   Determinística y re-ejecutable. */
SET NOCOUNT ON;
DECLARE @d NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.SP_Set_Minimum_Quantity_Alarm'));
IF @d IS NULL THROW 50030, N'SP_Set_Minimum_Quantity_Alarm no existe.', 1;
IF CHARINDEX('@IsBombPrice', @d) = 0
BEGIN PRINT 'Ya revertido.'; RETURN; END
SET @d = REPLACE(@d, ' AND @IsBombPrice = 0', '');
SET @d = REPLACE(@d, '@IsSaleOff = IS_SALE_OFF, @IsBombPrice = IS_BOMB_PRICE,', '@IsSaleOff = IS_SALE_OFF,');
SET @d = REPLACE(@d, '@IsSaleOff BIT, @IsBombPrice BIT, @IsDomesticProduct BIT', '@IsSaleOff BIT, @IsDomesticProduct BIT');
SET @d = STUFF(@d, PATINDEX('%CREATE%PROCEDURE%', @d), CHARINDEX('PROCEDURE', @d) - PATINDEX('%CREATE%PROCEDURE%', @d), 'ALTER ');
EXEC sys.sp_executesql @d;
PRINT 'Reversado: SP_Set_Minimum_Quantity_Alarm vuelve a generar alarma con Precio Bomba.';
