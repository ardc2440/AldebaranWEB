# Correcciones Aplicadas - Módulo de Conciliación de NC

**Fecha**: 2026-05-20  
**Versión**: 1.1  
**Responsable**: Validación de Escenarios de Conciliación

---

## Resumen Ejecutivo

Se identificaron y corrigieron **2 brechas críticas** en el diseño original del módulo de Conciliación de Notas Crédito (sección 2.2.3) que impedían el manejo completo de escenarios donde:
- **Brecha 4 (Manual)**: NC existe en sistema de Bonificación pero NO en TOTVS
- **Brecha 8 (Masiva)**: NC existe en sistema de Bonificación pero NO en TOTVS (carga masiva)

**Impacto:** Sin estas correcciones, las NC que el sistema calcula pero TOTVS no reconoce quedarían bloqueadas en estado `PENDIENTE` sin posibilidad de rechazo formal, contaminando indefinidamente el indicador de cuadre.

---

## Matriz de Validación de Escenarios

| # | Escenario | Camino | Estado Original | Estado Final |
|---|-----------|--------|-----------------|--------------|
| 1 | NC valores iguales Sistema = TOTVS | Manual | ? Cubierto | ? Cubierto |
| 2 | NC valores diferentes Sistema ? TOTVS | Manual | ? Cubierto | ? Cubierto |
| 3 | NC existe en TOTVS, NO en Bonificación | Manual | ? Cubierto (NC Externa) | ? Cubierto |
| 4 | NC existe en Bonificación, NO en TOTVS | Manual | ?? Brecha detectada | ? Cerrada con 5 correcciones |
| 5 | NC valores iguales Sistema = TOTVS | Masiva | ? Cubierto | ? Cubierto |
| 6 | NC valores diferentes Sistema ? TOTVS | Masiva | ? Cubierto | ? Cubierto |
| 7 | NC existe en TOTVS, NO en Bonificación | Masiva | ? Cubierto (NC Externa masiva) | ? Cubierto |
| 8 | NC existe en Bonificación, NO en TOTVS | Masiva | ? No cubierto | ? Cerrada con 5 correcciones |

**Resultado:** 8/8 escenarios completamente cubiertos.

---

## Detalle de Correcciones por Tarea

### TAREA-059 — Base de Datos (SQL)

**Archivo:** `scripts/CreateCreditNoteReconciliationTables.sql`

**Cambio:**
```sql
-- ANTES (problemático):
DIFFERENCE AS (ISNULL(TOTVS_AMOUNT,0) - SYSTEM_AMOUNT) PERSISTED

-- DESPUÉS (correcto):
DIFFERENCE AS (
    CASE WHEN TOTVS_AMOUNT IS NOT NULL 
         THEN TOTVS_AMOUNT - SYSTEM_AMOUNT 
         ELSE NULL 
    END
) PERSISTED
```

**Justificación:** 
- Con `ISNULL(TOTVS_AMOUNT, 0)`, las NC pendientes (donde `TOTVS_AMOUNT IS NULL`) retornaban `-SYSTEM_AMOUNT`, lo que contaminaba agregaciones que no filtraban por estado.
- `NULL` significa "no conciliada aún" — es semánticamente distinto de `0` (diferencia real de cero).

---

### TAREA-060 — EF Configuration

**Archivo:** `Aldebaran.DataAccess\Configuration\CreditNoteReconciliationConfiguration.cs`

**Cambio:**
```csharp
// ANTES:
.HasComputedColumnSql("ISNULL(TOTVS_AMOUNT,0) - SYSTEM_AMOUNT", stored: true)

// DESPUÉS:
.HasComputedColumnSql(
    "CASE WHEN TOTVS_AMOUNT IS NOT NULL THEN TOTVS_AMOUNT - SYSTEM_AMOUNT ELSE NULL END",
    stored: true)
```

---

### TAREA-061 — Servicio de Conciliación

**Archivos:**
- `ICreditNoteReconciliationRepository.cs`
- `ICreditNoteReconciliationService.cs`

**Cambio 1 — `GetConciliatedDifferenceAsync`:**
```csharp
/// <summary>
/// ?? CORRECCIÓN: Solo suma NC CONCILIADAS (excluye PENDIENTE y RECHAZADA).
/// Implementación: SUM(TOTVS_AMOUNT - SYSTEM_AMOUNT) WHERE STATUS = 'CONCILIADA'
/// </summary>
Task<decimal> GetConciliatedDifferenceAsync(int periodInstanceId, CancellationToken ct = default);
```

**Justificación:** Sin el filtro por estado, NC pendientes y rechazadas contaminan el cálculo del indicador de cuadre.

**Cambio 2 — Método `BulkProcessAsync` (reemplaza `BulkConciliateAsync`):**
```csharp
/// <summary>
/// ?? CORRECCIÓN: Procesa conciliaciones Y rechazos en una sola transacción.
/// </summary>
Task<BulkReconciliationResult> BulkProcessAsync(
    IEnumerable<ReconciliationImportRow> rows, int reviewedBy, CancellationToken ct = default);
```

**Justificación:** Cierra la brecha del Escenario 8 — permite rechazar masivamente NC que no existen en TOTVS.

**Cambio 3 — Regla de negocio documentada:**
> **DECISIÓN DE NEGOCIO**: Si la NC **no existe en TOTVS** ? usar **Rechazar**, no conciliar con `TotvsAmount = 0`. El dialog `ConciliateCreditNote` debe incluir nota informativa.

**Impacto en estimación:** +2 horas (TAREA-061 pasa de 8h a 10h).

---

### TAREA-064 — Dialogs de Conciliación

