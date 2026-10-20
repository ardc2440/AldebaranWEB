# Notas técnicas y funcionales – Última OC confirmada (versión 1.1)

Ajuste al RQM "Inventario del artículo en la alarma de cantidades mínimas" (Opción 2 de la propuesta *Histórico de Precios y Proveedor en Inventario de Mínimos*). Rama: `RQM_ProveedorEnInventarioCantidadMinima`. Manuales técnico y funcional versión 1.1 en esta carpeta (la versión 1.0 entregada queda en `RQM Modulo de Ordenes de Pago a Proveedores`).

## Regla de negocio
- **Última compra** = OC tipo 'O' en estado **Confirmada** (`STATUS_ORDER = 2`) con la mayor `REAL_RECEIPT_DATE`; desempate `PURCHASE_ORDER_ID` mayor (D11).
- Un detalle por referencia en cada OC → no se suman cantidades (D12). Sin OC confirmada → datos vacíos (D13). Sin número de OC (D14).

## Cambios por capa
| Capa | Archivo | Cambio |
|---|---|---|
| BD | `scripts/RQM Inventario Articulo Alarma Minimos/04 - SP_GET_ITEM_REFERENCES_INVENTORY (ultima OC).sql` | +5 columnas anulables vía `OUTER APPLY TOP 1` |
| BD (pruebas) | `.../02 - Pruebas SP_GET_ITEM_REFERENCES_INVENTORY.sql` | Control con `ROW_NUMBER`, C04 a 15 columnas, U01–U04 |
| DataAccess | `Entities/ItemReferenceInventory.cs` | +5 propiedades (`string?`, `DateTime?`, `int?`) |
| Application | `Models/ArticleReferenceInventory.cs`, `Services/ArticleInventoryService.cs` | Record +5 parámetros; mapeo explícito |
| Application | `InventoryMinimumAlerts/Models/InventoryMinimumGroupedExportDto.cs`, `InventoryMinimumGroupedExportBuilder.cs` | +5 columnas; solo `BuildChild` las llena (D15) |
| Web | `Shared/ArticleInventoryDialog.razor(.cs)` | Columna "Proveedor última compra" + tooltip con `TooltipService` (D17) |
| Web | `Pages/DashboardNotificationComponents/MinimumQuantityNotifications.razor.cs` | Diálogo 1000 → 1150 px |

Sin cambios: `SP_GET_MINIMUM_QUANTITY_ALARMS`, `ImageDialog`, `FileBytesGeneratorService`, generación del enlace "Marcar como leídas".

## Despliegue
1. Ejecutar el script `04` **antes** de publicar (la entidad espera 15 columnas).
2. Opcional: script `02` → TODAS LAS PRUEBAS OK.
3. Publicar Aldebaran.Web. No requiere cambios de appsettings.

**Compatibilidad:** la aplicación 1.0 funciona con el SP 1.1 (EF ignora columnas adicionales); la 1.1 **no** funciona con el SP 1.0.

## Para el manual funcional (incluido en la versión 1.1)
- Diálogo: columna Proveedor; detalle al pasar el cursor (Figura 4).
- Excel: 5 columnas al final, solo en las filas del detalle (Figura 7).
- Preguntas frecuentes: criterio de "última compra", proveedor vacío, mismo proveedor en varias referencias.

## Observaciones
- Columna Proveedor del Excel ancha (ancho calculado por el generador compartido); aceptado, no se modifica.
- En pruebas, gramirez (27) quedó con sus alarmas sin leer tras la prueba de T15.
