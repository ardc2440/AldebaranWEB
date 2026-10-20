# ESTIMACIÓN DE ESFUERZO POR HITOS – Módulo de Órdenes de Pago a Proveedores

**Uso interno. No se entrega al cliente.** Sirve de insumo para calcular la duración y el costo de la propuesta.
**Fecha**: 2026-09-26 · **Base**: `03 - Definicion funcional consolidada.md` y `PROPUESTA - Modulo de Ordenes de Pago a Proveedores.docx`

---

## 1. Criterios de estimación

| Factor | Criterio |
|---|---|
| Perfil | Analista + Arquitecto + Desarrollador Senior .NET / Blazor / Radzen / SQL Server, que conoce el codebase Aldebaran |
| Unidad | Horas de trabajo efectivo |
| Incluye | Diseño técnico, scripts BD, entidades/configuración EF, repositorios, servicios de caso de uso, UI, pruebas por tarea con regresión acumulada |
| Método de trabajo | Tareas mínimas comprobables: implementar → probar (con regresión acumulada) → corregir → OK → siguiente |
| Jornada | 8 h = 1 día hábil |
| Reutilización | Tablero de notificaciones, `notification_definitions`, `SYSTEM_PARAMETERS`, mecanismo PDF (`FileBytesGeneratorService`), MultiReferencePicker / componentes compartidos sin modificarlos |

---

## 2. Alcance base

### H1 – Modelo de datos y seguridad — 29 h
| # | Tarea | Horas |
|---|---|---|
| 1.1 | Script: moneda en proveedor; fecha estimada de pago, valor adicional, total proforma y moneda en OC; valor unitario en detalle (nullables) | 3 |
| 1.2 | Script: tablas de Órdenes de Pago, log de acciones, solicitudes de habilitación, tipos de pago, tipos de soporte, motivos por acción (aprobación, devolución, anulación, reintegro); secuencia de número de OP; índices y restricciones | 7 |
| 1.3 | Script: tipo de documento y estados de la OP, 5 roles, parámetros del semáforo en `SYSTEM_PARAMETERS`, datos iniciales (tipos de pago y motivos) | 3 |
| 1.4 | Entidades y configuraciones EF, modelos de Application.Services por caso de uso, mappings | 10 |
| 1.5 | Repositorios base (OP, habilitaciones, catálogos) | 6 |

### H2 – Moneda del proveedor — 4 h
| 2.1 | Moneda en crear/editar proveedor (UI + servicio) y consulta | 4 |
|---|---|---|

### H3 – Datos de pago en la Orden de Compra — 28 h
| # | Tarea | Horas |
|---|---|---|
| 3.1 | Encabezado: campos de pago en Crear / Editar OC; moneda copiada del proveedor | 6 |
| 3.2 | Detalle: valor unitario en agregar / editar referencia + valor calculado informativo | 6 |
| 3.3 | Regla Σ(valor unitario × cant. solicitada) = proforma − adicional (servicio + mensajes de diferencia) | 4 |
| 3.4 | Confirmar OC: exigir datos de pago cuando no existan | 5 |
| 3.5 | Editar OC Pendiente: total no inferior a lo comprometido | 3 |
| 3.6 | Cancelación de OC bloqueada con OP vigentes (al solicitar y al aprobar la cancelación) | 4 |

### H4 – Habilitación de datos de pago en OC Confirmadas — 25 h
| # | Tarea | Horas |
|---|---|---|
| 4.1 | Registro automático de la solicitud al intentar pagar una OC sin datos | 4 |
| 4.2 | Aprobar / Rechazar la habilitación (Aprobador) con motivo y comentario | 8 |
| 4.3 | Página de edición restringida de datos de pago (sin flujo de modificación de OC, sin notificaciones a clientes ni afectación de inventario) | 10 |
| 4.4 | Log y cierre de la habilitación | 3 |

### H5 – Órdenes de Pago (núcleo) — 60 h
| # | Tarea | Horas |
|---|---|---|
| 5.1 | Servicio de saldo (comprometido, ejecutado, saldo) y control de pagos dobles con bloqueo transaccional | 10 |
| 5.2 | Crear / editar OP (selección de OC, resumen de saldo, validaciones) | 12 |
| 5.3 | Enviar / reenviar a aprobación (comentario opcional); acciones del Aprobador: enviar a ejecución, devolver (estado Devuelta), anular; anulación por Creador y Ejecutor (diálogo motivo + comentario, validación de texto sin sentido) | 12 |
| 5.4 | Finalizar OP (tipo de pago, tipo de soporte Físico/Digital, referencia, comentario) | 5 |
| 5.4b | Registrar reintegro total de OP Ejecutada (motivo, comentario, fecha y soporte) + notificación al Aprobador | 5 |
| 5.5 | Log de acciones (cada transición, valor anterior/nuevo) y consulta de Historial | 6 |
| 5.6 | Página de Órdenes de Pago (opción base: crear, consultar todas, filtrar por estado, acciones por estado y rol, resaltado de devueltas/anuladas/reintegradas) + menú | 10 |

