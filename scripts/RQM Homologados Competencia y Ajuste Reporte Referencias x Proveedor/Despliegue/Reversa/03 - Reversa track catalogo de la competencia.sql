/* Reversa de 03 - Track catalogo de la competencia.sql: elimina las filas de catálogo agregadas. */
SET NOCOUNT ON;
GO
DELETE FROM dbo.LOG_TRACK_FIELD_NAMES  WHERE ENTITY_NAME IN (N'CompetitorCompany', N'CompetitorHomologation');
DELETE FROM dbo.LOG_TRACK_ENTITY_NAMES WHERE ENTITY_NAME IN (N'CompetitorCompany', N'CompetitorHomologation');
GO
