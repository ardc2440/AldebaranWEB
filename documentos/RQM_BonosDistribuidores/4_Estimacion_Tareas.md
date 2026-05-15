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
| ?? REQUERIDO | 43 | 274 | 74.7% |
| ?? SUGERIDO | 20 | 80 | 21.8% |
| ?? DESEABLE | 4 | 13 | 3.5% |
| **TOTAL** | **67** | **367** | **100%** |

> **Reducción máxima posible** (eliminando Sugeridos + Deseables): **?93 h** ? MVP en **274 h (~34.3 días hábiles)**  
> **Reducción moderada** (eliminando solo masivos y Deseables): **?66 h** ? MVP+ en **301 h (~37.6 días hábiles)**

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

### 2.1.5 – Vigencias de Bonificación

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
| TAREA-045 | Navegación | ??? Crear menú "Bonificaciones" como ítem raíz en `MainLayout.razor` | Ítem de nivel raíz (al nivel de Administración, Movimientos de Inventario y Reportes) con subgrupos "Configuración para Bonificaciones" y "Operaciones". Incluye todos los subitems de bonificación. | 1 | ?? REQUERIDO | Sin el ítem de menú ninguna de las pantallas de Bonificación es accesible desde la navegación principal. |
| | | | **Subtotal 2.1.6** | **41** | | |

---

### 2.2.1 – Gestión de OC Especiales (Ingreso Manual)

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-046 | Estructura de datos OC Especiales | ??? Crear tabla `BonificationSpecialOrders` | Tabla con estados PENDIENTE/APROBADA/RECHAZADA, auditoría (creado/revisado por), índices por cliente+instancia y por estado. | 3 | ?? REQUERIDO | Sin esta tabla no existe soporte para los ajustes manuales de facturación. Los distribuidores con NC retroactivas quedarían excluidos de la base de cálculo real. |
| TAREA-047 | Estructura de datos OC Especiales | ?? Entidad EF `BonificationSpecialOrder` + Configuration + DbContext | Entidad con navegaciones a `Customer` y `BonificationPeriodInstance`. | 3 | ?? REQUERIDO | Sin la entidad EF no se puede operar la tabla desde el código. Bloquea repositorio y servicio. |
| TAREA-048 | Estructura de datos OC Especiales | ?? Modelo de servicio POCO + AutoMapper | 1 POCO + mapping. | 1 | ?? REQUERIDO | Capa de desacoplamiento obligatoria en la arquitectura de Aldebaran. |
| TAREA-049 | Repositorio OC Especiales | ?? `IBonificationSpecialOrderRepository` + implementación | CRUD + `GetApprovedTotalAsync` (SUM aprobadas por cliente/período) + `UpdateStatusAsync`. | 5 | ?? REQUERIDO | `GetApprovedTotalAsync` es consumido directamente por el motor de cálculo del Bono por Facturación. Sin él el cálculo ignora los ajustes manuales. |
| TAREA-050 | Servicio OC Especiales | ?? `IBonificationSpecialOrderService` + `ApproveAsync` + `RejectAsync` | Validaciones: distribuidor real, período IN_PROGRESS+BILLING, estado solo desde PENDIENTE, motivo rechazo obligatorio. | 6 | ?? REQUERIDO | Sin el servicio no hay flujo de aprobación ni validaciones de negocio. Las OC podrían impactar el cálculo sin revisión. |
| TAREA-051 | Pantalla OC Especiales | ??? Página `BonificationSpecialOrders.razor` | Grid con badges de estado, filtros por distribuidor y estado, botones Aprobar/Rechazar condicionales, row expand motivo rechazo. | 8 | ?? REQUERIDO | Sin esta pantalla PROMOS no tiene interfaz para ver ni gestionar las OC. Es el panel operativo central del módulo. |
| TAREA-052 | Pantalla OC Especiales | ??? Dialog `AddBonificationSpecialOrder.razor` | Dropdown distribuidor + dropdown período activo BILLING (se recarga al cambiar distribuidor) + monto + descripción. | 5 | ?? REQUERIDO | Sin el formulario de ingreso manual no es posible registrar ninguna OC desde la UI. |
| TAREA-053 | Pantalla OC Especiales | ??? Dialog `ApproveBonificationSpecialOrder.razor` | Modal de confirmación con resumen del impacto en el cálculo del período. | 2 | ?? REQUERIDO | Sin esta confirmación el usuario podría aprobar accidentalmente. La pantalla muestra el impacto exacto antes de confirmar. |
| TAREA-054 | Pantalla OC Especiales | ??? Dialog `RejectBonificationSpecialOrder.razor` | Modal con campo obligatorio motivo del rechazo (mín. 10 chars) + advertencia de irreversibilidad. | 2 | ?? REQUERIDO | Sin este dialog el rechazo no puede registrar el motivo, perdiendo trazabilidad de la decisión. |
| | | | **Subtotal 2.2.1** | **35** | | |

