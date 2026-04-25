# 10. MIGRACIONES DE BASE DE DATOS - Bonos Distribuidores

## Status: ? PENDIENTE SCRIPTS FINALES

---

## ?? Resumen

Este documento contiene los scripts de migración EF Core y SQL para la implementación de Bonos.

**IMPORTANTE**: No ejecutar scripts hasta que estén 100% validados.

---

## ?? Estrategia de Migraciones

```
Ambientes:
  [ ] LOCAL (desarrollo)
      - Ejecutar automáticamente en Debug
      - Permitir Drop/Recreate

  [ ] DEV
      - Ejecutar en startup si "AutoMigrate" = true
      - Log de migraciones

  [ ] QA
      - Ejecutar manual con aprobación
      - Backup previo

  [ ] STAGING
      - Ejecutar manual con validación
      - Backup previo

  [ ] PROD
      - Ejecutar manual con rollback plan
      - Backup full
      - Validación post-migración
```

---

## 1?? MIGRACIÓN 1: Base de Datos Principal

### 1.1 Archivo de Migración EF Core

```csharp
// Aldebaran.DataAccess/Migrations/20250514000000_AddBonusFeature.cs

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldebaran.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBonusFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear tabla Bonus
            migrationBuilder.CreateTable(
                name: "Bonus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bonus_Distributor",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Crear tabla BonusDetail
            migrationBuilder.CreateTable(
                name: "BonusDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonusId = table.Column<int>(type: "int", nullable: false),
                    Criteria = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonusDetail_Bonus",
                        column: x => x.BonusId,
                        principalTable: "Bonus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Crear tabla BonusApplication
            migrationBuilder.CreateTable(
                name: "BonusApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonusId = table.Column<int>(type: "int", nullable: false),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentReference = table.Column<int>(type: "int", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonusApplication_Bonus",
                        column: x => x.BonusId,
                        principalTable: "Bonus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BonusApplication_Distributor",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Crear índices
            migrationBuilder.CreateIndex(
                name: "IX_Bonus_DistributorId",
                table: "Bonus",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bonus_Status",
                table: "Bonus",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Bonus_StartDate_EndDate",
                table: "Bonus",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BonusDetail_BonusId",
                table: "BonusDetail",
                column: "BonusId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusApplication_BonusId",
                table: "BonusApplication",
                column: "BonusId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusApplication_DistributorId",
                table: "BonusApplication",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusApplication_DocumentReference",
                table: "BonusApplication",
                column: "DocumentReference");

            migrationBuilder.CreateIndex(
                name: "IX_BonusApplication_ApplicationDate",
                table: "BonusApplication",
                column: "ApplicationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusApplication");

            migrationBuilder.DropTable(
                name: "BonusDetail");

            migrationBuilder.DropTable(
                name: "Bonus");
        }
    }
}
```

---

## 2?? MIGRACIÓN 2: Base de Datos Secundaria

### 2.1 Archivo de Migración EF Core

```csharp
// Aldebaran.DataAccess/Migrations/SecondaryDb/20250514000000_CreateSecondaryBonusDb.cs

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldebaran.DataAccess.Migrations.SecondaryDb
{
    /// <inheritdoc />
    public partial class CreateSecondaryBonusDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear tabla BonusSync
            migrationBuilder.CreateTable(
                name: "BonusSync",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceBonusId = table.Column<int>(type: "int", nullable: false),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SyncDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusSync", x => x.Id);
                });

            // Crear tabla BonusApplicationSync
            migrationBuilder.CreateTable(
                name: "BonusApplicationSync",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceApplicationId = table.Column<int>(type: "int", nullable: false),
                    SourceBonusId = table.Column<int>(type: "int", nullable: false),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentReference = table.Column<int>(type: "int", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SyncDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusApplicationSync", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonusApplicationSync_BonusSync",
                        column: x => x.SourceBonusId,
                        principalTable: "BonusSync",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Crear tabla de auditoría
            migrationBuilder.CreateTable(
                name: "SyncAudit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SyncDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordsAffected = table.Column<int>(type: "int", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    TriggerSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncAudit", x => x.Id);
                });

            // Crear índices
            migrationBuilder.CreateIndex(
                name: "IX_BonusSync_SourceBonusId",
                table: "BonusSync",
                column: "SourceBonusId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusSync_DistributorId",
                table: "BonusSync",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusSync_Status",
                table: "BonusSync",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BonusSync_SyncDate",
                table: "BonusSync",
                column: "SyncDate");

            migrationBuilder.CreateIndex(
                name: "IX_BonusApplicationSync_SourceBonusId",
                table: "BonusApplicationSync",
                column: "SourceBonusId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusApplicationSync_DistributorId",
                table: "BonusApplicationSync",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncAudit_SyncDateTime",
                table: "SyncAudit",
                column: "SyncDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_SyncAudit_Status",
                table: "SyncAudit",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncAudit");

            migrationBuilder.DropTable(
                name: "BonusApplicationSync");

            migrationBuilder.DropTable(
                name: "BonusSync");
        }
    }
}
```

