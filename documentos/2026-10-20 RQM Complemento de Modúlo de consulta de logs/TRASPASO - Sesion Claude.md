# Traspaso – RQM Complemento Consulta de Logs (continuar en otra sesión de Claude)

Fecha: 2026-10-23 20:45 · Rama: `RQM-ComplementoLogs+SaleOff` · Repo: Aldebaran.Web
Contexto en esta misma carpeta (léelos primero, en este orden):
1. `TRASPASO - Sesion Claude.md` (este archivo)
2. `Plan de trabajo.md` (historia completa, decisiones, tareas T1–T30)
3. `Criterios de desarrollo - Aldebaran.Web.md` (reglas obligatorias)

## Reglas de trabajo (obligatorias)
- Tareas mínimas comprobables: implementar → Andrés compila en VS → probar → corregir → OK → ChangeSet → siguiente. Nunca dos tareas sin compilar en medio.
- Andrés compila y maneja git. Claude edita archivos, BD de pruebas y prueba (navegador + BD). Al final de cada tarea: descripción del ChangeSet.
- Reportar pruebas solo como OK / correcciones (sin desglose).
- Clean Architecture; modelos por caso de uso; SOLID; **SqlParameter siempre** (helper `StoredProcedureCommand` en `Aldebaran.DataAccess.Infraestructure/Repository`); filtros tipados; nunca SQL concatenado.
- Codificación **UTF-8 sin BOM, CRLF** en todo archivo nuevo o tocado (convertir ISO-8859 con cp1252 y verificar tildes).
- Filtros Sí/No/Todos: `RadzenCheckBox TriState="true" TValue="bool?"` (raya = Todos, chulo = Sí, vacía = No). Si se toca una pantalla que no cumple, se ajusta.
- No modificar componentes compartidos (ej. MultiReferencePicker).
- Error preexistente dentro del alcance → se corrige en el mismo desarrollo.
- Scripts BD en `scripts\RQM Complemento Consulta de Logs\Despliegue\` (+ `Reversa\`, + `LEEME - Orden de despliegue.txt`), re-ejecutables, con respaldo en `RQM_LOGS_OBJECT_BACKUP` (OBJECT_TYPE = `o.type`); modificar SP existentes sobre la definición vigente (validar estructura con THROW) cuando sea posible. Datos: masivos, sin IDs fijos.
- Nunca reemplazar appsettings en producción. Claude nunca digita contraseñas reales de Andrés.

## Estado al traspaso
- E1 y E2 cerradas (logs, alarmas, Track, log general, autómata NLog, T8/T9 Precio bomba, T18 deudas 1 y 4, T19).
- E3:
  - T20 Artículos y referencias: OK.
  - T21 Stock mínimo de bodega: OK.
  - T22 + T22b Movimiento de referencia: **OK (23-oct 20:38)**. Filtros tri-estado Artículo activo / Referencia activa (Sí por defecto) / Precio bomba (Todos). SP con `@IsItemActive`, `@IsReferenceActive`, `@BombPrice` (script 16). Falta probar apertura desde alarma (referencia bloqueada) → incluido en T29.
- Datos de prueba marcados con Precio bomba: HO-13 (ITEM_ID 2554, ancho embalaje 29, original 28) y ASTON-STY (2047). Restaurar al cierre. Remitente del correo en pruebas quedó "Catalogos Promocionales T2" (devolver a "Catalogos Promocionales"; la clave la carga Andrés).

## Pendiente (en orden)
1. **T23–T28 Pedido especial** (`customer_orders.IS_SPECIAL_ORDER`, default 0): obligatorio al crear (Sí/No sin preselección), editable (queda en Track/logs); antiguos = No; sin cambios de lógica; filtro (tri-estado) donde exista filtro de pedido; dato en todos los resultados (reportes Pedidos+export, Actividades, Ventas, Back order+export, Asignación automática, Pendientes traslado automático; logs de pedidos y despachos/traslados; tablero; pantallas de despachos/traslados).
   - Corregir inyección SQL (filtro tipado + StoredProcedureCommand) en los repositorios que se toquen: CustomerOrderReport, CustomerOrderActivityReport, CustomerSaleReport, AutomaticPurchaseOrderAssigmentReport, DashBoardRepository.
   - Opcional propuesto: en los resúmenes de filtros de reportes mostrar "Condición: Sí/No" como en Artículos y referencias.
2. **T29** casos de prueba y ejecución completa E1+E2+E3 (incluye Excel del correo de mínimos con marca $ BOMBA y Movimiento de referencia abierto desde alarma).
3. **T17** manuales técnico y de usuario (Word, línea visual de las propuestas).
4. **T30** paquete de despliegue: scripts 01–16(+), reversas, LEEME, NLog.config del autómata (+ GRANT), validar script 05 (job) en producción. Script 16 se despliega junto con la app (cambian parámetros).

## Limitación importante para una sesión en la nube
La BD de pruebas (GML0999\SQLEXPRESS01) y el sitio (https://localhost:6001) están en el computador de Andrés. Una sesión en la nube solo los alcanza si está vinculada a ese computador (app de escritorio). Sin eso puede editar código en la rama pero no probar ni aplicar scripts: en ese caso, implementar, dejar los scripts listos y pedir a Andrés compilar/ejecutar.
