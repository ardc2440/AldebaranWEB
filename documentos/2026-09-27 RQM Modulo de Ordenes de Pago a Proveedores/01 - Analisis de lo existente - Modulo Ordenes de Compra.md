# RQM Módulo de Órdenes de Pago a Proveedores
## 01 - Análisis de lo existente: Módulo de Órdenes de Compra (OC)

Fecha: 2026-09-26 · Rama base analizada: `1600bb2`

---

## 1. Modelo de datos (SQL Server)

### 1.1 `purchase_orders` (encabezado)
| Columna | Tipo | Nota |
|---|---|---|
| PURCHASE_ORDER_ID | int identity PK | |
| ORDER_NUMBER | varchar(10) UQ | Secuencia (`[Sequence(10)]`) |
| REQUEST_DATE / EXPECTED_RECEIPT_DATE | date | |
| REAL_RECEIPT_DATE | date null | Se llena al confirmar |
| PROVIDER_ID | int FK providers | **Única relación con el proveedor** |
| FORWARDER_AGENT_ID / SHIPMENT_FORWARDER_AGENT_METHOD_ID | FK null | Transportadora / método |
| EMPLOYEE_ID | FK employees | Creador |
| STATUS_DOCUMENT_TYPE_ID | smallint FK | Estado |
| IMPORT_NUMBER varchar(20), EMBARKATION_PORT varchar(50), PROFORMA_NUMBER varchar(20) | | |
| CREATION_DATE | datetime | |

Índices: CREATION_DATE, REQUEST_DATE, STATUS_DOCUMENT_TYPE_ID, UQ ORDER_NUMBER. Trigger: `TRGINSERTSTRANSITO_ORDERS`.

### 1.2 `purchase_order_details`
PURCHASE_ORDER_DETAIL_ID, PURCHASE_ORDER_ID, REFERENCE_ID, WAREHOUSE_ID, REQUESTED_QUANTITY, RECEIVED_QUANTITY (null).
UQ (PURCHASE_ORDER_ID, REFERENCE_ID, WAREHOUSE_ID). Triggers: alarmas de mínimos/agotados, `Trg_Set_Minimum_Quantity_Reference`, `TRGINSERTSTRANSITO_DETAILS`.

### 1.3 Tablas satélite
- `purchase_order_activities` (anotaciones / actividades con responsable y fecha de ejecución).
- `modified_purchase_orders` (+ `purchase_order_notifications`, `purchase_order_transit_alarms`): log de modificaciones con motivo y notificación a clientes afectados.
- `canceled_purchase_orders` (1:1, motivo de cancelación) — la cancelación va por `cancellation_requests` (solicitud operativa, tipo documento "C").
- `purchase_order_adjustment_log` (Aprobación de ajustes OC) — **columna `EMPLOYE_ID`** (typo, mapeado así en EF).
- `purchase_order_approval_ranges` + `_log` (tolerancias %).
- Alarmas: `alarms` → `alarm_messages` → `alarm_types` → `document_types` (código "O").

### 1.4 Estados (document_types código **"O"**)
| STATUS_ORDER | Código | Nombre | EDIT_MODE |
|---|---|---|---|
| 1 | P | Pendiente (en tránsito) | 1 |
| 2 | C | Confirmada (recibida, afecta inventario) | 0 |
| 3 | A | Cancelada | 0 |
| 4 | J | Ajuste en aprobación | 1 |

El código de negocio se apoya en **STATUS_ORDER** (no en el Id).

### 1.5 Hallazgo clave para Órdenes de Pago — **la OC no tiene valores monetarios**
- Ni el encabezado ni el detalle guardan precio unitario, moneda, TRM, subtotal, impuestos ni total.
- Los costos existen solo en el **maestro** `items`: `FOB_COST`, `CIF_COST`, `CURRENCY_ID` (+ unidades de medida FOB/CIF). Son costos vigentes, no históricos: si cambian, una OC antigua no se puede valorizar.
- `provider_references` solo relaciona proveedor ↔ referencia (sin costo por proveedor).
- `providers` no tiene datos bancarios, condiciones de pago ni plazo.

➡️ Antes de diseñar la Orden de Pago hay que decidir **de dónde sale el valor a pagar**.

---

## 2. Arquitectura por capas (flujo OC)

```
Aldebaran.Web (Blazor Server + Radzen)
  Pages/PurchaseOrderPages/*  ·  Pages/ReportPages/Purchase Orders/*
        ↓ (Models de Application.Services, AutoMapper)
Aldebaran.Application.Services
  PurchaseOrderService, PurchaseOrderDetailService, PurchaseOrderActivityService,
  PurchaseOrderApprovalRangeService, Reports/PurchaseOrderReportService
        ↓
Aldebaran.DataAccess.Infraestructure (Repository: RepositoryBase<AldebaranDbContext>, ExecuteQueryAsync/ExecuteCommandAsync)
        ↓
Aldebaran.DataAccess (Entities + Configuration EF Core, Database First "fluent")
```

---

## 3. Consulta de Órdenes de Compra (`/purchase-orders`)
Archivo: `Pages/PurchaseOrderPages/PurchaseOrders.razor(.cs)`. Menú: Movimientos de Inventario → Ordenes de compra.

