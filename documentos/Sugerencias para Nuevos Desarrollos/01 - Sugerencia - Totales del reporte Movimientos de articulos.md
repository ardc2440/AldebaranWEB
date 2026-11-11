# Sugerencia 01 – Totales del reporte "Movimientos de artículos"

Fecha: 2026-11-11 · Origen: observación del usuario final · Estado: **idea / pendiente de propuesta**

## Problema observado
En **Reportes → Inventario → Movimientos de artículos** (`/report/reference-movement`), cada sección (Ajustes, Traslados entre bodegas, Órdenes de compra, Reservas, Pedidos) termina con una fila "Total" que es la **suma simple de cantidades** (`movement.Details.Sum(s => s.Amount)`).
En **Ajustes de inventario** mezcla entradas y salidas: un ingreso de 3.000 y un egreso de 2.000 muestran **5.000**, y el usuario lo interpretó como saldo.

## Ya hecho (despliegue v6.11.0)
Etiqueta cambiada a **"Total movido"** en todas las secciones (solo texto; `ReferenceMovementReport.razor`).

## Propuesta de mejora
| Sección | Pie de tabla propuesto |
|---|---|
| Ajustes de inventario | **Ingresos: X · Egresos: Y · Efecto neto: ±Z** |
| Demás secciones | **Total movido: X** (todo va en un solo sentido) |

- El SP `SP_GET_REFERENCE_MOVEMENT_REPORT` ya devuelve `Operator` (+1 Entrada / −1 Salida) y la pantalla ya lo carga en el ViewModel, pero no lo usa: el cambio es solo de presentación (razor + PDF), sin cambios de BD.
- Opcional: mostrar el signo o "Entrada/Salida" en cada línea del detalle de ajustes.
- Beneficio: el usuario ve el efecto real del período sobre el inventario sin hacer cuentas, y se elimina la confusión con un saldo.

## Defectos encontrados en el mismo código — CORREGIDOS en v6.11.0 (2026-11-11)
Corrección: `ReferenceMovementDetailRules.MovementLines` (Application.Services) toma una sola copia por bodega y ordena por fecha y código numérico; 7 pruebas unitarias.

1. **Líneas ocultas por `DistinctBy(d => d.Code)`** en `GetMovementDetailsByTitleIdAsync`: si un documento tiene la misma referencia en más de una línea (p. ej. un ajuste que afecta dos bodegas), solo se muestra la primera y el total queda incompleto.
   Evidencia BD de pruebas (2026-11-11): **7 ajustes** con la misma referencia repetida; traslados, pedidos, OC y reservas: 0.
2. **Orden de las líneas:** `.OrderBy(o => o.Date).OrderBy(o => o.Code)` — el segundo `OrderBy` anula el primero (queda ordenado por código como texto). Debe ser `OrderBy(Date).ThenBy(Code)`.

## Esfuerzo (interno)
Pequeño: ajuste de vista + ViewModel + corrección de agrupación + pruebas unitarias del cálculo + regresión del reporte (pantalla y PDF) + manual de usuario.
