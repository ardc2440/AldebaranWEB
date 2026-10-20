# Bitácora de pruebas – Última OC confirmada en el inventario del artículo

**RQM:** Opción 2 de la propuesta *Histórico de Precios y Proveedor en Inventario de Mínimos* · ajuste al RQM "Inventario del artículo en la alarma de cantidades mínimas".
**Ambiente:** base de datos de PRUEBAS · **Plan:** `01 - Plan de trabajo - Ultima OC confirmada en inventario del articulo.md`
**Método:** cada tarea se prueba y repite la regresión de las anteriores. Evidencias en `Evidencias\`.

---

## T12 – SP `SP_GET_ITEM_REFERENCES_INVENTORY` con la última OC confirmada ✅ (2026-10-10)

Script `04` aplicado por Claude en la BD de pruebas (autorizado por Andrés). Script `02` ampliado y ejecutado (mensajes capturados en tabla temporal).

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| A1 | MU-35 (ref. 2257, 21 refs): filas, cifras + última OC vs control `ROW_NUMBER` | Todo coincide | 21 = 21, 0 diferencias (sin OC confirmadas) | OK |
| A2 | LÁPIZ METALLIC (ref. 1335): solo activas | 1 activa | 1; 10 inactivas excluidas | OK |
| A3 | MU-303-1. (ref. 2094): tránsito y última OC | Tránsito > 0; OC = control | Tránsito 8.000; 4 refs con OC = control | OK |
| C01–C03 | Inexistente, NULL, inactiva | Igual que antes | 0, 0, 1 = 1 | OK |
| C04 | Estructura | 15 columnas; las 5 nuevas anulables | `ProviderName varchar(50) null`, `PurchaseOrderDate date null`, `RealReceiptDate date null`, `RequestedQuantity int null`, `ReceivedQuantity int null` | OK |
| U01 | Referencia con varias OC confirmadas (4821, 8 OC) | La de mayor fecha real de recibo | TOP ULTIMATE · OC 2025-03-27 · recibo 2025-07-18 · 20.000 / 20.000 | OK |
| U02 | Referencia sin OC confirmada (124) | 5 columnas NULL | NULL | OK |
| U03 | OC no confirmada más reciente que la confirmada (120) | La ignora | Toma la confirmada | OK |
| U04 | Consistencia en A1–A3 | Con proveedor → fechas y solicitada informadas | 0 incompletas (4 con OC, 22 sin OC) | OK |
| P01 | Rendimiento: 300 llamadas al SP | Sin impacto notable en el correo | 2,6 s (≈9 ms/llamada) | OK |

**Resultado:** 30/30 OK (incluye la regresión de los 25 casos de T1).

---

## T13 – Entidad, modelo del caso de uso y mapeo ✅ (2026-10-10)

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| T13-01 | Compilar | Sin errores | Correcta (Andrés) | OK |
| T13-02 | Diálogo de inventario (HO-13, 18 refs) | Carga igual que antes; EF mapea 15 columnas | Igual; sin `errors_` del día | OK |
| T13-03 | Bandeja "Sobrepaso de cantidades mínimas" | Carga igual | 416 alarmas | OK |

---

## T14 – Diálogo del Tablero: columna "Proveedor última compra" + tooltip ✅ (2026-10-10)

Decisión D17 (Andrés): solo la columna Proveedor; los datos de la OC en tooltip al pasar el cursor, para no ensanchar el diálogo (1000 → 1150 px).

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| T14-01 | Compilar | Sin errores | Correcta (Andrés) | OK |
| T14-02 | VA-434 (9 refs): columna Proveedor | = SP; vacía sin OC | 6 con TOP ULTIMATE…, 3 vacías (Amarillo/alarma, Morado, Rojo) | OK |
| T14-03 | Tooltip Blanco | = SP | 06/06/2024 · 04/10/2024 · 3.000 / 3.000 | OK |
| T14-04 | Tooltip Negro (cambio de fila) | = SP | 08/10/2024 · 14/02/2025 · 5.000 / 5.000 | OK |
| T14-05 | Tooltip sobre el diálogo | Visible, no queda detrás | Visible | OK |
| T14-06 | Regresión: fila de alarma resaltada, cifras, imagen | Igual | Igual | OK |
| T14-07 | Sesión tras recompilar/reiniciar | Se mantiene (hotfix Data Protection) | Se mantuvo | OK |

---

## T15 – Excel del correo periódico ✅ (2026-10-10)

Preparación: `visualized_minimum_quantity_alarms` del empleado 27 (gramirez) borradas en BD de pruebas (421 filas, autorizado por Andrés); adiaz (22) intacto como control. `ExecutionHours` 18:06 + sábado habilitado (solo en el repo, para la prueba).

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| T15-01 | Ejecución del servicio | Envía correo solo a gramirez | 18:06:00 → 18:06:30 (30,6 s, ambiente local); 1 correo | OK |
| T15-02 | Columnas K–O | 5 columnas nuevas con encabezados | Proveedor última compra, Fecha OC, Fecha real de recibo, Cantidad solicitada, Cantidad recibida | OK |
| T15-03 | Filas padre | Columnas nuevas vacías | Vacías | OK |
| T15-04 | Filas hijas ASHBY | = SP | Azul/Blanco/Negro: NINGBO CONCCORD · 22/01/2025 · 20/06/2025 · 5000/5000; Gris Oscuro (alarma, sin OC) vacía | OK |
| T15-05 | Regresión: "+", imagen solo en padre, cifras | Igual | Igual | OK |

Evidencia: `Evidencias/T15 - Excel correo filas hijas con ultima OC.png` (Andrés).
Nota: el enlace "Marcar como leídas" no se modificó (validado en producción con el hotfix); gramirez queda con alarmas sin leer en pruebas.
Observación aceptada: la columna Proveedor queda ancha (el generador compartido calcula el ancho por contenido; no se modifica).
