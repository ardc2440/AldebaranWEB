/* Control de Cambios 001 - Reversa Tarea 1
   Deja el SP generando alarma con Precio Bomba y el trigger sin vigilar IS_BOMB_PRICE.
   Deterministica y re-ejecutable.
   =====================================================================================*/
SET NOCOUNT ON;

DECLARE @d NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.SP_Set_Minimum_Quantity_Alarm'));
IF @d IS NOT NULL AND CHARINDEX('@IsBombPrice', @d) > 0
BEGIN
    SET @d = REPLACE(@d, ' AND @IsBombPrice = 0', '');
    SET @d = REPLACE(@d, '@IsSaleOff = IS_SALE_OFF, @IsBombPrice = IS_BOMB_PRICE,', '@IsSaleOff = IS_SALE_OFF,');
    SET @d = REPLACE(@d, '@IsSaleOff BIT, @IsBombPrice BIT, @IsDomesticProduct BIT', '@IsSaleOff BIT, @IsDomesticProduct BIT');
    SET @d = STUFF(@d, PATINDEX('%CREATE%PROCEDURE%', @d), CHARINDEX('PROCEDURE', @d) - PATINDEX('%CREATE%PROCEDURE%', @d), 'ALTER ');
    EXEC sys.sp_executesql @d;
    PRINT 'Reversado SP_Set_Minimum_Quantity_Alarm.';
END
ELSE PRINT 'SP ya estaba revertido.';

DECLARE @t NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.Trg_Is_Special_Import'));
IF @t IS NOT NULL AND CHARINDEX('UPDATE(IS_BOMB_PRICE)', @t) > 0
BEGIN
    SET @t = REPLACE(@t, ' OR UPDATE(IS_BOMB_PRICE)', '');
    SET @t = STUFF(@t, PATINDEX('%CREATE%TRIGGER%', @t), CHARINDEX('TRIGGER', @t) - PATINDEX('%CREATE%TRIGGER%', @t), 'ALTER ');
    EXEC sys.sp_executesql @t;
    PRINT 'Reversado Trg_Is_Special_Import.';
END
ELSE PRINT 'Trigger ya estaba revertido.';

/* Recalcular las referencias de los articulos Precio Bomba para que, ya sin la exclusion,
   se regeneren las alarmas de cantidades minimas que correspondan. */
DECLARE @Refs TABLE (RowNum INT IDENTITY(1,1), ReferenceId INT);
INSERT INTO @Refs (ReferenceId)
    SELECT r.REFERENCE_ID FROM item_references r JOIN items i ON i.ITEM_ID = r.ITEM_ID
     WHERE i.IS_BOMB_PRICE = 1 AND r.ALARM_MINIMUM_QUANTITY > 0;
DECLARE @n INT = 1, @tot INT = (SELECT MAX(RowNum) FROM @Refs), @ref INT;
WHILE @n <= @tot
BEGIN
    SELECT @ref = ReferenceId FROM @Refs WHERE RowNum = @n;
    EXEC dbo.SP_Set_Minimum_Quantity_Alarm @REFERENCE_ID = @ref, @SOURCE = 'Reversa Precio Bomba';
    SET @n += 1;
END
PRINT CONCAT('Reversa: referencias Bomba recalculadas = ', ISNULL(@tot,0));
