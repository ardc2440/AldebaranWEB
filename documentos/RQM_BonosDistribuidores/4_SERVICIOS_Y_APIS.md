# 4. SERVICIOS Y APIs - Bonos Distribuidores

## Status: ? PENDIENTE DEFINICIÓN

---

## ?? Secciones a Documentar

### 4.1 Servicios de Negocio (Application Layer)

#### Servicio: BonusService
```csharp
Interfaz: IBonusService

[ ] Métodos principales:
    [ ] CreateBonusAsync(CreateBonusDto)
        Retorna: BonusDetailDto
        Validaciones: ?

    [ ] UpdateBonusAsync(id, UpdateBonusDto)
        Retorna: BonusDetailDto

    [ ] DeleteBonusAsync(id)
        Retorna: bool

    [ ] GetBonusAsync(id)
        Retorna: BonusDetailDto

    [ ] GetBonusByDistributorAsync(distributorId)
        Retorna: IEnumerable<BonusDetailDto>

    [ ] GetActiveBonusesAsync()
        Retorna: IEnumerable<BonusDetailDto>

    [ ] ApplyBonusAsync(bonusId, orderId/reservationId)
        Retorna: BonusApplicationDto

    [ ] CalculateBonusAmountAsync(bonusId, criteria)
        Retorna: decimal

    [ ] Otros métodos: ?

Ubicación: Aldebaran.Application.Services/Services/BonusService.cs
Dependencias:
    - IBonusRepository
    - IBonusDetailRepository
    - IBonusApplicationRepository
    - ILogger
```

#### Servicio: BonusDetailService
```csharp
Interfaz: IBonusDetailService

[ ] Métodos principales:
    [ ] CreateDetailAsync(BonusDetailDto)
    [ ] UpdateDetailAsync(id, BonusDetailDto)
    [ ] DeleteDetailAsync(id)
    [ ] GetDetailsByBonusAsync(bonusId)
    [ ] Otros: ?

Ubicación: Aldebaran.Application.Services/Services/BonusDetailService.cs
```

#### Servicio: BonusApplicationService
```csharp
Interfaz: IBonusApplicationService

[ ] Métodos principales:
    [ ] RecordApplicationAsync(BonusApplicationDto)
    [ ] GetApplicationHistoryAsync(bonusId)
    [ ] GetApplicationHistoryByDistributorAsync(distributorId)
    [ ] ValidateApplicationAsync(bonusId, orderId)
    [ ] Otros: ?

Ubicación: Aldebaran.Application.Services/Services/BonusApplicationService.cs
```

#### Otros Servicios
```
[ ] DistributorBonusService (si requiere)
    Métodos: ?

[ ] BonusCalculationService
    Métodos: ?

[ ] BonusNotificationService
    Métodos: ?

[ ] Otros: ?
```

### 4.2 Repositorios (Data Access Layer)

#### Repositorio: IBonusRepository
```csharp
Interfaz: IBonusRepository : IGenericRepository<Bonus>

[ ] Métodos personalizados:
    [ ] GetByDistributorIdAsync(distributorId)
    [ ] GetActiveByDistributorAsync(distributorId)
    [ ] GetExpiredBonusesAsync()
    [ ] GetBonusesByStatusAsync(status)
    [ ] SearchBonusesAsync(criteria)
    [ ] Otros: ?

Ubicación: Aldebaran.DataAccess.Infraestructure/Repository/BonusRepository.cs
```

#### Repositorio: IBonusDetailRepository
```csharp
Interfaz: IBonusDetailRepository : IGenericRepository<BonusDetail>

[ ] Métodos personalizados:
    [ ] GetDetailsByBonusAsync(bonusId)
    [ ] SearchByArticleAsync(articleId)
    [ ] SearchByCustomerAsync(customerId)
    [ ] Otros: ?

Ubicación: Aldebaran.DataAccess.Infraestructure/Repository/BonusDetailRepository.cs
```

#### Repositorio: IBonusApplicationRepository
```csharp
Interfaz: IBonusApplicationRepository : IGenericRepository<BonusApplication>

[ ] Métodos personalizados:
    [ ] GetApplicationsByBonusAsync(bonusId)
    [ ] GetApplicationsByDistributorAsync(distributorId)
    [ ] GetApplicationsByDocumentAsync(documentId, documentType)
    [ ] GetPendingApplicationsAsync()
    [ ] Otros: ?

Ubicación: Aldebaran.DataAccess.Infraestructure/Repository/BonusApplicationRepository.cs
```

### 4.3 Controllers (API Layer)

#### Controller: BonusController
```
Ruta base: /api/bonus

[ ] Endpoints:
    [ ] GET /api/bonus
        Descripción: Obtener todos los bonos (con paginación/filtrado)
        Parámetros: skip, take, filter, sort
        Retorna: ODataServiceResult<BonusDetailDto>
        Roles requeridos: Administrador, Gestor de Bonos

    [ ] GET /api/bonus/{id}
        Descripción: Obtener bono por ID
        Retorna: BonusDetailDto

    [ ] GET /api/bonus/distributor/{distributorId}
        Descripción: Bonos de un distribuidor
        Retorna: IEnumerable<BonusDetailDto>

    [ ] POST /api/bonus
        Descripción: Crear nuevo bono
        Body: CreateBonusDto
        Retorna: BonusDetailDto
        Roles: Administrador

    [ ] PUT /api/bonus/{id}
        Descripción: Actualizar bono
        Body: UpdateBonusDto
        Retorna: BonusDetailDto
        Roles: Administrador

    [ ] DELETE /api/bonus/{id}
        Descripción: Eliminar bono
        Retorna: 204 NoContent
        Roles: Administrador

    [ ] POST /api/bonus/{id}/apply
        Descripción: Aplicar bono
        Body: ApplyBonusRequest
        Retorna: BonusApplicationDto

    [ ] GET /api/bonus/{id}/applications
        Descripción: Historial de aplicación
        Retorna: IEnumerable<BonusApplicationDto>

Ubicación: Aldebaran.Web/Controllers/BonusController.cs
```

