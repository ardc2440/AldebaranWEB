# Línea base de logs de Aldebaran.Web antes del despliegue (29-sep al 1-oct-2026)

Fuente: `src/Aldebaran.Web/logs` (copiados de producción por Andrés antes de publicar el RQM "Inventario del artículo en la alarma de cantidades mínimas", 1-oct-2026 ~19:00).
Objetivo: separar lo que **ya ocurría** de lo que pueda aparecer **después** del despliegue, e identificar deuda técnica.

## 1. Volumen

| Día | INF | WRN | ERR |
|---|---|---|---|
| 29-sep | 14.265 | 31 | 81 |
| 30-sep | 20.420 | 44 | 54 |
| 1-oct (hasta ~17:30) | 9.505 | 39 | 41 |

## 2. Errores existentes (NO atribuibles a este despliegue)

| # | Error | 29-sep | 30-sep | 1-oct | Origen / hipótesis |
|---|---|---|---|---|---|
| E1 | `NullReferenceException` en `Shared/MainLayout.OnInitializedAsync` | 16 | 14 | 17 | `LoggedEmployee` nulo (usuario sin empleado asociado o sesión vencida) → `LoggedEmployee.EmployeeId` revienta. Se registra también como WRN "Unhandled exception rendering component". |
| E2 | `TaskCanceledException` en circuito + "Navigation failed … customer-orders/N" | 58 | 30 | 18 | `NavigationManager.NavigateTo` sobre un circuito que el navegador ya cerró (usuario cambia de página / cierra pestaña). Ruido de Blazor Server, no pérdida de datos. |
| E3 | `TaskCanceledException` "Navigation failed … Login" | 2 | 2 | 2 | Igual a E2 en el redireccionamiento a Login. |
| E4 | `ObjectDisposedException` (RadzenSplitButton) | – | 1 | 3 | Render sobre componente ya desechado (circuito cerrado). |
| E5 | JS: `removeChild` de null / "No element is currently associated with component" | 5 | – | – | Desincronización DOM-Blazor (diálogos / navegación rápida). |
| E6 | `DbUpdateException` FK_REFERENCE_ITEM al borrar artículo | – | 2 | – | Se intenta eliminar un artículo con referencias; falta validación previa y mensaje de negocio. |
| E7 | `NullReferenceException` en `ReferenceMovementReport` (render) y `JSDisconnectedException` en `OpenFilters()` | – | 2 | – | Reporte de movimientos sin defensa ante datos nulos / circuito cerrado. |
| E8 | `NullReferenceException` en `EditAdjustmentDetail` (render) | – | – | 1 | Detalle de ajuste con dato nulo en la plantilla. |
| E9 | `JSException: 'Radzen' was undefined` | – | 1 | – | Script de Radzen no cargado (caché/arranque del circuito). |

## 3. Advertencias existentes

| # | Advertencia | Observación |
|---|---|---|
| W1 | EF: `Skip/Take` sin `OrderBy` (4 / 20 / 12) | Paginación no determinista: puede repetir u omitir filas entre páginas. |
| W2 | Data Protection: claves en memoria / repositorio efímero / sin cifrado XML | Al reiniciar el sitio se invalidan cookies y antiforgery → usuarios deslogueados. |
| W3 | Rutas de `ApplicationUsers` (GetUsersByRole, LockUser, UnlockUser) con plantilla ambigua | Advertencia de routing al arrancar. |
| W4 | `CorrelationId:` vacío en todas las líneas | Igual que en FileWritingService: no se puede correlacionar una petición. |

## 4. Referencias para comparar después del despliegue

| Indicador | Antes | Esperado después |
|---|---|---|
| Correo de cantidades mínimas | 17:00, duración **≈19 s** (30-sep y 1-oct) | Mismo horario; duración mayor por el inventario por artículo (≈300 consultas de ~1 ms). Vigilar si supera ~60 s. |
| Errores en `MinimumQuantityNotifications`, `ArticleInventoryDialog`, `ArticleInventoryService`, `ArticleInventoryRepository`, `InventoryMinimumAlertService`, `FileBytesGeneratorService` | 0 | 0. Cualquier error con estos nombres **es de este cambio**. |
| Errores en `Index` (Tablero) | 0 | 0 (cambio de pestaña inicial). |
| E1–E9, W1–W4 | Presentes | Siguen presentes: **no** son de este cambio. |

## 5. Deuda técnica propuesta (priorizada)

| Prioridad | Deuda | Acción sugerida |
|---|---|---|
| Alta | E1 MainLayout | Validar `LoggedEmployee` nulo y redirigir a Login / mostrar mensaje. Es el error más frecuente y afecta la carga de cualquier página. |
| Alta | W2 Data Protection | Persistir claves (`PersistKeysToFileSystem` en carpeta con permisos + protección) para no desloguear usuarios en cada reinicio. |
| Media | W1 Skip/Take sin OrderBy | Agregar `OrderBy` estable en las consultas paginadas. |
| Media | E6 borrado de artículo con referencias | Validar antes de borrar y mostrar mensaje de negocio. |
| Media | E7/E8 nulos en reportes/ajustes | Defensas de nulos en plantillas Razor. |
| Baja | E2–E5 ruido de circuito | Manejar `TaskCanceledException`/`JSDisconnectedException` en navegación y JS interop; bajar a nivel Debug. |
| Baja | W4 CorrelationId | Poblar `CorrelationId` en `LogContext` (middleware), igual que en FileWritingService Release 2. |
| Baja | W3 rutas ApplicationUsers | Corregir plantillas de ruta. |
