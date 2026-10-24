/* =====================================================================================
   P07 - Pruebas E4: reconocimiento de consecutivos usados y generación de sugerencias.
   Requiere el script 20. Crea artículos ficticios (códigos QZ, QY, QW, QV, QABCD, Q Z, QT) y genera
   sugerencias DENTRO DE UNA TRANSACCIÓN QUE SE REVIERTE: no deja datos. Resultado: columna RESULTADO = OK.
   ===================================================================================== */
SET NOCOUNT ON; SET XACT_ABORT OFF;
IF OBJECT_ID('tempdb..#R') IS NOT NULL DROP TABLE #R;
CREATE TABLE #R (ID VARCHAR(10), CASO NVARCHAR(200), ESPERADO NVARCHAR(200), OBTENIDO NVARCHAR(200));
GO
CREATE OR ALTER PROCEDURE #AddItem @Ref VARCHAR(10) AS
    INSERT INTO items (LINE_ID, INTERNAL_REFERENCE, ITEM_NAME, PROVIDER_REFERENCE, PROVIDER_ITEM_NAME, FOB_COST, CURRENCY_ID, CIF_COST, VOLUME, WEIGHT, FOB_MEASURE_UNIT_ID)
    SELECT TOP 1 LINE_ID, @Ref, 'PRUEBA E4 ' + @Ref, PROVIDER_REFERENCE, PROVIDER_ITEM_NAME, FOB_COST, CURRENCY_ID, CIF_COST, VOLUME, WEIGHT, FOB_MEASURE_UNIT_ID FROM items ORDER BY ITEM_ID;
GO
CREATE OR ALTER PROCEDURE #Check @Id VARCHAR(10), @Caso NVARCHAR(200), @Esperado NVARCHAR(200), @Obtenido NVARCHAR(200) AS
    INSERT #R VALUES (@Id, @Caso, @Esperado, ISNULL(@Obtenido, 'NULL'));
GO
/* Lista de disponibles (Consultar) como texto */
CREATE OR ALTER PROCEDURE #Avail @Code VARCHAR(10), @Out NVARCHAR(400) OUTPUT AS
BEGIN
    DECLARE @t TABLE (Code VARCHAR(10), Consecutive INT, InternalReference VARCHAR(20), IsNew BIT);
    INSERT @t EXEC dbo.SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES @Code;
    SET @Out = ISNULL((SELECT STRING_AGG(InternalReference, ',') WITHIN GROUP (ORDER BY Consecutive) FROM @t), '(vacío)');
END
GO
/* Generar: devuelve disponibles y nuevos como texto */
CREATE OR ALTER PROCEDURE #Gen @Code VARCHAR(10), @Qty INT, @Out NVARCHAR(400) OUTPUT AS
BEGIN
    DECLARE @t TABLE (Code VARCHAR(10), Consecutive INT, InternalReference VARCHAR(20), IsNew BIT);
    INSERT @t EXEC dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES @Code, @Qty;
    SET @Out = ISNULL((SELECT STRING_AGG(InternalReference + IIF(IsNew = 1, '*', ''), ',') WITHIN GROUP (ORDER BY Consecutive) FROM @t), '(vacío)');
END
GO

BEGIN TRANSACTION;
DECLARE @o NVARCHAR(400), @n INT;

