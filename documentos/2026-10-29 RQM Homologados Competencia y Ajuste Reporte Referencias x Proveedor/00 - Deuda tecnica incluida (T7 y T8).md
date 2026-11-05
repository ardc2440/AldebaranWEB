# 00 - Tareas de deuda técnica incluidas en esta entrega (T7 y T8)

Origen: rama `Deuda_Tecnica_AldebaranWeb_2026` (T1–T5 publicadas por separado). Decisión de Andrés (2026-11-05): T7 y T8 se desarrollan y publican junto con este RQM.
Detalle completo: documento del proyecto "Deuda Tecnica Aldebaran.Web - Diagnostico y plan".

**Instrucción para el plan de trabajo de este RQM:** agregar T7 y T8 como tareas propias (al final, después de las tareas funcionales), con su prueba y la regresión acumulada. Criterio de deuda técnica: todo error que se encuentre y se valide se corrige en el mismo trabajo.

## T7 – Seguridad de endpoints
1. `/export/AldebaranDb/*` (DT7): verificar si se puede exportar sin sesión; si es así, exigir autenticación (`[Authorize]`).
2. `Notification/PurchaseOrderUpdate`, `CustomerOrderUpdate`, `CustomerReservationUpdate`: hoy aceptan llamadas anónimas. Proteger con clave compartida con el servicio de notificaciones (revisar antes cómo llama el servicio para no romper la entrega de estados).
3. Errores 500 que devuelven el stack trace a quien llama: confirmar que producción no corre como Development y responder 400 ante mensajes mal formados.
4. `MarkMinimumQuantityAlarmsAsRead` (enlace del correo de mínimos) se deja anónimo: está protegido con token cifrado, de un solo uso y con vencimiento.

Prueba: sin sesión → 401/redirección; el notificador sigue actualizando estados; el enlace del correo sigue funcionando.

## T8 – SQL interpolado (DT3)
- Repositorios que ejecutan SP con SQL armado por interpolación de texto (ej. `InventoryReportRepository`) → parámetros (`SqlParameter` / `FromSqlInterpolated` / `StoredProcedureCommand`).
- Ya corregidos en el RQM de Logs: CustomerOrderReport, CustomerOrderActivityReport, CustomerSaleReport, AutomaticPurchaseOrderAssigmentReport, DashBoardRepository.

Prueba: cada reporte tocado devuelve los mismos conteos que antes; búsquedas con comillas simples no fallan.
