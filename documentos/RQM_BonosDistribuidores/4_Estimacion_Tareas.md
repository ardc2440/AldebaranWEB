# 4. ESTIMACIÓN DE TAREAS - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Fecha**: Mayo 2026  
**Versión**: 2.2  
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
| ?? REQUERIDO | 65 | 369 | 77.0% |
| ?? SUGERIDO | 24 | 97 | 20.3% |
| ?? DESEABLE | 4 | 13 | 2.7% |
| **TOTAL** | **93** | **479** | **100%** |

> **Reducción máxima posible** (eliminando Sugeridos + Deseables): **?110 h** ? MVP en **369 h (~46.1 días hábiles)**  
> **Reducción moderada** (eliminando solo masivos y Deseables): **?71 h** ? MVP+ en **408 h (~51.0 días hábiles)**

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
| | | | **Subtotal 2.1.1** | **21** | | |

---

### 2.1.2 – Reportes con filtro de Cliente

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-007 | Filtro distribuidores en reportes | ??? Filtro en `CustomerOrderReportFilter` | Checkbox + limpiar selección + reload dropdown. | 3 | ?? DESEABLE | El reporte de órdenes no es parte del flujo de bonificación. El filtro facilita encontrar distribuidores en el dropdown pero no es necesario para calcular bonos. Puede agregarse en sprint 2. |
| TAREA-008 | Filtro distribuidores en reportes | ??? Filtro en `CustomerSalesReportFilter` | Mismo patrón TAREA-007. | 2 | ?? DESEABLE | Igual a TAREA-007. Reporte de ventas no es parte del flujo de bonificación. Conveniencia visual diferible. |
| TAREA-009 | Filtro distribuidores en reportes | ??? Filtro en `CustomerReservationReportFilter` | Mismo patrón TAREA-007. | 2 | ?? DESEABLE | Igual a TAREA-007 y TAREA-008. Diferible sin impacto en el MVP. |
| TAREA-010 | Filtro distribuidores en reportes | ??? Filtro en 4 reportes adicionales | `BackOrder`, `CustomerOrderActivity`, `AutomaticAssignment`, `PendingAutomaticCustomerOrder`. | 6 | ?? DESEABLE | 4 reportes operativos que no forman parte del flujo de bonificación. Mayor cantidad de trabajo para el menor impacto en el objetivo principal. Candidato prioritario a diferir. |
| | | | **Subtotal 2.1.2** | **13** | | |

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
| TAREA-020 | Cancelada por definición | | | | | |
| TAREA-021 | Cancelada por definición | | | | | |
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
| TAREA-065 | Servicio importación NC sistema | ?? `ICreditNoteReconciliationImportService` — generación de plantilla pre-poblada + parseo reimportación | `GenerateReconciliationFileAsync`: Excel con NC PENDIENTES, columnas bloqueadas, **columna Acción (CONCILIAR/RECHAZAR) + Motivo Rechazo**. `ParseFileAsync`: retorna filas con acción completada (conciliar o rechazar). **Estructura extendida con 3 columnas adicionales vs diseño original**. | 10 | ?? SUGERIDO | La conciliación manual (TAREA-064) cubre el MVP. La masiva con rechazo (corrección aplicada) cierra el Escenario 8 completamente, pero es diferible. +2h por columnas adicionales y lógica de validación. |
| TAREA-066 | Servicio importación NC externas | ?? `IExternalCreditNoteImportService` — plantilla en blanco + parseo | Plantilla de 4 columnas + hojas Instrucciones/Ejemplo. Parseo valida estructura y tipos. | 5 | ?? SUGERIDO | Depende de TAREA-062. Si no se incluye la carga masiva de NC externas este servicio no es necesario para el MVP. |
| TAREA-067 | Página conciliación masiva NC sistema | ??? Página `BulkCreditNoteReconciliation.razor` | Flujo 3 pasos: generar archivo pre-poblado ? reimportar ? **vista previa con 4 tablas (A Conciliar / A Rechazar / Ignoradas / Errores)** + confirmación. **Llama `BulkProcessAsync` en lugar de `BulkConciliateAsync`**. | 10 | ?? SUGERIDO | Depende de TAREA-065. **Con las correcciones, cubre el Escenario 8 completo (rechazo masivo)**. Diferible sin impacto en MVP. +2h por tablas adicionales. |
| TAREA-068 | Página carga masiva NC externas | ??? Página `BulkExternalCreditNotes.razor` | Flujo 3 pasos: descargar plantilla en blanco ? cargar y procesar ? vista previa + confirmación. | 6 | ?? SUGERIDO | Depende de TAREA-066. Mismo argumento que TAREA-067: diferible sin impacto en el MVP. |
| TAREA-069 | Navegación | ??? Agregar "Conciliación de NC" al menú Operaciones en `MainLayout.razor` | Subítem dentro del grupo "Operaciones" del menú "Bonificaciones". | 1 | ?? REQUERIDO | Sin el ítem de menú la pantalla de Conciliación no es accesible desde la navegación principal. |
| | | | **Subtotal 2.2.3** | **78** | | **Incremento de +7h vs estimación original por correcciones de brechas (TAREA-061: +2h, TAREA-064: +1h, TAREA-065: +2h, TAREA-067: +2h)** |

