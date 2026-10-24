/* =====================================================================================
   RQM Complemento Consulta de Logs - E4 C1: Consecutivos de referencia interna
   -------------------------------------------------------------------------------------
   Pre-asignación de la nomenclatura inicial de artículos: CÓDIGO-NNN (3 dígitos, inicia en 001).
   - INTERNAL_REFERENCE_CODES: códigos (mayúsculas, sin espacios, máx. 6 caracteres porque
     items.INTERNAL_REFERENCE es varchar(10)).
   - INTERNAL_REFERENCE_CONSECUTIVES: SOLO consecutivos generados aún no usados. Los que ya existen como
     referencia interna de un artículo se eliminan (SP_PURGE_USED_...) al Consultar y al Generar.
   - SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES: depura y devuelve los disponibles.
   - SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES: completa hasta @Quantity disponibles; los nuevos siguen al
     mayor entre los disponibles y los usados en items + 1. Transaccional y con
     bloqueo por código (dos usuarios nunca reciben el mismo número).
   Uso en artículos: LIKE 'CÓDIGO-%' + bloque de 2 o 3 dígitos (ver FN_INTERNAL_REFERENCE_USED_CONSECUTIVES).
   No modifica la creación de artículos.
   Script re-ejecutable. Reversa: Reversa\20 - Reversa consecutivos de referencia interna.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.INTERNAL_REFERENCE_CODES') IS NULL
    CREATE TABLE dbo.INTERNAL_REFERENCE_CODES
    (
        CODE VARCHAR(6) NOT NULL CONSTRAINT PK_INTERNAL_REFERENCE_CODES PRIMARY KEY
    );
GO

IF OBJECT_ID('dbo.INTERNAL_REFERENCE_CONSECUTIVES') IS NULL
    CREATE TABLE dbo.INTERNAL_REFERENCE_CONSECUTIVES
    (
        CODE        VARCHAR(6) NOT NULL CONSTRAINT FK_INTERNAL_REFERENCE_CONSECUTIVES_CODE REFERENCES dbo.INTERNAL_REFERENCE_CODES (CODE),
        CONSECUTIVE INT        NOT NULL CONSTRAINT CK_INTERNAL_REFERENCE_CONSECUTIVES_POSITIVE CHECK (CONSECUTIVE > 0),
        CONSTRAINT PK_INTERNAL_REFERENCE_CONSECUTIVES PRIMARY KEY (CODE, CONSECUTIVE)
    );
GO

/* Consecutivos usados en items para un código.
   Se buscan las referencias con LIKE 'CÓDIGO-%' (el código se escapa para que '%', '_' y '[' no sean comodines;
   el guion evita mezclar MU con MUX). Después del guion va un bloque de 2 o 3 dígitos que puede terminar ahí o
   seguir con '-' y un complemento: MU-001, MU-001-1, MU-001-02, MU-001-X => 1. Las referencias actuales de
   2 dígitos (MU-25, MU-25-1) equivalen a 025. Se devuelve el valor numérico. */
CREATE OR ALTER FUNCTION dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES (@Code VARCHAR(6))
RETURNS TABLE
AS
RETURN
    SELECT TRY_CAST(d.DIGITS AS INT) AS CONSECUTIVE
      FROM dbo.items i
     CROSS APPLY (SELECT SUBSTRING(i.INTERNAL_REFERENCE, LEN(@Code) + 2, 10) AS REST) r
     CROSS APPLY (SELECT LEFT(r.REST, PATINDEX('%[^0-9]%', r.REST + 'x') - 1) AS DIGITS) d
     WHERE i.INTERNAL_REFERENCE LIKE REPLACE(REPLACE(REPLACE(REPLACE(@Code, '\', '\\'), '%', '\%'), '_', '\_'), '[', '\[') + '-%' ESCAPE '\'
       AND LEN(d.DIGITS) BETWEEN 2 AND 3
       AND (LEN(r.REST) = LEN(d.DIGITS) OR SUBSTRING(r.REST, LEN(d.DIGITS) + 1, 1) = '-');
GO

/* Elimina los consecutivos que ya se usaron como referencia interna de un artículo: la tabla solo conserva disponibles */
CREATE OR ALTER PROCEDURE dbo.SP_PURGE_USED_INTERNAL_REFERENCE_CONSECUTIVES
    @Code VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE c
      FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c
     WHERE c.CODE = @Code
       AND EXISTS (SELECT 1 FROM dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES(@Code) u WHERE u.CONSECUTIVE = c.CONSECUTIVE);
END
GO

/* Consultar: depura los usados y devuelve los disponibles */
CREATE OR ALTER PROCEDURE dbo.SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES
    @Code VARCHAR(6)
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
   del mayor entre los disponibles y los usados en artículos + 1. Devuelve todos los disponibles (IsNew = 1 los creados). */
CREATE OR ALTER PROCEDURE dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES
    @Code     VARCHAR(6),
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Code = UPPER(LTRIM(RTRIM(@Code)));

    IF @Code IS NULL OR LEN(@Code) = 0 OR CHARINDEX(' ', @Code) > 0
        THROW 50100, N'El código es obligatorio y no puede contener espacios.', 1;
    IF @Quantity IS NULL OR @Quantity < 1
        THROW 50101, N'La cantidad debe ser mayor que cero.', 1;

    DECLARE @Last INT, @Available INT, @ToCreate INT;

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
