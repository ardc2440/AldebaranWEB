# 9. CAMBIOS DE CÓDIGO - Bonos Distribuidores

## Status: ? PENDIENTE DETALLES

---

## ?? Resumen

Este documento especifica **exactamente** qué archivos se crearán, modificarán o eliminarán durante la implementación.

**IMPORTANTE**: No hacer cambios hasta que este documento esté completamente definido y aprobado.

---

## ?? ARCHIVOS A CREAR

### Backend - Entidades

```
?? Aldebaran.DataAccess/Entities/Bonus.cs
   - Entity class
   - Propiedades: Id, DistributorId, Type, Value, StartDate, EndDate, Status, etc.
   - DbSet property añadido al AldebaranDbContext

?? Aldebaran.DataAccess/Entities/BonusDetail.cs
   - Entity class
   - Relación 1:N con Bonus

?? Aldebaran.DataAccess/Entities/BonusApplication.cs
   - Entity class
   - Historial de aplicación de bonos

?? Aldebaran.DataAccess/Entities/SecondaryDb/BonusSync.cs
   (Si BD secundaria)

?? Aldebaran.DataAccess/Entities/SecondaryDb/BonusApplicationSync.cs
   (Si BD secundaria)
```

### Backend - Configuration

```
?? Aldebaran.DataAccess/Configuration/BonusConfiguration.cs
   - Entity configuration para EF Core
   - HasKey, HasMany, HasOne, Property configurations

?? Aldebaran.DataAccess/Configuration/BonusDetailConfiguration.cs

?? Aldebaran.DataAccess/Configuration/BonusApplicationConfiguration.cs

?? Aldebaran.DataAccess/Configuration/BonusSyncConfiguration.cs
   (Si BD secundaria)
```

### Backend - DbContext

```
?? Aldebaran.DataAccess/SecondaryBonusDbContext.cs
   (Si BD secundaria)
   - Hereda de DbContext
   - DbSets para entidades sincronizadas
   - OnModelCreating
```

### Backend - Repositories

```
?? Aldebaran.DataAccess.Infraestructure/Repository/IBonusRepository.cs
   - Interfaz del repositorio

?? Aldebaran.DataAccess.Infraestructure/Repository/BonusRepository.cs
   - Implementación

?? Aldebaran.DataAccess.Infraestructure/Repository/IBonusDetailRepository.cs

?? Aldebaran.DataAccess.Infraestructure/Repository/BonusDetailRepository.cs

?? Aldebaran.DataAccess.Infraestructure/Repository/IBonusApplicationRepository.cs

?? Aldebaran.DataAccess.Infraestructure/Repository/BonusApplicationRepository.cs

?? Aldebaran.DataAccess.Infraestructure/Repository/Secondary/IBonusSyncRepository.cs
   (Si BD secundaria)

?? Aldebaran.DataAccess.Infraestructure/Repository/Secondary/BonusSyncRepository.cs
   (Si BD secundaria)
```

### Backend - Services

```
?? Aldebaran.Application.Services/Services/IBonusService.cs
   - Interfaz del servicio

?? Aldebaran.Application.Services/Services/BonusService.cs
   - Implementación con lógica de negocio

?? Aldebaran.Application.Services/Services/IBonusDetailService.cs

?? Aldebaran.Application.Services/Services/BonusDetailService.cs

?? Aldebaran.Application.Services/Services/IBonusApplicationService.cs

?? Aldebaran.Application.Services/Services/BonusApplicationService.cs

?? Aldebaran.Application.Services/Services/ISecondaryDbSyncService.cs
   (Si BD secundaria)

?? Aldebaran.Application.Services/Services/SecondaryDbSyncService.cs
   (Si BD secundaria)

?? Aldebaran.Application.Services/Services/IBonusCalculationService.cs
   (Opcional)

?? Aldebaran.Application.Services/Services/BonusCalculationService.cs
   (Opcional)
```

### Backend - External APIs / Integrations

```
?? Aldebaran.Infraestructure.Core/ExternalApis/IProvider1Api.cs
   - Interfaz Refit para API externa

?? Aldebaran.Infraestructure.Core/ExternalApis/Models/Provider1Request.cs

?? Aldebaran.Infraestructure.Core/ExternalApis/Models/Provider1Response.cs

?? Aldebaran.Application.Services/Services/IProvider1IntegrationService.cs

?? Aldebaran.Application.Services/Services/Provider1IntegrationService.cs
   (Si aplica segundo proveedor)

?? Aldebaran.Infraestructure.Core/ExternalApis/IProvider2Api.cs

?? Aldebaran.Infraestructure.Core/ExternalApis/Models/Provider2Request.cs

?? Aldebaran.Infraestructure.Core/ExternalApis/Models/Provider2Response.cs

?? Aldebaran.Application.Services/Services/IProvider2IntegrationService.cs
```

### Backend - DTOs