---

### 2.2.2 – Carga Masiva de OC Especiales

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-055 | Backend carga masiva | ?? `BulkAddAsync` en repositorio y servicio + modelos `BonificationSpecialOrderImportRow` y `BulkSpecialOrderResult` | Inserción en lote en una transacción. Validación fila a fila: distribuidor, período, monto, descripción. Retorna resultado con filas válidas e inválidas. | 8 | ?? SUGERIDO | El ingreso manual (TAREA-052) puede cubrir el MVP. La carga masiva es un acelerador operativo para períodos con muchos ajustes, pero no bloquea el cálculo de bonos. |
| TAREA-056 | Servicio de parseo | ?? `IBonificationSpecialOrderImportService` — parseo Excel/CSV + generación de plantilla | ClosedXML para `.xlsx`, CsvHelper para `.csv`. Validación de encabezados, parseo de filas. Generación de plantilla con 3 hojas (Plantilla, Instrucciones, Ejemplo). | 8 | ?? SUGERIDO | Depende de TAREA-055. Si no se incluye la carga masiva este servicio no es necesario. |
| TAREA-057 | Pantalla carga masiva | ??? Página `BulkBonificationSpecialOrders.razor` | Flujo en 3 pasos: descarga plantilla ? upload y procesamiento ? vista previa + confirmación. `RadzenUpload`, tablas de filas válidas/inválidas, descarga reporte de errores. | 10 | ?? SUGERIDO | Depende de TAREA-055 y TAREA-056. Sin esta pantalla la carga masiva no tiene interfaz. |
| TAREA-058 | Integración navegación | ??? Botón "Carga Masiva" en `BonificationSpecialOrders.razor` | Agregar acceso desde el listado principal. Solo visible para rol `Ingreso de OC especiales de bonificación`. | 1 | ?? SUGERIDO | Pequeño cambio que completa la integración de la carga masiva en la navegación del módulo. Sin él el usuario tendría que escribir la URL directamente. |
| | | | **Subtotal 2.2.2** | **27** | | |

---

