# 11. CONFIGURACIÓN Y DESPLIEGUE - Bonos Distribuidores

## Status: ? PENDIENTE DETALLES

---

## ?? Resumen

Este documento cubre la configuración de infraestructura y el procedimiento de despliegue en todos los ambientes.

---

## 1?? CONFIGURACIÓN DE AMBIENTES

### 1.1 Variables de Entorno y appsettings

#### appsettings.json (Base)

```json
{
  "ConnectionStrings": {
    "AldebaranDbConnection": "Server=localhost;Database=Aldebaran;Trusted_Connection=true;Encrypt=false;MultipleActiveResultSets=true;",
    "LogDbConnection": "Server=localhost;Database=AldebaranLogs;Trusted_Connection=true;Encrypt=false;MultipleActiveResultSets=true;",
    "SecondaryDbConnection": "Server=SECONDARY_SERVER;Database=SecondaryBonusDb;User Id=sa;Password=???;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;"
  },
  "ExternalServices": {
    "Provider1": {
      "BaseUrl": "https://api.provider1.com/v1",
      "ApiKey": "${PROVIDER1_API_KEY}",
      "ClientId": "${PROVIDER1_CLIENT_ID}",
      "ClientSecret": "${PROVIDER1_CLIENT_SECRET}",
      "Timeout": 30,
      "RetryCount": 3,
      "RetryDelay": 1000,
      "EnableLogging": true
    },
    "Provider2": {
      "BaseUrl": "https://api.provider2.com/v1",
      "ApiKey": "${PROVIDER2_API_KEY}",
      "Timeout": 30,
      "RetryCount": 3,
      "RetryDelay": 1000
    }
  },
  "SecondaryDbSync": {
    "Enabled": true,
    "SyncStrategy": "Async",
    "SyncInterval": 3600,
    "BatchSize": 100,
    "MaxRetries": 3,
    "RetryDelayMs": 5000,
    "EnableAudit": true
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchanges": [
      {
        "Name": "bonuses.exchange",
        "Type": "topic",
        "Durable": true
      }
    ],
    "Queues": [
      {
        "Name": "bonus.apply.queue",
        "Exchange": "bonuses.exchange",
        "RoutingKey": "bonus.apply.*"
      },
      {
        "Name": "bonus.sync.queue",
        "Exchange": "bonuses.exchange",
        "RoutingKey": "bonus.sync.*"
      },
      {
        "Name": "bonus.apply.deadletter",
        "Exchange": "bonuses.deadletter.exchange",
        "RoutingKey": "bonus.apply.deadletter"
      }
    ]
  },
  "AppSettings": {
    "TrackEnabled": true
  }
}
```

#### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "AldebaranDbConnection": "Server=(localdb)\\mssqllocaldb;Database=Aldebaran_Dev;Trusted_Connection=true;",
    "LogDbConnection": "Server=(localdb)\\mssqllocaldb;Database=AldebaranLogs_Dev;Trusted_Connection=true;",
    "SecondaryDbConnection": "Server=(localdb)\\mssqllocaldb;Database=SecondaryBonusDb_Dev;Trusted_Connection=true;"
  },
  "ExternalServices": {
    "Provider1": {
      "BaseUrl": "https://sandbox.provider1.com/v1",
      "ApiKey": "sandbox_key_123",
      "EnableLogging": true
    },
    "Provider2": {
      "BaseUrl": "https://sandbox.provider2.com/v1",
      "ApiKey": "sandbox_key_456"
    }
  },
  "SecondaryDbSync": {
    "Enabled": true,
    "SyncStrategy": "Sync",
    "SyncInterval": 60,
    "BatchSize": 10
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672
  }
}
```

#### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "AldebaranDbConnection": "Server=PROD_SERVER;Database=Aldebaran;User Id=aldebaran_user;Password=???;Encrypt=true;TrustServerCertificate=false;",
    "LogDbConnection": "Server=PROD_SERVER;Database=AldebaranLogs;User Id=logs_user;Password=???;Encrypt=true;",
    "SecondaryDbConnection": "Server=SECONDARY_PROD_SERVER;Database=SecondaryBonusDb;User Id=secondary_user;Password=???;Encrypt=true;TrustServerCertificate=false;"
  },
  "ExternalServices": {
    "Provider1": {
      "BaseUrl": "https://api.provider1.com/v1",
      "Timeout": 30,
      "RetryCount": 5,
      "RetryDelay": 2000,
      "EnableLogging": false
    }
  },
  "SecondaryDbSync": {
    "Enabled": true,
    "SyncStrategy": "Async",
    "SyncInterval": 3600,
    "MaxRetries": 5,
    "EnableAudit": true
  }
}
```