```
?? Aldebaran.Web/Models/Bonus/CreateBonusDto.cs

?? Aldebaran.Web/Models/Bonus/UpdateBonusDto.cs

?? Aldebaran.Web/Models/Bonus/BonusDetailDto.cs

?? Aldebaran.Web/Models/BonusDetail/CreateBonusDetailDto.cs

?? Aldebaran.Web/Models/BonusDetail/BonusDetailDetailDto.cs

?? Aldebaran.Web/Models/BonusApplication/BonusApplicationDto.cs

?? Aldebaran.Web/Models/BonusApplication/ApplyBonusRequestDto.cs
```

### Backend - Events

```
?? Aldebaran.Infraestructure.Core/Queue/Events/BonusCreatedEvent.cs

?? Aldebaran.Infraestructure.Core/Queue/Events/BonusActivatedEvent.cs

?? Aldebaran.Infraestructure.Core/Queue/Events/BonusAppliedEvent.cs

?? Aldebaran.Infraestructure.Core/Queue/Events/BonusExpiredEvent.cs

?? Aldebaran.Infraestructure.Core/Queue/Events/BonusSyncedEvent.cs
```

### Backend - Controllers

```
?? Aldebaran.Web/Controllers/BonusController.cs
   - GET /api/bonus
   - POST /api/bonus
   - PUT /api/bonus/{id}
   - DELETE /api/bonus/{id}
   - GET /api/bonus/{id}
   - GET /api/bonus/distributor/{distributorId}
   - POST /api/bonus/{id}/apply
   - GET /api/bonus/{id}/applications

?? Aldebaran.Web/Controllers/BonusDetailController.cs

?? Aldebaran.Web/Controllers/BonusApplicationController.cs
```

### Backend - Exceptions

```
?? Aldebaran.Application.Services/Exceptions/BonusNotFoundException.cs

?? Aldebaran.Application.Services/Exceptions/InvalidBonusException.cs

?? Aldebaran.Application.Services/Exceptions/BonusApplicationException.cs

?? Aldebaran.Application.Services/Exceptions/BonusExpiredException.cs
```

### Backend - AutoMapper

```
?? Aldebaran.Web/Mappings/BonusProfile.cs
   - Mappeos de Bonus a DTOs y viceversa
   (Puede añadirse a ViewModelProfile.cs existente)
```

### Backend - Validations

```
?? Aldebaran.Application.Services/Validators/CreateBonusValidator.cs
   (Opcional, si usas FluentValidation)

?? Aldebaran.Application.Services/Validators/ApplyBonusValidator.cs
```

### Backend - Tests

```
?? [Project].Tests/Services/BonusServiceTests.cs

?? [Project].Tests/Services/BonusApplicationServiceTests.cs

?? [Project].Tests/Repositories/BonusRepositoryTests.cs

?? [Project].Tests/Controllers/BonusControllerTests.cs

?? [Project].Tests/Integration/BonusE2ETests.cs
```

### Frontend - Pages

```
?? Aldebaran.Web/Pages/BonusPages/Bonus.razor
   - Página de listado de bonos

?? Aldebaran.Web/Pages/BonusPages/Bonus.razor.cs
   - Code-behind

?? Aldebaran.Web/Pages/BonusPages/AddEditBonus.razor
   - Página de crear/editar

?? Aldebaran.Web/Pages/BonusPages/AddEditBonus.razor.cs

?? Aldebaran.Web/Pages/BonusPages/BonusDetail.razor
   - Página de detalles

?? Aldebaran.Web/Pages/BonusPages/BonusDetail.razor.cs

?? Aldebaran.Web/Pages/BonusPages/BonusApplicationHistory.razor
   - Página de historial

?? Aldebaran.Web/Pages/BonusPages/BonusApplicationHistory.razor.cs
```

### Frontend - Shared Components

```
?? Aldebaran.Web/Shared/BonusComponents/BonusGrid.razor
   - Componente de grid de bonos

?? Aldebaran.Web/Shared/BonusComponents/BonusGrid.razor.cs

?? Aldebaran.Web/Shared/BonusComponents/BonusForm.razor
   - Componente de formulario

?? Aldebaran.Web/Shared/BonusComponents/BonusForm.razor.cs
```

---

## ?? ARCHIVOS A MODIFICAR

### Backend - Program Extensions

```
?? Aldebaran.Web/Extensions/ArchitectureBuilderExtensions.cs
   Cambios:
   - Agregar registro de IBonusService
   - Agregar registro de IBonusDetailService
   - Agregar registro de IBonusApplicationService
   - Agregar registro de ISecondaryDbSyncService
   - Agregar DbContext para BD secundaria
   - Agregar registros de repositorios
   - Agregar registros de clientes Refit (integraciones)
   - Agregar configuración de eventos RabbitMQ
```

### Backend - DbContext

```
?? Aldebaran.DataAccess/AldebaranDbContext.cs
   Cambios:
   - Agregar DbSet<Bonus>
   - Agregar DbSet<BonusDetail>
   - Agregar DbSet<BonusApplication>
   - Agregar ApplyConfiguration<BonusConfiguration>()
   - Agregar ApplyConfiguration<BonusDetailConfiguration>()
   - Agregar ApplyConfiguration<BonusApplicationConfiguration>()
```

