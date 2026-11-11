# Sugerencia 02 – Paginación del reporte "Movimientos de artículos" (deuda técnica)

Fecha: 2026-11-11 · Origen: prueba de regresión del despliegue v6.11.0 · Tipo: **deuda técnica / rendimiento** · Estado: idea

## Situación observada
- Si el usuario filtra sin marcar referencias (solo fechas, o línea/artículo sin bajar a referencias), el reporte trae **todo el catálogo activo**: en la BD de pruebas, 01/01/2024 – 11/11/2026 = **222.368 filas** del SP.
- En la máquina de desarrollo el SP tarda ~23 s y luego la pantalla queda varios minutos sin responder (no se puede volver a abrir Filtros).
- En producción el SP ya se optimizó (variables tipo tabla → tablas temporales con preselección), así que la consulta responde rápido. **El costo que queda es el de la aplicación**: armar y pintar el árbol completo en Blazor Server.
- Nota: en la BD de pruebas el SP todavía es la versión con variables tipo tabla; conviene llevar la versión de producción a pruebas para medir igual.

## Por qué la aplicación se demora (además del SP)
1. **El SP repite cada movimiento por cada bodega** de la referencia (cruce final con `references_warehouse`): las filas se multiplican ×2.
2. **Armado del árbol en memoria con búsquedas repetidas**: por cada línea, artículo, referencia y sección se hace `DataReport.Where(...)` sobre la lista completa (costo ≈ referencias × filas).
3. **Se pinta todo de una vez**: miles de referencias con sus tablas de movimientos en un solo render de Blazor Server (circuito ocupado, sin respuesta del usuario mientras tanto).

## Alternativas
| Opción | Qué hace | Ventajas | Costo |
|---|---|---|---|
| A. Paginación por referencias (recomendada) | El SP recibe `@PageNumber/@PageSize` (o lista de ReferenceIds de la página) y devuelve solo esas referencias; la pantalla pagina por artículos/referencias (igual que el reporte Artículos y referencias, 50 por página) | Tiempo de respuesta constante; mismo patrón ya usado en Fase 2 | Medio: SP + caso de uso + pantalla; PDF/Imprimir siguen generando el documento completo en el servidor con aviso de "documento de gran tamaño" |
| B. Agrupar con diccionario en Application.Services | Construir el árbol en una sola pasada (`ToLookup` por referencia y sección) dentro del caso de uso | Baja el armado de minutos a segundos sin tocar el SP | Bajo; no resuelve el render masivo |
| C. Eliminar la repetición por bodega en el SP | Devolver movimientos y existencias por bodega en dos conjuntos (o existencias agregadas) | Mitad de filas; elimina la necesidad de "quitar repetidos" en pantalla | Medio; cambia el contrato del SP |
| D. Advertencia previa | Si no hay referencias seleccionadas, avisar el volumen estimado y pedir confirmación (como "Documento de gran tamaño") | Muy bajo costo; evita bloqueos accidentales | No mejora el tiempo |

Recomendación: **B + D** como mejora rápida, y **A (+ C)** como solución de fondo.

## Beneficio para el cliente
El reporte responde en segundos sin importar el rango de fechas, y el usuario no queda bloqueado esperando una consulta de catálogo completo hecha por error.

## Esfuerzo (interno)
B + D: pequeño. A + C: medio (SP, caso de uso, pantalla, PDF, pruebas de conteos contra BD y regresión del reporte, incluida la apertura desde alarmas).
