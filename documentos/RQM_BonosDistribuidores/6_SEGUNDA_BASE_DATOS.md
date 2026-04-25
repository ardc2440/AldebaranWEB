# 6. SEGUNDA BASE DE DATOS SQL SERVER - Bonos Distribuidores

## Status: ? PENDIENTE DEFINICIÓN

---

## ?? Secciones a Documentar

### 6.1 Información General de la Segunda BD

```
Nombre de la BD: ?
Nombre del Servidor: ?
Puerto: ? (default: 1433)
Instancia: ? (si aplica)
Ubicación: [ ] Mismo servidor [ ] Servidor distinto
Propietario/Administrador: ?
Contacto de soporte: ?
```

### 6.2 Conexión y Autenticación

```
Tipo de autenticación: [ ] SQL Server [ ] Windows [ ] Ambos

Credenciales SQL Server:
  [ ] Usuario: ?
  [ ] Password: ?
  [ ] Rol en BD: ?

Windows Auth:
  [ ] Service Account: ?
  [ ] Permisos: ?

Connection String:
  [ ] Server=?;Database=?;User Id=?;Password=?;Encrypt=true;TrustServerCertificate=?;Connection Timeout=30;

Dónde se almacena:
  [ ] appsettings.json ? SecondaryDbConnection
  [ ] Azure Key Vault
  [ ] User Secrets
  [ ] Variables de entorno

Certificados SSL/TLS:
  [ ] ¿Requerido?
  [ ] Ubicación: ?
  [ ] Validación: ?
```

### 6.3 Esquemas y Tablas

#### 6.3.1 Estructura Existente (a investigar)
```
Esquemas en la BD:
  [ ] dbo
  [ ] [Otro esquema 1]
  [ ] [Otro esquema 2]

Tablas principales:
  [ ] Tabla 1
      - Propósito: ?
      - Registros aprox: ?
      - Índices: ?

  [ ] Tabla 2
      - Propósito: ?

  [ ] Tabla N
      - Propósito: ?

Views (si existen):
  [ ] View 1: ?
  [ ] View 2: ?

Stored Procedures (si existen):
  [ ] SP 1: ?
  [ ] SP 2: ?

Functions:
  [ ] Function 1: ?
```

#### 6.3.2 Estructura Nueva (si aplica)
```
Nuevas tablas a crear:
  [ ] [Tabla 1]
  [ ] [Tabla 2]

Nuevos esquemas:
  [ ] [Esquema 1]
```

### 6.4 Relación Entre Bases de Datos

```
Tipo de relación:
  [ ] Master-Detail (BD1 = maestro, BD2 = detalles)
  [ ] Síncrona (datos duplicados para reportes)
  [ ] Referencial (FK entre BDs)
  [ ] Independiente (sin relación directa)
  [ ] Otros: ?

Datos que se sincronizan:
  [ ] Bonos principales ? BD2
  [ ] Detalles de bonos ? BD2
  [ ] Historial de aplicación ? BD2
  [ ] Otros: ?

Dirección de sincronización:
  [ ] BD1 ? BD2 (unidireccional)
  [ ] BD1 ? BD2 (bidireccional)
  [ ] Bajo demanda
  [ ] En tiempo real
  [ ] Periódica cada X minutos

Conflictos (si bidireccional):
  [ ] Estrategia de resolución: [ ] Última escritura gana [ ] Manualaprobación [ ] Otros
```

### 6.5 Estrategia de Persistencia en .NET

#### Opción A: DbContext Separado (Recomendado)
```csharp
// Crear nuevo DbContext
public class SecondaryBonusDbContext : DbContext
{
    public SecondaryBonusDbContext(DbContextOptions<SecondaryBonusDbContext> options)
        : base(options)
    {
    }

    public DbSet<BonusSync> BonusSync { get; set; }
    public DbSet<BonusApplicationSync> BonusApplicationSync { get; set; }
    // Otros DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuraciones
    }
}

// Ubicación: Aldebaran.DataAccess/SecondaryBonusDbContext.cs

// Registro en Program.cs:
services.AddDbContext<SecondaryBonusDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SecondaryDbConnection")),
    ServiceLifetime.Scoped, ServiceLifetime.Scoped);
```