---

### 1.2 User Secrets (.NET User Secrets)

```bash
# Para desarrollo local, guardar secrets sensibles:
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:SecondaryDbConnection" "Server=...;Password=..."
dotnet user-secrets set "ExternalServices:Provider1:ApiKey" "..."
dotnet user-secrets set "ExternalServices:Provider1:ClientSecret" "..."

# Verificar
dotnet user-secrets list
```

---

### 1.3 Azure Key Vault (si aplica)

```csharp
// En Program.cs
if (!app.Environment.IsDevelopment())
{
    var keyVaultUrl = new Uri($"https://{configuration["KeyVault:Name"]}.vault.azure.net/");
    var credential = new DefaultAzureCredential();
    builder.Configuration.AddAzureKeyVault(keyVaultUrl, credential);
}

// Secrets a crear en Key Vault:
// - ConnectionStrings--SecondaryDbConnection
// - ExternalServices--Provider1--ApiKey
// - ExternalServices--Provider1--ClientSecret
// - RabbitMQ--UserName
// - RabbitMQ--Password
```

---

## 2?? CONFIGURACIÓN DE INFRAESTRUCTURA

### 2.1 Base de Datos Secundaria

#### Creación Inicial

```sql
-- En PROD:
CREATE DATABASE [SecondaryBonusDb]
    COLLATE SQL_Latin1_General_CP1_CI_AS;

USE [SecondaryBonusDb];

-- Crear user para la aplicación
CREATE LOGIN [secondary_user] WITH PASSWORD = '???';
CREATE USER [secondary_user] FOR LOGIN [secondary_user];

-- Asignar permisos
ALTER ROLE db_owner ADD MEMBER [secondary_user];
-- O más restrictivo:
-- ALTER ROLE db_datareader ADD MEMBER [secondary_user];
-- ALTER ROLE db_datawriter ADD MEMBER [secondary_user];

-- Configurar backup automático
-- [Según política de backup de empresa]
```

#### Configuración de Backup

```sql
-- Backup automático cada noche
USE [msdb]
GO

EXEC dbo.sp_add_schedule
    @schedule_name = N'Secondary_DB_Daily_Backup',
    @freq_type = 4,
    @freq_interval = 1,
    @active_start_time = 020000
GO

-- [Completar con detalles de backup]
```

### 2.2 RabbitMQ

#### Configuración de Exchanges y Queues

```bash
# Usando CLI o Management UI de RabbitMQ

# 1. Crear Exchange
rabbitmqctl add_exchange bonuses topic

# 2. Crear Queues
rabbitmqctl add_queue bonus.apply.queue
rabbitmqctl add_queue bonus.sync.queue
rabbitmqctl add_queue bonus.apply.deadletter

# 3. Bindings
rabbitmqctl add_binding bonus.exchange bonus.apply.queue "bonus.apply.*"
rabbitmqctl add_binding bonus.exchange bonus.sync.queue "bonus.sync.*"

# 4. Dead Letter Exchange
rabbitmqctl add_exchange bonuses.deadletter topic
rabbitmqctl add_binding bonuses.deadletter bonus.apply.deadletter "bonus.apply.deadletter"

# 5. Establecer TTL
rabbitmqctl set_queue_arg bonus.apply.queue "x-message-ttl" 86400000  # 24 horas
```

---

### 2.3 Configuración de Seguridad

#### Certificados SSL/TLS

```bash
# Para SQL Server
# Validar que TrustServerCertificate se ajuste según política

# Para APIs (Refit)
# Los certificados se validan automáticamente con HttpClient

# Para RabbitMQ (si es necesario)
# Configurar con credenciales por separado

# Usar Azure Key Vault para gestionar certificados
```

#### Firewall y NSG (Azure)