**Archivo:** `ConciliateCreditNote.razor`

**Cambio:**
```razor
<!-- NUEVO MENSAJE GUÍA -->
<RadzenAlert AlertStyle="AlertStyle.Info" Variant="Variant.Flat" Shade="Shade.Lighter">
    Si esta NC <strong>no existe en TOTVS</strong>, use el botón <strong>Rechazar</strong> 
    en lugar de ingresar valor cero. Valor cero solo debe usarse cuando TOTVS reconoce 
    la NC con monto $0.
</RadzenAlert>
```

**Justificación:** Previene errores del usuario que podría confundir "NC sin TOTVS" con "NC con valor 0 en TOTVS".

**Impacto en estimación:** +1 hora (TAREA-064 pasa de 8h a 9h).

---

### TAREA-065 — Servicio de Importación (Conciliación Masiva)

**Archivos:**
- `ICreditNoteReconciliationImportService.cs`
- `Models\ReconciliationImportRow.cs`

**Cambio 1 — Estructura de la plantilla Excel:**

| Columna | Estado Actual | Estado Original |
|---------|--------------|-----------------|
| ReconciliationId | Oculta | Oculta |
| Número NC | Bloqueada | Bloqueada |
| Distribuidor | Bloqueada | Bloqueada |
| Valor Sistema | Bloqueada | Bloqueada |
| Valor TOTVS | Editable (amarillo) | Editable (amarillo) |
| **Acción** | **NUEVO — Dropdown: CONCILIAR/RECHAZAR** | N/A |
| **Motivo Rechazo** | **NUEVO — Editable** | N/A |
| Notas | Editable | Editable |

**Cambio 2 — Modelo de importación:**
```csharp
public class ReconciliationImportRow
{
    public int ReconciliationId { get; set; }
    public string CreditNoteNumber { get; set; }
    public string Action { get; set; }           // ?? NUEVO: CONCILIAR | RECHAZAR | (vacío = ignorar)
    public decimal? TotvsAmount { get; set; }    // ?? nullable si Action = RECHAZAR
    public string Notes { get; set; }
    public string RejectionReason { get; set; }  // ?? NUEVO: requerido si Action = RECHAZAR
}
```

**Reglas de validación:**
- Fila sin `Acción` ? ignorada (no es error)
- `Acción = CONCILIAR` + `Valor TOTVS` vacío ? error
- `Acción = RECHAZAR` + `Motivo Rechazo` vacío ? error

**Impacto en estimación:** +2 horas (TAREA-065 pasa de 8h a 10h).

---

### TAREA-067 — Pantalla de Conciliación Masiva

**Archivo:** `BulkCreditNoteReconciliation.razor`

**Cambio — Vista previa en 4 tablas (antes eran 2):**

1. **Tabla "NC a Conciliar"** (verde claro) — Número NC, Distribuidor, Valor Sistema, Valor TOTVS, Diferencia
2. **Tabla "NC a Rechazar"** (rojo claro) — Número NC, Distribuidor, Valor Sistema, Motivo Rechazo ? **NUEVA**
3. **Tabla "Filas Ignoradas"** (gris) — Fila #, Número NC, Motivo ? **NUEVA**
4. **Tabla "Errores de Validación"** (amarillo) — Fila #, Número NC, Mensaje

**Cambio — Llamada a servicio:**
```csharp
// ANTES:
await CreditNoteReconciliationService.BulkConciliateAsync(...)

// DESPUÉS:
await CreditNoteReconciliationService.BulkProcessAsync(...)
```

**Justificación:** `BulkProcessAsync` procesa conciliaciones **y rechazos** en una sola transacción.

**Impacto en estimación:** +2 horas (TAREA-067 pasa de 8h a 10h).

---

## Impacto en Estimación Global

| Categoría | Horas Originales | Horas Corregidas | Incremento |
|-----------|------------------|------------------|------------|
| **Tareas REQUERIDAS** | 267 h | 274 h | +7 h |
| **Tareas SUGERIDAS** | 80 h | 84 h | +4 h |
| **Tareas DESEABLES** | 13 h | 13 h | — |
| **TOTAL** | **360 h** | **367 h** | **+7 h (1.9%)** |

**Detalle del incremento:**
- TAREA-061 (REQUERIDA): +2 h ? `BulkProcessAsync` y lógica de rechazo
- TAREA-064 (REQUERIDA): +1 h ? nota informativa guía
- TAREA-065 (SUGERIDA): +2 h ? columnas Acción/Motivo Rechazo
- TAREA-067 (SUGERIDA): +2 h ? tablas de resultado adicionales

---

## Escenarios de Contratación Actualizados

| Escenario | Horas Originales | Horas Corregidas | Días Hábiles |
|-----------|------------------|------------------|--------------|
| MVP Mínimo (solo REQUERIDO) | 267 h | **274 h** | ~34.3 |
| MVP Recomendado | 294 h | **301 h** | ~37.6 |
| Alcance Completo | 360 h | **367 h** | ~45.9 |

---

## Conclusión

? **Todos los 8 escenarios de conciliación están completamente cubiertos** tras aplicar las correcciones.

Las brechas identificadas eran **críticas** — sin ellas, el módulo de Conciliación de NC estaría incompleto y generaría:
- NC bloqueadas indefinidamente en estado `PENDIENTE`
- Indicador de cuadre contaminado con valores incorrectos
- Imposibilidad de cerrar formalmente períodos donde existan NC sin correlación en TOTVS

El incremento de **+7 horas (+1.9%)** en el alcance total es mínimo comparado con el riesgo mitigado.

---

**Aprobación de correcciones:** Documentado para revisión del equipo técnico y product owner.
