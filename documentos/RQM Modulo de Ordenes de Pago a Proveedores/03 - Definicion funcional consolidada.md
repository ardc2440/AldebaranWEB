# RQM Módulo de Órdenes de Pago a Proveedores
## 03 - Definición funcional consolidada (respuestas de Andrés 2026-09-26)

Reemplaza las propuestas R1–R9 y las preguntas P1–P16 del documento 02 en todo lo que se contradiga.

---

## 1. Datos nuevos

### 1.1 Proveedor
- **Moneda** (`CURRENCY_ID` → `currencies`, tabla existente). Solo informativa: no hay TRM ni conversiones.

### 1.2 Orden de Compra (encabezado)
| Dato | Regla |
|---|---|
| Fecha estimada de pago | Requerida al **crear** y al **confirmar** si no la tiene |
| Valor adicional de costo | Un solo valor que totaliza los costos adicionales (≥ 0) |
| Valor total proforma | > 0 |
| Moneda | Se copia del proveedor al crear/registrar datos (snapshot informativo, para que un cambio posterior en el proveedor no altere OC ya emitidas) |

### 1.3 Orden de Compra (detalle)
- **Valor por unidad** (> 0). Se muestra **Valor unidad × Cantidad solicitada** (calculado, no se guarda).
- Validación: Σ(Valor unidad × Cant. solicitada) = Total proforma − Valor adicional.
- **El pago se basa siempre en la cantidad solicitada**: se paga antes de que llegue la mercancía.

### 1.4 OC existentes sin datos de pago
- En BD las columnas son **nullables**; la aplicación las exige:
  - OC **nueva** → al crear.
  - OC **Pendiente** → al editar y al **confirmar** (si no las tiene, se piden).
  - OC **Confirmada** → flujo de habilitación (sección 4). Caso transitorio que tiende a desaparecer.
- La Interfaz CSV (Fase 1) **no cambia**.

---

## 2. Orden de Pago (OP)

### 2.1 Reglas de saldo
- Una OC puede tener **varias OP**. Solo las OC **Canceladas** están excluidas (se puede pagar desde que se crea la OC).
- **Comprometido** = Σ OP no anuladas (Pendiente, Devuelta, Aprobada, Ejecutada).
- **Saldo por pagar** = Total proforma − Comprometido. Una OP nueva (o su edición) **no puede superar el saldo**.
- **Principio rector: no permitir pagos dobles.** La única excepción es una OP **anulada**, que deja registro, motivo y responsable. Control de concurrencia en BD (bloqueo de la OC dentro de la transacción de registro) para que dos usuarios no comprometan el mismo saldo al mismo tiempo.
- Anular una OP (en cualquier estado, **incluida Ejecutada**) restablece el saldo.
- Cancelar una OC: **no se permite si tiene OP no anuladas**. Se valida al solicitar la cancelación y al aprobarla.
- Editar una OC Pendiente: el nuevo total no puede quedar por debajo de lo comprometido.

### 2.2 Estados y flujo
```
                 Devolver (motivo)
            ┌──────────────────────────┐
            ▼                          │
Crear → PENDIENTE ──Aprobar (motivo)──► APROBADA ──Ejecutar (tipo de pago, referencia)──► EJECUTADA
            │                          │                                               │
            └──────────── Anular (motivo de lista + comentario) ───────────────────────┘
                                         ▼
                                      ANULADA (final)
```
- **Devolver** regresa la OP a Pendiente para que el creador la corrija (se registra la devolución en el historial).
- **Aprobar**, **Devolver** y **Anular** requieren **motivo parametrizado + comentario**. Se estandariza igual para las tres acciones.
- Validación de comentarios: se rechaza texto sin sentido (solo caracteres repetidos como `aaaaaa` o `......`, sin letras, o por debajo de una longitud mínima).
- **Ejecutar** registra **Tipo de pago** (catálogo: Cheque, Transferencia, Crédito, …) y el soporte de cómo se hizo (número de referencia / comprobante).

