/* =====================================================================================
   RQM Complemento Consulta de Logs - T6b: REVERSA de "08 - Busqueda Track por objetos.sql"
   Quita el trigger sobre log.track (deja de indexar) y borra el índice de objetos con su catálogo.
   SP_GET_TRACK_LOG (redefinido aquí) se borra con la reversa 07.
   ===================================================================================== */
SET NOCOUNT ON;
DROP TRIGGER IF EXISTS log.TRG_LOG_TRACK_INDEX_OBJECTS;
DROP PROCEDURE IF EXISTS dbo.SP_SEARCH_TRACK_OBJECTS;
DROP PROCEDURE IF EXISTS dbo.SP_GET_TRACK_OBJECT_TYPES;
DROP PROCEDURE IF EXISTS dbo.SP_GET_TRACK_OBJECT_DESCRIPTIONS;
DROP PROCEDURE IF EXISTS dbo.SP_BACKFILL_TRACK_OBJECTS;
DROP FUNCTION IF EXISTS dbo.FN_TRACK_LOOKUP_DESCRIPTION;
DROP FUNCTION IF EXISTS dbo.FN_TRACK_OBJECT_DESCRIPTION;
DROP FUNCTION IF EXISTS dbo.FN_TRACK_OBJECTS;
DROP TABLE IF EXISTS dbo.LOG_TRACK_OBJECTS_BACKFILL;
DROP TABLE IF EXISTS dbo.LOG_TRACK_OBJECTS;
DROP TABLE IF EXISTS dbo.LOG_TRACK_OBJECT_FIELDS;
DROP TABLE IF EXISTS dbo.LOG_TRACK_OBJECT_TYPES;
GO
