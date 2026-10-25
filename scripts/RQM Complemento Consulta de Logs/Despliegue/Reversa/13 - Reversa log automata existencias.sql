/* =====================================================================================
   RQM Complemento Consulta de Logs - T15b: REVERSA de "13 - Log automata existencias.sql"
   -------------------------------------------------------------------------------------
   Antes: restaurar el NLog.config anterior del Autómata de Existencias y reiniciar el servicio
   (si no, el autómata falla al escribir su log). Los registros ya escritos en log.Logs se conservan.
   Al borrar el SP también se elimina el permiso EXECUTE otorgado al usuario del autómata.
   ===================================================================================== */
SET NOCOUNT ON;
DROP PROCEDURE IF EXISTS log.SP_INSERT_AUTOMATA_EXISTENCIAS_LOG;
GO
