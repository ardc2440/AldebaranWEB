/* =====================================================================================
   RQM Complemento Consulta de Logs - T3: REVERSA de "03 - Consulta visualizacion de alarmas.sql"
   Borra vistas, SP y catálogo de tipos, y los roles 'Log del sistema' y 'Log de inventarios'
   (primero se retiran a los usuarios que los tengan asignados).
   ===================================================================================== */
SET NOCOUNT ON;
DROP PROCEDURE IF EXISTS dbo.SP_GET_ALARM_LOG_VISUALIZERS;
DROP PROCEDURE IF EXISTS dbo.SP_GET_ALARM_LOG_VISUALIZATIONS;
DROP PROCEDURE IF EXISTS dbo.SP_GET_ALARM_LOG;
DROP PROCEDURE IF EXISTS dbo.SP_GET_ALARM_LOG_TYPES;
DROP VIEW IF EXISTS dbo.VW_ALARM_LOG_VISUALIZATIONS;
DROP VIEW IF EXISTS dbo.VW_ALARM_LOG;
DROP TABLE IF EXISTS dbo.ALARM_VISUALIZATION_LOG_TYPES;
GO
DELETE ur FROM dbo.AspNetUserRoles ur JOIN dbo.AspNetRoles r ON r.Id = ur.RoleId
 WHERE r.NormalizedName IN (UPPER(N'Log del sistema'), UPPER(N'Log de inventarios'));
DELETE FROM dbo.AspNetRoles WHERE NormalizedName IN (UPPER(N'Log del sistema'), UPPER(N'Log de inventarios'));
PRINT 'Roles retirados';
GO