### H6 – Documento imprimible de la OP — 12 h
| 6.1 | Formato basado en Pedido, datos de aprobación/ejecución, marca ANULADA, impresión y PDF | 12 |
|---|---|---|

### H7 – Pestaña "Pagos" en la consulta de OC — 8 h
| 7.1 | Resumen de saldo, listado de OP, resaltado de anuladas, indicador en la OC, imprimir OP | 8 |
|---|---|---|

### H8 – Notificaciones de Windows — 13 h
| 8.1 | 7 definiciones (OC por pagar, OP devueltas, OP en aprobación, habilitaciones, OC habilitadas para registrar datos, OP en ejecución, reintegros), consultas, roles y pruebas | 13 |
|---|---|---|

### H9 – Tablero de notificaciones (6 bandejas con acciones) — 30 h
| 9.1 | Bandeja OC por pagar con semáforo (lectura de umbrales) + acceso a crear OP | 8 |
|---|---|---|
| 9.2 | Bandeja OP devueltas (Creador) con acciones | 4 |
| 9.3 | Bandeja OP en aprobación (Aprobador) con acciones | 5 |
| 9.4 | Bandeja habilitaciones de datos de pago (Aprobador) | 4 |
| 9.5 | Bandeja OC habilitadas para registrar datos (Registro de datos) | 4 |
| 9.6 | Bandeja OP en ejecución (Ejecutor) con acciones | 5 |

### H10 – Reporte de Órdenes de Pago — 34 h
| # | Tarea | Horas |
|---|---|---|
| 10.1 | SP parametrizado (base OC con LEFT JOIN a OP, situación de saldo, vencidas, totales por moneda) + pruebas SQL | 10 |
| 10.2 | Componente de filtros (12 filtros) | 10 |
| 10.3 | Página del reporte (Proveedor → OC → OP, semáforo, resaltado de anuladas, totales), impresión y PDF | 14 |

### H11 – Pruebas integrales, estabilización y documentación — 28 h
| 11.1 | Prueba conjunta con checklist (flujo de 3 roles, devoluciones, anulaciones y reintegro), ajustes, notas para manual funcional y notas técnicas | 28 |
|---|---|---|

### H12 – Instalación y acompañamiento — 8 h
| 12.1 | Scripts de producción, publicación, verificación y acompañamiento | 8 |
|---|---|---|

### H13 – Inventario del artículo en la alarma de cantidades mínimas — 8 h
| # | Tarea | Horas |
|---|---|---|
| 13.1 | Caso de uso "Inventario del artículo por referencia" (referencias activas; Disponible, Bodega Local, Zona Franca, Tránsito con el cálculo del reporte de inventario) | 3 |
| 13.2 | Componente `ArticleInventoryDialog` que reutiliza `ImageDialog` sin modificarlo + grilla con resaltado de la referencia de la alarma | 3 |
| 13.3 | Cambio de la llamada solo en la bandeja de cantidades mínimas + pruebas y regresión | 2 |
| | Fuera de alcance: inclusión en el Excel de la notificación por correo | |

### H14 – Reporte de Historial de Precios por Referencia y Proveedor — 20 h
| # | Tarea | Horas |
|---|---|---|
| 14.1 | SP parametrizado: compras por referencia y proveedor (OC no canceladas con valor unitario), variación vs compra anterior en la misma moneda, resumen (primero, último, mín, máx, promedio ponderado, variación total), filtro de variación mínima | 6 |
| 14.2 | Componente de filtros (proveedor, referencias con MultiReferencePicker sin modificarlo, rango de fechas, moneda, variación mínima) | 5 |
| 14.3 | Página del reporte (Artículo → Referencia → Proveedor → compras, indicador sube/baja), impresión y PDF | 7 |
| 14.4 | Pruebas y regresión | 2 |

