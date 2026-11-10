/* =====================================================================================
   Reversa del script 05 (OPCIONAL): restaura las ciudades "Manizales" eliminadas, con su mismo Id,
   a partir del respaldo en RQM_HOMOLOGADOS_OBJECT_BACKUP. Re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET XACT_ABORT ON;
IF OBJECT_ID('dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP') IS NULL
BEGIN
    PRINT 'No hay respaldo: no se hace nada.';
    RETURN;
END

DECLARE @r TABLE (CITY_ID INT, DEPARTMENT_ID INT, CITY_NAME VARCHAR(50));
INSERT INTO @r
SELECT PARSENAME(REPLACE(b.DEFINITION, '|', '.'), 3), PARSENAME(REPLACE(b.DEFINITION, '|', '.'), 2), PARSENAME(REPLACE(b.DEFINITION, '|', '.'), 1)
  FROM dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP b
 WHERE b.OBJECT_NAME LIKE 'cities.%';

BEGIN TRAN;
    SET IDENTITY_INSERT dbo.cities ON;
    INSERT INTO dbo.cities (CITY_ID, CITY_NAME, DEPARTMENT_ID)
    SELECT r.CITY_ID, r.CITY_NAME, r.DEPARTMENT_ID FROM @r r WHERE NOT EXISTS (SELECT 1 FROM dbo.cities c WHERE c.CITY_ID = r.CITY_ID);
    SET IDENTITY_INSERT dbo.cities OFF;
    DELETE FROM dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP WHERE OBJECT_NAME LIKE 'cities.%';
COMMIT;
GO