---

### 2.2.4 – Lista de Precios Promocional

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-070 | Estructura de datos | ??? Crear tablas `PromotionalPriceLists` + `PromotionalPriceListItems` | 2 tablas. `PromotionalPriceLists`: encabezado con fecha, estado (ACTIVE/HISTORICAL), fuente (AUTOMATIC/MANUAL). `PromotionalPriceListItems`: 13 columnas del Excel (Referencia, Nombre, Características, DescPrecio1-5, Precio1-5). Índices por fecha+estado y código de artículo. | 3 | ?? REQUERIDO | Sin estas tablas no existe soporte para almacenar precios del día. El cálculo de Bono por Facturación queda bloqueado. |
| TAREA-071 | Estructura de datos | ?? Entidades EF + Configurations + Models + Mappings | 2 entidades + 2 configuraciones + 2 modelos POCO. Mapeo completo de las 13 columnas. Registrar en DbContext. | 5 | ?? REQUERIDO | Sin entidades EF no se puede operar las tablas. Bloquea repositorios y servicios. |
| TAREA-072 | Backend base | ?? `IPromotionalPriceListRepository` + `IPromotionalPriceListService` | Repositorio con `GetActiveForDateAsync`, `GetMostRecentActiveAsync`, `LoadDayListAsync` (archiva anterior + **REEMPLAZA** del mismo día), `GetItemPriceAsync`. Servicio con validaciones. **+2h por lógica de REEMPLAZO (DELETE en lugar de historizar), testing de escenarios intra-día, y Application Logs para mitigación de auditoría**. | 9 | ?? REQUERIDO | `LoadDayListAsync` es consumido por el worker automático y la carga manual. `GetItemPriceAsync` es consumido por el motor de cálculo de bonificación. **Decisión arquitectural: foto única por día (no versiones intra-día)**. |
| TAREA-073 | Descarga automática | ?? `IPriceListFetchService` — descarga HTTP + parseo Excel | Descarga desde URL configurable (`https://www.catalogospromocionales.com/distribuidores/referenciasexcel`). **Doble estrategia**: descarga directa (sin auth) y descarga autenticada (login previo con user/pass). Parseo con ClosedXML de 13 columnas. | 8 | ?? REQUERIDO | Sin este servicio el worker automático no puede descargar la lista del día. **La doble estrategia previene bloqueos futuros si el proveedor cambia política de acceso**. +2h por complejidad de autenticación. |
| TAREA-074 | Descarga automática | ?? `PriceListFetchWorker` — job programado (BackgroundService) | Worker con NCrontab (hora configurable, default 6 AM). Descarga ? parseo ? `LoadDayListAsync` ? notificación email (éxito/fallo). Usa `ResilientExecutor` para reintentos. Configuración en `appsettings.json` (URL, UseAuthentication, Username, Password, CronExpression, NotificationRecipients). Registrar HttpClient + Worker en DI. | 6 | ?? REQUERIDO | Sin el worker automático, PROMOS debe cargar manualmente la lista todos los días a las 6 AM. La notificación email es crítica para detectar fallos inmediatamente. |
| TAREA-075 | Contingencia manual | ?? `IPromotionalPriceListImportService` — parseo archivo manual | Parsea archivo Excel subido manualmente. Reutiliza lógica de parseo de TAREA-073 (extracción a clase `PriceListParser`). | 3 | ?? REQUERIDO | Sin este servicio no hay contingencia manual ante fallo del worker o ajustes dentro del día. |
| TAREA-076 | Contingencia manual | ??? Página `PromotionalPriceLists.razor` + carga manual | Indicador de estado (lista activa hoy / usando lista del día X / sin lista). Grilla de historial con row expand (ítems). Sección de contingencia: `RadzenUpload` + campo Notas obligatorio + preview + confirmación ("Reemplazará lista activa"). Link "Cargar manual" desde alertas. | 12 | ?? REQUERIDO | Sin interfaz de contingencia, ante fallo del worker el sistema queda sin lista de precios y el cálculo no puede ejecutarse. |
| TAREA-077 | Alertas administrativas | ??? Notificación en Dashboard si no hay lista activa | Verifica en `OnInitializedAsync` del Dashboard si `GetActiveForTodayAsync() == null`. Si no hay, muestra `RadzenAlert` persistente con link a `/bonification/price-lists`. Solo visible para roles de administración/bonificación. | 2 | ?? REQUERIDO | Sin esta alerta, el administrador solo descubre la falta de lista cuando el cálculo falla. Prevención proactiva crítica. |
| TAREA-078 | Navegación | ??? Agregar "Lista de Precios" al menú Configuración en `MainLayout.razor` | Subítem dentro del grupo "Configuración para Bonificaciones" del menú "Bonificaciones". | 1 | ?? REQUERIDO | Sin el ítem de menú la pantalla no es accesible desde la navegación principal. |
| | | | **Subtotal 2.2.4** | **49** | | **Módulo completo REQUERIDO — sin lista de precios no hay cálculo de Bono por Facturación. +3h vs estimación original por lógica de REEMPLAZO (TAREA-072 +2h redondeadas a 9h totales)** |

