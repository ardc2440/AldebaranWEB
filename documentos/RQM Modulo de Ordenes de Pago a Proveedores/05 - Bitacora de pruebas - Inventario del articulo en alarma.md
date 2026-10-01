# Bitácora de pruebas – Inventario del artículo en la alarma de cantidades mínimas

**Rama:** `RQM-InventarioCompletoEnAlarmaDeCantidadMinima` · **Ambiente:** base de datos de PRUEBAS · **Plan:** `04 - Plan de trabajo - Inventario del articulo en alarma de cantidades minimas.md`
**Método:** cada tarea se prueba y repite la regresión de todas las anteriores. La regresión de T1 (script `02`) solo se repite si cambia la lógica de un SP o campos de tablas. Evidencias en `Evidencias Inventario Alarma\` con nombre `Tn-xx-descripcion.png`.

---

## T1 – SP `SP_GET_ITEM_REFERENCES_INVENTORY` + corrección `SP_GET_MINIMUM_QUANTITY_ALARMS` ✅ (2026-09-30)

Ejecutado por Andrés en SSMS (scripts `01`, `03` y `02`).

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| A1 | Artículo con varias referencias (MU-35, ref. 2257): filas, cifras vs control, sin inactivas, marca de alarma, D2, Stock Físico | Todo coincide | 21 = 21, 0 diferencias | OK |
| A2 | Artículo con inactivas (LÁPIZ METALLIC, ref. 1335) | Solo activas | 1 activa; 10 inactivas excluidas | OK |
| A3 | Artículo con tránsito (MU-303-1., ref. 2094) | Tránsito > 0 | 8.000 | OK |
| C01 | Referencia inexistente | Vacío, sin error | 0 filas | OK |
| C02 | Referencia NULL | Vacío, sin error | 0 filas | OK |
| C03 | Referencia inactiva | Activas, ninguna marcada | 1 = 1 | OK |
| C04 | Estructura | 10 columnas, ninguna anulable | Cumple | OK |
| I01 | Inactivas con mínima configurada | Informativo | 348 (la alarma no se genera para inactivas) | N/A |
| I02 | Local + ZF vs INVENTORY_QUANTITY | Informativo | 0 diferencias | OK |
| I03 | Índice por REFERENCE_ID | Informativo | Ya existía `IND_PURCHASE_ORDER_DETAIL_REFERENCE_ID` | OK |
| R1 | Bandeja "Sobrepaso de cantidades mínimas" con `03` | Carga, búsqueda y ocultar igual | Igual; hoy no hay alarmas con OC en aprobación | OK |

**Resultado:** 25/25 OK. Rendimiento A1: 0 ms CPU.

---

## T2 – Capa de datos (entidad, DbContext, repositorio, DI) ✅ (2026-09-30)

Ejecutado por Claude (control del PC: Visual Studio, aplicación y SSMS).

| Caso | Validación | Esperado | Obtenido | Resultado | Evidencia |
|---|---|---|---|---|---|
| T2-01 | Compilar la solución | Sin errores ni advertencias nuevas | Compilación correcta (reportado por Andrés) | OK | |
| T2-02 | Arrancar la aplicación (F5) | Inicia sin excepción (modelo EF con la nueva entidad keyless) | Login y Tablero cargan (Chrome, extensión Claude in Chrome) | OK | |
| T2-03 | Tablero: bandeja "Sobrepaso de cantidades mínimas" | Carga igual que antes | Carga: Página 1 de 42 (417 alarmas). Búsqueda "MU-372" + actualizar → 1 resultado (Inventario 0, Tránsito 3.000, Pedido 2.500, Disponible 500) | OK | |
| T2-04 | Tablero: resto de bandejas | Cargan igual que antes | Aprobación de ajustes en OC (1), Inventario sin stock (7), Bodega local (687), Reservas vencidas (sin registros) | OK | |
| R-T1 | Regresión T1: script `02` en SSMS | TODAS LAS PRUEBAS OK | 25/25 OK, mismas cifras, estructura sin anulables (ejecutado por Andrés) | OK | |

**Nota:** "Ocultar alarma" no se ejecutó en T2 (modifica datos y T2 no toca esa funcionalidad); se valida en T5.
**Hallazgo:** la columna "Disponible" de la bandeja = Inventario + Tránsito − Reservado − Pedido (fila MU-372: 0 + 3.000 − 0 − 2.500 = 500). Es la misma fórmula D2 del diálogo.

---

## T3 – Caso de uso (Application.Services) ✅ (2026-09-30)

| Caso | Validación | Esperado | Obtenido | Resultado | Evidencia |
|---|---|---|---|---|---|
| T3-01 | Compilar la solución | Sin errores ni advertencias nuevas | Compilación correcta (Andrés) | OK | |
| T3-02 | Arrancar la aplicación (F5, Development) | Inicia sin excepción: `ValidateOnBuild` resuelve `IArticleInventoryService` → `IArticleInventoryRepository` | App arranca; login y Tablero OK | OK | |
| T3-03 | Tablero y bandejas | Cargan igual que antes | Cantidades mínimas 417 (mismas cifras que T2), bodega local 687, y demás bandejas con datos (333, 36, 143, 2.139, 1.316, 191, 26) | OK | |
| T3-04 | Servicio = T1 para A1, A2, A3 (incl. referencia ≤ 0 → vacío sin ir a BD) | Mismas filas y cifras que la grilla del script `02` | Se verifica en T4 (primer consumidor del servicio) | Diferido | |
| R-T1 | Regresión T1: script `02` | No aplica | Sin cambios en SP ni tablas desde la última corrida (T2, 25/25) | N/A | |

---

## T4 + T5 – Diálogo `ArticleInventoryDialog` y conexión en la bandeja ✅ (2026-09-30)

Se prueban juntas (decisión de Andrés): el diálogo solo se abre desde la bandeja. Incluye la corrección de codificación de `Shared/ImageDialog.razor` (ISO-8859 → UTF-8, solicitada por Andrés).

| Caso | Validación | Esperado | Obtenido | Resultado | Evidencia |
|---|---|---|---|---|---|
| T45-01 | Compilar la solución | Sin errores ni advertencias nuevas | Compilación correcta (Andrés) | OK | |
| T45-02 | Clic en imagen de una alarma con imagen y varias referencias | Diálogo: nombre, imagen, grilla con todas las referencias activas, fila de la alarma resaltada, N0, Disponible rojo/verde, nota de fórmulas | ATTILA (179): imagen, 1 referencia, nota OK. MU-372 Transparente: 7 referencias, fila de la alarma en negrita. Ajuste visual aplicado: encabezado "Comprometido" cortado → diálogo 1000 px; resaltado no visible → estilo por celda (#fff3cd). Revalidado tras compilar: encabezados completos y fila de la alarma resaltada (negrita + fondo amarillo) | OK | |
| T45-03 | Alarma sin imagen | Mensaje "La imagen para este artículo no está disponible." con tildes correctas; grilla igual | MU-372: mensaje con tildes correctas, grilla de 7 referencias | OK | |
| T45-04 | Cifras del diálogo = SP (T3-04 diferido) | Cada columna igual a `EXEC SP_GET_ITEM_REFERENCES_INVENTORY` de esa referencia | MU-372 (ref. 6152): las 7 referencias, todas las columnas y la marca de alarma iguales al SP. Cierra también T3-04 | OK | |
| T45-05 | Cifras de la fila de la alarma = bandeja | Stock Físico = Inventario, Tránsito, Comprometido = Reservado + Pedido, Disponible iguales | MU-372 Transparente: diálogo 0 · 0 · 0 · 3.000 · 2.500 · 500 = bandeja (Inventario 0, Tránsito 3.000, Reservado 0 + Pedido 2.500, Disponible 500). ATTILA: todo 0 = bandeja | OK | |
| T45-06 | Enlace del nombre en la bandeja | Sigue abriendo el Reporte de movimientos | MU-372 abre el Reporte de movimientos con la referencia filtrada | OK | |
| T45-07 | Búsqueda y paginación de la bandeja | Igual que antes | Búsqueda (ATTILA, MU-372, HO-13) y paginación OK | OK | |
| T45-08 | Ocultar alarma (selección + botón) | Igual que antes | ATTILA (179): confirmación "Ocultar alarmas" → Sí → ya no aparece para `admin` (dato de prueba en `visualized_minimum_quantity_alarms`) | OK | |
| T45-09 | `ImageDialog` en otras bandejas (Inventario sin stock, Bodega local) | Abre igual, ahora con tildes correctas | Inventario sin stock (MU-155): abre igual, mensaje con tildes correctas | OK | |

**Set usado:** ARIA (144/146) y HO-13 (932) ya estaban ocultas para `admin`; se probó con ATTILA (con imagen, 1 referencia) y MU-372 (sin imagen, 7 referencias, con tránsito).

---

## T6 – Regresión final y cierre ✅ (2026-09-30)

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| T6-01 | Usos de `ImageDialog` | Inventario: 7 usos (6 sin cambios + bandeja de mínimos que ahora abre `ArticleInventoryDialog`) | ConfirmedPurchaseOrderNotifications, LocalWarehouseNotifications, MinimumLocalWarehouseQuantityNotifications, OutOfStockNotifications, ConfirmPurchaseOrder, ItemReferencesReportItemTable, MultiReferencePicker | OK |
| T6-02 | `ImageDialog` con imagen (Bodega local, ADVA 2-1) | Abre igual: título + imagen | Igual | OK |
| T6-03 | `ImageDialog` sin imagen (Inventario sin stock, MU-155) | Mensaje con tildes correctas | Correcto | OK |
| T6-04 | Rendimiento del diálogo | Apertura inmediata | MU-372 (7 referencias): diálogo completo en < 1 s. SP con 21 referencias (MU-35): 0 ms de CPU (T1) | OK |
| T6-05 | Regresión T1 | No aplica | Sin cambios en SP ni tablas desde T2 | N/A |

Los usos restantes de `ImageDialog` (OC confirmadas, Confirmar OC, Reporte de Artículos y Referencias, `MultiReferencePicker`) comparten el mismo componente, cuyo único cambio fue la codificación del archivo (contenido idéntico); no requieren prueba individual.

**Resultado del requerimiento:** T1–T6 cerradas. 

---

## T7 + T8 + T9 – Excel agrupado del correo periódico de cantidades mínimas ✅ (2026-10-01)

Se prueban juntas: el método nuevo del generador solo lo invoca el servicio del correo (`NotificationWorker` → `InventoryMinimumAlertService`), que se ejecuta por horario y envía mediante el Notificator. Destinatarios del rol en pruebas: empleados 22 (adiaz) y 27 (gramirez), ambos a ardc2440@gmail.com.

**Procedimiento:** compilar → ajustar `ExecutionHours` a una hora cercana → F5 → esperar la ejecución del worker (log "InventoryMinimumAlertService finalizado") → ejecutar el Notificator → abrir el adjunto del correo.

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| T7-01 | Compilar la solución | Sin errores ni advertencias nuevas | Compilación correcta (Andrés) | OK |
| T7-02 | Adjunto abre en Excel sin reparaciones | Abre sin mensaje "contenido ilegible" | Confirmado por Andrés | OK |
| T7-03 | Agrupamiento | Grupos contraídos; "+" en la fila de la alarma expande/contrae el detalle | Confirmado por Andrés | OK |
| T7-04 | Imagen | Solo en la fila padre (si el artículo tiene imagen); detalle sin imagen y con alto normal | 4 imágenes, todas ancladas en filas padre (alto 100); 1.050 filas de detalle sin imagen | OK |
| T7-05 | Hoja sin protección | Se puede expandir/contraer y seleccionar | Sin `sheetProtection`; `outlinePr summaryBelow=0`, `outlineLevelRow=1`; 1.050 hijos nivel 1 ocultos, padres `collapsed` | OK (estructura) |
| T8-01 | Detalle = diálogo | MU-372: 7 referencias con las mismas cifras del diálogo (Local, ZF, Físico, Tránsito, Comprometido, Disponible) | Idénticas al diálogo y al SP (ref. 6152); fórmulas correctas en las 1.353 filas | OK |
| T8-02 | Marcas de alarma | En el detalle, las referencias en alarma tienen "En alarma: Sí" y su Cantidad mínima | Cada padre tiene su referencia marcada en el detalle con las mismas cifras (303/303) | OK |
| T8-03 | Un grupo por artículo | Un artículo con dos alarmas aparece una sola vez, con ambas marcadas en el detalle | 71 grupos con más de una alarma; ej. ARIA: Amarillo (4.500) y Naranja (2.000) marcadas | OK |
| T9-01 | Correo | Llega con el adjunto `InventarioMinimo_yyyyMMdd HHmm.xlsx` | Recibido `InventarioMinimo_20261001_0823.xlsx`: 303 grupos, 1.353 filas | OK |
| T9-02 | "Marcar alarmas como leídas" | Desmarca **todas** las alarmas del lote (incluidas las que van en el detalle) | Confirmado por Andrés | OK |
| T9-03 | Regresión | Tablero, diálogo (T4–T5) y Excel del Reporte de Artículos y Referencias (`GetExcelBytes`) igual | Tablero y bandejas OK (T11); `GetExcelBytes` sin cambios de código | OK |

---

## T11 – Corrección: pestaña inicial del Tablero ✅ (2026-10-01)

Hallazgo de Andrés (2026-10-01): con un usuario sin el rol "Aprobación de ajustes en órdenes de compra" (gramirez) el botón de esa pestaña no se muestra (correcto), pero su contenido sí aparece al entrar. Causa: `RadzenTabs` sin `SelectedIndex` → Radzen selecciona la pestaña 0 aunque esté oculta. Corrección: `@bind-SelectedIndex` con la primera pestaña visible según permisos; sin permisos, ninguna (-1). `Index.razor(.cs)` conservan su codificación ISO-8859 (roles con tildes intactos).

| Caso | Validación | Esperado | Obtenido | Resultado |
|---|---|---|---|---|
| T11-01 | Compilar | Sin errores | Compilación correcta (Andrés) | OK |
| T11-02 | gustavor (sin el rol de aprobación) | Entra a su primera pestaña visible ("Sobrepaso de cantidades mínimas por referencia") con su contenido; no aparece la grilla de aprobación | Abre en Sobrepaso de cantidades mínimas (grilla de esa bandeja, vacía porque sus alarmas se marcaron como leídas en T9-02); sin grilla de aprobación | OK |
| T11-03 | admin (Administrador) | Entra a "Aprobación de ajustes en órdenes de compra"; todas las pestañas | Abre en Aprobación con su contenido (OC 0000000730); todas las pestañas visibles | OK |
| T11-05 | adiaz (solo el rol de aprobación) | Solo la pestaña "Aprobación de ajustes en órdenes de compra", seleccionada al entrar | Única pestaña visible, seleccionada, con su contenido (OC 0000000730) | OK |
| T11-04 | Cambio de pestaña y alertas | Navegar entre pestañas y clic en alerta funcionan igual | Aprobación → Inventario sin stock → Cantidades mínimas → Aprobación OK; clic en alerta de Inventario sin stock la apaga y abre la pestaña | OK |

---

**Resultado final del requerimiento (2026-10-01):** T1–T11 cerradas.
