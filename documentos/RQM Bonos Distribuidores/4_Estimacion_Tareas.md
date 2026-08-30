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

### 4.3.4 – Generación y Consulta de Bonificación Actual (CU7)

#### 4.3.4.1 – Modelos & Respuesta de Cálculo

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-120 | Crear modelo de respuesta de cálculo de bonificación | Estructura única `BonificationCalculationResult` + `PeriodDateRange` + `BonificationCalculationDetail` + `BonusRangeApplied` + `GamificationLevel` | 4 | ?? REQUERIDO |
| TAREA-121 | Crear modelo DTO para REST API (Request/Response) | `BonificationConsultationRequest` + `BonificationConsultationResponse` | 1.5 | ?? REQUERIDO |
| TAREA-122 | Crear enumeraciones y constantes de gamificación | Umbrales por nivel (Bronce $0, Plata $1M, Oro $5M, Platino $10M, Diamante $25M) + nombres + colores | 1 | ?? REQUERIDO |
| TAREA-123 | Crear modelo de solicitud de descarga PDF | `BonificationPdfDownloadRequest` + `BonificationPdfDownloadResponse` | 1 | ?? REQUERIDO |
| TAREA-124 | Crear mappings AutoMapper para modelos | Mappings bidireccionales en `ApplicationServicesProfile.cs` | 1.5 | ?? REQUERIDO |
| | | **Subtotal 4.3.4.1** | **9** | |

---

#### 4.3.4.2 – Lógica de Cálculo

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-125 | Crear interfaz de servicio de cálculo de bonificación | `IBonificationCalculationService` con métodos principales | 1 | ?? REQUERIDO |
| TAREA-126 | Crear servicio de cálculo de Bono por Facturación | `BillingBonusCalculationService` (TOTUS + OC especiales + vigencia) | 6 | ?? REQUERIDO |
| TAREA-127 | Crear servicio de cálculo de Bono por Pedido | `OrderBonusCalculationService` (pedidos + descuentos + OC especiales + vigencia) | 6 | ?? REQUERIDO |
| TAREA-128 | Crear servicio de cálculo de Gamificación | `GamificationCalculationService` (determinación de nivel + progreso) | 3 | ?? REQUERIDO |
| TAREA-129 | Crear implementación del servicio de cálculo principal | `BonificationCalculationService` (orquestación + consolidación) | 4 | ?? REQUERIDO |
| TAREA-130 | Crear extensiones a repositorios existentes | Métodos faltantes: `GetSumByCustomerInPeriodAsync`, `GetActiveInstanceAsync`, `GetActiveAsync` | 3 | ?? REQUERIDO |
| TAREA-131 | Crear servicio de validación de acceso | `BonificationAccessValidationService` (validaciones de distribuidor) | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.4.2** | **25** | |

---

#### 4.3.4.3 – REST API

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-132 | Crear controlador REST de consulta de bonificación | `BonificationConsultationController` con endpoint `GET /api/bonification/consult` | 3 | ?? REQUERIDO |
| TAREA-133 | Crear middleware de autenticación JWT+OTP | `BonificationAuthenticationMiddleware` (validación JWT + OTP) | 4 | ?? REQUERIDO |
| TAREA-134 | Crear endpoint de verificación de salud API | Health checks (BD Aldebaran, TOTUS, Redis) | 2 | ?? REQUERIDO |
| TAREA-135 | Crear configuración CORS para sitio externo | CORS para `https://www.catalogospromocionales.com` | 1 | ?? REQUERIDO |
| TAREA-136 | Crear documentación OpenAPI/Swagger | Documentación completa de endpoints | 2 | ?? SUGERIDO |
| | | **Subtotal 4.3.4.3** | **12** | |

---

#### 4.3.4.4 – Descarga PDF

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-137 | Crear servicio de generación PDF de bonificación | `IBonificationPdfGenerationService` (recalcula + genera documento + timestamp) | 5 | ?? REQUERIDO |
| TAREA-138 | Crear endpoint de descarga PDF | `BonificationPdfController` con endpoint `POST /api/bonification/pdf/download` | 2 | ?? REQUERIDO |
| TAREA-139 | Crear auditoría de descargas PDF | Tabla + logging de descargas y intentos | 1 | ?? REQUERIDO |
| | | **Subtotal 4.3.4.4** | **8** | |

---

#### 4.3.4.5 – Frontend Blazor

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-140 | Crear página de consulta interna `BonificationConsultation.razor` | Página con selector, desglose visual + botón descarga | 6 | ?? REQUERIDO |
| TAREA-141 | Crear componente de desglose de bonificación | `BonificationDetailComponent.razor` (facturación, pedido, totales) | 3 | ?? REQUERIDO |
| TAREA-142 | Crear componente de gamificación visual | `GamificationCardComponent.razor` (nivel + progreso + barra) | 4 | ?? REQUERIDO |
| TAREA-143 | Crear servicio cliente REST | `BonificationConsultationService.cs` (consumo API + retry lógico) | 3 | ?? REQUERIDO |
| TAREA-144 | Crear manejo de errores UI y fallback visual | Estados (cargando, error, sin datos, actuales) + mensajes neutros | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.4.5** | **18** | |

