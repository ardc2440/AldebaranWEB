# Bitácora de pruebas – Inventario del artículo en la alarma de cantidades mínimas

**Rama:** `RQM-InventarioCompletoEnAlarmaDeCantidadMinima` · **Ambiente:** base de datos de PRUEBAS · **Plan:** `04 - Plan de trabajo - Inventario del articulo en alarma de cantidades minimas.md`
**Método:** cada tarea se prueba y repite la regresión de todas las anteriores. Evidencias en `Evidencias Inventario Alarma\` con nombre `Tn-xx-descripcion.png`.

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
