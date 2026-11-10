# 03 – Plan de trabajo: Ajuste Reporte Referencias del Proveedor + Catálogo de la Competencia

Fecha: 2026-11-05 · Rama: `RQM-HomologadosCompetencia_AjusteReporteReferenciasProveedor` (contiene main y RQM Complemento Logs: `IS_BOMB_PRICE`, casillas tri-estado).
Propuesta aprobada: 1 iteración, 18 días de desarrollo, $5.800.000 contra entrega final.
Criterios: `Criterios de desarrollo - Aldebaran.Web.md` (siempre). Scripts BD en `scripts\RQM Homologados Competencia y Ajuste Reporte Referencias x Proveedor\Despliegue\` (+ `Reversa\`, + `LEEME`), re-ejecutables, con respaldo del objeto anterior.

## Forma de trabajo
Implementar → Andrés compila en VS → probar (con regresión acumulada) → corregir → OK → ChangeSet → siguiente. Nunca dos tareas sin compilar en medio.

## PARTE 1 – Ajuste Reporte de Referencias del Proveedor
| T | Tarea | Prueba de aceptación |
|---|---|---|
| T1 | SP `SP_GET_PROVIDER_REFERENCE_REPORT`: jerarquía Líneas/Artículos/Referencias (nivel más específico), 8 condiciones tri-estado, `TRY_CAST`, PK, `OPTION(RECOMPILE)`; script con respaldo + reversa + script de pruebas | Sin filtros/condiciones = resultado actual; conteos por filtro = consulta directa a BD |
| T2 | Application: filtro tipado + repositorio con `StoredProcedureCommand`; caso de uso `GetReportAsync` (árbol O(n)) y `GetExportAsync` (filas planas) | Árbol = SP (proveedores, líneas, artículos, referencias, bodegas) |
| T3 | Pantalla: usar el caso de uso (retirar armado del árbol y `.Result` del `.razor.cs`), sin cambios visuales | Reporte idéntico al actual con los mismos filtros (regresión) |
| T4 | Diálogo de filtro: Proveedor + selector jerárquico (clase hija existente) + 8 casillas tri-estado + "Mostrar inventario y bodegas" (Sí por defecto); validación; resumen de filtros en pantalla y documento | Conteos por condición = BD; las condiciones limitan el selector |
| T5 | Presentación con/sin inventario en pantalla, Imprimir y PDF | Con Sí = hoy; con No sin cantidades ni bodegas |
| T6 | Exportar → Excel con/sin inventario (columnas Bodega Local / Zona Franca) | Filas = referencias del filtro; columnas según opción |

## PARTE 2 – Catálogo de la Competencia
| T | Tarea | Prueba de aceptación |
|---|---|---|
| T7 | BD: tablas de compañías y homologaciones (FK geografía nullables + CHECK cascada; referencia opcional; únicos), rol "Catálogo de la competencia", opciones de menú; script + reversa | Restricciones probadas con inserciones válidas/inválidas |
| T8 | Entidades EF, configuraciones, repositorios, servicios por caso de uso (compañías) | CRUD por servicio contra BD |
| T9 | Pantalla Compañías de la competencia: listado, crear, editar, inactivar; cascada País → Departamento → Ciudad | Obligatorios/opcionales y cascada |
| T10 | Servicios de homologaciones | CRUD por servicio; duplicados rechazados |
| T11 | Pantalla Homologaciones: listado con filtros, crear/editar/inactivar a nivel Artículo o Artículo-Referencia | Ídem en pantalla |
| T12 | Imagen del producto de la competencia: carga, reemplazo, visualización (carpeta configurada, validación tipo/tamaño) | Archivo en carpeta; se ve en pantalla |
| T13 | SP del reporte de homologaciones + script de pruebas | Conteos por filtro = BD |
| T14 | Caso de uso del reporte (árbol + filas planas) | Árbol = SP |
| T15 | Pantalla del reporte: selector jerárquico limitado a lo homologado (clase hija), filtros de competencia, miniaturas | Solo ofrece lo homologado |
| T16 | Imprimir, PDF y Excel del reporte | Contenido = pantalla |
| T17 | Seguridad: rol aplicado a las dos opciones; reporte con roles de reportes | Visibilidad y URL directa por rol |

## Cierre
| T | Tarea |
|---|---|
| T18 | Casos de prueba y ejecución integral (P1 + P2) + regresión (Artículos y referencias, Inventario, otros usos del selector) |
| T19 | Manual funcional y técnico (Word, línea visual de las propuestas) |
| T20 | Paquete de despliegue (scripts, reversas, LEEME) y cierre |

## Deuda técnica incluida (ver `00 - Deuda tecnica incluida (T7 y T8).md`)
Se ejecutan después de las tareas funcionales y antes de T18 (para que la prueba integral las cubra).
| T | Tarea (origen) | Prueba de aceptación |
|---|---|---|
| T21 | Seguridad de endpoints (DT T7): `/export/AldebaranDb/*` con `[Authorize]`; endpoints `Notification/*Update` con clave compartida (revisar antes al servicio de notificaciones); sin stack trace en errores (400 ante mensajes mal formados); `MarkMinimumQuantityAlarmsAsRead` sigue anónimo | Sin sesión → 401/redirección; el notificador sigue actualizando; el enlace del correo funciona |
| T22 | SQL interpolado (DT T8): repositorios restantes → parámetros (`StoredProcedureCommand`). `ProviderReferenceReportRepository` ya se corrige en T2 | Mismos conteos que antes; búsquedas con comilla simple no fallan |

## Unificación con producción (antes del despliegue productivo)
Motivo (Andrés, 2026-11-05): en el último despliegue se partió de un SP desactualizado. Antes de armar el paquete se trae la versión de **producción** de cada objeto que este RQM modifica o del que depende, se actualiza la BD de desarrollo y se comparan.

| T | Tarea | Prueba de aceptación |
|---|---|---|
| T23 | Traer de producción (cuando haya conexión con PROMOS) la definición vigente de los objetos de la lista, actualizarlos en la BD de desarrollo, comparar con la versión en que se basó el desarrollo y reaplicar los cambios del RQM sobre la versión de producción si difieren | Diferencias documentadas; scripts del paquete construidos sobre la versión de producción; regresión de T1 y T22 OK |

### Objetos a sincronizar desde producción
**Se modifican en este RQM (SP):**
| Objeto | Tarea | Motivo |
|---|---|---|
| `dbo.SP_GET_PROVIDER_REFERENCE_REPORT` | T1 | Se reescribe (jerarquía, 8 condiciones, endurecimiento) |

**Sin cambio de SP, pero el código cambia la forma de invocarlos (T2 y T22: de texto concatenado a parámetros). Su firma en producción debe coincidir con la de desarrollo:**
| Objeto | Repositorio |
|---|---|
| `SP_GET_INVENTORY_REPORT` | InventoryReportRepository |
| `SP_GET_IN_PROCESS_INVENTORY_REPORT` | InProcessInventoryReportRepository |
| `SP_GET_FREEZONE_VS_AVAILABLE_REPORT` | FreezoneVsAvailableReportRepository |
| `SP_GET_INVENTORY_ADJUSTMENT_REPORT` | InventoryAdjustmentReportRepository |
| `SP_GET_WAREHOUSE_STOCK_REPORT` | WarehouseStockReportRepository |
| `SP_GET_WAREHOUSE_TRANSFER_REPORT` | WarehouseTransferReportRepository |
| `SP_GET_PURCHASE_ORDER_REPORT` | PurchaseOrderReportRepository |
| `SP_GET_CUSTOMER_RESERVATION_REPORT` | CustomerReservationReportRepository |

**Referencia (no se modifican, se reutiliza su comportamiento):** `SP_GET_ITEM_REFERENCES_REPORT` (modelo de condiciones y jerarquía).

**Objetos nuevos (no existen en producción, no requieren sincronización):** tablas de compañías y homologaciones de la competencia, SP del reporte de homologaciones (T7, T13), rol "Catálogo de la competencia" en `AspNetRoles` (dato).

**Tablas/columnas de las que dependen los objetos nuevos o modificados (verificar que existan en producción con la misma estructura):** `providers`, `provider_references`, `item_references`, `items` (incl. `IS_SALE_OFF`, `IS_BOMB_PRICE`, `IS_SPECIAL_IMPORT`), `lines`, `references_warehouse`, `warehouses`, `countries`, `departments`, `cities`, `AspNetRoles`.

Fecha de última modificación en la BD de **desarrollo** (2026-11-05), para comparar con producción: PROVIDER_REFERENCE 2024-07-13 · INVENTORY 2024-11-01 · IN_PROCESS_INVENTORY 2024-07-13 · FREEZONE_VS_AVAILABLE 2024-07-13 · INVENTORY_ADJUSTMENT 2024-07-13 · WAREHOUSE_STOCK 2024-07-13 · WAREHOUSE_TRANSFER 2024-07-13 · PURCHASE_ORDER 2026-08-29 · CUSTOMER_RESERVATION 2024-07-13 · ITEM_REFERENCES 2026-11-03.

> Esta lista se actualiza si alguna tarea agrega un objeto. Estado al 2026-11-05: T22 aún no analizado en detalle; si al ejecutarlo algún SP necesita cambio de firma, se agrega a la primera tabla.

## Barrida exploratoria de la aplicación
| T | Tarea | Prueba de aceptación |
|---|---|---|
| T24 | Recorrido con el navegador (usuario Administrador) por **todas** las opciones del menú: carga, consola sin errores, sin HTTP 500, abrir/cerrar diálogos sin guardar. Solo lectura. Entregable: lista de errores por pantalla → insumo de la tarea de pruebas de humo E2E (Playwright) del plan de deuda técnica | Lista entregada; errores dentro del alcance del RQM corregidos, el resto registrado en deuda técnica |

**T23 ✅ (2026-11-05):** Andrés trajo los SP de producción a desarrollo antes de T1; T1 y T22 se construyen sobre esa versión.

Orden de ejecución: T1–T17 → T21 → T22 → **T24** → T18 → T19 → T20.

## Decisiones vigentes
- Condiciones = tri-estado Sí/No/Todos (no switches como se escribió inicialmente). Sin cantidades mínimas.
- "Mostrar inventario y bodegas" es opción de presentación Sí/No (no tri-estado), Sí por defecto.
- Toda referencia tiene fila en `references_warehouse` (regla de negocio).
- Supuestos parte 2 (inactivar en vez de borrar; no duplicar homologación; precio en COP): confirmar al llegar a T7.

## Bitácora
| Fecha | T | Estado |
|---|---|---|
| 2026-11-05 | T7 | ✅ Script `02 - Catalogo de la competencia.sql` + reversa. 14/14 restricciones + 6/6 unicidad solo entre activos (índices únicos filtrados `IS_ACTIVE = 1`, decisión de Andrés). |
| 2026-11-05 | T1 | ✅ Script `01 - Reporte referencias del proveedor.sql` + reversa, construido sobre la versión de producción (traída por Andrés). 21/21: llamadas actuales idénticas a producción (filas + checksum), sin inventario 1 fila por proveedor-referencia, 8 condiciones = BD, jerarquía y lista inválida. Catálogo completo: 1,6 s con inventario / 0,6 s sin. Respaldo en `RQM_HOMOLOGADOS_OBJECT_BACKUP`. |
| 2026-11-05 | T2 | ✅ Caso de uso (filtro tipado, StoredProcedureCommand, árbol y filas planas) + 6 pruebas unitarias. Regresión en pantalla OK. ChangeSet aplicado. |
| 2026-11-05 | T3 | ✅ Pantalla usa GetReportAsync; retirados métodos obsoletos, modelo viejo y ViewModel. Pantalla idéntica (mismo texto y tablas), PDF OK. ChangeSet aplicado. |
| 2026-11-05 | T4 | Implementada, **pendiente compilar y probar (6-nov)**. Diálogo: proveedor, 8 condiciones tri-estado, "Mostrar inventario y bodegas", selector jerárquico, validación, resumen. Corregido error al reabrir con proveedor fuera de la página del combo. |
| 2026-11-05 | Decisión | Etiqueta de `IS_SALE_OFF` = **"En oferta"** (como en la página promocional). Se unifica también en Artículos y Referencias (filtro, pantalla, PDF y Excel) → incluir en la regresión de T4. |
| 2026-11-06 | T4 | ✅ Probada (validación, condiciones = BD, selector acotado, restauración, resumen; regresión "En oferta" en Artículos y Referencias). ChangeSet aplicado. |
| 2026-11-06 | T5 | Implementada y probada en pantalla (sin inventario: 3 columnas; con inventario idéntico a T3). Andrés detectó **página en blanco al imprimir** (preexistente: print.css evita cortar tablas `.edged`); corregido con estilo en línea en la tabla del proveedor. **Pendiente compilar y probar Imprimir/PDF.** |
| 2026-11-07 | T6 | Implementada sin compilar (Andrés fuera). Exportar → Excel: sin inventario 8 columnas; con inventario + Comprometido, Reservado, Existente, Bodega Local, Zona Franca (una fila por proveedor-referencia). 3 pruebas unitarias nuevas (7 en total). **Pendiente compilar y probar junto con T5.** |
| 2026-11-07 | T8 | Implementada sin compilar. Compañías: entidad (ITrackeable, sin navegación inversa en geografía), configuración EF, repositorio, servicio por caso de uso (crear, editar, inactivar, reactivar), reglas puras + 7 pruebas. Unicidad solo entre activas (validación previa + traducción del índice). **Pendiente compilar/probar.** Pendiente menor: nombres en español de los campos para Track (LOG_TRACK_FIELD_NAMES) → script 03 junto con T10. |
| 2026-11-07 | T10 | Implementada sin compilar. Homologaciones: entidad (ITrackeable), configuración, repositorio (listado con filtros, validaciones de compañía activa / artículo / referencia del artículo, duplicado activo, imagen), servicio (crear, editar, inactivar, reactivar), reglas puras + 10 pruebas. Script `03 - Track catalogo de la competencia.sql` (+ reversa) **ejecutado en desarrollo**: nombres en español de las 2 entidades y sus campos. **Pendiente compilar/probar.** |
| 2026-11-07 | T9 | Implementada sin compilar. Pantalla `/competitor-companies` (rol Administrador / Catálogo de la competencia): listado con búsqueda y "Mostrar inactivas", crear/editar en diálogo con País → Departamento → Ciudad opcional en cascada, inactivar (confirmación) / reactivar, mensajes de negocio. Menú Administración → Competencia → Compañías de la competencia. **Pendiente compilar/probar. Pausa aquí: siguiente sesión se prueba desde T5.** |
| 2026-11-07 | T5–T10 | ✅ Compilado y probado (pantalla sin/con inventario, Excel sin/con inventario = BD, compañías: obligatorios, cascada, duplicado activo, inactivar/reactivar, editar, Track). Corregidos combos cortados (display:block). Pendiente de Andrés: Imprimir/PDF sin página en blanco. ChangeSet único T5–T10. Datos de prueba: compañías "PRUEBA T9 COMPETIDOR" (ids 1 y 2) → borrar al cierre. |
| 2026-11-07 | Pruebas | +9 pruebas de servicios (CompetitorServicesTests, repositorios en memoria): 33 en total del RQM. Decisión: bUnit/Playwright como deuda técnica (T11/T12 del plan de deuda técnica); barrida T24 en este RQM. |
| 2026-11-07 | T11 | Implementada, pendiente compilar/probar. Pantalla `/competitor-homologations`: filtro por compañía (incluye inactivas), texto, "Mostrar inactivas"; columnas Compañía, Artículo/Referencia PROMOS, Nivel, Ref. y producto competidor, Precio, Enlace, Estado; diálogo con artículo (búsqueda paginada) y referencia opcional, compañías activas (y la actual al editar), enlace, precio COP; inactivar/reactivar. Menú Competencia → Homologaciones con la competencia. |
| 2026-11-07 | T11 | ✅ Probado (validaciones, nivel artículo/referencia, duplicado al editar, búsqueda, Track). Ajuste: título de Referencia sin placeholder superpuesto. |
| 2026-11-07 | T12 | Implementada, pendiente compilar/probar. Imagen en `ImageRepositoryPath\Competencia\` (decisión Andrés), URL `/externalimages/Competencia/`, nombre `H{id}_{fecha}{ext}`; JPG/PNG/WEBP ≤ 2 MB; se elige en el diálogo (crear o editar) y se guarda al pulsar Guardar; reemplazar elimina la anterior; quitar; sin huérfanos si falla el registro. Miniatura en el listado. +14 pruebas (reglas y servicio de imagen). |
| 2026-11-07 | T13 | ✅ Script `04 - Reporte homologaciones con la competencia.sql` (+ reversa) **ejecutado en desarrollo**. Jerarquía (con referencias incluye las de nivel artículo de su artículo), compañías, referencia/nombre del competidor (contiene), solo activas por defecto. Pruebas `scripts\...\Pruebas\04 - ...sql` 13/13 OK (con ROLLBACK). Imágenes de prueba para T12 en `documentos\...\Pruebas\`. **T12 y T13 pendientes de compilar/probar con Andrés.** |
| 2026-11-07 | T12 | ✅ Probado: GIF rechazado; PNG cargado (vista previa, miniatura, URL 200); reemplazo por JPG (nueva 200, anterior 404 = archivo borrado); quitar (sin miniatura, archivo 404). Queda una imagen PNG en la homologación 1 para probar el reporte. ChangeSet único T11–T13. |
| 2026-11-07 | T14 | Implementada, pendiente compilar. Entidad sin llave del SP + filtro, repositorio con StoredProcedureCommand y alcance homologado (EF); caso de uso GetReportAsync (árbol Compañía → Línea → Artículo → Homologaciones, nivel artículo primero), GetExportAsync (fila por homologación, sin imagen) y FilterHomologatedAsync (acota el selector). +4 pruebas (51 del RQM). |
| 2026-11-07 | T14 | ✅ Compila. Corregido borrado de imagen anterior al reemplazar (se captura el nombre previo) y 9 warnings CS8632 del Web. Regresión de imagen OK (nueva 200, anterior 404). |
| 2026-11-07 | T15 | Implementada, pendiente compilar/probar. Reportes → Catálogos → "Homologaciones con la competencia" (`/report/competitor-homologations`). Selector jerárquico acotado con FilterHomologatedAsync (sin tocar MultiReferencePicker), compañías (multiselección), referencia y producto del competidor (contiene); sin filtros lista todo. Árbol Compañía → Línea → Artículo con miniatura (clic amplía) y enlace. Menú: Administrador y Consulta de reportes (los reportes usan el permiso único de reportes; "Catálogo de la competencia" cubre Compañías y Homologaciones). |
| 2026-11-07 | T16 | Implementada, pendiente compilar/probar. Imprimir, Guardar PDF (horizontal) y Exportar → Excel (una fila por homologación, sin imagen). Para el PDF (generado en el servidor sin URL base) las imágenes se incrustan como data URI con `CompetitorReportImageEmbedder`; `ICompetitorImageStorage.ReadAsync` lee solo por nombre de archivo. +12 pruebas (incrustación y tipos de imagen; 63 del RQM). |
| 2026-11-07 | T15 | Ajuste de menú (Andrés): Administración → Competencia → "Compañías" y "Homologaciones"; el reporte conserva "Homologaciones con la competencia" en Reportes → Catálogos. |
| 2026-11-07 | T17 | Implementada, pendiente probar por rol. Opciones de Competencia: `[Authorize]` Administrador y Catálogo de la competencia (ya estaba en páginas y diálogos). Reportes del RQM (Homologaciones, Referencias del proveedor) y Artículos y referencias (Fase 2): `[Authorize]` Administrador y Consulta de reportes; antes la URL directa abría sin validar rol. **Hallazgo para T24:** los demás reportes (Inventario, Órdenes, Reservas, etc.) tampoco tienen `[Authorize]`; el menú los oculta pero la URL directa abre con cualquier sesión. |
| 2026-11-07 | T21 | Implementada, pendiente compilar/probar. `/export/AldebaranDb/*` ya tenía `[Authorize]` (RQM Logs), solo se verifica. `Notification/{PurchaseOrder,CustomerOrder,CustomerReservation}Update` ahora exigen firma en la ruta: `HookUrl` = `…/Notification/{endpoint}/{HMAC-SHA256 del MessageUid}` (`NotificationHookSigner`, Application); el procesador de notificaciones **no cambia** (hace POST a la URL que trae el mensaje). Mensaje incompleto → 400, firma inválida → 401, sin excepción. URL de retorno armada en un solo lugar (servicio de OC, resumen de pedido y de reserva). Clave nueva `AppSettings:NotificationHookKey` (≥ 32 caracteres; el sitio no inicia sin ella): valor de desarrollo en `appsettings.Development.json`; **pruebas y producción deben configurar su propio valor (LEEME T20)**. Mensajes en cola durante el despliegue con la URL vieja no actualizan su estado (decisión Andrés). `MarkMinimumQuantityAlarmsAsRead` sin cambios. +20 pruebas (83 del RQM). |
| 2026-11-07 | T22 | Implementada, pendiente compilar/probar. 8 reportes sin SQL concatenado: Reservas, Ajustes de inventario, Órdenes de compra, Existencias por bodega y Traslados pasan de texto `@X = '...'` armado en la página a filtro tipado (Web → Application → DataAccess, AutoMapper) y `ReportCommands` (StoredProcedureCommand + SqlParameter con tipos/tamaños del SP). Inventario, Inventario en proceso y Zona franca vs. disponible conservan su firma (lista de Ids; el FileWritingService no cambia de código, solo se recompila) y envían `@ReferenceIds` siempre (dos SP no tienen valor por defecto): nuevo `StoredProcedureCommand.VarCharOrEmpty`. Se elimina un `CreateMap` duplicado de ProviderReferenceReportFilter. Riesgo cerrado: textos libres (número de reserva, OC, importación, puerto, proforma, nacionalización) iban entre comillas en el SQL. Revisados los demás `FromSqlRaw($...)` del repositorio (Dashboard, solicitudes de cancelación, variación de OC): ya usan parámetros. +12 pruebas (95 del RQM). |
| 2026-11-07 | T24 (adelanto) | Deuda técnica incluida (Andrés): `[Authorize]` en los 16 reportes restantes con los mismos roles del menú (Administrador y Consulta de reportes; "Notificación de pedidos atendidos automáticamente" con su rol propio). Ningún otro sitio enlaza esas rutas. Manizales: ciudad 47 (Antioquia) duplicada sin uso en desarrollo, la correcta es la 48 (Caldas); solo COMPETITOR_COMPANIES guarda departamento. Script de verificación/eliminación condicionado para T20, se ejecuta solo con aprobación del cliente. |
| 2026-11-07 | Prueba conjunta | ✅ T12–T13, T15–T17, T21 (firma OK → estado 2; firma falsa 401; incompleto/JSON roto 400; BD sin cambios) y T22 (OC con apóstrofe y Shenzhen = 14 como BD, reservas, traslados con apóstrofe, zona franca). Roles probados con adiaz en ambos escenarios (roles restaurados). Enlace de alarmas mínimas del correo: ✅ "Proceso completado", sin error en log. Ajustes (Andrés): imagen de la competencia ampliable al clic en Homologaciones y en el reporte, con `CompetitorImageDialog` propio (no se modifica ImageDialog). Warning EF "IsActive con default de BD" en compañías y homologaciones corregido (se quita HasDefaultValue; el valor lo da la entidad). |
| 2026-11-08 | Prueba conjunta (cierre) | ✅ Imagen ampliable en Homologaciones y en el reporte (diálogo con título [ref] producto, sin abrir la edición). ✅ Warning IsActive ya no aparece al iniciar. ✅ T22 Ajustes (n.º 5983) y Existencias por bodega (Zona Franca). Inventario e Inventario en proceso comparten el mismo mecanismo probado en Zona franca. ChangeSet único T14–T22 + adelanto T24 entregado. Siguiente: T24 barrida. |
| 2026-11-08 | T24 | ✅ Recorrido de las 64 opciones del menú como Administrador (adiaz con rol Administrador temporal, BD de prueba): todas cargan en su ruta, sin la UI de error de Blazor, sin errores de consola y sin [ERR] en el log. Código: BOM retirado de 38 archivos tocados por el RQM (venían con BOM); MainLayout.razor a CRLF; `using Microsoft.IdentityModel.Tokens` sin uso retirado de 3 páginas de T22. Ruta vieja `Notification/*Update` sin firma responde 400 porque cae en `MapFallbackToPage("/_Host")` (POST sin antiforgery): rechazo correcto, se documenta. Warnings OData de ApplicationUsers (UsersByRole/LockUser/UnlockUser) son previos y no afectan funcionamiento: se documentan, no se tocan. Pendiente: compilar y correr pruebas. |
| 2026-11-08 | T18 (seguridad) | Enfoque Andrés: fallos e inyecciones (sin limpieza de datos). Revisión: los 10 SP del RQM/T22 sin SQL dinámico; inyección en @CompetitorReference devuelve vacío. Hallazgos corregidos: (1) sitio web de la compañía sin validar y publicado tal cual en href ("nueva.com" quedaba como ruta relativa; "javascript://..." ejecutable al clic) → `CompetitorWebLink` (solo http/https), validación al guardar y href seguro en Compañías, Homologaciones y reporte; (2) imagen validada solo por extensión → se verifica la firma del contenido (JPG/PNG/WEBP) antes de guardar. +23 pruebas. Pendiente: compilar y prueba en navegador con textos maliciosos. |
| 2026-11-08 | T18 (navegador) | ✅ Compañía con nombre `O'Neil <script>alert(1)</script> --`: se guarda y se muestra como texto (sin script en el DOM). ✅ Sitio `javascript://%0aalert(1)` rechazado; sitio con comillas guardado con href https correcto; "nueva.com" ahora enlaza a https://nueva.com. ✅ Búsqueda `O'Neil' OR '1'='1` sin resultados ni error. ✅ Enlace de producto `javascript://...` rechazado. ✅ HTML renombrado a .jpg rechazado ("El archivo no es una imagen..."), BD sin cambios. Hallazgo: en el reporte de homologaciones "contiene" usaba LIKE y `%`/`_` actuaban como comodines ("%" traía todo) → script 04 cambiado a CHARINDEX (literal, misma intercalación) y ejecutado en desarrollo; pruebas del SP ampliadas a 16/16 OK (P14 %, P15 _, P16 inyección). Previo, fuera de alcance: los SP de Reservas y Órdenes de compra usan LIKE en número de reserva/importación/puerto/proforma (comodines activos, sin riesgo de inyección por ser parámetros); se documenta. |
| 2026-11-08 | T20 | ✅ Paquete: `Despliegue\LEEME - Orden de despliegue.txt` (orden 01–05, clave NotificationHookKey por ambiente, subcarpeta Competencia con permisos, FileWritingService, NotificationProcessor sin cambios y cola vacía, rol, efecto del [Authorize] en reportes, verificación y reversa). Nuevo script `05 - OPCIONAL Ciudad Manizales duplicada.sql` (+ Reversa\05): elimina Manizales fuera de Caldas solo si no tiene uso, con respaldo; probado en desarrollo ida y vuelta (ciudad 47 eliminada y restaurada con su Id). |