---

#### 4.3.4.6 – Integración y Testing

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-145 | Crear pruebas unitarias de cálculo | `BonificationCalculationServiceTests.cs` (cobertura facturas, pedidos, gamificación) | 5 | ?? SUGERIDO |
| TAREA-146 | Crear pruebas de integración API REST | `BonificationApiTests.cs` (JWT+OTP, códigos HTTP, estructura) | 4 | ?? SUGERIDO |
| TAREA-147 | Crear pruebas E2E Blazor | `BonificationConsultationTests.cs` (Selenium/Playwright) | 6 | ?? SUGERIDO |
| TAREA-148 | Crear plan de seguridad y compliance | Documento de validaciones, autorización, encriptación, auditoría, RGPD | 3 | ?? SUGERIDO |
| | | **Subtotal 4.3.4.6** | **18** | |

---

#### 4.3.4.7 – Deployment y Monitoreo

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-149 | Crear configuración de variables de ambiente | `appsettings.*.json` y `.env` template (API URL, JWT, OTP, TOTUS, CORS) | 1 | ?? REQUERIDO |
| TAREA-150 | Crear configuración de logging y observabilidad | Application Insights + alertas latencia > 1s, error rate > 5% | 2 | ?? REQUERIDO |
| TAREA-151 | Crear guía de deployment y rollback | Documento con pasos, verificaciones, rollback plan, SLA esperado | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.4.7** | **5** | |

---

| | | **TOTAL 4.3.4** | **95** | |

---

### 4.3.5 – Notificaciones Automáticas de Bonificación

#### 4.3.5.1 – Infraestructura de Notificaciones

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-152 | Crear tabla `BonificationNotificationConfiguration` | Tabla global (una sola fila) con parámetros de configuración | 3 | ?? REQUERIDO |
| TAREA-153 | Crear tabla `BonificationNotificationLog` | Tabla de auditoría con estado de envío (ENQUEUED/SENT/FAILED) | 3 | ?? REQUERIDO |
| TAREA-154 | Crear entidades EF + Configurations + Models + Mappings | 2 entidades + 2 configuraciones + 2 modelos + mappings AutoMapper | 4 | ?? REQUERIDO |
| TAREA-155 | Crear repositorio `IBonificationNotificationRepository` | Interfaz + implementación con operaciones CRUD | 3 | ?? REQUERIDO |
| TAREA-156 | Crear servicio `IBonificationNotificationService` (orquestador principal) | Orquestación de 3 tipos + configuración global + encolamiento | 4 | ?? REQUERIDO |
| TAREA-157 | Crear servicio de encolamiento en Rabbit `IBonificationMessageQueueService` | Adaptador Rabbit MQ para notificaciones | 5 | ?? REQUERIDO |
| TAREA-158 | Crear componente de composición de email `IBonificationEmailComposer` | Generador de contenido HTML/texto por tipo | 4 | ?? REQUERIDO |
| | | **Subtotal 4.3.5.1** | **26** | |

---

#### 4.3.5.2 – Tipos de Notificaciones

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-159 | Notificación 1: "Alcanzó Nuevo Nivel" | Detector automático post-cálculo + composición + encolamiento | 5 | ?? REQUERIDO |
| TAREA-160 | Notificación 2: Job diario "Está Cerca del Siguiente Nivel" | Job diario (configurable, default 6 AM) con umbral % | 6 | ?? REQUERIDO |
| TAREA-161 | Notificación 3: Job periódico "Recordatorio de Progreso" | Job periódico (configurable, default Lunes 8 AM) | 5 | ?? REQUERIDO |
| | | **Subtotal 4.3.5.2** | **16** | |

---

#### 4.3.5.3 – Administración y Configuración

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-162 | Crear página Admin: Configuración de Notificaciones | Secciones de habilitación, umbrales, horarios, frecuencias | 7 | ?? REQUERIDO |
| TAREA-163 | Crear página Admin: Historial de Notificaciones Enviadas | Grid paginada con filtros (distribuidor, tipo, estado, fecha) | 8 | ?? REQUERIDO |
| TAREA-164 | Crear página Admin: Dashboard de Notificaciones | KPIs + gráficos + resumen por tipo + alertas | 10 | ?? SUGERIDO |
| | | **Subtotal 4.3.5.3** | **25** | |

---

#### 4.3.5.4 – Operabilidad y Observabilidad

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-165 | Crear health check para notificaciones | Endpoint con verificación Rabbit + configuración + estado | 2 | ?? SUGERIDO |
| TAREA-166 | Crear logging y observabilidad para notificaciones | Application Insights + eventos + alertas | 3 | ?? SUGERIDO |
| TAREA-167 | Crear pruebas unitarias de notificaciones | Cobertura de detección, throttling, encolamiento, composición | 6 | ?? SUGERIDO |
| | | **Subtotal 4.3.5.4** | **11** | |

---

| | | **TOTAL 4.3.5** | **78** | |

---

### 4.3.6 Consulta CU8: Histórico de Bonos (Períodos Anteriores)