---

### 2.2.5 – Gestión de Exclusiones (Pedido Especial)

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-079 | Exclusión a nivel de datos | ??? Agregar `IsSpecialOrder` en `CUSTOMER_ORDERS` | Script de migración con default `0` y backfill para históricos. | 2 | ?? REQUERIDO | Sin esta columna no existe forma técnica de marcar pedidos especiales y excluirlos del Bono por Pedido. |
| TAREA-080 | Auditoría específica del flag | ??? Estructura de log explícito para cambios de flag | Tabla de trazabilidad con valor anterior/nuevo, dirección del cambio, usuario y causa. | 3 | ?? REQUERIDO | La auditoría estándar no cubre explícitamente el sentido del cambio; esta evidencia es obligatoria para control interno. |
| TAREA-081 | Exclusión en consultas y reportes | ??? Ajuste de SPs y consultas impactadas | Actualizar datasets de `Customer Orders`, `Customer Orders Activities` y `Customer Sales` con bandera y exclusión dinámica en cálculo. | 5 | ?? REQUERIDO | Si no se actualizan consultas, el flag no tiene efecto real en el cálculo ni visibilidad operacional. |
| TAREA-082 | Dominio de pedidos | ?? Agregar `IsSpecialOrder` en entidades y modelos | Propagar campo en `CustomerOrder` (DataAccess/Application) y modelos de salida asociados. | 2 | ?? REQUERIDO | Sin propagación de modelo, el backend no puede leer ni persistir correctamente la bandera. |
| TAREA-083 | Mapeo transversal | ?? Actualizar configuraciones EF y AutoMapper | Ajustar mapeo de columna y perfil de transformación entre capas. | 2 | ?? REQUERIDO | Garantiza consistencia entre BD, entidades y servicios; evita omisiones silenciosas del nuevo campo. |
| TAREA-084 | Operación especializada | ?? Método dedicado `UpdateSpecialOrderFlagAsync` | Crear operación específica para cambiar solo el flag con captura de causa. | 5 | ?? REQUERIDO | Soporta el nuevo perfil funcional que solo puede modificar esta bandera y nada más del pedido. |
| TAREA-085 | Reglas de elegibilidad | ?? Validaciones de negocio del flag | Solo distribuidores (`IsDistributor=true`), bloqueo por período cerrado y mensajes funcionales controlados. | 4 | ?? REQUERIDO | Evita inconsistencias funcionales y protege reglas acordadas del proceso de bonificación. |
| TAREA-086 | Seguridad en update general | ?? Blindaje de `UpdateAsync` de pedido | Impedir que el flujo tradicional modifique `IsSpecialOrder` cuando no corresponde. | 3 | ?? REQUERIDO | Separa claramente responsabilidades entre perfil actual de modificación y nuevo perfil especializado. |
| TAREA-087 | Motor de cálculo | ?? Exclusión efectiva en Bono por Pedido | Ajustar consultas de consolidación para excluir pedidos con `IsSpecialOrder=true`. | 4 | ?? REQUERIDO | Es el objetivo funcional central de la historia; sin esto el flag sería decorativo. |
| TAREA-088 | Roles y autorización | ?? Nuevo permiso `Administración de Pedidos Especiales` | Alta de rol/permiso, asignación y políticas en backend/frontend. | 3 | ?? REQUERIDO | Sin control de permisos no se cumple la segregación de funciones definida por análisis. |
| TAREA-089 | UI operativa de pedido | ??? Visualización controlada de `Pedido Especial` | Mostrar campo en edición de pedido con habilitación condicional por rol y estado del período. | 5 | ?? REQUERIDO | La operación diaria requiere una interfaz directa para administrar la exclusión en pedidos existentes. |
| TAREA-090 | Flujo exclusivo de cambio | ??? Dialog/acción dedicada para el flag | Confirmación explícita de impacto + selección de causa para cambio del flag. | 4 | ?? REQUERIDO | Reduce riesgo operativo y estandariza el cambio sobre una acción auditable y controlada. |
| TAREA-091 | Reporte operacional | ??? Ajuste `Customer Orders` | Columna/filtro de `Pedido Especial` y propagación en exportables. | 4 | ?? SUGERIDO | Mejora visibilidad y control operativo, pero no bloquea la exclusión del cálculo en sí misma. |
| TAREA-092 | Reporte de actividades | ??? Ajuste `Customer Orders Activities` | Incluir indicador/filtro de `Pedido Especial` en consulta y exportación. | 4 | ?? SUGERIDO | Aporta trazabilidad analítica; puede diferirse sin bloquear la lógica principal de exclusión. |
| TAREA-093 | Reporte comercial | ??? Ajuste `Customer Sales` | Incluir indicador/filtro de `Pedido Especial` en grilla y export. | 4 | ?? SUGERIDO | Incrementa capacidad de análisis del negocio; no impide operación mínima del módulo. |
| TAREA-094 | Auditoría integral | ?? Integrar auditoría estándar + log específico | Conectar modificación de pedido con log adicional obligatorio del flag. | 4 | ?? REQUERIDO | Garantiza cumplimiento de requerimiento de auditoría dual (estándar + explícita). |
| TAREA-095 | Validación técnica | ?? Plan de pruebas de regresión del módulo | Cobertura unitaria/integración/UI para rol, regla de distribuidor, período cerrado y exclusión en cálculo. | 5 | ?? SUGERIDO | Aumenta confiabilidad de salida y reduce retrabajo post-release; puede ejecutarse en cierre de sprint. |
| | | | **Subtotal 2.2.5** | **63** | | **Nuevo módulo de control transversal sobre pedidos; +63h al total del proyecto.** |

