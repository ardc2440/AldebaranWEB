/* =====================================================================================
   RQM Complemento Consulta de Logs - T6c: REVERSA de "09 - Nombres de campo Track.sql"
   Borra el catálogo de nombres de campo (incluye las filas agregadas por los scripts 14 y 17).
   SP_GET_TRACK_LOG (redefinido aquí) se borra con la reversa 07.
   ===================================================================================== */
SET NOCOUNT ON;
DROP PROCEDURE IF EXISTS dbo.SP_GET_TRACK_FIELD_NAMES;
DROP TABLE IF EXISTS dbo.LOG_TRACK_FIELD_NAMES;
GO