---

## 3?? SCRIPTS SQL (Manual - Backup)

### 3.1 Crear BD Secundaria

```sql
-- scripts/CreateSecondaryDatabase.sql

-- Crear la base de datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SecondaryBonusDb')
BEGIN
    CREATE DATABASE [SecondaryBonusDb]
    COLLATE SQL_Latin1_General_CP1_CI_AS;
END
GO

-- Usar la nueva BD
USE [SecondaryBonusDb];
GO

-- Crear usuario y asignar permisos (si es necesario)
-- CREAR LOGIN Y USER según requerimientos de seguridad
```

### 3.2 Scripts de Rollback

```sql
-- scripts/RollbackBonusFeature.sql

-- Ejecutar en BD principal (Aldebaran)
USE [Aldebaran];
GO

-- Eliminar constraint de integridad referencial
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BonusApplication_Bonus')
    ALTER TABLE dbo.BonusApplication DROP CONSTRAINT FK_BonusApplication_Bonus;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BonusApplication_Distributor')
    ALTER TABLE dbo.BonusApplication DROP CONSTRAINT FK_BonusApplication_Distributor;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BonusDetail_Bonus')
    ALTER TABLE dbo.BonusDetail DROP CONSTRAINT FK_BonusDetail_Bonus;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Bonus_Distributor')
    ALTER TABLE dbo.Bonus DROP CONSTRAINT FK_Bonus_Distributor;

-- Eliminar tablas
DROP TABLE IF EXISTS dbo.BonusApplication;
DROP TABLE IF EXISTS dbo.BonusDetail;
DROP TABLE IF EXISTS dbo.Bonus;

-- Eliminar en BD secundaria
USE [SecondaryBonusDb];
GO

DROP TABLE IF EXISTS dbo.SyncAudit;
DROP TABLE IF EXISTS dbo.BonusApplicationSync;
DROP TABLE IF EXISTS dbo.BonusSync;
```

---

## 4?? PROCEDIMIENTO DE MIGRACIÓN

### 4.1 En Ambiente LOCAL

```bash
# Crear migración
dotnet ef migrations add AddBonusFeature -p Aldebaran.DataAccess -c AldebaranDbContext

# Actualizar BD local
dotnet ef database update -p Aldebaran.DataAccess -c AldebaranDbContext

# Crear migración para BD secundaria
dotnet ef migrations add CreateSecondaryBonusDb -p Aldebaran.DataAccess -c SecondaryBonusDbContext -o Migrations/SecondaryDb

# Actualizar BD secundaria local
dotnet ef database update -p Aldebaran.DataAccess -c SecondaryBonusDbContext
```

### 4.2 En Ambiente DEV

```bash
# Opción 1: Automático en startup (si AutoMigrate = true en appsettings)
# El aplicativo ejecutará migraciones automáticamente

# Opción 2: Manual
dotnet ef database update --configuration Release -p Aldebaran.DataAccess

# Validar
SELECT COUNT(*) FROM Bonus;
SELECT COUNT(*) FROM BonusDetail;
SELECT COUNT(*) FROM BonusApplication;
```

### 4.3 En Ambiente QA/STAGING

```bash
# 1. Validar en STAGING antes de ejecutar
dotnet ef migrations script --from 0 --output migration.sql

# 2. Revisar el script (migration.sql)

# 3. Crear BACKUP previo
BACKUP DATABASE [Aldebaran] TO DISK = 'C:\Backups\Aldebaran_PreBonusFeature.bak'

# 4. Ejecutar migración
dotnet ef database update

# 5. Validar tablas creadas
sp_help 'dbo.Bonus'
sp_help 'dbo.BonusDetail'
sp_help 'dbo.BonusApplication'

# 6. Pruebas de acceso
SELECT * FROM Bonus WHERE 1=0;  -- Validar estructura
```

