# 3. TAREAS DE DESAROLLO - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Rama**: RQM_BonosDistribuidores_052026  
**Fecha**: Mayo 2026  
**Versión**: 1.0

---

## Convenciones

| Símbolo | Capa |
|---------|------|
| ??? | Base de Datos |
| ?? | Backend – DataAccess / Services |
| ??? | Frontend – Blazor (Aldebaran.Web) |

| Estado | Significado |
|--------|-------------|
| ? | Pendiente |
| ?? | En progreso |
| ? | Completado |

---

## 2.1 Módulo de Administración

---

### 2.1.1 Configuración de Clientes Distribuidores

> Ref. propuesta funcional: sección 2.1.1 (HTML v1.0) / sección 2.2.0.1 + 2.2.0.2 (MD v1.4)  
> Permite marcar qué Clientes son tipo **DISTRIBUIDOR** y configurarles un **Email de Bonificación** independiente del email general.

---

#### TAREA-001 ? ???
**Agregar columnas `IsDistributor` y `BonusEmail` a la tabla `Customers`**

**Contexto del código:**  
La tabla `Customers` actualmente no tiene ningún indicador de tipo distribuidor ni campo de email de bonificación.

**Cambios:**
- Agregar columna `IsDistributor BIT NOT NULL DEFAULT 0`
- Agregar columna `BonusEmail NVARCHAR(254) NULL`
- Script de migración: registros existentes quedan con `IsDistributor = 0` y `BonusEmail = NULL`

**Archivo a crear/modificar:**
- `scripts/` ? nuevo script de migración (ej: `AddDistributorFieldsToCustomers.sql`)

---

#### TAREA-002 ? ??
**Agregar propiedades `IsDistributor` y `BonusEmail` a las entidades y modelos**

**Contexto del código:**
- `Aldebaran.DataAccess\Entities\Customer.cs` ? tiene: `CustomerId`, `IdentityNumber`, `CustomerName`, `Email`, `CellPhone`, etc. Falta `IsDistributor` y `BonusEmail`
- `Aldebaran.Application.Services\Models\Customer.cs` ? misma situación

**Cambios:**
- `Aldebaran.DataAccess\Entities\Customer.cs` ? agregar:
  ```csharp
  public bool IsDistributor { get; set; }
  public string BonusEmail { get; set; }
  ```
- `Aldebaran.Application.Services\Models\Customer.cs` ? agregar las mismas propiedades
- Actualizar el `CustomerRepository` para incluir los campos en SELECT e INSERT/UPDATE
- Actualizar el `CustomerService` para incluir los campos en las operaciones CRUD

---

#### TAREA-003 ? ??
**Agregar validación de negocio en `CustomerService`: reglas para `IsDistributor` y `BonusEmail`**

**Contexto del código:**  
`EditCustomer.razor.cs` llama `CustomerService.UpdateAsync(CUSTOMER_ID, Customer)` directamente. Las reglas de negocio deben vivir en el servicio.

**Reglas a implementar:**
- Si `IsDistributor = true` ? `BonusEmail` es **obligatorio** y debe tener formato de email válido
- Si `IsDistributor = false` ? `BonusEmail` puede ser null
- `BonusEmail` máximo 254 caracteres (RFC 5321)
- Un cliente no puede tener dos clasificaciones simultáneas (ya está implícito con bool, pero validar consistencia)
- ?? Restricción futura: no se puede desmarcar `IsDistributor` si tiene sesiones activas de bonificación (implementar cuando exista tabla de sesiones - **TAREA dependiente de módulo OTP**)

---

#### TAREA-004 ? ???
**Agregar columna "Es Distribuidor" en el listado `Customers.razor`**

**Contexto del código:**  
`Customers.razor` tiene una `RadzenDataGrid` con columnas: Nombre, Tipo doc, Número doc, Teléfono, Celular, Correo, Dirección, Ubicación. No tiene columna de distribuidor.

**Cambios:**
- Agregar columna `Es Distribuidor` en la grilla con un ícono o badge visual (ej: `RadzenIcon` check/cross o `RadzenBadge`)
- Agregar opción de filtro por distribuidor (checkbox o dropdown: Todos / Solo Distribuidores / Solo No Distribuidores) encima de la grilla
- Actualizar `GetCustomersAsync` en `Customers.razor.cs` para pasar el filtro al servicio

---

#### TAREA-005 ? ???
**Agregar campos de distribuidor en el formulario `EditCustomer.razor`**

**Contexto del código:**  
`EditCustomer.razor` tiene los campos: Tipo doc, Número doc, Nombre, Localización, Dirección, Celular, Teléfono, Teléfono opcional, Fax, Correo electrónico (chips multi-email). No tiene campos de distribuidor.

**Cambios en `EditCustomer.razor`:**
- Agregar `RadzenCheckBox` para `IsDistributor` con label "Es Distribuidor"
- Agregar `RadzenTextBox` para `BonusEmail` con label "Email de Bonificación"
  - Visible **siempre** (no solo cuando IsDistributor = true), para facilitar configuración previa
  - Validación requerida: solo obligatorio si `IsDistributor = true`
  - Validación de formato email (regex)
  - Indicación visual que es independiente del correo electrónico general
- `RadzenRequiredValidator` condicional para `BonusEmail` cuando `IsDistributor = true`

**Cambios en `EditCustomer.razor.cs`:**
- Los campos se mapean automáticamente al `Customer` existente, no requiere lógica adicional en el componente (la validación vive en TAREA-003)

---

#### TAREA-006 ? ???
**Agregar campos de distribuidor en el formulario `AddCustomer.razor`**

**Contexto del código:**  
`AddCustomer.razor` es el formulario de creación, estructura idéntica a `EditCustomer.razor`. Mismos campos, misma situación.

**Cambios:**  
Idénticos a TAREA-005 pero aplicados en `AddCustomer.razor` y `AddCustomer.razor.cs`.

**Nota:** Al crear un cliente nuevo con `IsDistributor = true`, el `BonusEmail` es obligatorio desde el inicio.

---

#### Resumen de archivos afectados

| Archivo | Tarea | Tipo de cambio |
|---------|-------|----------------|
| `scripts/AddDistributorFieldsToCustomers.sql` | 001 | Nuevo |
| `Aldebaran.DataAccess\Entities\Customer.cs` | 002 | Modificar |
| `Aldebaran.Application.Services\Models\Customer.cs` | 002 | Modificar |
| `CustomerRepository` | 002 | Modificar |
| `CustomerService` | 002, 003 | Modificar |
| `Pages\CustomerPages\Customers.razor` | 004 | Modificar |
| `Pages\CustomerPages\Customers.razor.cs` | 004 | Modificar |
| `Pages\CustomerPages\EditCustomer.razor` | 005 | Modificar |
| `Pages\CustomerPages\EditCustomer.razor.cs` | 005 | Modificar |
| `Pages\CustomerPages\AddCustomer.razor` | 006 | Modificar |
| `Pages\CustomerPages\AddCustomer.razor.cs` | 006 | Modificar |
| `ReportPages\Customer Orders\Components\CustomerOrderReportFilter.razor` | 007 | Modificar |
| `ReportPages\Customer Orders\Components\CustomerOrderReportFilter.razor.cs` | 007 | Modificar |
| `ReportPages\Customer Orders\ViewModel\CustomerOrderFilter.cs` | 007 | Modificar |
| `ReportPages\Customer Sales\Components\CustomerSalesReportFilter.razor` | 008 | Modificar |
| `ReportPages\Customer Sales\Components\CustomerSalesReportFilter.razor.cs` | 008 | Modificar |
| `ReportPages\Customer Sales\ViewModel\CustomerSalesFilter.cs` | 008 | Modificar |
| `ReportPages\Customer Reservations\Components\CustomerReservationReportFilter.razor` | 009 | Modificar |
| `ReportPages\Customer Reservations\Components\CustomerReservationReportFilter.razor.cs` | 009 | Modificar |
| `ReportPages\Customer Reservations\ViewModel\CustomerReservationFilter.cs` | 009 | Modificar |

---

### 2.1.2 – Reportes con filtro de Cliente

> Los siguientes 3 reportes tienen un `RadzenDropDownDataGrid` de Cliente que actualmente carga **todos** los clientes.  
> Se debe agregar un toggle/checkbox **"Solo Distribuidores"** que, cuando está activo, filtra el dropdown para mostrar únicamente clientes con `IsDistributor = true`.

---

#### TAREA-007 ? ???
**Agregar filtro "Solo Distribuidores" en `CustomerOrderReportFilter` (Reporte de Órdenes)**

**Contexto del código:**  
`CustomerOrderReportFilter.razor` tiene un `RadzenDropDownDataGrid` de Cliente con `LoadData=@LoadData`. El `CustomerOrderFilter.cs` tiene `int? CustomerId`. El método `LoadData` en el `.cs` llama a `CustomerService.GetAsync(...)` sin filtro de distribuidor.

**Cambios en `CustomerOrderFilter.cs`:**
- Agregar propiedad `bool OnlyDistributors { get; set; } = false`

**Cambios en `CustomerOrderReportFilter.razor`:**
- Agregar `RadzenCheckBox` con label "Solo Distribuidores" **antes** del `RadzenDropDownDataGrid` de Cliente
- Binding: `@bind-Value="@Filter.OnlyDistributors"`
- Al cambiar el checkbox ? limpiar `Filter.CustomerId` y recargar el dropdown

**Cambios en `CustomerOrderReportFilter.razor.cs`:**
- En el método `LoadData`: si `Filter.OnlyDistributors == true` ? llamar `CustomerService.GetAsync(..., onlyDistributors: true)` (requiere TAREA-002 completada)

---

#### TAREA-008 ? ???
**Agregar filtro "Solo Distribuidores" en `CustomerSalesReportFilter` (Reporte de Ventas)**

**Contexto del código:**  
`CustomerSalesReportFilter.razor` tiene exactamente el mismo patrón que `CustomerOrderReportFilter.razor`: `RadzenDropDownDataGrid` de Cliente con `LoadData=@LoadData`. Misma estructura.

**Cambios:**  
Idénticos a TAREA-007 pero aplicados en:
- `CustomerSalesReportFilter.razor`
- `CustomerSalesReportFilter.razor.cs`
- `ViewModel\CustomerSalesFilter.cs` ? agregar `bool OnlyDistributors`

---

#### TAREA-009 ? ???
**Agregar filtro "Solo Distribuidores" en `CustomerReservationReportFilter` (Reporte de Reservas)**

**Contexto del código:**  
`CustomerReservationReportFilter.razor` tiene el mismo patrón. Filtra por Cliente con `RadzenDropDownDataGrid` y `LoadData`.

**Cambios:**  
Idénticos a TAREA-007 pero aplicados en:
- `CustomerReservationReportFilter.razor`
- `CustomerReservationReportFilter.razor.cs`
- `ViewModel\CustomerReservationFilter.cs` ? agregar `bool OnlyDistributors`

---

#### Dependencia común de TAREA-007, 008 y 009

Las tres tareas requieren que `CustomerService.GetAsync` acepte un parámetro opcional `bool onlyDistributors` para filtrar en BD. Esto se implementa como parte de **TAREA-002/003**.

---

#### TAREA-010 ? ???
**Agregar filtro "Solo Distribuidores" en los 4 reportes adicionales con dropdown de Cliente**

**Contexto del código:**  
Los siguientes 4 filtros de reporte tienen también un `RadzenDropDownDataGrid` de Cliente con el mismo patrón que TAREA-007/008/009, pero aún no fueron cubiertos:

| Filtro | Ruta |
|--------|------|
| `BackOrderReportFilter.razor` | `ReportPages\Back Order\` |
| `CustomerOrderActivityReportFilter.razor` | `ReportPages\Customer Order Activities\` |
| `AutomaticAssigmentReportFilter.razor` | `ReportPages\Automatic Customer Order In Process Creation\` |
| `PendingAutomaticCustomerOrderInProcessReportFilter.razor` | `ReportPages\Customer Orders\` |

**Cambios en cada uno:**
- Agregar `bool OnlyDistributors` en el ViewModel/Filter correspondiente
- Agregar `RadzenCheckBox` "Solo Distribuidores" antes del dropdown de Cliente en el `.razor`
- En el `.razor.cs`, pasar `onlyDistributors` a `CustomerService.GetAsync` cuando el checkbox está activo

---

> ? **Con TAREA-001 a TAREA-010 quedan cubiertas todas las modificaciones necesarias relacionadas con Clientes Distribuidores.**  
> Los listados transaccionales (`CustomerOrders.razor`, `CustomerReservations.razor`, `CustomerOrderInProcesses.razor`) **no requieren cambios** para esta funcionalidad: el filtro por distribuidor es responsabilidad de los reportes y de la pantalla de Clientes, no de las pantallas operativas de órdenes.

---

### 2.1.3 Gestión de Períodos de Bonificación

> Ref. propuesta funcional: sección 2.1.3 (HTML v1.0) / sección 2.2.1.1 (MD v1.4)  
> Funcionalidad completamente nueva. No existe ningún artefacto previo en el sistema.  
> Permite definir plantillas de periodicidad (templates) y generar Instancias de Período Activas con fechas específicas.

---

#### TAREA-011 ? ???
**Crear tablas `BonificationPeriods`, `BonificationTypes` y `BonificationPeriodInstances`**

**Descripción:**  
Tres tablas nuevas:
- `BonificationPeriods`: Template reutilizable de periodicidad (ej: "Quincena 15 días")
- `BonificationTypes`: Tipo de bono que **asocia** un Período a una base de cálculo (ej: "Bono Facturación Quincenal")
- `BonificationPeriodInstances`: Ejecución concreta con fechas **ligada a un TipoBono** (no solo al período)

> ?? **DECISIÓN ARQUITECTURAL**: La instancia se crea **por TipoBono**, no por Período template. Un mismo período (ej: "Quincena 15 días") puede tener múltiples instancias activas si se usa en distintos tipos de bono.

**Script SQL a crear:** `scripts/CreateBonificationPeriodsTables.sql`

```sql
-- Template de periodicidad (reutilizable)
CREATE TABLE dbo.BonificationPeriods (
    BONIFICATION_PERIOD_ID   INT           NOT NULL IDENTITY(1,1),
    PERIOD_NAME              VARCHAR(100)  NOT NULL,
    PERIOD_TYPE              VARCHAR(20)   NOT NULL, -- MONTHLY | BIWEEKLY | WEEKLY | DAILY | CUSTOM
    DURATION_DAYS            INT           NOT NULL,
    DESCRIPTION              VARCHAR(250)  NULL,
    IS_ACTIVE                BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_BONIFICATION_PERIOD PRIMARY KEY CLUSTERED (BONIFICATION_PERIOD_ID),
    CONSTRAINT UQ_BONIFICATION_PERIOD_NAME UNIQUE (PERIOD_NAME),
    CONSTRAINT CK_BONIFICATION_PERIOD_DURATION CHECK (DURATION_DAYS > 0),
    CONSTRAINT CK_BONIFICATION_PERIOD_TYPE CHECK (PERIOD_TYPE IN ('MONTHLY','BIWEEKLY','WEEKLY','DAILY','CUSTOM'))
);

-- Tipo de Bono (asocia Período + Base de cálculo)
CREATE TABLE dbo.BonificationTypes (
    BONIFICATION_TYPE_ID     INT           NOT NULL IDENTITY(1,1),
    TYPE_NAME                VARCHAR(100)  NOT NULL,  -- ej: "Bono Facturación Quincenal"
    BONIFICATION_PERIOD_ID   INT           NOT NULL,  -- FK a Período template
    CALCULATION_BASE         VARCHAR(20)   NOT NULL,  -- BILLING | ORDER | DELIVERY
    DESCRIPTION              VARCHAR(250)  NULL,
    IS_ACTIVE                BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_BONIFICATION_TYPE PRIMARY KEY CLUSTERED (BONIFICATION_TYPE_ID),
    CONSTRAINT UQ_BONIFICATION_TYPE_NAME UNIQUE (TYPE_NAME),
    CONSTRAINT FK_BONIFICATION_TYPE_PERIOD FOREIGN KEY (BONIFICATION_PERIOD_ID)
        REFERENCES dbo.BonificationPeriods (BONIFICATION_PERIOD_ID),
    CONSTRAINT CK_BONIFICATION_TYPE_BASE CHECK (CALCULATION_BASE IN ('BILLING','ORDER','DELIVERY'))
);

-- Instancia concreta de un TipoBono con fechas
CREATE TABLE dbo.BonificationPeriodInstances (
    BONIFICATION_PERIOD_INSTANCE_ID  INT          NOT NULL IDENTITY(1,1),
    BONIFICATION_TYPE_ID             INT          NOT NULL,  -- ?? Ahora se asocia a TipoBono, no solo a Período
    INSTANCE_CODE                    VARCHAR(20)  NOT NULL,  -- ej: FAC-QUI-2026-03
    START_DATE                       DATE         NOT NULL,
    END_DATE                         DATE         NOT NULL,  -- calculado: START_DATE + DURATION_DAYS - 1
    STATUS                           VARCHAR(20)  NOT NULL DEFAULT 'OPEN', -- OPEN | IN_PROGRESS | CLOSED
    CONSTRAINT PK_BONIFICATION_PERIOD_INSTANCE PRIMARY KEY CLUSTERED (BONIFICATION_PERIOD_INSTANCE_ID),
    CONSTRAINT UQ_BONIFICATION_PERIOD_INSTANCE_CODE UNIQUE (INSTANCE_CODE),
    CONSTRAINT FK_BONIFICATION_PERIOD_INSTANCE_TYPE FOREIGN KEY (BONIFICATION_TYPE_ID)
        REFERENCES dbo.BonificationTypes (BONIFICATION_TYPE_ID),
    CONSTRAINT CK_BONIFICATION_PERIOD_INSTANCE_STATUS CHECK (STATUS IN ('OPEN','IN_PROGRESS','CLOSED')),
    CONSTRAINT CK_BONIFICATION_PERIOD_INSTANCE_DATES CHECK (END_DATE >= START_DATE)
);

-- Índice para búsquedas frecuentes
CREATE NONCLUSTERED INDEX IX_BONIFICATION_PERIOD_INSTANCES_TYPE_STATUS 
    ON dbo.BonificationPeriodInstances (BONIFICATION_TYPE_ID, STATUS);
```

---

**Ejemplo de uso:**

```
Período: "Quincena 15 días" (Template reutilizable)
    ?
Tipo Bono 1: "Bono Facturación Quincenal" ? usa "Quincena 15 días" + Base BILLING
Tipo Bono 2: "Bono Pedido Quincenal"      ? usa "Quincena 15 días" + Base ORDER
    ?
Instancias creadas al activar Vigencias:
    ?? FAC-QUI-2026-03 (TipoBono=1, 01/03?15/03, IN_PROGRESS)
    ?? PED-QUI-2026-03 (TipoBono=2, 01/03?15/03, IN_PROGRESS)