#### Opción B: Context Compartido
```csharp
// [Si los datos pertenecen al mismo modelo lógico]
// Agregar DbSets al AldebaranDbContext existente
```

### 6.6 Modelos de Sincronización

```csharp
Entidades para BD2:

[ ] BonusSync
    - Id (PK)
    - BonusId (from BD1)
    - DistributorId
    - Value
    - Type
    - StartDate
    - EndDate
    - Status
    - SyncDate
    - Otros: ?

[ ] BonusApplicationSync
    - Id (PK)
    - ApplicationId (from BD1)
    - BonusId
    - DocumentReference
    - AppliedAmount
    - ApplicationDate
    - Status
    - SyncDate
    - Otros: ?

[ ] Otros modelos: ?

Ubicación: Aldebaran.DataAccess/Entities/SecondaryDb/
```

### 6.7 Migraciones EF Core

```
Migración 1: CreateSecondaryBonusDbContext
  - Crear tabla BonusSync
  - Crear tabla BonusApplicationSync
  - Crear índices
  - Crear constraints

Ubicación: Aldebaran.DataAccess/Migrations/SecondaryDb/

Comandos:
  [ ] dotnet ef migrations add CreateSecondaryBonusDbContext -c SecondaryBonusDbContext -o Migrations/SecondaryDb
  [ ] dotnet ef database update -c SecondaryBonusDbContext
```

### 6.8 Repositorios para BD2

```csharp
[ ] IBonusSyncRepository : IGenericRepository<BonusSync>
    Métodos:
      - GetSyncedBonusesAsync()
      - GetUnsyncedBonusesAsync()
      - MarkAsSyncedAsync(bonusId)
      - Otros: ?

[ ] IBonusApplicationSyncRepository : IGenericRepository<BonusApplicationSync>
    Métodos:
      - GetSyncedApplicationsAsync()
      - GetUnsyncedApplicationsAsync()
      - Otros: ?

Ubicación: Aldebaran.DataAccess.Infraestructure/Repository/Secondary/
```

### 6.9 Servicio de Sincronización

```csharp
Interfaz: ISecondaryDbSyncService

Métodos:
  [ ] SyncBonusesAsync(bonusIds)
      Lógica:
        1. Obtener bonos de BD1
        2. Mapear a BonusSync
        3. Insertar/actualizar en BD2
        4. Registrar fecha de sync
        5. Publicar evento BonusSyncedEvent

  [ ] SyncBonusApplicationsAsync(applicationIds)
      Lógica: [Similar]

  [ ] FullSyncAsync()
      Lógica:
        1. Sincronizar todos los bonos
        2. Sincronizar todas las aplicaciones
        3. Limpiar datos expirados

  [ ] ReverseSync(secondaryDataId)
      Lógica:
        1. Obtener dato de BD2
        2. Actualizar en BD1
        3. Registrar cambios

Ubicación: Aldebaran.Application.Services/Services/SecondaryDbSyncService.cs
```

### 6.10 Estrategia de Sincronización

#### Opción A: Síncrona (Inmediata)
```csharp
// Al crear/actualizar bono en BD1, sincronizar inmediatamente a BD2
public async Task CreateBonusAsync(CreateBonusDto dto)
{
    var bonus = _mapper.Map<Bonus>(dto);
    await _bonusRepository.CreateAsync(bonus);

    // Sincronización inmediata
    await _secondaryDbSyncService.SyncBonusesAsync(new[] { bonus.Id });
}
```

#### Opción B: Asíncrona (con RabbitMQ)
```csharp
// Publicar evento, worker lo consume y sincroniza

public async Task CreateBonusAsync(CreateBonusDto dto)
{
    var bonus = _mapper.Map<Bonus>(dto);
    await _bonusRepository.CreateAsync(bonus);

    // Publicar evento
    await _queue.PublishAsync(new BonusCreatedEvent 
    { 
        BonusId = bonus.Id,
        CreatedDate = DateTime.UtcNow
    });
}

// En NotificationProcessor Worker:
public async Task ProcessBonusCreatedEventAsync(BonusCreatedEvent evt)
{
    await _secondaryDbSyncService.SyncBonusesAsync(new[] { evt.BonusId });
}
```

