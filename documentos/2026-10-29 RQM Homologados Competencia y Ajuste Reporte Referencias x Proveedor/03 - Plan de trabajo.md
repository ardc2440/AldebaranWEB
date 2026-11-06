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

Orden de ejecución: T1–T17 → T21 → T22 → T18 → **T23** → T19 → T20.

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