/* ---------- R: reconocimiento de consecutivos usados ---------- */
EXEC #AddItem 'QZ-1';  EXEC #AddItem 'QZ-025'; EXEC #AddItem 'QZ-0251'; EXEC #AddItem 'QZ-12345';
EXEC #AddItem 'QZ-';   EXEC #AddItem 'QZ-ABC'; EXEC #AddItem 'QZ-25X';  EXEC #AddItem 'QZ-984SURA';
EXEC #AddItem 'QZ-30-1'; EXEC #AddItem 'QZ-31-ABC'; EXEC #AddItem 'QZX-99'; EXEC #AddItem 'QZ 77';
EXEC #AddItem 'QZ77'; EXEC #AddItem 'qz-40'; EXEC #AddItem 'QZ-1 ROJO'; EXEC #AddItem 'QZ--5';
DECLARE @f TABLE (REF VARCHAR(10), C INT, REASON NVARCHAR(200));
INSERT @f SELECT INTERNAL_REFERENCE, CONSECUTIVE, REASON FROM dbo.FN_INTERNAL_REFERENCE_CODE_REFERENCES('QZ');
INSERT #R SELECT 'R01', 'QZ-1 (1 dígito)', '1', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-1')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R02', 'QZ-025 (ceros a la izquierda)', '25', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-025')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R03', 'QZ-0251 (4 dígitos con cero)', '251', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-0251')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R04', 'QZ-12345 (5 dígitos)', N'NULL | más de 4 dígitos', ISNULL(CAST(((SELECT ISNULL(CAST(C AS VARCHAR),'NULL') + ' | ' + IIF(REASON LIKE N'%más de 4%', N'más de 4 dígitos', REASON) FROM @f WHERE REF = 'QZ-12345')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R05', 'QZ- (nada después del guion)', N'NULL | no hay nada', ISNULL(CAST(((SELECT ISNULL(CAST(C AS VARCHAR),'NULL') + ' | ' + IIF(REASON LIKE N'%no hay nada%', N'no hay nada', REASON) FROM @f WHERE REF = 'QZ-')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R06', 'QZ-ABC (sin número)', N'NULL | no hay un número', ISNULL(CAST(((SELECT ISNULL(CAST(C AS VARCHAR),'NULL') + ' | ' + IIF(REASON LIKE N'%no hay un número%', N'no hay un número', REASON) FROM @f WHERE REF = 'QZ-ABC')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R07', 'QZ-25X (complemento pegado)', '25', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-25X')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R08', 'QZ-984SURA (complemento pegado largo)', '984', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-984SURA')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R09', 'QZ-30-1 (complemento con guion)', '30', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-30-1')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R10', 'QZ-31-ABC (complemento con guion y letras)', '31', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-31-ABC')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R11', 'QZX-99 no pertenece a QZ', '0', ISNULL(CAST(((SELECT CAST(COUNT(*) AS VARCHAR) FROM @f WHERE REF = 'QZX-99')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R12', 'QZ 77 (espacio en vez de guion) no pertenece a QZ', '0', ISNULL(CAST(((SELECT CAST(COUNT(*) AS VARCHAR) FROM @f WHERE REF = 'QZ 77')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R13', 'QZ77 (sin guion) no pertenece a QZ', '0', ISNULL(CAST(((SELECT CAST(COUNT(*) AS VARCHAR) FROM @f WHERE REF = 'QZ77')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R14', 'qz-40 (minúsculas) cuenta', '40', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'qz-40')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R15', 'QZ-1 ROJO (complemento con espacio)', '1', ISNULL(CAST(((SELECT CAST(C AS VARCHAR) FROM @f WHERE REF = 'QZ-1 ROJO')) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'R16', 'QZ--5 (doble guion)', N'NULL | no hay un número', ISNULL(CAST(((SELECT ISNULL(CAST(C AS VARCHAR),'NULL') + ' | ' + IIF(REASON LIKE N'%no hay un número%', N'no hay un número', REASON) FROM @f WHERE REF = 'QZ--5')) AS NVARCHAR(200)), 'NULL');
SELECT @n = COUNT(*) FROM @f WHERE C IS NULL;
INSERT #R SELECT 'R17', 'No conformes de QZ (QZ-12345, QZ-, QZ-ABC, QZ--5)', '4', ISNULL(CAST((CAST(@n AS VARCHAR)) AS NVARCHAR(200)), 'NULL');
SET @n = NULL; DECLARE @nc TABLE (InternalReference VARCHAR(10), ItemName VARCHAR(100), Reason NVARCHAR(200));
INSERT @nc EXEC dbo.SP_GET_INTERNAL_REFERENCE_NONCONFORMING 'qz';
INSERT #R SELECT 'R18', 'SP de no conformes con código en minúsculas', '4', ISNULL(CAST(((SELECT CAST(COUNT(*) AS VARCHAR) FROM @nc)) AS NVARCHAR(200)), 'NULL');