#### 4.3.6.1 – Modelos & Estructuras de Datos

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-168 | Crear modelos de respuesta para histórico mes/año | Estructura jerárquica consolidada + desglose por tipo de bono | 3 | ?? REQUERIDO |
| TAREA-169 | Especificación en repositorio para histórico | Métodos para consultar bonos cerrados por mes/año | 4 | ?? REQUERIDO |
| TAREA-170 | Crear servicio de consulta del histórico | Lógica de agrupación por tipo + cálculo de subtotales | 5 | ?? REQUERIDO |
| TAREA-171 | Crear DTOs para REST API (histórico) | Request/Response públicos para API | 1.5 | ?? REQUERIDO |
| TAREA-172 | Crear modelos para exportación (PDF/Excel) | Estructuras para preparar datos de exportación | 2 | ?? SUGERIDO |
| TAREA-173 | Crear mappings AutoMapper para histórico | Mappings de consolidación y agregación | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.6.1** | **17.5** | |

---

#### 4.3.6.2 – REST API

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-174 | Crear controlador REST de histórico | Endpoints GET de histórico + disponibilidad de meses | 4 | ?? REQUERIDO |
| TAREA-175 | Crear endpoint de descarga PDF histórico | POST para descarga PDF con consolidado + desglose | 3 | ?? SUGERIDO |
| TAREA-176 | Crear endpoint de descarga Excel histórico | POST para descarga Excel con 3 hojas (resumen, tipo, detalle) | 3 | ?? SUGERIDO |
| TAREA-177 | Crear auditoría de consultas históricas | Tabla + logging de acceso a histórico | 2 | ?? REQUERIDO |
| TAREA-178 | Crear rate limiting para histórico | Throttling 10/min consultas, 5/hora descargas | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.6.2** | **14** | |

---

#### 4.3.6.3 – Frontend Blazor

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-179 | Crear página principal `BonificationHistory.razor` | Selectores mes/año + resumen + desglose expandible + descargas | 8 | ?? REQUERIDO |
| TAREA-180 | Crear code-behind `BonificationHistory.razor.cs` | Lógica de carga, consulta, validación, errores | 5 | ?? REQUERIDO |
| TAREA-181 | Crear componente de resumen mensual | 4 tarjetas de totales con colores visuales | 4 | ?? REQUERIDO |
| TAREA-182 | Crear componente de desglose por tipo | Expanders con tabla de instancias por tipo | 5 | ?? REQUERIDO |
| TAREA-183 | Crear servicio cliente para histórico | Métodos para consumir API REST | 3 | ?? REQUERIDO |
| TAREA-184 | Crear manejo de errores UI en histórico | Estados de carga, error, sin datos, éxito | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.6.3** | **27** | |

---

#### 4.3.6.4 – Exportación PDF/Excel

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-185 | Crear servicio de generación PDF histórico | Recalcula bonificación + genera PDF con encabezado, resumen, desglose, detalle | 6 | ?? SUGERIDO |
| TAREA-186 | Crear servicio de exportación Excel histórico | 3 hojas: resumen, por tipo, detalle completo | 5 | ?? SUGERIDO |
| TAREA-187 | Crear auditoría de exportaciones | Tabla + logging de descarga de PDF/Excel | 2 | ?? SUGERIDO |
| | | **Subtotal 4.3.6.4** | **13** | |

---

| | | **TOTAL 4.3.6** | **71.5** | |

---

### 4.3.7 – Cierre Automático de Períodos (CU10)

#### 4.3.7.1 – Persistencia de Bonos Cerrados (FOTO Congelada)

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-188 | Crear tabla `BonificationClosedPeriods` | Tabla FOTO congelada con PK, FK, constraints, índices | 3 | ?? REQUERIDO |
| TAREA-189 | Crear entidad EF: `BonificationClosedPeriod` | Entidad + Configuration | 3 | ?? REQUERIDO |
| TAREA-190 | Crear modelo de servicio: `BonificationClosedPeriod` | POCO + mapping AutoMapper | 1 | ?? REQUERIDO |
| TAREA-191 | Crear `IBonificationClosedPeriodRepository` y `BonificationClosedPeriodRepository` | Repositorio completo con consultas por instancia y cliente | 6 | ?? REQUERIDO |
| TAREA-192 | Crear `IBonificationClosureService` y `BonificationClosureService` | Servicio de cálculo final y congelación de bonos | 8 | ?? REQUERIDO |
| TAREA-193 | Crear `IBonificationRecommendationService` | Servicio de detección y generación de NC recomendadas | 5 | ?? REQUERIDO |
| | | **Subtotal 4.3.7.1** | **26** | |

---

#### 4.3.7.2 – Job Nocturno de Cierre

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-194 | Crear job nocturno `BonificationPeriodClosureJob` | Job 23:59 con orquestación completa de cierre y rollover | 8 | ?? REQUERIDO |
| TAREA-195 | Crear tabla de auditoría `BonificationClosureLogs` | Tabla con registro de cada ejecución del job | 2 | ?? REQUERIDO |
| TAREA-200 | Crear servicio de notificación de cierre `IBonificationClosureNotificationService` | Envío de email a PROMOS con resumen y recomendaciones | 4 | ?? REQUERIDO |
| TAREA-201 | Integrar job de cierre con ciclo de vida de instancias | Registro y coordinación con `BonificationPeriodInstanceLifecycleService` | 3 | ?? REQUERIDO |
| | | **Subtotal 4.3.7.2** | **17** | |

