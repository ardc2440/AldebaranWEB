# Complemento a la Guía 00 – Integraciones, estados y preorden de compra

Fecha: 2026-11-11 · Fuente: Andrés + BD de pruebas · **Prevalece sobre la guía 00 donde se contradigan.**

## Correcciones a la guía 00
- **ArchivosTotvs** NO son jobs SQL: es un **worker service** que envía Pedidos y Órdenes de compra a **Totvs** (facturación y pagos de OC). Integración **solo de ida** (Aldebaran → Totvs). No hay acceso a datos de Totvs.
- **Aldebaran no maneja precios ni costos**, solo cantidades. Toda propuesta debe basarse en cantidades, fechas y estados.
- **AutomataExistencias** publica una **copia de `item_references`** (configuración y saldos) en las BD que leen las páginas promocionales **tercerizadas**; los cálculos de disponible los hace la página. Sin quejas conocidas de desactualización.
- Bodegas: solo 2 (Local y Zona Franca), semi quemadas; agregar más exige cambios de código (formatos/reportes).

## Ciclo del pedido (aclarado)
- Reserva → Pedido (o pedido desde cero). Reservado y pedido **marcan** cantidad pero no la sacan.
- Al **trasladar a proceso** se da por extraída del inventario; ahí se personaliza (marcación, sellado) — interna o con satélites externos. Aldebaran **no sabe cuándo termina el proceso**, solo cuándo se despacha.
- Estados: Pendiente · **En proceso** (hay cantidad en proceso, nada despachado) · **Parcialmente atendido** (ya se despachó algo) · Totalmente atendido (todo despachado) · **Cerrado** (se despachó una parte y no se despachará más; requiere nada en proceso y saldo pendiente) · Cancelado.
- OC: Pendiente (= en tránsito, ya se vende sobre ella) · Solicitando aprobación (recibido ≠ solicitado; aprobador registra causa) · Confirmada (entra a Bodega Local o Zona Franca) · Cancelada.

## Usuarios y negocio
- Todas las áreas usan Aldebaran, unas más manualmente que otras (sin claridad de cuál tiene más dolor).
- Hay temporadas fuertes, aunque en general la **rotación es alta**.

## Preorden de compra (proceso existente en BD)
- Se envían por correo PDF de **recomendación de compra** (`SP_Send_Preorder_Notification`, destinatarios en `Preorder_notification_emails`, log en `preorder_notifications`).
- Cálculo en `SP_GET_RECOMENDED_REFERENCE_QUANTITY(@ReferenceId, @Months)`: promedio de las **cantidades solicitadas en OC** de los últimos N meses, descartando atípicos (±2 desviaciones). Si no hay OC: mínimo − (físico − pedido − reservado), +30 %.
- Observaciones (posibles mejoras, a validar con Andrés):
  1. La recomendación se basa en **lo que se compró antes**, no en lo que se **vendió/consumió** (pedidos, traslados a proceso).
  2. El respaldo sin OC **no considera el tránsito** (OC pendientes) y, si no hay mínimo configurado, recomienda comprar el propio disponible.
  3. El comentario dice 10 % pero suma 30 %.
  4. No considera el tiempo de entrega del proveedor.
