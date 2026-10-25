/* =====================================================================================
   RQM Complemento Consulta de Logs - T1: REVERSA de "01 - Indices Log y Track.sql"
   ÚLTIMA reversa: borra índices y columnas calculadas, y la tabla de respaldo RQM_LOGS_OBJECT_BACKUP
   (ya usada por las reversas 04, 05, 14, 16 y 18).
   La ofuscación del histórico del Track (script 02) no se puede revertir: los valores no se guardan.
   ===================================================================================== */
SET NOCOUNT ON;
DROP INDEX IF EXISTS IX_Logs_TimeStamp ON log.Logs;
DROP INDEX IF EXISTS IX_Logs_Template_TimeStamp ON log.Logs;
DROP INDEX IF EXISTS IX_track_Entity_Date ON log.track;
DROP INDEX IF EXISTS IX_track_Entity_Key ON log.track;
GO
IF COL_LENGTH('log.Logs', 'TEMPLATE_HASH') IS NOT NULL ALTER TABLE log.Logs DROP COLUMN TEMPLATE_HASH;
IF COL_LENGTH('log.Logs', 'LEVEL_KEY') IS NOT NULL ALTER TABLE log.Logs DROP COLUMN LEVEL_KEY;
IF COL_LENGTH('log.track', 'ENTITY_KEY_HASH') IS NOT NULL ALTER TABLE log.track DROP COLUMN ENTITY_KEY_HASH;
GO
DROP TABLE IF EXISTS dbo.RQM_LOGS_OBJECT_BACKUP;
PRINT 'Reversa completa';
GO
