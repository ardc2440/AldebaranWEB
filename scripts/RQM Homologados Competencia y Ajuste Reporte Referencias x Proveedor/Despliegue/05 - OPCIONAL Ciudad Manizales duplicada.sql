/* =====================================================================================
   RQM Homologados Competencia y Ajuste Reporte Referencias x Proveedor - Dato observado (OPCIONAL)
   Ciudad "Manizales" duplicada bajo un departamento distinto de Caldas
   -------------------------------------------------------------------------------------
   EJECUTAR SOLO CON APROBACIÓN DEL CLIENTE.
   En desarrollo existen dos registros "Manizales": uno en Caldas (correcto, en uso) y otro en Antioquia
   (sin uso). El script:
     1. Busca "Manizales" fuera de Caldas, solo si existe también la de Caldas.
     2. Cuenta su uso en todas las tablas que referencian dbo.cities (clientes, proveedores, transportadoras,
        agentes, satélites y compañías de la competencia).
     3. Si NO tiene uso, la elimina y deja un respaldo en RQM_HOMOLOGADOS_OBJECT_BACKUP (para la reversa).
        Si SÍ tiene uso, no cambia nada y lista dónde se usa (reasignar a la de Caldas es decisión del cliente).
   Script re-ejecutable. Reversa: Reversa\05 - Reversa ciudad Manizales duplicada.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @caldas INT = (SELECT TOP 1 c.CITY_ID FROM dbo.cities c JOIN dbo.departments d ON d.DEPARTMENT_ID = c.DEPARTMENT_ID
                        WHERE c.CITY_NAME = 'Manizales' AND d.DEPARTMENT_NAME = 'Caldas');
IF @caldas IS NULL
BEGIN
    PRINT 'No existe Manizales en Caldas: no se hace nada.';
    RETURN;
END

DECLARE @dup TABLE (CITY_ID INT PRIMARY KEY, DEPARTMENT_ID INT, Usos INT);
INSERT INTO @dup (CITY_ID, DEPARTMENT_ID, Usos)
SELECT c.CITY_ID, c.DEPARTMENT_ID,
       (SELECT COUNT(*) FROM dbo.customers x WHERE x.CITY_ID = c.CITY_ID)
     + (SELECT COUNT(*) FROM dbo.providers x WHERE x.CITY_ID = c.CITY_ID)
     + (SELECT COUNT(*) FROM dbo.forwarders x WHERE x.CITY_ID = c.CITY_ID)
     + (SELECT COUNT(*) FROM dbo.forwarder_agents x WHERE x.CITY_ID = c.CITY_ID)
     + (SELECT COUNT(*) FROM dbo.process_satellites x WHERE x.CITY_ID = c.CITY_ID)
     + (SELECT COUNT(*) FROM dbo.COMPETITOR_COMPANIES x WHERE x.CITY_ID = c.CITY_ID)
  FROM dbo.cities c
 WHERE c.CITY_NAME = 'Manizales' AND c.CITY_ID <> @caldas;

IF NOT EXISTS (SELECT 1 FROM @dup)
BEGIN
    PRINT 'No hay Manizales duplicada: no se hace nada.';
    RETURN;
END

SELECT d.CITY_ID, dep.DEPARTMENT_NAME AS DepartamentoActual, d.Usos,
       CASE WHEN d.Usos = 0 THEN 'Se elimina' ELSE 'En uso: no se cambia (requiere decisión del cliente)' END AS Accion
  FROM @dup d JOIN dbo.departments dep ON dep.DEPARTMENT_ID = d.DEPARTMENT_ID;

IF OBJECT_ID('dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP') IS NULL
    CREATE TABLE dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP (OBJECT_NAME SYSNAME NOT NULL CONSTRAINT PK_RQM_HOMOLOGADOS_OBJECT_BACKUP PRIMARY KEY,
        OBJECT_TYPE CHAR(2) NOT NULL, DEFINITION NVARCHAR(MAX) NOT NULL,
        BACKUP_DATE DATETIME NOT NULL CONSTRAINT DF_RQM_HOMOLOGADOS_OBJECT_BACKUP_DATE DEFAULT (GETDATE()));

BEGIN TRAN;
    INSERT INTO dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP (OBJECT_NAME, OBJECT_TYPE, DEFINITION)
    SELECT CONCAT('cities.', d.CITY_ID), 'D', CONCAT(d.CITY_ID, '|', d.DEPARTMENT_ID, '|Manizales')
      FROM @dup d
     WHERE d.Usos = 0 AND NOT EXISTS (SELECT 1 FROM dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP b WHERE b.OBJECT_NAME = CONCAT('cities.', d.CITY_ID));
    DELETE c FROM dbo.cities c JOIN @dup d ON d.CITY_ID = c.CITY_ID WHERE d.Usos = 0;
COMMIT;
GO
