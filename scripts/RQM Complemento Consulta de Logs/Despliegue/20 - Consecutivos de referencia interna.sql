/* =====================================================================================
   RQM Complemento Consulta de Logs - E4 C1: Consecutivos de referencia interna
   -------------------------------------------------------------------------------------
   Pre-asignación de la nomenclatura inicial de artículos: CÓDIGO-NNN (3 dígitos, inicia en 001).
   - INTERNAL_REFERENCE_CODES: códigos (mayúsculas, sin espacios, máx. 6 caracteres porque
     items.INTERNAL_REFERENCE es varchar(10)).
   - INTERNAL_REFERENCE_CONSECUTIVES: consecutivos generados por código.
   - SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES: generados cuya referencia aún no existe en items.
   - SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES: genera @Quantity consecutivos seguidos por encima del
     mayor entre los generados y los usados en items con formato exacto CÓDIGO-NNN. Transaccional y con
     bloqueo por código (dos usuarios nunca reciben el mismo número).
   El código se compara por posición (no con LIKE) para que caracteres como '_', '%' o '[' no actúen
   como comodines. No modifica la creación de artículos.
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

/* Referencias internas de artículos con formato exacto CÓDIGO-NNN para un código */
CREATE OR ALTER FUNCTION dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES (@Code VARCHAR(6))
RETURNS TABLE
AS
RETURN
    SELECT TRY_CAST(SUBSTRING(i.INTERNAL_REFERENCE, LEN(@Code) + 2, 10) AS INT) AS CONSECUTIVE
      FROM dbo.items i
     WHERE LEFT(i.INTERNAL_REFERENCE, LEN(@Code) + 1) = @Code + '-'
       AND LEN(i.INTERNAL_REFERENCE) >= LEN(@Code) + 4
       AND SUBSTRING(i.INTERNAL_REFERENCE, LEN(@Code) + 2, 10) NOT LIKE '%[^0-9]%'
       AND LEN(SUBSTRING(i.INTERNAL_REFERENCE, LEN(@Code) + 2, 10)) BETWEEN 3 AND 9;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES
    @Code VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Code = UPPER(LTRIM(RTRIM(@Code)));

    SELECT c.CODE AS Code,
           c.CONSECUTIVE AS Consecutive,
           c.CODE + '-' + CASE WHEN c.CONSECUTIVE < 1000 THEN RIGHT('00' + CAST(c.CONSECUTIVE AS VARCHAR(10)), 3) ELSE CAST(c.CONSECUTIVE AS VARCHAR(10)) END AS InternalReference
      FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c
     WHERE c.CODE = @Code
       AND NOT EXISTS (SELECT 1 FROM dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES(@Code) u WHERE u.CONSECUTIVE = c.CONSECUTIVE)
     ORDER BY c.CONSECUTIVE;
END
GO

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
        THROW 50101, N'La cantidad a generar debe ser mayor que cero.', 1;

    DECLARE @Last INT;

    BEGIN TRANSACTION;

        /* Bloqueo por código: serializa generaciones concurrentes del mismo código */
        IF NOT EXISTS (SELECT 1 FROM dbo.INTERNAL_REFERENCE_CODES WITH (UPDLOCK, HOLDLOCK) WHERE CODE = @Code)
            INSERT INTO dbo.INTERNAL_REFERENCE_CODES (CODE) VALUES (@Code);

        SELECT @Last = MAX(x.CONSECUTIVE)
          FROM (SELECT c.CONSECUTIVE FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c WITH (UPDLOCK, HOLDLOCK) WHERE c.CODE = @Code
                UNION ALL
                SELECT u.CONSECUTIVE FROM dbo.FN_INTERNAL_REFERENCE_USED_CONSECUTIVES(@Code) u) x;

        SET @Last = ISNULL(@Last, 0);

        ;WITH n AS (SELECT TOP (@Quantity) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS k
                      FROM sys.all_objects a CROSS JOIN sys.all_objects b)
        INSERT INTO dbo.INTERNAL_REFERENCE_CONSECUTIVES (CODE, CONSECUTIVE)
        SELECT @Code, @Last + k FROM n;

    COMMIT TRANSACTION;

    SELECT @Code AS Code,
           c.CONSECUTIVE AS Consecutive,
           @Code + '-' + CASE WHEN c.CONSECUTIVE < 1000 THEN RIGHT('00' + CAST(c.CONSECUTIVE AS VARCHAR(10)), 3) ELSE CAST(c.CONSECUTIVE AS VARCHAR(10)) END AS InternalReference
      FROM dbo.INTERNAL_REFERENCE_CONSECUTIVES c
     WHERE c.CODE = @Code AND c.CONSECUTIVE > @Last AND c.CONSECUTIVE <= @Last + @Quantity
     ORDER BY c.CONSECUTIVE;
END
GO