---

#### 4.3.7.3 – Gestión de Recomendaciones de NC

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-202 | Crear tabla de recomendaciones `BonificationNCRecommendations` | Tabla con NC recomendadas, monto y razón | 2 | ?? REQUERIDO |
| TAREA-203 | Crear `IBonificationNCRecommendationRepository` y servicio | Repositorio + servicio de gestión de recomendaciones | 5 | ?? REQUERIDO |
| TAREA-204 | Crear página Admin: Gestión de Recomendaciones NC | Grid de recomendaciones con acciones Aprobar/Rechazar | 6 | ?? REQUERIDO |
| | | **Subtotal 4.3.7.3** | **13** | |

---

#### 4.3.7.4 – Operabilidad y Monitoreo

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-196 | Crear página Admin: Vista de Cierre de Períodos | Grid de cierres con filtros y panel lateral de detalles | 6 | ?? REQUERIDO |
| TAREA-197 | Crear página Admin: Dashboard de Cierre | KPIs + gráficos + alertas de cierres fallidos | 6 | ?? SUGERIDO |
| TAREA-198 | Crear endpoint REST para consultar cierres | GET `/api/bonification/closures/{instanceId}` y `/history` | 3 | ?? REQUERIDO |
| TAREA-199 | Crear modelo `BonificationClosureResult` para respuesta | DTOs y modelos para API REST + notificaciones | 2 | ?? REQUERIDO |
| TAREA-205 | Crear pruebas unitarias de cierre | Cobertura de cálculo, persistencia, creación de siguiente instancia | 6 | ?? SUGERIDO |
| TAREA-206 | Crear pruebas de integración del job de cierre | Cobertura de ejecución, notificación, auditoría | 5 | ?? SUGERIDO |
| TAREA-207 | Crear health check para job de cierre | Endpoint con verificación de última ejecución | 2 | ?? SUGERIDO |
| TAREA-208 | Implementar rollback manual de cierre (solo Admin) | Servicio de deshacer cierre erróneo | 4 | ?? SUGERIDO |
| TAREA-209 | Crear configuración de cronograma de cierre | `appsettings.json` con expresión cron y parámetros | 1 | ?? REQUERIDO |
| TAREA-210 | Agregar logging y observabilidad al cierre | Application Insights + métricas + alertas | 2 | ?? SUGERIDO |
| TAREA-211 | Crear documentación de procedimiento de cierre | Guía operativa para PROMOS | 2 | ?? SUGERIDO |
| TAREA-212 | Crear plan de validación y UAT del cierre automático | Casos de prueba + criterios de aceptación | 3 | ?? SUGERIDO |
| | | **Subtotal 4.3.7.4** | **42** | |

---

| | | **TOTAL 4.3.7** | **98** | |

---

### 4.3.8 – Consulta CU8: Histórico de Bonos (Distribuidores - Períodos Anteriores)

#### 4.3.8.1 – Modelos & Estructuras de Datos

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-168 | Crear modelos de respuesta para histórico mes/año | Estructura jerárquica consolidada + desglose por tipo de bono | 3 | ?? REQUERIDO |
| TAREA-169 | Especificación en repositorio para histórico | Métodos para consultar bonos cerrados por mes/año | 4 | ?? REQUERIDO |
| TAREA-170 | Crear servicio de consulta del histórico | Lógica de agrupación por tipo + cálculo de subtotales | 5 | ?? REQUERIDO |
| TAREA-171 | Crear DTOs para REST API (histórico) | Request/Response públicos para API | 1.5 | ?? REQUERIDO |
| TAREA-172 | Crear modelos para exportación (PDF/Excel) | Estructuras para preparar datos de exportación | 2 | ?? SUGERIDO |
| TAREA-173 | Crear mappings AutoMapper para histórico | Mappings de consolidación y agregación | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.8.1** | **17.5** | |

---

#### 4.3.8.2 – REST API

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-174 | Crear controlador REST de histórico | Endpoints GET de histórico + disponibilidad de meses | 4 | ?? REQUERIDO |
| TAREA-175 | Crear endpoint de descarga PDF histórico | POST para descarga PDF con consolidado + desglose | 3 | ?? SUGERIDO |
| TAREA-176 | Crear endpoint de descarga Excel histórico | POST para descarga Excel con 3 hojas (resumen, tipo, detalle) | 3 | ?? SUGERIDO |
| TAREA-177 | Crear auditoría de consultas históricas | Tabla + logging de acceso a histórico | 2 | ?? REQUERIDO |
| TAREA-178 | Crear rate limiting para histórico | Throttling 10/min consultas, 5/hora descargas | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.8.2** | **14** | |

---

