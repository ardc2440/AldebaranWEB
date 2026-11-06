/* =====================================================================================
   Reversa de 02 - Catalogo de la competencia.sql
   ATENCIÓN: elimina las tablas y su información (compañías y homologaciones registradas).
   Las imágenes de la carpeta configurada no se eliminan.
   ===================================================================================== */
SET NOCOUNT ON;
GO
DELETE ur FROM dbo.AspNetUserRoles ur JOIN dbo.AspNetRoles r ON r.Id = ur.RoleId
 WHERE r.NormalizedName = UPPER(N'Catálogo de la competencia');
DELETE FROM dbo.AspNetRoles WHERE NormalizedName = UPPER(N'Catálogo de la competencia');
GO
IF OBJECT_ID('dbo.COMPETITOR_HOMOLOGATIONS') IS NOT NULL DROP TABLE dbo.COMPETITOR_HOMOLOGATIONS;
IF OBJECT_ID('dbo.COMPETITOR_COMPANIES') IS NOT NULL DROP TABLE dbo.COMPETITOR_COMPANIES;
GO