---

## Resumen Final por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 2.1.1 – Clientes Distribuidores | 6 | 21 | 4.4% |
| 2.1.2 – Reportes con filtro de Cliente | 4 | 13 | 2.7% |
| 2.1.3 – Períodos de Bonificación | 9 | 63 | 13.2% |
| 2.1.4 – Tipos de Bono | 6 | 32 | 6.7% |
| 2.1.5 – Vigencias de Bonificación | 9 | 57 | 11.9% |
| 2.1.6 – Vigencias de Descuentos | 9 | 41 | 8.6% |
| 2.2.1 – OC Especiales (Manual) | 9 | 35 | 7.3% |
| 2.2.2 – OC Especiales (Masivo) | 4 | 27 | 5.6% |
| 2.2.3 – Conciliación de NC | 11 | 78 | 16.3% |
| 2.2.4 – Lista de Precios Promocional | 9 | 49 | 10.2% |
| 2.2.5 – Gestión de Exclusiones (Pedido Especial) | 17 | 63 | 13.2% |
| **TOTAL** | **93** | **479** | **100%** |

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
| TAREA-091 | Ajuste reporte `Customer Orders` | ?? SUGERIDO | 4 | La lógica de exclusión se mantiene operativa sin esta visualización de reporte en una primera salida. |
| TAREA-092 | Ajuste reporte `Customer Orders Activities` | ?? SUGERIDO | 4 | Misma justificación que TAREA-091; aporta visibilidad operativa no crítica para el MVP. |
| TAREA-093 | Ajuste reporte `Customer Sales` | ?? SUGERIDO | 4 | Misma justificación que TAREA-091; analítica diferible. |
| TAREA-095 | Plan de pruebas extendido del módulo 2.2.5 | ?? SUGERIDO | 5 | Recomendado para endurecer salida, pero puede ejecutarse al cierre de estabilización. |
| **TOTAL DIFERIBLE** | | | **114** | **Incluye +17h del módulo 2.2.5 (reportería y pruebas extendidas).** |