```

---

#### TAREA-012 ? ??
**Crear entidades EF: `BonificationPeriod`, `BonificationType` y `BonificationPeriodInstance`**

**Archivos a crear:**

- `Aldebaran.DataAccess\Entities\BonificationPeriod.cs`
  ```csharp
  public class BonificationPeriod
  {
      public int BonificationPeriodId { get; set; }
      public string PeriodName { get; set; }
      public string PeriodType { get; set; }
      public int DurationDays { get; set; }
      public string Description { get; set; }
      public bool IsActive { get; set; }

      // Navegación a tipos de bono que usan este período
      public ICollection<BonificationType> BonificationTypes { get; set; } = new List<BonificationType>();
  }
  ```

- `Aldebaran.DataAccess\Entities\BonificationType.cs`
  ```csharp
  public class BonificationType
  {
      public int BonificationTypeId { get; set; }
      public string TypeName { get; set; }
      public int BonificationPeriodId { get; set; }
      public string CalculationBase { get; set; } // BILLING | ORDER | DELIVERY
      public string Description { get; set; }
      public bool IsActive { get; set; }

      // Navegación
      public BonificationPeriod BonificationPeriod { get; set; }
      public ICollection<BonificationPeriodInstance> Instances { get; set; } = new List<BonificationPeriodInstance>();
  }
  ```

- `Aldebaran.DataAccess\Entities\BonificationPeriodInstance.cs`
  ```csharp
  public class BonificationPeriodInstance
  {
      public int BonificationPeriodInstanceId { get; set; }
      public int BonificationTypeId { get; set; } // ?? Cambio: ahora apunta a TipoBono, no a Período
      public string InstanceCode { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public string Status { get; set; }

      // Navegación
      public BonificationType BonificationType { get; set; }
  }
  ```

- `Aldebaran.DataAccess\Configuration\BonificationPeriodConfiguration.cs` — mapeo EF completo
- `Aldebaran.DataAccess\Configuration\BonificationTypeConfiguration.cs` — mapeo EF completo (nuevo)
- `Aldebaran.DataAccess\Configuration\BonificationPeriodInstanceConfiguration.cs` — mapeo EF completo

**Modificar:**
- `Aldebaran.DataAccess\AldebaranDbContext.cs` ? agregar:
  ```csharp
  public DbSet<BonificationPeriod> BonificationPeriods { get; set; }
  public DbSet<BonificationType> BonificationTypes { get; set; }
  public DbSet<BonificationPeriodInstance> BonificationPeriodInstances { get; set; }
  ```

---

#### TAREA-013 ? ??
**Crear modelos de servicio: `BonificationPeriod`, `BonificationType` y `BonificationPeriodInstance`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\BonificationPeriod.cs`
- `Aldebaran.Application.Services\Models\BonificationType.cs` (nuevo)
- `Aldebaran.Application.Services\Models\BonificationPeriodInstance.cs`

Estructura idéntica a las entidades pero sin dependencias de EF (POCO puros).

**Agregar mappings en:**
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs` ? agregar:
  ```csharp
  CreateMap<BonificationPeriod, Entities.BonificationPeriod>().ReverseMap();
  CreateMap<BonificationType, Entities.BonificationType>().ReverseMap();
  CreateMap<BonificationPeriodInstance, Entities.BonificationPeriodInstance>().ReverseMap();
  ```

---

#### TAREA-014 ? ??
**Crear `IBonificationPeriodRepository` y `BonificationPeriodRepository`**

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IBonificationPeriodRepository.cs`
  ```csharp
  public interface IBonificationPeriodRepository
  {
      Task AddAsync(BonificationPeriod period, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationPeriod period, CancellationToken ct = default);
      Task<BonificationPeriod?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationPeriod>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationPeriod>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
      Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
      Task<bool> HasInstancesAsync(int id, CancellationToken ct = default);
      Task AddInstanceAsync(BonificationPeriodInstance instance, CancellationToken ct = default);
      Task<IEnumerable<BonificationPeriodInstance>> GetInstancesAsync(int periodId, CancellationToken ct = default);
  }
  ```

- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationPeriodRepository.cs` — implementación usando `RepositoryBase<AldebaranDbContext>` (mismo patrón que `AreaRepository`)

---

#### TAREA-015 ? ??
**Crear `IBonificationPeriodService` y `BonificationPeriodService`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationPeriodService.cs`
  ```csharp
  public interface IBonificationPeriodService
  {
      Task AddAsync(BonificationPeriod period, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationPeriod period, CancellationToken ct = default);
      Task<BonificationPeriod?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationPeriod>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationPeriod>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
      Task AddInstanceAsync(int periodId, BonificationPeriodInstance instance, CancellationToken ct = default);
      Task<IEnumerable<BonificationPeriodInstance>> GetInstancesAsync(int periodId, CancellationToken ct = default);
  }
  ```

- `Aldebaran.Application.Services\Services\BonificationPeriodService.cs` — implementación usando `IBonificationPeriodRepository` + `IMapper` (mismo patrón que `AreaService`)

**Validaciones de negocio en el servicio:**
- `PeriodName` no puede duplicarse (`ExistsByNameAsync`)
- `DurationDays` debe ser > 0
- No se puede modificar `DurationDays` si el período tiene instancias cerradas (`HasInstancesAsync`)
- No se puede desactivar un período que tiene instancias activas (`STATUS = 'OPEN'` o `'IN_PROGRESS'`)
- `EndDate` de la instancia se calcula automáticamente: `StartDate + DurationDays - 1`
- `InstanceCode` se genera automáticamente (ej: prefijo 3 letras del tipo + año + secuencia)

**Modificar:**
- `Aldebaran.Web\Extensions\ArchitectureBuilderExtensions.cs` ? registrar:
  ```csharp
  services.AddTransient<IBonificationPeriodRepository, BonificationPeriodRepository>();
  services.AddTransient<IBonificationPeriodService, BonificationPeriodService>();
  ```

---

#### TAREA-016 ? ???
**Crear página de listado `BonificationPeriods.razor` + `BonificationPeriods.razor.cs`**

**Ruta:** `Pages\BonificationPages\BonificationPeriods.razor`  
**URL:** `/bonification/periods`  
**Rol requerido:** `Administrador`, `Consulta de bonificaciones`, `Modificación de bonificaciones`

**Estructura (patrón `Customers.razor`):**
- Título: "Períodos de Bonificación"
- Buscador por nombre
- `RadzenDataGrid` paginada con columnas:
  - Nombre
  - Tipo (MONTHLY/BIWEEKLY/etc. ? mostrar en español)
  - Duración (días)
  - Descripción
  - Estado (badge: Activo/Inactivo)
  - Nº Instancias generadas
  - Acciones: Editar | Ver Instancias
- Botón "Nuevo" (solo si tiene rol de modificación)
- Row expand: muestra `RadzenDataGrid` de Instancias del período (Código, Fecha inicio, Fecha fin, Estado)
  - Botón "Generar Instancia" en la sección expandida (abre dialog `AddBonificationPeriodInstance`)

---

#### TAREA-017 ? ???
**Crear dialog `AddBonificationPeriod.razor` + `AddBonificationPeriod.razor.cs`**

**Estructura (patrón `AddCustomer.razor`):**
- Campos:
  - Nombre (texto, obligatorio, único)
  - Tipo de período (`RadzenDropDown`: Mensual / Quincenal / Semanal / Diario / Personalizado)
  - Duración en días (`RadzenNumeric`, obligatorio, > 0) — pre-rellena automático según Tipo seleccionado; editable solo si Tipo = Personalizado
  - Descripción (texto, opcional)
  - Estado (`RadzenCheckBox` Activo, default: marcado)
- Validaciones client-side con `RadzenRequiredValidator`
- Botones: Guardar / Cancelar

---

#### TAREA-018 ? ???
**Crear dialog `EditBonificationPeriod.razor` + `EditBonificationPeriod.razor.cs`**

**Estructura (patrón `EditCustomer.razor`):**
- Mismos campos que TAREA-017
- Campo `DurationDays` ? bloqueado (`Disabled`) si el período tiene instancias cerradas (validación desde servicio, reflejada en UI con mensaje explicativo)
- Al guardar ? llama `BonificationPeriodService.UpdateAsync`

---

#### TAREA-019 ? ??
**Crear servicio de ciclo de vida automático de instancias**

> ?? **ARQUITECTURA AUTOMÁTICA**: Las instancias **NO son creadas manualmente**. Se generan automáticamente cuando se activa una Vigencia (la Vigencia pertenece a un TipoBono, y el TipoBono se parametrizó con un Período que define la duración). El job nocturno las rota según esa periodicidad.

---

**Relación de responsabilidades (quién configura a quién):**

```
BonificationType ???? se parametriza con ???? BonificationPeriod
   (TipoBono)                                     (Período: duración, tipo)
        ?
        ? tiene Vigencias
        ?
  BonificationVigencia
   (rangos de bono %)
        ?
        ? al ACTIVARSE genera
        ?
  BonificationPeriodInstance
   (instancia concreta con fechas)
   StartDate = fecha activación de la Vigencia
   EndDate   = StartDate + TipoBono.Período.DurationDays - 1
```

---

**Ciclo de vida automático:**

```
[Usuario parametriza TipoBono]
  TipoBono "Bono Facturación Quincenal"
  ?? se parametriza con Período "Quincena 15 días" (DurationDays=15)
                    ?
[Usuario activa Vigencia del TipoBono]
  Vigencia "V1-Facturación-Marzo" (ActivationDate=01/03/2026)
  ?? pertenece a TipoBono "Bono Facturación Quincenal"
                    ?
Sistema crea PRIMERA instancia para ese TipoBono:
  InstanceCode: FAC-QUI-2026-03
  BonificationTypeId: 1     ? TipoBono que tiene la vigencia activada
  StartDate:    01/03/2026  ? fecha de activación de la Vigencia
  EndDate:      15/03/2026  ? StartDate + TipoBono.Período.DurationDays(15) - 1
  Status:       IN_PROGRESS
                    ?
Job nocturno (23:59 diario) verifica instancias IN_PROGRESS
                    ?
Cuando hoy == EndDate:
  1. Ejecuta cierre del período (CU10 - FOTO congelada)
  2. Marca Status = CLOSED
  3. Verifica si el TipoBono tiene Vigencia activa
  4. Si sí ? crea SIGUIENTE instancia:
       StartDate = instancia_cerrada.EndDate + 1
       EndDate   = nuevo StartDate + TipoBono.Período.DurationDays - 1
       Status    = IN_PROGRESS
                    ?
Ciclo automático cada 15 días
Detención: cuando el TipoBono queda sin Vigencia activa
```

---

**Archivos a crear:**

**1. Servicio de ciclo de vida:**
- `Application.Services\Services\IBonificationPeriodInstanceLifecycleService.cs`
  ```csharp
  public interface IBonificationPeriodInstanceLifecycleService
  {
      /// <summary>
      /// Crea la primera instancia cuando se activa una Vigencia.
      /// Llamado desde: VigenciaService.ActivateAsync()
      /// El TipoBono ya contiene el Período con la duración necesaria.
      /// </summary>
      Task CreateFirstInstanceAsync(
          int bonificationTypeId,
          DateTime vigencyActivationDate,
          CancellationToken ct = default);

      /// <summary>
      /// Procesa cierre de instancias vencidas y apertura de siguientes.
      /// Llamado desde: Job nocturno (23:59)
      /// </summary>
      Task ProcessDailyRolloverAsync(CancellationToken ct = default);

      /// <summary>
      /// Obtiene la instancia IN_PROGRESS de un TipoBono (null si no hay).
      /// </summary>
      Task<BonificationPeriodInstance?> GetActiveInstanceAsync(
          int bonificationTypeId,
          CancellationToken ct = default);
  }
  ```

- `Application.Services\Services\BonificationPeriodInstanceLifecycleService.cs`
  ```csharp
  public class BonificationPeriodInstanceLifecycleService : IBonificationPeriodInstanceLifecycleService
  {
      private readonly IBonificationTypeRepository _typeRepo;    // TipoBono ? Período ? DurationDays
      private readonly IBonificationPeriodRepository _instanceRepo;
      private readonly IBonificationClosureService _closureService; // CU10 (TAREA futura)
      private readonly ILogger<BonificationPeriodInstanceLifecycleService> _logger;

      public async Task CreateFirstInstanceAsync(
          int bonificationTypeId,
          DateTime vigencyActivationDate,
          CancellationToken ct)
      {
          // TipoBono ya tiene el Período asociado con DurationDays
          var bonType = await _typeRepo.FindWithPeriodAsync(bonificationTypeId, ct);
          var duration = bonType.BonificationPeriod.DurationDays;
          var code = GenerateInstanceCode(
              bonType.BonificationPeriod.PeriodType,
              bonType.CalculationBase,
              vigencyActivationDate);

          var instance = new BonificationPeriodInstance
          {
              BonificationTypeId = bonificationTypeId,
              InstanceCode = code,
              StartDate = vigencyActivationDate,
              EndDate = vigencyActivationDate.AddDays(duration - 1),
              Status = "IN_PROGRESS"
          };

          await _instanceRepo.AddInstanceAsync(instance, ct);
          _logger.LogInformation("Instancia creada: {Code} para TipoBono {TypeId}", code, bonificationTypeId);
      }

      public async Task ProcessDailyRolloverAsync(CancellationToken ct)
      {
          var expiredInstances = await _instanceRepo.GetExpiredActiveInstancesAsync(DateTime.Today, ct);

          foreach (var instance in expiredInstances)
          {
              await _closureService.ClosePeriodAsync(instance.BonificationPeriodInstanceId, ct);
              await _instanceRepo.UpdateInstanceStatusAsync(instance.BonificationPeriodInstanceId, "CLOSED", ct);

              // Continúa solo si el TipoBono tiene Vigencia activa
              var hasActiveVigency = await _typeRepo.HasActiveVigencyAsync(instance.BonificationTypeId, ct);
              if (hasActiveVigency)
              {
                  await CreateFirstInstanceAsync(
                      instance.BonificationTypeId,
                      instance.EndDate.AddDays(1),
                      ct);
              }
          }
      }

      private string GenerateInstanceCode(string periodType, string calculationBase, DateTime startDate)
      {
          var periodPrefix = periodType switch
          {
              "BIWEEKLY" => "QUI",
              "MONTHLY"  => "MES",
              "WEEKLY"   => "SEM",
              "DAILY"    => "DIA",
              _          => "PER"
          };
          var basePrefix = calculationBase switch
          {
              "BILLING"  => "FAC",
              "ORDER"    => "PED",
              "DELIVERY" => "ENT",
              _          => "BON"
          };
          return $"{basePrefix}-{periodPrefix}-{startDate:yyyy}-{startDate:MM}";
      }
  }
  ```

**2. Job programado:**
- `Application.Services\Jobs\BonificationPeriodRolloverJob.cs`
  ```csharp
  public class BonificationPeriodRolloverJob : BackgroundService
  {
      private readonly IServiceProvider _serviceProvider;
      private readonly ILogger<BonificationPeriodRolloverJob> _logger;

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
          while (!stoppingToken.IsCancellationRequested)
          {
              var now = DateTime.Now;
              var scheduledTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 0);
              if (now > scheduledTime)
                  scheduledTime = scheduledTime.AddDays(1);

              await Task.Delay(scheduledTime - now, stoppingToken);

              using var scope = _serviceProvider.CreateScope();
              var lifecycle = scope.ServiceProvider
                  .GetRequiredService<IBonificationPeriodInstanceLifecycleService>();
              try
              {
                  await lifecycle.ProcessDailyRolloverAsync(stoppingToken);
              }
              catch (Exception ex)
              {
                  _logger.LogError(ex, "Error en rollover diario de instancias");
              }
          }
      }
  }
  ```

**Modificar `IBonificationPeriodRepository` (TAREA-014):**
```csharp
Task<IEnumerable<BonificationPeriodInstance>> GetExpiredActiveInstancesAsync(DateTime asOfDate, CancellationToken ct = default);
Task UpdateInstanceStatusAsync(int instanceId, string newStatus, CancellationToken ct = default);
Task<BonificationPeriodInstance?> GetActiveInstanceByTypeAsync(int bonificationTypeId, CancellationToken ct = default);
```

**Nueva interfaz `IBonificationTypeRepository` (TAREA-014):**
```csharp
Task<BonificationType> FindWithPeriodAsync(int bonificationTypeId, CancellationToken ct = default);
Task<bool> HasActiveVigencyAsync(int bonificationTypeId, CancellationToken ct = default);
```

**Punto de disparo en VigenciaService (TAREA futura):**
```csharp
// Al activar una Vigencia ? crear primera instancia para su TipoBono
public async Task ActivateAsync(int vigencyId, CancellationToken ct)
{
    var vigency = await _vigencyRepo.FindAsync(vigencyId, ct);
    // ... lógica de activación y desactivación de vigencia anterior ...

    await _lifecycleService.CreateFirstInstanceAsync(
        vigency.BonificationTypeId,   // TipoBono de la vigencia
        vigency.ActivationDate,       // Fecha de activación
        ct);
}
```

**Modificar `BonificationPeriods.razor` (TAREA-016):**
- Sin botón "Generar Instancia" (las instancias son automáticas)
- Row expand muestra instancias por TipoBono en **solo lectura**
- Columna Estado: ?? IN_PROGRESS | ? CLOSED

**Registrar en DI (`ArchitectureBuilderExtensions.cs`):**
```csharp
services.AddTransient<IBonificationPeriodInstanceLifecycleService, BonificationPeriodInstanceLifecycleService>();
services.AddHostedService<BonificationPeriodRolloverJob>();
```

---

#### ~~TAREA-020~~ **[CANCELADA POR ANÁLISIS]**

~~**Gestión de sesiones de bonificación**~~

> **Estado:** Cancelada en análisis de requisitos.  
> **Motivo de cancelación:** Funcionalidad diferida a Fase 2. Se reemplaza por validación simple en TAREA-003 (restricción de desmarcar `IsDistributor` si hay bonificaciones pendientes). Análisis definió que la gestión granular de sesiones no es crítica para MVP.

---

#### ~~TAREA-021~~ **[CANCELADA POR ANÁLISIS]**

~~**Auditoría detallada de ciclo de vida de instancias**~~

> **Estado:** Cancelada en análisis de requisitos.  
> **Motivo de cancelación:** Se usa Application Insights + Entity Framework change tracking en lugar de tabla específica. Análisis definió que el logging detallado mediante una tabla introduce complejidad innecesaria en BD.

---

#### TAREA-022 ? ??
**Crear `IBonificationTypeRepository` y `BonificationTypeRepository`**

> La entidad `BonificationType` ya fue creada en TAREA-012. Esta tarea implementa su repositorio completo incluyendo los métodos requeridos por el ciclo de vida (TAREA-019).

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IBonificationTypeRepository.cs`
  ```csharp
  public interface IBonificationTypeRepository
  {
      Task AddAsync(BonificationType bonificationType, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationType bonificationType, CancellationToken ct = default);
      Task<BonificationType?> FindAsync(int id, CancellationToken ct = default);
      Task<BonificationType?> FindWithPeriodAsync(int id, CancellationToken ct = default);
      Task<BonificationType?> GetActiveAsync(CancellationToken ct = default);
      Task<(IEnumerable<BonificationType>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationType>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
      Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
      Task<bool> HasActiveInstanceAsync(int id, CancellationToken ct = default);
      Task<bool> HasActiveVigencyAsync(int id, CancellationToken ct = default);
  }
  ```

- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationTypeRepository.cs` — implementación usando `RepositoryBase<AldebaranDbContext>`
  - `FindWithPeriodAsync`: hace `Include(t => t.BonificationPeriod)` para cargar la duración
  - `HasActiveInstanceAsync`: verifica si existe alguna `BonificationPeriodInstance` con `STATUS IN ('OPEN','IN_PROGRESS')` para este TipoBono
  - `HasActiveVigencyAsync`: verifica si existe alguna `BonificationVigency` activa para este TipoBono (usado por el job de rollover)

---

#### TAREA-023 ? ??
**Crear `IBonificationTypeService` y `BonificationTypeService`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationTypeService.cs`
  ```csharp
  public interface IBonificationTypeService
  {
      Task AddAsync(BonificationType bonificationType, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationType bonificationType, CancellationToken ct = default);
      Task<BonificationType?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationType>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationType>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
  }
  ```

- `Aldebaran.Application.Services\Services\BonificationTypeService.cs` — implementación usando `IBonificationTypeRepository` + `IMapper` (mismo patrón que `AreaService`)

**Validaciones de negocio en el servicio:**
- `TypeName` no puede duplicarse (`ExistsByNameAsync`)
- `BonificationPeriodId` debe ser válido
- `CalculationBase` debe ser uno de los valores permitidos
- No se puede modificar un TipoBono si tiene instancias activas (`HasActiveInstanceAsync`)

**Modificar:**
- `Aldebaran.Web\Extensions\ArchitectureBuilderExtensions.cs` ? registrar:
  ```csharp
  services.AddTransient<IBonificationTypeRepository, BonificationTypeRepository>();
  services.AddTransient<IBonificationTypeService, BonificationTypeService>();
  ```

---

#### TAREA-024 ? ???
**Crear página de listado `BonificationTypes.razor` + `BonificationTypes.razor.cs`**

**Ruta:** `Pages\BonificationPages\BonificationTypes.razor`  
**URL:** `/bonification/types`  
**Rol requerido:** `Administrador`, `Consulta de bonificaciones`, `Modificación de bonificaciones`

**Estructura (patrón `Customers.razor`):**
- Título: "Tipos de Bono"
- Buscador por nombre
- `RadzenDataGrid` paginada con columnas:
  - Nombre
  - Tipo (BILLING/ORDER/DELIVERY ? mostrar en español)
  - Estado (badge: Activo/Inactivo)
  - Acciones: Editar
- Botón "Nuevo" (solo si tiene rol de modificación)

---

#### TAREA-025 ? ???
**Crear dialog `AddBonificationType.razor` + `AddBonificationType.razor.cs`**

**Estructura (patrón `AddCustomer.razor`):**
- Campos:
  - Nombre (texto, obligatorio, único)
  - Período (`RadzenDropDown`: lista de períodos activos)
  - Base de cálculo (`RadzenDropDown`: Facturación / Pedido / Entrega)
  - Descripción (texto, opcional)
  - Estado (`RadzenCheckBox` Activo, default: marcado)
- Validaciones client-side con `RadzenRequiredValidator`
- Botones: Guardar / Cancelar

---

#### TAREA-026 ? ???
**Crear dialog `EditBonificationType.razor` + `EditBonificationType.razor.cs`**

**Estructura (patrón `EditCustomer.razor`):**
- Mismos campos que TAREA-025
- Al guardar ? llama `BonificationTypeService.UpdateAsync`

---

#### TAREA-027 ? ??
**Actualizar `BonificationPeriods.razor.cs` y `BonificationTypes.razor.cs` para incluir columnas de Acciones en el DataGrid**

**Cambios:**
- Ambas páginas tendrán una columna "Acciones" con un botón de edición (ícono lápiz) que abre el respectivo diálogo de edición.
- Se elimina el botón "Generar Instancia" en `BonificationPeriods`. Las instancias son automáticas y no deben ser manipuladas desde aquí.

> ?? **Nota de navegación:** Estas páginas se acceden desde el menú **"Bonificaciones"** como ítem de nivel raíz (ver TAREA-045), **no** desde el menú "Administración".

---

#### TAREA-028 ? ??
**Crear vista de cierre de período (CU10)**

> Permite cerrar manualmente un período y generar la próxima instancia si el tipo de bono lo tiene.

**Archivos a crear:**
- `Application.Services\UseCases\CloseBonificationPeriodCommand.cs`
  ```csharp
  public class CloseBonificationPeriodCommand : IRequestHandler<CloseBonificationPeriodRequest, CloseBonificationPeriodResponse>
  {
      private readonly IBonificationPeriodRepository _periodRepo;
      private readonly IBonificationTypeRepository _typeRepo;
      private readonly IBonificationPeriodInstanceLifecycleService _lifecycleService;

      public async Task<CloseBonificationPeriodResponse> Handle(CloseBonificationPeriodRequest request, CancellationToken cancellationToken)
      {
          // 1. Cerrar período actual
          await _periodRepo.UpdateAsync(request.PeriodId, request.NewEndDate, cancellationToken);

          // 2. Si el TipoBono tiene vigencia activa, crear la siguiente instancia
          var typeHasActiveVigency = await _typeRepo.HasActiveVigencyAsync(request.TypeId, cancellationToken);
          if (typeHasActiveVigency)
          {
              var newStartDate = request.NewEndDate.AddDays(1);
              await _lifecycleService.CreateFirstInstanceAsync(request.TypeId, newStartDate, cancellationToken);
          }

          return new CloseBonificationPeriodResponse { Success = true };
      }
  }
  ```

- `Application.Services\UseCases\CloseBonificationPeriod.csproj`
  ```xml
  <ItemGroup>
    <Protobuf Include="Definitions\bonification_periods.proto" GrpcServices="Server" />
  </ItemGroup>
  ```

- `Definitions/bonification_periods.proto`
  ```protobuf
  syntax = "proto3";

  option csharp_namespace = "Aldebaran.Services.Definitions.BonificationPeriods";

  import "google/protobuf/timestamp.proto";

  // Mensajes para el cierre de un período de bonificación
  message CloseBonificationPeriodRequest {
    int32 period_id = 1;
    int32 type_id = 2;
    google.protobuf.Timestamp new_end_date = 3;
  }

  message CloseBonificationPeriodResponse {
    bool success = 1;
  }
  ```

---

> ? **Con TAREA-011 a TAREA-027 quedan cubiertas todas las modificaciones necesarias relacionadas con Períodos y Tipos de Bono.**  
> La gestión de sesiones de bono queda registrada como una nueva historia de usuario independiente (requiere definición y diseño específicos).

---

### 2.1.5 Gestión de Rangos y Porcentajes de Bonificación

> Funcionalidad completamente nueva.  
> Permite crear y gestionar rangos de bonificación con porcentajes asociados a un tipo de bono y período específicos.

---

#### TAREA-029 ? ???
**Crear tabla `BonificationRanges`**

**Descripción:**  
- `BonificationRanges`: Define rangos mínimos y máximos con un porcentaje de bonificación asociado.

**Script SQL a crear:** `scripts/CreateBonificationRangesTable.sql`

```sql
CREATE TABLE dbo.BonificationRanges (
    BONIFICATION_RANGE_ID    INT             NOT NULL IDENTITY(1,1),
    BONIFICATION_TYPE_ID     INT             NOT NULL,    -- FK a Tipo de Bono
    RANGE_MINIMUM            DECIMAL(18,4)   NOT NULL,    -- Mínimo del rango (incluido)
    RANGE_MAXIMUM            DECIMAL(18,4)   NOT NULL,    -- Máximo del rango (incluido)
    BONUS_PERCENTAGE         DECIMAL(5,2)    NOT NULL,    -- Porcentaje de bonificación
    IS_ACTIVE                BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_BONIFICATION_RANGE PRIMARY KEY CLUSTERED (BONIFICATION_RANGE_ID),
    CONSTRAINT UQ_BONIFICATION_RANGE_UNIQUE UNIQUE (BONIFICATION_TYPE_ID, RANGE_MINIMUM, RANGE_MAXIMUM),
    CONSTRAINT FK_BONIFICATION_RANGE_TYPE FOREIGN KEY (BONIFICATION_TYPE_ID)
        REFERENCES dbo.BonificationTypes (BONIFICATION_TYPE_ID)
);
```

---

#### TAREA-030 ? ??
**Crear entidad EF: `BonificationRange`**

**Archivos a crear:**

- `Aldebaran.DataAccess\Entities\BonificationRange.cs`
  ```csharp
  public class BonificationRange
  {
      public int BonificationRangeId { get; set; }
      public int BonificationTypeId { get; set; }
      public decimal RangeMinimum { get; set; }
      public decimal RangeMaximum { get; set; }
      public decimal BonusPercentage { get; set; }
      public bool IsActive { get; set; }

      // Navegación
      public BonificationType BonificationType { get; set; }
  }
  ```

- `Aldebaran.DataAccess\Configuration\BonificationRangeConfiguration.cs` — mapeo EF completo

**Modificar:**
- `Aldebaran.DataAccess\AldebaranDbContext.cs` ? agregar:
  ```csharp
  public DbSet<BonificationRange> BonificationRanges { get; set; }
  ```

---

#### TAREA-031 ? ??
**Crear modelo de servicio: `BonificationRange`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\BonificationRange.cs`

Estructura idéntica a la entidad pero sin dependencias de EF (POCO puros).

**Agregar mappings en:**
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs` ? agregar:
  ```csharp
  CreateMap<BonificationRange, Entities.BonificationRange>().ReverseMap();
  ```

---

#### TAREA-032 ? ??
**Crear `IBonificationRangeRepository` y `BonificationRangeRepository`**

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IBonificationRangeRepository.cs`
  ```csharp
  public interface IBonificationRangeRepository
  {
      Task AddAsync(BonificationRange range, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationRange range, CancellationToken ct = default);
      Task<BonificationRange?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationRange>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationRange>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
  }
  ```

- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationRangeRepository.cs` — implementación usando `RepositoryBase<AldebaranDbContext>` (mismo patrón que `AreaRepository`)

---

#### TAREA-033 ? ??
**Crear `IBonificationRangeService` y `BonificationRangeService`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationRangeService.cs`
  ```csharp
  public interface IBonificationRangeService
  {
      Task AddAsync(BonificationRange range, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationRange range, CancellationToken ct = default);
      Task<BonificationRange?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationRange>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationRange>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
  }
  ```

- `Aldebaran.Application.Services\Services\BonificationRangeService.cs` — implementación usando `IBonificationRangeRepository` + `IMapper` (mismo patrón que `AreaService`)

**Validaciones de negocio en el servicio:**
- `RANGE_MINIMUM` debe ser menor que `RANGE_MAXIMUM`
- `BonusPercentage` debe estar entre 0 y 100
- No se puede modificar un rango si está cerrado (`IsActive = 0`)

**Modificar:**
- `Aldebaran.Web\Extensions\ArchitectureBuilderExtensions.cs` ? registrar:
  ```csharp
  services.AddTransient<IBonificationRangeRepository, BonificationRangeRepository>();
  services.AddTransient<IBonificationRangeService, BonificationRangeService>();
  ```

---

#### TAREA-034 ? ???
**Crear página de listado `BonificationRanges.razor` + `BonificationRanges.razor.cs`**

**Ruta:** `Pages\BonificationPages\BonificationRanges.razor`  
**URL:** `/bonification/ranges`  
**Rol requerido:** `Administrador`, `Consulta de bonificaciones`, `Modificación de bonificaciones`

**Estructura (patrón `Customers.razor`):**
- Título: "Rangos de Bonificación"
- Buscador por nombre
- `RadzenDataGrid` paginada con columnas:
  - Tipo de Bono (nombre)
  - Mínimo
  - Máximo
  - Porcentaje
  - Estado (badge: Activo/Inactivo)
  - Acciones: Editar
- Botón "Nuevo" (solo si tiene rol de modificación)

---

#### TAREA-035 ? ???
**Crear dialog `EditBonificationVigency.razor` + `EditBonificationVigency.razor.cs`**

**Estructura:**
- Mismos campos que TAREA-034
- **Si estado = `PENDING`** ? cabecera y rangos son editables (la fecha de activación aún no llegó)
- **Si estado = `ACTIVE` o `INACTIVE`** ? todo en modo solo lectura con mensaje explicativo: "Esta vigencia está en curso o fue reemplazada. No se pueden modificar sus rangos."
- Botones: Guardar (solo visible si PENDING) / Cerrar

---

> ? **Con TAREA-029 a TAREA-035 quedan cubiertas todas las modificaciones necesarias relacionadas con Rangos de Bonificación.**

#### TAREA-036 
**Agregar botón "Ver Vigencias" en `BonificationTypes.razor`**

**Contexto del código:**  
`BonificationTypes.razor` (TAREA-024) tiene una columna "Acciones" con botón Editar. Sin un vínculo explícito al listado de Vigencias, el usuario debe escribir la URL directamente para acceder a `/bonification/types/{id}/vigencies`.

**Cambios en `BonificationTypes.razor`:**
- Agregar botón "Ver Vigencias" en la columna Acciones del `RadzenDataGrid`
  - Ícono: `schedule` o `event_note`
  - Tooltip: "Ver Vigencias de este Tipo de Bono"
  - Navega a `/bonification/types/{BonificationTypeId}/vigencies`
  - Visible para todos los roles con acceso a la pantalla (`Administrador`, `Consulta de bonificaciones`, `Modificación de bonificaciones`)

**Cambios en `BonificationTypes.razor.cs`:**
- No requiere lógica adicional: la navegación es directa con `NavigationManager.NavigateTo`.

---

### 2.1.6 Vigencias de Descuentos por Total de Pedido

> Ref. propuesta funcional: seccion 2.2.0.3 (MD v1.4)  
> Funcionalidad completamente nueva. **Independiente de las Vigencias de Bonificación.**  
>  
> Permite configurar vigencias de descuento que se aplican de forma **uniforme a todos los distribuidores** en el cálculo del **Bono por Pedido**.  
> Solo puede existir **UNA vigencia ACTIVE** en el sistema en un momento dado (no es por TipoBono, es global).  
>  
> Cada vigencia tiene rangos de totales de pedido. Cuando el acumulado del distribuidor cae en un rango, se aplica el descuento correspondiente (porcentual o fijo) **antes** de calcular el bono.

---

**Modelo conceptual:**

```
Vigencia Descuento "V2 - Desc. Pedido Junio 2026"  (ACTIVE)
??? ActivationDate: 01/06/2026
??? Rangos:
?     ??? Desde $1,000,001  hasta $5,000,000   ? Fijo    $100,000
?     ??? Desde $5,000,001  hasta $10,000,000  ? %       2%
?     ??? Desde $10,000,001 hasta ?            ? %       5%
??? Estado: ACTIVE

Vigencia Descuento "V3 - Desc. Pedido Agosto 2026"  (PENDING)
??? ActivationDate: 01/08/2026
??? Estado: PENDING
```

> **Diferencia clave con Vigencias de Bonificación:**  
> Las Vigencias de Bono son **por TipoBono** (una activa por cada tipo).  
> Las Vigencias de Descuento son **globales** — solo existe una activa en todo el sistema.  
> Los rangos tampoco necesitan iniciar en $0. Si el total del pedido es inferior al primer rango, el descuento es $0.

---

#### TAREA-037 ? ???
**Crear tablas `DiscountVigencies` y `DiscountVigencyRanges`**

**Script SQL a crear:** `scripts/CreateDiscountVigencyTables.sql`

```sql
-- Vigencia de descuento por total de pedido (global, una sola activa)
CREATE TABLE dbo.DiscountVigencies (
    DISCOUNT_VIGENCY_ID   INT           NOT NULL IDENTITY(1,1),
    VIGENCY_NAME          VARCHAR(100)  NOT NULL,
    ACTIVATION_DATE       DATE          NOT NULL,
    DEACTIVATION_DATE     DATE          NULL,
    STATUS                VARCHAR(20)   NOT NULL DEFAULT 'PENDING', -- PENDING | ACTIVE | INACTIVE
    NOTES                 VARCHAR(500)  NULL,
    CONSTRAINT PK_DISCOUNT_VIGENCY PRIMARY KEY CLUSTERED (DISCOUNT_VIGENCY_ID),
    CONSTRAINT UQ_DISCOUNT_VIGENCY_NAME UNIQUE (VIGENCY_NAME),
    CONSTRAINT CK_DISCOUNT_VIGENCY_STATUS CHECK (STATUS IN ('PENDING','ACTIVE','INACTIVE')),
    CONSTRAINT CK_DISCOUNT_VIGENCY_DATES CHECK (
        DEACTIVATION_DATE IS NULL OR DEACTIVATION_DATE >= ACTIVATION_DATE
    )
);

-- Índice para obtener la vigencia activa rápidamente
CREATE NONCLUSTERED INDEX IX_DISCOUNT_VIGENCY_STATUS
    ON dbo.DiscountVigencies (STATUS);

-- Rangos de descuento por total de pedido
CREATE TABLE dbo.DiscountVigencyRanges (
    DISCOUNT_VIGENCY_RANGE_ID  INT             NOT NULL IDENTITY(1,1),
    DISCOUNT_VIGENCY_ID        INT             NOT NULL,
    RANGE_ORDER                INT             NOT NULL,
    FROM_AMOUNT                DECIMAL(18,2)   NOT NULL,   -- total mínimo del tramo (inclusive)
    TO_AMOUNT                  DECIMAL(18,2)   NULL,       -- total máximo del tramo (NULL = sin techo)
    VALUE_TYPE                 VARCHAR(10)     NOT NULL,   -- PERCENTAGE | FIXED
    DISCOUNT_VALUE             DECIMAL(18,2)   NOT NULL,   -- si PERCENTAGE: 0-100 / si FIXED: monto exacto
    CONSTRAINT PK_DISCOUNT_VIGENCY_RANGE PRIMARY KEY CLUSTERED (DISCOUNT_VIGENCY_RANGE_ID),
    CONSTRAINT FK_DISCOUNT_VIGENCY_RANGE_VIGENCY FOREIGN KEY (DISCOUNT_VIGENCY_ID)
        REFERENCES dbo.DiscountVigencies (DISCOUNT_VIGENCY_ID),
    CONSTRAINT CK_DISCOUNT_VIGENCY_RANGE_FROM CHECK (FROM_AMOUNT >= 0),
    CONSTRAINT CK_DISCOUNT_VIGENCY_RANGE_VALUE_TYPE CHECK (VALUE_TYPE IN ('PERCENTAGE','FIXED')),
    CONSTRAINT CK_DISCOUNT_VIGENCY_RANGE_VALUE CHECK (DISCOUNT_VALUE >= 0),
    CONSTRAINT CK_DISCOUNT_VIGENCY_RANGE_TO CHECK (TO_AMOUNT IS NULL OR TO_AMOUNT > FROM_AMOUNT),
    CONSTRAINT UQ_DISCOUNT_VIGENCY_RANGE_ORDER UNIQUE (DISCOUNT_VIGENCY_ID, RANGE_ORDER)
);
```

---

#### TAREA-038 ? ??
**Crear entidades EF: `DiscountVigency` y `DiscountVigencyRange`**

**Archivos a crear:**

- `Aldebaran.DataAccess\Entities\DiscountVigency.cs`
  ```csharp
  public class DiscountVigency
  {
      public int DiscountVigencyId { get; set; }
      public string VigencyName { get; set; }
      public DateTime ActivationDate { get; set; }
      public DateTime? DeactivationDate { get; set; }
      public string Status { get; set; }  // PENDING | ACTIVE | INACTIVE
      public string Notes { get; set; }

      public ICollection<DiscountVigencyRange> Ranges { get; set; } = new List<DiscountVigencyRange>();
  }
  ```

- `Aldebaran.DataAccess\Entities\DiscountVigencyRange.cs`
  ```csharp
  public class DiscountVigencyRange
  {
      public int DiscountVigencyRangeId { get; set; }
      public int DiscountVigencyId { get; set; }
      public int RangeOrder { get; set; }
      public decimal FromAmount { get; set; }
      public decimal? ToAmount { get; set; }       // null = sin techo
      public string ValueType { get; set; }        // PERCENTAGE | FIXED
      public decimal DiscountValue { get; set; }   // si PERCENTAGE: 0-100 / si FIXED: monto exacto

      public DiscountVigency DiscountVigency { get; set; }
  }
  ```

- `Aldebaran.DataAccess\Configuration\DiscountVigencyConfiguration.cs` — mapeo EF completo
- `Aldebaran.DataAccess\Configuration\DiscountVigencyRangeConfiguration.cs` — mapeo EF completo

**Modificar:**
- `Aldebaran.DataAccess\AldebaranDbContext.cs` ? agregar:
  ```csharp
  public DbSet<DiscountVigency> DiscountVigencies { get; set; }
  public DbSet<DiscountVigencyRange> DiscountVigencyRanges { get; set; }
  ```

---

#### TAREA-039 ? ??
**Crear modelos de servicio: `DiscountVigency` y `DiscountVigencyRange`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\DiscountVigency.cs`
- `Aldebaran.Application.Services\Models\DiscountVigencyRange.cs`

Estructura idéntica a las entidades (POCO puros).

**Agregar mappings en `ApplicationServicesProfile.cs`:**
```csharp
CreateMap<DiscountVigency, Entities.DiscountVigency>().ReverseMap();
CreateMap<DiscountVigencyRange, Entities.DiscountVigencyRange>().ReverseMap();
```

---

#### TAREA-040 ? ??
**Crear `IDiscountVigencyRepository` y `DiscountVigencyRepository`**

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IDiscountVigencyRepository.cs`
  ```csharp
  public interface IDiscountVigencyRepository
  {
      Task AddAsync(DiscountVigency vigency, CancellationToken ct = default);
      Task UpdateAsync(int id, DiscountVigency vigency, CancellationToken ct = default);
      Task<DiscountVigency?> FindAsync(int id, CancellationToken ct = default);
      Task<DiscountVigency?> FindWithRangesAsync(int id, CancellationToken ct = default);
      Task<DiscountVigency?> GetActiveAsync(CancellationToken ct = default);
      Task<(IEnumerable<DiscountVigency>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
      Task<bool> HasActiveVigencyAsync(CancellationToken ct = default);
      Task UpdateStatusAsync(int id, string newStatus, DateTime? deactivationDate, CancellationToken ct = default);
  }
  ```

- `Aldebaran.DataAccess.Infraestructure\Repository\DiscountVigencyRepository.cs` — implementación usando `RepositoryBase<AldebaranDbContext>`

---

#### TAREA-041 ? ??
**Crear `IDiscountVigencyService` y `DiscountVigencyService`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IDiscountVigencyService.cs`
  ```csharp
  public interface IDiscountVigencyService
  {
      Task AddAsync(DiscountVigency vigency, CancellationToken ct = default);
      Task UpdateAsync(int id, DiscountVigency vigency, CancellationToken ct = default);
      Task ActivateAsync(int vigencyId, CancellationToken ct = default);
      Task<DiscountVigency?> FindAsync(int id, CancellationToken ct = default);
      Task<DiscountVigency?> GetActiveAsync(CancellationToken ct = default);
      Task<(IEnumerable<DiscountVigency>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
  }
  ```

- `Aldebaran.Application.Services\Services\DiscountVigencyService.cs`

**Validaciones de negocio:**
- Una vigencia nueva siempre nace en estado `PENDING`
- `ActivationDate` no puede ser anterior a la fecha actual
- **Estado `PENDING`**: `ActivationDate` ? hoy ? rangos **editables**
- **Estado `ACTIVE` / `INACTIVE`**: rangos **solo lectura**
- Los rangos son entidad independiente (`DiscountVigencyRange`) — no arreglo embebido
- Los rangos **no necesitan iniciar en $0**: si el total del pedido es inferior al primer `FromAmount`, descuento = $0
- Los rangos dentro de la misma vigencia **no pueden solaparse**: `FromAmount` de cada rango > `ToAmount` del anterior
- El rango superior debe tener `ToAmount = null` (sin techo)
- `ValueType` debe ser `PERCENTAGE` o `FIXED`:
  - Si `PERCENTAGE` ? `DiscountValue` entre 0 y 100
  - Si `FIXED` ? `DiscountValue` ? 0
- **Solo puede existir UNA vigencia `ACTIVE` en todo el sistema** (al activar una nueva ? la anterior pasa a `INACTIVE`)
- No se puede eliminar una vigencia `ACTIVE`

**Lógica de `ActivateAsync`:**
```csharp
public async Task ActivateAsync(int vigencyId, CancellationToken ct)
{
    var vigency = await _repo.FindWithRangesAsync(vigencyId, ct);

    if (!vigency.Ranges.Any())
        throw new BusinessException("La vigencia debe tener al menos un rango antes de activarse");

    // Desactivar la vigencia global actual (si existe)
    var currentActive = await _repo.GetActiveAsync(ct);
    if (currentActive != null)
        await _repo.UpdateStatusAsync(currentActive.DiscountVigencyId, "INACTIVE", vigency.ActivationDate.AddDays(-1), ct);

    // Activar la nueva
    await _repo.UpdateStatusAsync(vigencyId, "ACTIVE", null, ct);
}
```

> **Nota:** `ActivateAsync` de descuentos **NO dispara creación de instancias de período** — eso es exclusivo de las vigencias de bonificación.

**Modificar:**
- `Aldebaran.Web\Extensions\ArchitectureBuilderExtensions.cs` ? agregar:
  ```csharp
  services.AddTransient<IDiscountVigencyRepository, DiscountVigencyRepository>();
  services.AddTransient<IDiscountVigencyService, DiscountVigencyService>();
  ```

---

#### TAREA-042 ? ???
**Crear página de listado `DiscountVigencies.razor` + `.razor.cs`**

**Ruta:** `Pages\BonificationPages\DiscountVigencies.razor`  
**URL:** `/bonification/discount-vigencies`  
**Rol requerido:** `Administrador`, `Consulta de bonificaciones`, `Modificación de bonificaciones`

**Estructura:**
- Título: "Vigencias de Descuento por Pedido"
- Indicador visual destacado: **"Vigencia Activa: {VigencyName}"** (o "Sin vigencia activa" si no hay ninguna)
- `RadzenDataGrid` con columnas:

| Columna | Detalle |
|---------|---------|
| Nombre | `VigencyName` |
| Fecha Activación | `ActivationDate` |
| Fecha Desactivación | `DeactivationDate` (o "Vigente" si null) |
| Estado | Badge: ?? PENDING / ?? ACTIVE / ? INACTIVE |
| Nº Rangos | Count de rangos configurados |
| Acciones | Editar |

- Botón "Nueva Vigencia" (solo rol modificación)
- Botón "Activar" ? visible solo si estado = `PENDING` y tiene rangos
- Row expand: tabla de rangos (Orden, Desde, Hasta, Tipo, Valor) — solo lectura si estado ? `PENDING`

---

#### TAREA-043 ? ???
**Crear dialog `AddDiscountVigency.razor` + `AddDiscountVigency.razor.cs`**

**Estructura (idéntica a `AddBonificationVigency` — TAREA-034, sin campo TipoBono):**
- Campos de cabecera:
  - Nombre de Vigencia (texto, obligatorio, único)
  - Fecha de Activación (`RadzenDatePicker`, obligatorio, no puede ser anterior a hoy)
  - Notas (texto, opcional)
- Sección "Rangos de Descuento" — grilla editable inline:
  - Columnas: Orden | Desde ($) | Hasta ($) | Tipo | Valor | Acciones (eliminar fila)
    - **Tipo**: `RadzenDropDown` ? Porcentaje (%) / Valor Fijo ($)
    - **Valor**: `RadzenNumeric` — si Porcentaje: máximo 100 / si Fijo: ? 0
  - Botón "+ Agregar rango"
  - El último rango muestra "Sin límite" en Hasta
  - Validación en tiempo real: sin solapamiento dentro de la misma vigencia
- Botones: Guardar como PENDING / Cancelar

---

#### TAREA-044 ? ???
**Crear dialog `EditDiscountVigency.razor` + `EditDiscountVigency.razor.cs`**

**Estructura:**
- Mismos campos que TAREA-043
- **Si estado = `PENDING`** ? cabecera y rangos editables
- **Si estado = `ACTIVE` o `INACTIVE`** ? todo en modo solo lectura con mensaje: "Esta vigencia está en curso o fue reemplazada. No se pueden modificar sus rangos."
- Botones: Guardar (solo visible si `PENDING`) / Cerrar

---

#### TAREA-045 ? ???
**Crear menú "Bonificaciones" como ítem raíz independiente en `MainLayout.razor`**

> ?? **DECISIÓN ARQUITECTURAL DE NAVEGACIÓN** (documentada — no implementada hasta aprobación del proyecto)

**Decisión:** El módulo de Bonificaciones se ubica como **ítem de nivel raíz** en el menú lateral, al mismo nivel que "Administración", "Movimientos de Inventario" y "Reportes". **No va dentro de Administración.**

**Justificación:**
- Bonificación es un **dominio de negocio independiente**, no configuración del sistema
- El módulo tiene 6+ subitems y seguirá creciendo (OC Especiales, Reportes de Bonificación, etc.)
- Audiencia diferente: el personal de bonificación no necesariamente tiene rol de Administración
- Patrón consistente con los otros módulos de primer nivel del sistema

**Estructura del menú propuesta:**

```
??? Tablero de notificaciones
??? Administración                    ? sin cambios
??? Movimientos de Inventario         ? sin cambios
??? Bonificaciones                    ? NUEVO ítem de nivel raíz
?   ??? Configuración para Bonificaciones
?   ?   ??? Períodos                  ? bonification/periods      (TAREA-016)
?   ?   ??? Tipos de Bono             ? bonification/types        (TAREA-024)
?   ?   ??? Vigencias                 ? bonification/vigencies    (TAREA-033)
?   ?   ??? Descuentos por Pedido     ? bonification/discount-vigencies (TAREA-042)
?   ??? Operaciones
?       ??? OC Especiales             ? bonification/special-orders (TAREA futura)
??? Reportes                          ? sin cambios
```

**Roles por subítem:**
- `Configuración.*` ? `Administrador`, `Consulta de bonificaciones`, `Modificación de bonificaciones`
- `Operaciones > OC Especiales` ? `Administrador`, `Ingreso de OC especiales de bonificación`
- Padre `Bonificaciones` ? visible si tiene **cualquiera** de los roles anteriores

**Archivo a modificar (cuando se apruebe el desarrollo):**
- `Web\Shared\MainLayout.razor`

**Bloque Razor a agregar (entre `</RadzenPanelMenuItem>` de Movimientos de Inventario y `<RadzenPanelMenuItem Text="Reportes"`):**

```razor
<RadzenPanelMenuItem Text="Bonificaciones" Icon="percent"
    Visible="@Security.IsInRole("Administrador","Consulta de bonificaciones","Modificación de bonificaciones","Ingreso de OC especiales de bonificación")">
    <RadzenPanelMenuItem Text="Configuración para Bonificaciones" Icon="settings">
        <RadzenPanelMenuItem Text="Períodos"
            Path="bonification/periods"
            Visible="@Security.IsInRole("Administrador","Consulta de bonificaciones","Modificación de bonificaciones")" />
        <RadzenPanelMenuItem Text="Tipos de Bono"
            Path="bonification/types"
            Visible="@Security.IsInRole("Administrador","Consulta de bonificaciones","Modificación de bonificaciones")" />
        <RadzenPanelMenuItem Text="Vigencias"
            Path="bonification/vigencies"
            Visible="@Security.IsInRole("Administrador","Consulta de bonificaciones","Modificación de bonificaciones")" />
        <RadzenPanelMenuItem Text="Descuentos por Pedido"
            Path="bonification/discount-vigencies"
            Visible="@Security.IsInRole("Administrador","Consulta de bonificaciones","Modificación de bonificaciones")" />
    </RadzenPanelMenuItem>
    <RadzenPanelMenuItem Text="Operaciones" Icon="edit_note">
        <RadzenPanelMenuItem Text="OC Especiales"
            Path="bonification/special-orders"
            Visible="@Security.IsInRole("Administrador","Ingreso de OC especiales de bonificación")" />
    </RadzenPanelMenuItem>
</RadzenPanelMenuItem>

```

---

## 2.2 Modulo de Operaciones de Bonificacion

> Accesible desde el menu **Bonificaciones > Operaciones**.  
> Contiene las operaciones manuales que el equipo de PROMOS ejecuta durante el ciclo de bonificacion.

---

### 2.2.1 Gestión de Ordenes de Compra Especiales (OC Especiales)

> Ref. propuesta funcional: sección 2.1.3 (modalidad Bonificación por Facturación).  
> Permite registrar montos de facturación especiales para un distribuidor en un periodo activo,  
> que deben sumarse a la base de cálculo del **Bono por Facturación** (fuente TOTUS).  
>  
> Casos de uso típicos: descuentos retroactivos, ajustes de NC, operaciones fuera de TOTUS que PROMOS  
> decide reconocer como base de bonificación.  
>  
> **Dos caminos de ingreso:**
> 1. **Manual** — Ingreso OC por OC desde la interfaz (TAREA-051 a TAREA-054)
> 2. **Masivo** — Carga de archivo Excel/CSV con múltiples OC en un solo paso (TAREA-055 a TAREA-058)
>
> En ambos casos las OC nacen en estado `PENDIENTE` y requieren aprobación explícita antes de impactar el cálculo.  
>  
> **Solo un usuario con rol `Ingreso de OC especiales de bonificación`** puede registrar y gestionar estas OC.

---

#### TAREA-046 ? ???
**Crear tabla `BonificationSpecialOrders`**

**Script SQL a crear:** `scripts/CreateBonificationSpecialOrdersTable.sql`

```sql
CREATE TABLE dbo.BonificationSpecialOrders (
    SPECIAL_ORDER_ID                 INT             NOT NULL IDENTITY(1,1),
    CUSTOMER_ID                      INT             NOT NULL,   -- FK a Customers (solo distribuidores)
    BONIFICATION_PERIOD_INSTANCE_ID  INT             NOT NULL,   -- FK a instancia activa (TipoBono BILLING)
    AMOUNT                           DECIMAL(18,2)   NOT NULL,   -- monto a sumar en base de calculo
    DESCRIPTION                      VARCHAR(500)    NOT NULL,   -- motivo obligatorio
    STATUS                           VARCHAR(20)     NOT NULL DEFAULT 'PENDIENTE',
    CREATED_BY                       INT             NOT NULL,   -- FK a ApplicationUser
    CREATED_AT                       DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    REVIEWED_BY                      INT             NULL,       -- FK a ApplicationUser (quien aprobo/rechazo)
    REVIEWED_AT                      DATETIME        NULL,
    REJECTION_REASON                 VARCHAR(500)    NULL,       -- obligatorio si STATUS = RECHAZADA
    CONSTRAINT PK_BONIFICATION_SPECIAL_ORDER PRIMARY KEY CLUSTERED (SPECIAL_ORDER_ID),
    CONSTRAINT FK_SPECIAL_ORDER_CUSTOMER FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID),
    CONSTRAINT FK_SPECIAL_ORDER_PERIOD_INSTANCE FOREIGN KEY (BONIFICATION_PERIOD_INSTANCE_ID)
        REFERENCES dbo.BonificationPeriodInstances (BONIFICATION_PERIOD_INSTANCE_ID),
    CONSTRAINT CK_SPECIAL_ORDER_STATUS CHECK (STATUS IN ('PENDIENTE','APROBADA','RECHAZADA')),
    CONSTRAINT CK_SPECIAL_ORDER_AMOUNT CHECK (AMOUNT >= 0)
);

-- Indices para busquedas frecuentes
CREATE NONCLUSTERED INDEX IX_SPECIAL_ORDER_CUSTOMER_INSTANCE
    ON dbo.BonificationSpecialOrders (CUSTOMER_ID, BONIFICATION_PERIOD_INSTANCE_ID, STATUS);

CREATE NONCLUSTERED INDEX IX_SPECIAL_ORDER_STATUS
    ON dbo.BonificationSpecialOrders (STATUS);
```

---

#### TAREA-047 ? ??
**Crear entidad EF: `BonificationSpecialOrder`**

**Archivo a crear:**
- `Aldebaran.DataAccess\Entities\BonificationSpecialOrder.cs`
  ```csharp
  public class BonificationSpecialOrder
  {
      public int SpecialOrderId { get; set; }
      public int CustomerId { get; set; }
      public int BonificationPeriodInstanceId { get; set; }
      public decimal Amount { get; set; }
      public string Description { get; set; }
      public string Status { get; set; }  // PENDIENTE | APROBADA | RECHAZADA
      public int CreatedBy { get; set; }
      public DateTime CreatedAt { get; set; }
      public int? ReviewedBy { get; set; }
      public DateTime? ReviewedAt { get; set; }
      public string RejectionReason { get; set; }

      // Navegacion
      public Customer Customer { get; set; }
      public BonificationPeriodInstance PeriodInstance { get; set; }
  }
  ```

- `Aldebaran.DataAccess\Configuration\BonificationSpecialOrderConfiguration.cs` — mapeo EF completo

**Modificar:**
- `Aldebaran.DataAccess\AldebaranDbContext.cs` ? agregar:
  ```csharp
  public DbSet<BonificationSpecialOrder> BonificationSpecialOrders { get; set; }
  ```

---

#### TAREA-048 ? ??
**Crear modelo de servicio: `BonificationSpecialOrder`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\BonificationSpecialOrder.cs` — POCO puro

**Agregar mappings en `ApplicationServicesProfile.cs`:**
```csharp
CreateMap<BonificationSpecialOrder, Entities.BonificationSpecialOrder>().ReverseMap();
```

---

#### TAREA-049 ? ??
**Crear `IBonificationSpecialOrderRepository` y `BonificationSpecialOrderRepository`**

**Archivo a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IBonificationSpecialOrderRepository.cs`
  ```csharp
  public interface IBonificationSpecialOrderRepository
  {
      Task AddAsync(BonificationSpecialOrder order, CancellationToken ct = default);
      Task UpdateAsync(int id, BonificationSpecialOrder order, CancellationToken ct = default);
      Task<BonificationSpecialOrder?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationSpecialOrder>, int)> GetAsync(
          int? skip, int? top,
          int? customerId, string? status,
          CancellationToken ct = default);
      Task<decimal> GetApprovedTotalAsync(
          int customerId,
          int periodInstanceId,
          CancellationToken ct = default);
      Task UpdateStatusAsync(
          int id, string newStatus,
          int reviewedBy, DateTime reviewedAt,
          string? rejectionReason,
          CancellationToken ct = default);
  }
  ```

- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationSpecialOrderRepository.cs` — implementacion usando `RepositoryBase<AldebaranDbContext>`
  - `GetApprovedTotalAsync`: `SUM(AMOUNT) WHERE STATUS = 'APROBADA' AND CUSTOMER_ID = x AND PERIOD_INSTANCE_ID = y`

---

#### TAREA-050 ? ??
**Crear `IBonificationSpecialOrderService` y `BonificationSpecialOrderService`**

**Archivo a crear:**
- `Aldebaran.Application.Services\Services\IBonificationSpecialOrderService.cs`
  ```csharp
  public interface IBonificationSpecialOrderService
  {
      Task AddAsync(BonificationSpecialOrder order, CancellationToken ct = default);
      Task ApproveAsync(int id, int reviewedBy, CancellationToken ct = default);
      Task RejectAsync(int id, int reviewedBy, string rejectionReason, CancellationToken ct = default);
      Task<BonificationSpecialOrder?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<BonificationSpecialOrder>, int)> GetAsync(
          int? skip, int? top,
          int? customerId, string? status,
          CancellationToken ct = default);
      Task<decimal> GetApprovedTotalAsync(int customerId, int periodInstanceId, CancellationToken ct = default);
  }
  ```

- `Aldebaran.Application.Services\Services\BonificationSpecialOrderService.cs`

**Validaciones de negocio:**
- `CustomerId` debe existir y tener `IsDistributor = true`
- `BonificationPeriodInstanceId` debe existir y tener `STATUS = 'IN_PROGRESS'` con `CalculationBase = 'BILLING'`
- `Amount` debe ser `>= 0`
- `Description` obligatoria (minimo 10 caracteres)
- Una OC solo puede aprobarse o rechazarse si esta en estado `PENDIENTE`
- `RejectionReason` es obligatoria si el estado cambia a `RECHAZADA` (minimo 10 caracteres)
- Solo el rol `Ingreso de OC especiales de bonificación` puede crear OC (validacion de rol en el controlador / pagina)
- No se puede modificar una OC en estado `APROBADA` o `RECHAZADA`

**Modificar:**
- `Aldebaran.Web\Extensions\ArchitectureBuilderExtensions.cs` ? agregar:
  ```csharp
  services.AddTransient<IBonificationSpecialOrderRepository, BonificationSpecialOrderRepository>();
  services.AddTransient<IBonificationSpecialOrderService, BonificationSpecialOrderService>();
  ```

---

#### TAREA-051 ? ???
**Crear pagina de listado `BonificationSpecialOrders.razor` + `.razor.cs`**

**Ruta:** `Pages\BonificationPages\BonificationSpecialOrders.razor`  
**URL:** `/bonification/special-orders`  
**Rol requerido:** `Administrador`, `Ingreso de OC especiales de bonificación`

**Estructura:**
- Titulo: "OC Especiales de Bonificacion"
- Filtros superiores:
  - Dropdown **Distribuidor** (`RadzenDropDownDataGrid` — solo clientes con `IsDistributor = true`)
  - Dropdown **Estado** (Todos / Pendiente / Aprobada / Rechazada)
  - Boton "Buscar"
- `RadzenDataGrid` con columnas:

| Columna | Detalle |
|---------|---------|
| # | `SpecialOrderId` |
| Distribuidor | `Customer.CustomerName` |
| Periodo | `PeriodInstance.InstanceCode` + rango fechas |
| Monto | `Amount` formateado como moneda |
| Descripcion | `Description` (truncada a 60 chars con tooltip completo) |
| Estado | Badge: ?? PENDIENTE / ?? APROBADA / ?? RECHAZADA |
| Creada por | nombre del usuario + fecha |
| Revisada por | nombre del usuario + fecha (vacio si PENDIENTE) |
| Acciones | Ver detalle / Aprobar / Rechazar |

- Boton "Nueva OC Especial" (solo rol `Ingreso de OC especiales de bonificación`)
- Botones **Aprobar** y **Rechazar** visibles solo si estado = `PENDIENTE`
- Row expand: muestra `RejectionReason` si estado = `RECHAZADA`

---

#### TAREA-052 ? ???
**Crear dialog `AddBonificationSpecialOrder.razor` + `.razor.cs`**

**Estructura:**
- Campos:
  - **Distribuidor** (`RadzenDropDownDataGrid` — solo `IsDistributor = true`, con buscador)
  - **Periodo Activo** (`RadzenDropDown` — carga instancias `IN_PROGRESS` con `CalculationBase = BILLING` del distribuidor seleccionado; se recarga al cambiar distribuidor)
  - **Monto** (`RadzenNumeric`, obligatorio, `>= 0`, formato moneda)
  - **Descripcion / Motivo** (`RadzenTextArea`, obligatorio, minimo 10 caracteres, maximo 500)
- Validaciones client-side con `RadzenRequiredValidator`
- Nota informativa: "Esta OC quedara en estado PENDIENTE hasta ser aprobada o rechazada."
- Botones: Guardar / Cancelar

---

#### TAREA-053 ? ???
**Crear dialog `ApproveBonificationSpecialOrder.razor` + `ApproveBonificationSpecialOrder.razor.cs`**

**Estructura (modal de confirmacion simple):**
- Muestra resumen de la OC:
  - Distribuidor, Periodo, Monto, Descripción
- Mensaje: "Al aprobar esta OC, el monto **$[Amount]** sera incluido en la base de calculo del Bono por Facturacion del distribuidor para el periodo **[InstanceCode]**."
- Botones: Aprobar / Cancelar
- Al confirmar ? llama `BonificationSpecialOrderService.ApproveAsync`

---

#### TAREA-054 ? ???
**Crear dialog `RejectBonificationSpecialOrder.razor` + `RejectBonificationSpecialOrder.razor.cs`**

**Estructura:**
- Muestra resumen de la OC (Distribuidor, Periodo, Monto)
- Campo obligatorio: **Motivo del Rechazo** (`RadzenTextArea`, minimo 10 caracteres, maximo 500)
- Mensaje de advertencia: "Esta accion es irreversible. La OC no podra ser reactivada."
- Botones: Rechazar / Cancelar
- Al confirmar ? llama `BonificationSpecialOrderService.RejectAsync`

---

### 2.2.2 Carga Masiva de OC Especiales

> Complemento del camino manual (sección 2.2.1).  
> Permite a PROMOS cargar un archivo **Excel/CSV** con múltiples OC Especiales en un solo paso,  
> en lugar de ingresarlas una a una. Útil al inicio de un período o en ajustes masivos de NC.  
>  
> El archivo es procesado fila a fila; cada fila genera una `BonificationSpecialOrder` en estado `PENDIENTE`.  
> Las OC generadas son idénticas en estructura a las ingresadas manualmente y pasan por el mismo  
> flujo de aprobación/rechazo (TAREA-053/054).

---

**Flujo del proceso:**

```
[Usuario] Descarga plantilla Excel
       ?
[Usuario] Completa el archivo con las OC (Distribuidor, Período, Monto, Descripción)
       ?
[Usuario] Sube el archivo en la pantalla de Carga Masiva
       ?
[Sistema] Valida encabezados y estructura del archivo
       ?
[Sistema] Valida fila a fila:
  - Distribuidor existe y es DISTRIBUIDOR
  - Período está IN_PROGRESS y es BILLING
  - Monto >= 0
  - Descripción >= 10 caracteres
       ?
[Sistema] Muestra resumen previo a confirmación:
  - N filas válidas ? se crearán como PENDIENTE
  - M filas con errores ? se listan con mensaje específico por fila
       ?
[Usuario] Confirma la carga (solo las filas válidas)
       ?
[Sistema] Inserta las OC válidas como PENDIENTE
[Sistema] Descarga reporte de resultado (válidas + errores)
```

**Reglas de procesamiento:**
- Una fila con error **no cancela** el resto del lote — se procesan las válidas y se reportan los errores
- El usuario puede corregir el archivo y volver a subir solo las filas fallidas
- Máximo **500 filas** por archivo (configurable)
- Formatos soportados: `.xlsx` y `.csv` (separado por coma o punto y coma)

---

**Estructura de la plantilla Excel/CSV:**

| Columna | Campo | Obligatorio | Validación |
|---------|-------|-------------|------------|
| A | Número Documento Distribuidor | Sí | Debe existir en Customers con IsDistributor = true |
| B | Código Período | Sí | InstanceCode de BonificationPeriodInstance IN_PROGRESS + BILLING |
| C | Monto | Sí | Decimal >= 0 |
| D | Descripción / Motivo | Sí | 10 a 500 caracteres |

> **Nota:** La plantilla se descarga directamente desde la pantalla de carga masiva e incluye una hoja de instrucciones y una hoja de ejemplo.

---

#### TAREA-055 ? ??
**Agregar método `BulkAddAsync` en `IBonificationSpecialOrderRepository` y `BonificationSpecialOrderService`**

**Cambios en `IBonificationSpecialOrderRepository` (TAREA-049):**
```csharp
Task<int> BulkAddAsync(IEnumerable<BonificationSpecialOrder> orders, CancellationToken ct = default);
```
- Inserta múltiples registros en una sola transacción
- Retorna el número de registros insertados

**Cambios en `IBonificationSpecialOrderService` (TAREA-050):**
```csharp
Task<BulkSpecialOrderResult> BulkAddAsync(
    IEnumerable<BonificationSpecialOrderImportRow> rows,
    int createdBy,
    CancellationToken ct = default);
```

**Modelo de resultado:**
- `Aldebaran.Application.Services\Models\BulkSpecialOrderResult.cs`
  ```csharp
  public class BulkSpecialOrderResult
  {
      public int TotalRows { get; set; }
      public int SuccessCount { get; set; }
      public int ErrorCount { get; set; }
      public IEnumerable<BulkSpecialOrderRowError> Errors { get; set; }
  }

  public class BulkSpecialOrderRowError
  {
      public int RowNumber { get; set; }
      public string IdentityNumber { get; set; }   // Número doc del distribuidor en el archivo
      public string PeriodCode { get; set; }        // Código período en el archivo
      public string ErrorMessage { get; set; }      // Descripción del error
  }
  ```

**Modelo de fila de importación:**
- `Aldebaran.Application.Services\Models\BonificationSpecialOrderImportRow.cs`
  ```csharp
  public class BonificationSpecialOrderImportRow
  {
      public int RowNumber { get; set; }
      public string DistributorIdentityNumber { get; set; }
      public string PeriodInstanceCode { get; set; }
      public decimal Amount { get; set; }
      public string Description { get; set; }
  }
  ```

**Lógica en `BonificationSpecialOrderService.BulkAddAsync`:**
```csharp
public async Task<BulkSpecialOrderResult> BulkAddAsync(
    IEnumerable<BonificationSpecialOrderImportRow> rows,
    int createdBy,
    CancellationToken ct)
{
    var errors = new List<BulkSpecialOrderRowError>();
    var validOrders = new List<BonificationSpecialOrder>();

    foreach (var row in rows)
    {
        // Validar distribuidor
        var customer = await _customerRepo.FindByIdentityNumberAsync(row.DistributorIdentityNumber, ct);
        if (customer == null || !customer.IsDistributor)
        {
            errors.Add(new() { RowNumber = row.RowNumber, IdentityNumber = row.DistributorIdentityNumber,
                PeriodCode = row.PeriodInstanceCode, ErrorMessage = "Distribuidor no encontrado o no es tipo DISTRIBUIDOR" });
            continue;
        }

        // Validar instancia de período
        var instance = await _instanceRepo.FindByCodeAsync(row.PeriodInstanceCode, ct);
        if (instance == null || instance.Status != "IN_PROGRESS" || instance.BonificationType.CalculationBase != "BILLING")
        {
            errors.Add(new() { RowNumber = row.RowNumber, IdentityNumber = row.DistributorIdentityNumber,
                PeriodCode = row.PeriodInstanceCode, ErrorMessage = "Período no encontrado, no está activo o no es de tipo Facturación" });
            continue;
        }

        // Validar monto
        if (row.Amount < 0)
        {
            errors.Add(new() { RowNumber = row.RowNumber, ErrorMessage = "El monto no puede ser negativo" });
            continue;
        }

        // Validar descripción
        if (string.IsNullOrWhiteSpace(row.Description) || row.Description.Length < 10)
        {
            errors.Add(new() { RowNumber = row.RowNumber, ErrorMessage = "La descripción debe tener al menos 10 caracteres" });
            continue;
        }

        validOrders.Add(new BonificationSpecialOrder
        {
            CustomerId = customer.CustomerId,
            BonificationPeriodInstanceId = instance.BonificationPeriodInstanceId,
            Amount = row.Amount,
            Description = row.Description,
            Status = "PENDIENTE",
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        });
    }

    if (validOrders.Any())
        await _repo.BulkAddAsync(_mapper.Map<IEnumerable<Entities.BonificationSpecialOrder>>(validOrders), ct);

    return new BulkSpecialOrderResult
    {
        TotalRows = rows.Count(),
        SuccessCount = validOrders.Count,
        ErrorCount = errors.Count,
        Errors = errors
    };
}
```

---

#### TAREA-056 ? ??
**Crear `IBonificationSpecialOrderImportService` para parseo y validación del archivo**

**Archivo a crear:**
- `Aldebaran.Application.Services\Services\IBonificationSpecialOrderImportService.cs`
  ```csharp
  public interface IBonificationSpecialOrderImportService
  {
      /// <summary>
      /// Parsea el archivo Excel/CSV y retorna las filas como modelos de importación.
      /// NO valida contra BD — solo estructura y tipos de datos.
      /// </summary>
      Task<IEnumerable<BonificationSpecialOrderImportRow>> ParseFileAsync(
          Stream fileStream,
          string fileName,
          CancellationToken ct = default);

      /// <summary>
      /// Genera la plantilla Excel para descarga.
      /// </summary>
      Task<byte[]> GenerateTemplateAsync(CancellationToken ct = default);
  }
  ```

- `Aldebaran.Application.Services\Services\BonificationSpecialOrderImportService.cs`
  - Usa **ClosedXML** para leer `.xlsx` y **CsvHelper** para `.csv` (ambas librerías ya disponibles en el proyecto si se usan en reportes; si no, agregar NuGet)
  - `ParseFileAsync`: lee encabezados, valida que existan las 4 columnas requeridas, parsea cada fila a `BonificationSpecialOrderImportRow`, asigna `RowNumber`
  - `GenerateTemplateAsync`: genera un `.xlsx` con:
    - Hoja 1 "Plantilla": encabezados con formato y columnas bloqueadas
    - Hoja 2 "Instrucciones": descripción de cada campo y ejemplos
    - Hoja 3 "Ejemplo": 3 filas de ejemplo con datos ficticios

**Modificar `ArchitectureBuilderExtensions.cs`:**
```csharp
services.AddTransient<IBonificationSpecialOrderImportService, BonificationSpecialOrderImportService>();
```

---

#### TAREA-057 ? ???
**Crear página `BulkBonificationSpecialOrders.razor` + `.razor.cs`**

**Ruta:** `Pages\BonificationPages\BulkBonificationSpecialOrders.razor`  
**URL:** `/bonification/special-orders/bulk`  
**Rol requerido:** `Administrador`, `Ingreso de OC especiales de bonificación`

**Estructura:**
- Título: "Carga Masiva de OC Especiales"
- Link "? Volver al listado de OC" (navega a `/bonification/special-orders`)

**Paso 1 — Descarga de plantilla:**
- Botón "Descargar Plantilla Excel"
  - Llama `BonificationSpecialOrderImportService.GenerateTemplateAsync()`
  - Descarga archivo `Plantilla_OC_Especiales.xlsx`

**Paso 2 — Carga del archivo:**
- `RadzenUpload` (acepta `.xlsx` y `.csv`, máximo 5 MB)
- Al seleccionar archivo ? muestra nombre y tamaño
- Botón "Procesar Archivo"
  - Llama `BonificationSpecialOrderImportService.ParseFileAsync()`
  - Si hay errores de estructura ? muestra mensaje de error y detiene el proceso

**Paso 3 — Vista previa y confirmación:**
- Muestra resumen antes de confirmar:
  - `RadzenAlert` con: "Se encontraron **N filas válidas** y **M filas con errores**."
- Tabla de filas válidas (solo lectura): Distribuidor, Período, Monto, Descripción
- Tabla de filas con errores: Fila #, Distribuidor, Período, Mensaje de Error
  - Botón "Descargar reporte de errores" (Excel con las filas fallidas)
- Botón "Confirmar Carga" ? solo si hay al menos 1 fila válida
  - Llama `BonificationSpecialOrderService.BulkAddAsync()`
  - Muestra resultado final: "Se crearon N OC Especiales en estado PENDIENTE."
  - Link "Ver OC creadas" ? navega a `/bonification/special-orders?filter=PENDIENTE`

---

#### TAREA-058 ? ???
**Agregar acceso a Carga Masiva desde `BonificationSpecialOrders.razor`**

**Cambios en `BonificationSpecialOrders.razor` (TAREA-051):**
- Agregar botón "Carga Masiva" junto al botón "Nueva OC Especial"
  - Solo visible para rol `Ingreso de OC especiales de bonificación`
  - Navega a `/bonification/special-orders/bulk`

---

#### Resumen de archivos adicionales — Carga Masiva

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `IBonificationSpecialOrderImportService.cs` | 056 | Nuevo |
| `BonificationSpecialOrderImportService.cs` | 056 | Nuevo |
| `Models\BonificationSpecialOrderImportRow.cs` | 055 | Nuevo |
| `Models\BulkSpecialOrderResult.cs` | 055 | Nuevo |
| `IBonificationSpecialOrderRepository.cs` | 055 | Modificar (`BulkAddAsync`) |
| `IBonificationSpecialOrderService.cs` | 055 | Modificar (`BulkAddAsync`) |
| `BonificationSpecialOrderService.cs` | 055 | Modificar (`BulkAddAsync`) |
| `ArchitectureBuilderExtensions.cs` | 056 | Modificar |
| `Pages\BonificationPages\BulkBonificationSpecialOrders.razor` | 057 | Nuevo |

---

> ? **Con TAREA-046 a TAREA-058 queda cubierto el módulo completo de Gestión de OC Especiales,**
> con soporte para ingreso **manual** (una a una) y **masivo** (archivo Excel/CSV).
> Todas las OC pasan por el mismo flujo de aprobación/rechazo y se integran al cálculo del
> Bono por Facturación mediante `GetApprovedTotalAsync`.

---

### 2.2.3 Conciliación de Notas Crédito (período cerrado)

> Ref. propuesta funcional: módulo de Operaciones — Conciliación post-cierre.
>
> Una vez que un `BonificationPeriodInstance` pasa a estado `CLOSED`, el sistema genera una **foto congelada**
> del Bono calculado. Sin embargo, PROMOS debe conciliar esos valores contra la información real
> de **TOTVS** (el ERP), ya que pueden existir:
>
> - **NC conciliadas**: NC que el sistema ya conoce (generadas en el proceso de cálculo)
>   y a las que solo se ajusta el valor real confirmado en TOTVS.
> - **NC externas**: NC bonificadas **fuera del sistema** (acuerdos directos, ajustes manuales
>   en TOTVS) que deben registrarse para que el cuadre quede completo.
>
> **Tres caminos de operación:**
> 1. **Conciliación Manual** — Registro y ajuste NC a NC desde la interfaz (TAREA-063 a TAREA-064)
> 2. **Conciliación Masiva** — Exportar plantilla pre-poblada con las NC del sistema ? usuario
>    completa valores reales en TOTVS ? reimporta el archivo ajustado (TAREA-065 a TAREA-067)
> 3. **NC Externas** — Registro de NC bonificadas fuera del sistema, con su propio flujo de
>    aprobación, manual y masivo (TAREA-062 a TAREA-064 + TAREA-066 + TAREA-068)
>
> **El período debe estar en estado `CLOSED`** para que cualquier conciliación sea posible.

---

**Modelo conceptual:**

```
BonificationPeriodInstance (CLOSED)
    ??? CreditNoteReconciliations          ? NC que el sistema conoce (ajuste de valor)
    ?     ??? NC-001  Valor Sistema: $500,000  ?  Valor TOTVS: $480,000  (CONCILIADA)
    ?     ??? NC-002  Valor Sistema: $300,000  ?  Valor TOTVS: (pendiente) (PENDIENTE)
    ?     ??? NC-003  Valor Sistema: $150,000  ?  Rechazada con motivo    (RECHAZADA)
    ?
    ??? ExternalCreditNotes               ? NC bonificadas fuera del sistema
          ??? NC-EXT-001  Valor: $200,000  (APROBADA)
          ??? NC-EXT-002  Valor: $100,000  (PENDIENTE)
```

**Indicador de cuadre:**
`Valor Cuadre Final = Valor Sistema + Diferencia Conciliada + NC Externas Aprobadas`

| Aspecto | NC Conciliada | NC Externa |
|---------|--------------|------------|
| Origen | Proceso de cálculo del sistema | Acuerdo fuera del sistema |
| Acción | Ajustar valor Sistema ? valor real TOTVS | Registrar desde cero |
| Impacto en cuadre | Diferencia (Sistema ? TOTVS) | Valor completo |
| Requiere aprobación | Sí (si ajuste > umbral configurable) | Siempre |

---

#### TAREA-059 ? ???
**Crear tablas `CreditNoteReconciliations` y `ExternalCreditNotes`**

**Script SQL a crear:** `scripts/CreateCreditNoteReconciliationTables.sql`

```sql
-- NC que el sistema conoce: se ajusta Valor Sistema ? Valor TOTVS
CREATE TABLE dbo.CreditNoteReconciliations (
    RECONCILIATION_ID                INT             NOT NULL IDENTITY(1,1),
    BONIFICATION_PERIOD_INSTANCE_ID  INT             NOT NULL,
    CUSTOMER_ID                      INT             NOT NULL,
    CREDIT_NOTE_NUMBER               VARCHAR(50)     NOT NULL,
    SYSTEM_AMOUNT                    DECIMAL(18,2)   NOT NULL,
    TOTVS_AMOUNT                     DECIMAL(18,2)   NULL,
    -- ?? CORRECCIÓN: NULL significa "no conciliada aún", 0 significa "diferencia real de cero"
    DIFFERENCE                       AS (
        CASE WHEN TOTVS_AMOUNT IS NOT NULL 
             THEN TOTVS_AMOUNT - SYSTEM_AMOUNT 
             ELSE NULL 
        END
    ) PERSISTED,
    STATUS                           VARCHAR(20)     NOT NULL DEFAULT 'PENDIENTE',
    NOTES                            VARCHAR(500)    NULL,
    CREATED_BY                       INT             NOT NULL,
    CREATED_AT                       DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    REVIEWED_BY                      INT             NULL,
    REVIEWED_AT                      DATETIME        NULL,
    REJECTION_REASON                 VARCHAR(500)    NULL,
    CONSTRAINT PK_CREDIT_NOTE_RECONCILIATION  PRIMARY KEY CLUSTERED (RECONCILIATION_ID),
    CONSTRAINT UQ_CREDIT_NOTE_RECONCILIATION  UNIQUE (BONIFICATION_PERIOD_INSTANCE_ID, CREDIT_NOTE_NUMBER),
    CONSTRAINT FK_RECONCILIATION_INSTANCE     FOREIGN KEY (BONIFICATION_PERIOD_INSTANCE_ID)
        REFERENCES dbo.BonificationPeriodInstances (BONIFICATION_PERIOD_INSTANCE_ID),
    CONSTRAINT FK_RECONCILIATION_CUSTOMER     FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID),
    CONSTRAINT CK_RECONCILIATION_STATUS       CHECK (STATUS IN ('PENDIENTE','CONCILIADA','RECHAZADA')),
    CONSTRAINT CK_RECONCILIATION_TOTVS        CHECK (TOTVS_AMOUNT IS NULL OR TOTVS_AMOUNT >= 0)
);
CREATE NONCLUSTERED INDEX IX_RECONCILIATION_INSTANCE_STATUS
    ON dbo.CreditNoteReconciliations (BONIFICATION_PERIOD_INSTANCE_ID, STATUS);

-- NC bonificadas fuera del sistema
CREATE TABLE dbo.ExternalCreditNotes (
    EXTERNAL_CREDIT_NOTE_ID          INT             NOT NULL IDENTITY(1,1),
    BONIFICATION_PERIOD_INSTANCE_ID  INT             NOT NULL,
    CUSTOMER_ID                      INT             NOT NULL,
    CREDIT_NOTE_NUMBER               VARCHAR(50)     NOT NULL,
    AMOUNT                           DECIMAL(18,2)   NOT NULL,
    DESCRIPTION                      VARCHAR(500)    NOT NULL,
    STATUS                           VARCHAR(20)     NOT NULL DEFAULT 'PENDIENTE',
    CREATED_BY                       INT             NOT NULL,
    CREATED_AT                       DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    REVIEWED_BY                      INT             NULL,
    REVIEWED_AT                      DATETIME        NULL,
    REJECTION_REASON                 VARCHAR(500)    NULL,
    CONSTRAINT PK_EXTERNAL_CREDIT_NOTE   PRIMARY KEY CLUSTERED (EXTERNAL_CREDIT_NOTE_ID),
    CONSTRAINT UQ_EXTERNAL_CREDIT_NOTE   UNIQUE (BONIFICATION_PERIOD_INSTANCE_ID, CREDIT_NOTE_NUMBER),
    CONSTRAINT FK_EXTERNAL_CN_INSTANCE   FOREIGN KEY (BONIFICATION_PERIOD_INSTANCE_ID)
        REFERENCES dbo.BonificationPeriodInstances (BONIFICATION_PERIOD_INSTANCE_ID),
    CONSTRAINT FK_EXTERNAL_CN_CUSTOMER   FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID),
    CONSTRAINT CK_EXTERNAL_CN_STATUS     CHECK (STATUS IN ('PENDIENTE','APROBADA','RECHAZADA')),
    CONSTRAINT CK_EXTERNAL_CN_AMOUNT     CHECK (AMOUNT >= 0)
);
CREATE NONCLUSTERED INDEX IX_EXTERNAL_CN_INSTANCE_STATUS
    ON dbo.ExternalCreditNotes (BONIFICATION_PERIOD_INSTANCE_ID, STATUS);
```

---

#### TAREA-060 ? ??
**Crear entidades EF: `CreditNoteReconciliation` y `ExternalCreditNote`**

**Archivos a crear:**

- `Aldebaran.DataAccess\Entities\CreditNoteReconciliation.cs`
  ```csharp
  public class CreditNoteReconciliation
  {
      public int ReconciliationId { get; set; }
      public int BonificationPeriodInstanceId { get; set; }
      public int CustomerId { get; set; }
      public string CreditNoteNumber { get; set; }
      public decimal SystemAmount { get; set; }
      public decimal? TotvsAmount { get; set; }
      public decimal? Difference { get; set; }   // columna calculada — solo lectura
      public string Status { get; set; }          // PENDIENTE | CONCILIADA | RECHAZADA
      public string Notes { get; set; }
      public int CreatedBy { get; set; }
      public DateTime CreatedAt { get; set; }
      public int? ReviewedBy { get; set; }
      public DateTime? ReviewedAt { get; set; }
      public string RejectionReason { get; set; }

      public BonificationPeriodInstance PeriodInstance { get; set; }
      public Customer Customer { get; set; }
  }
  ```

- `Aldebaran.DataAccess\Entities\ExternalCreditNote.cs`
  ```csharp
  public class ExternalCreditNote
  {
      public int ExternalCreditNoteId { get; set; }
      public int BonificationPeriodInstanceId { get; set; }
      public int CustomerId { get; set; }
      public string CreditNoteNumber { get; set; }
      public decimal Amount { get; set; }
      public string Description { get; set; }
      public string Status { get; set; }          // PENDIENTE | APROBADA | RECHAZADA
      public int CreatedBy { get; set; }
      public DateTime CreatedAt { get; set; }
      public int? ReviewedBy { get; set; }
      public DateTime? ReviewedAt { get; set; }
      public string RejectionReason { get; set; }

      public BonificationPeriodInstance PeriodInstance { get; set; }
      public Customer Customer { get; set; }
  }
  ```

- `Aldebaran.DataAccess\Configuration\CreditNoteReconciliationConfiguration.cs`
  - ?? **CORRECCIÓN**: `Difference` ? `HasComputedColumnSql("CASE WHEN TOTVS_AMOUNT IS NOT NULL THEN TOTVS_AMOUNT - SYSTEM_AMOUNT ELSE NULL END", stored: true)`
- `Aldebaran.DataAccess\Configuration\ExternalCreditNoteConfiguration.cs`

**Modificar `AldebaranDbContext.cs`:**
```csharp
public DbSet<CreditNoteReconciliation> CreditNoteReconciliations { get; set; }
public DbSet<ExternalCreditNote> ExternalCreditNotes { get; set; }
```

---

#### TAREA-061 ? ??
**Crear modelos, repositorio y servicio para `CreditNoteReconciliation`**

**Archivos a crear:**
- `Models\CreditNoteReconciliation.cs` — POCO puro + mapping AutoMapper
- `ICreditNoteReconciliationRepository.cs`
  ```csharp
  public interface ICreditNoteReconciliationRepository
  {
      Task AddAsync(CreditNoteReconciliation item, CancellationToken ct = default);
      Task UpdateAsync(int id, CreditNoteReconciliation item, CancellationToken ct = default);
      Task<CreditNoteReconciliation?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<CreditNoteReconciliation>, int)> GetByInstanceAsync(
          int periodInstanceId, string? status, int? skip, int? top, CancellationToken ct = default);
      /// <summary>
      /// ?? CORRECCIÓN: Solo suma NC CONCILIADAS (excluye PENDIENTE y RECHAZADA).
      /// Implementación: SUM(TOTVS_AMOUNT - SYSTEM_AMOUNT) WHERE STATUS = 'CONCILIADA'
      /// </summary>
      Task<decimal> GetConciliatedDifferenceAsync(int periodInstanceId, CancellationToken ct = default);
      Task UpdateStatusAsync(int id, string newStatus, int reviewedBy, DateTime reviewedAt,
          decimal? totvsAmount, string? rejectionReason, CancellationToken ct = default);
      Task<int> BulkAddAsync(IEnumerable<CreditNoteReconciliation> items, CancellationToken ct = default);
  }
  ```
- `ICreditNoteReconciliationService.cs`
  ```csharp
  public interface ICreditNoteReconciliationService
  {
      Task AddAsync(CreditNoteReconciliation item, CancellationToken ct = default);
      Task ConciliateAsync(int id, decimal totvsAmount, int reviewedBy, CancellationToken ct = default);
      Task RejectAsync(int id, int reviewedBy, string rejectionReason, CancellationToken ct = default);
      Task<CreditNoteReconciliation?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<CreditNoteReconciliation>, int)> GetByInstanceAsync(
          int periodInstanceId, string? status, int? skip, int? top, CancellationToken ct = default);
      Task<decimal> GetConciliatedDifferenceAsync(int periodInstanceId, CancellationToken ct = default);
      /// <summary>
      /// ?? CORRECCIÓN: Procesa conciliaciones Y rechazos en una sola transacción (reemplaza BulkConciliateAsync).
      /// </summary>
      Task<BulkReconciliationResult> BulkProcessAsync(
          IEnumerable<ReconciliationImportRow> rows, int reviewedBy, CancellationToken ct = default);
  }
  ```

**Validaciones de negocio:**
- Solo se puede operar si la instancia está `CLOSED`
- Solo se puede conciliar/rechazar si la NC está `PENDIENTE`
- `TotvsAmount >= 0` (se permite `0` para NC reconocidas en TOTVS con valor cero)
- **?? DECISIÓN DE NEGOCIO**: Si la NC **no existe en TOTVS** ? usar **Rechazar**, no conciliar con `TotvsAmount = 0`. El dialog `ConciliateCreditNote` debe incluir nota informativa: _"Si la NC no existe en TOTVS, use Rechazar en lugar de ingresar valor cero."_
- `RejectionReason` obligatoria al rechazar (mín. 10 chars)
- No se puede modificar una NC en estado `CONCILIADA` o `RECHAZADA`

---

#### TAREA-062 ? ??
**Crear modelos, repositorio y servicio para `ExternalCreditNote`**

**Archivos a crear:**
- `Models\ExternalCreditNote.cs` — POCO puro + mapping AutoMapper
- `IExternalCreditNoteRepository.cs`
  ```csharp
  public interface IExternalCreditNoteRepository
  {
      Task AddAsync(ExternalCreditNote item, CancellationToken ct = default);
      Task<ExternalCreditNote?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<ExternalCreditNote>, int)> GetByInstanceAsync(
          int periodInstanceId, string? status, int? skip, int? top, CancellationToken ct = default);
      Task<decimal> GetApprovedTotalAsync(int periodInstanceId, CancellationToken ct = default);
      Task UpdateStatusAsync(int id, string newStatus, int reviewedBy, DateTime reviewedAt,
          string? rejectionReason, CancellationToken ct = default);
      Task<int> BulkAddAsync(IEnumerable<ExternalCreditNote> items, CancellationToken ct = default);
  }
  ```
- `IExternalCreditNoteService.cs`
  ```csharp
  public interface IExternalCreditNoteService
  {
      Task AddAsync(ExternalCreditNote item, CancellationToken ct = default);
      Task ApproveAsync(int id, int reviewedBy, CancellationToken ct = default);
      Task RejectAsync(int id, int reviewedBy, string rejectionReason, CancellationToken ct = default);
      Task<ExternalCreditNote?> FindAsync(int id, CancellationToken ct = default);
      Task<(IEnumerable<ExternalCreditNote>, int)> GetByInstanceAsync(
          int periodInstanceId, string? status, int? skip, int? top, CancellationToken ct = default);
      Task<decimal> GetApprovedTotalAsync(int periodInstanceId, CancellationToken ct = default);
      Task<BulkExternalCNResult> BulkAddAsync(
          IEnumerable<ExternalCNImportRow> rows, int createdBy, CancellationToken ct = default);
  }
  ```

**Validaciones de negocio:**
- Solo se puede agregar si la instancia está `CLOSED`
- `CreditNoteNumber` único dentro del período (valida contra ambas tablas)
- `Amount >= 0`, `Description` mín. 10 chars
- Nacen en estado `PENDIENTE`

**Modificar `ArchitectureBuilderExtensions.cs`:**
```csharp
services.AddTransient<ICreditNoteReconciliationRepository, CreditNoteReconciliationRepository>();
services.AddTransient<ICreditNoteReconciliationService, CreditNoteReconciliationService>();
services.AddTransient<IExternalCreditNoteRepository, ExternalCreditNoteRepository>();
services.AddTransient<IExternalCreditNoteService, ExternalCreditNoteService>();
```

---

#### TAREA-063 ? ???
**Crear página principal `CreditNoteReconciliation.razor` + `.razor.cs`**

**Ruta:** `Pages\BonificationPages\CreditNoteReconciliation.razor`
**URL:** `/bonification/reconciliation`
**Rol requerido:** `Administrador`, `Ingreso de OC especiales de bonificación`

**Estructura:**
- Título: "Conciliación de Notas Crédito"
- Filtros: **Instancia de Período** (solo `CLOSED`) | **Distribuidor** | **Estado** | Botón "Buscar"

**Dos pestañas (`RadzenTabs`):**

**Pestaña 1 — "NC del Sistema":**

| Columna | Detalle |
|---------|---------|
| Número NC | `CreditNoteNumber` |
| Distribuidor | `Customer.CustomerName` |
| Valor Sistema | `SystemAmount` |
| Valor TOTVS | `TotvsAmount` o "Pendiente" |
| Diferencia | `Difference` (verde=0 / rojo?0 / naranja si `\|dif\|>5%`) |
| Estado | ?? PENDIENTE / ?? CONCILIADA / ?? RECHAZADA |
| Acciones | Conciliar / Rechazar (solo PENDIENTE) |

- Botón: "Conciliación Masiva" ? navega a `/bonification/reconciliation/bulk`

**Pestaña 2 — "NC Externas":**

| Columna | Detalle |
|---------|---------|
| Número NC | `CreditNoteNumber` |
| Distribuidor | `Customer.CustomerName` |
| Valor | `Amount` |
| Descripción | truncada 60 chars + tooltip |
| Estado | ?? PENDIENTE / ?? APROBADA / ?? RECHAZADA |
| Acciones | Aprobar / Rechazar (solo PENDIENTE) |

- Botones: "Nueva NC Externa" | "Carga Masiva NC Externas" ? navega a `/bonification/reconciliation/external/bulk`

**Indicador de cuadre** (se actualiza al seleccionar instancia):
```
Valor Sistema Total:    $X,XXX,XXX
Diferencia Conciliada:  $±X,XXX
NC Externas Aprobadas:  $X,XXX
??????????????????????????????????
Valor Cuadre Final:     $X,XXX,XXX
```

---

#### TAREA-064 ? ???
**Crear 4 dialogs de conciliación manual**

**1. `ConciliateCreditNote.razor` + `.razor.cs`** — Conciliar NC del sistema
- Muestra: Número NC, Distribuidor, Valor Sistema
- Campo editable: **Valor TOTVS** (`RadzenNumeric`, obligatorio, `>= 0`)
- Campo opcional: **Notas**
- Preview en tiempo real: `Diferencia = TotvsAmount ? SystemAmount`
- Alerta naranja si `|Diferencia| > 5%` del valor sistema
- **?? NOTA INFORMATIVA**: _"Si esta NC **no existe en TOTVS**, use el botón **Rechazar** en lugar de ingresar valor cero. Valor cero solo debe usarse cuando TOTVS reconoce la NC con monto $0."_
- Al confirmar ? `CreditNoteReconciliationService.ConciliateAsync`

**2. `RejectReconciliation.razor` + `.razor.cs`** — Rechazar NC del sistema
- Motivo obligatorio (`RadzenTextArea`, mín. 10 chars, máx. 500)
- Al confirmar ? `CreditNoteReconciliationService.RejectAsync`

**3. `AddExternalCreditNote.razor` + `.razor.cs`** — Nueva NC externa
- Distribuidor + Número NC + Valor + Descripción/Justificación obligatoria
- Nota: "Quedará en estado PENDIENTE hasta aprobación."
- Al guardar ? `ExternalCreditNoteService.AddAsync`

**4. `ApproveRejectExternalCreditNote.razor` + `.razor.cs`** — Aprobar o rechazar NC externa
- Modo Aprobar: resumen + confirmación
- Modo Rechazar: resumen + motivo obligatorio
- Parámetro `bool IsApproving` para reutilizar el componente

---

#### TAREA-065 ? ??
**Crear `ICreditNoteReconciliationImportService` — plantilla pre-poblada + reimportación**

> **Flujo diferenciador vs carga masiva de OC:**
> El archivo que se exporta **ya viene con los datos del sistema** (NC, distribuidor, valor sistema).
> El usuario **solo completa** la columna `Valor TOTVS`. Las filas sin ese valor son ignoradas.

**Archivos a crear:**
- `ICreditNoteReconciliationImportService.cs`
  ```csharp
  public interface ICreditNoteReconciliationImportService
  {
      /// <summary>
      /// Genera Excel pre-poblado con las NC PENDIENTES de la instancia.
      /// ?? CORRECCIÓN: Incluye columnas de Acción (CONCILIAR/RECHAZAR) y Motivo Rechazo.
      /// Columnas bloqueadas: ReconciliationId (oculta), Número NC, Distribuidor, Valor Sistema.
      /// Columnas editables: Valor TOTVS, Acción (dropdown), Motivo Rechazo, Notas.
      /// </summary>
      Task<byte[]> GenerateReconciliationFileAsync(int periodInstanceId, CancellationToken ct = default);

      /// <summary>
      /// Parsea el archivo reimportado. Retorna filas con Acción = CONCILIAR o RECHAZAR.
      /// Filas sin acción son ignoradas.
      /// </summary>
      Task<IEnumerable<ReconciliationImportRow>> ParseFileAsync(
          Stream fileStream, string fileName, CancellationToken ct = default);
  }
  ```

- `Models\ReconciliationImportRow.cs`
  ```csharp
  public class ReconciliationImportRow
  {
      public int ReconciliationId { get; set; }
      public string CreditNoteNumber { get; set; }
      public string Action { get; set; }           // ?? NUEVO: CONCILIAR | RECHAZAR | (vacío = ignorar)
      public decimal? TotvsAmount { get; set; }    // ?? CORRECCIÓN: null si Action = RECHAZAR
      public string Notes { get; set; }
      public string RejectionReason { get; set; }  // ?? NUEVO: requerido si Action = RECHAZAR
  }
  ```

- `Models\BulkReconciliationResult.cs`
  ```csharp
  public class BulkReconciliationResult
  {
      public int TotalRows { get; set; }
      public int SuccessCount { get; set; }
      public int ErrorCount { get; set; }
      public IEnumerable<BulkReconciliationRowError> Errors { get; set; }
  }
  public class BulkReconciliationRowError
  {
      public int RowNumber { get; set; }
      public string CreditNoteNumber { get; set; }
      public string ErrorMessage { get; set; }
  }
  ```

**?? ESTRUCTURA DE LA PLANTILLA EXCEL (CORRECCIÓN):**

| Columna | Estado | Valor por defecto |
|---------|--------|-------------------|
| ReconciliationId | Oculta | Pre-poblada por el sistema |
| Número NC | Bloqueada | Pre-poblada por el sistema |
| Distribuidor | Bloqueada | Pre-poblada por el sistema |
| Valor Sistema | Bloqueada | Pre-poblada por el sistema |
| **Valor TOTVS** | Editable (amarillo) | Vacío |
| **Acción** | Editable (dropdown) | Vacío — opciones: `CONCILIAR` / `RECHAZAR` |
| **Motivo Rechazo** | Editable | Vacío — obligatorio si Acción = RECHAZAR |
| Notas | Editable | Vacío |

**Reglas de validación en `ParseFileAsync`:**
- Fila sin `Acción` ? ignorada (no es error)
- `Acción = CONCILIAR` + `Valor TOTVS` vacío ? error: "Valor TOTVS requerido para conciliar"
- `Acción = RECHAZAR` + `Motivo Rechazo` vacío ? error: "Motivo obligatorio para rechazar"
- `Acción = RECHAZAR` + `Valor TOTVS` completado ? se ignora el valor TOTVS (solo se procesa el rechazo)

---

#### TAREA-066 ? ??
**Crear `IExternalCreditNoteImportService` — plantilla en blanco + carga masiva NC externas**

**Archivos a crear:**
- `IExternalCreditNoteImportService.cs`
  ```csharp
  public interface IExternalCreditNoteImportService
  {
      Task<byte[]> GenerateTemplateAsync(CancellationToken ct = default);
      Task<IEnumerable<ExternalCNImportRow>> ParseFileAsync(
          Stream fileStream, string fileName, CancellationToken ct = default);
  }
  ```

- `Models\ExternalCNImportRow.cs`
  ```csharp
  public class ExternalCNImportRow
  {
      public int RowNumber { get; set; }
      public string DistributorIdentityNumber { get; set; }
      public string CreditNoteNumber { get; set; }
      public decimal Amount { get; set; }
      public string Description { get; set; }
  }
  ```

- `Models\BulkExternalCNResult.cs` (mismo patrón que `BulkReconciliationResult`)

**Plantilla (4 columnas):** Número Doc Distribuidor | Número NC | Valor | Descripción/Justificación

**Modificar `ArchitectureBuilderExtensions.cs`:**
```csharp
services.AddTransient<ICreditNoteReconciliationImportService, CreditNoteReconciliationImportService>();
services.AddTransient<IExternalCreditNoteImportService, ExternalCreditNoteImportService>();
```

---

#### TAREA-067 ? ???
**Crear página `BulkCreditNoteReconciliation.razor` + `.razor.cs` — conciliación masiva NC sistema**

**Ruta:** `Pages\BonificationPages\BulkCreditNoteReconciliation.razor`
**URL:** `/bonification/reconciliation/bulk`
**Rol requerido:** `Administrador`, `Ingreso de OC especiales de bonificación`

**Flujo en 3 pasos:**

**Paso 1 — Generar archivo:**
- Dropdown **Instancia de Período** (solo `CLOSED` con NC pendientes)
- Botón "Generar Archivo de Conciliación"
  - Llama `CreditNoteReconciliationImportService.GenerateReconciliationFileAsync(instanceId)`
  - Descarga `Conciliacion_{InstanceCode}_{Fecha}.xlsx`
- **?? NOTA INFORMATIVA**: "El archivo contiene las NC pendientes. Complete las columnas **Valor TOTVS** (para conciliar) o **Acción = RECHAZAR + Motivo** (para rechazar). Las filas sin acción serán ignoradas."

**Paso 2 — Reimportar:**
- `RadzenUpload` (`.xlsx`, máx. 5 MB) + Botón "Procesar Archivo"
  - Llama `ParseFileAsync()` ? muestra filas procesadas

**Paso 3 — Vista previa y confirmación (?? 3 SECCIONES):**
- **`RadzenAlert`**: "Se procesarán **N conciliaciones**, **M rechazos** y **K filas ignoradas** (sin acción)."

**Tabla 1 — "NC a Conciliar" (verde claro):**
- Columnas: Número NC | Distribuidor | Valor Sistema | Valor TOTVS | Diferencia
  - Diferencia coloreada: verde si = 0 / rojo si ? 0 / naranja si `|Diferencia| > 5%`

**Tabla 2 — "NC a Rechazar" (rojo claro):**
- Columnas: Número NC | Distribuidor | Valor Sistema | Motivo Rechazo

**Tabla 3 — "Filas Ignoradas" (gris):**
- Columnas: Fila # | Número NC | Motivo
  - Muestra filas sin acción (no es error, es información)

**Tabla 4 — "Errores de Validación" (solo si existen):**
- Columnas: Fila # | Número NC | Mensaje Error
- Botón "Descargar reporte de errores" (Excel con filas fallidas)

**Botón "Confirmar Procesamiento"** (solo si hay al menos 1 fila válida):
- Llama `CreditNoteReconciliationService.BulkProcessAsync()`
- Resultado: "Se conciliaron **N NC** y rechazaron **M NC** exitosamente."
- Link "Ver conciliación" ? navega a `/bonification/reconciliation?instance={id}`

---

#### TAREA-068 ? ???
**Crear página `BulkExternalCreditNotes.razor` + `.razor.cs` — carga masiva NC externas**

**Ruta:** `Pages\BonificationPages\BulkExternalCreditNotes.razor`
**URL:** `/bonification/reconciliation/external/bulk`
**Rol requerido:** `Administrador`, `Ingreso de OC especiales de bonificación`

**Estructura (flujo en 3 pasos — idéntico al patrón TAREA-067 pero para NC externas):**

**Paso 1 — Plantilla y selección de instancia:**
- Dropdown **Instancia de Período** (solo `CLOSED`) — obligatorio antes de continuar
- Botón "Descargar Plantilla"
  - Llama `ExternalCreditNoteImportService.GenerateTemplateAsync()`
  - Descarga `Plantilla_NC_Externas.xlsx` con instrucciones incluidas

**Paso 2 — Carga y procesamiento:**
- `RadzenUpload` + Botón "Procesar Archivo"
  - Llama `ParseFileAsync()` — valida estructura y tipos, no valida contra BD

**Paso 3 — Vista previa y confirmación:**
- `RadzenAlert`: "N filas válidas y M filas con errores."
- Tabla válidas: Distribuidor, Número NC, Valor, Descripción
- Tabla errores: Fila #, Distribuidor, Número NC, Mensaje
  - Botón "Descargar reporte de errores"
- Botón "Confirmar Carga" ? `ExternalCreditNoteService.BulkAddAsync()`
- Resultado: "Se registraron N NC Externas en estado PENDIENTE."
- Link "Ver NC Externas" ? navega a `/bonification/reconciliation?instance={id}&tab=external`

---

#### TAREA-069 ? ???
**Agregar "Conciliación de NC" al menú Operaciones en `MainLayout.razor`**

**Cambio en el bloque de TAREA-045:**
```razor
<RadzenPanelMenuItem Text="Operaciones" Icon="edit_note">
    <RadzenPanelMenuItem Text="OC Especiales"
        Path="bonification/special-orders"
        Visible="@Security.IsInRole("Administrador","Ingreso de OC especiales de bonificación")" />
    <RadzenPanelMenuItem Text="Conciliación de NC"
        Path="bonification/reconciliation"
        Visible="@Security.IsInRole("Administrador","Ingreso de OC especiales de bonificación")" />
</RadzenPanelMenuItem>
```

---

#### Resumen de archivos — Conciliación de NC

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `scripts/CreateCreditNoteReconciliationTables.sql` | 059 | Nuevo |
| `Entities\CreditNoteReconciliation.cs` | 060 | Nuevo |
| `Entities\ExternalCreditNote.cs` | 060 | Nuevo |
| `Configuration\CreditNoteReconciliationConfiguration.cs` | 060 | Nuevo |
| `Configuration\ExternalCreditNoteConfiguration.cs` | 060 | Nuevo |
| `AldebaranDbContext.cs` | 060 | Modificar |
| `Models\CreditNoteReconciliation.cs` | 061 | Nuevo |
| `Models\ExternalCreditNote.cs` | 062 | Nuevo |
| `Models\ReconciliationImportRow.cs` | 065 | Nuevo |
| `Models\BulkReconciliationResult.cs` | 065 | Nuevo |
| `Models\ExternalCNImportRow.cs` | 066 | Nuevo |
| `Models\BulkExternalCNResult.cs` | 066 | Nuevo |
| `ApplicationServicesProfile.cs` | 061, 062 | Modificar |
| `ICreditNoteReconciliationRepository.cs` | 061 | Nuevo |
| `CreditNoteReconciliationRepository.cs` | 061 | Nuevo |
| `IExternalCreditNoteRepository.cs` | 062 | Nuevo |
| `ExternalCreditNoteRepository.cs` | 062 | Nuevo |
| `ICreditNoteReconciliationService.cs` | 061 | Nuevo |
| `CreditNoteReconciliationService.cs` | 061 | Nuevo |
| `IExternalCreditNoteService.cs` | 062 | Nuevo |
| `ExternalCreditNoteService.cs` | 062 | Nuevo |
| `ICreditNoteReconciliationImportService.cs` | 065 | Nuevo |
| `CreditNoteReconciliationImportService.cs` | 065 | Nuevo |
| `IExternalCreditNoteImportService.cs` | 066 | Nuevo |
| `ExternalCreditNoteImportService.cs` | 066 | Nuevo |
| `ArchitectureBuilderExtensions.cs` | 061, 062, 065, 066 | Modificar |
| `Pages\BonificationPages\CreditNoteReconciliation.razor` | 063 | Nuevo |
| `Pages\BonificationPages\CreditNoteReconciliation.razor.cs` | 063 | Nuevo |
| `Pages\BonificationPages\ConciliateCreditNote.razor` | 064 | Nuevo |
| `Pages\BonificationPages\ConciliateCreditNote.razor.cs` | 064 | Nuevo |
| `Pages\BonificationPages\RejectReconciliation.razor` | 064 | Nuevo |
| `Pages\BonificationPages\RejectReconciliation.razor.cs` | 064 | Nuevo |
| `Pages\BonificationPages\AddExternalCreditNote.razor` | 064 | Nuevo |
| `Pages\BonificationPages\AddExternalCreditNote.razor.cs` | 064 | Nuevo |
| `Pages\BonificationPages\ApproveRejectExternalCreditNote.razor` | 064 | Nuevo |
| `Pages\BonificationPages\ApproveRejectExternalCreditNote.razor.cs` | 064 | Nuevo |
| `Pages\BonificationPages\BulkCreditNoteReconciliation.razor` | 067 | Nuevo |
| `Pages\BonificationPages\BulkCreditNoteReconciliation.razor.cs` | 067 | Nuevo |
| `Pages\BonificationPages\BulkExternalCreditNotes.razor` | 068 | Nuevo |
| `Pages\BonificationPages\BulkExternalCreditNotes.razor.cs` | 068 | Nuevo |
| `Shared\MainLayout.razor` | 069 | Modificar |

---

> ? **Con TAREA-059 a TAREA-069 queda cubierto el módulo de Conciliación de Notas Crédito.**
> El módulo soporta los tres caminos: conciliación manual NC a NC, conciliación masiva con plantilla
> pre-poblada por el sistema (incluyendo rechazo masivo), y registro de NC externas (manual y masivo).
>
> El **indicador de cuadre** en la pantalla principal consolida:
> `Valor Cuadre Final = Valor Sistema + Diferencia Conciliada + NC Externas Aprobadas`
> y es el insumo para el cierre contable del período por parte de PROMOS.

---

## Validación de Cobertura de Escenarios de Conciliación

### Matriz de Cobertura

| # | Escenario | Camino | Tareas que lo cubren | Estado |
|---|-----------|--------|---------------------|--------|
| 1 | **Manual**: NC con valores iguales Sistema = TOTVS | Manual | TAREA-064 (`ConciliateCreditNote` ? Diferencia = 0 ? verde) | ? Cubierto |
| 2 | **Manual**: NC con valores diferentes Sistema ? TOTVS | Manual | TAREA-064 (`ConciliateCreditNote` ? Diferencia ? 0 ? rojo/naranja) | ? Cubierto |
| 3 | **Manual**: NC existe en TOTVS pero NO en Bonificación | Manual | TAREA-064 (`AddExternalCreditNote` ? NC Externa PENDIENTE ? Aprobar) | ? Cubierto |
| 4 | **Manual**: NC existe en Bonificación pero NO en TOTVS | Manual | TAREA-064 (`RejectReconciliation` con motivo "NC no existe en TOTVS") | ? Cubierto (con correcciones aplicadas) |
| 5 | **Masiva**: NC con valores iguales Sistema = TOTVS | Masiva | TAREA-067 (Paso 3 ? Tabla "A Conciliar" ? Diferencia = 0 verde) | ? Cubierto |
| 6 | **Masiva**: NC con valores diferentes Sistema ? TOTVS | Masiva | TAREA-067 (Paso 3 ? Tabla "A Conciliar" ? Diferencia ? 0 rojo/naranja) | ? Cubierto |
| 7 | **Masiva**: NC existe en TOTVS pero NO en Bonificación | Masiva | TAREA-068 (plantilla en blanco ? carga NC externas) | ? Cubierto |
| 8 | **Masiva**: NC existe en Bonificación pero NO en TOTVS | Masiva | TAREA-067 (Acción = RECHAZAR + Motivo ? Tabla "A Rechazar") | ? Cubierto (con correcciones aplicadas) |

### Correcciones Aplicadas para Cerrar Brechas

**Brecha 4 (Escenario 4 — Manual: NC en Bonificación sin TOTVS):**
- ? **Corrección 1 (TAREA-059)**: Columna `DIFFERENCE` usa `CASE WHEN TOTVS_AMOUNT IS NOT NULL` — `NULL` diferencia de `0`
- ? **Corrección 2 (TAREA-060)**: EF Configuration actualizada con el CASE
- ? **Corrección 3 (TAREA-061)**: `GetConciliatedDifferenceAsync` **solo suma STATUS = 'CONCILIADA'**, excluye PENDIENTE y RECHAZADA
- ? **Corrección 4 (TAREA-061 + 064)**: Regla de negocio documentada: _"Si NC no existe en TOTVS ? Rechazar, no conciliar con 0"_
- ? **Corrección 5 (TAREA-064)**: Dialog `ConciliateCreditNote` incluye nota informativa guiando al usuario

**Brecha 8 (Escenario 8 — Masiva: NC en Bonificación sin TOTVS):**
- ? **Corrección 6 (TAREA-065)**: Plantilla Excel incluye columna **Acción** (CONCILIAR / RECHAZAR)
- ? **Corrección 7 (TAREA-065)**: Plantilla Excel incluye columna **Motivo Rechazo** (obligatoria si Acción = RECHAZAR)
- ? **Corrección 8 (TAREA-065)**: Modelo `ReconciliationImportRow` incluye campos `Action` y `RejectionReason`
- ? **Corrección 9 (TAREA-061)**: Servicio tiene método `BulkProcessAsync` que procesa conciliaciones **y rechazos** en una transacción
- ? **Corrección 10 (TAREA-067)**: Pantalla muestra 3 tablas de resultado: NC a Conciliar / NC a Rechazar / Ignoradas

### Resultado Final

**Todos los 8 escenarios están completamente cubiertos** con las correcciones aplicadas:
- Escenarios 1-7: ya estaban cubiertos en el diseño original
- Escenario 4 (Manual NC sin TOTVS): cerrado con correcciones 1-5
- Escenario 8 (Masiva NC sin TOTVS): cerrado con correcciones 6-10

**No quedan brechas pendientes.** El módulo de Conciliación de NC está completo y operativo.

---

### 2.2.4 Lista de Precios Promocional

> Ref. contexto funcional: La página promocional (https://www.catalogospromocionales.com) publica diariamente un archivo Excel con los precios del día de todos los artículos. Este archivo es el **insumo crítico** para el cálculo del Bono por Facturación, ya que los precios varían día a día.
>
> **Proceso automático:** Un job nocturno descarga el archivo a las 6 AM (hora configurable) y lo carga en el sistema, archivando la lista del día anterior.
>
> **Contingencia manual:** Si el proceso automático falla o si se publican ajustes dentro del día, un usuario con rol de administración/bonificación puede recargar la lista manualmente.
>
> **Escenarios de descarga soportados:**
> 1. **Descarga directa** — Sin autenticación (actual política del proveedor)
> 2. **Descarga con autenticación** — Usuario/contraseña de distribuidor configurados (previsión futura)

---

#### Estructura de datos y servicios base

#### TAREA-070 ??? — Crear tablas `PromotionalPriceLists` y `PromotionalPriceListItems`

**Script SQL a crear:** `scripts/CreatePromotionalPriceListTables.sql`

```sql
-- Encabezado de la lista (una por día)
CREATE TABLE dbo.PromotionalPriceLists (
    PRICE_LIST_ID     INT           NOT NULL IDENTITY(1,1),
    LIST_DATE         DATE          NOT NULL,           -- fecha de vigencia
    STATUS            VARCHAR(20)   NOT NULL DEFAULT 'ACTIVE', -- ACTIVE | HISTORICAL
    SOURCE            VARCHAR(20)   NOT NULL DEFAULT 'AUTOMATIC', -- AUTOMATIC | MANUAL
    LOADED_BY         INT           NULL,               -- NULL si automático, FK ApplicationUser si manual
    LOADED_AT         DATETIME      NOT NULL DEFAULT GETUTCDATE(),
    FILE_NAME         VARCHAR(255)  NULL,               -- nombre del archivo fuente
    NOTES             VARCHAR(500)  NULL,               -- motivo si carga manual o recarga del día
    CONSTRAINT PK_PROMOTIONAL_PRICE_LIST PRIMARY KEY CLUSTERED (PRICE_LIST_ID),
    CONSTRAINT UQ_PRICE_LIST_DATE_ACTIVE UNIQUE (LIST_DATE, STATUS), -- solo 1 ACTIVE por fecha
    CONSTRAINT CK_PRICE_LIST_STATUS CHECK (STATUS IN ('ACTIVE','HISTORICAL')),
    CONSTRAINT CK_PRICE_LIST_SOURCE CHECK (SOURCE IN ('AUTOMATIC','MANUAL'))
);
CREATE NONCLUSTERED INDEX IX_PRICE_LIST_DATE_STATUS
    ON dbo.PromotionalPriceLists (LIST_DATE, STATUS);

-- Líneas de la lista (columnas A-M del Excel)
CREATE TABLE dbo.PromotionalPriceListItems (
    PRICE_LIST_ITEM_ID  INT             NOT NULL IDENTITY(1,1),
    PRICE_LIST_ID       INT             NOT NULL,
    ITEM_CODE           VARCHAR(50)     NOT NULL,       -- columna A: Referencia
    ITEM_NAME           VARCHAR(500)    NULL,           -- columna B: NombreProducto
    FEATURES            VARCHAR(1000)   NULL,           -- columna C: Caracteristicas
    PRICE1_DESC         VARCHAR(100)    NULL,           -- columna D: DescPrecio1
    PRICE1              DECIMAL(18,4)   NULL,           -- columna E: Precio1
    PRICE2_DESC         VARCHAR(100)    NULL,           -- columna F: DescPrecio2
    PRICE2              DECIMAL(18,4)   NULL,           -- columna G: Precio2
    PRICE3_DESC         VARCHAR(100)    NULL,           -- columna H: DescPrecio3
    PRICE3              DECIMAL(18,4)   NULL,           -- columna I: Precio3
    PRICE4_DESC         VARCHAR(100)    NULL,           -- columna J: DescPrecio4
    PRICE4              DECIMAL(18,4)   NULL,           -- columna K: Precio4
    PRICE5_DESC         VARCHAR(100)    NULL,           -- columna L: DescPrecio5
    PRICE5              DECIMAL(18,4)   NULL,           -- columna M: Precio5
    CONSTRAINT PK_PRICE_LIST_ITEM PRIMARY KEY CLUSTERED (PRICE_LIST_ITEM_ID),
    CONSTRAINT UQ_PRICE_LIST_ITEM UNIQUE (PRICE_LIST_ID, ITEM_CODE),
    CONSTRAINT FK_PRICE_LIST_ITEM_LIST FOREIGN KEY (PRICE_LIST_ID)
        REFERENCES dbo.PromotionalPriceLists (PRICE_LIST_ID) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_PRICE_LIST_ITEM_LIST
    ON dbo.PromotionalPriceListItems (PRICE_LIST_ID);
CREATE NONCLUSTERED INDEX IX_PRICE_LIST_ITEM_CODE
    ON dbo.PromotionalPriceListItems (ITEM_CODE);
```

**Estimación:** 3 horas | Prioridad: ?? REQUERIDO

---

#### TAREA-071 ?? — Entidades EF + Configurations + Models + Mappings

**Archivos a crear:**

- `Aldebaran.DataAccess\Entities\PromotionalPriceList.cs` + `PromotionalPriceListItem.cs`
- `Aldebaran.DataAccess\Configuration\PromotionalPriceListConfiguration.cs` + `PromotionalPriceListItemConfiguration.cs`
- `Aldebaran.Application.Services\Models\PromotionalPriceList.cs` + `PromotionalPriceListItem.cs` (POCO)

**Modificar:**
- `AldebaranDbContext.cs` ? agregar `DbSet<PromotionalPriceList>` + `DbSet<PromotionalPriceListItem>`
- `ApplicationServicesProfile.cs` ? mappings AutoMapper

**Estimación:** 5 horas | Prioridad: ?? REQUERIDO

---

#### TAREA-072 ?? — Repositorio y servicio `IPromotionalPriceListRepository` / `IPromotionalPriceListService`

**Archivos a crear:**
- `IPromotionalPriceListRepository.cs` + `PromotionalPriceListRepository.cs`
  - `GetActiveForDateAsync(DateTime date)` ? lista activa de una fecha específica
  - `GetMostRecentActiveAsync()` ? lista activa más reciente (fallback si no hay del día)
  - `LoadDayListAsync(...)` ? archiva anterior y activa nueva
  - `GetItemPriceAsync(string itemCode, DateTime date)` ? retorna primer precio > 0 de un artículo
- `IPromotionalPriceListService.cs` + `PromotionalPriceListService.cs`

**Lógica de `LoadDayListAsync`:**
1. Archivar lista `ACTIVE` del día (si existe) ? `STATUS = HISTORICAL`
2. Insertar nueva lista con `STATUS = ACTIVE`
3. Validación: mínimo 1 ítem

**Lógica de `GetItemPriceAsync`:**
- Retorna el primer precio > 0 en orden: `Price1`, `Price2`, `Price3`, `Price4`, `Price5`
- Si no hay lista activa del día ? busca la más reciente

**Modificar `ArchitectureBuilderExtensions.cs`:**
```csharp
services.AddTransient<IPromotionalPriceListRepository, PromotionalPriceListRepository>();
services.AddTransient<IPromotionalPriceListService, PromotionalPriceListService>();
```

**Estimación:** 7 horas | Prioridad: ?? REQUERIDO

---

#### Proceso automático de descarga

#### TAREA-073 ?? — Servicio de descarga HTTP + parseo `IPriceListFetchService`

**Archivo a crear:** `Aldebaran.Application.FileWritingService\Services\IPriceListFetchService.cs` + implementación

**Responsabilidad:** Descarga el archivo desde `https://www.catalogospromocionales.com/distribuidores/referenciasexcel` y parsea las 13 columnas (A-M).

**Escenarios soportados:**
1. **Descarga directa** (sin autenticación) — `UseAuthentication = false` en config
2. **Descarga autenticada** — Login previo con usuario/contraseña de distribuidor

**Configuración en `appsettings.json` del `FileWritingService`:**
```json
{
  "PriceListFetchOptions": {
    "Url": "https://www.catalogospromocionales.com/distribuidores/referenciasexcel",
    "UseAuthentication": false,
    "LoginUrl": "https://www.catalogospromocionales.com/distribuidores/login",
    "Username": "",
    "Password": "",
    "CronExpression": "0 0 6 * * *",
    "NotificationRecipients": ["bonificacion@promos.com"]
  }
}
```

**Parseo con ClosedXML:**
- Lee columnas A-M (13 columnas)
- Valida encabezados mínimos
- Convierte cada fila a `PromotionalPriceListItem`

**Estimación:** 8 horas | Prioridad: ?? REQUERIDO

---

#### TAREA-074 ?? — Worker automático `PriceListFetchWorker`

**Archivo a crear:** `Aldebaran.Application.FileWritingService\Workers\PriceListFetchWorker.cs`

**Patrón idéntico a `InventoryFtpPdfWorker`:**
- Programación con NCrontab (configurable, default: 6 AM)
- Descarga ? parseo ? carga en BD ? notificación email

**Flujo:**
1. `IPriceListFetchService.FetchTodayListAsync()` ? descarga y parsea
2. `IPromotionalPriceListService.LoadDayListAsync(items, today, "AUTOMATIC", null, fileName, null)`
3. Notificación de éxito o fallo vía email
4. Si falla: `ResilientExecutor` reintenta N veces

**Registrar en `Program.cs` del `FileWritingService`:**
```csharp
services.AddHttpClient("PriceListDownloader", client => {
    client.Timeout = TimeSpan.FromMinutes(5);
    client.DefaultRequestHeaders.Add("User-Agent", "AldebaranSystem/1.0");
});
services.AddHostedService<PriceListFetchWorker>();
services.AddTransient<IPriceListFetchService, PriceListFetchService>();
services.AddTransient<IPromotionalPriceListService, PromotionalPriceListService>();
```

**Estimación:** 6 horas | Prioridad: ?? REQUERIDO

---

#### Contingencia manual

#### TAREA-075 ?? — Servicio de parseo manual `IPromotionalPriceListImportService`

**Archivo a crear:** `Aldebaran.Application.Services\Services\IPromotionalPriceListImportService.cs`

**Responsabilidad:** Parsea un archivo Excel subido manualmente (reutiliza lógica de `IPriceListFetchService`).

**Extracción a clase compartida:** `PriceListParser.ParseExcelAsync(Stream)` — usado por ambos servicios.

**Estimación:** 3 horas | Prioridad: ?? REQUERIDO

---

#### TAREA-076 ??? — Página `PromotionalPriceLists.razor` + carga manual

**Ruta:** `Pages\BonificationPages\PromotionalPriceLists.razor`
**URL:** `/bonification/price-lists`
**Rol:** `Administrador`, `Modificación de bonificaciones`

**Estructura:**

**Indicador de estado:**
```razor
@if (ActiveToday != null)
{
    <RadzenAlert AlertStyle="AlertStyle.Success">
        Lista activa hoy: @ActiveToday.ListDate — @ActiveToday.Items.Count artículos — 
        cargada @ActiveToday.LoadedAt vía @(ActiveToday.Source == "AUTOMATIC" ? "automático" : "manual")
    </RadzenAlert>
}
else if (MostRecentActive != null)
{
    <RadzenAlert AlertStyle="AlertStyle.Warning">
        ?? No hay lista activa para hoy. Se está usando la del @MostRecentActive.ListDate 
        (@MostRecentActive.Items.Count artículos). Cargue la lista del día manualmente.
    </RadzenAlert>
}
else
{
    <RadzenAlert AlertStyle="AlertStyle.Danger">
        ? No hay ninguna lista de precios. El cálculo de bonificación no puede realizarse.
    </RadzenAlert>
}
```

**Grilla historial:**
- Columnas: Fecha | Estado | Fuente | Cargada | Artículos | Notas | Acciones (Ver ítems)
- Row expand: tabla de ítems (Código, Nombre, Precio1-5)

**Sección de contingencia manual:**
- `RadzenUpload` (`.xlsx`/`.xls`, máx. 10 MB)
- `RadzenTextArea` para notas (obligatorio, mín. 10 chars)
- Botón "Cargar Lista del Día"
  - Preview de N ítems parseados
  - Confirmación: "Reemplazará la lista activa de hoy. La anterior pasará a HISTÓRICO."
  - Llama `IPromotionalPriceListService.LoadDayListAsync(..., "MANUAL", currentUserId, fileName, notes)`

**Estimación:** 12 horas | Prioridad: ?? REQUERIDO

---

#### TAREA-077 ??? — Notificación en Dashboard cuando no hay lista activa

**Archivo a modificar:** Dashboard o `MainLayout.razor` (componente de alertas administrativas)

**Lógica:**
```csharp
var activeToday = await priceListService.GetActiveForTodayAsync();
if (activeToday == null && Security.IsInRole("Administrador", "Modificación de bonificaciones"))
{
    var mostRecent = await priceListService.GetMostRecentActiveAsync();
    var message = mostRecent != null
        ? $"?? No hay lista de precios para hoy. Se usa la del {mostRecent.ListDate:dd/MM/yyyy}. <a href='/bonification/price-lists'>Cargar manual</a>"
        : "? No hay lista de precios. <a href='/bonification/price-lists'>Cargar urgente</a>";
    NotificationService.Notify(NotificationSeverity.Warning, "Lista de Precios", message, duration: 0);
}
```

**Estimación:** 2 horas | Prioridad: ?? REQUERIDO

---

#### TAREA-078 ??? — Agregar "Lista de Precios" al menú Configuración en `MainLayout.razor`

**Modificar el bloque de TAREA-045:**
```razor
<RadzenPanelMenuItem Text="Configuración para Bonificaciones" Icon="settings">
    <RadzenPanelMenuItem Text="Períodos" Path="bonification/periods" ... />
    <RadzenPanelMenuItem Text="Tipos de Bono" Path="bonification/types" ... />
    <RadzenPanelMenuItem Text="Vigencias" Path="bonification/vigencies" ... />
    <RadzenPanelMenuItem Text="Descuentos por Pedido" Path="bonification/discount-vigencies" ... />
    <RadzenPanelMenuItem Text="Lista de Precios"
        Path="bonification/price-lists"
        Visible="@Security.IsInRole("Administrador","Modificación de bonificaciones")" />
</RadzenPanelMenuItem>
```

**Estimación:** 1 hora | Prioridad: ?? REQUERIDO

---

### Resumen de archivos — Lista de Precios Promocional

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `scripts/CreatePromotionalPriceListTables.sql` | 070 | Nuevo |
| `Entities\PromotionalPriceList.cs` + `PromotionalPriceListItem.cs` | 071 | Nuevo |
| `Configuration\PromotionalPriceListConfiguration.cs` + `...ItemConfiguration.cs` | 071 | Nuevo |
| `Models\PromotionalPriceList.cs` + `PromotionalPriceListItem.cs` | 071 | Nuevo |
| `AldebaranDbContext.cs` | 071 | Modificar |
| `ApplicationServicesProfile.cs` | 071 | Modificar |
| `IPromotionalPriceListRepository.cs` + implementación | 072 | Nuevo |
| `IPromotionalPriceListService.cs` + implementación | 072 | Nuevo |
| `ArchitectureBuilderExtensions.cs` (Web) | 072 | Modificar |
| `IPriceListFetchService.cs` + implementación (FileWritingService) | 073 | Nuevo |
| `PriceListFetchWorker.cs` (FileWritingService) | 074 | Nuevo |
| `Program.cs` (FileWritingService) | 074 | Modificar |
| `appsettings.json` (FileWritingService) | 074 | Modificar |
| `IPromotionalPriceListImportService.cs` + implementación | 075 | Nuevo |
| `Pages\BonificationPages\PromotionalPriceLists.razor` + `.cs` | 076 | Nuevo |
| Dashboard / `MainLayout.razor` (notificación) | 077 | Modificar |
| `Shared\MainLayout.razor` (menú) | 078 | Modificar |

---

> ? **Con TAREA-070 a TAREA-078 queda cubierto el módulo de Lista de Precios Promocional,**  
> incluyendo descarga automática diaria, contingencia manual, y notificaciones de alerta.  
> Este es el **último insumo requerido** para el cálculo de bonificación por facturación.
---

### 2.2.5 Gestión de Exclusiones – Pedido Especial

> Permite marcar pedidos con bandera `IsSpecialOrder` para excluirlos del Bono por Pedido, aplicable solo a distribuidores, con control por roles, auditoría reforzada e impacto dinámico en reportes y consultas.

---

## 1. Base de Datos

#### TAREA-079 ? ???
**Agregar columna `IsSpecialOrder` en `CUSTOMER_ORDERS`**

**Contexto del código:**  
La entidad de pedido no expone actualmente una marca explícita para exclusión funcional del cálculo de bono.

**Cambios:**
- Agregar `IS_SPECIAL_ORDER BIT NOT NULL DEFAULT 0` en `CUSTOMER_ORDERS`.
- Garantizar valor por defecto para históricos (`0`).

**Archivo a crear/modificar:**
- `scripts/` ? nuevo script de migración (ej: `AddIsSpecialOrderToCustomerOrders.sql`)

---

#### TAREA-080 ? ???
**Crear estructura de auditoría explícita para cambios de `IsSpecialOrder`**

**Contexto del código:**  
Existe auditoría estándar de modificación, pero se requiere log adicional explícito del flag y su dirección de cambio.

**Cambios:**
- Crear tabla dedicada de trazabilidad (ej: `CustomerOrderSpecialOrderFlagLogs`) con: Pedido, usuario, fecha, causa, valor anterior, valor nuevo, dirección (`false->true` / `true->false`).
- Índices por `CustomerOrderId` y fecha para consulta operativa/auditoría.

**Archivo a crear/modificar:**
- `scripts/` ? nuevo script (ej: `CreateCustomerOrderSpecialOrderFlagLogs.sql`)

---

#### TAREA-081 ? ???
**Actualizar SP/consultas de cálculo y reportes para exclusión dinámica**

**Contexto del código:**  
Los reportes y cálculos consumen SPs y consultas SQL que hoy no consideran `IsSpecialOrder`.

**Cambios:**
- Ajustar SPs de: `Customer Orders`, `Customer Orders Activities`, `Customer Sales`.
- Incluir bandera en dataset de salida y filtro opcional.
- Excluir de cálculos de Bono por Pedido cuando `IS_SPECIAL_ORDER = 1`.

**Archivo a crear/modificar:**
- `scripts/Full Database Creation Script.sql` y scripts de SPs asociados

---

## 2. Entidades / Modelo de dominio

#### TAREA-082 ? ??
**Agregar propiedad `IsSpecialOrder` en entidades y modelos de pedido**

**Contexto del código:**  
`CustomerOrder` en DataAccess y Application Services no incluye el nuevo campo.

**Cambios:**
- Agregar `IsSpecialOrder` en:
  - `Aldebaran.DataAccess\Entities\CustomerOrder.cs`
  - `Aldebaran.Application.Services.Models\CustomerOrder.cs`
- Propagar en DTO/ViewModel de reportes impactados.

---

#### TAREA-083 ? ??
**Actualizar mapeos EF/AutoMapper para `IsSpecialOrder`**

**Cambios:**
- Configuración EF de columna (`IS_SPECIAL_ORDER`).
- AutoMapper profiles para incluir la propiedad en ida/vuelta.

**Archivo a crear/modificar:**
- `Aldebaran.DataAccess\Configuration\CustomerOrderConfiguration.cs` (o similar)
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs`

---

## 3. Backend (repositorios / servicios / reglas de negocio)

#### TAREA-084 ? ??
**Implementar operación dedicada para modificar solo `IsSpecialOrder`**

**Contexto del código:**  
Se requiere perfil nuevo que solo modifique esta bandera.

**Cambios:**
- Crear método específico en repositorio/servicio (ej: `UpdateSpecialOrderFlagAsync`).
- Registrar causa obligatoria de modificación (auditoría estándar).
- Persistir log adicional explícito (TAREA-080).

**Archivo a crear/modificar:**
- `ICustomerOrderRepository` / `CustomerOrderRepository`
- `ICustomerOrderService` / `CustomerOrderService`

---

#### TAREA-085 ? ??
**Aplicar reglas de negocio de exclusión y validación de elegibilidad**

**Reglas a implementar:**
- Solo permite marcar `IsSpecialOrder = true` si el cliente del pedido tiene `IsDistributor = true`.
- Bloquear cambio de bandera cuando el pedido pertenece a período cerrado.
- Si cliente no distribuidor: impedir activar bandera y devolver error funcional claro.
- Mantener la exclusión del bono únicamente para Bono por Pedido.

**Archivo a crear/modificar:**
- Servicio de pedidos + componente de validaciones de negocio

---

#### TAREA-086 ? ??
**Blindar actualización general de pedidos para impedir cambio del flag por perfil no autorizado**

**Cambios:**
- En `UpdateAsync` general: bloquear/ignorar cambios de `IsSpecialOrder`.
- Forzar que el cambio de flag pase únicamente por el nuevo flujo especializado.

**Archivo a crear/modificar:**
- `CustomerOrderService`
- `CustomerOrderRepository`

---

#### TAREA-087 ? ??
**Actualizar consultas de cálculo de Bono por Pedido para exclusión efectiva**

**Cambios:**
- Ajustar repositorios/servicios que consolidan pedidos para bono: excluir `IsSpecialOrder = true`.
- Verificar impacto en recalculo dinámico sin reprocesos manuales.

**Archivo a crear/modificar:**
- Repositorios y servicios de cálculo/consulta del bono por pedido

---

## 4. Seguridad y permisos

#### TAREA-088 ? ???
**Crear permiso `Administración de Pedidos Especiales` y política de acceso**

**Cambios:**
- Alta del nuevo rol/permiso.
- Asignación en gestión de roles/usuarios.
- Aplicar a endpoint/página de cambio del flag.
- Confirmar que `Modificación de pedidos` **no** habilita cambio del flag.

**Archivo a crear/modificar:**
- Scripts de roles (`scripts/...`)
- Puntos de autorización en frontend/backend

---

## 5. Frontend Blazor

#### TAREA-089 ? ???
**Agregar visualización y edición controlada de “Pedido Especial” en pantalla de pedido**

**Cambios:**
- Mostrar campo funcional “Pedido Especial”.
- Habilitar edición solo con rol `Administración de Pedidos Especiales`.
- Mostrar mensaje de bloqueo cuando el período esté cerrado.
- Mantener UX consistente con validaciones del backend.

**Archivo a crear/modificar:**
- `Pages\CustomerOrderPages\EditCustomerOrder.razor`
- `Pages\CustomerOrderPages\EditCustomerOrder.razor.cs`

---

#### TAREA-090 ? ???
**Implementar flujo de modificación exclusiva del flag (perfil restringido)**

**Cambios:**
- Crear interacción dedicada (botón/diálogo) para cambio de `IsSpecialOrder`.
- Solicitar causa obligatoria y confirmar impacto (“excluye Bono por Pedido”).
- Consumir método especializado del servicio (TAREA-084).

**Archivo a crear/modificar:**
- Componente de diálogo/página de cambio de flag
- Integración en pantalla de pedido

---

## 6. Reportes afectados (ReportPages)

#### TAREA-091 ? ???
**Actualizar reporte `Customer Orders`**

**Cambios:**
- Incluir columna/indicador `Pedido Especial`.
- Agregar filtro: Todos / Solo Especiales / Solo No Especiales.
- Propagar en exportación.

**Archivo a crear/modificar:**
- `ReportPages\Customer Orders\...` (filtro, viewmodel, page, export)

---

#### TAREA-092 ? ???
**Actualizar reporte `Customer Orders Activities`**

**Cambios:**
- Incluir indicador `Pedido Especial`.
- Filtro equivalente y propagación a consulta.
- Exportación consistente.

**Archivo a crear/modificar:**
- `ReportPages\Customer Order Activities\...`

---

#### TAREA-093 ? ???
**Actualizar reporte `Customer Sales`**

**Cambios:**
- Incluir indicador `Pedido Especial`.
- Filtro equivalente y propagación a consulta.
- Exportación consistente.

**Archivo a crear/modificar:**
- `ReportPages\Customer Sales\...`

---

## 7. Auditoría y trazabilidad

#### TAREA-094 ? ??
**Integrar auditoría estándar + log adicional obligatorio del flag**

**Cambios:**
- Mantener auditoría estándar de modificación con selección de causa.
- Persistir log adicional explícito con: cambio de valor, sentido del cambio, usuario y timestamp.
- Exponer consulta de auditoría para soporte y control.

**Archivo a crear/modificar:**
- Repositorio/servicio de pedido
- Vista o consulta de logs (si aplica)

---

## 8. Pruebas, validación y despliegue

#### TAREA-095 ? ??
**Plan de pruebas técnicas y regresión integral**

**Cobertura mínima:**
- Unitarias: reglas de distribuidor, bloqueo por período cerrado, autorización por rol, exclusión en cálculo.
- Integración: persistencia del flag, SP/reportes con filtro, auditoría doble (estándar + explícita).
- UI/E2E: perfil nuevo solo modifica flag, perfil de modificación actual no puede cambiarlo.
- Regresión: procesos de pedido existentes sin alteración funcional.

**Archivo a crear/modificar:**
- Proyecto(s) de pruebas y plan de validación funcional/técnica

---

### Nota técnica (recomendación no crítica)
**Concurrencia de actualización de flag**: evaluar control optimista (`timestamp/rowversion`) como mejora recomendada para evitar sobrescrituras en escenarios de alta simultaneidad. No bloquea el MVP.

---

### 2.2.6 Integración TOTUS – Extracción de Facturación por SP

> Permite invocar el SP provisto por fábrica externa sobre BD TOTUS local para obtener la facturación consolidada del distribuidor, con resiliencia operativa y trazabilidad.

---

## 1. Infraestructura y configuración

#### TAREA-096 ? ??
**Definir contrato técnico formal del SP `sp_ObtenerFacturacionDistribuidor`**

**Contexto del código:**  
Se requiere documentar y validar la interfaz exacta del SP que proporcionará fábrica externa.

**Cambios:**
- Documentar parámetros de entrada (tipo, formato, obligatoriedad, rangos válidos).
- Documentar respuesta esperada (`ValorTotalFacturadoSinImpuestos`, `TotalNotasCredito`, `TotalFletes`, `TotalDescuentos`, `FechaConsulta`, etc.).
- Definir catálogo de errores técnicos y semántica de retorno (códigos de error específicos, mensajes).
- Establecer SLA de respuesta esperado (<500ms).
- Validar con fábrica externa antes de implementación.

**Archivo a crear/modificar:**
- `docs/` ? nuevo documento (ej: `TOTUS_SP_Contract.md`)

---

#### TAREA-097 ? ??
**Configurar conexión segura a BD TOTUS por ambiente**

**Contexto del código:**  
La invocación al SP requiere parámetros de conexión específicos por ambiente (Dev/QA/Prod).

**Cambios:**
- Incorporar `appsettings.Development.json`, `appsettings.Staging.json`, `appsettings.Production.json` con:
  - `TotusConnectionString` (credenciales en secretos, no en config)
  - `TotusCommandTimeout` (default: 30 seg, máximo 120 seg)
  - `TotusRetryCount` (default: 3)
  - `TotusRetryDelayMs` (default: 1000)
  - `TotusUseFallback` (true/false por ambiente)
- Registrar secretos en Azure Key Vault / gestión de secretos local
- Asegurar manejo sin hardcode mediante `IConfiguration` + `IOptions<TotusOptions>`

---

#### TAREA-098 ? ??
**Crear modelos request/response para consulta de facturación TOTUS**

**Contexto del código:**  
La comunicación con el SP requiere modelos tipados para evitar errores de mapeo.

**Cambios:**
- Crear `TotusInvoicingRequest` con propiedades: `DocumentNumber`, `DocumentType`, `StartDate`, `EndDate`, `IncludeDetailedItems`
- Crear `TotusInvoicingResponse` con propiedades: `TotalInvoicedWithoutTax`, `TotalCreditNotes`, `TotalFreight`, `TotalDiscounts`, `QueryDate`, `InvoiceCount`, `Currency`, `Details`, `Status`, `ErrorMessage`
- Tipado y nullabilidad alineados al contrato del SP (TAREA-096)

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\TotusInvoicingRequest.cs`
- `Aldebaran.Application.Services\Models\TotusInvoicingResponse.cs`
- `Aldebaran.Application.Services\Models\TotusInvoiceDetail.cs` (opcional, si hay detalle)

---

## 2. Capa de acceso a datos

#### TAREA-099 ? ??
**Implementar adaptador DataAccess para invocar el SP de TOTUS**

**Contexto del código:**  
Se requiere un repositorio/adaptador específico para la integración con TOTUS.

**Cambios:**
- Crear `ITotusInvoicingRepository` con método `GetInvoicingAsync(TotusInvoicingRequest, CancellationToken)`
- Crear `TotusInvoicingRepository` con invocación parametrizada del SP
- Mapeo robusto de nulos: `DBNull.Value` → valores por defecto o null
- Conversión segura de tipos numéricos

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\ITotusInvoicingRepository.cs`
- `Aldebaran.DataAccess.Infraestructure\Repository\TotusInvoicingRepository.cs`

---

## 3. Capa de lógica de negocio

#### TAREA-100 ? ??
**Implementar servicio de negocio de facturación TOTUS**

**Contexto del código:**  
Se requiere una capa de servicio que encapsule la lógica de validación y normalización.

**Cambios:**
- Crear `ITotusInvoicingService` con métodos: `GetDistributorInvoicingAsync`, `GetLastKnownGoodResponseAsync`
- Crear `TotusInvoicingService` con validación de entradas y normalización de salida
- Manejo de excepciones específicas y trazabilidad de cada invocación

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\ITotusInvoicingService.cs`
- `Aldebaran.Application.Services\Services\TotusInvoicingService.cs`

---

## 4. Resiliencia operativa

#### TAREA-101 ? ??
**Implementar fallback y reintentos controlados en consulta TOTUS**

**Contexto del código:**  
Una caída de TOTUS no debe romper la consulta de bonificación. Se requiere mecanismo de fallback.

**Cambios:**
- Agregar lógica de reintentos en `TotusInvoicingService` (configurable: default 3 reintentos)
- Cachear último valor conocido válido para fallback
- Retornar respuesta degradada si todos los reintentos fallan
- Configuración de retry delay y command timeout en `appsettings.json`

**Archivos a crear/modificar:**
- `Aldebaran.Application.Services\Services\TotusInvoicingService.cs` (modificar)
- `appsettings.*.json` (crear configuración)

---

## 5. Trazabilidad y auditoría

#### TAREA-102 ? ??
**Implementar auditoría técnica de invocaciones al SP**

**Contexto del código:**  
Se requiere registro de cada invocación para diagnóstico operativo.

**Cambios:**
- Crear tabla `TotusInvoicingAudit` con campos: `DocumentNumber`, `QueryDate`, `RequestParams`, `ResponseStatus`, `ResponseTotal`, `DurationMs`, `ErrorMessage`, `CorrelationId`
- Agregar índices por fecha y documento
- Logging de cada invocación en `TotusInvoicingService`

**Archivos a crear:**
- `scripts/CreateTotusInvoicingAuditTable.sql`
- `Aldebaran.DataAccess\Entities\TotusInvoicingAudit.cs`
- `ITotusInvoicingAuditRepository.cs` + implementación

---

## 6. Alertamiento operativo

#### TAREA-103 ? ??
**Implementar alertamiento administrativo por degradación TOTUS**

**Contexto del código:**  
Se requiere notificación proactiva cuando el sistema cambia a modo fallback.

**Cambios:**
- Crear servicio `ITotusHealthService` para monitoreo de salud de conexión a TOTUS
- Lógica de alertas por: primer fallback del día (email), fallos consecutivos (Slack), latencia fuera de SLA (email)
- Registrar umbrales en `appsettings.json`

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\ITotusHealthService.cs`
- `Aldebaran.Application.Services\Services\TotusHealthService.cs`

---

## 7. Experiencia de usuario

#### TAREA-104 ? ??
**Mostrar banner de facturación desactualizada en CU7**

**Contexto del código:**  
Cuando el distribuidor consulta su bonificación y la facturación viene de fallback, debe saberlo.

**Cambios:**
- En componente de consulta del distribuidor mostrar `RadzenAlert` cuando `Status == "FALLBACK"`
- Banner debe incluir última fecha de consulta exitosa
- Incluir en respuesta del servicio: `LastSuccessfulQueryDate`, `IsUsingFallback`

**Archivos a modificar:**
- `Pages\BonificationPages\DistributorBonificationView.razor` (CU7)

---

## 8. Integración con cálculo de bonificación

#### TAREA-105 ? ??
**Integrar consulta TOTUS al cálculo dinámico de bonificación (CU7)**

**Contexto del código:**  
El motor de cálculo de bonificación (especialmente Bono por Facturación) debe consumir la facturación de TOTUS.

**Cambios:**
- Consumir `TotusInvoicingService.GetDistributorInvoicingAsync` en `BonificationCalculationService`
- Política sin cache: cada consulta obtiene datos actuales de TOTUS
- Usar facturación real o fallback según disponibilidad

**Archivos a modificar:**
- `BonificationCalculationService.cs`

---

## 9. Pruebas técnicas

#### TAREA-106 ? ???
**Crear pruebas unitarias de integración lógica TOTUS**

**Cobertura mínima:**
- Validaciones de entrada: documento vacío, formato inválido, tipos de datos
- Mapeo de salida: nullables, conversiones de tipo, valores por defecto
- Flujo de error: excepciones SQL, timeout, respuesta degradada
- Fallback: último valor conocido disponible vs no disponible
- Auditoría: registro correcto de parámetros y resultado

**Archivos a crear:**
- `Aldebaran.Tests\Services\TotusInvoicingServiceTests.cs`

---

#### TAREA-107 ? ??
**Crear pruebas de integración contra entorno TOTUS de pruebas**

**Contexto del código:**  
Se requiere validación real con el SP antes de producción.

**Cambios:**
- Conectar a BD TOTUS de pruebas (QA)
- Test de invocación real con distribuidor válido/inválido
- Validación de timeout y mapeo de nullables

**Archivos a crear:**
- `Aldebaran.Tests.Integration\TotusInvoicingIntegrationTests.cs`

---

#### TAREA-108 ? ???
**Ejecutar pruebas de rendimiento E2E de consulta TOTUS**

**Contexto del código:**  
Se debe validar que el SLA de <500ms se cumple bajo carga normal.

**Cambios:**
- Test de carga con 10 consultas concurrentes
- Validación de latencia P95, P99
- Registro de métricas de rendimiento

**Archivos a crear:**
- `Aldebaran.Tests.Performance\TotusInvoicingPerformanceTests.cs`

---

## 10. Operación y soporte

#### TAREA-109 ? ???
**Documentar runbook operativo de incidentes TOTUS**

**Contexto del código:**  
Se requiere guía rápida para diagnóstico y escalamiento.

**Cambios:**
- Crear documento `TOTUS_Incident_Runbook.md` con: síntomas, diagnóstico rápido, recuperación, escalamiento
- Checklist de verificación pre-producción

**Archivos a crear:**
- `docs/TOTUS_Incident_Runbook.md`

---

#### TAREA-110 ? ???
**Formalizar criterios de aceptación con PROMOS y fábrica externa**

**Contexto del código:**  
Antes de Go Live, validar con ambas partes que el contrato está cumplido.

**Cambios:**
- Crear documento `TOTUS_Integration_Acceptance_Criteria.md` con criterios de aceptación
- Obtener sign-off de PROMOS, fábrica externa y DBA TOTUS

**Archivos a crear:**
- `docs/TOTUS_Integration_Acceptance_Criteria.md`

---

### Resumen de archivos — Integración TOTUS por SP

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `docs/TOTUS_SP_Contract.md` | 096 | Nuevo |
| `appsettings.*.json` | 097, 101, 103 | Modificar |
| `Models\TotusInvoicingRequest.cs` | 098 | Nuevo |
| `Models\TotusInvoicingResponse.cs` | 098 | Nuevo |
| `Models\TotusInvoiceDetail.cs` | 098 | Nuevo |
| `ITotusInvoicingRepository.cs` | 099 | Nuevo |
| `TotusInvoicingRepository.cs` | 099 | Nuevo |
| `ITotusInvoicingService.cs` | 100 | Nuevo |
| `TotusInvoicingService.cs` | 100, 101, 102 | Nuevo |
| `TotusOptions.cs` | 101 | Nuevo |
| `scripts/CreateTotusInvoicingAuditTable.sql` | 102 | Nuevo |
| `Entities\TotusInvoicingAudit.cs` | 102 | Nuevo |
| `ITotusInvoicingAuditRepository.cs` | 102 | Nuevo |
| `TotusInvoicingAuditRepository.cs` | 102 | Nuevo |
| `ITotusHealthService.cs` | 103 | Nuevo |
| `TotusHealthService.cs` | 103 | Nuevo |
| `Pages\BonificationPages\DistributorBonificationView.razor` | 104 | Modificar |
| `BonificationCalculationService.cs` | 105 | Modificar |
| `TotusInvoicingServiceTests.cs` | 106 | Nuevo |
| `TotusInvoicingIntegrationTests.cs` | 107 | Nuevo |
| `TotusInvoicingPerformanceTests.cs` | 108 | Nuevo |
| `docs/TOTUS_Incident_Runbook.md` | 109 | Nuevo |
| `docs/TOTUS_Integration_Acceptance_Criteria.md` | 110 | Nuevo |
| `ArchitectureBuilderExtensions.cs` | 099, 100, 102, 103 | Modificar |

---

> ? **Con TAREA-096 a TAREA-110 queda cubierto el módulo de Integración TOTUS por SP,**  
> permitiendo consulta robusta de facturación en tiempo real con resiliencia operativa, fallback,  
> auditoría técnica, alertamiento proactivo y validación de SLA. Este es el **insumo crítico final**  
> para el cálculo de Bono por Facturación con datos actualizados del ERP externo.


---

### 2.2.7 Autenticación OTP (MVP Email)

> Ref. propuesta funcional: sección 2.2.2.1 (CU6) + ajustes MVP de seguridad OTP.  
> Alcance MVP: **solo OTP por Email de Bonificación** (SMS diferido a fase posterior).

---

#### TAREA-111 ? ??
**Crear infraestructura de persistencia OTP segura**
 
**Cambios:**
- Crear tabla para OTP con:
  - Documento
  - OTP hash
  - Salt
  - Expiración (TTL 10 min)
  - Intentos fallidos
  - Fecha/hora de uso (invalidación por uso)
  - Estado (Activo/Usado/Expirado/Bloqueado)
- Índices por documento + estado + expiración

**Archivo a crear/modificar:**
- `scripts/CreateOtpAuthenticationTables.sql`

---

#### TAREA-112 ? ??
**Crear modelos/entidades y repositorio OTP**

**Cambios:**
- Crear entidad EF para OTP
- Crear modelo de servicio OTP (POCO)
- Crear `IOtpAuthenticationRepository` + implementación con operaciones:
  - Crear OTP
  - Obtener OTP activo por documento
  - Incrementar intentos
  - Invalidar OTP de forma atómica
  - Verificar bloqueo temporal

**Archivos a crear/modificar:**
- `Aldebaran.DataAccess\Entities\OtpAuthentication.cs`
- `Aldebaran.Application.Services\Models\OtpAuthentication.cs`
- `Aldebaran.DataAccess.Infraestructure\Repository\IOtpAuthenticationRepository.cs`
- `Aldebaran.DataAccess.Infraestructure\Repository\OtpAuthenticationRepository.cs`
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs`

---

#### TAREA-113 ? ??
**Crear servicio de negocio OTP (canal Email MVP)**

**Reglas a implementar:**
- Canal inicial único: Email de Bonificación
- OTP de 6 dígitos
- TTL 10 minutos
- Reenvío con cooldown 60 segundos
- Invalidación atómica al validar correctamente
- No reutilización de OTP
- Mensajes de error neutros

**Archivos a crear/modificar:**
- `Aldebaran.Application.Services\Services\IOtpAuthenticationService.cs`
- `Aldebaran.Application.Services\Services\OtpAuthenticationService.cs`

---

#### TAREA-114 ? ??
**Implementar throttling y bloqueo temporal OTP**

**Reglas a implementar:**
- Throttling por documento/IP: máximo 5 solicitudes / 15 minutos
- Bloqueo temporal: 15 minutos tras 3 fallos de validación
- Cooldown de reenvío: 60 segundos
- Mensajes neutros para evitar enumeración de usuarios

**Archivos a crear/modificar:**
- `OtpAuthenticationService.cs`
- `appsettings.*.json` (parámetros configurables de límites)

---

#### TAREA-115 ? ??
**Integrar envío OTP por Email de Bonificación**

**Cambios:**
- Enviar OTP únicamente a `BonusEmail`
- Validar prerrequisitos RF32/RF33:
  - Cliente tipo distribuidor
  - Email de Bonificación configurado
- Manejo de errores SMTP sin revelar detalle técnico al usuario final

**Archivos a crear/modificar:**
- `Aldebaran.Application.Services\Services\OtpAuthenticationService.cs`
- Servicios de notificación email existentes
- `appsettings.*.json` (plantilla/asunto/remitente)

---

#### TAREA-116 ? ??
**Crear endpoints/API para flujo OTP**

**Cambios:**
- Endpoint solicitar OTP
- Endpoint reenviar OTP
- Endpoint validar OTP y emitir JWT (8 horas)
- Respuestas estandarizadas con mensajes neutros

**Archivos a crear/modificar:**
- Controlador/endpoint de autenticación pública OTP
- DTOs Request/Response de OTP
- Configuración JWT (si aplica ajuste)

---

#### TAREA-117 ? ???
**Crear interfaz Blazor de autenticación OTP**

**Cambios:**
- Pantalla 1: ingreso de documento y solicitud OTP
- Pantalla 2: ingreso de OTP, contador TTL y opción de reenvío (cooldown 60s)
- Mensajes neutros y manejo visual de bloqueo temporal

**Archivos a crear/modificar:**
- Página pública de login OTP (Blazor)
- Componentes de formulario OTP

---

#### TAREA-118 ? ??
**Implementar auditoría mínima obligatoria OTP**

**Eventos auditables:**
- Solicitud OTP (éxito/fallo)
- Reenvío OTP (éxito/fallo)
- Validación OTP (éxito/fallo)
- Motivo de fallo
- IP
- Timestamp

**Archivos a crear/modificar:**
- Tabla/estructura de auditoría OTP (si separada) o repositorio de auditoría existente
- `OtpAuthenticationService.cs`

---

#### TAREA-119 ? ??
**Pruebas técnicas OTP (unitarias + integración básica)**

**Cobertura mínima:**
- TTL 10 min
- Hash + salt
- Invalidación atómica
- Límite 5/15 min por documento/IP
- Bloqueo tras 3 fallos
- Cooldown 60s de reenvío
- Emisión JWT tras validación correcta

**Archivos a crear/modificar:**
- Proyecto de pruebas (`Aldebaran.Tests\...`)

---

#### Resumen de archivos — OTP MVP Email

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `scripts/CreateOtpAuthenticationTables.sql` | 111 | Nuevo |
| `Entities\OtpAuthentication.cs` | 112 | Nuevo |
| `Models\OtpAuthentication.cs` | 112 | Nuevo |
| `IOtpAuthenticationRepository.cs` | 112 | Nuevo |
| `OtpAuthenticationRepository.cs` | 112 | Nuevo |
| `IOtpAuthenticationService.cs` | 113 | Nuevo |
| `OtpAuthenticationService.cs` | 113,114,115,118 | Nuevo |
| `ApplicationServicesProfile.cs` | 112 | Modificar |
| Endpoint/Controller OTP público | 116 | Nuevo |
| DTOs OTP request/response | 116 | Nuevo |
| Página/componentes Blazor OTP | 117 | Nuevo |
| `appsettings.*.json` | 114,115,116 | Modificar |
| Pruebas OTP | 119 | Nuevo |

---

> ? **Con TAREA-111 a TAREA-119 queda cubierto el MVP de autenticación OTP por Email**,  
> incluyendo seguridad mínima obligatoria: throttling, bloqueo temporal, cooldown de reenvío,  
> almacenamiento hash+salt, invalidación atómica y auditoría por intento.

---

## 2.3 Módulo de Consulta - CU7: Consulta de Bonificación (Período Actual)

> **Ref. propuesta funcional:** Casos de Uso CU7 + especificaciones de integración.  
> **Alcance:** Consulta REST sin cache + Página interna Blazor + Descarga PDF.  
> **Autenticación:** JWT Bearer + OTP (Email).  
> **Consumo:** API REST (desde sitio externo) + Página interna Aldebaran.  
> **Sin cache:** Datos actualizados en tiempo real desde TOTUS y BD.

---

### 2.3.1 Modelos & Respuesta de Cálculo

#### **TAREA-120 ⏳ CREAR: Modelo de respuesta de cálculo de bonificación**

**Propósito:** Estructura única que encapsula TODA la información de bonificación para un distribuidor en un período.

**Ubicación:** `Aldebaran.Application.Services\Models\BonificationCalculationResult.cs`

**Responsabilidades:**
- Contiene resultados de los 3 bonos (Facturación, Pedido, Gamificación)
- Nivel de detalle: solo totales (no líneas individuales)
- Timestamps de cálculo y validez
- Indicadores de estado (errores parciales, fallback TOTUS, etc.)

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\BonificationCalculationResult.cs`
- `Aldebaran.Application.Services\Models\PeriodDateRange.cs`
- `Aldebaran.Application.Services\Models\BonificationCalculationDetail.cs`
- `Aldebaran.Application.Services\Models\BonusRangeApplied.cs`
- `Aldebaran.Application.Services\Models\GamificationLevel.cs`

**Modificar:**
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs` → Agregar mappings AutoMapper

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-121 ⏳ CREAR: Modelo DTO para REST API (Request/Response)**

**Propósito:** DTOs públicos que la API REST expone (sin dependencias internas).

**Ubicación:**
- `Aldebaran.Application.Services\DTOs\BonificationConsultationResponse.cs`

**BonificationConsultationResponse:**
```csharp
public class BonificationConsultationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public BonificationCalculationResult Data { get; set; }
    public DateTime ResponseTimestamp { get; set; }
    public string TraceId { get; set; }
}
```

**Archivos a crear:**
- `Aldebaran.Application.Services\DTOs\BonificationConsultationRequest.cs`
- `Aldebaran.Application.Services\DTOs\BonificationConsultationResponse.cs`

**Estimación:** 1.5 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-122 ⏳ CREAR: Enumeraciones y constantes de gamificación**

**Propósito:** Definir niveles, nombres, rangos de gamificación (valores de negocio).

**Ubicación:** `Aldebaran.Domain\Constants\GamificationConstants.cs`

**Archivos a crear:**
- `Aldebaran.Domain\Constants\GamificationConstants.cs`
- `Aldebaran.Domain\Enums\GamificationLevelEnum.cs`

**Contenido:**
- Diccionarios de umbrales por nivel (Bronce $0, Plata $1M, Oro $5M, Platino $10M, Diamante $25M)
- Nombres y colores hexadecimales por nivel

**Estimación:** 1 hora | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-123 ⏳ CREAR: Modelo de solicitud de descarga PDF**

**Ubicación:** `Aldebaran.Application.Services\DTOs\BonificationPdfDownloadRequest.cs`

**Archivos a crear:**
- `Aldebaran.Application.Services\DTOs\BonificationPdfDownloadRequest.cs`
- `Aldebaran.Application.Services\DTOs\BonificationPdfDownloadResponse.cs`

**Estimación:** 1 hora | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-124 ⏳ CREAR: Mappings AutoMapper para modelos**

**Modificar:**
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs`

**Mappings necesarios:**
- `BonificationCalculationResult` → `BonificationConsultationResponse`
- Modelos de gamificación y detalles de bonos

**Estimación:** 1.5 horas | **Prioridad:** 🟡 MEDIA

---

### 2.3.2 Lógica de Cálculo (8 tareas)

#### **TAREA-125 ⏳ CREAR: Interfaz de servicio de cálculo de bonificación**

**Ubicación:** `Aldebaran.Application.Services\Services\IBonificationCalculationService.cs`

**Métodos principales:**
- `CalculateCurrentPeriodAsync(int customerId, string distributorIdentityNumber, CancellationToken ct)`
- `CalculateGamificationAsync(decimal totalAccumulatedAmount, CancellationToken ct)`
- `GetActivePeriodAsync(CancellationToken ct)`

**Responsabilidad:** Contrato que orquesta los 3 servicios de cálculo

**Estimación:** 1 hora | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-126 ⏳ CREAR: Servicio de cálculo de Bono por Facturación**

**Ubicación:** `Aldebaran.Application.Services\Services\BillingBonusCalculationService.cs`

**Responsabilidad:**
1. Obtiene facturación de TOTUS (sin cache, datos en tiempo real)
2. Suma OC Especiales APROBADAS en el período
3. Aplica vigencia activa (rangos % según tramo)
4. Retorna: base total × % aplicable

**Inyecciones:**
- `ITotusInvoicingService`
- `IBonificationSpecialOrderRepository`
- `IBonificationTypeRepository`
- `ILogger<BillingBonusCalculationService>`

**Estimación:** 6 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-127 ⏳ CREAR: Servicio de cálculo de Bono por Pedido**

**Ubicación:** `Aldebaran.Application.Services\Services\OrderBonusCalculationService.cs`

**Responsabilidad:**
1. Suma todas las órdenes del distribuidor (excluyendo `IsSpecialOrder=true`)
2. Aplica descuento global por volumen (DiscountVigency si existe)
3. Suma OC Especiales APROBADAS
4. Aplica vigencia de tipo ORDER
5. Retorna: (base total - descuentos) × % aplicable

**Estimación:** 6 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-128 ⏳ CREAR: Servicio de cálculo de Gamificación**

**Ubicación:** `Aldebaran.Application.Services\Services\GamificationCalculationService.cs`

**Responsabilidad:** Calcula el nivel actual y próximo basado en total acumulado

**Lógica:**
- Determinar nivel actual según umbral
- Calcular próximo nivel
- Calcular progreso en porcentaje
- Monto faltante para siguiente nivel

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-129 ⏳ CREAR: Implementación del servicio de cálculo principal**

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationCalculationService.cs`

**Responsabilidad:** Orquesta los 3 servicios de cálculo y consolida resultado

**Flujo:**
1. Validar distribuidor
2. Obtener período actual
3. Calcular bonos (Facturación + Pedido)
4. Calcular gamificación
5. Construir resultado consolidado
6. Manejo de excepciones con mensajes neutros

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-130 ⏳ CREAR: Extensiones a repositorios existentes**

**Modificar:**
- `ICustomerOrderRepository` → Agregar `GetSumByCustomerInPeriodAsync`
- `IBonificationPeriodRepository` → Agregar `GetActiveInstanceAsync`
- `IDiscountVigencyRepository` → Validar `GetActiveAsync`

**Métodos a agregar:**
- `GetSumByCustomerInPeriodAsync(customerId, startDate, endDate, excludeSpecialOrders, ct)`
- `GetActiveInstanceAsync(ct)`
- `GetActiveAsync(ct)`

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-131 ⏳ CREAR: Servicio de validación de acceso**

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationAccessValidationService.cs`

**Responsabilidad:** Valida que un distribuidor puede consultar su bonificación

**Reglas:**
- Existe en BD
- Es tipo DISTRIBUIDOR
- Está activo
- Tiene email de bonificación configurado

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### 2.3.3 REST API (5 tareas)

#### **TAREA-132 ⏳ CREAR: Controlador REST de consulta de bonificación**

**Ubicación:** `Aldebaran.Web\Controllers\Api\BonificationConsultationController.cs`

**Endpoints:**
- `GET /api/bonification/consult` → Endpoint principal sin parámetros en body

**Autenticación:**
- JWT Bearer (8 horas) + OTP validado

**Respuesta:**
- Siempre 200 OK con formato `BonificationConsultationResponse`
- Mensajes neutros ante errores
- `TraceId` para auditoría

**Estimación:** 3 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-133 ⏳ CREAR: Middleware de autenticación JWT+OTP**

**Ubicación:** `Aldebaran.Web\Middleware\BonificationAuthenticationMiddleware.cs`

**Responsabilidad:**
- Validar JWT Bearer
- Verificar OTP válido en sesión
- Inyectar contexto de distribuidor
- Rechazar con 401 si no válido

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-134 ⏳ CREAR: Endpoint de verificación de salud API**

**Ubicación:** `Aldebaran.Web\Controllers\Api\HealthController.cs`

**Endpoints:**
- `GET /api/health/ready` → Liveness probe
- `GET /api/health/live` → Readiness probe

**Checks:**
- Conexión BD Aldebaran
- Conexión TOTUS (fallback OK)
- Cache Redis (si aplica)

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-135 ⏳ CREAR: Configuración CORS para sitio externo**

**Modificar:**
- `Aldebaran.Web\Program.cs` o `Startup.cs`

**Configuración:**
- Origen permitido: `https://www.catalogospromocionales.com` (configurable por ambiente)
- Métodos: GET
- Headers: `Authorization`, `Content-Type`
- Credentials: false (sitio externo)

**Estimación:** 1 hora | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-136 ⏳ CREAR: Documentación OpenAPI/Swagger**

**Ubicación:** `Aldebaran.Web\SwaggerDocs\BonificationApi.cs`

**Documentación:**
- Descripción de endpoints
- Modelos de request/response
- Códigos de respuesta (200, 401, 500)
- Autenticación JWT+OTP
- Rate limiting (si aplica)

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### 2.3.4 Descarga PDF (3 tareas)

#### **TAREA-137 ⏳ CREAR: Servicio de generación PDF de bonificación**

**Ubicación:** `Aldebaran.Application.Services\Services\IBonificationPdfGenerationService.cs`

**Responsabilidad:**
- Recalcula bonificación (sin cache, datos puntuales)
- Genera documento PDF con:
  - Encabezado: distribuidor, período, fecha emisión
  - Desglose de bonos
  - Gamificación actual
  - Descargo: "Consulta puntual sin garantía de vigencia"
  - Timestamp y firma (opcional para MVP)

**Estimación:** 5 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-138 ⏳ CREAR: Endpoint de descarga PDF**

**Ubicación:** `Aldebaran.Web\Controllers\Api\BonificationPdfController.cs`

**Endpoint:**
- `POST /api/bonification/pdf/download` → Body: `BonificationPdfDownloadRequest`

**Flujo:**
1. Autenticación JWT+OTP
2. Recalcular bonificación
3. Generar PDF
4. Retornar archivo o URL temporal

**Respuesta:**
- Cabecera `Content-Disposition: attachment`
- MIME type: `application/pdf`
- Nombre: `Bonificacion_{Distribuidor}_{Fecha}.pdf`

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-139 ⏳ CREAR: Auditoría de descargas PDF**

**Ubicación:** Tabla de auditoría `BonificationPdfDownloads`

**Campos:**
- Distribuidor
- Fecha descarga
- IP
- User-Agent
- Éxito/Fallo
- Timestamp

**Lógica:**
- Registrar cada descarga exitosa
- Registrar intentos fallidos
- Índices por distribuidor + fecha

**Estimación:** 1 hora | **Prioridad:** 🟡 MEDIA

---

### 2.3.5 Frontend Blazor (5 tareas)

#### **TAREA-140 ⏳ CREAR: Página de consulta interna `BonificationConsultation.razor`**

**Ruta:** `Pages\BonificationPages\BonificationConsultation.razor`  
**URL:** `/bonification/consult` (solo acceso interno autenticado)  
**Rol:** `Consulta de bonificaciones`

**Estructura:**
- Selector de distribuidor (dropdown o búsqueda)
- Botón "Consultar"
- Indicador de carga
- Desglose visual:
  - Total del bono (grande y destacado)
  - Tarjetas por tipo: Facturación, Pedido, Gamificación
  - Tabla de detalles de cálculo
- Botón "Descargar PDF"

**Estimación:** 6 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-141 ⏳ CREAR: Componente de desglose de bonificación**

**Ubicación:** `Pages\BonificationPages\Components\BonificationDetailComponent.razor`

**Responsabilidad:**
- Mostrar bono por facturación (base, %, monto)
- Mostrar bono por pedido (base, descuentos, %, monto)
- Mostrar total consolidado

**Props:**
- `BonificationCalculationDetail` para cada tipo

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-142 ⏳ CREAR: Componente de gamificación visual**

**Ubicación:** `Pages\BonificationPages\Components\GamificationCardComponent.razor`

**Responsabilidad:**
- Mostrar nivel actual (nombre + color + ícono)
- Barra de progreso hacia siguiente nivel
- Texto: "Falta $X para Nivel Y"
- Información de umbrales

**Props:**
- `GamificationLevel`

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-143 ⏳ CREAR: Servicio cliente REST**

**Ubicación:** `Aldebaran.Web\Services\BonificationConsultationService.cs`

**Responsabilidad:**
- Consumir `/api/bonification/consult`
- Manejar JWT + OTP desde sesión
- Manejo de errores con try/catch
- Retry lógico

**Métodos:**
- `GetBonificationAsync(customerId, ct)`
- `DownloadPdfAsync(customerId, ct)`

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-144 ⏳ CREAR: Manejo de errores UI y fallback visual**

**Ubicación:** `Pages\BonificationPages\BonificationConsultation.razor.cs`

**Responsabilidad:**
- Mostrar `RadzenAlert` rojo si error
- Mensajes neutros al usuario
- Sugerir contactar a soporte
- Logging de error (incluye TraceId)

**Estados:**
- Cargando
- Error con fallback (usando último valor conocido si aplica)
- Sin datos
- Datos actuales

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### 2.3.6 Integración y Testing (4 tareas)

#### **TAREA-145 ⏳ CREAR: Pruebas unitarias de cálculo**

**Ubicación:** `Aldebaran.Tests\Services\BonificationCalculationServiceTests.cs`

**Cobertura:**
- Cálculo bono facturación (con/sin OC especiales)
- Cálculo bono pedido (con/sin descuentos)
- Gamificación (progreso entre niveles)
- Manejo de errores TOTUS (fallback)

**Estimación:** 5 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-146 ⏳ CREAR: Pruebas de integración API REST**

**Ubicación:** `Aldebaran.Tests.Integration\BonificationApiTests.cs`

**Cobertura:**
- GET `/api/bonification/consult` con JWT+OTP válido
- Rechazo sin JWT
- Rechazo con OTP inválido
- Respuesta con estructura esperada
- Códigos HTTP correctos

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-147 ⏳ CREAR: Pruebas E2E Blazor**

**Ubicación:** `Aldebaran.Tests.E2E\BonificationConsultationTests.cs`

**Cobertura:**
- Cargar página de consulta
- Seleccionar distribuidor
- Consultar bonificación
- Verificar desglose visible
- Descargar PDF

**Herramienta:** Selenium / Playwright / Cypress

**Estimación:** 6 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-148 ⏳ CREAR: Plan de seguridad y compliance**

**Ubicación:** `docs/CU7_Security_Compliance_Plan.md`

**Cobertura:**
- Validaciones de entrada (inyección SQL, XSS)
- Autorización por rol
- Encriptación de datos en tránsito (HTTPS)
- Auditoría de accesos
- RGPD: logs con PII limitado (documento solo últimos 4 dígitos)
- Rate limiting

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

### 2.3.7 Deployment y Monitoreo (3 tareas)

#### **TAREA-149 ⏳ CREAR: Configuración de variables de ambiente**

**Ubicación:** `appsettings.*.json` y `.env` template

**Variables:**
- `BonificationApiBaseUrl`
- `JwtTokenExpiry` (8 horas)
- `OtpExpiry` (10 min)
- `TotusConnectionString`
- `CorsOrigin` (sitio externo)
- `PdfGenerationTimeout`

**Estimación:** 1 hora | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-150 ⏳ CREAR: Configuración de logging y observabilidad**

**Ubicación:** `Program.cs` / `Startup.cs`

**Implementación:**
- Logging de cada consulta (documento + timestamp)
- Application Insights para monitoreo
- Alertas por latencia > 1s
- Alertas por error rate > 5%

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-151 ⏳ CREAR: Guía de deployment y rollback**

**Ubicación:** `docs/CU7_Deployment_Guide.md`

**Contenido:**
- Prerequisitos (TOTUS accesible, BD actualizada)
- Pasos de deployment (scripts DB, configuración)
- Verificaciones post-deploy (health checks)
- Plan de rollback
- SLA esperado (< 500ms latencia, 99.5% disponibilidad)

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### Resumen de archivos — CU7: Consulta de Bonificación

**Total de TAREAS:** 151 - 119 = **32 TAREAS NUEVAS (TAREA-120 a TAREA-151)**

**Estimación total:** ~120 horas de desarrollo

| Área | Tareas | Estimación |
|------|--------|-----------|
| Modelos & DTOs | 120-124 | 9 h |
| Lógica Cálculo | 125-131 | 24 h |
| REST API | 132-136 | 12 h |
| PDF | 137-139 | 8 h |
| Frontend Blazor | 140-144 | 18 h |
| Testing | 145-148 | 18 h |
| Deploy | 149-151 | 5 h |

---

> ✅ **Con TAREA-120 a TAREA-151 queda completamente cubierto CU7: Consulta de Bonificación (Período Actual)**, incluyendo:
> - Cálculo dinámico sin cache
> - API REST pública + autenticación OTP
> - Página interna Blazor
> - Descarga PDF con recálculo
> - Auditoría y observabilidad
> - Seguridad y compliance
> - Testing completo
> - Deployment y monitoreo

---

## 2.4 Módulo de Notificaciones Automáticas de Bonificación

> Ref. propuesta funcional: sección 2.2.2.2 (CU7 - Notificaciones Automáticas Integradas)  
> Alcance: Notificaciones por Email de Bonificación mediante Rabbit + Notificator Service existente.  
> Aplicación automática de 3 tipos de notificaciones con configuración global y sin preferencias de distribuidor.

---

#### TAREA-152 ? ???
**Crear tabla `BonificationNotificationConfiguration`**

**Descripción:**
Tabla única para almacenar la configuración GLOBAL de notificaciones (no por distribuidor).
Parámetros configurables: umbrales de gamificación, frecuencias de jobs, horarios, activación/desactivación.

**Script SQL a crear:** `scripts/CreateBonificationNotificationConfigurationTable.sql`

```sql
CREATE TABLE dbo.BonificationNotificationConfiguration (
    CONFIG_ID                    INT           NOT NULL PRIMARY KEY DEFAULT 1,  -- una sola fila
    NEAR_LEVEL_THRESHOLD_PERCENT DECIMAL(5,2)  NOT NULL DEFAULT 80,            -- % para alertar cercanía nivel
    NEAR_LEVEL_CHECK_HOUR        INT           NOT NULL DEFAULT 6,             -- hora del día (0-23) para job diario
    NEAR_LEVEL_ENABLED           BIT           NOT NULL DEFAULT 1,
    REMINDER_FREQUENCY           VARCHAR(20)   NOT NULL DEFAULT 'WEEKLY',     -- DAILY | WEEKLY | BIWEEKLY | MONTHLY
    REMINDER_DAY                 VARCHAR(20)   NOT NULL DEFAULT 'MONDAY',     -- día de la semana
    REMINDER_HOUR                INT           NOT NULL DEFAULT 8,             -- hora (0-23)
    REMINDER_ENABLED             BIT           NOT NULL DEFAULT 1,
    NEW_LEVEL_ENABLED            BIT           NOT NULL DEFAULT 1,
    UPDATED_AT                   DATETIME      NOT NULL DEFAULT GETUTCDATE(),
    UPDATED_BY                   INT           NULL,
    CONSTRAINT CK_CONFIG_ID CHECK (CONFIG_ID = 1),
    CONSTRAINT CK_NEAR_LEVEL_THRESHOLD CHECK (NEAR_LEVEL_THRESHOLD_PERCENT BETWEEN 0 AND 100),
    CONSTRAINT CK_NEAR_LEVEL_HOUR CHECK (NEAR_LEVEL_CHECK_HOUR BETWEEN 0 AND 23),
    CONSTRAINT CK_REMINDER_HOUR CHECK (REMINDER_HOUR BETWEEN 0 AND 23),
    CONSTRAINT CK_REMINDER_FREQUENCY CHECK (REMINDER_FREQUENCY IN ('DAILY','WEEKLY','BIWEEKLY','MONTHLY')),
    CONSTRAINT CK_REMINDER_DAY CHECK (REMINDER_DAY IN ('MONDAY','TUESDAY','WEDNESDAY','THURSDAY','FRIDAY','SATURDAY','SUNDAY'))
);
```

---

#### TAREA-153 ? ???
**Crear tabla `BonificationNotificationLog`**

**Descripción:**
Tabla de auditoría para registrar TODAS las notificaciones procesadas.
Campos: distribuidor, tipo de notificación, fecha/hora, estado (encolada/enviada/fallida), motivo de fallo.

**Script SQL a crear:** `scripts/CreateBonificationNotificationLogTable.sql`

```sql
CREATE TABLE dbo.BonificationNotificationLog (
    NOTIFICATION_LOG_ID          INT             NOT NULL IDENTITY(1,1),
    CUSTOMER_ID                  INT             NOT NULL,
    NOTIFICATION_TYPE            VARCHAR(30)     NOT NULL,  -- NEW_LEVEL | NEAR_LEVEL | REMINDER
    PERIOD_INSTANCE_ID           INT             NULL,      -- opcional, depende del tipo
    EMAIL_ADDRESS                VARCHAR(254)    NOT NULL,
    STATUS                       VARCHAR(20)     NOT NULL,  -- ENQUEUED | SENT | FAILED
    QUEUE_TIMESTAMP              DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    SEND_ATTEMPT_TIMESTAMP       DATETIME        NULL,
    ERROR_MESSAGE                VARCHAR(500)    NULL,
    RETRY_COUNT                  INT             NOT NULL DEFAULT 0,
    CORRELATION_ID               VARCHAR(50)     NULL,      -- para tracking en Rabbit
    CONSTRAINT PK_NOTIFICATION_LOG PRIMARY KEY CLUSTERED (NOTIFICATION_LOG_ID),
    CONSTRAINT FK_NOTIFICATION_LOG_CUSTOMER FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID),
    CONSTRAINT CK_NOTIFICATION_TYPE CHECK (NOTIFICATION_TYPE IN ('NEW_LEVEL','NEAR_LEVEL','REMINDER')),
    CONSTRAINT CK_NOTIFICATION_STATUS CHECK (STATUS IN ('ENQUEUED','SENT','FAILED'))
);
CREATE NONCLUSTERED INDEX IX_NOTIFICATION_LOG_CUSTOMER_TYPE
    ON dbo.BonificationNotificationLog (CUSTOMER_ID, NOTIFICATION_TYPE, QUEUE_TIMESTAMP);
CREATE NONCLUSTERED INDEX IX_NOTIFICATION_LOG_STATUS
    ON dbo.BonificationNotificationLog (STATUS, QUEUE_TIMESTAMP);
```

---

#### TAREA-154 ? ??
**Crear entidades EF + Configurations + Models + Mappings**

**Archivos a crear:**

- `Aldebaran.DataAccess\Entities\BonificationNotificationConfiguration.cs`
- `Aldebaran.DataAccess\Entities\BonificationNotificationLog.cs`
- `Aldebaran.DataAccess\Configuration\BonificationNotificationConfigurationConfiguration.cs`
- `Aldebaran.DataAccess\Configuration\BonificationNotificationLogConfiguration.cs`
- `Aldebaran.Application.Services\Models\BonificationNotificationConfiguration.cs`
- `Aldebaran.Application.Services\Models\BonificationNotificationLog.cs`

**Modificar:**
- `Aldebaran.DataAccess\AldebaranDbContext.cs` → agregar DbSets
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs` → agregar mappings

**Estimación:** 4 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-155 ? ??
**Crear repositorio `IBonificationNotificationRepository`**

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IBonificationNotificationRepository.cs`
- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationNotificationRepository.cs`

**Métodos:**
- `GetConfigurationAsync(CancellationToken ct)`
- `UpdateConfigurationAsync(config, CancellationToken ct)`
- `AddLogAsync(log, CancellationToken ct)`
- `UpdateLogStatusAsync(logId, status, errorMessage, CancellationToken ct)`
- `GetLogsAsync(skip, top, customerId, status, CancellationToken ct)`

**Estimación:** 3 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-156 ? ??
**Crear servicio `IBonificationNotificationService` (orquestador principal)**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationNotificationService.cs`
- `Aldebaran.Application.Services\Services\BonificationNotificationService.cs`

**Responsabilidades:**
- Coordina los 3 tipos de notificaciones
- Lee configuración global
- Encola mensajes en Rabbit
- Registra auditoría en log

**Métodos:**
- `CheckAndSendNewLevelNotificationAsync(customerId, bonificationTypeId, CancellationToken ct)`
- `CheckAndSendNearLevelNotificationAsync(customerId, bonificationTypeId, CancellationToken ct)`
- `SendReminderNotificationsAsync(CancellationToken ct)`
- `GetConfigurationAsync(CancellationToken ct)`
- `UpdateConfigurationAsync(config, CancellationToken ct)`
- `GetLogsAsync(skip, top, customerId, status, CancellationToken ct)`

**Estimación:** 4 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-157 ? ??
**Crear servicio de encolamiento en Rabbit `IBonificationMessageQueueService`**

**Descripción:**
Adaptador que toma un modelo de notificación y lo encola en Rabbit para que lo procese Notificator Service.

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationMessageQueueService.cs`
- `Aldebaran.Application.Services\Services\BonificationMessageQueueService.cs`
- `Aldebaran.Application.Services\Models\BonificationNotificationMessage.cs`

**Responsabilidad:**
- Conecta con Rabbit MQ
- Encola mensaje en exchange de notificaciones
- Manejo de errores de conexión

**Estimación:** 5 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-158 ? ??
**Crear componente de composición de email `IBonificationEmailComposer`**

**Descripción:**
Servicio que arma el contenido HTML/texto de cada notificación según su tipo.

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationEmailComposer.cs`
- `Aldebaran.Application.Services\Services\BonificationEmailComposer.cs`

**Métodos:**
- `ComposeNewLevelNotificationAsync(customer, bonType, newBonus, CancellationToken ct)`
- `ComposeNearLevelNotificationAsync(customer, bonType, moneyNeeded, progressPercent, CancellationToken ct)`
- `ComposeReminderNotificationAsync(customer, bonusInfo, CancellationToken ct)`

**Estimación:** 4 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-159 ? ??
**Notificación 1: Detectar y enviar "Alcanzó Nuevo Nivel"**

**Descripción:**
Post-cálculo en CU7: si el distribuidor sube de tramo, enviar notificación.

**Ubicación:** `BonificationNotificationService.cs`

**Lógica:**
1. Obtener rango anterior y actual del distribuidor
2. Verificar si cambió de tramo
3. Aplicar throttling (máx 1 notificación/tipo/distribuidor/24h)
4. Componer email con `IBonificationEmailComposer`
5. Encolar en Rabbit con `IBonificationMessageQueueService`
6. Registrar en log

**Integración:**
- Hook en `BonificationCalculationService` post-cálculo

**Estimación:** 5 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-160 ? ??
**Notificación 2: Job diario "Está Cerca del Siguiente Nivel"**

**Descripción:**
Job diario (configurable, default 6 AM) que verifica distribuidores cercanos a siguiente tramo.

**Archivo a crear:**
- `Aldebaran.Application.Services\Jobs\BonificationNearLevelCheckJob.cs`

**Lógica:**
1. Lee configuración global (umbral %, hora)
2. Para cada distribuidor con período activo:
   - Calcula progreso a siguiente nivel
   - Si ≥ umbral (default 80%) ? evalúa envío
   - Aplica throttling (máx 1 notificación/distribuidor/24h)
   - Encola notificación
3. Registra ejecución en log

**Estimación:** 6 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-161 ? ??
**Notificación 3: Job periódico "Recordatorio de Progreso"**

**Descripción:**
Job periódico (configurable, default Lunes 8 AM) que envía resumen completo de bonificación.

**Archivo a crear:**
- `Aldebaran.Application.Services\Jobs\BonificationReminderJob.cs`

**Lógica:**
1. Lee configuración global (frecuencia, día, hora)
2. Para cada distribuidor activo:
   - Construye resumen de 3 bonos
   - Encola notificación tipo REMINDER
3. Registra ejecución

**Estimación:** 5 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-162 ? ???
**Crear página Admin: Configuración de Notificaciones**

**Ruta:** `Pages\BonificationPages\BonificationNotificationSettings.razor`  
**URL:** `/bonification/notification-settings`  
**Rol:** `Administrador`

**Estructura:**
- Sección "Notificación: Nuevo Nivel"
  - Checkbox: Habilitada/Deshabilitada
- Sección "Notificación: Cerca del Siguiente Nivel"
  - Checkbox: Habilitada/Deshabilitada
  - Slider/Numeric: Umbral % (0-100, default 80)
  - TimePicker: Hora de ejecución del job (default 06:00)
- Sección "Recordatorio Periódico"
  - Checkbox: Habilitada/Deshabilitada
  - Dropdown: Frecuencia (Diaria, Semanal, Bisemanal, Mensual)
  - Dropdown: Día (Lunes, Martes, etc.) — visible si Semanal+
  - TimePicker: Hora de ejecución (default 08:00)
- Botón "Guardar Configuración"
  - Llama `BonificationNotificationService.UpdateConfigurationAsync`

**Estimación:** 7 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-163 ? ???
**Crear página Admin: Historial de Notificaciones Enviadas**

**Ruta:** `Pages\BonificationPages\BonificationNotificationHistory.razor`  
**URL:** `/bonification/notification-history`  
**Rol:** `Administrador`

**Estructura:**
- Filtros superiores:
  - Dropdown Distribuidor
  - Dropdown Tipo (NEW_LEVEL | NEAR_LEVEL | REMINDER | Todos)
  - Dropdown Estado (ENQUEUED | SENT | FAILED | Todos)
  - Date Range
  - Botón "Filtrar"
- `RadzenDataGrid` paginada:
  - Columnas: ID | Distribuidor | Tipo | Email | Estado | Encolada | Enviada | Error | Acciones
  - Row expand: detalles completos + mensajes de error

**Estimación:** 8 horas | **Prioridad:** 🔴 REQUERIDO

---

#### TAREA-164 ? ???
**Crear página Admin: Dashboard de Notificaciones**

**Ruta:** `Pages\BonificationPages\BonificationNotificationDashboard.razor`  
**URL:** `/bonification/notification-dashboard`  
**Rol:** `Administrador`

**Estructura:**
- KPIs destacados (últimas 24h):
  - Total encoladas | Total enviadas | Total fallidas | Tasa de éxito %
- Gráfico de líneas: Notificaciones por hora
- Tabla resumen por tipo (últimas 24h):
  - NEW_LEVEL: X encoladas, Y enviadas, Z fallidas
  - NEAR_LEVEL: X encoladas, Y enviadas, Z fallidas
  - REMINDER: X encoladas, Y enviadas, Z fallidas
- Alertas:
  - Si tasa de fallo > 5%? mostrar alerta roja
  - Si ningún distribuidor recibió recordatorio en últimos 7 días ? alerta amarilla

**Estimación:** 10 horas | **Prioridad:** 🟡 SUGERIDO

---

#### TAREA-165 ? ??
**Crear health check para notificaciones**

**Descripción:**
Endpoint que verifica si el servicio de notificaciones está operativo.

**Archivo a crear/modificar:**
- `Aldebaran.Web\Controllers\Api\HealthController.cs` → agregar check

**Verificaciones:**
- Conexión a Rabbit
- Configuración cargada
- Retorna estado healthy/degraded

**Estimación:** 2 horas | **Prioridad:** 🟡 SUGERIDO

---

#### TAREA-166 ? ??
**Crear logging y observabilidad para notificaciones**

**Descripción:**
Integrar Application SEQ (Free) + logging para seguimiento de todas las notificaciones.

**Cambios:**
- Agregar logging en `BonificationNotificationService`
- Configurar Application SEQ en `Program.cs`
- Configurar niveles de log en `appsettings.json`

**Métricas:**
- Eventos: NotificationEnqueued, NotificationSent, NotificationFailed
- Propiedades: CustomerId, NotificationType, Email, ErrorMessage, RetryCount

**Estimación:** 3 horas | **Prioridad:** 🟡 SUGERIDO

---

#### TAREA-167 ? ??
**Crear pruebas unitarias de notificaciones**

**Ubicación:** `Aldebaran.Tests\Services\BonificationNotificationServiceTests.cs`

**Cobertura:**
- Detección de nuevo nivel
- Cálculo de cercanía a nivel
- Throttling: máx 1/24h
- Encolamiento en Rabbit
- Composición de emails
- Logging correcto

**Estimación:** 6 horas | **Prioridad:** 🟡 SUGERIDO

---

### Resumen de archivos — Notificaciones Automáticas

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `scripts/CreateBonificationNotificationConfigurationTable.sql` | 152 | Nuevo |
| `scripts/CreateBonificationNotificationLogTable.sql` | 153 | Nuevo |
| `Entities\BonificationNotificationConfiguration.cs` | 154 | Nuevo |
| `Entities\BonificationNotificationLog.cs` | 154 | Nuevo |
| `Configuration\BonificationNotificationConfigurationConfiguration.cs` | 154 | Nuevo |
| `Configuration\BonificationNotificationLogConfiguration.cs` | 154 | Nuevo |
| `Models\BonificationNotificationConfiguration.cs` | 154 | Nuevo |
| `Models\BonificationNotificationLog.cs` | 154 | Nuevo |
| `Models\BonificationNotificationMessage.cs` | 157 | Nuevo |
| `IBonificationNotificationRepository.cs` | 155 | Nuevo |
| `BonificationNotificationRepository.cs` | 155 | Nuevo |
| `IBonificationNotificationService.cs` | 156 | Nuevo |
| `BonificationNotificationService.cs` | 156 | Nuevo |
| `IBonificationMessageQueueService.cs` | 157 | Nuevo |
| `BonificationMessageQueueService.cs` | 157 | Nuevo |
| `IBonificationEmailComposer.cs` | 158 | Nuevo |
| `BonificationEmailComposer.cs` | 158 | Nuevo |
| `BonificationNearLevelCheckJob.cs` | 160 | Nuevo |
| `BonificationReminderJob.cs` | 161 | Nuevo |
| `Pages\BonificationPages\BonificationNotificationSettings.razor` | 162 | Nuevo |
| `Pages\BonificationPages\BonificationNotificationSettings.razor.cs` | 162 | Nuevo |
| `Pages\BonificationPages\BonificationNotificationHistory.razor` | 163 | Nuevo |
| `Pages\BonificationPages\BonificationNotificationHistory.razor.cs` | 163 | Nuevo |
| `Pages\BonificationPages\BonificationNotificationDashboard.razor` | 164 | Nuevo |
| `Pages\BonificationPages\BonificationNotificationDashboard.razor.cs` | 164 | Nuevo |
| `Controllers\Api\HealthController.cs` | 165 | Modificar |
| `Program.cs` | 166 | Modificar |
| `appsettings.json` | 166 | Modificar |
| `BonificationNotificationServiceTests.cs` | 167 | Nuevo |
| `AldebaranDbContext.cs` | 154 | Modificar |
| `ApplicationServicesProfile.cs` | 154 | Modificar |
| `ArchitectureBuilderExtensions.cs` | 156, 157, 158, 160, 161 | Modificar |

---

> ✅ **Con TAREA-152 a TAREA-167 queda completamente cubierto el módulo de Notificaciones Automáticas**,  
> incluyendo:
> - 3 tipos de notificaciones (Nuevo Nivel, Cerca del Nivel, Recordatorio)
> - Configuración global por Admin (umbrales, horarios, frecuencias)
> - Encolamiento en Rabbit + Notificator Service existente
> - Auditoría completa de envíos
> - Dashboard y reportería
> - Seguridad y compliance
> - Health checks y observabilidad
> - Testing 

---

## 2.5 Módulo de Consulta - CU8: Consulta de Histórico de Bonos (Períodos Anteriores)

> **Ref. propuesta funcional:** Casos de Uso CU8 + especificaciones de integración con MES + AÑO.  
> **Alcance:** Consulta REST de histórico + Página Blazor interna + Exportación PDF/Excel.  
> **Autenticación:** JWT Bearer + OTP (Email).  
> **Datos:** Bonos cerrados e inmutables, agrupados por MES/AÑO del calendario.  
> **Filtrado:** Por Tipo de Bono (selector) para claridad en múltiples periodicidades.

---

### 2.5.1 Modelos & Estructuras de Datos (6 tareas)

#### **TAREA-168 ⏳ CREAR: Modelos de respuesta para histórico mes/año**

**Propósito:** Estructura jerárquica que encapsula el histórico consolidado por mes/año + desglose por tipo de bono.

**Ubicación:** `Aldebaran.Application.Services\Models\`

**Archivos a crear:**
- `BonificationHistoryMonthResponse.cs` — Respuesta consolidada del mes
- `BonificationTypeGroupDetail.cs` — Desglose por tipo de bono
- `ClosedInstanceDetail.cs` — Detalle de cada instancia cerrada
- `BonificationMonthAvailableDto.cs` — Dropdown de meses disponibles

**Estimación:** 3 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-169 ⏳ CREAR: Especificación en repositorio para histórico**

**Propósito:** Agregar métodos al repositorio para consultar bonos cerrados por mes/año.

**Archivos a modificar:**
- `IBonificationClosedPeriodRepository.cs` — Agregar:
  ```csharp
  Task<List<BonificationClosedPeriod>> GetClosedByMonthAndYearAsync(
      int customerId, int year, int month, CancellationToken ct);
  Task<List<(int Year, int Month)>> GetAvailableHistoryMonthsAsync(
      int customerId, CancellationToken ct);
  ```
- `BonificationClosedPeriodRepository.cs` — Implementar con:
  - Query por `DATEPART(YEAR, ClosedAt)` y `DATEPART(MONTH, ClosedAt)`
  - JOIN a `BonificationType` + `BonificationPeriod`
  - Ordenamiento DESC por fecha

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-170 ⏳ CREAR: Servicio de consulta del histórico**

**Propósito:** Lógica de negocio que obtiene histórico, agrupa por tipo y calcula subtotales.

**Ubicación:** `Aldebaran.Application.Services\Services\`

**Archivos a crear/modificar:**
- `IBonificationHistoryService.cs` — Interfaz con:
  ```csharp
  Task<BonificationHistoryMonthResponse> GetHistoryByMonthAsync(
      int customerId, int year, int month, CancellationToken ct);
  Task<List<BonificationMonthAvailableDto>> GetAvailableHistoryMonthsAsync(
      int customerId, CancellationToken ct);
  ```
- `BonificationHistoryService.cs` — Implementar:
  - Validar CustomerId es distribuidor
  - Consultar bonos cerrados
  - Agrupar por `BonificationType`
  - Calcular subtotales + totales
  - Mapear a DTOs

**Estimación:** 5 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-171 ⏳ CREAR: DTOs para REST API (histórico)**

**Propósito:** DTOs públicos expuestos por la API REST.

**Ubicación:** `Aldebaran.Application.Services\DTOs\`

**Archivos a crear:**
- `BonificationHistoryRequest.cs` — Request con `year` y `month`
- `BonificationHistoryResponse.cs` — Wrapper de respuesta

**Estimación:** 1.5 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-172 ⏳ CREAR: Modelos para exportación (PDF/Excel)**

**Propósito:** Estructuras para preparar datos de exportación.

**Archivos a crear:**
- `BonificationHistoryExportModel.cs` — Para PDF
- `BonificationHistoryExcelModel.cs` — Para Excel

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-173 ⏳ CREAR: Mappings AutoMapper para histórico**

**Modificar:**
- `Aldebaran.Application.Services\Mappings\ApplicationServicesProfile.cs`

**Mappings:**
- `BonificationClosedPeriod` → `ClosedInstanceDetail`
- Consolidación en `BonificationTypeGroupDetail`
- Agregación final en `BonificationHistoryMonthResponse`

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-174 ⏳ CREAR: Controlador REST de histórico**

**Ubicación:** `Aldebaran.Web\Controllers\Api\BonificationHistoryController.cs`

**Endpoints:**
- `GET /api/bonification/history/{year}/{month}` — Consultar histórico mes específico
- `GET /api/bonification/history/available-months` — Obtener dropdown de meses

**Autenticación:**
- JWT Bearer (8 horas) + OTP validado

**Validaciones:**
- Año entre (actual - 24) y actual
- Mes entre 1 y 12
- CustomerId válido

**Respuestas HTTP:**
- 200 OK: Histórico encontrado (puede estar vacío)
- 400 Bad Request: Parámetros inválidos
- 401 Unauthorized: No autenticado
- 500 Internal Server Error

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-175 ⏳ CREAR: Endpoint de descarga PDF histórico**

**Ubicación:** Agregar a `BonificationHistoryController.cs`

**Endpoint:**
- `POST /api/bonification/history/pdf/download` — Body: `BonificationHistoryPdfRequest`

**Flujo:**
1. Validar autenticación
2. Consultar histórico
3. Generar PDF con consolidado + desglose
4. Retornar archivo

**Respuesta:**
- Content-Disposition: attachment
- Nombre: `Historico_Bonificacion_{Distribuidor}_{Mes}_{Año}.pdf`

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-176 ⏳ CREAR: Endpoint de descarga Excel histórico**

**Ubicación:** Agregar a `BonificationHistoryController.cs`

**Endpoint:**
- `POST /api/bonification/history/excel/download` — Body: `BonificationHistoryExcelRequest`

**Contenido:**
- Hoja 1: Resumen del mes (totales consolidados)
- Hoja 2: Desglose por tipo de bono
- Hoja 3: Detalle por instancia

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-177 ⏳ CREAR: Auditoría de consultas históricas**

**Propósito:** Registrar cada consulta de histórico realizada.

**Tabla:** `BonificationHistoryAuditLog`
- Campos: Distribuidor, Mes/Año consultado, Fecha/hora, IP, Éxito/Fallo

**Ubicación:**
- `Aldebaran.Application.Services\Services\BonificationHistoryService.cs` — Agregar logging

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-178 ⏳ CREAR: Rate limiting para histórico**

**Propósito:** Controlar uso abusivo de consultas.

**Configuración:**
- Máximo 10 consultas/minuto por distribuidor
- Máximo 5 descargas/hora por distribuidor

**Ubicación:**
- `Aldebaran.Web\Middleware\RateLimitingMiddleware.cs` (si no existe, crear)
- Registrar en `Program.cs`

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-179 ⏳ CREAR: Página principal `BonificationHistory.razor`**

**Ruta:** `Pages\BonificationPages\BonificationHistory.razor`  
**URL:** `/bonification/history` (acceso interno)  
**Rol:** `Consulta de bonificaciones`

**Estructura:**
- Selectores: Mes (dropdown) | Año (dropdown) | Botón "Consultar"
- Indicador de carga
- Resumen consolidado del mes (4 tarjetas: Total, NC Aplicadas, Pendientes, Rechazadas)
- Sección expandible por tipo de bono:
  - Subtotal por tipo
  - Tabla de instancias (Código, Fechas, Bono, Estado NC)
- Botones: Descargar PDF | Exportar Excel

**Estimación:** 8 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-180 ⏳ CREAR: Code-behind `BonificationHistory.razor.cs`**

**Responsabilidad:**
- Propiedades: `selectedMonth`, `selectedYear`, `availableMonths`, `historyData`, `isLoading`
- Métodos: `OnInitializedAsync()`, `OnConsult()`, `GetBonusTypeSummary()`
- Métodos: `OnDownloadPdf()`, `OnExportExcel()`

**Lógica:**
- Cargar meses disponibles en init
- Validar año/mes antes de consultar
- Manejar errores con mensajes amigables
- Aplicar defaults (mes anterior, año actual)

**Estimación:** 5 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-181 ⏳ CREAR: Componente de resumen mensual**

**Ubicación:** `Pages\BonificationPages\Components\BonificationHistoryMonthlySummaryComponent.razor`

**Responsabilidad:**
- Mostrar 4 tarjetas de totales
- Colores visuales: Verde (Aplicadas), Naranja (Pendientes), Rojo (Rechazadas), Azul (Total)

**Props:**
- `BonificationHistoryMonthResponse`

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-182 ⏳ CREAR: Componente de desglose por tipo**

**Ubicación:** `Pages\BonificationPages\Components\BonificationHistoryTypeBreakdownComponent.razor`

**Responsabilidad:**
- Mostrar `RadzenExpander` por cada tipo de bono
- Tabla de instancias dentro del expander
- Indicadores visuales de estado NC

**Props:**
- `BonificationTypeGroupDetail`

**Estimación:** 5 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-183 ⏳ CREAR: Servicio cliente para histórico**

**Ubicación:** `Aldebaran.Web\Services\BonificationHistoryService.cs`

**Métodos:**
- `GetHistoryByMonthAsync(year, month, ct)`
- `GetAvailableMonthsAsync(ct)`
- `DownloadPdfAsync(year, month, ct)`
- `DownloadExcelAsync(year, month, ct)`

**Características:**
- Consumir API REST
- Manejar JWT + OTP
- Retry lógico
- Error handling

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-184 ⏳ CREAR: Manejo de errores UI en histórico**

**Ubicación:** `Pages\BonificationPages\BonificationHistory.razor.cs`

**Estados:**
- Cargando (spinner)
- Error de consulta (alert rojo)
- Sin datos (info)
- Datos exitosos

**Mensajes:**
- "No hay datos para ese mes"
- "Error al consultar. Intente nuevamente"
- "La descarga tardará unos segundos..."

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-185 ⏳ CREAR: Servicio de generación PDF histórico**

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationHistoryPdfService.cs`

**Responsabilidad:**
- Recibe `BonificationHistoryMonthResponse`
- Genera documento PDF con:
  - Encabezado: Logo, Distribuidor, Mes/Año
  - Resumen consolidado (4 totales)
  - Desglose por tipo (tabla expandida)
  - Detalle por instancia
  - Footer con timestamp

**Estimación:** 6 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-186 ⏳ CREAR: Servicio de exportación Excel histórico**

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationHistoryExcelService.cs`

**Hojas:**
1. "Resumen" — Totales consolidados
2. "Por Tipo" — Desglose por tipo de bono
3. "Detalle Completo" — Todas las instancias

**Estimación:** 5 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-187 ⏳ CREAR: Auditoría de exportaciones**

**Tabla:** `BonificationHistoryExportLog`
- Campos: Distribuidor, Mes/Año, Formato (PDF/Excel), Fecha/hora, IP

**Ubicación:**
- Agregar logging en servicios de PDF/Excel

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### Resumen de archivos — CU8: Histórico de Bonos (COMPLETO: Distribuidores + PROMOS)

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `Models\BonificationHistoryMonthResponse.cs` | 168 | Nuevo |
| `Models\BonificationTypeGroupDetail.cs` | 168 | Nuevo |
| `Models\ClosedInstanceDetail.cs` | 168 | Nuevo |
| `Models\BonificationMonthAvailableDto.cs` | 168 | Nuevo |
| `IBonificationClosedPeriodRepository.cs` | 169 | Modificar |
| `BonificationClosedPeriodRepository.cs` | 169 | Modificar |
| `IBonificationHistoryService.cs` | 170 | Nuevo |
| `BonificationHistoryService.cs` | 170 | Nuevo |
| `DTOs\BonificationHistoryRequest.cs` | 171 | Nuevo |
| `DTOs\BonificationHistoryResponse.cs` | 171 | Nuevo |
| `Models\BonificationHistoryExportModel.cs` | 172 | Nuevo |
| `Models\BonificationHistoryExcelModel.cs` | 172 | Nuevo |
| `ApplicationServicesProfile.cs` | 173 | Modificar |
| `Controllers\Api\BonificationHistoryController.cs` | 174, 175, 176 | Nuevo |
| `Services\BonificationHistoryAuditLog.sql` | 177 | Nuevo script |
| `Middleware\RateLimitingMiddleware.cs` | 178 | Nuevo |
| `Pages\BonificationPages\BonificationHistory.razor` | 179 | Nuevo |
| `Pages\BonificationPages\BonificationHistory.razor.cs` | 180 | Nuevo |
| `Components\BonificationHistoryMonthlySummaryComponent.razor` | 181 | Nuevo |
| `Components\BonificationHistoryTypeBreakdownComponent.razor` | 182 | Nuevo |
| `Services\BonificationHistoryService.cs` (cliente) | 183 | Nuevo |
| `BonificationHistoryPdfService.cs` | 185 | Nuevo |
| `BonificationHistoryExcelService.cs` | 186 | Nuevo |
| `BonificationHistoryExportLog.sql` | 187 | Nuevo script |
| `ArchitectureBuilderExtensions.cs` | 170, 183 | Modificar |

---

> ✅ **Con TAREA-168 a TAREA-187 queda completamente cubierto CU8: Consulta de Histórico de Bonos (Distribuidores)**,  
> incluyendo:
> - Agrupación por mes/año con desglose por tipo de bono
> - API REST sin cache con datos puntuales
> - Página Blazor interna para visualización
> - Exportación PDF/Excel con análisis
> - Auditoría y rate limiting
> - Dashboard de KPIs

---

## 2.6 Módulo de Cierre Automático de Períodos (CU10)

> Ref. propuesta funcional: Cierre Automático de Instancia de Período  
> Alcance: Job nocturno + cálculo final + congelación + recomendación NC  
> Responsable: Sistema (automático) + Admin PROMOS (notificación)

---

### 2.6.1 Persistencia de Bonos Cerrados (FOTO Congelada)

#### TAREA-188 ? ???
**Crear tabla `BonificationClosedPeriods`**

**Descripción:**
Tabla que almacena la FOTO congelada de bonos finales cuando se cierra un período.
Es el registro inmutable de cada cierre de instancia de período.

**Script SQL a crear:** `scripts/CreateBonificationClosedPeriodsTable.sql`

```sql
CREATE TABLE dbo.BonificationClosedPeriods (
    BONIFICATION_CLOSED_PERIOD_ID    INT             NOT NULL IDENTITY(1,1),
    BONIFICATION_PERIOD_INSTANCE_ID  INT             NOT NULL,  -- FK a instancia que se cierra
    CUSTOMER_ID                      INT             NOT NULL,  -- distribuidor del bono
    TOTAL_BILLING_BONUS              DECIMAL(18,2)   NOT NULL,  -- bono facturación calculado
    TOTAL_ORDER_BONUS                DECIMAL(18,2)   NOT NULL,  -- bono pedido calculado
    TOTAL_BONUS                      DECIMAL(18,2)   NOT NULL,  -- suma de ambos
    CLOSED_AT                        DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    CLOSED_BY                        INT             NULL,      -- usuario si cierre manual
    NOTES                            VARCHAR(500)    NULL,
    CONSTRAINT PK_BONIFICATION_CLOSED_PERIOD PRIMARY KEY CLUSTERED (BONIFICATION_CLOSED_PERIOD_ID),
    CONSTRAINT UQ_BONIFICATION_CLOSED_PERIOD UNIQUE (BONIFICATION_PERIOD_INSTANCE_ID, CUSTOMER_ID),
    CONSTRAINT FK_CLOSED_PERIOD_INSTANCE FOREIGN KEY (BONIFICATION_PERIOD_INSTANCE_ID)
        REFERENCES dbo.BonificationPeriodInstances (BONIFICATION_PERIOD_INSTANCE_ID),
    CONSTRAINT FK_CLOSED_PERIOD_CUSTOMER FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID),
    CONSTRAINT CK_CLOSED_PERIOD_BONUS CHECK (TOTAL_BONUS = TOTAL_BILLING_BONUS + TOTAL_ORDER_BONUS)
);
CREATE NONCLUSTERED INDEX IX_CLOSED_PERIOD_INSTANCE
    ON dbo.BonificationClosedPeriods (BONIFICATION_PERIOD_INSTANCE_ID);
CREATE NONCLUSTERED INDEX IX_CLOSED_PERIOD_CUSTOMER
    ON dbo.BonificationClosedPeriods (CUSTOMER_ID, CLOSED_AT);
```

---

#### TAREA-189 ? ??
**Crear entidad EF: `BonificationClosedPeriod`**

**Archivo a crear:**
- `Aldebaran.DataAccess\Entities\BonificationClosedPeriod.cs`
```csharp
public class BonificationClosedPeriod
{
    public int BonificationClosedPeriodId { get; set; }
    public int BonificationPeriodInstanceId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalBillingBonus { get; set; }
    public decimal TotalOrderBonus { get; set; }
    public decimal TotalBonus { get; set; }
    public DateTime ClosedAt { get; set; }
    public int? ClosedBy { get; set; }
    public string Notes { get; set; }

    // Navegación
    public BonificationPeriodInstance PeriodInstance { get; set; }
    public Customer Customer { get; set; }
}
```

- `Aldebaran.DataAccess\Configuration\BonificationClosedPeriodConfiguration.cs` — mapeo EF completo

**Modificar:**
- `Aldebaran.DataAccess\AldebaranDbContext.cs` ? agregar:
```csharp
public DbSet<BonificationClosedPeriod> BonificationClosedPeriods { get; set; }
```

---

#### TAREA-190 ? ??
**Crear modelo de servicio: `BonificationClosedPeriod`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Models\BonificationClosedPeriod.cs` — POCO puro

**Agregar mappings en `ApplicationServicesProfile.cs`:**
```csharp
CreateMap<BonificationClosedPeriod, Entities.BonificationClosedPeriod>().ReverseMap();
```

---

#### TAREA-191 ? ??
**Crear `IBonificationClosedPeriodRepository` y `BonificationClosedPeriodRepository`**

**Archivos a crear:**
- `Aldebaran.DataAccess.Infraestructure\Repository\IBonificationClosedPeriodRepository.cs`
```csharp
public interface IBonificationClosedPeriodRepository
{
    Task AddAsync(BonificationClosedPeriod closedPeriod, CancellationToken ct = default);
    Task<BonificationClosedPeriod?> FindAsync(int id, CancellationToken ct = default);
    Task<(IEnumerable<BonificationClosedPeriod>, int)> GetByInstanceAsync(
        int periodInstanceId, int? skip, int? top, CancellationToken ct = default);
    Task<(IEnumerable<BonificationClosedPeriod>, int)> GetByCustomerAsync(
        int customerId, int? skip, int? top, CancellationToken ct = default);
    Task<decimal> GetTotalBonusByCustomerAndPeriodAsync(
        int customerId, int periodInstanceId, CancellationToken ct = default);
}
```

- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationClosedPeriodRepository.cs` — implementación

---

#### TAREA-192 ? ??
**Crear `IBonificationClosureService` y `BonificationClosureService`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationClosureService.cs`
```csharp
public interface IBonificationClosureService
{
    Task ClosePeriodAsync(int periodInstanceId, CancellationToken ct = default);
    Task<BonificationClosedPeriod?> GetClosureAsync(int closureId, CancellationToken ct = default);
    Task<List<BonificationClosedPeriod>> GetPeriodClosuresAsync(
        int periodInstanceId, CancellationToken ct = default);
}
```

- `Aldebaran.Application.Services\Services\BonificationClosureService.cs` — implementación
  - Calcula bonos finales para todos los distribuidores del período
  - Registra FOTO congelada en `BonificationClosedPeriods`
  - Manejo de excepciones sin interrumpir el cierre

---

#### TAREA-193 ? ??
**Crear `IBonificationRecommendationService` para NC recomendadas**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationRecommendationService.cs`
```csharp
public interface IBonificationRecommendationService
{
    Task<List<CreditNoteRecommendation>> GenerateRecommendationsAsync(
        int periodInstanceId, CancellationToken ct = default);
}
```

- `Aldebaran.Application.Services\Models\CreditNoteRecommendation.cs`
- `Aldebaran.Application.Services\Services\BonificationRecommendationService.cs` — implementación
  - Detecta NC faltantes basándose en patrón histórico
  - Genera lista de recomendaciones para revisión manual por PROMOS

---

#### TAREA-194 ? ???
**Crear job nocturno `BonificationPeriodClosureJob`**

**Archivo a crear:**
- `Aldebaran.Application.Services\Jobs\BonificationPeriodClosureJob.cs`

**Responsabilidad:**
- Ejecuta diariamente a las 23:59
- Busca instancias `IN_PROGRESS` con `EndDate = TODAY`
- Para cada instancia:
  1. Calcula bonos finales para todos los distribuidores
  2. Registra FOTO congelada en `BonificationClosedPeriods`
  3. Marca instancia como `CLOSED`
  4. Genera recomendaciones de NC
  5. Crea siguiente instancia si el TipoBono tiene vigencia activa
  6. Envía notificación a PROMOS con resumen del cierre

---

#### TAREA-195 ? ???
**Crear tabla de auditoría `BonificationClosureLogs`**

**Script SQL a crear:** `scripts/CreateBonificationClosureLogsTable.sql`

```sql
CREATE TABLE dbo.BonificationClosureLogs (
    CLOSURE_LOG_ID                   INT             NOT NULL IDENTITY(1,1),
    BONIFICATION_PERIOD_INSTANCE_ID  INT             NOT NULL,
    CLOSURE_START_TIME               DATETIME        NOT NULL,
    CLOSURE_END_TIME                 DATETIME        NOT NULL,
    TOTAL_DISTRIBUTORS_PROCESSED     INT             NOT NULL,
    TOTAL_BONUSES_CALCULATED         DECIMAL(18,2)   NOT NULL,
    TOTAL_NC_RECOMMENDED             INT             NOT NULL,
    STATUS                           VARCHAR(20)     NOT NULL,  -- SUCCESS | PARTIAL | FAILED
    ERROR_MESSAGE                    VARCHAR(500)    NULL,
    NEXT_INSTANCE_CREATED            BIT             NOT NULL,
    CONSTRAINT PK_CLOSURE_LOG PRIMARY KEY CLUSTERED (CLOSURE_LOG_ID),
    CONSTRAINT FK_CLOSURE_LOG_INSTANCE FOREIGN KEY (BONIFICATION_PERIOD_INSTANCE_ID)
        REFERENCES dbo.BonificationPeriodInstances (BONIFICATION_PERIOD_INSTANCE_ID)
);
```

---

#### TAREA-196 ? ??
**Crear página Admin: Vista de Cierre de Períodos**

**Ruta:** `Pages\BonificationPages\BonificationClosures.razor`  
**URL:** `/bonification/closures`  
**Rol requerido:** `Administrador`

**Estructura:**
- Título: "Cierre de Períodos de Bonificación"
- Filtros: Instancia | Estado | Date Range | Botón "Filtrar"
- `RadzenDataGrid` con:
  - Columnas: Instancia | Período | Fecha Cierre | Distribuidores | Bono Total | Estado | Acciones
- Botón "Ver Detalle" → abre panel lateral con:
  - Resumen del cierre
  - Lista de distribuidores procesados
  - Recomendaciones NC generadas
  - Log de ejecución

---

#### TAREA-197 ? ??
**Crear página Admin: Dashboard de Cierre**

**Ruta:** `Pages\BonificationPages\BonificationClosureDashboard.razor`  
**URL:** `/bonification/closure-dashboard`  
**Rol requerido:** `Administrador`

**Estructura:**
- KPIs (últimos 7 días):
  - Períodos cerrados | Distribuidores procesados | Bono total | Tasa éxito
- Gráfico de líneas: Bonos por día
- Tabla resumen: Últimos 10 cierres
- Alertas: si hay cierres fallidos

---

#### TAREA-198 ? ???
**Crear endpoint REST para consultar cierres**

**Ubicación:** `Aldebaran.Web\Controllers\Api\BonificationClosureController.cs`

**Endpoints:**
- `GET /api/bonification/closures/{instanceId}` — Detalle de cierre
- `GET /api/bonification/closures/{customerId}/history` — Histórico de cierres del distribuidor

---

#### TAREA-199 ? ??
**Crear modelo `BonificationClosureResult` para respuesta de cierre**

**Archivos a crear:**
- `Aldebaran.Application.Services\DTOs\BonificationClosureResult.cs`
- `Aldebaran.Application.Services\Models\CreditNoteRecommendation.cs`
- `Aldebaran.Application.Services\Models\ClosureNotificationData.cs`

---

#### TAREA-200 ? ??
**Crear servicio de notificación de cierre `IBonificationClosureNotificationService`**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationClosureNotificationService.cs`
- `Aldebaran.Application.Services\Services\BonificationClosureNotificationService.cs`

**Responsabilidad:**
- Envía email a PROMOS cuando se completa un cierre
- Incluye resumen de bonos calculados
- Incluye recomendaciones NC
- Incluye link a panel de revisión

---

#### TAREA-201 ? ??
**Integrar job de cierre con ciclo de vida de instancias**

**Modificar:**
- `BonificationPeriodInstanceLifecycleService.cs` → Registrar cierre al marcar instancia como CLOSED
- `Program.cs` → Registrar `BonificationPeriodClosureJob` como hosted service

---

#### TAREA-202 ? ???
**Crear tabla de recomendaciones `BonificationNCRecommendations`**

**Script SQL a crear:** `scripts/CreateBonificationNCRecommendationsTable.sql`

```sql
CREATE TABLE dbo.BonificationNCRecommendations (
    RECOMMENDATION_ID                INT             NOT NULL IDENTITY(1,1),
    BONIFICATION_CLOSED_PERIOD_ID    INT             NOT NULL,
    CUSTOMER_ID                      INT             NOT NULL,
    RECOMMENDED_NC_NUMBER            VARCHAR(50)     NOT NULL,
    RECOMMENDED_AMOUNT               DECIMAL(18,2)   NOT NULL,
    REASON                           VARCHAR(500)    NOT NULL,
    STATUS                           VARCHAR(20)     NOT NULL DEFAULT 'PENDING',  -- PENDING | APPROVED | REJECTED
    CREATED_AT                       DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    REVIEWED_BY                      INT             NULL,
    REVIEWED_AT                      DATETIME        NULL,
    CONSTRAINT PK_NC_RECOMMENDATION PRIMARY KEY CLUSTERED (RECOMMENDATION_ID),
    CONSTRAINT FK_RECOMMENDATION_CLOSED_PERIOD FOREIGN KEY (BONIFICATION_CLOSED_PERIOD_ID)
        REFERENCES dbo.BonificationClosedPeriods (BONIFICATION_CLOSED_PERIOD_ID),
    CONSTRAINT FK_RECOMMENDATION_CUSTOMER FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID),
    CONSTRAINT CK_RECOMMENDATION_STATUS CHECK (STATUS IN ('PENDING','APPROVED','REJECTED'))
);
```

---

#### TAREA-203 ? ??
**Crear `IBonificationNCRecommendationRepository` y servicio**

**Archivos a crear:**
- `IBonificationNCRecommendationRepository.cs` y implementación
- `IBonificationNCRecommendationService.cs` y implementación

---

#### TAREA-204 ? ??
**Crear página Admin: Gestión de Recomendaciones NC**

**Ruta:** `Pages\BonificationPages\BonificationNCRecommendations.razor`  
**URL:** `/bonification/nc-recommendations`  
**Rol requerido:** `Administrador`

**Estructura:**
- Filtros: Período | Estado | Distribuidor
- `RadzenDataGrid` con:
  - Columnas: Distribuidor | NC Recomendada | Monto | Razón | Estado | Acciones
- Botones: Aprobar | Rechazar (solo si PENDING)
- Al aprobar: se puede crear automáticamente la NC externa

---

#### TAREA-205 ? ???
**Crear pruebas unitarias de cierre**

**Ubicación:** `Aldebaran.Tests\Services\BonificationClosureServiceTests.cs`

**Cobertura:**
- Cálculo correcto de bonos finales
- Registro inmutable en BD
- Creación de siguiente instancia
- Generación de recomendaciones
- Manejo de errores parciales

---

#### TAREA-206 ? ???
**Crear pruebas de integración del job de cierre**

**Ubicación:** `Aldebaran.Tests.Integration\BonificationPeriodClosureJobTests.cs`

**Cobertura:**
- Ejecución del job a la hora correcta
- Procesamiento de múltiples instancias
- Notificación a PROMOS
- Log de auditoría

---

#### TAREA-207 ? ??
**Crear health check para job de cierre**

**Modificar:**
- `Aldebaran.Web\Controllers\Api\HealthController.cs` → Agregar check de última ejecución del job

---

#### TAREA-208 ? ??
**Implementar rollback manual de cierre (solo Admin)**

**Archivos a crear:**
- `Aldebaran.Application.Services\Services\IBonificationClosureRollbackService.cs`

**Responsabilidad:**
- Permite deshacer un cierre que fue erróneo
- Requiere roles de admin elevado
- Registra auditoría completa del rollback

---

#### TAREA-209 ? ??
**Crear configuración de cronograma de cierre**

**Modificar:**
- `appsettings.json` → Agregar:
```json
"BonificationClosureOptions": {
  "CronExpression": "0 59 23 * * *",
  "TimeoutSeconds": 3600,
  "NotificationRecipients": ["bonificacion@promos.com"],
  "SendDetailedReport": true,
  "ArchiveClosureFiles": true
}
```

---

#### TAREA-210 ? ??
**Agregar logging y observabilidad al cierre**

**Modificar:**
- Servicios de cierre para registrar métricas en Application Insights
- Dashboard de cierre para mostrar métricas en tiempo real

---

#### TAREA-211 ? ???
**Crear documentación de procedimiento de cierre**

**Archivos a crear:**
- `docs/BonificationClosure_Procedure.md` — Guía operativa para PROMOS

---

#### TAREA-212 ? ???
**Crear plan de validación y UAT del cierre automático**

**Archivos a crear:**
- `docs/BonificationClosure_UAT_Plan.md` — Casos de prueba y criterios de aceptación

---

> ✅ **Con TAREA-188 a TAREA-212 queda completamente cubierto CU10: Cierre Automático de Períodos**,  
> incluyendo:
> - FOTO congelada inmutable de bonos
> - Job nocturno automático
> - Recomendaciones de NC para revisión
> - Auditoría completa de cierre
> - Dashboard y reportería
> - Rollback manual si es necesario
> - Health checks y monitoreo

---

## 2.7 Módulo de Consulta - CU6: Consulta de Bono Actual (PROMOS)

> **Ref. propuesta funcional:** Casos de Uso CU6 + especificaciones funcionales.  
> **Alcance:** Consulta interna sin persistencia + Página Blazor interna + Descarga PDF puntual.  
> **Autenticación:** JWT interno (solo PROMOS autenticados).  
> **Datos:** Bonificación calculada en tiempo real, SIN auditoría de acceso.  
> **Uso:** Consulta operativa interna para revisión de bonificación actual durante el período.

---

### 2.7.1 Modelos & Respuesta de Cálculo Consolidado

#### **TAREA-213 ⏳ CREAR: Modelo consolidado de consulta sin persistencia**

**Propósito:** Estructura que consolida TODO para PROMOS sin auditoría, SIN persistencia de consulta.

**Ubicación:** `Aldebaran.Application.Services\Models\`

**Archivos a crear:**
- `BonificationCurrentPeriodDetail.cs` — Consolidado completo:
  - Facturación actual (TOTUS)
  - OC Especiales (desglose: aprobadas, pendientes, rechazadas)
  - Vigencia aplicada (tramo, % aplicable)
  - Bonificación parcial calculada
  - NC reconciliadas (estados actuales)
  - Descuentos globales aplicados
  - Timestamps de cálculo (solo internos, no persistidos)

**Características:**
- Solo datos en memoria, descartados al terminar request
- Consolidación en tiempo real, sin cache
- Indicadores de estado (errores parciales, fallback TOTUS, etc.)

**Estimación:** 2 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-214 ⏳ CREAR: Servicio de cálculo sin persistencia**

**Ubicación:** `Aldebaran.Application.Services\Services\`

**Archivos a crear:**
- `IBonificationCurrentPeriodService.cs`
  ```csharp
  public interface IBonificationCurrentPeriodService
  {
      /// Obtiene el cálculo ACTUAL del distribuidor (sin caché, sin persistencia)
      Task<BonificationCurrentPeriodDetail> GetDetailedCalculationAsync(
          int customerId, 
          int periodInstanceId, 
          CancellationToken ct);
  }
  ```

- `BonificationCurrentPeriodService.cs`
  - Orquesta servicios de cálculo (Facturación, Pedido)
  - Consolida estados actuales de BD
  - **NO persiste nada** — todo en memoria

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-215 ⏳ CREAR: Endpoint interno sin persistencia**

**Ubicación:** `Aldebaran.Web\Controllers\InternalApi\`

**Archivo a crear:**
- `BonificationCurrentPeriodController.cs`

**Endpoint:**
```
GET /api/internal/bonification/current-period/{customerId}/{periodInstanceId}
```

**Características:**
- Endpoint **INTERNO** (NO REST pública)
- Autenticación: JWT interno (sin OTP, solo internos)
- Respuesta: `BonificationCurrentPeriodDetail`
- **SIN auditoría de acceso** — se calcula y se descarta
- Cada request es independiente, sin persistencia

**Estimación:** 2 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-216 ⏳ CREAR: Página Blazor `BonificationCurrentPeriodDetail.razor`**

**Ruta:** `Pages\BonificationPages\BonificationCurrentPeriodDetail.razor`  
**URL:** `/bonification/current-period/{customerId}/{periodInstanceId}`  
**Rol:** `Administrador`, `Ingreso de OC especiales de bonificación` (PROMOS)

**Estructura (acceso interno PROMOS):**

1. **Encabezado:**
   - Distribuidor (nombre + doc)
   - Período (código + fechas)
   - Fecha de consulta (ahora)

2. **Sección 1 - Facturación:**
   - Total facturación TOTUS (actual)
   - Menos: NC anteriores descontadas
   - Más: OC Especiales aprobadas
   - **= Base de cálculo**

3. **Sección 2 - Pedidos:**
   - Total pedidos (excluyendo especiales)
   - Menos: Descuento por volumen (si aplica)
   - Más: OC Especiales aprobadas
   - **= Base de cálculo**

4. **Sección 3 - Vigencias Aplicadas:**
   - Vigencia facturación (rango, %)
   - Vigencia pedido (rango, %)
   - Descuento global (si aplica)

5. **Sección 4 - Bonificación Parcial:**
   - Bono Facturación = Base × %
   - Bono Pedido = Base × %
   - **Total = Suma de bonos**

6. **Botones:**
   - Actualizar (recalcula en tiempo real)
   - Descargar PDF (snapshot puntual)

**Estimación:** 10 horas | **Prioridad:** 🔴 CRÍTICA

---

#### **TAREA-217 ⏳ CREAR: Componente desglose de cálculo**

**Ubicación:** `Pages\BonificationPages\Components\BonificationCalculationBreakdownComponent.razor`

**Responsabilidad:**
- Mostrar desglose paso a paso de facturación → bono
- Con colores: inputs (verde), cálculos intermedios (azul), resultado (rojo)
- Totalmente dinámico, sin persistencia

**Props:** `BonificationCurrentPeriodDetail`

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-218 ⏳ CREAR: Servicio cliente Blazor**

**Ubicación:** `Aldebaran.Web\Services\BonificationCurrentPeriodService.cs`

**Métodos:**
- `GetDetailAsync(customerId, periodInstanceId, ct)`

**Características:**
- Consumir endpoint interno `/api/internal/bonification/current-period`
- Manejo de errores
- No cachea nada (cada llamada es nueva)

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-219 ⏳ CREAR: Endpoint PDF (snapshot puntual)**

**Ubicación:** `Aldebaran.Web\Controllers\InternalApi\`

**Endpoint:**
```
POST /api/internal/bonification/current-period/pdf
Body: { customerId, periodInstanceId }
```

**Características:**
- Genera PDF con datos **actuales** (sin persistencia)
- Incluye timestamp de generación
- Se descarga inmediatamente, no se guarda en BD
- Cada descarga es un nuevo cálculo

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-220 ⏳ CREAR: Acceso desde menú**

**Ubicación:** `Pages\BonificationPages\BonificationSpecialOrders.razor` (desde aquí accede PROMOS)

**Cambios:**
- Botón "Ver Detalle Bonificación" en listado de OC Especiales
- Navega a `/bonification/current-period/{customerId}/{periodInstanceId}`

**Estimación:** 1 hora | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-221 ⏳ CREAR: Pruebas unitarias**

**Ubicación:** `Aldebaran.Tests\Services\BonificationCurrentPeriodServiceTests.cs`

**Cobertura:**
- Consolidación de datos sin persistencia
- Cálculo correcto en tiempo real
- Manejo de errores parciales (TOTUS fallido, etc.)

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

#### **TAREA-222 ⏳ CREAR: Documentación de consulta**

**Ubicación:** `docs/CU6_CurrentPeriodConsultation_Guide.md`

**Contenido:**
- Cómo acceder a la consulta
- Interpretación de desglose
- Casos de uso típicos
- Limitaciones (sin persistencia = datos puntuales)

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### Resumen de archivos — CU6: Consulta Bonificación Actual (PROMOS)

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `Models\BonificationCurrentPeriodDetail.cs` | 213 | Nuevo |
| `IBonificationCurrentPeriodService.cs` | 214 | Nuevo |
| `BonificationCurrentPeriodService.cs` | 214 | Nuevo |
| `Controllers\InternalApi\BonificationCurrentPeriodController.cs` | 215 | Nuevo |
| `Pages\BonificationPages\BonificationCurrentPeriodDetail.razor` | 216 | Nuevo |
| `Pages\BonificationPages\BonificationCurrentPeriodDetail.razor.cs` | 216 | Nuevo |
| `Components\BonificationCalculationBreakdownComponent.razor` | 217 | Nuevo |
| `Services\BonificationCurrentPeriodService.cs` (cliente) | 218 | Nuevo |
| `Controllers\InternalApi\BonificationCurrentPeriodPdfController.cs` | 219 | Nuevo |
| `Pages\BonificationPages\BonificationSpecialOrders.razor` | 220 | Modificar |
| `BonificationCurrentPeriodServiceTests.cs` | 221 | Nuevo |
| `docs/CU6_CurrentPeriodConsultation_Guide.md` | 222 | Nuevo |
| `ArchitectureBuilderExtensions.cs` | 214, 218 | Modificar |

---

> ✅ **Con TAREA-213 a TAREA-222 queda completamente cubierto CU6: Consulta de Bonificación Actual (PROMOS)**,  
> incluyendo:
> - Cálculo en tiempo real sin persistencia
> - Endpoint interno sin auditoría de acceso
> - Página Blazor interna para PROMOS
> - Descarga PDF con snapshot puntual
> - Componentes de desglose visual
> - Testing unitario
> - Documentación operativa

---

## 2.8 Módulo de Consulta - CU8: Histórico de Bonos (PROMOS - Panel Administrativo Interno)

> **Ref. propuesta funcional:** Mismo CU8 que distribuidores, pero acceso interno para PROMOS.  
> **Alcance:** Página Blazor interna + Selectores de Distribuidor/Mes/Año + Exportación PDF/Excel.  
> **Autenticación:** JWT interno (solo PROMOS autenticados).  
> **Datos:** Bonos cerrados (FOTO congelada), inmutables, consolidados por mes/año.  
> **Diferencia con CU8 Distribuidor:** Acceso a TODO distribuidor, no solo el propio.

---

### TAREA-223 ⏳ CREAR: Página `BonificationHistoryPromos.razor`

**Ruta:** `Pages\BonificationPages\BonificationHistoryPromos.razor`  
**URL:** `/bonification/history/promos`  
**Rol:** `Administrador`, `Consulta de bonificaciones`

**Estructura:**

1. **Encabezado y Filtros:**
   - Dropdown **Distribuidor** (búsqueda por nombre/documento) — obligatorio
   - Dropdown **Mes** (1-12)
   - Dropdown **Año** (últimos 2 años)
   - Botón "Consultar"

2. **Resumen Consolidado (si hay datos):**
   - 4 tarjetas: Total Bono | Facturación | Pedidos | Gamificación

3. **Desglose por Tipo de Bono:**
   - Expandibles (`RadzenExpander`) para cada tipo:
     - Bono Facturación
     - Bono Pedido
   - Dentro: tabla de instancias cerradas del período (Código, Fechas, Bono, NC Aplicadas)

4. **Botones:**
   - Descargar PDF (reporte completo del mes del distribuidor)
   - Exportar Excel (con hojas: Resumen | Desglose | Detalle)

5. **Estados:**
   - Cargando (spinner)
   - Sin datos (mensaje "No hay datos para este distribuidor en este período")
   - Error (alerta roja con opción de reintentar)

**Estimación:** 8 horas | **Prioridad:** 🔴 CRÍTICA

---

### TAREA-224 ⏳ CREAR: Code-behind `BonificationHistoryPromos.razor.cs`

**Responsabilidad:**

```csharp
public partial class BonificationHistoryPromos
{
    private List<Customer> availableDistributors;
    private int? selectedCustomerId;
    private int selectedMonth = DateTime.Now.Month;
    private int selectedYear = DateTime.Now.Year;
    private BonificationHistoryMonthResponse historyData;
    private bool isLoading = false;
    private string errorMessage = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadAvailableDistributorsAsync();
    }

    private async Task LoadAvailableDistributorsAsync()
    {
        // Carga lista de distribuidores activos (IsDistributor = true)
        availableDistributors = await customerService.GetDistributorsAsync();
    }

    private async Task OnConsultAsync()
    {
        if (!selectedCustomerId.HasValue)
        {
            errorMessage = "Selecciona un distribuidor";
            return;
        }

        isLoading = true;
        errorMessage = null;

        try
        {
            historyData = await bonificationHistoryService.GetHistoryByMonthAsync(
                selectedCustomerId.Value, 
                selectedYear, 
                selectedMonth);
        }
        catch (Exception ex)
        {
            errorMessage = $"Error al consultar histórico: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnDownloadPdfAsync()
    {
        // Llama al servicio de PDF
        await bonificationHistoryService.DownloadPdfAsync(
            selectedCustomerId.Value, 
            selectedYear, 
            selectedMonth);
    }

    private async Task OnExportExcelAsync()
    {
        // Llama al servicio de Excel
        await bonificationHistoryService.DownloadExcelAsync(
            selectedCustomerId.Value, 
            selectedYear, 
            selectedMonth);
    }
}
```

**Estimación:** 4 horas | **Prioridad:** 🔴 CRÍTICA

---

### TAREA-225 ⏳ MODIFICAR: Extender `BonificationHistoryService` para PROMOS

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationHistoryService.cs`

**Cambios:**
- Agregar overload de `GetHistoryByMonthAsync` que **NO** valide que CustomerId sea el del usuario actual
- Agregar método `GetDistributorsAsync()` → retorna todos los distribuidores activos (para dropdown)
- Agregar auditoría de acceso (quién consulta a quién)

```csharp
public interface IBonificationHistoryService
{
    // Existente (distribuidor consultando su propio histórico)
    Task<BonificationHistoryMonthResponse> GetHistoryByMonthAsync(
        int customerId, int year, int month, CancellationToken ct);

    // NUEVO: PROMOS consultando histórico de otro distribuidor
    Task<BonificationHistoryMonthResponse> GetHistoryByMonthAsAdminAsync(
        int customerId, int year, int month, int adminUserId, CancellationToken ct);

    // NUEVO: Dropdown de distribuidores
    Task<List<Customer>> GetDistributorsAsync(CancellationToken ct);
}
```

**Estimación:** 3 horas | **Prioridad:** 🔴 CRÍTICA

---

### TAREA-226 ⏳ CREAR: Tabla de auditoría para consultas PROMOS

**Script SQL a crear:** `scripts/CreateBonificationHistoryPromoAuditTable.sql`

```sql
CREATE TABLE dbo.BonificationHistoryPromoAudit (
    AUDIT_ID                    INT             NOT NULL IDENTITY(1,1),
    ADMIN_USER_ID               INT             NOT NULL,       -- quién consultó
    CUSTOMER_ID                 INT             NOT NULL,       -- del distribuidor consultado
    YEAR                        INT             NOT NULL,
    MONTH                       INT             NOT NULL,
    QUERY_TIMESTAMP             DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    IP_ADDRESS                  VARCHAR(50)     NULL,
    CONSTRAINT PK_HISTORY_PROMO_AUDIT PRIMARY KEY CLUSTERED (AUDIT_ID),
    CONSTRAINT FK_AUDIT_ADMIN FOREIGN KEY (ADMIN_USER_ID)
        REFERENCES dbo.AspNetUsers (Id),
    CONSTRAINT FK_AUDIT_CUSTOMER FOREIGN KEY (CUSTOMER_ID)
        REFERENCES dbo.Customers (CUSTOMER_ID)
);
CREATE NONCLUSTERED INDEX IX_AUDIT_ADMIN_DATE
    ON dbo.BonificationHistoryPromoAudit (ADMIN_USER_ID, QUERY_TIMESTAMP);
CREATE NONCLUSTERED INDEX IX_AUDIT_CUSTOMER_DATE
    ON dbo.BonificationHistoryPromoAudit (CUSTOMER_ID, QUERY_TIMESTAMP);
```

**Estimación:** 1 hora | **Prioridad:** 🔴 CRÍTICA

---

### TAREA-227 ⏳ CREAR: Panel lateral de detalle (TAREA-227 del documento original)

**Ubicación:** `Pages\BonificationPages\Components\BonificationHistoryPromosSidePanel.razor`

**Responsabilidad:**
- Cuando PROMOS hace clic en "Ver detalle" de una instancia cerrada, se abre un panel lateral (slide-in desde la derecha)
- Muestra desglose completo:
  - Base de facturación (TOTUS)
  - NC conciliadas
  - OC especiales aprobadas
  - Vigencia aplicada (%)
  - Bono calculado
  - Desglose adicional si hay cambios manuales

**Props:**
- `BonificationClosedPeriod closedPeriod`
- `EventCallback<bool> OnClose`

**Estimación:** 10 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-228 ⏳ CREAR: Servicio de auditoría para consultas PROMOS

**Ubicación:** `Aldebaran.Application.Services\Services\IBonificationHistoryAuditService.cs`

**Responsabilidad:**
- Registra cada consulta de PROMOS a histórico de distribuidor
- Captura: quién, cuándo, a quién, desde dónde

```csharp
public interface IBonificationHistoryAuditService
{
    Task LogHistoryConsultationAsync(
        int adminUserId, 
        int customerId, 
        int year, 
        int month, 
        string ipAddress,
        CancellationToken ct);

    Task<List<BonificationHistoryAuditLog>> GetAuditLogsAsync(
        int? adminUserId, 
        int? customerId, 
        int? skip, 
        int? top, 
        CancellationToken ct);
}
```

**Estimación:** 3 horas | **Prioridad:** 🔴 CRÍTICA

---

### TAREA-229 ⏳ CREAR: Dashboard de Auditoría (opcional pero recomendado)

**Ubicación:** `Pages\BonificationPages\BonificationHistoryPromoAuditDashboard.razor`  
**URL:** `/bonification/history/promos/audit`  
**Rol:** `Administrador`

**Estructura:**
- Filtros: Mes | Año | Usuario PROMOS
- Tabla de consultas realizadas: Quién | A quién | Cuándo | IP
- Gráfico: Consultas por usuario en los últimos 30 días
- Alertas: Si un usuario consultó a más de 100 distribuidores en un día (posible abuso)

**Estimación:** 6 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-230 ⏳ CREAR: Servicio de PDF para PROMOS

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationHistoryPromosReportService.cs`

**Responsabilidad:**
- Genera PDF del histórico (igual que para distribuidor, pero con encabezado indicando que fue consultado por PROMOS)
- Incluye:
  - Nombre distribuidor
  - Mes/Año consultado
  - Resumen consolidado
  - Desglose por tipo de bono
  - Footer: "Consulta generada por PROMOS"

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-231 ⏳ CREAR: Servicio de Excel para PROMOS

**Ubicación:** `Aldebaran.Application.Services\Services\BonificationHistoryPromosExcelService.cs`

**Estructura (3 hojas):**
1. **"Resumen"** — Totales consolidados del mes
2. **"Por Tipo"** — Desglose por Facturación/Pedido
3. **"Detalle Completo"** — Cada instancia cerrada con sus líneas

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-232 ⏳ CREAR: Validación de permisos

**Ubicación:** Middleware o Guard Blazor

**Responsabilidad:**
- Validar que el usuario que accede a `/bonification/history/promos` tiene rol `Administrador` o `Consulta de bonificaciones`
- Si intenta acceder sin permiso → redirecciona a acceso denegado
- Log de intentos fallidos

**Estimación:** 2 horas | **Prioridad:** 🔴 CRÍTICA

---

### TAREA-233 ⏳ CREAR: Componente de búsqueda de distribuidor

**Ubicación:** `Pages\BonificationPages\Components\DistributorSelectorComponent.razor`

**Responsabilidad:**
- Dropdown searchable con:
  - Búsqueda por nombre
  - Búsqueda por documento
  - Mostrar solo distribuidores activos
  - Caché de 5 minutos

**Props:**
- `List<Customer> Distributors`
- `EventCallback<int> OnSelected`

**Estimación:** 3 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-234 ⏳ CREAR: Rate limiting para consultas PROMOS

**Ubicación:** Middleware de rate limiting

**Regla:**
- Máximo 50 consultas/minuto por usuario PROMOS
- Si excede → error 429 (Too Many Requests)

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-235 ⏳ CREAR: Testing de acceso PROMOS

**Ubicación:** `Aldebaran.Tests\Pages\BonificationHistoryPromosTests.cs`

**Cobertura:**
- Acceso autorizado: PROMOS ve página
- Acceso denegado: No-PROMOS es redirigido
- Consulta exitosa: Datos se cargan correctamente
- Auditoría se registra
- Rate limiting funciona

**Estimación:** 4 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-236 ⏳ CREAR: Documentación de uso para PROMOS

**Ubicación:** `docs/CU8_PROMOS_HistoryConsultation_Guide.md`

**Contenido:**
- Cómo acceder a `/bonification/history/promos`
- Filtros disponibles
- Cómo descargar PDF/Excel
- Qué información se guarda en auditoría
- Troubleshooting común

**Estimación:** 2 horas | **Prioridad:** 🟡 MEDIA

---

### TAREA-237 ⏳ CREAR: Link en menú principal

**Modificar:** `Shared\MainLayout.razor`

**Cambios:**
- Agregar ítem en menú "Bonificaciones > Operaciones":
```
<RadzenPanelMenuItem Text="Consultar Histórico de Distribuidor"
      Path="bonification/history/promos"
      Icon="history"
      Visible="@Security.IsInRole("Administrador","Consulta de bonificaciones")" />
```

**Estimación:** 1 hora | **Prioridad:** 🔴 CRÍTICA

---

## Resumen Final — CU8 para PROMOS (TAREA-223 a TAREA-237)

| Tarea | Descripción | Estimación | Prioridad |
|-------|-------------|-----------|-----------|
| 223 | Página `BonificationHistoryPromos.razor` con filtros | 8 h | 🔴 |
| 224 | Code-behind con lógica de consulta | 4 h | 🔴 |
| 225 | Extender `BonificationHistoryService` | 3 h | 🔴 |
| 226 | Tabla de auditoría | 1 h | 🔴 |
| 227 | Panel lateral de detalle | 10 h | 🟡 |
| 228 | Servicio de auditoría | 3 h | 🔴 |
| 229 | Dashboard de auditoría | 6 h | 🟡 |
| 230 | Servicio de PDF | 4 h | 🟡 |
| 231 | Servicio de Excel | 4 h | 🟡 |
| 232 | Validación de permisos | 2 h | 🔴 |
| 233 | Componente de búsqueda | 3 h | 🟡 |
| 234 | Rate limiting | 2 h | 🟡 |
| 235 | Testing | 4 h | 🟡 |
| 236 | Documentación | 2 h | 🟡 |
| 237 | Link en menú | 1 h | 🔴 |
| **TOTAL** | **CU8 PROMOS** | **~58 h** | — |

---

> ✅ **Con TAREA-223 a TAREA-237 queda completamente cubierto CU8 para PROMOS: Consulta de Histórico de Bonos desde Panel Administrativo Interno**, incluyendo:
> - Acceso a histórico de **cualquier distribuidor**
> - Agrupación por mes/año con desglose por tipo de bono
> - Exportación PDF/Excel con análisis
> - Auditoría completa de consultas (quién consultó a quién, cuándo)
> - Dashboard de auditoría para supervisar accesos
> - Rate limiting para prevenir abuso
> - Componentes reutilizables
> - Testing integral

---



---

## 📊 RESUMEN GLOBAL DE TAREAS

**Estructura completa del Sistema de Bonificación de Distribuidores:**

| Módulo | Sección | Tareas | Estimación | Estado |
|--------|---------|--------|-----------|--------|
| **Administración** | 2.1.1 Clientes Distribuidores | 001-010 | 25 h | 🔴 Pendiente |
| | 2.1.2 Reportes con Filtro | (incluidas 001-010) | — | 🔴 Pendiente |
| | 2.1.3 Gestión de Períodos | 011-028 | 40 h | 🔴 Pendiente |
| | 2.1.4 Rangos de Bonificación | 029-036 | 18 h | 🔴 Pendiente |
| | 2.1.5 Vigencias de Descuentos | 037-045 | 22 h | 🔴 Pendiente |
| **Operaciones** | 2.2.1 OC Especiales (Manual) | 046-050 | 20 h | 🔴 Pendiente |
| | 2.2.2 OC Especiales (Carga Masiva) | 051-058 | 18 h | 🔴 Pendiente |
| | 2.2.3 Conciliación de NC | 059-069 | 40 h | 🔴 Pendiente |
| | 2.2.4 Lista de Precios Promocional | 070-078 | 32 h | 🔴 Pendiente |
| | 2.2.5 Gestión de Exclusiones | 079-095 | 45 h | 🔴 Pendiente |
| | 2.2.6 Integración TOTUS por SP | 096-110 | 50 h | 🔴 Pendiente |
| | 2.2.7 Autenticación OTP (MVP Email) | 111-119 | 20 h | 🔴 Pendiente |
| **Notificaciones** | 2.3 Notificaciones Automáticas de Bonificación | 120-134 | 50 h | 🔴 Pendiente |
| **Consulta CU7** | 2.4 Consulta de Bonificación (Período Actual) | 135-166 | 120 h | 🔴 Pendiente |
| **Consulta CU8** | 2.5 Consulta de Histórico (Períodos Cerrados) - Distribuidores | 167-186 | 50 h | 🔴 Pendiente |
| **Cierre CU10** | 2.6 Cierre Automático de Períodos | 187-212 | 65 h | 🔴 Pendiente |
| **Consulta CU6** | 2.7 Consulta de Bonificación Actual (PROMOS) | 213-222 | 33 h | 🔴 Pendiente |
| **Histórico PROMOS** | 2.8 Consulta de Histórico (PROMOS - Panel Administrativo) | 223-237 | 92 h | 🔴 Pendiente |
| | | | | |
| **TOTAL** | | **237 TAREAS** | **~875 h** | 🔴 Pendiente |

---

**Desglose por Área de Trabajo:**

| Área | Tareas | Estimación | % del Proyecto |
|------|--------|-----------|-----------------|
| Backend (DB + Services) | 001-110, 120-134, 135-150, 167-177, 187-212, 223-237 | ~500 h | 57% |
| Frontend (Blazor Pages + Components) | 004-009, 089-090, 135-144, 151-166, 223-237 | ~235 h | 27% |
| API REST + Integraciones | 116, 132-136, 151-166, 223-237 | ~100 h | 11% |
| Configuración + DevOps | 097, 149-150, 205 | ~20 h | 2% |
| Testing (Unit + Integration + E2E) *Fase General* | Tests en cada módulo | ~40 h | 3% |
| **SUBTOTAL** | | **~875 h** | **100%** |

---

**Duración Estimada por Equipo:**

| Equipo | Duración | Fases |
|--------|----------|-------|
| **1 Desarrollador** | 43-54 semanas | 10-13 meses |
| **2 Desarrolladores** | 22-27 semanas | 5-6 meses |
| **3 Desarrolladores** | 15-18 semanas | 3.5-4.5 meses |
| **4 Desarrolladores** | 11-14 semanas | 2.5-3.5 meses |

---

**Dependencias Críticas (Orden de Ejecución Recomendado):**

```
Fase 1 (Semana 1-2): Prerequisitos
├─ TAREA-001 a TAREA-010 (Configuración de Clientes)
├─ TAREA-111 a TAREA-119 (Autenticación OTP)
└─ TAREA-070 a TAREA-078 (Lista de Precios)

Fase 2 (Semana 3-5): Configuración de Bonificación
├─ TAREA-011 a TAREA-028 (Períodos)
├─ TAREA-029 a TAREA-036 (Rangos)
├─ TAREA-037 a TAREA-045 (Descuentos)
└─ TAREA-079 a TAREA-095 (Exclusiones)

Fase 3 (Semana 6-8): Integraciones Críticas
├─ TAREA-096 a TAREA-110 (TOTUS SP)
└─ TAREA-046 a TAREA-058 (OC Especiales)

Fase 4 (Semana 9-12): Funcionalidad Principal
├─ TAREA-059 a TAREA-069 (Conciliación NC)
├─ TAREA-120 a TAREA-151 (CU7 Consulta Actual)
├─ TAREA-168 a TAREA-187 (CU8 Histórico)
├─ TAREA-188 a TAREA-212 (CU10 Cierre Automático)
└─ TAREA-213 a TAREA-222 (CU6 Consulta Actual PROMOS)

Fase 5 (Semana 13-15): Notificaciones + Testing/Docs Integral
├─ TAREA-152 a TAREA-167 (Notificaciones)
└─ Testing integral + Documentación final
```

---

**Criterios de Aceptación Global:**

✅ **Funcionalidad Completa:**
- Todos los CU implementados y operativos
- APIs REST documentadas y probadas
- UI Blazor responsiva y accesible

✅ **Calidad:**
- Cobertura de testing ≥ 80% (unit + integration + E2E)
- Cero defectos críticos
- SLA cumplido: < 500ms para CU7, < 200ms para CU8

✅ **Seguridad:**
- Autenticación OTP + JWT implementada
- Validaciones de entrada en todos los endpoints
- HTTPS obligatorio en producción
- RGPD compliance (PII limitado en logs)

✅ **Operabilidad:**
- Health checks funcionales
- Observabilidad con Application Insights
- Runbooks de incident disponibles
- Plan de rollback documentado

✅ **Documentación:**
- Propuesta funcional final actualizada
- OpenAPI/Swagger completo
- Guías de deployment
- README de configuración

---

**Riesgos Identificados y Mitigaciones:**

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|-------------|--------|-----------|
| Retraso en SP TOTUS (fábrica externa) | ALTA | CRÍTICO | Coordinación temprana, mock de fallback |
| Performance TOTUS (> 500ms) | MEDIA | CRÍTICO | Índices optimizados, connection pooling |
| Cambios scope a mitad del proyecto | MEDIA | ALTO | Change control, validación semanal |
| Falta de claridad en requisitos | MEDIA | ALTO | Weekly sync con PROMOS, doc actualizado |
| Integración Rabbit Message Queue | BAJA | ALTO | Testing temprano, fallback local |
| Performance bajo carga (100+ usuarios) | BAJA | MEDIO | Load testing en fase de testing |

---

---