### 2.3 Registro de la OP
Número (secuencia propia) · OC asociada · Proveedor (vía OC) · Moneda (informativa) · Valor · Estado · Creación (fecha, funcionario) · Aprobación (fecha, funcionario, motivo, comentario) · Ejecución (fecha, funcionario, tipo de pago, referencia) · Anulación (fecha, funcionario, motivo, comentario) · Historial de todas las acciones (incluye devoluciones, que pueden repetirse).

### 2.4 Documento imprimible
- Imprimir y descargar PDF con el mecanismo actual (`getContent` → `FileBytesGeneratorService`).
- Formato basado en el **formato de Pedido**, con estilo propio. Si está aprobada o ejecutada, muestra **quién la aprobó** (fecha y motivo). Es el **documento soporte del pago al proveedor**.
- Marca visual de estado (ej. "ANULADA").

---

## 3. Roles
| Rol | Permisos |
|---|---|
| Consulta de órdenes de pago | Ver OP, pestaña de pagos, reporte, imprimir |
| Creación de órdenes de pago | Crear OP; corregir OP devueltas; solicitar habilitación de datos de pago en OC Confirmadas |
| **Gestión de órdenes de pago** (rol único de gestión) | Aprobar para ejecución, Devolver, Anular, Aprobar habilitación de datos de pago en OC Confirmadas. Recibe las notificaciones Windows |
| Registro de datos de pago en OC | Editar **solo** los datos de pago de OC Confirmadas **con habilitación aprobada** |
| **Ejecución de órdenes de pago** (rol nuevo) | Pasar la OP Aprobada a Ejecutada con tipo de pago, referencia (y comprobante si se contrata la opción F4). Recibe notificación Windows de OP pendientes de ejecución y tiene su componente en el Dashboard |

---

## 4. Habilitación de datos de pago en OC Confirmadas (flujo transitorio)
1. El usuario de Creación de OP selecciona la OC. Si le faltan datos, **no se permite crear la OP** y el sistema registra una **solicitud de habilitación**.
2. Se genera una **notificación Windows** al rol de Gestión.
3. Gestión **aprueba** la habilitación (motivo + comentario).
4. El rol de Registro de datos de pago edita **solo** estos campos: fecha estimada de pago, valor adicional, total proforma y valor unitario por referencia. Aplica la validación Σ = proforma − adicional.
5. Se cierra la habilitación y la OC queda disponible para crear la OP.
- **No reutiliza** la modificación de OC actual: no dispara notificaciones a clientes, alarmas de tránsito ni triggers de inventario. Tiene su propio log.

---

## 5. Notificaciones Windows (sobre `notification_definitions` existente)
Se configuran como en Aprobación de Ajustes de OC (frecuencia y roles por BD). Se desarrollan desde cero como definiciones nuevas:
1. OC con **saldo por pagar > 0** cuya **fecha estimada de pago ya llegó**.
2. OP **pendientes de gestión** (Pendiente → aprobar).
3. **Solicitudes de habilitación** de datos de pago pendientes.
4. OP **Aprobadas pendientes de ejecución** → rol de Ejecución.
(El sistema es web: no hay alarmas; solo notificación Windows + Dashboard.)

## 6. Dashboard (Tablero de notificaciones existente, único)
- Nuevo componente: OC con saldo por pagar y fecha estimada de pago **próxima o vencida**, con semáforo **Verde / Amarillo / Rojo**.
- Componente para el rol de Ejecución: OP Aprobadas pendientes de ejecutar.
- Umbrales en `SYSTEM_PARAMETERS` (tabla existente de la Fase 1).

## 7. Consulta de OC
- Nueva pestaña **"Pagos"** en el detalle expandible: OP con estado, valores, responsables y un resumen (Total proforma · Comprometido · Ejecutado · Saldo).
- Las **OP anuladas se resaltan** (fila marcada y/o ícono con tooltip del motivo), y la OC muestra un indicador de que tiene pagos anulados.
- Acción "Imprimir OP" desde la pestaña.

## 8. Reporte de Órdenes de Pago (único)
Es un **listado, no operacional**: un solo reporte que filtra por estado. Cubre también "OC pendientes de pago" (decisión F5).

**Base del reporte: la OC**, con sus OP (LEFT JOIN). Así aparecen también las OC **sin ninguna OP** que tienen saldo por pagar.

