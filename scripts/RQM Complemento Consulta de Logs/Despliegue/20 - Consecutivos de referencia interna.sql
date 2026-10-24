/* =====================================================================================
   RQM Complemento Consulta de Logs - E4 C1: Consecutivos de referencia interna
   -------------------------------------------------------------------------------------
   SUGIERE la nomenclatura inicial de artículos: CÓDIGO-NNN (mínimo 3 dígitos con ceros a la izquierda;
   desde 1000, 4 dígitos: VA-1227). Máximo CÓDIGO-9999.
   Los consecutivos son SUGERENCIAS: no se reservan ni se enlazan con la creación del artículo. La
   referencia válida es la que finalmente se guarda en el artículo (items.INTERNAL_REFERENCE).
   - INTERNAL_REFERENCE_CODES: códigos (mayúsculas; letras, números y un espacio entre palabras; sin guion;
     máx. 5 caracteres para que CÓDIGO + '-' + 4 dígitos quepa en items.INTERNAL_REFERENCE varchar(10)).
   - INTERNAL_REFERENCE_CONSECUTIVES: SOLO sugerencias aún no usadas. Las que ya existen como referencia
     interna de un artículo se eliminan (SP_PURGE_USED_...) al Consultar y al Generar.
   - SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES: depura y devuelve las disponibles.
   - SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES: completa hasta @Quantity disponibles; las nuevas siguen al
     mayor entre las disponibles y los consecutivos usados en items + 1 (no rellena huecos). Transaccional y
     con bloqueo por código (dos usuarios nunca reciben el mismo número).
   Consecutivo usado en artículos (ver FN_INTERNAL_REFERENCE_CODE_REFERENCES): la referencia empieza por
   'CÓDIGO-' y sigue con 1 a 4 dígitos; después puede venir cualquier complemento (VA-1015-1, CP-59T,
   VA-984SURA). Las que empiezan por el código pero no cumplen se informan con su motivo.
   No modifica la creación de artículos.
   Script re-ejecutable (también sobre la versión anterior de este script, con códigos de 6 caracteres).
   Reversa: Reversa\20 - Reversa consecutivos de referencia interna.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.INTERNAL_REFERENCE_CODES') IS NULL
    CREATE TABLE dbo.INTERNAL_REFERENCE_CODES
    (
        CODE VARCHAR(5) NOT NULL CONSTRAINT PK_INTERNAL_REFERENCE_CODES PRIMARY KEY
    );
GO

IF OBJECT_ID('dbo.INTERNAL_REFERENCE_CONSECUTIVES') IS NULL
    CREATE TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES
    (
        CODE        VARCHAR(5) NOT NULL CONSTRAINT FK_INTERNAL_REFERENCE_CONSECUTIVES_CODE REFERENCES dbo.INTERNAL_REFERENCE_CODES (CODE),
        CONSECUTIVE INT        NOT NULL CONSTRAINT CK_INTERNAL_REFERENCE_CONSECUTIVES_POSITIVE CHECK (CONSECUTIVE > 0),
        CONSTRAINT PK_INTERNAL_REFERENCE_CONSECUTIVES PRIMARY KEY (CODE, CONSECUTIVE)
    );
GO

/* Versión anterior (CODE varchar(6), consecutivo de 3 dígitos): ajusta las columnas a varchar(5).
   Los códigos de 6 caracteres (solo pruebas) se eliminan con sus sugerencias. */
IF COL_LENGTH('dbo.INTERNAL_REFERENCE_CODES', 'CODE') = 6
BEGIN
    BEGIN TRANSACTION;
        DELETE FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES WHERE LEN(CODE) > 5;
        DELETE FROM dbo.INTERNAL_REFERENCE_CODES WHERE LEN(CODE) > 5;
        ALTER TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES DROP CONSTRAINT FK_INTERNAL_REFERENCE_CONSECUTIVES_CODE;
        ALTER TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES DROP CONSTRAINT PK_INTERNAL_REFERENCE_CONSECUTIVES;
        ALTER TABLE dbo.INTERNAL_REFERENCE_CODES DROP CONSTRAINT PK_INTERNAL_REFERENCE_CODES;
        ALTER TABLE dbo.INTERNAL_REFERENCE_CODES ALTER COLUMN CODE VARCHAR(5) NOT NULL;
        ALTER TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES ALTER COLUMN CODE VARCHAR(5) NOT NULL;
        ALTER TABLE dbo.INTERNAL_REFERENCE_CODES ADD CONSTRAINT PK_INTERNAL_REFERENCE_CODES PRIMARY KEY (CODE);
        ALTER TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES ADD CONSTRAINT PK_INTERNAL_REFERENCE_CONSECUTIVES PRIMARY KEY (CODE, CONSECUTIVE);
        ALTER TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES ADD CONSTRAINT FK_INTERNAL_REFERENCE_CONSECUTIVES_CODE
            FOREIGN KEY (CODE) REFERENCES dbo.INTERNAL_REFERENCE_CODES (CODE);
    COMMIT TRANSACTION;
    PRINT 'INTERNAL_REFERENCE_CODES / CONSECUTIVES: CODE ajustado a varchar(5)';
