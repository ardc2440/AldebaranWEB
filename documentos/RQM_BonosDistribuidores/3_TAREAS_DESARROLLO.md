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

### 2.1.1 – Reportes con filtro de Cliente

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

---

### 2.1.4 Gestión de Tipos de Bono

> Funcionalidad completamente nueva.  
> Permite crear **Tipos de Bono** que combinan un Período de periodicidad con una Base de Cálculo (Facturación, Pedido o Entrega). Son la unidad de configuración a la que se le asignan Vigencias con rangos y porcentajes.

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
      Task<(IEnumerable<BonificationType>, int)> GetAsync(int? skip, int? top, CancellationToken ct = default);
      Task<(IEnumerable<BonificationType>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
      Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
      Task<bool> HasActiveInstanceAsync(int id, CancellationToken ct = default);
      Task<bool> HasActiveVigencyAsync(int id, CancellationToken ct = default);
  }
  ```

- `Aldebaran.DataAccess.Infraestructure\Repository\BonificationTypeRepository.cs` — implementación usando `RepositoryBase<AldebaranDbContext>` (mismo patrón que `AreaRepository`)
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
**Actualizar `BonificationPeriods.razor.cs` y `BonificationTypes.razor.cs` para incluir columnas de Actions en el DataGrid**

**Cambios:**
- Ambas páginas ahora tendrán una columna "Acciones" con un botón de edición (icono lápiz) que abre el respetivo diálogo de edición.
- Se elimina el botón de "Generar Instancia" en `BonificationPeriods`. Las instancias son automáticas y no deben ser manipuladas desde aquí.

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
- Misma estructura que TAREA-034
- **Si estado = `PENDING`** ? cabecera y rangos son editables (la fecha de activación aún no llegó)
- **Si estado = `ACTIVE` o `INACTIVE`** ? todo en modo solo lectura con mensaje explicativo: "Esta vigencia está en curso o fue reemplazada. No se pueden modificar sus rangos."
- Botones: Guardar (solo visible si PENDING) / Cerrar

---

> ? **Con TAREA-029 a TAREA-035 quedan cubiertas todas las modificaciones necesarias relacionadas con Rangos de Bonificación.**

---

### 2.1.6 Vigencias de Descuentos por Total de Pedido

> Ref. propuesta funcional: sección 2.2.0.3 (MD v1.4)  
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

**Ejemplo de rangos:**

| Orden | Desde | Hasta | Tipo | Valor |
|-------|-------|-------|------|-------|
| 1 | $1,000,001 | $5,000,000 | Fijo | $100,000 |
| 2 | $5,000,001 | $10,000,000 | % | 2% |
| 3 | $10,000,001 | ? (NULL) | % | 5% |

> Si el total del pedido del distribuidor es ? $1,000,000 ? descuento = $0 (no hay rango que lo cubra).

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

- `Aldebaran.DataAccess.Infraestructure\Repository\DiscountVigencyRepository.cs`

> **Diferencia clave vs `BonificationVigencyRepository`:**  
> `GetActiveAsync()` no recibe `bonificationTypeId` porque la vigencia de descuento es **global** — hay una sola activa en el sistema.

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
**Crear dialog `AddDiscountVigency.razor` + `.razor.cs`**

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
**Crear dialog `EditDiscountVigency.razor` + `.razor.cs`**

**Estructura:**
- Misma estructura que TAREA-043
- **Si estado = `PENDING`** ? cabecera y rangos editables
- **Si estado = `ACTIVE` o `INACTIVE`** ? todo en modo solo lectura con mensaje: "Esta vigencia está en curso o fue reemplazada. No se pueden modificar sus rangos."
- Botones: Guardar (solo visible si `PENDING`) / Cerrar

---

#### TAREA-045 ? ???
**Agregar ítem "Descuentos por Pedido" al menú de Bonificaciones en `MainLayout.razor`**

```razor
<RadzenPanelMenuItem Text="Bonificaciones" Icon="percent"
    Visible="@Security.IsInRole(...)">
    <RadzenPanelMenuItem Text="Períodos" Path="bonification/periods" />
    <RadzenPanelMenuItem Text="Tipos de Bono" Path="bonification/types" />
    <RadzenPanelMenuItem Text="Descuentos por Pedido" Path="bonification/discount-vigencies" />
</RadzenPanelMenuItem>
```

---

#### Resumen de archivos afectados — 2.1.6 Vigencias de Descuentos por Total de Pedido

| Archivo | Tarea | Tipo |
|---------|-------|------|
| `scripts/CreateDiscountVigencyTables.sql` | 037 | Nuevo |
| `DataAccess\Entities\DiscountVigency.cs` | 038 | Nuevo |
| `DataAccess\Entities\DiscountVigencyRange.cs` | 038 | Nuevo |
| `DataAccess\Configuration\DiscountVigencyConfiguration.cs` | 038 | Nuevo |
| `DataAccess\Configuration\DiscountVigencyRangeConfiguration.cs` | 038 | Nuevo |
| `DataAccess\AldebaranDbContext.cs` | 038 | Modificar |
| `Application.Services\Models\DiscountVigency.cs` | 039 | Nuevo |
| `Application.Services\Models\DiscountVigencyRange.cs` | 039 | Nuevo |
| `Application.Services\Mappings\ApplicationServicesProfile.cs` | 039 | Modificar |
| `DataAccess.Infraestructure\Repository\IDiscountVigencyRepository.cs` | 040 | Nuevo |
| `DataAccess.Infraestructure\Repository\DiscountVigencyRepository.cs` | 040 | Nuevo |
| `Application.Services\Services\IDiscountVigencyService.cs` | 041 | Nuevo |
| `Application.Services\Services\DiscountVigencyService.cs` | 041 | Nuevo |
| `Web\Extensions\ArchitectureBuilderExtensions.cs` | 041 | Modificar |
| `Web\Pages\BonificationPages\DiscountVigencies.razor` | 042 | Nuevo |
| `Web\Pages\BonificationPages\DiscountVigencies.razor.cs` | 042 | Nuevo |
| `Web\Pages\BonificationPages\AddDiscountVigency.razor` | 043 | Nuevo |
| `Web\Pages\BonificationPages\AddDiscountVigency.razor.cs` | 043 | Nuevo |
| `Web\Pages\BonificationPages\EditDiscountVigency.razor` | 044 | Nuevo |
| `Web\Pages\BonificationPages\EditDiscountVigency.razor.cs` | 044 | Nuevo |
| `Web\Shared\MainLayout.razor` | 045 | Modificar |