### 8.1 Filtros
| # | Filtro | Origen | Para qué sirve (no relleno) |
|---|---|---|---|
| 1 | Proveedor | Cliente | Estado de cuenta con un proveedor |
| 2 | Orden de compra | Cliente | Todo lo pagado/pendiente de una OC |
| 3 | Orden de pago | Cliente | Ubicar un soporte puntual |
| 4 | Fecha de pago (ejecución), rango | Cliente | Cierre / conciliación de un periodo: qué salió |
| 5 | Estado de la OP (selección múltiple) | Cliente | Pendientes, aprobadas por ejecutar, ejecutadas, anuladas |
| 6 | **Situación de saldo de la OC**: Con saldo pendiente · Pagada totalmente · Sin pagos | Propuesto | Reemplaza el "reporte de OC pendientes de pago" |
| 7 | **Fecha estimada de pago**, rango + atajo "Vencidas" (≤ hoy con saldo) | Propuesto | Flujo de caja: qué hay que pagar la próxima semana / qué está atrasado |
| 8 | **Moneda** | Propuesto | Tesorería planea por moneda; los totales nunca mezclan monedas |
| 9 | **Tipo de pago** | Propuesto | Conciliación bancaria (transferencias vs cheques) |
| 10 | **Estado de la OC** (Pendiente / Confirmada / Ajuste en aprobación) | Propuesto | Distinguir pagos anticipados (mercancía aún no recibida) de pagos sobre mercancía recibida: exposición al riesgo con el proveedor |
| 11 | **Con OP anuladas** | Propuesto | Auditoría: OC donde hubo anulaciones o devoluciones de pagos |
| 12 | **OC sin datos de pago** | Propuesto (transitorio) | Depurar OC antiguas que aún no tienen proforma / valores; desaparece cuando se normalicen |

Descartados por ser relleno: fecha de creación de la OP, creado/aprobado por, puerto, número de importación, transportadora, referencias (se ven en el detalle y en el reporte de OC).

### 8.2 Presentación
Proveedor → OC (Moneda, Estado OC, Fecha estimada de pago con semáforo, Total proforma, Comprometido, Ejecutado, Saldo) → OP (Número, Estado, Valor, Fecha creación, Aprobado por/fecha, Fecha de pago, Ejecutado por, Tipo de pago, Referencia; si anulada: motivo, por, fecha — resaltada).
Totales por OC, por proveedor y **por moneda**. PDF / impresión como los demás. SP con **parámetros** (`SqlParameter`).

## 9. Parametrización — dos opciones para cotizar al cliente
| Catálogo | Opción A: solo BD (script) | Opción B: página de administración |
|---|---|---|
| Umbrales del semáforo (`SYSTEM_PARAMETERS`) | Incluido | Costo adicional |
| Tipos de pago | Incluido | Costo adicional |
| Motivos (aprobación, devolución, anulación) | Incluido | Costo adicional |

---

## 10. Decisiones finales (2026-09-26)
- **F1.** Ejecuta un **rol nuevo** (Ejecución de órdenes de pago), con notificación Windows de pendientes de ejecución y componente en el Dashboard.
- **F2.** Anula el rol de **Gestión** directamente, con motivo y comentario. No hay estado de "solicitud de anulación".
- **F3.** El creador puede **editar el valor** de la OP mientras esté Pendiente (incluye las devueltas), respetando el saldo.
- **F4.** Adjuntar comprobante en la ejecución: **opcional para cotizar**; el cliente decide. El diseño deja la ejecución preparada para agregarlo sin rehacer.
- **F5.** Un **único reporte** (listado) filtrado por estado y por situación de saldo; incluye las OC con saldo pendiente.

---

## 11. Redefinición del flujo de la OP (Andrés, 2026-09-26 20:30–20:53) — REEMPLAZA 2.2, 3 y F1/F2 donde se contradiga

### 11.1 Roles del flujo
Tres roles: **Creador**, **Aprobador** (antes "Gestión") y **Ejecutor**. Cada uno puede anular en su estado. Se mantiene el rol **Registro de datos de pago en OC** y el de **Consulta**.