### Total alcance base
| Hito | Horas |
|---|---|
| H1 Modelo de datos y seguridad | 29 |
| H2 Moneda del proveedor | 4 |
| H3 Datos de pago en la OC | 28 |
| H4 Habilitación en OC Confirmadas | 25 |
| H5 Órdenes de Pago (núcleo) | 60 |
| H6 Documento de la OP | 12 |
| H7 Pestaña Pagos | 8 |
| H8 Notificaciones Windows | 13 |
| H9 Tablero | 30 |
| H10 Reporte | 34 |
| H11 Pruebas, estabilización y documentación | 28 |
| H12 Instalación y acompañamiento | 8 |
| H13 Inventario del artículo en alarma de cantidades mínimas | 8 |
| H14 Reporte de Historial de Precios | 20 |
| **TOTAL BASE** | **307 h ≈ 38.4 días de esfuerzo** |

---

## 3. Opciones adicionales (se cotizan por separado)

| Opción | Alcance | Horas |
|---|---|---|
| O1 – Comprobante adjunto | Almacenamiento de archivos (ruta configurable), carga al finalizar (soporte Digital) y al registrar reintegro, descarga desde página de OP y pestaña Pagos, validación de tipo/tamaño | 20 |
| O2.1 – Página umbrales del semáforo | Edición de parámetros con validación (Verde > Amarillo > 0) | 5 |
| O2.2 – Página tipos de pago y tipos de soporte | CRUD con activación/inactivación | 9 |
| O2.3 – Página motivos | CRUD por acción (aprobación, devolución, anulación, reintegro) con activación/inactivación | 10 |
| **Total opciones** | | **44 h** |

---

## 4. Insumo para la tabla "Tiempo de desarrollo" de la propuesta (alcance base)

| Fase (tabla de la propuesta) | Hitos | Horas | Días hábiles |
|---|---|---|---|
| Preparación de equipo y ambientes | — | 4 | 0.5 |
| Exploración del negocio | (ya realizada en el análisis) | 0 | 0 |
| Desarrollo del sistema – iteraciones | H1–H10 | 243 | 30.4 |
| Documentación, pruebas integrales, estabilización y capacitación | H11 | 28 | 3.5 |
| Instalación | H12 (parte) | 4 | 0.5 |
| Acompañamiento en producción | H12 (parte) | 4 | 0.5 |
| **TOTAL** | | **283 h** | **35.4 días ≈ 1.6 meses** |

> Nota: las 4 h de preparación se suman al total base (279 + 4 = 283 h). Si se aprueban opciones, sumar sus horas a "Desarrollo del sistema" (O1 + O2 completas = +44 h ≈ +5.5 días).
> Versión 2 (2026-09-26 21:10): flujo de 3 roles con estado Devuelta, reintegro, log de acciones, 7 notificaciones y 6 bandejas.

> Versión 3 (2026-09-26 22:20): se agrega H13 (+8 h, +1 día de esfuerzo) por solicitud del cliente. Andrés ajusta tiempos y costos en su hoja.

> Versión 4 (2026-09-27): se agrega H14 (+20 h = 2.5 días de esfuerzo = 5 días calendario a 4 h/día). Costo adicional calculado a la tarifa vigente de la propuesta: $22.400.000 ÷ 65.5 días calendario ≈ $341.985/día × 5 días ≈ $1.710.000. Nuevo total: $24.110.000; duración 70.5 días (3.525 meses de 20 días).

> Versión 5 (2026-09-28): la propuesta separa el precio.
> - **Módulo de Pagos (base, incluye H14 Historial de Precios, sin H13 ni opciones):** 68.5 días calendario (3.425 meses de 20 días) · **$23.426.000**.
> - **Costos adicionales** (tarifa $85.496/h = $24.110.000 ÷ 282 h; duración = horas ÷ 4), válidos solo dentro de la misma propuesta — si se piden aparte, se cotizan de forma independiente (sumando preparación, documentación, pruebas, instalación y acompañamiento):
>   | Funcionalidad | Horas | Duración (días) | Valor |
>   |---|---|---|---|
>   | Inventario del artículo en alarma de cantidades mínimas (H13) | 8 | 2 | $684.000 |
>   | Comprobante adjunto (O1) | 20 | 5 | $1.710.000 |
>   | Página umbrales del semáforo (O2.1) | 5 | 1.25 | $427.000 |
>   | Página tipos de pago (O2.2a) | 4 | 1 | $342.000 |
>   | Página tipos de soporte por tipo de pago (O2.2b, relación pago–soporte) | 6 | 1.5 | $513.000 |
>   | Página motivos (O2.3) | 10 | 2.5 | $855.000 |
