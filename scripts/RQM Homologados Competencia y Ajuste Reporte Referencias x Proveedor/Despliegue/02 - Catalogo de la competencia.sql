/* =====================================================================================
   RQM Homologados Competencia y Ajuste Reporte Referencias x Proveedor - T7
   Catálogo de la competencia: estructuras de datos y rol
   -------------------------------------------------------------------------------------
   - COMPETITOR_COMPANIES: compañías de la competencia. Obligatorios: nombre y sitio web.
     Opcionales: contacto, teléfono, país, departamento/estado y ciudad, en cascada
     (departamento solo con país; ciudad solo con departamento).
     La pertenencia departamento -> país y ciudad -> departamento la valida la aplicación
     (no se alteran las tablas existentes countries/departments/cities).
   - COMPETITOR_HOMOLOGATIONS: homologación de un artículo (y opcionalmente una referencia)
     de PROMOS con un producto de una compañía de la competencia. Solo informativa.
     Obligatorios: compañía, artículo, referencia y nombre del producto en la competencia.
     Opcionales: referencia de PROMOS, enlace, precio (COP) y nombre del archivo de imagen.
     La pertenencia referencia -> artículo la valida la aplicación.
   - Unicidad SOLO entre registros activos (índices únicos filtrados IS_ACTIVE = 1):
     compañías por nombre; homologaciones por compañía + artículo + referencia PROMOS + referencia competencia.
     Puede existir un duplicado inactivo; al reactivarlo, si ya hay uno activo igual, la BD lo rechaza.
   - Registros se inactivan (IS_ACTIVE), no se eliminan desde la aplicación.
   - Rol: "Catálogo de la competencia" (administra compañías y homologaciones).
   Script re-ejecutable. Reversa: Reversa\02 - Reversa catalogo de la competencia.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.COMPETITOR_COMPANIES') IS NULL
    CREATE TABLE dbo.COMPETITOR_COMPANIES
    (
        COMPETITOR_COMPANY_ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_COMPETITOR_COMPANIES PRIMARY KEY,
        COMPANY_NAME          VARCHAR(100) NOT NULL,
        WEBSITE               VARCHAR(250) NOT NULL,
        CONTACT_PERSON        VARCHAR(50)  NULL,
        PHONE                 VARCHAR(20)  NULL,
        COUNTRY_ID            INT          NULL CONSTRAINT FK_COMPETITOR_COMPANIES_COUNTRY REFERENCES dbo.countries (COUNTRY_ID),
        DEPARTMENT_ID         INT          NULL CONSTRAINT FK_COMPETITOR_COMPANIES_DEPARTMENT REFERENCES dbo.departments (DEPARTMENT_ID),
        CITY_ID               INT          NULL CONSTRAINT FK_COMPETITOR_COMPANIES_CITY REFERENCES dbo.cities (CITY_ID),
        IS_ACTIVE             BIT          NOT NULL CONSTRAINT DF_COMPETITOR_COMPANIES_IS_ACTIVE DEFAULT (1),
        CREATION_DATE         DATETIME     NOT NULL CONSTRAINT DF_COMPETITOR_COMPANIES_CREATION_DATE DEFAULT (GETDATE()),
        CONSTRAINT CK_COMPETITOR_COMPANIES_NAME CHECK (LEN(LTRIM(RTRIM(COMPANY_NAME))) > 0),
        CONSTRAINT CK_COMPETITOR_COMPANIES_WEBSITE CHECK (LEN(LTRIM(RTRIM(WEBSITE))) > 0),
        CONSTRAINT CK_COMPETITOR_COMPANIES_LOCATION CHECK ((DEPARTMENT_ID IS NULL OR COUNTRY_ID IS NOT NULL)
                                                       AND (CITY_ID IS NULL OR DEPARTMENT_ID IS NOT NULL))
    );
GO

IF OBJECT_ID('dbo.COMPETITOR_HOMOLOGATIONS') IS NULL
    CREATE TABLE dbo.COMPETITOR_HOMOLOGATIONS
    (
        COMPETITOR_HOMOLOGATION_ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_COMPETITOR_HOMOLOGATIONS PRIMARY KEY,
        COMPETITOR_COMPANY_ID      INT           NOT NULL CONSTRAINT FK_COMPETITOR_HOMOLOGATIONS_COMPANY REFERENCES dbo.COMPETITOR_COMPANIES (COMPETITOR_COMPANY_ID),
        ITEM_ID                    INT           NOT NULL CONSTRAINT FK_COMPETITOR_HOMOLOGATIONS_ITEM REFERENCES dbo.items (ITEM_ID),
        REFERENCE_ID               INT           NULL     CONSTRAINT FK_COMPETITOR_HOMOLOGATIONS_REFERENCE REFERENCES dbo.item_references (REFERENCE_ID),
        COMPETITOR_REFERENCE       VARCHAR(50)   NOT NULL,
        COMPETITOR_PRODUCT_NAME    VARCHAR(150)  NOT NULL,
        PRODUCT_URL                VARCHAR(500)  NULL,
        COMPETITOR_PRICE           DECIMAL(18,2) NULL,
        IMAGE_FILE_NAME            VARCHAR(100)  NULL,
        IS_ACTIVE                  BIT           NOT NULL CONSTRAINT DF_COMPETITOR_HOMOLOGATIONS_IS_ACTIVE DEFAULT (1),
        CREATION_DATE              DATETIME      NOT NULL CONSTRAINT DF_COMPETITOR_HOMOLOGATIONS_CREATION_DATE DEFAULT (GETDATE()),
        CONSTRAINT CK_COMPETITOR_HOMOLOGATIONS_REFERENCE CHECK (LEN(LTRIM(RTRIM(COMPETITOR_REFERENCE))) > 0),
        CONSTRAINT CK_COMPETITOR_HOMOLOGATIONS_NAME CHECK (LEN(LTRIM(RTRIM(COMPETITOR_PRODUCT_NAME))) > 0),
        CONSTRAINT CK_COMPETITOR_HOMOLOGATIONS_PRICE CHECK (COMPETITOR_PRICE IS NULL OR COMPETITOR_PRICE >= 0)
    );
GO

/* Versión anterior del script: unicidad sin filtro -> se reemplaza por índices filtrados (solo activos) */
IF OBJECT_ID('dbo.UQ_COMPETITOR_COMPANIES_NAME', 'UQ') IS NOT NULL
    ALTER TABLE dbo.COMPETITOR_COMPANIES DROP CONSTRAINT UQ_COMPETITOR_COMPANIES_NAME;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_COMPETITOR_HOMOLOGATIONS' AND object_id = OBJECT_ID('dbo.COMPETITOR_HOMOLOGATIONS') AND has_filter = 0)
    DROP INDEX UQ_COMPETITOR_HOMOLOGATIONS ON dbo.COMPETITOR_HOMOLOGATIONS;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_COMPETITOR_COMPANIES_NAME_ACTIVE' AND object_id = OBJECT_ID('dbo.COMPETITOR_COMPANIES'))
    CREATE UNIQUE INDEX UQ_COMPETITOR_COMPANIES_NAME_ACTIVE
        ON dbo.COMPETITOR_COMPANIES (COMPANY_NAME) WHERE IS_ACTIVE = 1;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_COMPETITOR_HOMOLOGATIONS' AND object_id = OBJECT_ID('dbo.COMPETITOR_HOMOLOGATIONS'))
    CREATE UNIQUE INDEX UQ_COMPETITOR_HOMOLOGATIONS
        ON dbo.COMPETITOR_HOMOLOGATIONS (COMPETITOR_COMPANY_ID, ITEM_ID, REFERENCE_ID, COMPETITOR_REFERENCE) WHERE IS_ACTIVE = 1;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_COMPETITOR_HOMOLOGATIONS_ITEM' AND object_id = OBJECT_ID('dbo.COMPETITOR_HOMOLOGATIONS'))
    CREATE INDEX IX_COMPETITOR_HOMOLOGATIONS_ITEM
        ON dbo.COMPETITOR_HOMOLOGATIONS (ITEM_ID, REFERENCE_ID) INCLUDE (COMPETITOR_COMPANY_ID, IS_ACTIVE);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_COMPETITOR_HOMOLOGATIONS_REFERENCE' AND object_id = OBJECT_ID('dbo.COMPETITOR_HOMOLOGATIONS'))
    CREATE INDEX IX_COMPETITOR_HOMOLOGATIONS_REFERENCE
        ON dbo.COMPETITOR_HOMOLOGATIONS (REFERENCE_ID) WHERE REFERENCE_ID IS NOT NULL;
GO

/* ---------- Rol ---------- */
INSERT INTO dbo.AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
SELECT CAST(NEWID() AS NVARCHAR(450)), CAST(NEWID() AS NVARCHAR(MAX)), r.Name, UPPER(r.Name)
  FROM (VALUES (N'Catálogo de la competencia')) r(Name)
 WHERE NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles x WHERE x.NormalizedName = UPPER(r.Name));
GO

/* ---------- Verificación ---------- */
SELECT t.name AS Tabla, (SELECT COUNT(*) FROM sys.columns c WHERE c.object_id = t.object_id) AS Columnas
  FROM sys.tables t WHERE t.name IN ('COMPETITOR_COMPANIES', 'COMPETITOR_HOMOLOGATIONS');
SELECT Name FROM dbo.AspNetRoles WHERE NormalizedName = UPPER(N'Catálogo de la competencia');
GO