#### Opción C: Periódica (Scheduled)
```csharp
// Ejecutar cada X minutos/horas
// En HostedService o NCrontab

[0 * * * *]  // Cada hora
await _secondaryDbSyncService.FullSyncAsync();
```

### 6.11 Manejo de Errores y Reintentos

```csharp
Estrategia:
  [ ] Retry con backoff exponencial
  [ ] Registrar error en Log
  [ ] Alertar a administrador si falla N veces
  [ ] Dead Letter Queue para fallos persistentes
  [ ] Manual review requerido

Implementación:
  using (var tx = new TransactionScope(...))
  {
      try
      {
          await _secondaryDbSyncService.SyncBonusesAsync(bonusIds);
          // Éxito
      }
      catch (DbUpdateException ex)
      {
          _logger.LogError(ex, "Sync failed for bonuses {bonusIds}", bonusIds);
          // Reintentar o alertar
          throw;
      }
  }
```

### 6.12 Monitoreo y Auditoría

```
Tabla de Auditoría: SecondaryDbSyncAudit
  - SyncId (PK)
  - EntityType (Bonus, BonusApplication, etc.)
  - EntityIds
  - SyncDateTime
  - Status (Success, Failed, Pending)
  - ErrorMessage (si aplica)
  - RecordsAffected
  - DurationMs
  - User/Service que disparó sync

Consultas de Monitoreo:
  [ ] SELECT COUNT(*) FROM BonusSync WHERE SyncDate IS NULL
      ? Registros pendientes de sincronizar

  [ ] SELECT * FROM SecondaryDbSyncAudit WHERE Status = 'Failed' ORDER BY SyncDateTime DESC LIMIT 10
      ? Últimos fallos

  [ ] SELECT AVG(DurationMs) FROM SecondaryDbSyncAudit WHERE SyncDateTime > DATEADD(HOUR, -24, GETUTCDATE())
      ? Tiempo promedio de sync en últimas 24h
```

### 6.13 Queries Importantes en BD2

```sql
-- Bonos activos para reporte
SELECT * 
FROM BonusSync 
WHERE Status = 'Activo' 
  AND StartDate <= GETUTCDATE() 
  AND EndDate >= GETUTCDATE()

-- Historial de aplicación
SELECT *
FROM BonusApplicationSync
WHERE BonusId = @BonusId
ORDER BY ApplicationDate DESC

-- Estadísticas por distribuidor
SELECT 
    DistributorId,
    COUNT(*) as TotalBonos,
    SUM(Value) as TotalValor,
    COUNT(DISTINCT CAST(ApplicationDate AS DATE)) as DíasAplicación
FROM BonusSync bs
LEFT JOIN BonusApplicationSync bas ON bs.Id = bas.BonusId
GROUP BY DistributorId

-- Otros: [Por completar]
```

### 6.14 Configuración en appsettings.json

```json
{
  "ConnectionStrings": {
    "AldebaranDbConnection": "Server=...;Initial Catalog=Aldebaran;...",
    "LogDbConnection": "Server=...;Initial Catalog=AldebaranLogs;...",
    "SecondaryDbConnection": "Server=SERVER2;Database=SecondaryBonusDb;User Id=sa;Password=???;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;"
  },
  "SecondaryDbSync": {
    "Enabled": true,
    "SyncStrategy": "Async",  // Async, Sync, Scheduled
    "SyncInterval": 3600,  // segundos (si Scheduled)
    "BatchSize": 100,
    "MaxRetries": 3,
    "RetryDelayMs": 5000
  }
}
```

---

## ?? Referencias Cruzadas

- Ver: **3_ENTIDADES_Y_MODELOS.md** - Modelos de datos
- Ver: **2_ARQUITECTURA.md** - Decisiones de persistencia
- Ver: **10_MIGRACIONES_BD.md** - Scripts de migración
- Ver: **4_SERVICIOS_Y_APIS.md** - Servicios de sincronización

---

## ?? Notas sobre Segunda BD

> [Aquí irán decisiones técnicas]

---

**Última actualización**: [Pendiente]
**Responsable**: [Usuario]
**Estado**: ?? Incompleto - Pendiente información de BD secundaria
