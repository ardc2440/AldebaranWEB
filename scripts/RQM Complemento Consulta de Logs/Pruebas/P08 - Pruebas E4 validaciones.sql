/* =====================================================================================
   P08 - Pruebas E4: validaciones de SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES y tope 9999.
   Requiere el script 20. Las validaciones rechazan antes de escribir; la prueba del tope crea un artículo
   ficticio QU-9998 dentro de una transacción que se revierte. No deja datos. Resultado: RESULTADO = OK.
   ===================================================================================== */
SET NOCOUNT ON;
DECLARE @c TABLE (ID VARCHAR(5), CODE NVARCHAR(20), QTY INT, ESPERADO NVARCHAR(200));
INSERT @c VALUES
 ('V01', '', 1, N'El código es obligatorio.'),
 ('V02', NULL, 1, N'El código es obligatorio.'),
 ('V03', '   ', 1, N'El código es obligatorio.'),
 ('V04', 'ABCDEF', 1, N'El código puede tener máximo 5 caracteres.'),
 ('V05', 'Q-Z', 1, N'El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).'),
 ('V06', 'Q_Z', 1, N'El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).'),
 ('V07', 'Q%', 1, N'El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).'),
 ('V08', 'Q  Z', 1, N'El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).'),
 ('V09', 'Q.Z', 1, N'El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).'),
 ('V10', 'QZ', 0, N'La cantidad debe estar entre 1 y 1000.'),
 ('V11', 'QZ', -1, N'La cantidad debe estar entre 1 y 1000.'),
 ('V12', 'QZ', 1001, N'La cantidad debe estar entre 1 y 1000.'),
 ('V13', 'QZ', NULL, N'La cantidad debe estar entre 1 y 1000.');
DECLARE @id VARCHAR(5), @code NVARCHAR(20), @qty INT, @esp NVARCHAR(200), @obt NVARCHAR(400);
DECLARE @r TABLE (ID VARCHAR(5), CODE NVARCHAR(20), QTY INT, ESPERADO NVARCHAR(200), OBTENIDO NVARCHAR(400));
DECLARE k CURSOR LOCAL FAST_FORWARD FOR SELECT ID, CODE, QTY, ESPERADO FROM @c;
OPEN k; FETCH k INTO @id, @code, @qty, @esp;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @obt = N'(no rechazó)';
    BEGIN TRY
        DECLARE @t TABLE (a VARCHAR(10), b INT, c VARCHAR(20), d BIT);
        INSERT @t EXEC dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES @code, @qty;
    END TRY
    BEGIN CATCH
        SET @obt = ERROR_MESSAGE();
        IF XACT_STATE() <> 0 ROLLBACK;
    END CATCH
    INSERT @r VALUES (@id, @code, @qty, @esp, @obt);
    FETCH k INTO @id, @code, @qty, @esp;
END
CLOSE k; DEALLOCATE k;
SELECT ID, ISNULL('[' + CODE + ']', 'NULL') CODIGO, ISNULL(CAST(QTY AS VARCHAR), 'NULL') CANT, OBTENIDO, IIF(ESPERADO = OBTENIDO, 'OK', 'FALLA') RESULTADO FROM @r;
SELECT COUNT(*) AS codigos_de_prueba_creados FROM INTERNAL_REFERENCE_CODES WHERE CODE LIKE 'Q%';
GO
/* Tope: usado 9998, se piden 3 -> solo cabe 1 */
SET NOCOUNT ON;
DECLARE @msg NVARCHAR(400) = N'(no rechazó)', @after INT;
BEGIN TRANSACTION;
INSERT INTO items (LINE_ID, INTERNAL_REFERENCE, ITEM_NAME, PROVIDER_REFERENCE, PROVIDER_ITEM_NAME, FOB_COST, CURRENCY_ID, CIF_COST, VOLUME, WEIGHT, FOB_MEASURE_UNIT_ID)
SELECT TOP 1 LINE_ID, 'QU-9998', 'PRUEBA E4', PROVIDER_REFERENCE, PROVIDER_ITEM_NAME, FOB_COST, CURRENCY_ID, CIF_COST, VOLUME, WEIGHT, FOB_MEASURE_UNIT_ID FROM items ORDER BY ITEM_ID;
SAVE TRANSACTION antes;
BEGIN TRY
    DECLARE @t TABLE (a VARCHAR(10), b INT, c VARCHAR(20), d BIT);
    INSERT @t EXEC dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES 'QU', 3;
END TRY
BEGIN CATCH
    SET @msg = ERROR_MESSAGE();
END CATCH
IF XACT_STATE() <> 0 ROLLBACK;
SELECT 'T01' ID, @msg OBTENIDO, IIF(@msg = N'El código QU llega como máximo a QU-9999: solo se pueden generar 1 consecutivo(s) más.', 'OK', 'FALLA') RESULTADO;
SELECT COUNT(*) AS sugerencias_QU_tras_rechazo FROM INTERNAL_REFERENCE_CONSECUTIVES WHERE CODE = 'QU';
GO
