/* =====================================================================================
   Reversa de 01 - Reporte referencias del proveedor.sql
   Restaura SP_GET_PROVIDER_REFERENCE_REPORT desde RQM_HOMOLOGADOS_OBJECT_BACKUP.
   Desplegar junto con la versión anterior de la aplicación.
   ===================================================================================== */
SET NOCOUNT ON;
GO
DECLARE @def NVARCHAR(MAX) = (SELECT DEFINITION FROM dbo.RQM_HOMOLOGADOS_OBJECT_BACKUP WHERE OBJECT_NAME = 'SP_GET_PROVIDER_REFERENCE_REPORT');
IF @def IS NULL
    THROW 50001, 'No existe respaldo de SP_GET_PROVIDER_REFERENCE_REPORT en RQM_HOMOLOGADOS_OBJECT_BACKUP.', 1;
SET @def = STUFF(@def, PATINDEX('%CREATE%PROC%', @def), 6, 'CREATE OR ALTER');
EXEC (@def);
GO
SELECT CONVERT(VARCHAR(16), modify_date, 120) AS Restaurado FROM sys.objects WHERE name = 'SP_GET_PROVIDER_REFERENCE_REPORT';
GO