### 11.2 Estados y acciones
| Estado | Rol | Acciones |
|---|---|---|
| En creación | Creador | Editar · Enviar a aprobación (comentario opcional) · Anular |
| En aprobación | Aprobador | Enviar a ejecución · Devolver a creación · Anular |
| En ejecución | Ejecutor | Finalizar · Anular |
| Ejecutada | Ejecutor | Registrar reintegro (total) |
| Anulada | — | Final. Libera saldo |
| Reintegrada | — | Final. Libera saldo |

- El saldo se compromete desde la creación (incluye En creación).
- Solo se edita En creación (incluye las devueltas).
- De En ejecución **no** se vuelve a En aprobación: si hay problema, se anula y se crea una nueva OP.

### 11.3 Información por acción
| Acción | Tipificación (lista) | Comentario |
|---|---|---|
| Crear / Editar | — | — |
| Enviar a aprobación | — | Opcional |
| Aprobar (enviar a ejecución) | Motivo de aprobación | Obligatorio |
| Devolver a creación | Motivo de devolución | Obligatorio |
| Anular (3 roles) | Motivo de anulación | Obligatorio |
| Finalizar | Tipo de pago + Tipo de soporte (Físico / Digital) + referencia | Obligatorio |
| Registrar reintegro | Motivo de reintegro (Devolución del proveedor, Rechazo bancario, Registro erróneo…) + fecha y soporte del reintegro | Obligatorio |
Comentarios obligatorios con validación de texto con sentido.

### 11.4 Reintegro
- Una OP Ejecutada significa que ya se pagó al proveedor; no se "anula": se registra un **reintegro** cuando consta que el dinero no quedó en poder del proveedor.
- Siempre por el **total** de la OP. Si el proveedor retuvo una parte, se reintegra la OP completa y se crea una nueva por el valor correcto.
- Lo registra el **Ejecutor**, sin aprobación; se notifica al Aprobador y queda en el historial. El documento impreso muestra "REINTEGRADA".

### 11.5 Otras confirmaciones
- Soporte: alcance base = tipo de soporte (Físico / Digital) + referencia/observación. Adjuntar archivo = Opción 1 (opcional para el cliente).
- La habilitación de datos de pago en OC Confirmadas la aprueba el **Aprobador**.

### 11.6 Estado Devuelta (confirmado)
- **Devuelta** es un estado propio (no vuelve a En creación): Creador → Editar · Reenviar a aprobación (comentario opcional) · Anular; muestra el motivo de la devolución.

### 11.7 Tablero, notificaciones y página de OP (confirmado)
- Regla: una bandeja existe solo para lo que **me llega de otro (o del tiempo) y debo atender**; no depende de que la notificación funcione.
- **6 bandejas**: OC por pagar (semáforo) → Creador · OP devueltas → Creador · OP en aprobación → Aprobador · Habilitaciones de datos de pago → Aprobador (Aprobar / Rechazar) · OC habilitadas para registrar datos → Registro de datos · OP en ejecución → Ejecutor.
- **7 notificaciones Windows**: una por bandeja + reintegro registrado → Aprobador.
- **Página "Órdenes de Pago"** (opción base, como Órdenes de compra o Pedidos): crear, consultar todas, filtrar por estado y ejecutar todas las acciones según estado y rol, incluido el reintegro.
- **Pestaña "Pagos" en la consulta de OC se mantiene** (consulta de las OP de la OC, resumen de saldo, anuladas/reintegradas resaltadas, imprimir).

### 11.8 Trazabilidad: datos en la OP + log de acciones (confirmado)
- La OP guarda en su registro los datos de la **última** creación, aprobación, ejecución, anulación / reintegro (lo que se imprime en el documento soporte).
- Como el flujo es de ida y vuelta (devoluciones y reenvíos repetidos), se agrega un **log de acciones** con cada transición: fecha, funcionario, acción, estado anterior → estado nuevo, motivo, comentario y, cuando aplique, valor anterior → valor nuevo (ediciones) y datos del pago / reintegro.
- El log se consulta como **Historial** desde la página de Órdenes de Pago y desde la pestaña Pagos de la OC.