#### 4.3.8.3 – Frontend Blazor

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-179 | Crear página principal `BonificationHistory.razor` | Selectores mes/año + resumen + desglose expandible + descargas | 8 | ?? REQUERIDO |
| TAREA-180 | Crear code-behind `BonificationHistory.razor.cs` | Lógica de carga, consulta, validación, errores | 5 | ?? REQUERIDO |
| TAREA-181 | Crear componente de resumen mensual | 4 tarjetas de totales con colores visuales | 4 | ?? REQUERIDO |
| TAREA-182 | Crear componente de desglose por tipo | Expanders con tabla de instancias por tipo | 5 | ?? REQUERIDO |
| TAREA-183 | Crear servicio cliente para histórico | Métodos para consumir API REST | 3 | ?? REQUERIDO |
| TAREA-184 | Crear manejo de errores UI en histórico | Estados de carga, error, sin datos, éxito | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.8.3** | **27** | |

---

#### 4.3.8.4 – Exportación PDF/Excel

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-185 | Crear servicio de generación PDF histórico | Recalcula bonificación + genera PDF con encabezado, resumen, desglose, detalle | 6 | ?? SUGERIDO |
| TAREA-186 | Crear servicio de exportación Excel histórico | 3 hojas: resumen, por tipo, detalle completo | 5 | ?? SUGERIDO |
| TAREA-187 | Crear auditoría de exportaciones | Tabla + logging de descarga de PDF/Excel | 2 | ?? SUGERIDO |
| | | **Subtotal 4.3.8.4** | **13** | |

---

| | | **TOTAL 4.3.8** | **71.5** | |

---

### 4.3.9 – Consulta CU6: Bonificación Actual (PROMOS - Panel Administrativo Interno)

#### 4.3.9.1 – Modelos & Respuesta de Cálculo

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-213 | Crear modelo consolidado de consulta sin persistencia | Estructura `BonificationCurrentPeriodDetail` con facturación, OC, vigencias, NC, descuentos (solo datos en memoria) | 2 | ?? REQUERIDO |
| TAREA-214 | Crear servicio de cálculo sin persistencia | `IBonificationCurrentPeriodService` que consolida en tiempo real sin caché | 4 | ?? REQUERIDO |
| TAREA-215 | Crear endpoint interno sin persistencia | Endpoint GET `/api/internal/bonification/current-period/{customerId}/{periodInstanceId}` (SIN auditoría) | 2 | ?? REQUERIDO |
| TAREA-216 | Crear página Blazor `BonificationCurrentPeriodDetail.razor` | Página interna PROMOS con desglose completo (6 secciones: Facturación, Pedidos, Vigencias, Bono, Botones) | 10 | ?? REQUERIDO |
| TAREA-217 | Crear componente desglose de cálculo | Componente `BonificationCalculationBreakdownComponent.razor` (paso a paso con colores) | 4 | ?? REQUERIDO |
| TAREA-218 | Crear servicio cliente Blazor | Servicio que consume `/api/internal/bonification/current-period` | 2 | ?? REQUERIDO |
| TAREA-219 | Crear endpoint PDF (snapshot puntual) | POST `/api/internal/bonification/current-period/pdf` (genera PDF con datos actuales, sin persistencia) | 3 | ?? REQUERIDO |
| TAREA-220 | Crear acceso desde menú | Botón "Ver Detalle Bonificación" en listado de OC Especiales | 1 | ?? REQUERIDO |
| TAREA-221 | Crear pruebas unitarias | `BonificationCurrentPeriodServiceTests.cs` (consolidación, tiempo real, errores parciales) | 3 | ?? SUGERIDO |
| TAREA-222 | Crear documentación de consulta | `docs/CU6_CurrentPeriodConsultation_Guide.md` (acceso, desglose, casos de uso, limitaciones) | 2 | ?? SUGERIDO |
| | | **TOTAL 4.3.9** | **33** | |

---

### 4.3.10 – Consulta CU8: Histórico de Bonos (PROMOS - Panel Administrativo Interno)

#### 4.3.10.1 – Página Principal y Filtros

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-223 | Crear página `BonificationHistoryPromos.razor` | Página Blazor interna PROMOS con filtros (Distribuidor/Mes/Año) + resumen + desglose | 8 | ?? REQUERIDO |
| TAREA-224 | Crear code-behind `BonificationHistoryPromos.razor.cs` | Lógica de carga de distribuidores y consulta de histórico | 4 | ?? REQUERIDO |
| TAREA-225 | Modificar `BonificationHistoryService` para PROMOS | Overload sin validación de CustomerId + método `GetDistributorsAsync()` + auditoría | 3 | ?? REQUERIDO |
| | | **Subtotal 4.3.10.1** | **15** | |

---

#### 4.3.10.2 – Auditoría y Seguridad

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-226 | Crear tabla de auditoría para consultas PROMOS | `BonificationHistoryPromoAuditTable.sql` (quién consultó a quién, cuándo, IP) | 1 | ?? REQUERIDO |
| TAREA-228 | Crear servicio de auditoría | `IBonificationHistoryAuditService` (registro + consulta de logs) | 3 | ?? REQUERIDO |
| TAREA-232 | Crear validación de permisos | Guards Blazor (solo Admin / Consulta de bonificaciones) | 2 | ?? REQUERIDO |
| TAREA-234 | Crear rate limiting para consultas PROMOS | Throttling 50 consultas/minuto por usuario PROMOS | 2 | ?? REQUERIDO |
| | | **Subtotal 4.3.10.2** | **8** | |