### 4.4 En Ambiente PRODUCCIÓN

```bash
# ?? PROCEDIMIENTO MÁS RIGUROSO

# 1. Backup completo
BACKUP DATABASE [Aldebaran] TO DISK = 'C:\Backups\Aldebaran_PreBonusFeature_PROD.bak'
BACKUP DATABASE [SecondaryBonusDb] TO DISK = 'C:\Backups\SecondaryBonusDb_Initial_PROD.bak'

# 2. Generar script de migración
dotnet ef migrations script -o prod_migration.sql

# 3. Revisar script (verificar cada sentencia SQL)

# 4. Ejecutar con transaction wrapper
BEGIN TRANSACTION;
  -- [Ejecutar script de migración aquí]
COMMIT; -- O ROLLBACK; si hay error

# 5. Validar
SELECT COUNT(*) FROM Bonus;
SELECT COUNT(*) FROM BonusDetail;
SELECT COUNT(*) FROM BonusApplication;

# 6. Verificar integridad referencial
SELECT * FROM sysobjects WHERE type = 'F' AND name LIKE '%Bonus%'

# 7. Monitoreo post-migración
-- Revisar logs de aplicación
-- Validar tiempo de respuesta de consultas
-- Revisar bloqueos de BD
```

---

## 5?? VALIDACIONES POST-MIGRACIÓN

### 5.1 Checklist de Validación

```sql
-- BD Principal
USE [Aldebaran];
GO

-- 1. Validar tablas existen
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Bonus', 'BonusDetail', 'BonusApplication');

-- 2. Validar columnas
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Bonus';

-- 3. Validar índices
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Bonus');

-- 4. Validar foreign keys
SELECT name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Bonus');

-- 5. Validar que no hay datos duplicados en migraciones anteriores
SELECT COUNT(*) FROM Bonus;  -- Debe ser 0 inicialmente

-- 6. Validar que puedo insertar data
BEGIN TRANSACTION;
INSERT INTO Bonus (DistributorId, Type, Value, StartDate, EndDate, Status, CreatedDate)
VALUES (1, 1, 100.00, GETUTCDATE(), DATEADD(MONTH, 1, GETUTCDATE()), 1, GETUTCDATE());
ROLLBACK;  -- No commitear, solo validar

-- BD Secundaria
USE [SecondaryBonusDb];
GO

-- 1. Validar tablas existen
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('BonusSync', 'BonusApplicationSync', 'SyncAudit');

-- 2. Validar estructura
EXEC sp_help 'dbo.BonusSync';
EXEC sp_help 'dbo.BonusApplicationSync';
EXEC sp_help 'dbo.SyncAudit';
```

---

## 6?? ROLLBACK PROCEDURE

```sql
-- Si algo sale mal, ejecutar rollback:

-- BD Principal
USE [Aldebaran];
GO

-- Si se ejecutó manual:
-- 1. Restore from backup
RESTORE DATABASE [Aldebaran] FROM DISK = 'C:\Backups\Aldebaran_PreBonusFeature.bak'

-- Si se ejecutó via EF Core:
-- 1. Revertir a migración anterior
dotnet ef database update XXXXXXXXXXXX_[NombreAnterior] -p Aldebaran.DataAccess

-- BD Secundaria
USE [master];
GO

DROP DATABASE [SecondaryBonusDb];

-- Recrear desde backup si es necesario
RESTORE DATABASE [SecondaryBonusDb] FROM DISK = 'C:\Backups\SecondaryBonusDb_Initial.bak'
```

---

## ?? Checklist Pre-Migración

```
[ ] Backups creados y verificados
[ ] Script de migración revisado
[ ] Connection strings confirmados
[ ] Credenciales validadas
[ ] Segunda BD creada y accesible
[ ] Ventana de mantenimiento confirmada (si aplica)
[ ] Equipo de soporte en espera
[ ] Plan de rollback documentado
[ ] Pruebas post-migración planificadas
[ ] Stakeholders notificados
```

---

## ?? Notas sobre Migraciones

> [Aquí irán notas específicas según ambiente]

---

**Última actualización**: [Pendiente]
**Responsable**: [DBA / DevOps]
**Estado**: ?? Incompleto - Pendiente scripts finales