/* ---------- G: generación sobre código nuevo QY ---------- */
EXEC #Avail 'QY', @o OUTPUT;            INSERT #R SELECT 'G01', 'Consultar código nuevo sin sugerencias', N'(vacío)', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #Gen 'qy', 3, @o OUTPUT;            INSERT #R SELECT 'G02', 'Generar 3 en código nuevo (minúsculas)', 'QY-001*,QY-002*,QY-003*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #Gen 'QY', 3, @o OUTPUT;            INSERT #R SELECT 'G03', 'Generar 3 otra vez: ya hay 3, no crea', 'QY-001,QY-002,QY-003', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #Gen 'QY', 5, @o OUTPUT;            INSERT #R SELECT 'G04', 'Generar 5: completa los 2 que faltan', 'QY-001,QY-002,QY-003,QY-004*,QY-005*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #Gen 'QY', 2, @o OUTPUT;            INSERT #R SELECT 'G05', 'Generar 2 con 5 disponibles: no borra ni crea', 'QY-001,QY-002,QY-003,QY-004,QY-005', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY-002';
EXEC #Avail 'QY', @o OUTPUT;            INSERT #R SELECT 'G06', 'Artículo creado con la sugerencia QY-002: sale de disponibles', 'QY-001,QY-003,QY-004,QY-005', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY-003-ROJO';
EXEC #Avail 'QY', @o OUTPUT;            INSERT #R SELECT 'G07', 'Artículo creado con QY-003-ROJO (complemento): 003 sale', 'QY-001,QY-004,QY-005', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY4';
EXEC #Avail 'QY', @o OUTPUT;            INSERT #R SELECT 'G08', 'Artículo creado como QY4 (sin guion): 004 NO sale', 'QY-001,QY-004,QY-005', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY-050';
EXEC #Gen 'QY', 4, @o OUTPUT;            INSERT #R SELECT 'G09', 'Artículo QY-050 fuera de lo sugerido: el siguiente salta a 051', 'QY-001,QY-004,QY-005,QY-051*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY-5';
EXEC #Avail 'QY', @o OUTPUT;            INSERT #R SELECT 'G10', 'Artículo QY-5 (sin ceros) equivale a QY-005', 'QY-001,QY-004,QY-051', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY-ESPECIAL';
EXEC #Gen 'QY', 4, @o OUTPUT;            INSERT #R SELECT 'G11', 'QY-ESPECIAL (no conforme) no afecta el siguiente', 'QY-001,QY-004,QY-051,QY-052*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QY-12345';
EXEC #Gen 'QY', 5, @o OUTPUT;            INSERT #R SELECT 'G12', 'QY-12345 (5 dígitos, no conforme) no afecta el siguiente', 'QY-001,QY-004,QY-051,QY-052,QY-053*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');

/* ---------- D: paso a 4 dígitos y tope 9999 ---------- */
EXEC #AddItem 'QW-998';
EXEC #Gen 'QW', 3, @o OUTPUT;            INSERT #R SELECT 'D01', 'Usado 998: genera 999, 1000, 1001 (4 dígitos sin ceros)', 'QW-999*,QW-1000*,QW-1001*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QW-1000';
EXEC #Avail 'QW', @o OUTPUT;            INSERT #R SELECT 'D02', 'Artículo QW-1000: sale de disponibles', 'QW-999,QW-1001', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QV-9997';
EXEC #Gen 'QV', 2, @o OUTPUT;            INSERT #R SELECT 'D03', 'Usado 9997: genera 9998 y 9999 (tope exacto)', 'QV-9998*,QV-9999*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #AddItem 'QABCD-1';
EXEC #Gen 'QABCD', 1, @o OUTPUT;         INSERT #R SELECT 'D04', 'Código de 5 caracteres', 'QABCD-002*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
INSERT #R SELECT 'D05', 'Largo de QABCD-9999 cabe en varchar(10)', '10', ISNULL(CAST((CAST(LEN('QABCD-9999') AS VARCHAR)) AS NVARCHAR(200)), 'NULL');

/* ---------- E: códigos con espacio y con signos ---------- */
EXEC #AddItem 'Q Z-010';
EXEC #Gen 'q z', 1, @o OUTPUT;           INSERT #R SELECT 'E01', 'Código con espacio interno (Q Z)', 'Q Z-011*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');
EXEC #Gen '  QT  ', 1, @o OUTPUT;        INSERT #R SELECT 'E02', 'Código con espacios al inicio/fin se recorta', 'QT-001*', ISNULL(CAST((@o) AS NVARCHAR(200)), 'NULL');

SELECT ID, CASO, ESPERADO, OBTENIDO, IIF(ESPERADO = OBTENIDO, 'OK', 'FALLA') RESULTADO FROM #R ORDER BY ID;
ROLLBACK TRANSACTION;
GO