---

#### 4.3.10.3 – Componentes y Servicios

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-227 | Crear panel lateral de detalle | Componente `BonificationHistoryPromosSidePanel.razor` (slide-in con desglose completo) | 10 | ?? SUGERIDO |
| TAREA-229 | Crear dashboard de auditoría | Página `BonificationHistoryPromoAuditDashboard.razor` (opcional, consultas/usuario, alertas de abuso) | 6 | ?? SUGERIDO |
| TAREA-233 | Crear componente de búsqueda de distribuidor | Componente `DistributorSelectorComponent.razor` (searchable, caché 5 min) | 3 | ?? SUGERIDO |
| | | **Subtotal 4.3.10.3** | **19** | |

---

#### 4.3.10.4 – Exportación y Documentación

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-230 | Crear servicio de PDF para PROMOS | Generador de PDF histórico con encabezado "Consulta generada por PROMOS" | 4 | ?? SUGERIDO |
| TAREA-231 | Crear servicio de Excel para PROMOS | Exportador Excel con 3 hojas (Resumen, Por Tipo, Detalle Completo) | 4 | ?? SUGERIDO |
| TAREA-235 | Crear testing de acceso PROMOS | Pruebas de autorización, auditoría, rate limiting | 4 | ?? SUGERIDO |
| TAREA-236 | Crear documentación de uso para PROMOS | `docs/CU8_PROMOS_HistoryConsultation_Guide.md` | 2 | ?? SUGERIDO |
| TAREA-237 | Crear link en menú principal | Agregar subítem en "Bonificaciones > Operaciones" | 1 | ?? REQUERIDO |
| | | **Subtotal 4.3.10.4** | **15** | |

---

| | | **TOTAL 4.3.10** | **57** | |

---

### 4.3.11 – Módulo de Reportería Analítica

#### 4.3.11.1 – Reportes Críticos de Negocio

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-238 | Crear reporte "Bonos Calculados vs Bonos Aplicados" | Comparativa de bonos teóricos vs reales, consolidación y gráficos (barras + líneas) | 12 | ?? REQUERIDO |
| TAREA-239 | Crear reporte "Distribuidores que Consultaron Bonos" | Auditoría de acceso (CU7/CU8), tabla de consultas, gráficos (líneas + heatmap + pastel), alertas de abuso | 10 | ?? REQUERIDO |
| TAREA-240 | Crear reporte "Discrepancias de NC (Calculada vs Real)" | Diferencias NC Sistema vs TOTVS, consolidación, gráficos (dispersión + barras + Sankey) | 14 | ?? REQUERIDO |
| TAREA-241 | Crear reporte "Auditoría de Acciones del Usuario PROMOS" | Rastro completo de acciones de PROMOS, consolidación por usuario/tipo/hora, gráficos (barras + timeline + heatmap) | 12 | ?? REQUERIDO |
| TAREA-242 | Crear reporte "Precios y Vigencias Usados en Período" | Documentación de parámetros activos, trazabilidad de cambios, gráficos (timeline + barras + líneas) | 10 | ?? SUGERIDO |
| TAREA-243 | Crear reporte "Ingresos Manuales Aplicados" | Detalle OC Especiales y NC Externas, consolidación por estado, gráficos (barras + pastel + líneas) | 11 | ?? REQUERIDO |
| | | **Subtotal 4.3.11.1** | **69** | |

---

#### 4.3.11.2 – Reportes Adicionales (Recomendados)

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-244 | Crear reporte "Resumen Mensual de Bonificación" | Consolidado ejecutivo mensual, KPIs + desglose por tipo de bono + top 10 distribuidores + análisis de problemas + comparativa histórica | 15 | ?? SUGERIDO |
| TAREA-245 | Crear reporte "Análisis de Vigencias y Rangos Activos" | Verificación de configuraciones, validaciones de rangos solapados/huérfanos, análisis de sensibilidad, gráficos (barras + líneas + histograma) | 12 | ?? SUGERIDO |
| TAREA-246 | Crear reporte "Dashboard de Salud del Sistema" | Monitoreo de disponibilidad, latencia, tasa de error, estado de jobs automáticos, gráficos (gauge + sparklines + timeline) + alertas | 10 | ?? SUGERIDO |
| TAREA-247 | Crear reporte "Análisis de Cambios de Precio" | Impacto de cambios en lista de precios, detección de manipulaciones sospechosas, gráficos (dispersión + barras + líneas) | 11 | ?? SUGERIDO |
| | | **Subtotal 4.3.11.2** | **48** | |

---

#### 4.3.11.3 – Infraestructura y Componentes Reutilizables