- **Roles**: Administrador, Consulta / Creación / Modificación / Confirmación / Cancelación de órdenes de compra.
- **Grilla principal** (paginación en servidor, `LoadData` → `PurchaseOrderService.GetAsync(skip, top[, search])`, orden `ORDER_NUMBER desc`).
  Columnas: Número, Fecha creación (oculta), Solicitud, Esperada, Real, Estado, Nro. importación/Puerto/Proforma (ocultas), Proveedor, Transportadora, Método de envío.
- **Búsqueda** libre (`@oninput`): OrderNumber, ImportNumber, Proveedor, Transportadora, Agente, Puerto, Proforma, fechas formateadas (`dbContext.Format`).
- **Acciones por fila** (solo `StatusOrder == 1`): Confirmar, Editar, Cancelar (con validación de solicitud de cancelación pendiente).
- **Detalle expandible** (`RowExpand` → `GetChildData`, 4 llamadas por fila): pestañas **Referencias**, **Actividades**, **Alarmas**, **Notificaciones generadas al cliente**.

Puntos de extensión naturales para Órdenes de Pago:
1. Nueva columna "Estado de pago / Saldo" en la grilla (requiere proyección en el repositorio, no Include).
2. Nueva pestaña "Órdenes de pago" en el detalle expandible (carga perezosa en `GetChildData`).
3. Nuevo botón de acción "Generar orden de pago" condicionado por estado y rol.

---

## 4. Reporte de Órdenes de Compra (`/report/purchase-orders`)
Menú: Reportes → Proveedores → Ordenes de compra. Otros reportes relacionados en el mismo grupo: Referencias del proveedor, Notificación de pedidos atendidos automáticamente.

- **Filtro** (`PurchaseOrderReportFilter` / `PurchaseOrderFilter`): número, fechas (creación, solicitud, esperada, real), importación, puerto, proforma, proveedor, transportadora, agente, método de envío, bodega, referencias (MultiReferencePicker), estado, "diferencia cantidad solicitada".
- **Datos**: `PurchaseOrderReportRepository` → `EXEC SP_GET_PURCHASE_ORDER_REPORT {filter}` (resultado plano por referencia) → el razor.cs arma la jerarquía Orden → Bodega → Línea → Artículo → Referencia.
- Cantidad mostrada: `RECEIVED` si Confirmada, si no `REQUESTED`; peso y volumen calculados.
- Salida: HTML → PDF (`FileBytesGeneratorService`) o impresión.

---

## 5. Hallazgos técnicos (deuda a considerar al modificar)
| # | Severidad | Hallazgo |
|---|---|---|
| H1 | **Alta (seguridad)** | Reporte: los filtros se concatenan a texto y se ejecutan con `FromSqlRaw($"EXEC ... {filter}")` → **SQL injection** (OrderNumber, ImportNumber, Puerto, Proforma). Al tocar el reporte se debe pasar a `SqlParameter`. |
| H2 | Alta | El SP versionado en `scripts/Full Database Creation Script.sql` **no tiene** `@RequestedQuantityDifference` ni la columna `RequestedAmount` que el código ya usa. La versión productiva del SP no está en el repositorio. Hay que extraerla de la BD antes de modificarla. |
| H3 | Media | Filtro "Fecha real de recepción" se muestra en el encabezado del reporte pero **no se envía** al SP. |
| H4 | Media | Consulta: `RowExpand` hace 4 consultas por expansión; la grilla usa `Include` completos para listar (sin proyección). |
| H5 | Media | Acciones de la grilla ligadas a `StatusOrder == 1`; el estado 4 "Ajuste en aprobación" (EDIT_MODE=1) queda sin acciones en la grilla (se gestiona desde el tablero). Considerarlo en reglas de pago. |
| H6 | Baja | `PurchaseOrderService` instancia `Entities.PurchaseOrderAdjustmentLog` (la capa de aplicación conoce entidades de datos). |
| H7 | Baja | Typo de columna `EMPLOYE_ID` en `purchase_order_adjustment_log`. |

---

## 6. Preguntas abiertas para definir el requerimiento
1. **Valor a pagar**: ¿se captura en la Orden de Pago (valor factura del proveedor) o se agrega costo unitario + moneda al detalle de la OC? ¿Se toma FOB o CIF del maestro como sugerido?
2. **Moneda / TRM**: ¿pagos en USD y COP? ¿Se registra TRM del día de pago?
3. **Relación**: ¿una OP paga una OC, varias OC del mismo proveedor, o pagos parciales (anticipo / saldo) de una OC?
4. **Estados de OC que permiten pago**: ¿Pendiente (anticipos), Confirmada, ambas? ¿Qué pasa con OP si la OC se cancela o se ajusta?
5. **Ciclo de vida de la OP**: estados (Borrador, Aprobada, Pagada, Anulada...), aprobaciones y roles nuevos.
6. **Datos del proveedor**: ¿se requieren datos bancarios / condiciones de pago en `providers`?
7. **Soportes**: ¿adjuntar factura / comprobante?
8. **Cambios esperados en Consulta y Reporte de OC**: ¿columnas de valor pagado / saldo, filtro por estado de pago, pestaña de pagos?
