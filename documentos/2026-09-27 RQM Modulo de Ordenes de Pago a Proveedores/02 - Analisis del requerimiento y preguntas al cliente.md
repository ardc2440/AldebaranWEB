# RQM Módulo de Órdenes de Pago a Proveedores
## 02 - Análisis del requerimiento del cliente vs. lo existente

Fecha: 2026-09-26 · Base: `01 - Analisis de lo existente - Modulo Ordenes de Compra.md`

---

## 1. Requerimiento interpretado por bloques

### B1. Valores en la Orden de Compra (Crear / Editar OC)
| Dato | Dónde | Regla |
|---|---|---|
| Fecha estimada de pago | Encabezado OC | Requerido |
| Valor adicional de costo | Encabezado OC | Requerido (≥ 0) |
| Valor total proforma | Encabezado OC | Requerido (> 0) |
| Valor por unidad | Cada referencia (detalle) | Requerido (> 0) |
| Subtotal (Valor unidad × Cantidad solicitada) | Detalle | Informativo, calculado (no se persiste) |
| **Validación al guardar** | OC | Σ(Valor unidad × Cant. solicitada) = Total proforma − Valor adicional |

Impacto: `purchase_orders` (+3 columnas), `purchase_order_details` (+1 columna), páginas Add/Edit OC y AddPurchaseOrderDetail/EditPurchaseOrderDetail, CSV de interfaz de OC (evaluar).

### B2. Pagos (nuevo módulo)
- Varios pagos por OC; **Σ pagos vigentes ≤ Valor total de la OC**.
- Un pago nuevo **no puede superar el saldo por pagar**.
- Un pago se puede **anular** y el saldo se restituye.
- Base del valor a pagar: **cantidad solicitada** (proforma), no la recibida *(pendiente de confirmar, ver P1)*.
- Registro del pago: Número, OC, Valor, Estado (Pendiente, Ejecutado, Anulado), creación/aprobación(+motivo)/ejecución/anulación(+motivo) con fecha y funcionario.
- Devolución de un pago ya ejecutado por parte del proveedor: caso excepcional (~1 vez cada 5 años).

### B3. Notificaciones Windows
- Para el perfil (rol) correspondiente: "OC pendientes de pago".
- Se activan desde la **fecha estimada de pago** mientras exista **saldo pendiente**.
- ✅ Ya existe infraestructura: `notification_definitions` + `notification_definition_roles` + `NotificationProcessingService` (creada en RQM Aprobación Ajustes OC). Se agrega una definición nueva; no se construye un mecanismo nuevo.

### B4. Dashboard de pagos (semáforo)
- OC con fecha estimada de pago próxima o vencida, con saldo pendiente. Verde / Amarillo / Rojo por días.
- Se propone como componente del **Tablero de notificaciones** existente (`DashboardNotificationComponents`) o página propia *(ver P9)*.

### B5. Consulta de OC
- Nueva pestaña **"Pagos realizados"** en el detalle expandible de `/purchase-orders`.

### B6. Reportes
- Nuevo: **OC pendientes de pago**.
- Nuevo: **Pagos a proveedores**.
- (Sugerido) Reporte OC existente: incluir valores — y corregir de paso la inyección SQL (H1) y el SP no versionado (H2).

---

## 2. Reglas de negocio que el requerimiento no define (propuesta de Claude)

| # | Tema | Propuesta |
|---|---|---|
| R1 | Saldo | **Comprometido** = Σ pagos Pendientes + Ejecutados. **Pagado** = Σ Ejecutados. **Saldo por pagar** = Total OC − Comprometido. Así dos pagos pendientes no pueden sumar más que el total. |
| R2 | Ciclo de vida | Pendiente →(Aprobar)→ Pendiente aprobado →(Ejecutar)→ Ejecutado. Pendiente / Ejecutado →(Anular con motivo)→ Anulado. Anulado es final. |
| R3 | Devolución del proveedor | Sin estado nuevo: se modela como **anulación de un pago Ejecutado** con motivo "Devolución del proveedor". El saldo se restituye. (YAGNI por la frecuencia indicada.) |
| R4 | Editar OC con pagos | Si la OC se modifica (cantidades, valores), el nuevo total no puede quedar por debajo de lo comprometido. |
| R5 | Cancelar OC con pagos | No se permite cancelar una OC con pagos Pendientes o Ejecutados vigentes; primero se anulan. |
| R6 | OC históricas | **Decisión Andrés (2026-09-26):** todas las OC (nuevas y antiguas, en cualquier estado) quedan habilitadas para pago. Ver sección 2.1. |
| R7 | Número de pago | Secuencia propia (como `ORDER_NUMBER`). |
| R8 | Precisión | `DECIMAL(18,2)`; comparación de la regla B1 exacta a 2 decimales. |
| R9 | Roles nuevos | Creación, Aprobación, Ejecución y Anulación de pagos a proveedores + Consulta. Rol destinatario de la notificación Windows. |