### Escenarios de contratación

| Escenario | Descripción | Horas | Días hábiles |
|-----------|-------------|-------|--------------|
| **MVP Mínimo** | Solo tareas REQUERIDAS | 369 | ~46.1 |
| **MVP Recomendado** | REQUERIDAS + Sugeridas sin masivos (OC + NC) + sin Deseables | 408 | ~51.0 |
| **Alcance Completo** | Todas las tareas (93) | 479 | ~59.9 |

> **Recomendación**: El **MVP Mínimo (369 h)** cubre el ciclo completo de bonificación operativo,
> incluida la exclusión por `Pedido Especial` con control de rol y auditoría reforzada.
> Los caminos masivos (OC + NC, 58 h en total) y la reportería extendida de exclusiones
> se recomiendan como **Fase 2** una vez estabilizado el flujo principal.
>
> **Nota sobre correcciones aplicadas (v2.0)**: Se agregaron +7 h en tareas REQUERIDAS (TAREA-061: +2h, TAREA-064: +1h)
> y +4 h en tareas SUGERIDAS (TAREA-065: +2h, TAREA-067: +2h) para cubrir los Escenarios 4 y 8 de conciliación
> (NC sin TOTVS en camino manual y masivo).
>
> **Agregado Lista de Precios Promocional (v2.0)**: +46 h en 9 tareas REQUERIDAS (TAREA-070 a 078). Sin este módulo no es posible
> calcular el Bono por Facturación. Incluye descarga automática con 2 estrategias (directa y autenticada), contingencia manual,
> y notificación de fallo.
>
> **Ajuste lógica de REEMPLAZO (v2.1)**: +3 h en módulo 2.2.4 (TAREA-072 de 7h a 9h). Decisión arquitectural: lista de precios del mismo día
> se REEMPLAZA (no historiza versión anterior). Incluye testing adicional de escenarios intra-día, Application Logs para mitigación
> de auditoría, y documentación de decisión.
>
> **Agregado Gestión de Exclusiones – Pedido Especial (v2.2)**: +63 h en 17 tareas (TAREA-079 a TAREA-095),
> con foco en exclusión de Bono por Pedido, control de permisos dedicado, auditoría dual y cobertura operativa en frontend/reportes.
> Total proyecto actualizado: **479 h**.

---

## Resumen Ejecutivo por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| ?? REQUERIDO | 65 | 369 | 77.0% |
| ?? SUGERIDO | 24 | 97 | 20.3% |
| ?? DESEABLE | 4 | 13 | 2.7% |
| **TOTAL** | **93** | **479** | **100%** |

> **Reducción máxima posible** (eliminando Sugeridos + Deseables): **?110 h** ? MVP en **369 h (~46.1 días hábiles)**  
> **Reducción moderada** (eliminando solo masivos y Deseables): **?71 h** ? MVP+ en **408 h (~51.0 días hábiles)**

---

## Distribución por Tipo de Tarea

| Tipo | Horas | % |
|------|-------|---|
| ??? Base de Datos (scripts SQL) | 33 | 6.9% |
| ?? Backend (entidades, repos, servicios, jobs, import) | 259 | 54.1% |
| ??? Frontend Blazor (páginas, dialogs, filtros) | 187 | 39.0% |
| **Total** | **479** | **100%** |

---

## Distribución por Tipo de Trabajo

| Tipo | Horas | % |
|------|-------|---|
| ??? Base de Datos (scripts SQL) | 33 | 6.9% |
| ?? Backend (entidades, repos, servicios, jobs, import) | 259 | 54.1% |
| ??? Frontend Blazor (páginas, dialogs, filtros) | 187 | 39.0% |
| **Total** | **479** | **100%** |
