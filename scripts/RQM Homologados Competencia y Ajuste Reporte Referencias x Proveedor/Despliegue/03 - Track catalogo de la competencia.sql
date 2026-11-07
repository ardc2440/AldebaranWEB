/* =====================================================================================
   RQM Homologados Competencia y Ajuste Reporte Referencias x Proveedor - T8/T10
   Track: nombres en español de las entidades y campos del Catálogo de la competencia
   -------------------------------------------------------------------------------------
   Requiere los objetos del RQM Complemento Consulta de Logs (LOG_TRACK_ENTITY_NAMES, LOG_TRACK_FIELD_NAMES)
   y el script 02 de este RQM. Solo agrega/actualiza filas de catálogo; no cambia estructuras.
   Script re-ejecutable. Reversa: Reversa\03 - Reversa track catalogo de la competencia.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.LOG_TRACK_ENTITY_NAMES') IS NULL OR OBJECT_ID('dbo.LOG_TRACK_FIELD_NAMES') IS NULL
    THROW 50001, 'Faltan LOG_TRACK_ENTITY_NAMES / LOG_TRACK_FIELD_NAMES (RQM Complemento Consulta de Logs).', 1;
GO

MERGE dbo.LOG_TRACK_ENTITY_NAMES AS t
USING (VALUES
    (N'CompetitorCompany',      N'Compañía de la competencia'),
    (N'CompetitorHomologation', N'Homologación con la competencia')
) AS s (ENTITY_NAME, DISPLAY_NAME)
ON t.ENTITY_NAME = s.ENTITY_NAME
WHEN MATCHED AND t.DISPLAY_NAME <> s.DISPLAY_NAME THEN UPDATE SET DISPLAY_NAME = s.DISPLAY_NAME
WHEN NOT MATCHED THEN INSERT (ENTITY_NAME, DISPLAY_NAME, IS_ACTIVE) VALUES (s.ENTITY_NAME, s.DISPLAY_NAME, 1);
GO

MERGE dbo.LOG_TRACK_FIELD_NAMES AS t
USING (VALUES
    (N'CompetitorCompany',      N'CompetitorCompanyId',        N'Id de la compañía de la competencia', 1),
    (N'CompetitorCompany',      N'CompanyName',                N'Nombre de la compañía', 0),
    (N'CompetitorCompany',      N'Website',                    N'Sitio web', 0),
    (N'CompetitorCompany',      N'CountryId',                  N'País', 0),
    (N'CompetitorCompany',      N'DepartmentId',               N'Departamento / Estado', 0),
    (N'CompetitorCompany',      N'IsActive',                   N'Compañía activa', 0),
    (N'CompetitorHomologation', N'CompetitorHomologationId',   N'Id de la homologación', 1),
    (N'CompetitorHomologation', N'CompetitorCompanyId',        N'Compañía de la competencia', 0),
    (N'CompetitorHomologation', N'CompetitorReference',        N'Referencia del competidor', 0),
    (N'CompetitorHomologation', N'CompetitorProductName',      N'Nombre del producto del competidor', 0),
    (N'CompetitorHomologation', N'ProductUrl',                 N'Enlace al producto', 0),
    (N'CompetitorHomologation', N'CompetitorPrice',            N'Precio de la competencia', 0),
    (N'CompetitorHomologation', N'ImageFileName',              N'Imagen del producto', 0),
    (N'CompetitorHomologation', N'IsActive',                   N'Homologación activa', 0)
) AS s (ENTITY_NAME, FIELD_NAME, DISPLAY_NAME, IS_TECHNICAL)
ON t.ENTITY_NAME = s.ENTITY_NAME AND t.FIELD_NAME = s.FIELD_NAME
WHEN MATCHED AND (t.DISPLAY_NAME <> s.DISPLAY_NAME OR t.IS_TECHNICAL <> s.IS_TECHNICAL)
    THEN UPDATE SET DISPLAY_NAME = s.DISPLAY_NAME, IS_TECHNICAL = s.IS_TECHNICAL
WHEN NOT MATCHED THEN INSERT (ENTITY_NAME, FIELD_NAME, DISPLAY_NAME, IS_TECHNICAL) VALUES (s.ENTITY_NAME, s.FIELD_NAME, s.DISPLAY_NAME, s.IS_TECHNICAL);
GO

/* ---------- Verificación ---------- */
SELECT ENTITY_NAME, DISPLAY_NAME FROM dbo.LOG_TRACK_ENTITY_NAMES WHERE ENTITY_NAME LIKE N'Competitor%';
SELECT ENTITY_NAME, COUNT(*) AS Campos FROM dbo.LOG_TRACK_FIELD_NAMES WHERE ENTITY_NAME LIKE N'Competitor%' GROUP BY ENTITY_NAME;
GO
