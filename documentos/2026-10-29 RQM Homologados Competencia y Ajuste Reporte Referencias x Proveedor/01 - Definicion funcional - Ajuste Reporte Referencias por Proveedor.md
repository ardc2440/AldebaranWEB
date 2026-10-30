# 01 – Definición funcional: Ajuste al Reporte de Referencias del Proveedor

Fecha: 2026-10-29 · Estado: definida con Andrés; pendiente propuesta/aprobación del cliente.
Alcance de este documento: **solo el ajuste del reporte**. Los Homologados de la Competencia se definen aparte (no tienen relación con este punto).

## Situación actual (análisis 2026-10-29)
- Ruta `/report/provider-references` ("Reporte de referencias del proveedor"). SP `SP_GET_PROVIDER_REFERENCE_REPORT (@ProviderId, @ReferenceIds)`.
- Árbol Proveedor (datos de contacto) → Línea → Artículo → Referencias (Código, Nombre, Nombre para el proveedor, Pedida `ORDERED_QUANTITY`, Reservada, Disponible `INVENTORY_QUANTITY`) → Bodegas (Bodega Local, Zona Franca; `references_warehouse`).
- Filtros: Proveedor + `MultiReferencePicker`; al menos uno obligatorio. Acciones: Imprimir y Guardar PDF (HTML de pantalla). Sin Excel.
- Deuda detectada: `STRING_SPLIT` sin `TRY_CAST`; tabla variable sin PK con todas las referencias si no hay filtro; filtro armado como texto → `FromSqlRaw($"EXEC ... {filter}")`; árbol armado en el `.razor.cs` con `.Result` y búsquedas lineales anidadas (cuadrático).
- Regla de negocio (Andrés): toda referencia tiene siempre fila en `references_warehouse` (aunque sea 0). El JOIN interno no pierde referencias.

## Ajuste solicitado
1. **Filtro jerárquico** Línea → Artículo → Referencia (`HierarchicalReferencePicker` de Fase 2). Prevalece el nivel más específico; ninguno obligatorio. Se conserva el filtro de Proveedor.
2. **Switches** (encendido = filtra, apagado = trae todo), iguales al Reporte de Artículos y Referencias, **excepto cantidades mínimas** (no aplican a la finalidad del reporte):
   - Artículo: Solo activos · Solo visibles en página · Solo producto nacional · Solo importación especial · Solo en promoción.
   - Referencia: Solo referencias activas · Solo referencias agotadas.
   - Los switches también filtran lo que ofrece el picker (mismo comportamiento de Fase 2).
3. **Switch "Mostrar inventario y bodegas"**, encendido por defecto:
   - Sí → igual que hoy (cantidades y desglose por bodega).
   - No → solo referencias (Código, Nombre, Nombre para el proveedor), sin saldos ni bodegas.
   - No filtra registros, solo presentación. Aplica a pantalla, Imprimir, PDF y Excel.
4. **Exportar a Excel** según el filtro y lo que muestra la pantalla (con o sin inventario). Una fila por referencia con datos de proveedor, línea y artículo repetidos; con inventario: columnas Pedida, Reservada, Disponible, Bodega Local, Zona Franca.
5. Validación "al menos un filtro": Proveedor, Línea/Artículo/Referencia o algún switch de filtro.

## Técnico (incluido, no como ítem para el cliente)
- SP endurecido: `TRY_CAST`, tablas con PK, `OPTION(RECOMPILE)`, nuevos parámetros; script de pruebas.
- Repositorio con parámetros (`SqlParameter`), no texto concatenado.
- Caso de uso en Application: `GetReportAsync` (árbol) y `GetExportAsync` (filas planas), builders internos, sin `.Result`, agrupación O(n).
- Criterios: ver `Criterios de desarrollo - Aldebaran.Web.md`.
