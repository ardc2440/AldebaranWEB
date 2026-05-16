# 4. ESTIMACIÓN DE TAREAS - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Fecha**: Mayo 2026  
**Versión**: 3.0  
**Perfil asumido**: Analista, Diseñador, Arquitecto y Desarrollador Senior .NET / Blazor con conocimiento del codebase Aldebaran

---

## 4.1 Criterios de Estimación

| Factor | Criterio |
|--------|----------|
| **Perfil** | Analista + Diseñador + Arquitecto + Desarrollador Senior con experiencia en el stack (C#, EF, Blazor, Radzen, SQL Server) |
| **Familiaridad** | Conoce el codebase Aldebaran (patrones Repository, Service, Radzen UI) |
| **Unidad** | Horas de desarrollo efectivo (incluye análisis, diseño, arquitectura, codificación) |
| **Incluye** | Análisis detallado, diseño técnico, arquitectura de solución, codificación, prueba unitaria básica, ajustes por feedback inmediato |
| **No incluye** | QA formal, pruebas de integración, documentación adicional, despliegue |

---

## 4.2 Leyenda de Prioridad

| Prioridad | Significado |
|-----------|-------------|
| ?? **REQUERIDO** | El sistema no funciona o no cumple el objetivo principal sin esta tarea. Su omisión bloquea otras tareas o casos de uso críticos. |
| ?? **SUGERIDO** | Mejora la operabilidad o la experiencia de usuario de forma significativa, pero el sistema puede funcionar sin ella en una primera versión. |
| ?? **DESEABLE** | Agrega comodidad, eficiencia o cobertura adicional. Puede diferirse a una segunda iteración sin impacto en el MVP. |

---

## 4.3 Resumen de Tareas por Módulo

### 4.3.1 Funcionalidades de Configuración

#### 4.3.1.1 – Configuración de Clientes Distribuidores

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-001 | Agregar columnas `IsDistributor` y `BonusEmail` a la tabla `Customers` | Script de migración SQL | 2 | ?? REQUERIDO |
| TAREA-002 | Agregar propiedades `IsDistributor` y `BonusEmail` a las entidades y modelos | Entidades, modelos, repositorio y servicio | 4 | ?? REQUERIDO |
| TAREA-003 | Agregar validación de negocio en `CustomerService` | Reglas para `IsDistributor` y `BonusEmail` | 3 | ?? REQUERIDO |
| TAREA-004 | Agregar columna "Es Distribuidor" en el listado `Customers.razor` | Columna con badge, filtro por distribuidor | 5 | ?? SUGERIDO |
| TAREA-005 | Agregar campos de distribuidor en el formulario `EditCustomer.razor` | CheckBox + TextBox con validadores | 4 | ?? REQUERIDO |
| TAREA-006 | Agregar campos de distribuidor en el formulario `AddCustomer.razor` | Idéntico a TAREA-005 | 3 | ?? SUGERIDO |
| TAREA-007 | Agregar filtro "Solo Distribuidores" en `CustomerOrderReportFilter` | Checkbox + reload dropdown | 3 | ?? DESEABLE |
| TAREA-008 | Agregar filtro "Solo Distribuidores" en `CustomerSalesReportFilter` | Mismo patrón TAREA-007 | 2 | ?? DESEABLE |
| TAREA-009 | Agregar filtro "Solo Distribuidores" en `CustomerReservationReportFilter` | Mismo patrón TAREA-007 | 2 | ?? DESEABLE |
| TAREA-010 | Agregar filtro "Solo Distribuidores" en 4 reportes adicionales | BackOrder, CustomerOrderActivity, AutomaticAssignment, PendingAutomatic | 6 | ?? DESEABLE |
| | | **Subtotal 4.3.1.1** | **21** | |

---

#### 4.3.1.2 – Configuración de Períodos de Bonificación

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
| | | **Subtotal 4.3.1.2** | **102** | |

---

#### 4.3.1.3 – Configuración de Vigencias para Tipos de Bonificación

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
| | | **Subtotal 4.3.1.3** | **36** | |

---

#### 4.3.1.4 – Configuración de Vigencias para Descuentos por Total de Pedido

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
| | | **Subtotal 4.3.1.4** | **41** | |

---

#### 4.3.2 – Gestión de Operaciones Manuales

#### 4.3.2.1 – Gestión Manual de Ordenes de Compra Especiales

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
| | | **Subtotal 4.3.2.1** | **35** | |

---

#### 4.3.2.2 – Gestión Masiva de Ordenes de Compra Especiales

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-055 | Agregar método `BulkAddAsync` | Repositorio + Servicio + Modelos | 8 | ?? SUGERIDO |
| TAREA-056 | Crear `IBonificationSpecialOrderImportService` | Parseo Excel/CSV + plantilla | 8 | ?? SUGERIDO |
| TAREA-057 | Crear página `BulkBonificationSpecialOrders.razor` | Flujo 3 pasos | 10 | ?? SUGERIDO |
| TAREA-058 | Agregar acceso a Carga Masiva | Botón en listado | 1 | ?? SUGERIDO |
| | | **Subtotal 4.3.2.2** | **27** | |

---

#### 4.3.2.3 – Gestión Manual de Conciliación de Notas Crédito

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-059 | Crear tablas `CreditNoteReconciliations` y `ExternalCreditNotes` | 2 tablas con columna calculada | 4 | ?? REQUERIDO |
| TAREA-060 | Crear entidades EF | 2 entidades + configuraciones | 4 | ?? REQUERIDO |
| TAREA-061 | Crear modelos, repositorio y servicio para `CreditNoteReconciliation` | Completo con `BulkProcessAsync` | 10 | ?? REQUERIDO |
| TAREA-062 | Crear modelos, repositorio y servicio para `ExternalCreditNote` | Completo con validaciones | 7 | ?? REQUERIDO |
| TAREA-063 | Crear página principal `CreditNoteReconciliation.razor` | 2 pestañas + indicador cuadre | 12 | ?? REQUERIDO |
| TAREA-064 | Crear 4 dialogs de conciliación manual | Conciliar, Rechazar, Add, Approve/Reject | 9 | ?? REQUERIDO |
| | | **Subtotal 4.3.2.3** | **46** | |

---

#### 4.3.2.4 – Gestión Masiva de Conciliación de Notas Crédito

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-065 | Crear `ICreditNoteReconciliationImportService` | Plantilla pre-poblada + parseo | 10 | ?? SUGERIDO |
| TAREA-066 | Crear `IExternalCreditNoteImportService` | Plantilla en blanco + parseo | 5 | ?? SUGERIDO |
| TAREA-067 | Crear página `BulkCreditNoteReconciliation.razor` | Flujo 3 pasos con 4 tablas | 10 | ?? SUGERIDO |
| TAREA-068 | Crear página `BulkExternalCreditNotes.razor` | Flujo 3 pasos | 6 | ?? SUGERIDO |
| TAREA-069 | Agregar "Conciliación de NC" al menú | Subítem en Operaciones | 1 | ?? REQUERIDO |
| | | **Subtotal 4.3.2.4** | **32** | |

---

#### 4.3.2.5 – Actualización Diaria de Lista de Precios Promocional

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
| | | **Subtotal 4.3.2.5** | **49** | |

---

#### 4.3.2.6 – Gestión de Pedidos Especiales

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
| | | **Subtotal 4.3.2.6** | **63** | |

---

#### 4.3.2.7 – Solicitud de Información de Facturación a TOTUS

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
| | | **Subtotal 4.3.2.7** | **45** | |

---

### 4.3.3 – Funcionalidades de Autenticación y Seguridad para Distribuidores

### 4.3.3.1 – Autenticación OTP (MVP Email)

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-111 | Crear infraestructura de persistencia OTP segura | Tabla OTP con hash/salt, TTL, intentos, estado, bloqueo e índices | 3 | ?? REQUERIDO |
| TAREA-112 | Crear modelos/entidades y repositorio OTP | Entidad EF, modelo de servicio, repositorio y operaciones de ciclo de vida OTP | 6 | ?? REQUERIDO |
| TAREA-113 | Crear servicio de negocio OTP (Email MVP) | Generación OTP, validación, invalidación atómica y emisión segura | 8 | ?? REQUERIDO |
| TAREA-114 | Implementar throttling y bloqueo temporal OTP | Límite 5 solicitudes/15 min por documento/IP + bloqueo por intentos fallidos | 6 | ?? REQUERIDO |
| TAREA-115 | Integrar envío OTP por Email de Bonificación | Envío SMTP a `BonusEmail`, validaciones RF32/RF33 y manejo de errores neutros | 4 | ?? REQUERIDO |
| TAREA-116 | Crear endpoints/API para flujo OTP | Solicitar OTP, reenviar OTP, validar OTP + emisión JWT | 6 | ?? REQUERIDO |
| TAREA-117 | Crear interfaz Blazor de autenticación OTP | Pantallas de documento/OTP, countdown TTL, cooldown de reenvío y mensajes neutros | 8 | ?? REQUERIDO |
| TAREA-118 | Implementar auditoría mínima obligatoria OTP | Registro de intentos (éxito/fallo), motivo, IP y timestamp | 4 | ?? REQUERIDO |
| TAREA-119 | Pruebas técnicas OTP (unitarias + integración básica) | Cobertura de reglas críticas: TTL, bloqueo, throttling, invalidación, cooldown | 6 | ?? SUGERIDO |
| | | **Subtotal 4.3.3.1** | **51** | |

---

## Resumen Ejecutivo

### Por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 4.3.1.1 – Configuración de Clientes Distribuidores | 10 | 21 | 3.7% |
| 4.3.1.2 – Configuración de Períodos de Bonificación | 18 | 102 | 17.7% |
| 4.3.1.3 – Configuración de Vigencias para Tipos de Bonificación | 8 | 36 | 6.3% |
| 4.3.1.4 – Configuración de Vigencias para Descuentos por Total de Pedido | 9 | 41 | 7.1% |
| 4.3.2.1 – Gestión Manual de Ordenes de Compra Especiales | 9 | 35 | 6.1% |
| 4.3.2.2 – Gestión Masiva de Ordenes de Compra Especiales | 4 | 27 | 4.7% |
| 4.3.2.3 – Gestión Manual de Conciliación de Notas Crédito | 6 | 46 | 8.0% |
| 4.3.2.4 – Gestión Masiva de Conciliación de Notas Crédito | 5 | 32 | 5.6% |
| 4.3.2.5 – Actualización Diaria de Lista de Precios Promocional | 9 | 49 | 8.5% |
| 4.3.2.6 – Gestión de Pedidos Especiales | 17 | 63 | 11.0% |
| 4.3.2.7 – Solicitud de Información de Facturación a TOTUS | 15 | 45 | 7.8% |
| 4.3.3.1 – Autenticación OTP (MVP Email) | 9 | 51 | 8.9% |
| **TOTAL** | **119** | **575** | **100%** |

### Por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| 🔴 REQUERIDO | 87 | 470 | 81.7% |
| 🟡 SUGERIDO | 28 | 105 | 18.3% |
| 🟢 DESEABLE | 4 | 13 | 2.3% |
| **TOTAL** | **119** | **575** | **100%** |

### Escenarios de Contratación

| Escenario | Descripción | Horas | Días hábiles (8h) |
|-----------|-------------|-------|-------------------|
| **MVP Mínimo** | Solo tareas REQUERIDAS (87 tareas) | 470 | ~59 |
| **MVP Recomendado** | REQUERIDAS + SUGERIDAS sin masivos | 525 | ~66 |
| **Alcance Completo** | Todas las tareas (119) | 575 | ~72 |

---

## Resumen de Priorización

### Tareas REQUERIDAS por Módulo
- **4.3.1.1**: 6 tareas | 15h
- **4.3.1.2**: 16 tareas | 97h
- **4.3.1.3**: 8 tareas | 36h
- **4.3.1.4**: 8 tareas | 40h
- **4.3.2.1**: 9 tareas | 35h
- **4.3.2.2**: 0 tareas | 0h (todas SUGERIDAS)
- **4.3.2.3**: 6 tareas | 46h
- **4.3.2.4**: 1 tarea | 1h (solo TAREA-069)
- **4.3.2.5**: 9 tareas | 49h
- **4.3.2.6**: 12 tareas | 54h
- **4.3.2.7**: 10 tareas | 32h
- **4.3.3.1**: 8 tareas | 45h
- **Total REQUERIDAS**: 87 tareas | 470h

### Tareas SUGERIDAS por Módulo
- **4.3.1.1**: 4 tareas | 6h
- **4.3.1.2**: 2 tareas | 9h
- **4.3.2.2**: 4 tareas | 27h
- **4.3.2.4**: 4 tareas | 31h
- **4.3.2.5**: 0 tareas | 0h
- **4.3.2.6**: 5 tareas | 17h
- **4.3.2.7**: 5 tareas | 13h
- **4.3.3.1**: 1 tarea | 6h
- **Total SUGERIDAS**: 28 tareas | 105h

### Tareas DESEABLE por Módulo
- **4.3.1.1**: 4 tareas | 13h (TAREA-007, TAREA-008, TAREA-009, TAREA-010)
- **Total DESEABLE**: 4 tareas | 13h

---

## Verificación de Totales

| Categoría | Tareas | Horas | % |
|-----------|--------|-------|---|
| 🔴 REQUERIDO | 87 | 470 | 81.7% |
| 🟡 SUGERIDO | 28 | 105 | 18.3% |
| 🟢 DESEABLE | 4 | 13 | 2.3% |
| **TOTAL** | **119** | **588** | **100%** |

> **Nota**: El total verificado es **588h** = 470 + 105 + 13. Este es el verdadero total del proyecto incluyendo todas las categorías de prioridad.