### 2.2.3 – Conciliación de Notas Crédito

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-059 | Estructura de datos conciliación | ??? Crear tablas `CreditNoteReconciliations` y `ExternalCreditNotes` | 2 tablas. `CreditNoteReconciliations`: ajuste Valor Sistema ? Valor TOTVS, **columna calculada `DIFFERENCE` con CASE para distinguir NULL (no conciliada) de 0 (diferencia cero)**. `ExternalCreditNotes`: NC bonificadas fuera del sistema. Índices por instancia+estado. | 4 | ?? REQUERIDO | Sin estas tablas no existe soporte para el cuadre contable post-cierre. **La corrección del CASE previene errores en agregaciones que no filtran por estado**. |
| TAREA-060 | Estructura de datos conciliación | ?? Entidades EF + Configurations + DbContext | 2 entidades + 2 configuraciones. `CreditNoteReconciliation` incluye `Difference` como `HasComputedColumnSql` **con CASE WHEN para manejar NULL correctamente**. | 4 | ?? REQUERIDO | Sin entidades EF no se puede operar las tablas. Bloquea repositorios y servicios. |
| TAREA-061 | Backend NC sistema | ?? Modelo + repo + servicio `CreditNoteReconciliation` | POCO, AutoMapper, `ICreditNoteReconciliationRepository` (con `GetConciliatedDifferenceAsync` **filtrando solo STATUS='CONCILIADA'**, `BulkAddAsync`), `ICreditNoteReconciliationService` (con `ConciliateAsync`, `RejectAsync`, **`BulkProcessAsync` para procesar conciliaciones y rechazos masivos**). **Incluye regla de negocio documentada: NC sin TOTVS ? Rechazar, no conciliar con 0**. | 10 | ?? REQUERIDO | `GetConciliatedDifferenceAsync` es consumido por el indicador de cuadre. **El `BulkProcessAsync` cierra la brecha del Escenario 8 (rechazo masivo)**. +2h por complejidad adicional. |
| TAREA-062 | Backend NC externas | ?? Modelo + repo + servicio `ExternalCreditNote` | POCO, AutoMapper, `IExternalCreditNoteRepository` (con `GetApprovedTotalAsync`), `IExternalCreditNoteService` (con `ApproveAsync`, `RejectAsync`, `BulkAddAsync`). **Validación cruzada de `CreditNoteNumber` contra ambas tablas**. | 7 | ?? REQUERIDO | `GetApprovedTotalAsync` es consumido por el indicador de cuadre. Sin este servicio las NC externas no pueden registrarse ni aprobarse. |
| TAREA-063 | Pantalla principal conciliación | ??? Página `CreditNoteReconciliation.razor` con `RadzenTabs` | Dos pestañas: NC del Sistema y NC Externas. Filtros por instancia/distribuidor/estado. Indicador de cuadre con Valor Sistema + Diferencia Conciliada + NC Externas Aprobadas = Valor Final. | 12 | ?? REQUERIDO | Pantalla operativa central del módulo. Sin ella PROMOS no tiene interfaz para gestionar el cierre contable del período. |
| TAREA-064 | Dialogs conciliación manual | ??? 4 dialogs: `ConciliateCreditNote`, `RejectReconciliation`, `AddExternalCreditNote`, `ApproveRejectExternalCreditNote` | `ConciliateCreditNote` incluye preview diferencia en tiempo real, alerta si `\|dif\|>5%`, **y nota informativa guiando al usuario a Rechazar si NC no existe en TOTVS**. `ApproveRejectExternalCreditNote` reutilizable con parámetro `IsApproving`. | 9 | ?? REQUERIDO | Sin estos dialogs la operación manual NC a NC no es posible desde la UI. **La nota informativa previene errores del Escenario 4**. +1h por mensaje guía. |
| TAREA-065 | Servicio importación NC sistema | ?? `ICreditNoteReconciliationImportService` — generación de plantilla pre-poblada + parseo reimportación | `GenerateReconciliationFileAsync`: Excel con NC PENDIENTES, columnas bloqueadas, **columna Acción (CONCILIAR/RECHAZAR) + Motivo Rechazo**. `ParseFileAsync`: retorna filas con acción completada (conciliar o rechazar). **Estructura extendida con 3 columnas adicionales vs diseño original**. | 10 | ?? SUGERIDO | La conciliación manual (TAREA-064) cubre el MVP. **La masiva con rechazo (corrección aplicada) cierra el Escenario 8 completamente, pero es diferible**. +2h por columnas adicionales y lógica de validación. |
| TAREA-066 | Servicio importación NC externas | ?? `IExternalCreditNoteImportService` — plantilla en blanco + parseo | Plantilla de 4 columnas + hojas Instrucciones/Ejemplo. Parseo valida estructura y tipos. | 5 | ?? SUGERIDO | Depende de TAREA-062. Si no se incluye la carga masiva de NC externas este servicio no es necesario para el MVP. |
| TAREA-067 | Página conciliación masiva NC sistema | ??? Página `BulkCreditNoteReconciliation.razor` | Flujo 3 pasos: generar archivo pre-poblado ? reimportar ? **vista previa con 4 tablas (A Conciliar / A Rechazar / Ignoradas / Errores)** + confirmación. **Llama `BulkProcessAsync` en lugar de `BulkConciliateAsync`**. | 10 | ?? SUGERIDO | Depende de TAREA-065. **Con las correcciones, cubre el Escenario 8 completo (rechazo masivo)**. Diferible sin impacto en MVP. +2h por tablas adicionales. |
| TAREA-068 | Página carga masiva NC externas | ??? Página `BulkExternalCreditNotes.razor` | Flujo 3 pasos: descargar plantilla en blanco ? cargar y procesar ? vista previa + confirmación. | 6 | ?? SUGERIDO | Depende de TAREA-066. Mismo argumento que TAREA-067: diferible sin impacto en el MVP. |
| TAREA-069 | Navegación | ??? Agregar "Conciliación de NC" al menú Operaciones en `MainLayout.razor` | Subítem dentro del grupo "Operaciones" del menú "Bonificaciones". | 1 | ?? REQUERIDO | Sin el ítem de menú la pantalla de Conciliación no es accesible desde la navegación principal. |
| | | | **Subtotal 2.2.3** | **78** | | **Incremento de +7h vs estimación original por correcciones de brechas (TAREA-061: +2h, TAREA-064: +1h, TAREA-065: +2h, TAREA-067: +2h)** |

---

