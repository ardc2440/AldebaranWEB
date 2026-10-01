# Notas técnicas y funcionales – Inventario del artículo en la alarma de cantidades mínimas

Insumo para los manuales técnico y funcional (Word, línea visual de la Fase 1). Rama `RQM-InventarioCompletoEnAlarmaDeCantidadMinima`.

## Funcional

**Dónde:** Tablero de notificaciones → bandeja **"Sobrepaso de cantidades mínimas por referencia"** → botón de imagen de una alarma (tooltip "Ver imagen e inventario del artículo").

**Qué muestra:**
1. Nombre del artículo y referencia de la alarma, e imagen del artículo (o el mensaje "La imagen para este artículo no está disponible.").
2. **Inventario del artículo**: todas las referencias **activas** del artículo, ordenadas por nombre, con:

| Columna | Significado |
|---|---|
| Bodega Local | Existencia en la bodega local |
| Zona Franca | Existencia en zona franca |
| Stock Físico | Bodega Local + Zona Franca |
| Tránsito | Órdenes de compra Pendientes + En aprobación |
| Comprometido | Reservas + Pedidos |
| Disponible | Stock Físico + Tránsito − Comprometido (verde si es positivo, rojo si es 0 o negativo) |

3. La referencia de la alarma aparece **resaltada** (negrita, fondo amarillo).

**Sin cambios:** el clic en el nombre sigue abriendo el Reporte de movimientos; búsqueda, paginación y ocultar alarmas funcionan igual. Las demás bandejas siguen mostrando solo la imagen.

**Coherencia de cifras:** el Disponible y el Tránsito de la fila resaltada coinciden con las columnas de la bandeja; el Stock Físico coincide con su columna "Inventario".

## Técnico

```
MinimumQuantityNotifications (UI) ── DialogService.OpenAsync<ArticleInventoryDialog>(ReferenceId, ArticleName)
  └─ ArticleInventoryDialog (Shared) ── ImageDialog (reutilizado) + LocalizedDataGrid
       └─ IArticleInventoryService / ArticleInventoryService (Application.Services; Model ArticleReferenceInventory)
            └─ IArticleInventoryRepository / ArticleInventoryRepository (DataAccess.Infraestructure; entidad keyless ItemReferenceInventory)
                 └─ dbo.SP_GET_ITEM_REFERENCES_INVENTORY @ReferenceId
```

| Capa | Archivo | Tipo |
|---|---|---|
| BD | `scripts/RQM Inventario Articulo Alarma Minimos/01 - SP_GET_ITEM_REFERENCES_INVENTORY.sql` | Nuevo (SP + índice condicional; el índice ya existía en BD) |
| BD | `.../02 - Pruebas SP_GET_ITEM_REFERENCES_INVENTORY.sql` | Nuevo (pruebas automáticas, solo lectura) |
| BD | `.../03 - Correccion SP_GET_MINIMUM_QUANTITY_ALARMS (transito).sql` | Corrección (D5): tránsito = Pendientes + En aprobación, tipo 'O' |
| DataAccess | `Entities/ItemReferenceInventory.cs` + `HasNoKey` en `AldebaranDbContext` | Nuevo |
| Infraestructura | `Repository/IArticleInventoryRepository.cs`, `ArticleInventoryRepository.cs` | Nuevo (`SqlParameter` tipado) |
| Application | `Models/ArticleReferenceInventory.cs`, `Services/IArticleInventoryService.cs`, `ArticleInventoryService.cs` | Nuevo (record del caso de uso, mapeo explícito) |
| Web | `Shared/ArticleInventoryDialog.razor(.cs)` | Nuevo |
| Web | `Pages/DashboardNotificationComponents/MinimumQuantityNotifications.razor(.cs)` | Modificado (abre el nuevo diálogo) |
| Web | `Shared/ImageDialog.razor` | Codificación ISO-8859 → UTF-8 (contenido igual) |
| Web | `Extensions/ArchitectureBuilderExtensions.cs` | Registro DI (repositorio y servicio) |

**Decisiones:** D1 solo bandeja de mínimos generales · D2 fórmula de Disponible · D3 columna Comprometido · D4 columna Stock Físico · D5 corrección del tránsito de la alarma. Componentes compartidos sin cambios funcionales. Archivos nuevos en UTF-8 sin BOM.

## Ampliación (T7–T9): Excel del correo periódico de cantidades mínimas

**Funcional:** el adjunto `InventarioMinimo_yyyyMMdd HHmm.xlsx` ahora muestra **un grupo por artículo**:
- Fila de la alarma (visible): imagen del artículo, "En alarma: Sí", Cantidad mínima y las cifras de esa referencia.
- Con el **"+"** de la izquierda se despliega el inventario de **todas las referencias activas del artículo** (sin imagen), con las mismas columnas y fórmulas del diálogo del Tablero. Las referencias que también están en alarma aparecen marcadas ("Sí" y su Cantidad mínima).
- Si un artículo tiene varias alarmas, aparece una sola vez con todas marcadas en el detalle. "Marcar alarmas como leídas" sigue desmarcando **todas** las alarmas incluidas en el correo.
- Columnas: Artículo / Referencia · Imagen · En alarma · Cantidad mínima · Bodega Local · Zona Franca · Stock Físico · En tránsito · Comprometido · Disponible. El Disponible ahora también resta las Reservas (igual que el Tablero).

**Técnico:**

| Capa | Archivo | Tipo |
|---|---|---|
| Common | `Utils/IFileBytesGeneratorService.cs` | Adición de `GetExcelBytesWithChildRows<T>(parents, childrenSelector)` |
| Common | `Utils/FileBytesGeneratorService.cs` | Solo `partial` (el método `GetExcelBytes` no cambia) |
| Common | `Utils/FileBytesGeneratorService.ChildRows.cs` | Nuevo: outline de Excel (hijos nivel 1 ocultos, resumen arriba, hoja sin protección, imagen solo en el padre) |
| Application | `InventoryMinimumAlerts/Models/InventoryMinimumGroupedExportDto.cs` | Nuevo (columnas + `Children`) |
| Application | `InventoryMinimumAlerts/InventoryMinimumGroupedExportBuilder.cs` | Nuevo (armado puro de grupos) |
| Application | `InventoryMinimumAlerts/InventoryMinimumAlertService.cs` | Inyecta `IArticleInventoryService`; agrupa por artículo; usa `GetExcelBytesWithChildRows` |

Decisiones D6–D10 (ver plan `04`).

## Corrección (T11): pestaña inicial del Tablero

**Funcional:** al entrar al Tablero se selecciona la **primera pestaña que el usuario tiene permiso de ver**. Antes, un usuario sin el rol "Aprobación de ajustes en órdenes de compra" veía el contenido de esa pestaña aunque no tuviera el botón. Sin ninguna pestaña permitida, no se muestra contenido.

**Técnico:** `Pages/Index.razor` (`@bind-SelectedIndex`) y `Pages/Index.razor.cs` (`selectedTabIndex`, `GetFirstVisibleTabIndex()` en el mismo orden de declaración de las pestañas). Archivos conservan codificación ISO-8859.

**Despliegue a producción:** ejecutar `01` y `03` en la BD; publicar la aplicación (incluye el servicio del correo, que corre dentro de Aldebaran.Web). `02` es solo para pruebas.
