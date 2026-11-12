/* Control de Cambios 001 - Tarea 1
   Precio Bomba se comporta igual que Oferta en la alarma de Cantidades Minimas:
   - No genera alarma de cantidades minimas.
   - Al marcar Precio Bomba se inactiva la alarma activa.
   - Al desmarcar se recalcula (via el trigger de items que dispara este SP).
   Solo afecta Cantidades Minimas. No toca Sin Stock ni Bodega Local.
   Re-ejecutable: valida estructura, no se aplica dos veces. */
SET NOCOUNT ON;
DECLARE @d NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.SP_Set_Minimum_Quantity_Alarm'));
IF @d IS NULL THROW 50030, N'SP_Set_Minimum_Quantity_Alarm no existe.', 1;
IF CHARINDEX('@IsBombPrice', @d) > 0
BEGIN PRINT 'Ya aplicado: el SP ya excluye Precio Bomba.'; RETURN; END
IF CHARINDEX('@IsSaleOff BIT, @IsDomesticProduct BIT', @d) = 0
   OR CHARINDEX('@IsSaleOff = IS_SALE_OFF,', @d) = 0
   OR CHARINDEX('@IsDomesticProduct = 0 AND @IsSpecialImport = 0 AND @IsSaleOff = 0', @d) = 0
    THROW 50031, N'SP_Set_Minimum_Quantity_Alarm no tiene la estructura esperada; revisar antes de desplegar.', 1;
SET @d = REPLACE(@d, '@IsSaleOff BIT, @IsDomesticProduct BIT', '@IsSaleOff BIT, @IsBombPrice BIT, @IsDomesticProduct BIT');
SET @d = REPLACE(@d, '@IsSaleOff = IS_SALE_OFF,', '@IsSaleOff = IS_SALE_OFF, @IsBombPrice = IS_BOMB_PRICE,');
SET @d = REPLACE(@d, '@IsDomesticProduct = 0 AND @IsSpecialImport = 0 AND @IsSaleOff = 0', '@IsDomesticProduct = 0 AND @IsSpecialImport = 0 AND @IsSaleOff = 0 AND @IsBombPrice = 0');
SET @d = STUFF(@d, PATINDEX('%CREATE%PROCEDURE%', @d), CHARINDEX('PROCEDURE', @d) - PATINDEX('%CREATE%PROCEDURE%', @d), 'ALTER ');
EXEC sys.sp_executesql @d;
PRINT 'SP_Set_Minimum_Quantity_Alarm actualizado: Precio Bomba excluido de cantidades minimas.';
