# 4. ESTIMACIÓN DE TAREAS - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Fecha**: Mayo 2026  
**Versión**: 3.0  
**Perfil asumido**: Analista, Diseñador, Arquitecto y Desarrollador Senior .NET / Blazor con conocimiento del codebase Aldebaran

---

## Criterios de Estimación

| Factor | Criterio |
|--------|----------|
| **Perfil** | Analista + Diseñador + Arquitecto + Desarrollador Senior con experiencia en el stack (C#, EF, Blazor, Radzen, SQL Server) |
| **Familiaridad** | Conoce el codebase Aldebaran (patrones Repository, Service, Radzen UI) |
| **Unidad** | Horas de desarrollo efectivo (incluye análisis, diseño, arquitectura, codificación) |
| **Incluye** | Análisis detallado, diseño técnico, arquitectura de solución, codificación, prueba unitaria básica, ajustes por feedback inmediato |
| **No incluye** | QA formal, pruebas de integración, documentación adicional, despliegue |

---

## Leyenda de Prioridad

| Prioridad | Significado |
|-----------|-------------|
| ?? **REQUERIDO** | El sistema no funciona o no cumple el objetivo principal sin esta tarea. Su omisión bloquea otras tareas o casos de uso críticos. |
| ?? **SUGERIDO** | Mejora la operabilidad o la experiencia de usuario de forma significativa, pero el sistema puede funcionar sin ella en una primera versión. |
| ?? **DESEABLE** | Agrega comodidad, eficiencia o cobertura adicional. Puede diferirse a una segunda iteración sin impacto en el MVP. |

---

## Resumen de Tareas por Módulo

### 2.1.1 – Configuración de Clientes Distribuidores

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-001 | Agregar columnas `IsDistributor` y `BonusEmail` a la tabla `Customers` | Script de migración SQL | 2 | ?? REQUERIDO |
| TAREA-002 | Agregar propiedades `IsDistributor` y `BonusEmail` a las entidades y modelos | Entidades, modelos, repositorio y servicio | 4 | ?? REQUERIDO |
| TAREA-003 | Agregar validación de negocio en `CustomerService` | Reglas para `IsDistributor` y `BonusEmail` | 3 | ?? REQUERIDO |
| TAREA-004 | Agregar columna "Es Distribuidor" en el listado `Customers.razor` | Columna con badge, filtro por distribuidor | 5 | ?? SUGERIDO |
| TAREA-005 | Agregar campos de distribuidor en el formulario `EditCustomer.razor` | CheckBox + TextBox con validadores | 4 | ?? REQUERIDO |
| TAREA-006 | Agregar campos de distribuidor en el formulario `AddCustomer.razor` | Idéntico a TAREA-005 | 3 | ?? SUGERIDO |
| | | **Subtotal 2.1.1** | **21** | |

---

### 2.1.2 – Reportes con filtro de Cliente

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-007 | Agregar filtro "Solo Distribuidores" en `CustomerOrderReportFilter` | Checkbox + reload dropdown | 3 | ?? DESEABLE |
| TAREA-008 | Agregar filtro "Solo Distribuidores" en `CustomerSalesReportFilter` | Mismo patrón TAREA-007 | 2 | ?? DESEABLE |
| TAREA-009 | Agregar filtro "Solo Distribuidores" en `CustomerReservationReportFilter` | Mismo patrón TAREA-007 | 2 | ?? DESEABLE |
| TAREA-010 | Agregar filtro "Solo Distribuidores" en 4 reportes adicionales | BackOrder, CustomerOrderActivity, AutomaticAssignment, PendingAutomatic | 6 | ?? DESEABLE |
| | | **Subtotal 2.1.2** | **13** | |

---

### 2.1.3 – Gestión de Períodos de Bonificación

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-011 | Crear tablas `BonificationPeriods`, `BonificationTypes` y `BonificationPeriodInstances` | 3 tablas con PKs, FKs, constraints | 4 | ?? REQUERIDO |
| TAREA-012 | Crear entidades EF: `BonificationPeriod`, `BonificationType` y `BonificationPeriodInstance` | 3 entidades + 3 configuraciones | 6 | ?? REQUERIDO |
| TAREA-013 | Crear modelos de servicio | 3 POCOs + mappings AutoMapper | 3 | ?? REQUERIDO |
| TAREA-014 | Crear `IBonificationPeriodRepository` y `BonificationPeriodRepository` | Interfaz + implementación | 6 | ?? REQUERIDO |
| TAREA-015 | Crear `IBonificationPeriodService` y `BonificationPeriodService` | Servicio con validaciones | 8 | ?? REQUERIDO |
| TAREA-016 | Crear página de listado `BonificationPeriods.razor` | Grid paginada con row expand | 10 | ?? REQUERIDO |
| TAREA-017 | Crear dialog `AddBonificationPeriod.razor` | Formulario con validaciones | 6 | ?? REQUERIDO |
| TAREA-018 | Crear dialog `EditBonificationPeriod.razor` | Formulario con bloqueo condicional | 5 | ?? SUGERIDO |
| TAREA-019 | Crear servicio de ciclo de vida automático de instancias | Servicio + Job nocturno | 15 | ?? REQUERIDO |
| TAREA-020 | [CANCELADA] Gestión de sesiones de bonificación | Diferida a Fase 2 | 0 | N/A |
| TAREA-021 | [CANCELADA] Auditoría detallada de ciclo de vida | Se usa Application Insights | 0 | N/A |
| TAREA-022 | Crear `IBonificationTypeRepository` y `BonificationTypeRepository` | Repositorio completo | 6 | ?? REQUERIDO |
| TAREA-023 | Crear `IBonificationTypeService` y `BonificationTypeService` | Servicio con validaciones | 8 | ?? REQUERIDO |
| TAREA-024 | Crear página de listado `BonificationTypes.razor` | Grid paginada con acciones | 8 | ?? REQUERIDO |
| TAREA-025 | Crear dialog `AddBonificationType.razor` | Formulario con validaciones | 5 | ?? REQUERIDO |
| TAREA-026 | Crear dialog `EditBonificationType.razor` | Formulario con bloqueo | 4 | ?? SUGERIDO |
| TAREA-027 | Actualizar DataGrids con columna de Acciones | Agregar botón editar | 2 | ?? REQUERIDO |
| TAREA-028 | Crear vista de cierre de período (CU10) | Command + Protobuf | 6 | ?? REQUERIDO |
| | | **Subtotal 2.1.3** | **102** | |

---

### 2.1.5 – Gestión de Rangos y Porcentajes de Bonificación (Vigencias)

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-029 | Crear tabla `BonificationRanges` | Tabla con rangos y porcentajes | 3 | ?? REQUERIDO |
| TAREA-030 | Crear entidad EF: `BonificationRange` | Entidad + Configuration | 3 | ?? REQUERIDO |
| TAREA-031 | Crear modelo de servicio: `BonificationRange` | POCO + mapping | 2 | ?? REQUERIDO |
| TAREA-032 | Crear `IBonificationRangeRepository` y `BonificationRangeRepository` | Repositorio completo | 5 | ?? REQUERIDO |
| TAREA-033 | Crear `IBonificationRangeService` y `BonificationRangeService` | Servicio con validaciones | 6 | ?? REQUERIDO |
| TAREA-034 | Crear página de listado `BonificationRanges.razor` | Grid paginada | 8 | ?? REQUERIDO |
| TAREA-035 | Crear dialog `EditBonificationVigency.razor` | Formulario con edición condicional | 8 | ?? REQUERIDO |
| TAREA-036 | Agregar botón "Ver Vigencias" en `BonificationTypes.razor` | Navegación a vigencias | 1 | ?? REQUERIDO |
| | | **Subtotal 2.1.5** | **36** | |

---

### 2.1.6 – Vigencias de Descuentos por Total de Pedido

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-037 | Crear tablas `DiscountVigencies` y `DiscountVigencyRanges` | 2 tablas globales | 3 | ?? REQUERIDO |
| TAREA-038 | Crear entidades EF: `DiscountVigency` y `DiscountVigencyRange` | 2 entidades + configuraciones | 4 | ?? REQUERIDO |
| TAREA-039 | Crear modelos de servicio | 2 POCOs + mappings | 2 | ?? REQUERIDO |
| TAREA-040 | Crear `IDiscountVigencyRepository` y `DiscountVigencyRepository` | Repositorio global | 5 | ?? REQUERIDO |
| TAREA-041 | Crear `IDiscountVigencyService` y `DiscountVigencyService` | Servicio con validaciones | 8 | ?? REQUERIDO |
| TAREA-042 | Crear página de listado `DiscountVigencies.razor` | Grid con indicador activo | 8 | ?? REQUERIDO |
| TAREA-043 | Crear dialog `AddDiscountVigency.razor` | Formulario con grilla editable | 6 | ?? REQUERIDO |
| TAREA-044 | Crear dialog `EditDiscountVigency.razor` | Formulario con edición condicional | 4 | ?? SUGERIDO |
| TAREA-045 | Crear menú "Bonificaciones" en `MainLayout.razor` | Ítem raíz con submenús | 1 | ?? REQUERIDO |
| | | **Subtotal 2.1.6** | **41** | |

---

### 2.2.1 – Gestión de OC Especiales (Manual)

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-046 | Crear tabla `BonificationSpecialOrders` | Tabla con estados y auditoría | 3 | ?? REQUERIDO |
| TAREA-047 | Crear entidad EF: `BonificationSpecialOrder` | Entidad + Configuration | 3 | ?? REQUERIDO |
| TAREA-048 | Crear modelo de servicio | POCO + mapping | 1 | ?? REQUERIDO |
| TAREA-049 | Crear `IBonificationSpecialOrderRepository` | Repositorio con `GetApprovedTotalAsync` | 5 | ?? REQUERIDO |
| TAREA-050 | Crear `IBonificationSpecialOrderService` | Servicio con Approve/Reject | 6 | ?? REQUERIDO |
| TAREA-051 | Crear página de listado `BonificationSpecialOrders.razor` | Grid con filtros y acciones | 8 | ?? REQUERIDO |
| TAREA-052 | Crear dialog `AddBonificationSpecialOrder.razor` | Formulario de ingreso | 5 | ?? REQUERIDO |
| TAREA-053 | Crear dialog `ApproveBonificationSpecialOrder.razor` | Modal de confirmación | 2 | ?? REQUERIDO |
| TAREA-054 | Crear dialog `RejectBonificationSpecialOrder.razor` | Modal con motivo | 2 | ?? REQUERIDO |
| | | **Subtotal 2.2.1** | **35** | |

---

### 2.2.2 – Carga Masiva de OC Especiales

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-055 | Agregar método `BulkAddAsync` | Repositorio + Servicio + Modelos | 8 | ?? SUGERIDO |
| TAREA-056 | Crear `IBonificationSpecialOrderImportService` | Parseo Excel/CSV + plantilla | 8 | ?? SUGERIDO |
| TAREA-057 | Crear página `BulkBonificationSpecialOrders.razor` | Flujo 3 pasos | 10 | ?? SUGERIDO |
| TAREA-058 | Agregar acceso a Carga Masiva | Botón en listado | 1 | ?? SUGERIDO |
| | | **Subtotal 2.2.2** | **27** | |

---

### 2.2.3 – Conciliación de Notas Crédito

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-059 | Crear tablas `CreditNoteReconciliations` y `ExternalCreditNotes` | 2 tablas con columna calculada | 4 | ?? REQUERIDO |
| TAREA-060 | Crear entidades EF | 2 entidades + configuraciones | 4 | ?? REQUERIDO |
| TAREA-061 | Crear modelos, repositorio y servicio para `CreditNoteReconciliation` | Completo con `BulkProcessAsync` | 10 | ?? REQUERIDO |
| TAREA-062 | Crear modelos, repositorio y servicio para `ExternalCreditNote` | Completo con validaciones | 7 | ?? REQUERIDO |
| TAREA-063 | Crear página principal `CreditNoteReconciliation.razor` | 2 pestañas + indicador cuadre | 12 | ?? REQUERIDO |
| TAREA-064 | Crear 4 dialogs de conciliación manual | Conciliar, Rechazar, Add, Approve/Reject | 9 | ?? REQUERIDO |
| TAREA-065 | Crear `ICreditNoteReconciliationImportService` | Plantilla pre-poblada + parseo | 10 | ?? SUGERIDO |
| TAREA-066 | Crear `IExternalCreditNoteImportService` | Plantilla en blanco + parseo | 5 | ?? SUGERIDO |
| TAREA-067 | Crear página `BulkCreditNoteReconciliation.razor` | Flujo 3 pasos con 4 tablas | 10 | ?? SUGERIDO |
| TAREA-068 | Crear página `BulkExternalCreditNotes.razor` | Flujo 3 pasos | 6 | ?? SUGERIDO |
| TAREA-069 | Agregar "Conciliación de NC" al menú | Subítem en Operaciones | 1 | ?? REQUERIDO |
| | | **Subtotal 2.2.3** | **78** | |

---

### 2.2.4 – Lista de Precios Promocional

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-070 | Crear tablas `PromotionalPriceLists` y `PromotionalPriceListItems` | 2 tablas con 13 columnas | 3 | ?? REQUERIDO |
| TAREA-071 | Entidades EF + Configurations + Models + Mappings | Estructura completa | 5 | ?? REQUERIDO |
| TAREA-072 | Repositorio y servicio `IPromotionalPriceListRepository` | Con lógica de reemplazo | 9 | ?? REQUERIDO |
| TAREA-073 | Servicio de descarga HTTP + parseo `IPriceListFetchService` | 2 estrategias (directa + autenticada) | 8 | ?? REQUERIDO |
| TAREA-074 | Worker automático `PriceListFetchWorker` | Job programado 6 AM | 6 | ?? REQUERIDO |
| TAREA-075 | Servicio de parseo manual | Reutiliza lógica de TAREA-073 | 3 | ?? REQUERIDO |
| TAREA-076 | Página `PromotionalPriceLists.razor` + carga manual | Grid + indicadores + contingencia | 12 | ?? REQUERIDO |
| TAREA-077 | Notificación en Dashboard | Alerta sin lista activa | 2 | ?? REQUERIDO |
| TAREA-078 | Agregar "Lista de Precios" al menú | Subítem en Configuración | 1 | ?? REQUERIDO |
| | | **Subtotal 2.2.4** | **49** | |

---

### 2.2.5 – Gestión de Exclusiones (Pedido Especial)

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-079 | Agregar columna `IsSpecialOrder` en `CUSTOMER_ORDERS` | Script de migración | 2 | ?? REQUERIDO |
| TAREA-080 | Crear estructura de auditoría explícita | Tabla de trazabilidad | 3 | ?? REQUERIDO |
| TAREA-081 | Actualizar SP/consultas de cálculo y reportes | Exclusión dinámica | 5 | ?? REQUERIDO |
| TAREA-082 | Agregar propiedad `IsSpecialOrder` en entidades y modelos | Propagación completa | 2 | ?? REQUERIDO |
| TAREA-083 | Actualizar mapeos EF/AutoMapper | Configuraciones | 2 | ?? REQUERIDO |
| TAREA-084 | Implementar operación dedicada `UpdateSpecialOrderFlagAsync` | Método especializado | 5 | ?? REQUERIDO |
| TAREA-085 | Aplicar reglas de negocio | Validaciones de elegibilidad | 4 | ?? REQUERIDO |
| TAREA-086 | Blindar actualización general | Bloqueo en `UpdateAsync` | 3 | ?? REQUERIDO |
| TAREA-087 | Actualizar consultas de cálculo de Bono por Pedido | Exclusión efectiva | 4 | ?? REQUERIDO |
| TAREA-088 | Crear permiso `Administración de Pedidos Especiales` | Rol + políticas | 3 | ?? REQUERIDO |
| TAREA-089 | Agregar visualización en pantalla de pedido | Campo controlado | 5 | ?? REQUERIDO |
| TAREA-090 | Implementar flujo de modificación exclusiva | Dialog dedicado | 4 | ?? REQUERIDO |
| TAREA-091 | Actualizar reporte `Customer Orders` | Columna + filtro | 4 | ?? SUGERIDO |
| TAREA-092 | Actualizar reporte `Customer Orders Activities` | Columna + filtro | 4 | ?? SUGERIDO |
| TAREA-093 | Actualizar reporte `Customer Sales` | Columna + filtro | 4 | ?? SUGERIDO |
| TAREA-094 | Integrar auditoría estándar + log específico | Doble auditoría | 4 | ?? REQUERIDO |
| TAREA-095 | Plan de pruebas técnicas | Unitarias + Integración + UI/E2E | 5 | ?? SUGERIDO |
| | | **Subtotal 2.2.5** | **63** | |

---

### 2.2.6 – Integración TOTUS por SP

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-096 | Definir contrato técnico formal del SP | Documentación completa | 2 | ?? REQUERIDO |
| TAREA-097 | Configurar conexión segura por ambiente | Settings + secretos | 2 | ?? REQUERIDO |
| TAREA-098 | Crear modelos request/response | Tipado completo | 2 | ?? REQUERIDO |
| TAREA-099 | Implementar adaptador DataAccess | Repositorio para SP | 3 | ?? REQUERIDO |
| TAREA-100 | Implementar servicio de negocio | Capa de validación | 3 | ?? REQUERIDO |
| TAREA-101 | Implementar fallback y reintentos | Resiliencia operativa | 4 | ?? REQUERIDO |
| TAREA-102 | Implementar auditoría técnica | Tabla + logging | 3 | ?? REQUERIDO |
| TAREA-103 | Implementar alertamiento administrativo | Servicio de salud | 2 | ?? REQUERIDO |
| TAREA-104 | Mostrar banner de facturación desactualizada | RadzenAlert en CU7 | 2 | ?? REQUERIDO |
| TAREA-105 | Integrar consulta TOTUS al cálculo | Sin cache | 4 | ?? REQUERIDO |
| TAREA-106 | Crear pruebas unitarias | Cobertura completa | 4 | ?? SUGERIDO |
| TAREA-107 | Crear pruebas de integración | Contra BD TOTUS QA | 4 | ?? REQUERIDO |
| TAREA-108 | Ejecutar pruebas de rendimiento E2E | Validación SLA | 4 | ?? SUGERIDO |
| TAREA-109 | Documentar runbook operativo | Guía de incidentes | 3 | ?? SUGERIDO |
| TAREA-110 | Formalizar criterios de aceptación | Sign-off con stakeholders | 3 | ?? SUGERIDO |
| | | **Subtotal 2.2.6** | **45** | |

---

## Resumen Ejecutivo

### Por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 2.1.1 – Clientes Distribuidores | 6 | 21 | 3.9% |
| 2.1.2 – Reportes con filtro | 4 | 13 | 2.4% |
| 2.1.3 – Períodos de Bonificación | 18 | 102 | 18.8% |
| 2.1.5 – Rangos y Vigencias | 8 | 36 | 6.6% |
| 2.1.6 – Descuentos por Pedido | 9 | 41 | 7.6% |
| 2.2.1 – OC Especiales (Manual) | 9 | 35 | 6.4% |
| 2.2.2 – OC Especiales (Masivo) | 4 | 27 | 5.0% |
| 2.2.3 – Conciliación NC | 11 | 78 | 14.4% |
| 2.2.4 – Lista Precios | 9 | 49 | 9.0% |
| 2.2.5 – Exclusiones Pedido | 17 | 63 | 11.6% |
| 2.2.6 – Integración TOTUS | 15 | 45 | 8.3% |
| **TOTAL** | **110** | **543** | **100%** |

### Por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| ?? REQUERIDO | 78 | 416 | 76.6% |
| ?? SUGERIDO | 28 | 114 | 21.0% |
| ?? DESEABLE | 4 | 13 | 2.4% |
| **TOTAL** | **110** | **543** | **100%** |

### Escenarios de Reducción

| Escenario | Descripción | Horas | Días hábiles (8h) |
|-----------|-------------|-------|-------------------|
| **MVP Mínimo** | Solo REQUERIDAS | 416 | ~52 |
| **MVP Recomendado** | REQUERIDAS + SUGERIDAS (sin masivos) | 469 | ~59 |
| **Alcance Completo** | Todas las tareas | 543 | ~68 |

---

## Notas Importantes

1. **TAREA-028 incluida**: Esta tarea (Crear vista de cierre de período CU10) aparece en el documento de definición pero estaba ausente en la versión anterior de estimaciones. Se ha añadido con 6 horas como REQUERIDA.

2. **Total de tareas**: 110 (incluyendo 2 canceladas que no suman horas)

3. **Incremento vs versión anterior**: +19 horas por:
   - TAREA-028 agregada: +6h
   - Ajustes en TAREA-072 (Lista Precios): +2h
   - Correcciones módulo Conciliación NC: +7h
   - Ajustes arquitectura: +4h

4. **Tareas masivas diferibles**: TAREA-055 a 058 (OC), TAREA-065 a 068 (NC) = 58 horas totales
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

### 2.2.6 – Integración TOTUS: Extracción de Facturación por SP

| # | Funcionalidad | Tarea | Descripción | Horas | Prioridad | Sustentación |
|---|--------------|-------|-------------|-------|-----------|--------------|
| TAREA-096 | Contrato técnico de integración | ?? Definir contrato formal del SP | Parametría de entrada/salida, tipos, nullabilidad, semántica y convención de errores. | 2 | ?? REQUERIDO | Sin contrato técnico no hay forma de implementar integración estable ni validar con la fábrica externa. |
| TAREA-097 | Configuración de conectividad | ?? Configurar conexión segura a TOTUS por ambiente | `appsettings`, secretos, timeout y parámetros base de resiliencia. | 2 | ?? REQUERIDO | Sin conectividad robusta por ambiente no existe invocación operativa al SP en el ciclo real. |
| TAREA-098 | Modelado de datos | ?? Modelos request/response de facturación TOTUS | Modelos tipados para documento, fechas, filtros opcionales y respuesta consolidada. | 2 | ?? REQUERIDO | Evita acoplamiento implícito y errores de mapeo en la integración. |
| TAREA-099 | Acceso a datos | ?? Adaptador DataAccess de invocación al SP | Ejecución parametrizada, mapeo robusto de nulos y conversión de tipos numéricos. | 3 | ?? REQUERIDO | Núcleo técnico de consumo del SP; bloquea el servicio de negocio. |
| TAREA-100 | Lógica de aplicación | ?? Servicio de negocio de facturación TOTUS | Validaciones de entrada y normalización de salida para motor de bonificación. | 3 | ?? REQUERIDO | Permite encapsular reglas y desacoplar la consulta externa del cálculo de bono. |
| TAREA-101 | Resiliencia operativa | ?? Implementar fallback y reintentos controlados | Reintentos configurables y uso de último valor conocido ante fallo de TOTUS. | 4 | ?? REQUERIDO | Sin resiliencia, una caída de TOTUS rompe consulta de bonificación al distribuidor. |
| TAREA-102 | Trazabilidad | ?? Auditoría técnica de consultas TOTUS | Log de parámetros, duración, resultado (`OK/FALLBACK/ERROR`) y correlación. | 3 | ?? REQUERIDO | Necesario para diagnóstico, soporte y control operacional en incidencias. |
| TAREA-103 | Operación | ?? Alertamiento administrativo de degradación TOTUS | Alertas por fallback, errores consecutivos y latencia anómala. | 2 | ?? REQUERIDO | Asegura detección temprana de incidentes y respuesta de soporte. |
| TAREA-104 | UX en degradación | ?? Banner de facturación desactualizada en CU7 | Mensaje visible con timestamp cuando se use fallback. | 2 | ?? REQUERIDO | Transparencia al distribuidor cuando no hay dato en tiempo real. |
| TAREA-105 | Integración funcional | ?? Integrar TOTUS al cálculo dinámico de bonificación | Consumo efectivo del servicio en CU7 sin cache de resultados. | 4 | ?? REQUERIDO | Cumple la promesa de cálculo en línea con información actualizada de facturación. |
| TAREA-106 | Calidad técnica | ??? Pruebas unitarias de integración lógica TOTUS | Validaciones, mapeo de datos, nullables y rutas de error. | 4 | ?? SUGERIDO | Reduce retrabajo y regresiones; recomendable para estabilizar salida. |
| TAREA-107 | Validación externa | ?? Pruebas de integración con entorno TOTUS de pruebas | Invocación real del SP, casos de éxito y errores controlados. | 4 | ?? REQUERIDO | Sin validación real no hay garantía de compatibilidad con el SP entregado por fábrica externa. |
| TAREA-108 | Performance | ??? Pruebas de rendimiento E2E de consulta TOTUS | Verificar SLA objetivo y comportamiento con concurrencia. | 4 | ?? SUGERIDO | Recomendable para robustez operativa; puede hacerse en fase de endurecimiento. |
| TAREA-109 | Operación y soporte | ??? Documentar runbook de incidentes TOTUS | Diagnóstico, contingencia y escalamiento hacia fábrica externa. | 3 | ?? SUGERIDO | Mejora tiempo de respuesta ante incidentes sin bloquear MVP funcional. |
| TAREA-110 | Gobierno de integración | ??? Cierre de criterios de aceptación con PROMOS y fábrica externa | Validar contrato final, SLA y manejo de errores antes de producción. | 3 | ?? SUGERIDO | Ordena salida a producción y reduce riesgo contractual/técnico. |
| | | | **Subtotal 2.2.6** | **45** | | **Módulo de integración crítica para Bonificación por Facturación; +45h al total del proyecto.** |

---

## Resumen Final por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 2.1.1 – Clientes Distribuidores | 6 | 21 | 4.0% |
| 2.1.2 – Reportes con filtro de Cliente | 4 | 13 | 2.5% |
| 2.1.3 – Períodos de Bonificación | 9 | 63 | 12.0% |
| 2.1.4 – Tipos de Bono | 6 | 32 | 6.1% |
| 2.1.5 – Vigencias de Bonificación | 9 | 57 | 10.9% |
| 2.1.6 – Vigencias de Descuentos | 9 | 41 | 7.8% |
| 2.2.1 – OC Especiales (Manual) | 9 | 35 | 6.7% |
| 2.2.2 – OC Especiales (Masivo) | 4 | 27 | 5.2% |
| 2.2.3 – Conciliación de NC | 11 | 78 | 14.9% |
| 2.2.4 – Lista de Precios Promocional | 9 | 49 | 9.4% |
| 2.2.5 – Gestión de Exclusiones (Pedido Especial) | 17 | 63 | 12.0% |
| 2.2.6 – Integración TOTUS por SP | 15 | 45 | 8.6% |
| **TOTAL** | **108** | **524** | **100%** |

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
| TAREA-106 | Pruebas unitarias integración TOTUS | ?? SUGERIDO | 4 | Mejora calidad y estabilidad técnica; no bloquea salida inicial funcional. |
| TAREA-108 | Pruebas de rendimiento TOTUS | ?? SUGERIDO | 4 | Endurece capacidad operativa; puede ejecutarse en estabilización. |
| TAREA-109 | Runbook operativo TOTUS | ?? SUGERIDO | 3 | Recomendado para soporte, pero diferible sin bloquear MVP. |
| TAREA-110 | Criterios de aceptación formal TOTUS | ?? SUGERIDO | 3 | Aporta gobernanza de integración; puede cerrarse al final del ciclo. |
| **TOTAL DIFERIBLE** | | | **124** | **Incluye +10h del módulo 2.2.6 (calidad/performance/operación).** |

### Escenarios de contratación

| Escenario | Descripción | Horas | Días hábiles |
|-----------|-------------|-------|--------------|
| **MVP Mínimo** | Solo tareas REQUERIDAS | 400 | ~50.0 |
| **MVP Recomendado** | REQUERIDAS + Sugeridas sin masivos (OC + NC) + sin Deseables | 453 | ~56.6 |
| **Alcance Completo** | Todas las tareas (108) | 524 | ~65.5 |

> **Recomendación**: El **MVP Mínimo (400 h)** cubre el ciclo completo de bonificación operativo,
> incluida la exclusión por `Pedido Especial` y la integración base de facturación con TOTUS por SP.
> Los caminos masivos (OC + NC, 58 h en total) y el endurecimiento de calidad/performance de TOTUS
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
>
> **Agregado Integración TOTUS por SP (v2.3)**: +45 h en 15 tareas (TAREA-096 a TAREA-110),
> orientadas a contrato técnico, invocación robusta, fallback operativo, trazabilidad y validación de SLA.
> Total proyecto actualizado: **524 h**.

---

## Resumen Ejecutivo por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| ?? REQUERIDO | 76 | 400 | 76.3% |
| ?? SUGERIDO | 28 | 111 | 21.2% |
| ?? DESEABLE | 4 | 13 | 2.5% |
| **TOTAL** | **108** | **524** | **100%** |

> **Reducción máxima posible** (eliminando Sugeridos + Deseables): **?124 h** ? MVP en **400 h (~50.0 días hábiles)**  
> **Reducción moderada** (eliminando solo masivos y Deseables): **?71 h** ? MVP+ en **453 h (~56.6 días hábiles)**

---

## Distribución por Tipo de Tarea

| Tipo | Horas | % |
|------|-------|---|
| ??? Base de Datos (scripts SQL) | 33 | 6.3% |
| ?? Backend (entidades, repos, servicios, jobs, import) | 302 | 57.6% |
| ??? Frontend Blazor (páginas, dialogs, filtros) | 189 | 36.1% |
| **Total** | **524** | **100%** |

---

## Distribución por Tipo de Trabajo

| Tipo | Horas | % |
|------|-------|---|
| ??? Base de Datos (scripts SQL) | 33 | 6.3% |
| ?? Backend (entidades, repos, servicios, jobs, import) | 302 | 57.6% |
| ??? Frontend Blazor (páginas, dialogs, filtros) | 189 | 36.1% |
| **Total** | **524** | **100%** |