| # | Tarea | Descripción | Horas | Prioridad |
|---|-------|-------------|-------|-----------|
| TAREA-248 | Crear infraestructura base para reportería | Clase base `BonificationReportBase.cs` + interfaz `IBonificationReportService` + servicio de exportación `IBonificationReportExportService` + controlador centralizado + página base Blazor reutilizable | 8 | ?? REQUERIDO |
| TAREA-249 | Crear servicios de datos para cada reporte | 10 servicios específicos (uno por tipo de reporte), consultas desde repositorios, agrupación/consolidación, cálculo de estadísticas, alertas | 12 | ?? REQUERIDO |
| TAREA-250 | Crear componentes de gráficos reutilizables | 7 componentes Blazor: barras, líneas, pastel, dispersión, mapa de calor, tabla con paginación, KPI cards | 10 | ?? REQUERIDO |
| TAREA-251 | Crear jobs para generación de reportes automáticos | Job diario (reportes críticos) + Job semanal (reportes operacionales), almacenamiento de archivos, envío por email, registro en auditoría | 6 | ?? SUGERIDO |
| TAREA-252 | Crear página de "Descarga de Reportes" predefinidos | Tabla de reportes generados automáticamente, filtros (tipo/período/formato), descarga directa, opción de regenerar | 4 | ?? SUGERIDO |
| TAREA-253 | Crear auditoría y permisos para reportería | Rol `Consulta de reportes de bonificación`, tabla `BonificationReportAccessLog`, validación de permisos en endpoints, registro de auditoría | 3 | ?? REQUERIDO |
| TAREA-254 | Crear documentación de reportería | Guía de cada reporte + Arquitectura interna + Ejemplos de uso | 3 | ?? SUGERIDO |
| | | **Subtotal 4.3.11.3** | **46** | |

---

| | | **TOTAL 4.3.11** | **163** | |

---

## Resumen Ejecutivo

### Por Módulo

| Módulo | Tareas | Horas | % del Total |
|--------|--------|-------|-------------|
| 4.3.1.1 – Configuración de Clientes Distribuidores | 10 | 21 | 1.8% |
| 4.3.1.2 – Configuración de Períodos de Bonificación | 18 | 102 | 8.8% |
| 4.3.1.3 – Configuración de Vigencias para Tipos de Bonificación | 8 | 36 | 3.1% |
| 4.3.1.4 – Configuración de Vigencias para Descuentos por Total de Pedido | 9 | 41 | 3.5% |
| 4.3.2.1 – Gestión Manual de Ordenes de Compra Especiales | 9 | 35 | 3.0% |
| 4.3.2.2 – Gestión Masiva de Ordenes de Compra Especiales | 4 | 27 | 2.3% |
| 4.3.2.3 – Gestión Manual de Conciliación de Notas Crédito | 6 | 46 | 4.0% |
| 4.3.2.4 – Gestión Masiva de Conciliación de Notas Crédito | 5 | 32 | 2.8% |
| 4.3.2.5 – Actualización Diaria de Lista de Precios Promocional | 9 | 49 | 4.2% |
| 4.3.2.6 – Gestión de Pedidos Especiales | 17 | 63 | 5.4% |
| 4.3.2.7 – Solicitud de Información de Facturación a TOTUS | 15 | 45 | 3.9% |
| 4.3.3.1 – Autenticación OTP (MVP Email) | 9 | 51 | 4.4% |
| 4.3.4.1 – Modelos & Respuesta de Cálculo | 5 | 9 | 0.8% |
| 4.3.4.2 – Lógica de Cálculo | 7 | 25 | 2.2% |
| 4.3.4.3 – REST API | 5 | 12 | 1.0% |
| 4.3.4.4 – Descarga PDF | 3 | 8 | 0.7% |
| 4.3.4.5 – Frontend Blazor | 5 | 18 | 1.5% |
| 4.3.4.6 – Integración y Testing | 4 | 18 | 1.5% |
| 4.3.4.7 – Deployment y Monitoreo | 3 | 5 | 0.4% |
| 4.3.5.1 – Infraestructura de Notificaciones | 7 | 26 | 2.2% |
| 4.3.5.2 – Tipos de Notificaciones | 3 | 16 | 1.4% |
| 4.3.5.3 – Administración y Configuración | 3 | 25 | 2.2% |
| 4.3.5.4 – Operabilidad y Observabilidad | 3 | 11 | 0.9% |
| 4.3.6.1 – Modelos & Estructuras de Datos (CU8 Distribuidores) | 6 | 17.5 | 1.5% |
| 4.3.6.2 – REST API (CU8 Distribuidores) | 5 | 14 | 1.2% |
| 4.3.6.3 – Frontend Blazor (CU8 Distribuidores) | 6 | 27 | 2.3% |
| 4.3.6.4 – Exportación PDF/Excel (CU8 Distribuidores) | 3 | 13 | 1.1% |
| 4.3.7.1 – Persistencia de Bonos Cerrados (CU10) | 6 | 26 | 2.2% |
| 4.3.7.2 – Job Nocturno de Cierre (CU10) | 4 | 17 | 1.5% |
| 4.3.7.3 – Gestión de Recomendaciones de NC (CU10) | 3 | 13 | 1.1% |
| 4.3.7.4 – Operabilidad y Monitoreo (CU10) | 12 | 42 | 3.6% |
| 4.3.8 – Consulta CU6: Bonificación Actual (PROMOS) | 10 | 33 | 2.8% |
| 4.3.9 – Consulta CU8: Histórico de Bonos (PROMOS) | 15 | 57 | 4.9% |
| 4.3.10 – Consulta CU8: Histórico de Bonos (PROMOS - Panel Administrativo Interno) | 15 | 57 | 12.5% |
| 4.3.11 – Módulo de Reportería Analítica | 17 | 163 | 12.5% |
| **TOTAL** | **254** | **1,304.5** | **100%** |


