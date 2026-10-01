# Plan de trabajo – Inventario del artículo en la alarma de cantidades mínimas

**Estado:** ✅ Desarrollo terminado (2026-09-30). Aprobado por el cliente (2026-09-28). Plan aprobado por Andrés. Rama: `RQM-InventarioCompletoEnAlarmaDeCantidadMinima`.
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

## 2. Decisiones (cerradas por Andrés, 2026-09-28)

- **D1.** Solo la bandeja **"Sobrepaso de cantidades mínimas"** (`MinimumQuantityNotifications`). La de bodega local no se toca.
- **D2.** **Disponible = (Bodega Local + Zona Franca) + Tránsito − Comprometido**, con Comprometido = Reservas + Pedidos.
- **D3.** Se agrega la columna **Comprometido** (informativa).
- **D4.** (2026-09-30, en T1) Se agrega la columna **Stock Físico** = Bodega Local + Zona Franca, para diferenciarlo del Disponible.
- **D5.** (2026-09-30, en T1) Se corrige `SP_GET_MINIMUM_QUANTITY_ALARMS`: el tránsito de la bandeja solo sumaba OC Pendientes; ahora suma Pendientes + En aprobación (tipo 'O'), igual que el diálogo y el CSV de Fase 1. La alarma no cambia de firma ni de columnas.
- Columnas finales: Referencia · Bodega Local · Zona Franca · Stock Físico · Tránsito · Comprometido · Disponible. Solo referencias activas (la alarma tampoco se genera para referencias inactivas). La referencia de la alarma se resalta.

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
- El modelo es del caso de uso (no copia de entidades): `ReferenceId, ReferenceCode, ReferenceName, LocalWarehouse, FreeZone, PhysicalStock, InTransit, Committed, Available, IsAlarmReference`.
- `ImageDialog` y demás componentes compartidos **no se modifican**.
- Archivos nuevos en UTF-8 sin BOM.

## 4. Tareas

| # | Tarea | Entregable | Prueba (+ regresión acumulada) | Horas |
|---|---|---|---|---|
| **T1** | SP `SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId`: ubica el artículo de la referencia, devuelve sus **referencias activas** con Local, Zona Franca, Comprometida, Tránsito y Disponible (fórmula D2) y marca la referencia de la alarma | `scripts/RQM Inventario Articulo Alarma Minimos/01 - SP_GET_ITEM_REFERENCES_INVENTORY.sql` + `02 - Pruebas SP_GET_ITEM_REFERENCES_INVENTORY.sql` + `03 - Correccion SP_GET_MINIMUM_QUANTITY_ALARMS (transito).sql` (D5) | Ejecutar para 3 artículos (uno con varias referencias, uno con referencias inactivas, uno con tránsito) y comparar contra la fuente de D2 | 2 |
| **T2** | Capa de datos: entidad keyless + configuración + `IArticleInventoryRepository` / `ArticleInventoryRepository` con `SqlParameter` + registro DI | Código DataAccess | Compila; consulta desde el repositorio devuelve lo mismo que T1 · Regresión: T1 | 1 |
| **T3** | Caso de uso: `ArticleReferenceInventory` (Model), `IArticleInventoryService` / `ArticleInventoryService`, mapping y registro DI | Código Application.Services | Resultado del servicio = T1 para los 3 artículos · Regresión: T1–T2 | 1 |
| **T4** | Componente `ArticleInventoryDialog`: `ImageDialog` embebido + grilla (orden por referencia, fila de la alarma resaltada, números con formato N0, mensaje si no hay datos) | `Shared/ArticleInventoryDialog.razor(.cs)` | Abrir el diálogo con los 3 casos; imagen existente e inexistente · Regresión: T1–T3 | 2 |
| **T5** | Conectar en la(s) bandeja(s) de D1: solo cambia `ShowImageDialogAsync` → `ArticleInventoryDialog` (con `ReferenceId`) y el tamaño del diálogo | Cambio solo en `MinimumQuantityNotifications` | Desde el Tablero: clic en imagen muestra imagen + referencias; enlace del nombre sigue abriendo el reporte de movimientos; ocultar alarmas sigue funcionando · Regresión: T1–T4 | 1 |
| **T6** | Regresión final y cierre | Checklist | `ImageDialog` en otras bandejas (Agotados, OC confirmadas, Bodega local) y en Órdenes de Compra sigue igual; rendimiento del diálogo; notas para manual funcional | 1 |
| | | | **Total** | **8 h** |

## 4.1 Avance

| # | Estado | Evidencia |
|---|---|---|
| T1 | ✅ Cerrada (2026-09-30) | Script `02`: 25/25 OK (A1 21 refs, A2 con 10 inactivas, A3 con tránsito 8.000; C01–C04). Estructura sin columnas anulables. I02 = 0 (Local + ZF = INVENTORY_QUANTITY: cifras consistentes con el CSV de Fase 1). Índice por REFERENCE_ID ya existía (`IND_PURCHASE_ORDER_DETAIL_REFERENCE_ID`). A1 en 0 ms de CPU. Bandeja con D5: carga, búsqueda y ocultar alarmas OK; hoy no hay alarmas con OC en aprobación. Push hecho por Andrés. |
| T2 | ✅ Cerrada (2026-09-30) — compila, app y Tablero OK, regresión T1 25/25 (ver bitácora `05`) |
| T3 | ✅ Cerrada (2026-09-30) — compila, app arranca (DI validado), Tablero OK; servicio vs T1 se verifica en T4 |
| T4 + T5 | ✅ Cerradas (2026-09-30) — probadas juntas: diálogo (imagen + grilla, fila de la alarma resaltada, N0), cifras = SP y = bandeja, enlace al reporte, búsqueda, ocultar alarmas; `ImageDialog.razor` convertido a UTF-8 (tildes) |
| T6 | ✅ Cerrada (2026-09-30) — regresión de `ImageDialog` en otras bandejas, rendimiento < 1 s; notas para manuales en `06 - Notas tecnicas y funcionales...md` | `ArticleReferenceInventory` (record del caso de uso), `IArticleInventoryService` / `ArticleInventoryService` (orquestador + mapeo explícito, sin AutoMapper para no tocar el Profile compartido), registro DI. | Entidad `ItemReferenceInventory` (keyless), `HasNoKey` en `AldebaranDbContext`, `IArticleInventoryRepository` / `ArticleInventoryRepository` (`SqlParameter`), registro DI. |

## 5. Fuera de alcance
- Excel de la notificación periódica de cantidades mínimas por correo (se evaluará aparte).
- Cualquier cambio a `ImageDialog` o al resto de usos del popup de imagen.

## 6. Riesgos
- **Diferencia de cifras** entre el popup y lo que el usuario ve en el reporte, si D2 no se define con claridad → mitigado con D2 y la prueba de T1.
- **Artículos sin imagen:** `ImageDialog` ya muestra el mensaje; la grilla se muestra igual.
