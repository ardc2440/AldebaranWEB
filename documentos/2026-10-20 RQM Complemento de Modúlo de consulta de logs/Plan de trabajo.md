# RQM Complemento módulo de Consulta de Logs + Marca de Oferta – Plan de trabajo

Fecha: 2026-10-23 (copia en repo del documento del Proyecto Claude) · Proyecto: Aldebaran.Web (PROMOS) · Rama: `RQM-ComplementoLogs+SaleOff`
Propuesta aprobada: `documentos\2026-10-20 RQM Complemento de Modúlo de consulta de logs\PROPUESTA - Complemento modulo de Consulta de Logs.docx`
Scripts: `scripts\RQM Complemento Consulta de Logs\` → `Despliegue\` (producción) y `Pruebas\` (solo ambiente de pruebas)
Estado actual: ver `TRASPASO - Sesion Claude.md` (E1 y E2 cerradas; E3: T19–T22b OK; sigue T23). BD de pruebas `GML0999\SQLEXPRESS01` (no productiva). Usuario de prueba: admin. Cliente para pruebas con correo: cédula 79872389 (de Andrés).

## Regla de alcance (22-oct, 22:24)
**Limitarse a lo que pide el requerimiento. No cambiar ninguna lógica actual de alarmas.** Las posibles deudas técnicas encontradas se analizan al terminar el RQM.

## Scripts
| Carpeta | Script | Tarea |
|---|---|---|
| Despliegue | 01 - Indices Log y Track | T1 |
| Despliegue | 02 - Ofuscar historico Track | T2 |
| Despliegue | 03 - Consulta visualizacion de alarmas | T3 |
| Despliegue | 04 - Alarmas inventario estandar y log | T3b / T3c / T19 |
| Despliegue | 05 - Job verificacion alarmas log | T3c |
| Despliegue | 06 - Consulta log alarmas inventario | T4 |
| Despliegue | 11 – Logs de OC (ajustes y rangos de aprobación) | T14a/T14b |
| Despliegue | 12 – Log general | T15 |
| Despliegue | 13 – Log del autómata de existencias | E2 |
| Despliegue | 14 – items.IS_BOMB_PRICE + alarmas de mínimos | T8/T9 |
| Despliegue | 15 – Corrección estado en PURCHASE_ORDER_ADJUSTMENT_LOG | T18 deuda 4 |
| Despliegue | 16 – Precio bomba en reportes | T20–T22b |
| Despliegue\Reversa | 04, 05, 14, 15, 16 | — |
| Despliegue | LEEME - Orden de despliegue.txt | — |

## Decisiones de Andrés (22-oct)
- Índices: crear todos los necesarios. Ninguna consulta de log puede afectar la aplicación.
- Trazabilidad: toda entidad que se modifique desde la app debe implementar `ITrackeable`.
- Credenciales: se trazan con contraseñas **ofuscadas**. Histórico se ofusca con script masivo sin llaves fijas (regla para todo script de datos).
- Roles nuevos: `Log del sistema` (Log general y Track) y `Log de inventarios`. Envíos y Traslados → `Log de pedidos`. Alarmas → cada una en su contexto.
- Fechas: rango obligatorio, sin máximo. En Track la entidad también es filtro principal.
- Entregas: E1 = Alarmas + Índices + Track + Marca; E2 = Logs Envíos/Traslados + Logs OC + Log general + Manuales; E3 = Precio bomba en reportes + Pedido especial.
- Tarea final: **Manual Técnico y Manual de Usuario** (Word).
- Alarmas: **diseño genérico**. Solo consulta de visualizaciones. Fecha de filtro = la que hace visible la alarma en el tablero. Solo alarmas **activas** y visualizaciones vigentes.
- **Ciclo de vida de una alarma de inventario:** nace cuando se cumple la condición y no hay activa; vive mientras está activa; muere al inactivarse. Si mientras está activa se vuelve a dar la condición, se conserva con el estado más reciente y se borran sus visualizaciones.
- H1: bodega local con el mismo estándar. H2: el log guarda estado original y nuevo. H3: `@JOB = 1` no se modifica (lógica). H4: la app actualiza uno a uno. H5: mientras el disponible esté bajo el mínimo, cada evaluación refresca y vuelve a mostrar la alarma. H6: una alarma activa solo se inactiva cuando la OC se confirma (mínimos y sin stock).
- T3c: el refresco del job se registra **solo si cambió la cantidad**; las inactivaciones directas del job **se registran**.

## Hallazgos técnicos clave
- Módulo modelo: `Pages/LogPages/CustomerOrderModificationsLog`; menú `Shared/MainLayout.razor`.
- Track: `AldebaranDbContext.TrackEntities()`; escrituras vía SP/triggers no se trazan. Tablas con trigger en EF7: `ToTable(tb => tb.HasTrigger(...))`.
- `alarms.DOCUMENT_ID` = PURCHASE_ORDER_ID (tipo 1), CUSTOMER_ORDER_ID (2), CUSTOMER_RESERVATION_ID (3).
- Alarmas de inventario: mínimos `SP_Set_Minimum_Quantity_Alarm` y sin stock `SP_Set_Out_Of_Stock_Alarm`; bodega local `Trg_Minimum_Local_warehouse_Quantity_Alarm`. OC en tránsito: `STATUS_ORDER IN (1,4)` (igual que producción).
- Inventario: se mueve desde la app con triggers EF (`Aldebaran.DataAccess/Core/Triggers`).
- Excel de mínimos (`InventoryMinimumAlertService`, `NotificationWorker`) usa `SP_GET_MINIMUM_QUANTITY_ALARMS`; el correo sale por cola RabbitMQ → NotificationProcessor → SMTP.
- Job "Verificacion Alarmas": diario 6:00 a. m., llama los SP con `@JOB = 1` y luego inactiva con UPDATE directo.

## Datos de prueba creados
Conexión autómata `SERVIDOR-PRUEBA-T2` y FTP `ftp.prueba-t2.invalid` (inactivas), patrón `PATRON PRUEBA T2 NO USAR` (inactivo), reserva 92 y pedido 39469 (cliente 79872389), alarmas 825 y 826, tolerancia 351–400 en 0,06, pedido 39463 cancelado, 39175 cerrado, OC 752 confirmada, ajuste OC 742 devuelto. Config SMTP de pruebas con remitente "Catalogos Promocionales T2". Precio bomba en HO-13 (2554) y ASTON-STY (2047).

## ENTREGA 1 — Alarmas · Índices · Track · Marca  (cerrada)
| T | Tarea | Estado |
|---|---|---|
| T1 | Índices log | ✅ |
| T2a/T2b | Ofuscación en Track + 19 entidades `ITrackeable` | ✅ |
| T3/T3b/T3c | Visualización de alarmas, estandarización + log, job | ✅ |
| T4/T5 | Servicio y pantallas de alarmas por contexto + inactivaciones/refrescos | ✅ |
| T6/T7 | Track por entidad | ✅ |
| T8/T9 | Precio bomba en Artículos (sufijo " - $ BOMBA", excluyente con Oferta, sí genera alarmas) + marca en diálogo de inventario y Excel de mínimos | ✅ |

## ENTREGA 2 — Logs de Envíos, Traslados y OC · Log general  (cerrada salvo manuales)
| T | Tarea | Estado |
|---|---|---|
| T10–T14c | Logs de envíos, traslados, ajustes de OC y tolerancias + menús | ✅ |
| T15/T16 | Log general (+ log del autómata con NLog) | ✅ |
| T17 | Manual Técnico y Manual de Usuario (Word), incluyen E3 | Pendiente |
| T18 | Deudas técnicas: 1 → T19 corregida; 2 y 3 descartadas; 4 corregida (script 15) | ✅ |

## Decisiones Precio Bomba (23-oct)
1. Sufijo del nombre: `- $ BOMBA` (máx. 50 caracteres). 2. Oferta y Precio Bomba son **excluyentes**. 3. Preorden: como artículo normal. 4. Reportes: aprobados por el cliente en E3.

## ENTREGA 3 – Precio Bomba en reportes + Pedido especial (aprobada 23-oct 18:18)
| T | Tarea | Prueba de aceptación | Estado |
|---|---|---|---|
| T19 | No refrescar alarma activa si cantidad y mínimo no cambian | Edición de OC sin cambio no borra visualizaciones | ✅ |
| T20 | Artículos y referencias: Precio bomba en resultado + 10 condiciones tri-estado | Conteos = BD | ✅ |
| T21 | Stock mínimo de bodega: filtro tipado + Precio bomba tri-estado | Ídem | ✅ |
| T22/T22b | Movimiento de referencia: Precio bomba + Artículo activo / Referencia activa tri-estado | Ídem | ✅ |
| T23 | BD Pedido especial: `customer_orders.IS_SPECIAL_ORDER` (default 0) + script/reversa + nombre en Track | Antiguos = No | |
| T24 | Captura: obligatorio al crear (Sí/No sin preselección); editable (queda en Track/logs) | No guarda sin elegir | |
| T25 | Lista y detalle de pedidos: columna + filtro | | |
| T26 | Reportes con filtro de pedido (Pedidos+export, Actividades, Ventas, Back order+export, Asignación automática, Pendientes traslado automático): filtro tri-estado + columna; corregir SQL concatenado | Conteos = BD | |
| T27 | Logs de pedidos (mod./canc./cierre) y de despachos/traslados: columna | | |
| T28 | Tablero (componentes de pedidos) y pantallas de despachos/traslados: mostrar dato | | |
| T29 | Casos de prueba y ejecución completa del RQM (E1+E2+E3) | Todo OK | |
| T30 | Paquete de despliegue final (scripts, reversas, LEEME, NLog autómata + GRANT) y cierre; restaurar datos de prueba | | |

**Regresión acumulada (siempre):** logs existentes de OC, pedidos y reservas; tablero de notificaciones/alarmas y marcar visualizado; reporte de cantidades mínimas y correo con Excel; crear/modificar/cancelar/confirmar pedido, OC y reserva; pruebas de todas las tareas anteriores.

## Notas de despliegue
- Script 13 + desplegar el `NLog.config` del Autómata de Existencias (target `asyncDatabase` → `log.SP_INSERT_AUTOMATA_EXISTENCIAS_LOG`), reiniciar el servicio y dar EXECUTE al usuario de BD del autómata.
- Script 16 se despliega junto con la app (cambian parámetros de SP).
- Script 05 requiere permisos en msdb (validar en producción).