### Por Prioridad

| Prioridad | Tareas | Horas | % del Total |
|-----------|--------|-------|-------------|
| 🔴 REQUERIDO | 200 | 1,008.5 | 86.9% |
| 🟡 SUGERIDO | 45 | 149 | 12.8% |
| 🟢 DESEABLE | 2 | 3 | 0.3% |
| **TOTAL** | **247** | **1,160.5** | **100%** |

### Escenarios de Contratación

| Escenario | Descripción | Horas | Días hábiles (8h) | Semanas (5 días) |
|-----------|-------------|-------|-------------------|------------------|
| **MVP Mínimo** | Solo tareas REQUERIDAS (200 tareas) | 1,008.5 | ~126 | ~25 |
| **MVP Recomendado** | REQUERIDAS + SUGERIDAS sin masivos | 1,100 | ~138 | ~27 |
| **Alcance Completo** | Todas las tareas (247) | 1,160.5 | ~145 | ~29 |

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
- **4.3.4.1**: 5 tareas | 9h (todas REQUERIDAS)
- **4.3.4.2**: 7 tareas | 25h (todas REQUERIDAS)
- **4.3.4.3**: 4 tareas | 10h (sin TAREA-136)
- **4.3.4.4**: 3 tareas | 8h (todas REQUERIDAS)
- **4.3.4.5**: 5 tareas | 18h (todas REQUERIDAS)
- **4.3.4.7**: 3 tareas | 5h (todas REQUERIDAS)
- **4.3.5.1**: 7 tareas | 26h (todas REQUERIDAS)
- **4.3.5.2**: 3 tareas | 16h (todas REQUERIDAS)
- **4.3.5.3**: 2 tareas | 15h (TAREA-162, TAREA-163; sin TAREA-164)
- **4.3.6.1**: 5 tareas | 15.5h (sin TAREA-172)
- **4.3.6.2**: 3 tareas | 8h (sin TAREA-175, TAREA-176)
- **4.3.6.3**: 6 tareas | 27h (todas REQUERIDAS)
- **4.3.7.1**: 6 tareas | 26h (todas REQUERIDAS)
- **4.3.7.2**: 4 tareas | 17h (todas REQUERIDAS)
- **4.3.7.3**: 3 tareas | 13h (todas REQUERIDAS)
- **4.3.7.4**: 4 tareas | 18h (sin TAREA-197, TAREA-205, TAREA-206, TAREA-207, TAREA-208, TAREA-210, TAREA-211, TAREA-212)
- **Total REQUERIDAS**: 171 tareas | 840.5h

### Tareas SUGERIDAS por Módulo
- **4.3.1.1**: 4 tareas | 6h
- **4.3.1.2**: 2 tareas | 9h
- **4.3.2.2**: 4 tareas | 27h
- **4.3.2.4**: 4 tareas | 31h
- **4.3.2.6**: 5 tareas | 17h
- **4.3.2.7**: 5 tareas | 13h
- **4.3.3.1**: 1 tarea | 6h
- **4.3.4.3**: 1 tarea | 2h (TAREA-136)
- **4.3.4.6**: 4 tareas | 18h
- **4.3.5.3**: 1 tarea | 10h (TAREA-164)
- **4.3.5.4**: 3 tareas | 11h
- **4.3.6.1**: 1 tarea | 2h (TAREA-172)
- **4.3.6.2**: 2 tareas | 6h (TAREA-175, TAREA-176)
- **4.3.6.4**: 3 tareas | 13h (todas SUGERIDAS)
- **4.3.7.4**: 8 tareas | 24h (TAREA-197, TAREA-205, TAREA-206, TAREA-207, TAREA-208, TAREA-210, TAREA-211, TAREA-212)
- **Total SUGERIDAS**: 37 tareas | 128h

### Tareas DESEABLE por Módulo
- **4.3.1.1**: 4 tareas | 5h (TAREA-007, TAREA-008, TAREA-009, TAREA-010)
- **Total DESEABLE**: 2 tareas | 5h

---

## Verificación de Totales

| Categoría | Tareas | Horas | % |
|-----------|--------|-------|---|
| 🔴 REQUERIDO | 171 | 840.5 | 86.5% |
| 🟡 SUGERIDO | 37 | 128 | 13.2% |
| 🟢 DESEABLE | 2 | 5 | 0.5% |
| **TOTAL** | **210** | **971.5** | **100%** |

> **Nota**: El total verificado es **971.5h** = 840.5 + 128 + 5. Este es el total actualizado del proyecto incluyendo todas las categorías de prioridad y las 210 tareas principales (sin contar las 2 tareas canceladas de TAREA-020 y TAREA-021). La sección 4.3.7 (Cierre Automático de Períodos - CU10) agrega 25 tareas nuevas con 98 horas de desarrollo.

