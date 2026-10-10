# Plan de trabajo – Última OC confirmada en el inventario del artículo

**Estado:** 📝 Pendiente de aprobación (2026-10-10). Ajuste al RQM "Inventario del artículo en la alarma de cantidades mínimas" (plan `04`, en producción desde 2026-10-01).
**Rama:** por confirmar (Andrés).
**Esfuerzo estimado:** 5 h.
**Método:** tareas mínimas comprobables → implementar → probar (con regresión acumulada) → corregir → OK de Andrés → siguiente.

---

## 1. Requerimiento

A cada referencia que se muestra en el **Inventario del artículo** se agregan los datos de la **última orden de compra Confirmada** de esa referencia (la que afectó el inventario):

| Columna | Origen |
|---|---|
| Proveedor | `purchase_orders.PROVIDER_ID` → `providers.PROVIDER_NAME` |
| Fecha OC | `purchase_orders.REQUEST_DATE` |
| Fecha real de recibo | `purchase_orders.REAL_RECEIPT_DATE` |
| Cantidad solicitada | `purchase_order_details.REQUESTED_QUANTITY` |
| Cantidad recibida | `purchase_order_details.RECEIVED_QUANTITY` |

Aplica a los dos frentes:
1. **Diálogo del Tablero** (`ArticleInventoryDialog`): columnas nuevas en todas las filas.
2. **Excel del correo periódico**: columnas nuevas **solo en las filas hijas** (la fila padre repite la referencia de la alarma, que ya está en el detalle).

## 2. Decisiones (Andrés, 2026-10-10)

- **D11.** OC considerada: estado **Confirmada** (`STATUS_ORDER = 2`, tipo 'O'). "Más reciente" = mayor `REAL_RECEIPT_DATE`; desempate por `PURCHASE_ORDER_ID` mayor.
- **D12.** Una OC tiene un solo registro por referencia (verificado en BD: 0 duplicados OC+referencia) → no se suma. Si varias referencias comparten la misma OC, el proveedor se repite en cada una (aceptado).
- **D13.** Referencia sin OC confirmada → las cinco columnas vacías.
- **D14.** No se muestra el número de la OC.
- **D15.** Excel: columnas solo en filas hijas; en la fila padre quedan vacías.

Datos verificados (BD de pruebas, 2026-10-10): 460 OC confirmadas, todas con `REAL_RECEIPT_DATE`.

## 3. Diseño

Se extiende lo existente, sin componentes nuevos:

```
SP_GET_ITEM_REFERENCES_INVENTORY  (+5 columnas, OUTER APPLY TOP 1 por referencia)
   → ItemReferenceInventory (entidad keyless, +5 propiedades anulables)
   → ArticleReferenceInventory (record del caso de uso, +5)
   → ArticleInventoryDialog (grilla, +5 columnas)
   → InventoryMinimumGroupedExportDto (+5) — solo BuildChild las llena
```

- Columnas nuevas **anulables** (D13); las existentes conservan `ISNULL`.
- `OUTER APPLY (SELECT TOP 1 … ORDER BY REAL_RECEIPT_DATE DESC, PURCHASE_ORDER_ID DESC)` apoyado en el índice existente `IND_PURCHASE_ORDER_DETAIL_REFERENCE_ID`.
- Sin cambios en `SP_GET_MINIMUM_QUANTITY_ALARMS`, `ImageDialog` ni `FileBytesGeneratorService`.
- Formato: fechas `dd/MM/yyyy`, cantidades N0.

## 4. Tareas

| # | Tarea | Entregable | Prueba (+ regresión acumulada) | Horas |
|---|---|---|---|---|
| **T12** | SP: agregar `ProviderName, PurchaseOrderDate, RealReceiptDate, RequestedQuantity, ReceivedQuantity` (D11–D13) | `scripts/RQM Inventario Articulo Alarma Minimos/04 - SP_GET_ITEM_REFERENCES_INVENTORY (ultima OC).sql` + ampliar `02 - Pruebas…` con casos U01–U0n | Artículo con varias OC confirmadas (toma la más reciente), referencia sin OC (nulos), OC pendiente/cancelada más reciente que la confirmada (se ignora), varias referencias en la misma OC · Regresión: casos C/I existentes 25/25 | 1,5 |
| **T13** | Datos + caso de uso: entidad, record y mapeo explícito | `ItemReferenceInventory`, `ArticleReferenceInventory`, `ArticleInventoryService` | Compila; servicio = SP · Regresión: T12 | 0,5 |
| **T14** | Diálogo del Tablero: 5 columnas en la grilla, ancho del diálogo ajustado | `ArticleInventoryDialog.razor` (y tamaño en `MinimumQuantityNotifications` si se requiere) | Datos = SP; referencia sin OC con celdas vacías; fila de alarma sigue resaltada; imagen OK · Regresión: T12–T13, bandeja (búsqueda, ocultar, enlace a movimientos) | 1 |
| **T15** | Excel del correo: DTO +5, `BuildChild` las llena, padre vacío | `InventoryMinimumGroupedExportDto`, `InventoryMinimumGroupedExportBuilder` | Correo de prueba: hijas con datos = diálogo, padre vacío, "+" expande, imagen solo en padre, "Marcar como leídas" OK · Regresión: T12–T14 | 1 |
| **T16** | Cierre | Bitácora `05`, notas `06`, actualización de manuales técnico y funcional | — | 1 |
| | | | **Total** | **5 h** |

## 5. Riesgos
- **Ancho del diálogo:** pasa de 7 a 12 columnas → revisar legibilidad (posible ancho mayor o texto de proveedor con elipsis + tooltip).
- **Rendimiento del correo:** una llamada al SP por artículo (≈300); el `OUTER APPLY` usa índice por referencia → medir en T15 contra la línea base (~19 s).

## 6. Fuera de alcance
- Número de OC (D14), historial de OC, cambios a la bandeja de bodega local.