```
Reglas necesarias:
  [ ] Permitir acceso BD primaria (puerto 1433) desde App Service
  [ ] Permitir acceso BD secundaria (puerto 1433) desde App Service
  [ ] Permitir acceso RabbitMQ (puerto 5672) desde servidores
  [ ] Permitir acceso a APIs externas (puertos 443)
  [ ] Restricción IP si aplica
```

---

## 3?? PROCEDIMIENTO DE DESPLIEGUE

### 3.1 Pre-Despliegue (Staging)

```bash
# 1. Build de aplicación
dotnet build -c Release

# 2. Tests
dotnet test --configuration Release

# 3. Publish
dotnet publish -c Release -o ./publish

# 4. Validar artefactos
ls -la ./publish

# 5. Crear imagen Docker (si aplica)
docker build -t aldebaran-web:bonus-feature-v1.0 .

# 6. Push a registry
docker push aldebaran-web:bonus-feature-v1.0

# 7. Deploy a STAGING
# [Usando Azure DevOps, GitHub Actions, o manual]
```

### 3.2 Despliegue en DEV

```bash
# 1. Desplegar aplicación
dotnet publish -c Debug -o ./publish
# Copiar a servidor DEV

# 2. Aplicar migraciones
dotnet ef database update -c AldebaranDbContext
dotnet ef database update -c SecondaryBonusDbContext

# 3. Validar
curl https://dev.aldebaran.com/api/bonus
```

### 3.3 Despliegue en QA

```bash
# 1. Backup previo
BACKUP DATABASE [Aldebaran] TO DISK = 'C:\Backups\Aldebaran_PreBonus_QA.bak'
BACKUP DATABASE [SecondaryBonusDb] TO DISK = 'C:\Backups\SecondaryBonusDb_PreBonus_QA.bak'

# 2. Detener servicios
# Parar NotificationProcessor y FileWritingService si es necesario

# 3. Desplegar
# [Copiar archivos compilados a servidor QA]

# 4. Aplicar migraciones
dotnet ef database update -c AldebaranDbContext
dotnet ef database update -c SecondaryBonusDbContext

# 5. Iniciar servicios
# Restart NotificationProcessor y FileWritingService

# 6. Validar
# [Pruebas manuales de funcionalidad]
```

### 3.4 Despliegue en STAGING

```bash
# [Proceso similar a QA, con validación adicional]
```

### 3.5 Despliegue en PRODUCCIÓN

#### Checklist Pre-Producción

```
[ ] Build exitoso sin warnings críticos
[ ] Todos los tests pasando (cobertura > 80%)
[ ] Documentación técnica completa
[ ] Plan de rollback documentado
[ ] Equipo de soporte capacitado
[ ] Aprobación de cambios (Change Advisory Board)
[ ] Ventana de mantenimiento confirmada
[ ] Comunicación a usuarios finales
[ ] Monitoreo post-despliegue configurado
```

#### Procedimiento de Despliegue PROD

```bash
# 1. Backup full de ambas BDs
BACKUP DATABASE [Aldebaran] TO DISK = 'E:\Backups\Aldebaran_PreBonus_PROD_$(date).bak'
BACKUP DATABASE [SecondaryBonusDb] TO DISK = 'E:\Backups\SecondaryBonusDb_PreBonus_PROD_$(date).bak'

# 2. Notificar a usuarios
# [Enviar comunicado de mantenimiento]

# 3. Detener aplicación (ventana de mantenimiento)
# [Parar IIS / App Service]

# 4. Detener servicios Windows
net stop "Notification Processor"
net stop "Ftp File Writing Service"

# 5. Desplegar código
# [Copiar archivos compilados]
# [Verificar integridad de archivos]

# 6. Aplicar migraciones
dotnet ef database update -c AldebaranDbContext
dotnet ef database update -c SecondaryBonusDbContext

# 7. Iniciar servicios
net start "Notification Processor"
net start "Ftp File Writing Service"

# 8. Iniciar aplicación
# [Start IIS / App Service]

# 9. Smoke Tests
curl https://aldebaran.com/api/bonus
curl https://aldebaran.com/api/bonus/1

# 10. Validaciones
# - [ ] Aplicación responde (200 OK)
# - [ ] BD secundaria sincroniza
# - [ ] RabbitMQ procesa eventos
# - [ ] APIs terceros responden
# - [ ] Logs sin errores críticos

# 11. Monitoreo
# [Vigilar métricas por 30 minutos]

# 12. Notificar completación
# [Enviar comunicado a usuarios]
```

