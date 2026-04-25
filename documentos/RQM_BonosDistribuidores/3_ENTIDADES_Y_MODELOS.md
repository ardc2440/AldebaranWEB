# 3. ENTIDADES Y MODELOS DE DATOS - Bonos Distribuidores

## Status: ? PENDIENTE DEFINICIÓN

---

## ?? Secciones a Documentar

### 3.1 Entidades Principales

#### Entidad: Distribuidor (si no existe)
```csharp
[ ] Nombre: Distributor / Distribuidor
[ ] Atributos:
    [ ] Id (PK)
    [ ] Nombre
    [ ] Contacto
    [ ] Email
    [ ] Otros: ?

[ ] Relaciones:
    [ ] 1:N con Bonos
    [ ] 1:N con [Otros]

[ ] Validaciones:
    [ ]
```

#### Entidad: Bono
```csharp
[ ] Nombre: Bonus / Bono / BonusDistributor
[ ] Atributos:
    [ ] Id (PK)
    [ ] DistributorId (FK)
    [ ] Tipo (Enum: Porcentaje, Monto fijo, Cantidad unidades, etc.)
    [ ] Valor
    [ ] FechaInicio
    [ ] FechaFin
    [ ] Estado (Enum: Activo, Inactivo, Vencido, etc.)
    [ ] FechaCreacion
    [ ] FechaModificacion
    [ ] CreadoPor
    [ ] ModificadoPor
    [ ] Otros: ?

[ ] Relaciones:
    [ ] N:1 con Distribuidor
    [ ] 1:N con Detalles de Bono
    [ ] 1:N con [Otros]

[ ] Validaciones:
    [ ]
```

#### Entidad: Detalle de Bono
```csharp
[ ] Nombre: BonusDetail / BonusCriteria
[ ] Atributos:
    [ ] Id (PK)
    [ ] BonusId (FK)
    [ ] Criterio (Enum: Por artículo, Por cliente, Por cantidad, etc.)
    [ ] Referencia/ArticuloId
    [ ] ClienteId (si aplica)
    [ ] Cantidad/Rango
    [ ] Otros: ?

[ ] Relaciones:
    [ ] N:1 con Bono
    [ ] 1:N con [Otros]

[ ] Validaciones:
    [ ]
```

#### Entidad: Aplicación/Historial de Bono
```csharp
[ ] Nombre: BonusApplication / BonusHistory / BonusLog
[ ] Propósito: Registrar cuándo y cómo se aplicó cada bono
[ ] Atributos:
    [ ] Id (PK)
    [ ] BonusId (FK)
    [ ] DocumentoReferencia (OrderId, ReservationId, etc.)
    [ ] FechaAplicacion
    [ ] MontoAplicado
    [ ] Estado (Aplicado, Rechazado, Pendiente)
    [ ] Justificacion
    [ ] Otros: ?

[ ] Relaciones:
    [ ] N:1 con Bono
    [ ] Relación con documento (Order, Reservation, etc.)

[ ] Validaciones:
    [ ]
```

#### Entidades Adicionales (si aplica)
```
[ ] Tipo de Bono
[ ] Condición de Bono
[ ] Configuración de Bono
[ ] Otras: ?
```

### 3.2 Modelos de Dominio

#### Enums Necesarios
```csharp
[ ] BonusType
    - Porcentaje
    - MontoFijo
    - CantidadUnidades
    - Otros: ?

[ ] BonusStatus
    - Activo
    - Inactivo
    - Vencido
    - Pausado
    - Otros: ?

[ ] BonusCriteria
    - PorArticulo
    - PorCliente
    - PorCantidad
    - PorValor
    - Otros: ?

[ ] ApplicationStatus
    - Aplicado
    - Rechazado
    - Pendiente
    - Cancelado
    - Otros: ?

[ ] Otros Enums: ?
```

### 3.3 DTOs (Data Transfer Objects)

#### Para UI/API
```csharp
[ ] CreateBonusDto
    - Campos necesarios para crear

[ ] UpdateBonusDto
    - Campos editables

[ ] BonusDetailDto
    - Información para visualizar

[ ] BonusApplicationDto
    - Información de aplicación

[ ] Otros DTOs: ?
```

### 3.4 ViewModels (Blazor Components)

```csharp
[ ] BonusListViewModel
[ ] CreateEditBonusViewModel
[ ] BonusDetailViewModel
[ ] Otros ViewModels: ?
```

### 3.5 Relaciones Entre Entidades

```
Diagrama ER (ASCII):

????????????????
? Distribuidor ?
????????????????
       ? 1:N
       ?
       ????????????????
                      ?
                  ?????????????????
                  ?     Bono      ?
                  ?????????????????
                      ? 1:N
                      ?
            ??????????????????????
            ?                    ?
        ????????????????????  ???????????????????
        ?  BonusDetail    ?  ? BonusApplication?
        ??????????????????  ???????????????????
```

### 3.6 Configuraciones de Entidades (EF Core Fluent API)

```csharp
[ ] Bonus Configuration
    [ ] HasKey
    [ ] HasMany/HasOne
    [ ] Property conversions
    [ ] Indexes
    [ ] Constraints

[ ] BonusDetail Configuration
    [ ]

[ ] BonusApplication Configuration
    [ ]

[ ] Otros: ?
```

### 3.7 Migraciones Planeadas

```
[ ] Crear tabla Bonus
[ ] Crear tabla BonusDetail
[ ] Crear tabla BonusApplication
[ ] Agregar FK en tablas relacionadas
[ ] Crear índices
[ ] Agregar triggers EF Core
[ ] Otros: ?
```

### 3.8 Datos Iniciales (Seed Data)

```
[ ] Tipos de bono por defecto
[ ] Configuraciones iniciales
[ ] Datos maestros
[ ] Otros: ?
```

### 3.9 Segunda Base de Datos - Entidades

```
[ ] Si la segunda BD tiene entidades propias:
    [ ] Entidad 1: ?
    [ ] Entidad 2: ?
    [ ] Entidad N: ?

[ ] Cómo mapean con la BD principal: ?
[ ] DbContext separado o compartido: ?
```

---

## ?? Modelo de Datos Completo (a definir)

```sql
-- [SCRIPT SQL A DEFINIR POSTERIORMENTE]
```

---

## ?? Referencias Cruzadas

- Ver: **1_REQUERIMIENTOS.md** - Casos de uso
- Ver: **2_ARQUITECTURA.md** - Patrones
- Ver: **6_SEGUNDA_BASE_DATOS.md** - Integraciones con otra BD
- Ver: **10_MIGRACIONES_BD.md** - Scripts de migración

---

## ?? Notas del Diseño de Datos

> [Aquí irán decisiones sobre el modelo de datos]

---

**Última actualización**: [Pendiente]
**Responsable**: [Usuario]
**Estado**: ?? Incompleto - Pendiente diseño de modelo de datos
