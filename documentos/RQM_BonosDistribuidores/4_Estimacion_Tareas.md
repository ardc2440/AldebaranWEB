# 4. ESTIMACIÓN DE TAREAS - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Fecha**: Mayo 2026  
**Versión**: 2.0  
**Perfil asumido**: Desarrollador humano Senior .NET / Blazor con conocimiento del codebase Aldebaran  

---

## Criterios de Estimación

| Factor | Criterio |
|--------|----------|
| **Perfil** | Desarrollador Senior con experiencia en el stack (C#, EF, Blazor, Radzen, SQL Server) |
| **Familiaridad** | Conoce el codebase Aldebaran (patrones Repository, Service, Radzen UI) |
| **Unidad** | Horas de desarrollo efectivo (excluye reuniones, code review, deploy) |
| **Incluye** | Análisis, codificación, prueba unitaria básica, ajustes por feedback inmediato |
| **No incluye** | QA formal, pruebas de integración, documentación adicional, despliegue |

---

## Leyenda de Prioridad

| Prioridad | Significado |
|-----------|-------------|
| ?? **REQUERIDO** | El sistema no funciona o no cumple el objetivo principal sin esta tarea. Su omisión bloquea otras tareas o casos de uso críticos. |
| ?? **SUGERIDO** | Mejora la operabilidad o la experiencia de usuario de forma significativa, pero el sistema puede funcionar sin ella en una primera versión. |
| ?? **DESEABLE** | Agrega comodidad, eficiencia o cobertura adicional. Puede diferirse a una segunda iteración sin impacto en el MVP. |

---

## Resumen Ejecutivo por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| ?? REQUERIDO | 29 | 170 | 74.6% |
| ?? SUGERIDO | 10 | 42 | 18.4% |
| ?? DESEABLE | 4 | 16 | 7.0% |
| **TOTAL** | **43** | **228** | **100%** |

> **Reducción máxima posible** (eliminando Sugeridos + Deseables): **?58 h** ? MVP en **170 h (~21 días hábiles)**  
> **Reducción moderada** (eliminando solo Deseables): **?16 h** ? MVP+ en **212 h (~26.5 días hábiles)**

---

## Detalle por Tarea

### 2.1.1 – Configuración de Clientes Distribuidores

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-001 | Identificar distribuidores en BD | ??? Script migración `Customers` | Agregar columnas `IsDistributor` y `BonusEmail`. Script idempotente con `IF NOT EXISTS`. | 2 | ?? REQUERIDO | Sin estas columnas ningún cliente puede ser marcado como distribuidor. Bloquea absolutamente todo el sistema de bonificación (CU6, CU7, CU8, CU9, CU13). |
| TAREA-002 | Identificar distribuidores en BD | ?? Entidades, modelos, repo y servicio | Agregar propiedades en `Customer.cs`, actualizar `CustomerRepository` y `CustomerService`. | 4 | ?? REQUERIDO | Sin el soporte en capa de acceso a datos, las columnas de TAREA-001 no son utilizables por ningún servicio. Bloquea TAREA-003 al TAREA-010. |
| TAREA-003 | Identificar distribuidores en BD | ?? Validaciones negocio `CustomerService` | Reglas `IsDistributor`/`BonusEmail`: obligatoriedad, formato email, longitud máxima. | 3 | ?? REQUERIDO | Sin validación, un distribuidor puede quedar sin `BonusEmail` y el OTP (CU6) nunca podría enviarse. Garantiza integridad de datos desde el origen. |
| TAREA-004 | Gestión visual de distribuidores | ??? Columna y filtro en `Customers.razor` | Nueva columna con badge, dropdown filtro Todos/Distribuidores/No Distribuidores. | 5 | ?? SUGERIDO | PROMOS necesita identificar visualmente qué clientes son distribuidores. Sin esto, la gestión debe hacerse abriendo cada registro individualmente. Impacto operativo alto pero no bloquea el cálculo de bonos. |
| TAREA-005 | Gestión visual de distribuidores | ??? Campos distribuidor en `EditCustomer.razor` | `RadzenCheckBox` + `RadzenTextBox` BonusEmail + validadores condicionales. | 4 | ?? REQUERIDO | Sin este formulario, PROMOS no tiene interfaz para marcar distribuidores ni configurar su email de bonificación. Prerequisito para que el sistema arranque con datos reales. |
| TAREA-006 | Gestión visual de distribuidores | ??? Campos distribuidor en `AddCustomer.razor` | Idéntico a TAREA-005 aplicado en el formulario de creación. | 3 | ?? SUGERIDO | Los distribuidores existentes se gestionan desde `EditCustomer` (TAREA-005). Un distribuidor nuevo puede crearse sin el flag y luego editarse. Reduce urgencia aunque es conveniente tenerlo desde el inicio. |
| TAREA-007 | Filtro distribuidores en reportes | ??? Filtro en `CustomerOrderReportFilter` | Checkbox + limpiar selección + reload dropdown. | 3 | ?? DESEABLE | El reporte de órdenes no es parte del flujo de bonificación. El filtro facilita encontrar distribuidores en el dropdown pero no es necesario para calcular bonos. Puede agregarse en sprint 2. |
| TAREA-008 | Filtro distribuidores en reportes | ??? Filtro en `CustomerSalesReportFilter` | Mismo patrón TAREA-007. | 2 | ?? DESEABLE | Igual a TAREA-007. Reporte de ventas no es parte del flujo de bonificación. Conveniencia visual diferible. |
| TAREA-009 | Filtro distribuidores en reportes | ??? Filtro en `CustomerReservationReportFilter` | Mismo patrón TAREA-007. | 2 | ?? DESEABLE | Igual a TAREA-007 y TAREA-008. Diferible sin impacto en el MVP. |
| TAREA-010 | Filtro distribuidores en reportes | ??? Filtro en 4 reportes adicionales | `BackOrder`, `CustomerOrderActivity`, `AutomaticAssignment`, `PendingAutomaticCustomerOrder`. | 6 | ?? DESEABLE | 4 reportes operativos que no forman parte del flujo de bonificación. Mayor cantidad de trabajo para el menor impacto en el objetivo principal. Candidato prioritario a diferir. |
| | | | **Subtotal 2.1.1** | **34** | | |

---

### 2.1.3 – Gestión de Períodos de Bonificación

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-011 | Estructura de datos de períodos | ??? Crear tablas `BonificationPeriods`, `BonificationTypes`, `BonificationPeriodInstances` | 3 tablas con PKs, FKs, constraints CHECK, índice. | 4 | ?? REQUERIDO | Sin estas tablas no existe la estructura que organiza temporalmente los cálculos de bonos. Bloquea todos los módulos de bonificación posteriores. |
| TAREA-012 | Estructura de datos de períodos | ?? Entidades EF + Configurations | 3 entidades + 3 configuraciones EF + `DbContext`. | 6 | ?? REQUERIDO | Sin las entidades EF no es posible operar las tablas desde el código. Bloquea TAREA-013 en adelante. |
| TAREA-013 | Estructura de datos de períodos | ?? Modelos de servicio + AutoMapper | 3 POCOs + mappings. | 3 | ?? REQUERIDO | Capa de desacoplamiento obligatoria en la arquitectura de Aldebaran. Sin ella los servicios no pueden operar sobre los modelos. |
| TAREA-014 | Repositorio de períodos | ?? `IBonificationPeriodRepository` + implementación | Interfaz completa + métodos de ciclo de vida. | 6 | ?? REQUERIDO | Acceso a datos de períodos e instancias. Sin esto el servicio no puede leer ni escribir en las tablas. |
| TAREA-015 | Servicio de períodos | ?? `IBonificationPeriodService` + implementación | Servicio con validaciones de negocio. | 8 | ?? REQUERIDO | Encapsula las reglas de negocio (nombre único, duración > 0, bloqueo de modificación). Sin esto no hay capa de aplicación funcional. |
| TAREA-016 | Pantalla gestión de períodos | ??? Página `BonificationPeriods.razor` | Grid paginada, buscador, row expand con instancias. | 10 | ?? REQUERIDO | PROMOS necesita una interfaz para crear y consultar períodos. Sin esta pantalla no puede operar el módulo de administración. |
| TAREA-017 | Pantalla gestión de períodos | ??? Dialog `AddBonificationPeriod.razor` | Campos + dropdown Tipo + autocompletado duración + validaciones. | 6 | ?? REQUERIDO | Sin el formulario de creación, PROMOS no puede crear ningún período desde la UI. |
| TAREA-018 | Pantalla gestión de períodos | ??? Dialog `EditBonificationPeriod.razor` | Igual a TAREA-017 + bloqueo condicional de `DurationDays`. | 5 | ?? SUGERIDO | Los períodos creados correctamente desde el inicio no necesitarán edición frecuente. Puede diferirse si los datos iniciales se cargan directamente en BD. Sin embargo es necesario para operación normal a largo plazo. |
| TAREA-019 | Ciclo de vida automático de instancias | ?? Servicio ciclo de vida + `BonificationPeriodRolloverJob` | `IBonificationPeriodInstanceLifecycleService` + job nocturno 23:59. | 15 | ?? REQUERIDO | Sin el job nocturno, las instancias de período no se crean ni se cierran automáticamente. El sistema dejaría de funcionar al vencer el primer período. Es la pieza central de la automatización. |
| | | | **Subtotal 2.1.3** | **63** | | |

---

### 2.1.4 – Gestión de Tipos de Bono

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-022 | Repositorio de tipos de bono | ?? `IBonificationTypeRepository` + implementación | `FindWithPeriodAsync`, `HasActiveInstanceAsync`, `HasActiveVigencyAsync`. | 6 | ?? REQUERIDO | El repositorio de TipoBono es consumido por el servicio de ciclo de vida (TAREA-019) y por el servicio de vigencias (TAREA-032). Sin él el sistema no puede calcular ni rotar instancias. |
| TAREA-023 | Servicio de tipos de bono | ?? `IBonificationTypeService` + implementación | Validaciones: nombre único, período activo, bloqueo si tiene instancias. | 8 | ?? REQUERIDO | Capa de negocio obligatoria. Sin ella la UI no tiene cómo crear o validar TiposBono. Bloquea TAREA-024 al TAREA-026. |
| TAREA-024 | Pantalla gestión de tipos de bono | ??? Página `BonificationTypes.razor` | Grid paginada con buscador, columnas, row expand instancias, botón "Ver Vigencias". | 8 | ?? REQUERIDO | Sin esta pantalla PROMOS no puede crear TiposBono ni navegar a sus Vigencias. Es el punto de entrada obligatorio para configurar el sistema. |
| TAREA-025 | Pantalla gestión de tipos de bono | ??? Dialog `AddBonificationType.razor` | Campos + dropdown períodos activos + chip "Ciclo N días" + validaciones. | 5 | ?? REQUERIDO | Sin el formulario de alta no es posible registrar nuevos TiposBono desde la UI. |
| TAREA-026 | Pantalla gestión de tipos de bono | ??? Dialog `EditBonificationType.razor` | Igual a TAREA-025 + bloqueo si tiene instancias activas. | 4 | ?? SUGERIDO | Si los TiposBono se configuran correctamente desde el inicio, la edición no es crítica para el MVP. La lógica de bloqueo ya vive en el servicio (TAREA-023). Puede diferirse. |
| TAREA-027 | Navegación | ??? Agregar "Tipos de Bono" al menú en `MainLayout.razor` | Subítem en el grupo "Bonificaciones" (ítem raíz, **no** dentro de Administración). | 1 | ?? REQUERIDO | Sin el ítem de menú la pantalla de Tipos de Bono (TAREA-024) no es accesible desde la navegación principal. |
| | | | **Subtotal 2.1.4** | **32** | | |

---

### 2.1.5 – Gestión de Vigencias de Bonificación

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-028 | Estructura de datos de vigencias | ??? Crear tablas `BonificationVigencies` y `BonificationVigencyRanges` | 2 tablas con PKs, FKs, constraints CHECK, índice por tipo+status. | 4 | ?? REQUERIDO | Sin estas tablas no existe la configuración de rangos y porcentajes. Sin vigencias no hay cálculo de bonos posible. |
| TAREA-029 | Estructura de datos de vigencias | ?? Entidades EF + Configurations | 2 entidades + 2 configuraciones + actualizar `BonificationType` + `DbContext`. | 5 | ?? REQUERIDO | Sin entidades EF no se puede operar con las tablas. Bloquea el repositorio y el servicio. |
| TAREA-030 | Estructura de datos de vigencias | ?? Modelos de servicio + AutoMapper | 2 POCOs + mappings. | 2 | ?? REQUERIDO | Igual que TAREA-013: capa de desacoplamiento obligatoria en la arquitectura. |
| TAREA-031 | Repositorio de vigencias | ?? `IBonificationVigencyRepository` + implementación | `FindWithRangesAsync`, `GetActiveByTypeAsync`, `UpdateStatusAsync`. | 6 | ?? REQUERIDO | Sin el repositorio el servicio no puede leer rangos ni cambiar estados. Bloquea TAREA-032. |
| TAREA-032 | Servicio de vigencias + activación | ?? `IBonificationVigencyService` + `ActivateAsync` | Validaciones: no solapamiento, PENDING editable, una ACTIVE por TipoBono, dispara `CreateFirstInstanceAsync`. | 12 | ?? REQUERIDO | `ActivateAsync` es el punto de disparo del ciclo de vida completo del sistema. Sin esta lógica nunca se crean instancias y el cálculo de bonos es imposible. Es la tarea de mayor impacto del módulo. |
| TAREA-033 | Pantalla de vigencias | ??? Página `BonificationVigencies.razor` | Breadcrumb, encabezado TipoBono, grid con badges, botón Activar condicional, row expand rangos. | 10 | ?? REQUERIDO | Sin esta pantalla PROMOS no puede ver ni activar vigencias desde la UI. Impide operar el sistema desde la interfaz web. |
| TAREA-034 | Pantalla de vigencias | ??? Dialog `AddBonificationVigency.razor` | Grilla editable inline de rangos + validación solapamiento en tiempo real + Tipo/Valor. | 12 | ?? REQUERIDO | Sin el formulario de creación PROMOS no puede configurar rangos de bono. Si no hay vigencias no hay cálculo posible. La grilla editable inline es compleja pero no puede eliminarse sin degradar gravemente la UX. |
| TAREA-035 | Pantalla de vigencias | ??? Dialog `EditBonificationVigency.razor` | Igual a TAREA-034 + modo solo lectura si estado ? PENDING. | 5 | ?? SUGERIDO | Las vigencias PENDING pueden eliminarse y recrearse en lugar de editarse. Para MVP puede omitirse el dialog de edición si PROMOS acepta crear una nueva vigencia ante cualquier corrección. |
| TAREA-036 | Navegación | ??? "Ver Vigencias" desde `BonificationTypes.razor` | Botón en columna Acciones que navega a `/bonification/types/{id}/vigencies`. | 1 | ?? REQUERIDO | Sin este botón no hay forma de llegar a la pantalla de Vigencias desde la UI (la URL directa existe pero no hay vínculo desde el listado de TiposBono). |
| | | | **Subtotal 2.1.5** | **57** | | |

---

### 2.1.6 – Vigencias de Descuentos por Total de Pedido

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-037 | Estructura de datos de descuentos | ??? Crear tablas `DiscountVigencies` y `DiscountVigencyRanges` | Estructura global: sin FK a TipoBono. Constraints VALUE_TYPE/DISCOUNT_VALUE. | 3 | ?? REQUERIDO | Sin estas tablas no existe el descuento por volumen de pedido. Según el requerimiento funcional (RF34) el descuento es parte integral del cálculo del Bono por Pedido. |
| TAREA-038 | Estructura de datos de descuentos | ?? Entidades EF + Configurations | 2 entidades + 2 configuraciones + `DbContext`. | 4 | ?? REQUERIDO | Sin entidades EF no se puede operar la tabla. Bloquea el repositorio y el servicio. |
| TAREA-039 | Estructura de datos de descuentos | ?? Modelos de servicio + AutoMapper | 2 POCOs + mappings. | 2 | ?? REQUERIDO | Capa de desacoplamiento obligatoria en la arquitectura. |
| TAREA-040 | Repositorio de descuentos | ?? `IDiscountVigencyRepository` + implementación | `GetActiveAsync()` global, `FindWithRangesAsync`, `UpdateStatusAsync`. | 5 | ?? REQUERIDO | El motor de cálculo del Bono por Pedido llama a `GetActiveAsync()` para obtener el descuento aplicable. Sin repositorio no hay cálculo. |
| TAREA-041 | Servicio de descuentos | ?? `IDiscountVigencyService` + `ActivateAsync` | Validaciones idénticas a TAREA-032 pero scope global. Sin disparo de instancias. | 8 | ?? REQUERIDO | Sin el servicio no es posible activar una vigencia de descuento. Si no hay vigencia activa el cálculo del Bono por Pedido no tiene descuento aplicable (se asume $0, lo cual puede ser un valor válido pero requiere configuración explícita). |
| TAREA-042 | Pantalla de descuentos | ??? Página `DiscountVigencies.razor` | Indicador "Vigencia Activa", grid con badges, Activar condicional, row expand rangos. | 8 | ?? REQUERIDO | Sin esta pantalla PROMOS no puede ver ni activar vigencias de descuento. El sistema no tiene interfaz para gestionar este parámetro clave. |
| TAREA-043 | Pantalla de descuentos | ??? Dialog `AddDiscountVigency.razor` | Grilla rangos editable + Tipo/Valor + validación solapamiento. | 6 | ?? REQUERIDO | Sin el formulario de creación no es posible configurar ninguna vigencia de descuento desde la UI. |
| TAREA-044 | Pantalla de descuentos | ??? Dialog `EditDiscountVigency.razor` | Igual a TAREA-043 + solo lectura si estado ? PENDING. | 4 | ?? SUGERIDO | Al igual que TAREA-035, una vigencia PENDING puede recrearse en lugar de editarse. Diferible si PROMOS acepta crear nueva vigencia ante cualquier corrección. |
| TAREA-045 | Navegación | ??? Crear menú "Bonificaciones" como ítem raíz en `MainLayout.razor` | Ítem de nivel raíz (al nivel de Administración, Movimientos de Inventario y Reportes) con subgrupos "Configuración" y "Operaciones". Incluye todos los subitems de bonificación. | 1 | ?? REQUERIDO | Sin el ítem de menú ninguna de las pantallas de Bonificación es accesible desde la navegación principal. |
| | | | **Subtotal 2.1.6** | **41** | | |

---

## Resumen Final por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 2.1.1 – Clientes Distribuidores | 10 | 34 | 14.9% |
| 2.1.3 – Períodos de Bonificación | 9 | 63 | 27.6% |
| 2.1.4 – Tipos de Bono | 6 | 32 | 14.0% |
| 2.1.5 – Vigencias de Bonificación | 9 | 57 | 25.0% |
| 2.1.6 – Vigencias de Descuentos | 9 | 41 | 18.0% |
| **TOTAL** | **43** | **228** | **100%** |

---

## Análisis de Reducción de Costo

### Tareas diferibles sin impacto en el MVP (Sugeridas + Deseables)

| # | Funcionalidad | Prioridad | Horas | Impacto si se omite |
|---|--------------|-----------|-------|---------------------|
| TAREA-004 | Columna y filtro distribuidor en `Customers.razor` | ?? SUGERIDO | 5 | PROMOS debe abrir cada cliente para verificar si es distribuidor. Operativamente incómodo pero funcional. |
| TAREA-006 | Campos distribuidor en `AddCustomer.razor` | ?? SUGERIDO | 3 | Nuevos distribuidores se crean sin el flag y se editan luego desde `EditCustomer` (TAREA-005). |
| TAREA-007 | Filtro "Solo Distribuidores" en `CustomerOrderReportFilter` | ?? DESEABLE | 3 | Reporte de órdenes no forma parte del flujo de bonificación. Sin impacto en el MVP. |
| TAREA-008 | Filtro en `CustomerSalesReportFilter` | ?? DESEABLE | 2 | Igual a TAREA-007. |
| TAREA-009 | Filtro en `CustomerReservationReportFilter` | ?? DESEABLE | 2 | Igual a TAREA-007. |
| TAREA-010 | Filtro en 4 reportes adicionales | ?? DESEABLE | 6 | Igual a TAREA-007. Mayor esfuerzo para menor impacto. |
| TAREA-018 | Dialog `EditBonificationPeriod.razor` | ?? SUGERIDO | 5 | Períodos configurados correctamente desde el inicio no requieren edición frecuente. |
| TAREA-026 | Dialog `EditBonificationType.razor` | ?? SUGERIDO | 4 | Igual a TAREA-018 para TiposBono. |
| TAREA-035 | Dialog `EditBonificationVigency.razor` | ?? SUGERIDO | 5 | Vigencias PENDING pueden recrearse ante correcciones. |
| TAREA-044 | Dialog `EditDiscountVigency.razor` | ?? SUGERIDO | 4 | Igual a TAREA-035 para descuentos. |
| **TOTAL DIFERIBLE** | | | **39** | |

### Escenarios de contratación

| Escenario | Descripción | Horas | Días hábiles |
|-----------|-------------|-------|--------------|
| **MVP Mínimo** | Solo tareas REQUERIDAS | 189 | ~23.6 |
| **MVP Recomendado** | REQUERIDAS + Sugeridas (sin Deseables) | 215 | ~26.9 |
| **Alcance Completo** | Todas las tareas (43) | 228 | ~28.5 |

> **Recomendación**: El **MVP Recomendado (215 h)** ofrece el mejor balance entre costo y experiencia de usuario. Las 4 tareas Deseables (TAREA-007 a 010) son filtros de conveniencia en reportes que no aportan al objetivo principal del proyecto y representan 13 horas diferibles con cero impacto en la funcionalidad de bonificación.

---

## Distribución por Tipo de Trabajo

| Tipo | Horas | % |
|------|-------|---|
| ??? Base de Datos (scripts SQL) | 13 | 5.7% |
| ?? Backend (entidades, repos, servicios, jobs) | 109 | 47.8% |
| ??? Frontend Blazor (páginas, dialogs, filtros) | 106 | 46.5% |
| **Total** | **228** | **100%** |