END
GO

/* Referencias de items que empiezan por 'CÓDIGO-' (LIKE con el código escapado: '%', '_' y '[' no son
   comodines; el guion evita mezclar MU con MUX) y su consecutivo:
   - Es consecutivo si después del guion hay de 1 a 4 dígitos; lo que siga es complemento libre:
     MU-5 = 5, MU-025 = 25, MU-25-1 = 25, CP-59T = 59, VA-984SURA = 984, VA-1015-1 = 1015, MU-0251 = 251.
   - Lo demás (MU-ABC, MU-, MU-12345) NO es consecutivo: CONSECUTIVE = NULL y REASON explica por qué.
   Las consultas de uso filtran por CONSECUTIVE (los NULL no cuentan). */
CREATE OR ALTER FUNCTION dbo.FN_INTERNAL_REFERENCE_CODE_REFERENCES (@Code VARCHAR(10))
RETURNS TABLE
AS
RETURN
    SELECT i.INTERNAL_REFERENCE,
           i.ITEM_NAME,
           CASE WHEN LEN(d.DIGITS) BETWEEN 1 AND 4 THEN CAST(d.DIGITS AS INT) END AS CONSECUTIVE,
           CASE WHEN LEN(d.DIGITS) BETWEEN 1 AND 4 THEN NULL
                WHEN LEN(r.REST) = 0 THEN 'Después del guion no hay nada.'
                WHEN LEN(d.DIGITS) = 0 THEN 'Después del guion no hay un número (sigue "' + LEFT(r.REST, 1) + '").'
                ELSE 'Después del guion hay más de 4 dígitos.'
           END AS REASON
      FROM dbo.items i
     CROSS APPLY (SELECT SUBSTRING(i.INTERNAL_REFERENCE, LEN(@Code) + 2, 10) AS REST) r
     CROSS APPLY (SELECT LEFT(r.REST, PATINDEX('%[^0-9]%', r.REST + 'x') - 1) AS DIGITS) d
     WHERE i.INTERNAL_REFERENCE LIKE REPLACE(REPLACE(REPLACE(REPLACE(@Code, '\', '\\'), '%', '\%'), '_', '\_'), '[', '\[') + '-%' ESCAPE '\';
GO

/* Consecutivos usados en items para un código (solo las referencias que cumplen el formato) */
CREATE OR ALTER FUNCTION dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES (@Code VARCHAR(10))
RETURNS TABLE
AS
RETURN
    SELECT f.CONSECUTIVE
      FROM dbo.FN_INTERNAL_REFERENCE_CODE_REFERENCES(@Code) f
     WHERE f.CONSECUTIVE IS NOT NULL;
GO

/* Referencias del código que no se toman como consecutivo, con el motivo (explican por qué el siguiente
   consecutivo no sigue a esas referencias) */
CREATE OR ALTER PROCEDURE dbo.SP_GET_INTERNAL_REFERENCE_NONCONFORMING
    @Code VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Code = UPPER(LTRIM(RTRIM(@Code)));

    SELECT f.INTERNAL_REFERENCE AS InternalReference,
           f.ITEM_NAME AS ItemName,
           f.REASON AS Reason
      FROM dbo.FN_INTERNAL_REFERENCE_CODE_REFERENCES(@Code) f
     WHERE f.CONSECUTIVE IS NULL
     ORDER BY f.INTERNAL_REFERENCE;
END
GO

/* Elimina las sugerencias que ya se usaron como referencia interna de un artículo: la tabla solo conserva disponibles */
CREATE OR ALTER PROCEDURE dbo.SP_PURGE_USED_INTERNAL_REFERENCE_CONSECUTIVES
    @Code VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE c
      FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c
     WHERE c.CODE = @Code
       AND EXISTS (SELECT 1 FROM dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES(@Code) u WHERE u.CONSECUTIVE = c.CONSECUTIVE);
END
GO

/* Consultar: depura las usadas y devuelve las disponibles */
CREATE OR ALTER PROCEDURE dbo.SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES
    @Code VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Code = UPPER(LTRIM(RTRIM(@Code)));

    BEGIN TRANSACTION;
        EXEC dbo.SP_PURGE_USED_INTERNAL_REFERENCE_CONSECUTIVES @Code;
    COMMIT TRANSACTION;

    SELECT c.CODE AS Code,
           c.CONSECUTIVE AS Consecutive,
           c.CODE + '-' + CASE WHEN c.CONSECUTIVE < 1000 THEN RIGHT('00' + CAST(c.CONSECUTIVE AS VARCHAR(10)), 3) ELSE CAST(c.CONSECUTIVE AS VARCHAR(10)) END AS InternalReference,
           CAST(0 AS BIT) AS IsNew
      FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c
     WHERE c.CODE = @Code
     ORDER BY c.CONSECUTIVE;
END
GO

/* Generar: completa hasta @Quantity disponibles. Crea (@Quantity - disponibles) consecutivos seguidos a partir
   del mayor entre los disponibles y los usados en artículos + 1, sin pasar de 9999. Devuelve todos los
   disponibles (IsNew = 1 los creados). Las validaciones repiten las de la aplicación (defensa en la BD). */
CREATE OR ALTER PROCEDURE dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES
    @Code     VARCHAR(10),
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Code = UPPER(LTRIM(RTRIM(@Code)));

    IF @Code IS NULL OR LEN(@Code) = 0
        THROW 50100, N'El código es obligatorio.', 1;
    IF LEN(@Code) > 5
        THROW 50103, N'El código puede tener máximo 5 caracteres.', 1;
    IF @Code LIKE '%[^A-Z0-9Ñ ]%' OR CHARINDEX('  ', @Code) > 0
        THROW 50104, N'El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).', 1;
    IF @Quantity IS NULL OR @Quantity < 1 OR @Quantity > 1000
        THROW 50101, N'La cantidad debe estar entre 1 y 1000.', 1;

    DECLARE @Last INT, @Available INT, @ToCreate INT, @Msg NVARCHAR(400);

    BEGIN TRANSACTION;

        /* Bloqueo por código: serializa generaciones concurrentes del mismo código */
        IF NOT EXISTS (SELECT 1 FROM dbo.INTERNAL_REFERENCE_CODES WITH (UPDLOCK, HOLDLOCK) WHERE CODE = @Code)
            INSERT INTO dbo.INTERNAL_REFERENCE_CODES (CODE) VALUES (@Code);

        EXEC dbo.SP_PURGE_USED_INTERNAL_REFERENCE_CONSECUTIVES @Code;

        SELECT @Available = COUNT(*) FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES WITH (UPDLOCK, HOLDLOCK) WHERE CODE = @Code;
        SET @ToCreate = CASE WHEN @Quantity > @Available THEN @Quantity - @Available ELSE 0 END;

        SELECT @Last = MAX(x.CONSECUTIVE)
          FROM (SELECT c.CONSECUTIVE FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c WHERE c.CODE = @Code
                UNION ALL
                SELECT u.CONSECUTIVE FROM dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES(@Code) u) x;
        SET @Last = ISNULL(@Last, 0);

        IF @Last + @ToCreate > 9999
        BEGIN
            SET @Msg = N'El código ' + @Code + N' llega como máximo a ' + @Code + N'-9999: solo se pueden generar '
                     + CAST(9999 - @Last AS NVARCHAR(10)) + N' consecutivo(s) más.';
            THROW 50102, @Msg, 1;
        END

        IF @ToCreate > 0
        BEGIN
            ;WITH n AS (SELECT TOP (@ToCreate) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS k
                          FROM sys.all_objects a CROSS JOIN sys.all_objects b)
            INSERT INTO dbo.INTERNAL_REFERENCE_CONSECUTIVES (CODE, CONSECUTIVE)
            SELECT @Code, @Last + k FROM n;
        END

    COMMIT TRANSACTION;

    SELECT c.CODE AS Code,
           c.CONSECUTIVE AS Consecutive,
           c.CODE + '-' + CASE WHEN c.CONSECUTIVE < 1000 THEN RIGHT('00' + CAST(c.CONSECUTIVE AS VARCHAR(10)), 3) ELSE CAST(c.CONSECUTIVE AS VARCHAR(10)) END AS InternalReference,
           CAST(CASE WHEN c.CONSECUTIVE > @Last THEN 1 ELSE 0 END AS BIT) AS IsNew
      FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c
     WHERE c.CODE = @Code
     ORDER BY c.CONSECUTIVE;
END
GO
