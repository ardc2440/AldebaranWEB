/* =====================================================================================
   RQM Complemento Consulta de Logs: REVERSA de "21 - Log archivos totvs.sql"
   Antes: restaurar el NLog.config anterior de ArchivosTotvs y reiniciar el servicio.
   Los registros ya escritos en log.Logs se conservan. Al borrar el SP se elimina también su permiso EXECUTE.
   ===================================================================================== */
SET NOCOUNT ON;
DROP PROCEDURE IF EXISTS log.SP_INSERT_ARCHIVOS_TOTVS_LOG;
GO