## Resumen Final por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 2.1.1 – Clientes Distribuidores | 10 | 34 | 9.4% |
| 2.1.3 – Períodos de Bonificación | 9 | 63 | 17.5% |
| 2.1.4 – Tipos de Bono | 6 | 32 | 8.9% |
| 2.1.5 – Vigencias de Bonificación | 9 | 57 | 15.8% |
| 2.1.6 – Vigencias de Descuentos | 9 | 41 | 11.1% |
| 2.2.1 – OC Especiales (Manual) | 9 | 35 | 9.5% |
| 2.2.2 – OC Especiales (Masivo) | 4 | 27 | 7.3% |
| 2.2.3 – Conciliación de NC | 11 | 78 | 21.1% |
| **TOTAL** | **67** | **367** | **100%** |

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
| TAREA-055 | `BulkAddAsync` + modelos de importación OC | ?? SUGERIDO | 8 | El ingreso manual cubre el MVP. La carga masiva de OC es un acelerador diferible. |
| TAREA-056 | Servicio de parseo OC Excel/CSV | ?? SUGERIDO | 8 | Depende de TAREA-055. Diferible junto con ella. |
| TAREA-057 | Página carga masiva OC `BulkBonificationSpecialOrders.razor` | ?? SUGERIDO | 10 | Depende de TAREA-055 y TAREA-056. Diferible junto con ellas. |
| TAREA-058 | Botón "Carga Masiva" en listado OC | ?? SUGERIDO | 1 | Solo relevante si se implementan TAREA-055 a 057. |
| TAREA-065 | Servicio generación plantilla + parseo reimportación NC | ?? SUGERIDO | 10 | La conciliación manual cubre el MVP. La masiva es un acelerador diferible. **+2h por columnas Acción/Motivo Rechazo**. |
| TAREA-066 | Servicio importación NC externas | ?? SUGERIDO | 5 | Depende de TAREA-062. Diferible junto con TAREA-068. |
| TAREA-067 | Página conciliación masiva NC sistema | ?? SUGERIDO | 10 | Depende de TAREA-065. Diferible. **+2h por tablas de resultado adicionales (A Conciliar/A Rechazar/Ignoradas/Errores)**. |
| TAREA-068 | Página carga masiva NC externas | ?? SUGERIDO | 6 | Depende de TAREA-066. Diferible. |
| **TOTAL DIFERIBLE** | | | **97** | **+4h vs estimación original por correcciones** |

### Escenarios de contratación

| Escenario | Descripción | Horas | Días hábiles |
|-----------|-------------|-------|--------------|
| **MVP Mínimo** | Solo tareas REQUERIDAS | 274 | ~34.3 |
| **MVP Recomendado** | REQUERIDAS + Sugeridas sin masivos (OC + NC) + sin Deseables | 301 | ~37.6 |
| **Alcance Completo** | Todas las tareas (67) | 367 | ~45.9 |
| **Alcance Completo** | Todas las tareas (67) | 360 | ~45.0 |

> **Recomendación**: El **MVP Mínimo (274 h)** cubre el ciclo completo de bonificación operativo:
> configuración, cálculo, gestión de OC especiales (manual) y conciliación de NC (manual).
> Los caminos masivos (OC + NC, 50 h en total) se recomiendan como **Fase 2** una vez el sistema
> esté en producción y se valide el volumen real de registros por período.
>
> **Nota sobre correcciones aplicadas**: Se agregaron +7 h en tareas REQUERIDAS (TAREA-061: +2h, TAREA-064: +1h)
> y +4 h en tareas SUGERIDAS (TAREA-065: +2h, TAREA-067: +2h) para cubrir los Escenarios 4 y 8 de conciliación
> (NC sin TOTVS en camino manual y masivo). Estas correcciones cierran brechas críticas que impedirían
> el rechazo de NC inexistentes en TOTVS.

---

## Resumen Ejecutivo por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| ?? REQUERIDO | 43 | 267 | 74.2% |
| ?? SUGERIDO | 20 | 80 | 22.2% |
| ?? DESEABLE | 4 | 13 | 3.6% |
| **TOTAL** | **67** | **360** | **100%** |

---

## Distribución por Tipo de Trabajo

| Tipo | Horas | % |
|------|-------|---|
| ??? Base de Datos (scripts SQL) | 20 | 5.4% |
| ?? Backend (entidades, repos, servicios, jobs, import) | 176 | 48.0% |
| ??? Frontend Blazor (páginas, dialogs, filtros) | 171 | 46.6% |
| **Total** | **367** | **100%** |
