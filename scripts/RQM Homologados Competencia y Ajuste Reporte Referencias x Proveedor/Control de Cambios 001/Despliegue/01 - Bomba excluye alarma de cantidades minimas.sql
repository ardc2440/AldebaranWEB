/* Control de Cambios 001 - Tarea 1
   Precio Bomba se comporta igual que Oferta en la alarma de Cantidades Minimas:
   - No genera alarma de cantidades minimas (SP_Set_Minimum_Quantity_Alarm).
   - Al marcar/desmarcar Precio Bomba en el articulo se recalcula la alarma
     (Trg_Is_Special_Import, igual que ya ocurre con Oferta / Importacion especial / Nacional).
   Solo afecta Cantidades Minimas. No toca Sin Stock ni Bodega Local.
   Re-ejecutable: valida estructura, no se aplica dos veces.
   =====================================================================================*/
SET NOCOUNT ON;

/* 1) SP: excluir Precio Bomba de la generacion de alarma (misma compuerta de Oferta) */
DECLARE @d NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.SP_Set_Minimum_Quantity_Alarm'));
IF @d IS NULL THROW 50030, N'SP_Set_Minimum_Quantity_Alarm no existe.', 1;
IF CHARINDEX('@IsBombPrice', @d) = 0
BEGIN
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
END
ELSE PRINT 'SP_Set_Minimum_Quantity_Alarm ya estaba actualizado.';

/* 2) Trigger: recalcular alarmas tambien cuando cambia IS_BOMB_PRICE (igual que IS_SALE_OFF) */
DECLARE @t NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.Trg_Is_Special_Import'));
IF @t IS NULL THROW 50040, N'Trg_Is_Special_Import no existe.', 1;
IF CHARINDEX('UPDATE(IS_BOMB_PRICE)', @t) = 0
BEGIN
    IF CHARINDEX('UPDATE(IS_SPECIAL_IMPORT) OR UPDATE(IS_DOMESTIC_PRODUCT) OR UPDATE(IS_SALE_OFF)', @t) = 0
        THROW 50041, N'Trg_Is_Special_Import no tiene la estructura esperada; revisar antes de desplegar.', 1;
    SET @t = REPLACE(@t, 'UPDATE(IS_SPECIAL_IMPORT) OR UPDATE(IS_DOMESTIC_PRODUCT) OR UPDATE(IS_SALE_OFF)',
                         'UPDATE(IS_SPECIAL_IMPORT) OR UPDATE(IS_DOMESTIC_PRODUCT) OR UPDATE(IS_SALE_OFF) OR UPDATE(IS_BOMB_PRICE)');
    SET @t = STUFF(@t, PATINDEX('%CREATE%TRIGGER%', @t), CHARINDEX('TRIGGER', @t) - PATINDEX('%CREATE%TRIGGER%', @t), 'ALTER ');
    EXEC sys.sp_executesql @t;
    PRINT 'Trg_Is_Special_Import actualizado: recalcula alarma al cambiar Precio Bomba.';
END
ELSE PRINT 'Trg_Is_Special_Import ya estaba actualizado.';

/* 3) Correccion masiva (una sola vez, re-ejecutable): inactivar las alarmas de cantidades minimas
   ya existentes de los articulos que HOY estan marcados como Precio Bomba.
   Se recalcula cada referencia; como el SP ya excluye Bomba, las alarmas activas se inactivan.
   Sin IDs fijos (produccion puede tener mas datos que pruebas). */
DECLARE @Refs TABLE (RowNum INT IDENTITY(1,1), ReferenceId INT);
INSERT INTO @Refs (ReferenceId)
    SELECT r.REFERENCE_ID
      FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID
     WHERE i.IS_BOMB_PRICE = 1 AND r.ALARM_MINIMUM_QUANTITY > 0;
DECLARE @n INT = 1, @tot INT = (SELECT MAX(RowNum) FROM @Refs), @ref INT;
WHILE @n <= @tot
BEGIN
    SELECT @ref = ReferenceId FROM @Refs WHERE RowNum = @n;
    EXEC dbo.SP_Set_Minimum_Quantity_Alarm @REFERENCE_ID = @ref, @SOURCE = 'Correccion Precio Bomba';
    SET @n += 1;
END
PRINT CONCAT('Correccion Precio Bomba: referencias evaluadas = ', ISNULL(@tot,0));
