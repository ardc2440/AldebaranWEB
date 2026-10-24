# Traspaso – RQM Complemento Consulta de Logs (continuar en otra sesión de Claude)

Fecha: 2026-10-24 (actualizado) · Rama: `RQM-ComplementoLogs+SaleOff` · Repo: Aldebaran.Web
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
- Fecha de actualización: 2026-10-24 (sesión en la nube, ya con push a la rama).
- E1 y E2 cerradas (logs, alarmas, Track, log general, autómata NLog, T8/T9 Precio bomba, T18 deudas 1 y 4, T19).
- E3:
  - T20, T21, T22/T22b: OK.
  - T23 BD Pedido especial (script 17): OK (probado).
  - T24 Captura (obligatorio al crear sin preselección, confirmación al guardar "Sí, crear / No, modificar", confirmación al cambiar en Editar): OK con correcciones de Andrés (AllowFloatingLabel, TabIndex, "¿").
  - T25 Lista/detalle/resumen: implementado, **sin probar**. El resumen muestra Pedido especial y Observaciones internas al imprimir/PDF y los retira del PDF del correo al cliente (InternalOnlyContent).
  - T26a–f Reportes (Pedidos+export, Back order+export, Actividades, Ventas, Asignación automática, Pendientes traslado): implementado, **sin probar**. Script 18 (8 SP desde respaldo con THROW). Filtros tipados + StoredProcedureCommand. Exportaciones con [Authorize] y filtro por query string (antes: SQL desde la URL sin autenticación). Corrige filtros ignorados en Pendientes de traslado y errores de filtro en Asignación automática.
  - T27 Logs (pedidos, despachos y traslados): implementado, **sin probar**. Script 19. Corrige "Moddificado por".
  - T28 Tablero y pantallas de despachos/traslados: implementado, **sin probar**. DashBoardRepository ya estaba parametrizado.
- Datos de prueba marcados con Precio bomba: HO-13 (ITEM_ID 2554, ancho embalaje 29, original 28) y ASTON-STY (2047). Pedido 39471 (cancelado, 1 un. VA-196 Azul) y los pedidos de prueba de T24. Restaurar al cierre. Remitente del correo en pruebas quedó "Catalogos Promocionales T2" (devolver a "Catalogos Promocionales"; la clave la carga Andrés).

## Pendiente (en orden)
1. **Prueba conjunta T25–T28** (scripts 18 y 19 en BD de pruebas; guías en la conversación / ChangeSets de cada commit).
2. **T29** casos de prueba y ejecución completa E1+E2+E3 (incluye Excel del correo de mínimos con marca $ BOMBA, Movimiento de referencia abierto desde alarma, cierre de un pedido en proceso y crear/modificar OC y reserva).
3. **T17** manuales técnico y de usuario (Word, línea visual de las propuestas).
4. **T30** paquete de despliegue: scripts 01–19, reversas, LEEME, NLog.config del autómata (+ GRANT), validar script 05 (job) en producción. Scripts 16, 18 y 19 se despliegan junto con la app (cambian parámetros/columnas).
5. Deudas técnicas para analizar al cierre: 9 reportes más con SQL concatenado (OC, Reservas, Inventario, Traslados, Ajustes, Stock bodega, Proveedor-referencias, Zona franca, Inventario en proceso).
6. RQM siguiente (anotado, sin iniciar): consecutivos de referencia interna por código (CÓDIGO-NNN), página con Código + Cantidad, Consultar (generados no usados) y Generar; estimado 18–24 h.

## Limitación importante para una sesión en la nube
La BD de pruebas (GML0999\SQLEXPRESS01) y el sitio (https://localhost:6001) están en el computador de Andrés. Una sesión en la nube solo los alcanza si está vinculada a ese computador (app de escritorio). Sin eso puede editar código en la rama pero no probar ni aplicar scripts: en ese caso, implementar, dejar los scripts listos y pedir a Andrés compilar/ejecutar.