#### Controller: BonusDetailController
```
Ruta base: /api/bonus-detail

[ ] Endpoints:
    [ ] GET /api/bonus-detail?bonusId={bonusId}
    [ ] POST /api/bonus-detail
    [ ] PUT /api/bonus-detail/{id}
    [ ] DELETE /api/bonus-detail/{id}
    [ ] Otros: ?

Ubicación: Aldebaran.Web/Controllers/BonusDetailController.cs
```

#### Controller: BonusApplicationController
```
Ruta base: /api/bonus-application

[ ] Endpoints:
    [ ] GET /api/bonus-application (historial)
    [ ] GET /api/bonus-application/distributor/{distributorId}
    [ ] GET /api/bonus-application/document/{documentId}
    [ ] POST /api/bonus-application/validate
    [ ] Otros: ?

Ubicación: Aldebaran.Web/Controllers/BonusApplicationController.cs
```

### 4.4 OData Endpoints

```
Rutas OData (si se implementan):

[ ] /odata/Bonus
    - Filtrado por $filter
    - Ordenamiento por $orderby
    - Expansión por $expand
    - Selección por $select

[ ] /odata/BonusDetail
[ ] /odata/BonusApplication
```

### 4.5 Validaciones en Servicios

```csharp
[ ] ValidateBonusCreation(bonus)
    - Validar fechas (inicio < fin)
    - Validar valor > 0
    - Validar distribuidor existe
    - Otros: ?

[ ] ValidateBonusApplication(bonus, order/reservation)
    - Validar bono activo
    - Validar dentro de rango de fechas
    - Validar criterios de aplicación
    - Validar no duplicado
    - Otros: ?

[ ] Otros validadores: ?
```

### 4.6 Mapeos AutoMapper

```csharp
Perfil: BonusProfile

[ ] Bonus ? BonusDetailDto
[ ] CreateBonusDto ? Bonus
[ ] UpdateBonusDto ? Bonus
[ ] BonusDetail ? BonusDetailDto
[ ] BonusApplication ? BonusApplicationDto
[ ] Otros: ?

Ubicación: Aldebaran.Web/Mappings/ViewModelProfile.cs (agregar)
```

### 4.7 DTOs Detallados

```csharp
[ ] CreateBonusDto
    - DistributorId: int
    - Type: BonusType
    - Value: decimal
    - StartDate: DateTime
    - EndDate: DateTime
    - Otros: ?

[ ] UpdateBonusDto
    - Id: int
    - Value: decimal (¿editable?)
    - EndDate: DateTime (¿editable?)
    - Status: BonusStatus
    - Otros: ?

[ ] BonusDetailDto
    - Todos los campos de Bonus
    - Información del Distribuidor
    - Estado (activo/vencido/etc)
    - Detalles relacionados

[ ] CreateBonusDetailDto
    - BonusId: int
    - Criteria: BonusCriteria
    - ArticleId/CustomerId: int?
    - Cantidad: int?
    - Otros: ?

[ ] ApplyBonusRequest
    - BonusId: int
    - OrderId/ReservationId: int
    - Cantidad: decimal?
    - Otros: ?

[ ] BonusApplicationDto
    - ApplicationId: int
    - BonusId: int
    - DistributorId: int
    - DocumentReference: string
    - ApplicationDate: DateTime
    - AppliedAmount: decimal
    - Status: ApplicationStatus
    - Otros: ?
```

### 4.8 Excepciones Personalizadas

```csharp
[ ] BonusNotFoundException
[ ] BonusApplicationException
[ ] InvalidBonusException
[ ] BonusExpiredException
[ ] Otros: ?
```

### 4.9 Eventos de Negocio (RabbitMQ)

```
[ ] BonusCreatedEvent
    Propiedades: BonusId, DistributorId, CreatedDate

[ ] BonusActivatedEvent
    Propiedades: BonusId, StartDate

[ ] BonusAppliedEvent
    Propiedades: BonusId, ApplicationId, Amount, OrderId

[ ] BonusExpiredEvent
    Propiedades: BonusId, EndDate

[ ] Otros eventos: ?
```

---

## ?? Diagrama de Capas (a completar)

```
???????????????????????????????????
?      Controllers / Pages        ?
?    (BonusController, UI)        ?
???????????????????????????????????
             ?
???????????????????????????????????
?   Services Layer                ?
?   (IBonusService, etc.)         ?
???????????????????????????????????
             ?
???????????????????????????????????
?   Repository Layer              ?
?   (IBonusRepository, etc.)      ?
???????????????????????????????????
             ?
???????????????????????????????????
?   EF Core DbContext             ?
?   (AldebaranDbContext)          ?
???????????????????????????????????
             ?
        [SQL Server]
```

---

## ?? Referencias Cruzadas

- Ver: **3_ENTIDADES_Y_MODELOS.md** - Modelos de datos
- Ver: **2_ARQUITECTURA.md** - Decisiones arquitectónicas
- Ver: **9_CAMBIOS_CODIGO.md** - Cambios a implementar

---

## ?? Notas de Diseño de Servicios

> [Aquí irán decisiones sobre servicios]

---

**Última actualización**: [Pendiente]
**Responsable**: [Usuario]
**Estado**: ?? Incompleto - Pendiente definición de servicios
