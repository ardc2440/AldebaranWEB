# Guía de arranque – Conocimiento funcional del sistema Aldebaran (PROMOS)

Carpeta: `documentos\Sugerencias para Nuevos Desarrollos\` · Fecha de corte: 2026-11-11 · Versión en producción al corte: **Aldebaran.Web v6.11.0**

## 1. Para qué existe esta carpeta y cómo usarla

Esta carpeta es el punto de partida de una **nueva conversación** cuyo objetivo es **proponer soluciones nuevas que den beneficio al cliente PROMOS**. No es un requerimiento aprobado: es un banco de ideas que, si se validan, se convierten en propuesta al cliente.

Reglas para la conversación que arranca aquí:
1. Leer primero esta guía completa; luego los archivos `NN - Sugerencia - ….md` de la carpeta (cada uno es una idea ya identificada).
2. Cada idea nueva se registra como un archivo `NN - Sugerencia - <tema>.md` con: problema observado (con evidencia: datos de BD de pruebas, pantalla, reporte), beneficio para el cliente, propuesta funcional, alcance base y opcionales, riesgos, y esfuerzo estimado **solo** en sección interna.
3. Una idea pasa a propuesta formal siguiendo el flujo de Andrés (sección 10). La propuesta al cliente **nunca** lleva horas ni hitos.
4. Antes de explorar código a ciegas, preguntarle a Andrés. La BD de pruebas y el sitio local se pueden usar libremente para buscar evidencia.

## 2. El cliente y el negocio

- **PROMOS** (marca *Catálogos Promocionales / Cataprom*): importa, fabrica/marca y comercializa **artículos promocionales** (bolígrafos, mugs, termos, audífonos, textiles, etc.) a empresas, a través de asesoras comerciales y de un catálogo web.
- El cliente es **sensible al costo** ("le gusta todo sin gastar"): toda propuesta debe demostrar valor concreto (tiempo ahorrado, errores evitados, ventas no perdidas, inventario mejor usado) para justificar el precio.
- Quien desarrolla y mantiene: **Andrés** (factura los roles de Arquitecto, Desarrollador Senior, Analista de pruebas y Analista funcional).
- Contacto de soporte técnico en PROMOS: Jhon (destinatario de informes técnicos).

Volumen aproximado (BD de pruebas, copia de producción, nov-2026): ~2.700 artículos, ~7.550 referencias, ~6.000 clientes, ~450 proveedores, ~39.000 pedidos, ~740 órdenes de compra, 2 bodegas.

## 3. Mapa del ecosistema (qué piezas existen)

| Pieza | Qué hace | Tecnología |
|---|---|---|
| **Aldebaran.Web** | ERP operativo de inventario: maestros, pedidos, reservas, OC, ajustes, traslados, despachos, alarmas, logs y reportes | .NET 7, Blazor Server + Radzen, EF Core 7, SQL Server |
| **NotificationProcessor** + Notificator | Cola de correos: envía las notificaciones (OC, pedidos, reservas) y reporta el estado a la web por un *hook* firmado (HMAC) | Servicio Windows .NET |
| **Correo periódico de cantidades mínimas** | Envía a cada funcionario el Excel de alarmas de cantidades mínimas (con miniaturas, tope de tamaño 15 MB) | Worker dentro de la web |
| **FileWritingService** | Genera PDF y Excel de inventario y los sube por FTP (consumidos por terceros/página) | Servicio Windows .NET |
| **AutomataExistencias** | Sincroniza existencias de Aldebaran hacia **Cataprom** (catálogo web) | Servicio Windows; log NLog a BD |
| **Exportación CSV diaria** (Fase 1) | Job SQL 02:00: 3 CSV (Pedidos, Inventario, Órdenes de compra) incrementales para la **IA** del cliente, que reemplazó al "Módulo Gerencial de Inventarios V2" | SP + bcp + Job SQL Agent |
| **ArchivosTotvs / Jobs SQL** | Procesos de BD (preorden, alarmas diarias, depuración) | SQL Server |

## 4. Conceptos de inventario (el corazón del sistema)

- **Línea → Artículo → Referencia.** El artículo es el producto (p. ej. "Mug Metálico Wilem 380 ml"); la referencia es la variante vendible (color/talla), con código interno. El inventario se lleva **por referencia y por bodega**.
- **Bodegas:** *Bodega Local* y *Zona Franca* (mercancía importada aún no nacionalizada). Reporte "Zona franca vs. Disponible".
- **Cantidades por referencia:** Físico (INVENTORY_QUANTITY), **Reservado** (reservas vigentes), **Pedido/Comprometido** (pedidos pendientes), **Tránsito** (OC Pendientes o en Ajuste en aprobación), **En proceso** (cantidades trasladadas a proceso).
  Fórmula usada en el CSV para la IA: `Disponible = Físico + Tránsito − (Reservado + Pedido)`.
- **Marcas del artículo:** Activo, Visible en página, Producto nacional, Importación especial, **En oferta** (antes "Promoción"; agrega sufijo al nombre y NO genera alarmas de mínimos) y **Precio bomba** (sufijo `- $ BOMBA`; excluyente con Oferta; SÍ genera alarmas).
- **Marcas de la referencia:** Activa, Agotada, Cantidad mínima general, Cantidad mínima en Bodega Local, Variación permitida en OC.
- **Consecutivos de referencia interna** por línea/área; **Artículos por área** (qué funcionario/área maneja qué artículos).
- Movimientos de inventario los hace la **aplicación** (triggers de EF en `Aldebaran.DataAccess/Core/Triggers`), no triggers de BD; las **alarmas** sí las calculan SP y triggers de BD.

## 5. Procesos y ciclos de vida de documentos

| Documento | Estados | Notas de negocio |
|---|---|---|
| **Reserva** | Pendiente → En pedido / Cancelada / Vencida | Aparta cantidad para un cliente por un tiempo; al vencer se libera (alarma de reservas vencidas). Se puede convertir en pedido. |
| **Pedido** | Pendiente → En proceso → Parcialmente atendido → Totalmente atendido → Cerrado; Cancelado | Marca **Pedido especial** (obligatoria al crear). Cancelación y cierre requieren **solicitud y aprobación** (Solicitudes operativas). Descarga de formato de pedido. |
| **Traslado a proceso** | Realizado / Cancelado | Pasa cantidades del pedido a producción/marcación (incluye **traslados automáticos** y cambio de satélite – confirmar detalle con Andrés). |
| **Despacho** | Realizado / Cancelado | Entrega al cliente de lo procesado; descuenta inventario. Protegido contra doble despacho. |
| **Orden de compra** | Pendiente → Confirmada; Ajuste en aprobación; Cancelada | Confirmar = recibir mercancía (sube inventario). Cambios de cantidad fuera de **tolerancia** pasan a "Ajuste en aprobación". Cancelación por solicitud aprobada. Al cambiar fechas de una OC en tránsito se alerta a pedidos afectados. |
| **Ajuste de inventario** | Realizado / Cancelado | Tipo Entrada (+) o Salida (−), con **motivo** (~80 motivos: inventario físico, devolución, producto defectuoso, faltante, nacionalización, traslado entre referencias…) y bodega. |
| **Traslado entre bodegas** | Realizado / Cancelado | De Zona Franca a Local, etc. |
| **Solicitudes** (cancelación OC, cancelación pedido, cierre pedido) | Pendiente → Aprobada / Denegada | Menú "Gestión de solicitudes operativas" (pendientes / atendidas). |

## 6. Alarmas y Tablero de notificaciones

El **Tablero de notificaciones** es la pantalla de inicio. Cada funcionario ve las alarmas que le asignaron ("Alarmas por funcionario") y las marca como vistas (queda log de visualización). Tipos (cada uno con su rol de consulta):
- Cantidades mínimas (general) · Cantidad mínima en Bodega Local · Artículos sin disponible · Ingreso de mercancía a Bodega Local.
- Órdenes próximas a vencer · Órdenes modificadas con afectación en pedidos.
- Pedidos vencidos · Reservas vencidas.
- Creación automática de traslados a proceso · Modificaciones automáticas de traslados.
- Alarmas del día · Correos enviados con error.
El diálogo de inventario de la alarma muestra el inventario completo del artículo. El correo periódico envía el mismo contenido de mínimos en Excel.

## 7. Menú completo (v6.11.0)

- **Administración:** Artículos · Consecutivos de referencia · Artículos por áreas · Transportadoras · Funcionarios · Alarmas por funcionario · Clientes · Proveedores · **Competencia** (Compañías, Homologaciones) · Tolerancias en ajustes de O.C. · Notificaciones (configuración de correos) · Conectividad · **Log del sistema** (Log general, Track por entidad).
- **Movimientos de inventario:** Órdenes de compra · Reserva de artículos · Pedido de artículos · Traslado de pedidos a proceso · Despacho de pedidos procesados · Ajustes de inventario · Traslado entre bodegas.
- **Gestión de solicitudes operativas:** Pendientes · Atendidas.
- **Consulta de Log:** de órdenes (modificación, cancelación, solicitudes de ajuste, tolerancias, visualización de alarmas) · de pedidos (modificación, cancelación, cierre, despachos y traslados a proceso mod./canc., alarmas) · de reservas · de inventarios (visualización de alarmas, inactivaciones y refrescos de alarmas).
- **Reportes** (todos con PDF; varios con Excel; rol "Consulta de reportes"):
  - Inventario: Inventario · Inventario en proceso · **Movimientos de artículos** · Existencias por bodega · Referencias con existencia mínima en bodega · Ajustes de inventario · Traslados entre bodegas · Zona franca vs. Disponible.
  - Catálogos: Artículos y referencias · Homologaciones con la competencia.
  - Clientes: Reservas por cliente · Pedidos por cliente · Backorder por cliente · Traslados a proceso sin cambio de satélite · Actividades de pedidos · Ventas por cliente.
  - Proveedores: Referencias del proveedor · Órdenes de compra · Notificación de pedidos atendidos automáticamente.
- **Usuario:** Perfil · Inicios de sesión · Roles (~65 roles granulares: consulta/modificación por módulo, aprobaciones, logs, cada tipo de alarma).

## 8. Historia reciente de requerimientos (lo que el cliente ya compró)

| Requerimiento | Qué entregó |
|---|---|
| Fase 1 – Interfaz Inventario y Movimientos | 3 CSV diarios para la IA del cliente |
| Fase 2 – Reporte Artículos y Referencias | Reporte de catálogo con filtros, PDF y Excel |
| Inventario completo en alarma de mínimos | Diálogo con inventario del artículo en la alarma |
| Envío periódico de cantidades mínimas | Correo con Excel por funcionario |
| Complemento Consulta de Logs + Oferta/Precio bomba | Logs consultables, Track, log general, Precio bomba, Pedido especial, filtros en reportes |
| Deuda técnica 2026 | Doble despacho, despachado > solicitado, correo con miniaturas, etc. |
| **Homologados Competencia + Referencias por Proveedor** (nov-2026) | Catálogo de la competencia con imagen y reporte; reporte de proveedor con filtro jerárquico, 8 condiciones, inventario opcional y Excel; seguridad (hook firmado, SQL parametrizado, [Authorize] en reportes) |
| Propuesta pendiente de decisión | **Módulo de Órdenes de Pago a Proveedores** (ligado a OC); solo se aprobó una parte |

Ideas descartadas o no aprobadas antes: "Módulo Gerencial de Inventarios V2" (reemplazado por la IA), "API Inventario y Movimientos".

## 9. Deuda técnica y debilidades conocidas (fuente de propuestas)

- **.NET 7 fuera de soporte** → migración a .NET 8/9 (DT2/DT4/DT5/DT6 del diagnóstico).
- Menú: grupos padre visibles aunque el usuario no tenga roles hijos (DT1).
- Sin pruebas E2E automatizadas (propuesta Playwright) ni bUnit en componentes compartidos.
- FileWritingService pendiente de publicar con la llamada parametrizada (T22).
- Reporte Movimientos de artículos: ver `01 - Sugerencia - Totales del reporte Movimientos de articulos.md` (incluye un defecto con casos reales).
- ~200 referencias con stock físico negativo en origen (dato observado en Fase 1).
- Alarmas: el estado activo depende de jobs diarios (depuración de inactivos).

## 10. Cómo se trabaja con Andrés (obligatorio)

- Flujo: análisis → definición funcional → **propuesta al cliente** (formato Word de la propuesta de Fase 1, con opcionales que el cliente decide) → aprobación → plan de tareas → rama → desarrollo.
- Estimación por hitos y roles en archivo **interno** aparte (insumo de costo), nunca en la propuesta.
- Desarrollo en tareas mínimas comprobables con **regresión acumulada**; Andrés compila en VS, corre pruebas y maneja git; Claude edita, prueba en navegador y BD, y entrega texto de ChangeSet.
- Criterios técnicos: Clean Architecture (UI → Application.Services por caso de uso → DataAccess), SOLID, `StoredProcedureCommand`/SqlParameter (nunca SQL concatenado), UTF-8 sin BOM + CRLF, filtros Sí/No/Todos tri-estado, no tocar componentes compartidos (MultiReferencePicker, ImageDialog, print.css), errores preexistentes en alcance se corrigen, scripts de BD re-ejecutables con reversa y masivos (sin IDs fijos), nunca reemplazar appsettings en producción.
- Entorno: sitio local `https://localhost:6001` y BD de pruebas (copia de producción) modificables sin riesgo.
- Reportar pruebas como OK / correcciones, sin desglose largo.

## 11. Preguntas semilla para generar propuestas

1. ¿Dónde pierde tiempo hoy el usuario? (pasos repetitivos en pedidos, despachos, aprobaciones).
2. ¿Dónde se pierden ventas? (agotados, reservas que vencen, OC atrasadas sin aviso al cliente).
3. ¿Qué decisiones toma gerencia sin dato? (rotación, inventario inmóvil, rentabilidad por cliente/proveedor, precio vs. competencia ahora que hay homologaciones).
4. ¿Qué de la IA del cliente se podría alimentar mejor (más CSV, datos de competencia, tiempos de proveedor)?
5. ¿Qué riesgos operativos existen? (stock negativo, ajustes masivos sin control, dependencia de .NET 7).