---

## 4?? ROLLBACK PROCEDURE

### Escenario 1: Error Inmediato (dentro de 5 minutos)

```bash
# 1. Parar aplicación
net stop "Notification Processor"
net stop "Ftp File Writing Service"
# [Stop IIS / App Service]

# 2. Revertir código
# [Reemplazar archivos con versión anterior]

# 3. Revertir BD
dotnet ef database update [MigraciónAnterior] -c AldebaranDbContext
# O
RESTORE DATABASE [Aldebaran] FROM DISK = 'E:\Backups\Aldebaran_PreBonus_PROD.bak'

# 4. Reiniciar
net start "Notification Processor"
net start "Ftp File Writing Service"
# [Start IIS / App Service]

# 5. Validar
curl https://aldebaran.com/api/

# 6. Notificar
```

### Escenario 2: Error Posterior (> 5 minutos)

```bash
# [Análisis del error]
# [Posible opción: parche rápido]
# [Si no es viable: proceder con rollback completo]
```

---

## 5?? MONITOREO POST-DESPLIEGUE

### 5.1 Dashboards y Alertas

```
Métricas a monitorear:
  [ ] Availability (Target: > 99.5%)
  [ ] Response Time (Target: < 500ms)
  [ ] Error Rate (Target: < 0.1%)
  [ ] Database Performance (CPU, IO, Connections)
  [ ] RabbitMQ Queue Depth
  [ ] External API Response Times
  [ ] Secondary DB Sync Success Rate

Alertas a configurar:
  [ ] CPU > 80% por 5 minutos
  [ ] Response Time > 1000ms
  [ ] Error Rate > 1%
  [ ] DB Connections > 90% capacity
  [ ] RabbitMQ Queue Depth > 1000 messages
  [ ] Sync Failures > 3 veces consecutivas
```

### 5.2 Logs a Revisar

```
Archivos de log:
  [ ] Application Logs (Serilog SQL)
  [ ] IIS Logs
  [ ] Windows Event Logs (Servicios)
  [ ] RabbitMQ Logs
  [ ] SQL Server Error Log
  [ ] Secondary DB Sync Audit

Consultas de validación:
  SELECT TOP 50 * FROM log.logs WHERE Source = 'Aldebaran.Web' ORDER BY TimeStamp DESC
  SELECT * FROM [SecondaryBonusDb].dbo.SyncAudit WHERE Status = 'Failed' ORDER BY SyncDateTime DESC
```

---

## 6?? CONFIGURACIÓN DE CI/CD

### 6.1 GitHub Actions (Ejemplo)

```yaml
# .github/workflows/deploy-bonus.yml

name: Deploy Bonus Feature

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '7.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Test
        run: dotnet test --configuration Release --no-build --verbosity normal

      - name: Publish
        run: dotnet publish -c Release -o ./publish

      - name: Docker Build
        run: docker build -t ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:bonus-feature .

      - name: Docker Push
        run: docker push ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:bonus-feature

  deploy-dev:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/develop'
    steps:
      - name: Deploy to Dev
        run: |
          # [Script de despliegue a DEV]
          echo "Deploying to DEV..."

  deploy-prod:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    environment: production
    steps:
      - name: Deploy to Prod
        run: |
          # [Script de despliegue a PROD con migraciones]
          echo "Deploying to PROD..."
```

---

## 7?? CHECKLIST FINAL

```
Pre-Despliegue:
  [ ] Código en rama correcta
  [ ] Tests pasando 100%
  [ ] Documentación actualizada
  [ ] Migraciones probadas en STAGING
  [ ] Configuración validada
  [ ] Backups creados
  [ ] Equipo en standby
  [ ] Usuarios notificados

Post-Despliegue:
  [ ] Aplicación responde
  [ ] APIs funcionando
  [ ] BD sincronizando
  [ ] Logs sin errores críticos
  [ ] Monitoreo activo
  [ ] Usuarios confirmando OK
  [ ] Documentación de despliegue completada
```

---

## ?? Notas de Configuración

> [Aquí irán ajustes específicos por ambiente]

---

**Última actualización**: [Pendiente]
**Responsable**: [DevOps / SRE]
**Estado**: ?? Incompleto - Pendiente detalles de despliegue
