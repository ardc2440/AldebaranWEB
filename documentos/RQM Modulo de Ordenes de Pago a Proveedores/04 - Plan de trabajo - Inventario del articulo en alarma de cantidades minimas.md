# Plan de trabajo – Inventario del artículo en la alarma de cantidades mínimas

**Estado:** Aprobado por el cliente (2026-09-28). Pendiente: aprobación del plan por Andrés y creación de la rama.
**Esfuerzo estimado:** 8 h (1 día de esfuerzo · 2 días calendario a 4 h/día).
**Método:** tareas mínimas comprobables → implementar → probar (con regresión acumulada) → corregir → OK de Andrés → siguiente.

---

## 1. Lo existente (revisado en el código, sin modificar)

| Elemento | Ubicación | Observación |
|---|---|---|
| Ventana de imagen | `Aldebaran.Web/Shared/ImageDialog.razor` | Componente compartido (≈100 usos). Recibe `ArticleName` y extrae la referencia interna entre `[ ]` para buscar `{codigo}.jpg`. **No se modifica.** |
| Bandeja "Sobrepaso de cantidades mínimas" | `Pages/DashboardNotificationComponents/MinimumQuantityNotifications.razor(.cs)` | Botón de imagen → `ShowImageDialogAsync(detail.ArticleName)` abre `ImageDialog`. El nombre abre el Reporte de movimientos (se mantiene). |
| Bandeja "Cantidades mínimas en bodega local" | `MinimumLocalWarehouseQuantityNotifications.razor(.cs)` | Mismo patrón. **Ver decisión D1.** |
| Modelo de la alarma | `Application.Services/Models/MinimumQuantityArticle` | Trae `ReferenceId` (suficiente para ubicar el artículo y sus referencias). |
| Cálculo de inventario de referencia | Fase 1: `SP_CSV_EXPORT_INVENTORY` | Stock Físico = Local + Zona Franca · Comprometida = Pedidos + Reservas · Tránsito = OC Pendientes + En aprobación · **Disponible = Físico + Tránsito − Comprometida**. |

## 2. Decisiones previas (resolver antes de T1)

- **D1. ¿Qué bandeja(s)?** Solo "Sobrepaso de cantidades mínimas", o también "Cantidades mínimas en bodega local". Propuesta: ambas, con el mismo componente (≈+0.5 h por la segunda, solo el cambio de llamada).
- **D2. Fórmula de "Disponible".** Hay tres criterios distintos en el sistema:
  - Bandeja actual: `Inventario + Tránsito − Pedido`.
  - `SP_GET_INVENTORY_REPORT`: `Inventario − Reservado − Pedido` (sin tránsito; además filtra solo artículos `IS_EXTERNAL_INVENTORY = 1`).
  - Interfaz CSV Fase 1 (definición validada con el cliente): `Físico + Tránsito − (Pedidos + Reservas)`.
  **Propuesta:** usar la definición de la Fase 1 (es la acordada funcionalmente y trae Local / Zona Franca / Tránsito por separado). La propuesta dice "mismos criterios del reporte de inventario": confirmar con Andrés cuál se considera "el reporte" para el cliente.
- **D3. Columnas.** Referencia · Disponible · Bodega Local · Zona Franca · Tránsito (según lo solicitado). ¿Se agrega Comprometida para que el Disponible sea explicable? Propuesta: sí, como columna informativa (sin costo relevante).

## 3. Diseño

```
MinimumQuantityNotifications (UI, solo cambia la llamada)
        │ DialogService.OpenAsync<ArticleInventoryDialog>(ReferenceId, ArticleName)
        ▼
ArticleInventoryDialog (UI nuevo)
   ├─ <ImageDialog ArticleName="..." />   ← reutilizado tal cual
   └─ Grilla: referencias activas del artículo, fila de la alarma resaltada
        │ IArticleInventoryService.GetByReferenceAsync(referenceId)
        ▼
Application.Services: ArticleInventoryService (caso de uso) + Model ArticleReferenceInventory
        ▼
DataAccess.Infraestructure: ArticleInventoryRepository (SqlParameter, sin concatenar)
        ▼
SQL: SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId  (nuevo, solo lectura)
```

- Clean Architecture: la UI solo conoce `Application.Services`.
- El modelo es del caso de uso (no copia de entidades): `ReferenceId, ReferenceCode, ReferenceName, LocalWarehouse, FreeZone, InTransit, Committed, Available, IsAlarmReference`.
- `ImageDialog` y demás componentes compartidos **no se modifican**.
- Archivos nuevos en UTF-8 sin BOM.

## 4. Tareas

| # | Tarea | Entregable | Prueba (+ regresión acumulada) | Horas |
|---|---|---|---|---|
| **T1** | SP `SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId`: ubica el artículo de la referencia, devuelve sus **referencias activas** con Local, Zona Franca, Comprometida, Tránsito y Disponible (fórmula D2) y marca la referencia de la alarma | `scripts/RQM Inventario Articulo Alarma Minimos/01 - SP_GET_ITEM_REFERENCES_INVENTORY.sql` + `02 - Pruebas.sql` | Ejecutar para 3 artículos (uno con varias referencias, uno con referencias inactivas, uno con tránsito) y comparar contra la fuente de D2 | 2 |
| **T2** | Capa de datos: entidad keyless + configuración + `IArticleInventoryRepository` / `ArticleInventoryRepository` con `SqlParameter` + registro DI | Código DataAccess | Compila; consulta desde el repositorio devuelve lo mismo que T1 · Regresión: T1 | 1 |
| **T3** | Caso de uso: `ArticleReferenceInventory` (Model), `IArticleInventoryService` / `ArticleInventoryService`, mapping y registro DI | Código Application.Services | Resultado del servicio = T1 para los 3 artículos · Regresión: T1–T2 | 1 |
| **T4** | Componente `ArticleInventoryDialog`: `ImageDialog` embebido + grilla (orden por referencia, fila de la alarma resaltada, números con formato N0, mensaje si no hay datos) | `Shared/ArticleInventoryDialog.razor(.cs)` | Abrir el diálogo con los 3 casos; imagen existente e inexistente · Regresión: T1–T3 | 2 |
| **T5** | Conectar en la(s) bandeja(s) de D1: solo cambia `ShowImageDialogAsync` → `ArticleInventoryDialog` (con `ReferenceId`) y el tamaño del diálogo | Cambio en `MinimumQuantityNotifications` (y `MinimumLocalWarehouseQuantityNotifications` si D1) | Desde el Tablero: clic en imagen muestra imagen + referencias; enlace del nombre sigue abriendo el reporte de movimientos; ocultar alarmas sigue funcionando · Regresión: T1–T4 | 1 |
| **T6** | Regresión final y cierre | Checklist | `ImageDialog` en otras bandejas (Agotados, OC confirmadas, Bodega local) y en Órdenes de Compra sigue igual; rendimiento del diálogo; notas para manual funcional | 1 |
| | | | **Total** | **8 h** |

## 5. Fuera de alcance
- Excel de la notificación periódica de cantidades mínimas por correo (se evaluará aparte).
- Cualquier cambio a `ImageDialog` o al resto de usos del popup de imagen.

## 6. Riesgos
- **Diferencia de cifras** entre el popup y lo que el usuario ve en el reporte, si D2 no se define con claridad → mitigado con D2 y la prueba de T1.
- **Artículos sin imagen:** `ImageDialog` ya muestra el mensaje; la grilla se muestra igual.
