/* Reversa de 04 - Reporte homologaciones con la competencia.sql (SP nuevo: se elimina). */
SET NOCOUNT ON;
GO
IF OBJECT_ID('dbo.SP_GET_COMPETITOR_HOMOLOGATION_REPORT', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GET_COMPETITOR_HOMOLOGATION_REPORT;
GO
