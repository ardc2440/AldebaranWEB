# 01 – Definición funcional: Ajuste al Reporte de Referencias del Proveedor

Fecha: 2026-10-29 · Estado: **APROBADA por el cliente** (actualizado 2026-11-05). Rama `RQM-HomologadosCompetencia_AjusteReporteReferenciasProveedor`.
Alcance de este documento: **solo el ajuste del reporte**. Los Homologados de la Competencia se definen aparte (no tienen relación con este punto).

## Situación actual (análisis 2026-10-29)
- Ruta `/report/provider-references` ("Reporte de referencias del proveedor"). SP `SP_GET_PROVIDER_REFERENCE_REPORT (@ProviderId, @ReferenceIds)`.
- Árbol Proveedor (datos de contacto) → Línea → Artículo → Referencias (Código, Nombre, Nombre para el proveedor, Pedida `ORDERED_QUANTITY`, Reservada, Disponible `INVENTORY_QUANTITY`) → Bodegas (Bodega Local, Zona Franca; `references_warehouse`).
- Filtros: Proveedor + `MultiReferencePicker`; al menos uno obligatorio. Acciones: Imprimir y Guardar PDF (HTML de pantalla). Sin Excel.
- Deuda detectada: `STRING_SPLIT` sin `TRY_CAST`; tabla variable sin PK con todas las referencias si no hay filtro; filtro armado como texto → `FromSqlRaw($"EXEC ... {filter}")`; árbol armado en el `.razor.cs` con `.Result` y búsquedas lineales anidadas (cuadrático).
- Regla de negocio (Andrés): toda referencia tiene siempre fila en `references_warehouse` (aunque sea 0). El JOIN interno no pierde referencias.

## Ajuste solicitado
1. **Filtro jerárquico** Línea → Artículo → Referencia (`HierarchicalReferencePicker` de Fase 2). Prevalece el nivel más específico; ninguno obligatorio. Se conserva el filtro de Proveedor.
2. **Condiciones de filtro Sí / No / Todos** (casilla de tres estados, criterio 12; raya = Todos por defecto), iguales al Reporte de Artículos y Referencias vigente (tras RQM Complemento Logs), **excepto cantidades mínimas**:
   - Artículo: Activo · Visible en página · Producto nacional · Importación especial · En oferta (`IS_SALE_OFF`) · Precio bomba (`IS_BOMB_PRICE`).
   - Referencia: Activa · Agotada.
   - Las condiciones también limitan lo que ofrece el selector jerárquico (mismo comportamiento de Artículos y Referencias).
3. **Switch "Mostrar inventario y bodegas"**, encendido por defecto:
   - Sí → igual que hoy (cantidades y desglose por bodega).
   - No → solo referencias (Código, Nombre, Nombre para el proveedor), sin saldos ni bodegas.
   - No filtra registros, solo presentación. Aplica a pantalla, Imprimir, PDF y Excel.
4. **Exportar a Excel** según el filtro y lo que muestra la pantalla (con o sin inventario). Una fila por referencia con datos de proveedor, línea y artículo repetidos; con inventario: columnas Pedida, Reservada, Disponible, Bodega Local, Zona Franca.
5. Validación "al menos un filtro": Proveedor, Línea/Artículo/Referencia o alguna condición en Sí/No.

## Técnico (incluido, no como ítem para el cliente)
- SP endurecido: `TRY_CAST`, tablas con PK, `OPTION(RECOMPILE)`, nuevos parámetros; script de pruebas.
- Repositorio con parámetros (`SqlParameter`), no texto concatenado.
- Caso de uso en Application: `GetReportAsync` (árbol) y `GetExportAsync` (filas planas), builders internos, sin `.Result`, agrupación O(n).
- Criterios: ver `Criterios de desarrollo - Aldebaran.Web.md`.