### 2.1 Decisión: OC sin datos de pago (históricas) — definido por Andrés 2026-09-26
1. El **primer paso para crear un pago es seleccionar la OC** a pagar.
2. Si la OC no tiene los datos de control de pago (fecha estimada de pago, valor adicional, total proforma, valor unitario por referencia), el sistema:
   - **Bloquea la creación del pago**, y
   - **Genera una alarma/alerta** a un responsable para que complete los datos.
3. La OC se debe modificar para registrar esos datos. **Una OC Confirmada podrá modificarse solo para estos datos**, y solo en el contexto de la creación de un pago (edición restringida: no cambia cantidades, bodegas, fechas logísticas ni proveedor).
4. Implicaciones técnicas a resolver en el diseño:
   - La edición restringida no debe disparar el flujo actual de modificación de OC (notificación a clientes afectados, alarmas de tránsito, triggers de inventario). Se propone un caso de uso propio "Registrar datos de pago de la OC" con su propio log.
   - La validación Σ(valor unidad × cant. solicitada) = proforma − adicional aplica también a la edición restringida.
   - En la BD las columnas nuevas quedan **nullables** (por las OC existentes); la obligatoriedad se controla en la aplicación (crear OC y crear pago).

### 2.2 Decisión: documento de la Orden de Pago
- Una vez generada, la Orden de Pago se puede **imprimir y descargar en PDF** con el mismo mecanismo de los demás documentos (`getContent` → `FileBytesGeneratorService.GetPdfBytes`).
- Si fue aprobada, el documento muestra **quién la aprobó** (y fecha/motivo). Es el **documento soporte del pago al proveedor**.

---

## 3. Preguntas para el cliente

**P1.** El cliente lo dejó como pregunta: *¿el pago se maneja siempre por la cantidad solicitada, sin importar la cantidad recibida al confirmar?* Se necesita la respuesta. Si hay faltantes al recibir, ¿se paga igual el total de la proforma?
**P2.** Moneda: la proforma y los pagos, ¿en qué moneda? ¿USD/COP? ¿se registra TRM? (Hoy la OC no tiene moneda; `items` sí maneja `CURRENCY_ID`.)
**P3.** "Valor adicional de costo": ¿qué incluye (fletes, seguros, gastos bancarios)? ¿Es un solo valor o se detalla?
**P4.** Aprobación: el registro tiene fecha/usuario/motivo de aprobación pero no hay estado "Aprobado". ¿Todo pago requiere aprobación antes de ejecutarse? ¿Quién aprueba? ¿Por qué un motivo en la aprobación?
**P5.** "Motivos de anulación deben ser aprobados y texto libre": ¿significa (a) lista de motivos parametrizada ("aprobados") + texto libre de observación, o (b) la anulación requiere aprobación de un superior + motivo libre?
**P6.** ¿Se permite anular un pago Ejecutado (devolución) o solo Pendientes? ¿Mismo rol?
**P7.** ¿En qué estados de la OC se permite registrar pagos: Pendiente (anticipos), Confirmada, Ajuste en aprobación?
**P8.** Semáforo: ¿cuántos días definen Verde / Amarillo / Rojo? ¿Parametrizables por el usuario?
**P9.** Dashboard: ¿dentro del Tablero de notificaciones actual o como página independiente?
**P10.** ¿Se adjunta soporte del pago (comprobante, factura)? ¿Se registra medio de pago / cuenta / referencia bancaria?
**P11.** Notificación Windows: ¿frecuencia de validación? ¿para qué rol(es)?
**P12.** Reportes: ¿filtros y columnas esperados? ¿exportación PDF como los demás?
**P14.** ¿A quién se envía la alerta de "OC sin datos de pago" (rol)? ¿Por alarma del sistema, notificación Windows o ambas? ¿Quién completa los datos: quien crea el pago o un rol de compras?
**P15.** ¿Se permite completar datos de pago en una OC **Cancelada**? (Se propone: no.)
**P16.** ¿Qué formato/encabezado lleva el documento impreso de la Orden de Pago (logo, firmas, datos del proveedor)?
**P13.** ¿La interfaz CSV de OC (Fase 1) debe incluir los nuevos valores?

---

## 4. Impacto estimado (sin plan aún)
- **BD**: ALTER `purchase_orders` (+3), `purchase_order_details` (+1); tablas nuevas `purchase_order_payments` (+ log si aplica), motivos de anulación, parámetros de semáforo; estados de pago en `document_types`/`status_document_types` (tipo nuevo) o tabla propia; roles; `notification_definitions`; SPs de reportes y dashboard.
- **Backend**: entidades/configuración EF, repositorios, servicios de caso de uso (Registrar, Aprobar, Ejecutar, Anular pago; Consultar saldo).
- **UI**: Add/Edit OC y detalle, Confirmar OC (solo lectura de valores), pestaña Pagos, páginas de pago, dashboard, 2 reportes, menú.