### Backend - appsettings.json

```
?? Aldebaran.Web/appsettings.json
   Cambios:
   - Agregar ConnectionString para SecondaryDbConnection
   - Agregar sección ExternalServices (API keys, URLs)
   - Agregar sección SecondaryDbSync
   - Agregar configuración de RabbitMQ Exchanges/Queues

?? Aldebaran.Application.FileWritingService/appsettings.json
   (Si aplica - trabajos de sincronización)

?? Aldebaran.Application.NotificationProcessor/appsettings.json
   (Si aplica - procesamiento de eventos)
```

### Frontend - Layout

```
?? Aldebaran.Web/Shared/MainLayout.razor
   Cambios:
   - Agregar MenuItems para Bonos en RadzenPanelMenu
   - Path: "bonus" o "/bonus"
   - Visibilidad por roles
```

### Frontend - Mappings

```
?? Aldebaran.Web/Mappings/ViewModelProfile.cs
   Cambios:
   - Agregar mappeos de Bonus ? BonusDetailDto
   - Agregar mappeos de BonusDetail ? BonusDetailDetailDto
   - Agregar mappeos de BonusApplication ? BonusApplicationDto
```

---

## ?? MIGRACIONES DE BASE DE DATOS

### EF Core Migrations

```
Migración 1: AddBonusEntities
?? Aldebaran.DataAccess/Migrations/YYYYMMDDHHMM_AddBonusEntities.cs
   - Crear tabla [Bonus]
   - Crear tabla [BonusDetail]
   - Crear tabla [BonusApplication]
   - Crear índices
   - Crear FKs

Migración 2: CreateSecondaryBonusDbContext
?? Aldebaran.DataAccess/Migrations/SecondaryDb/YYYYMMDDHHMM_CreateSecondaryBonusDbContext.cs
   - Crear tabla [BonusSync]
   - Crear tabla [BonusApplicationSync]
   - Crear auditoría de sync
```

### SQL Scripts (Manual)

```
?? scripts/CreateSecondaryDatabase.sql
   - CREATE DATABASE [SecondaryBonusDb]
   - Configuración de collation
   - Configuración de seguridad

?? scripts/SeedInitialData.sql
   - Seed data para tipos de bono
   - Datos iniciales de configuración

?? scripts/RollbackBonusFeature.sql
   - Script para deshacer cambios (si es necesario)
```

---

## ??? ARCHIVOS A ELIMINAR

```
[ ] Ninguno previsto (Solo adiciones/modificaciones)
```

---

## ?? Resumen de Cambios

| Categoría | Crear | Modificar | Eliminar | Total |
|-----------|-------|-----------|----------|-------|
| Entidades | 5 | 1 | 0 | 6 |
| Repositorios | 7 | 0 | 0 | 7 |
| Servicios | 5 | 1 | 0 | 6 |
| Controllers | 3 | 0 | 0 | 3 |
| DTOs | 7 | 0 | 0 | 7 |
| Frontend Pages | 8 | 0 | 0 | 8 |
| Frontend Components | 4 | 0 | 0 | 4 |
| Integraciones | 5 | 0 | 0 | 5 |
| Tests | 5 | 0 | 0 | 5 |
| Config/Migraciones | 3 | 2 | 0 | 5 |
| **TOTAL** | **52** | **4** | **0** | **56** |

---

## ?? Orden de Implementación Recomendado

1. **Entidades** ? Crear clases y configuraciones
2. **DbContext** ? Agregar DbSets y migraciones
3. **Repositorios** ? Implementar acceso a datos
4. **DTOs** ? Crear transfer objects
5. **Servicios** ? Implementar lógica de negocio
6. **Controllers** ? Crear endpoints
7. **IntegracionesExternas** ? Clientes Refit
8. **Eventos** ? Configurar RabbitMQ
9. **Frontend** ? Crear páginas y componentes
10. **Tests** ? Implementar cobertura de tests
11. **Documentación** ? Swagger, README
12. **Despliegue** ? Scripts de migración

---

## ? Checklist de Revisión Pre-Código

Antes de empezar a codificar, validar:

```
[ ] Documento 1_REQUERIMIENTOS.md 100% completo
[ ] Documento 2_ARQUITECTURA.md 100% aprobado
[ ] Documento 3_ENTIDADES_Y_MODELOS.md detallado
[ ] Documento 4_SERVICIOS_Y_APIS.md definido
[ ] Documento 5_INTEGRACIONES_TERCEROS.md claro
[ ] Documento 6_SEGUNDA_BASE_DATOS.md listo
[ ] Este documento (9_CAMBIOS_CODIGO.md) revisado
[ ] Ambiente configurado (BD2, APIs sandbox)
[ ] Equipo capacitado
[ ] Aprobación final del Product Owner
```

---

## ?? Notas Técnicas

> [Aquí irán consideraciones de implementación]

---

**Última actualización**: [Pendiente]
**Responsable**: [Arquitecto / Developer]
**Estado**: ?? Incompleto - Pendiente lista final de cambios
