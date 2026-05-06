# 2. PROPUESTA FUNCIONAL - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS  
**Estado**: PROPUESTA FUNCIONAL  
**Fecha**: Mayo 2026  
**Versión**: 1.0

---

## 2.1 ALCANCE DE LA SOLUCIÓN

### 2.1.1 Problemática Actual

Actualmente, PROMOS enfrenta las siguientes dificultades en el proceso de bonificación de distribuidores:

- **Proceso Manual y Lento**: Los distribuidores deben calcular manualmente sus bonificaciones y validar el cálculo con personal de PROMOS, lo cual consume tiempo valioso de ambas partes.

- **Falta de Transparencia**: Los distribuidores no tienen visibilidad en tiempo real sobre cómo se calculan sus bonos ni cuánto les falta para alcanzar el siguiente nivel de bonificación.

- **Riesgo Competitivo**: Los distribuidores prefieren comprar con la competencia que ya tiene procesos automatizados y transparentes de bonificación.

- **Tiempo Administrativo No Productivo**: PROMOS invierte tiempo validando cálculos manuales que no agregan valor estratégico al negocio.

- **Dificultad para Resolver Reclamaciones**: Sin auditoría automática, resolver reclamaciones de distribuidores sobre bonos calculados incorrectamente es complejo y consume recursos.

### 2.1.2 Solución Propuesta

Se propone desarrollar un **Sistema Automatizado de Bonificación de Distribuidores** que permita:

**Para Distribuidores:**
- Acceder desde la **Página Promocional** mediante un botón "Ver mi bonificación"
- Autenticarse de forma segura mediante **código OTP** enviado por SMS o Email
- Consultar en **tiempo real** (menos de 500ms) el bono acumulado del período actual
- Ver **gamificación**: cuánto falta para alcanzar el siguiente nivel de bonificación
- Acceder al **histórico** de bonos de períodos anteriores
- Descargar **comprobantes** de bonificación en formato PDF
- Recibir **notificaciones automáticas** (SMS/Email) cuando:
  - Alcanza un nuevo nivel de bonificación
  - Está cerca de alcanzar el siguiente nivel
  - Recordatorios periódicos con su progreso

**Para PROMOS:**
- **Gestionar configuraciones** de períodos, tipos de bonos y vigencias desde Aldebaran.Web
- **Consultar bonos calculados** de cualquier distribuidor en cualquier período
- **Generar recomendaciones** de Nota de Crédito (NC) para aplicar en TOTUS
- **Reconciliar NC**: comparar lo calculado vs lo realmente aplicado en TOTUS
- **Resolver reclamaciones** con acceso completo al historial de cálculos
- **Generar reportes** de auditoría, discrepancias, consultas de distribuidores
- **Exportar datos** en Excel/PDF para análisis adicionales

### 2.1.3 Modalidades de Bonificación

El sistema calculará **tres tipos de bonos independientes** para cada distribuidor:

1. **Bonificación por Facturación** (Fuente: TOTUS)
   - Base: Valor total facturado sin impuestos en el período
   - Ajustes: Descuenta NC del período anterior, fletes, descuentos
   - Suma: Órdenes de Compra Especiales aprobadas manualmente por PROMOS
   - Ejemplo: Si facturó $100M ? Bono = $100M × 6% = $6M

2. **Bonificación por Pedido** (Fuente: Aldebaran - Órdenes)
   - Base: Valor total de órdenes pedidas en el período
   - Cálculo: Cantidad pedida × Precio del día del pedido
   - Incentivo: Motiva a distribuidores a pedir más volumen
   - Ejemplo: Si pidió por $50M ? Bono = $50M × 5% = $2.5M

3. **Bonificación por Entregado** (Fuente: Aldebaran - Entregas)
   - Base: Valor total de entregas confirmadas en el período
   - Cálculo: Cantidad realmente entregada × Precio del día del pedido
   - Incentivo: Motiva a distribuidores a confirmar lo que pidieron
   - Ejemplo: Si se entregaron $45M ? Bono = $45M × 4% = $1.8M

**Bono Total** = Bonificación por Facturación + Bonificación por Pedido + Bonificación por Entregado

Este bono total se aplicará como **Nota de Crédito (NC)** en el siguiente período en TOTUS.

### 2.1.4 Beneficios Esperados

**Para Distribuidores:**
- ? **Transparencia total**: Ven exactamente cómo se calcula su bono
- ? **Acceso 24/7**: Consultan su bonificación en cualquier momento sin contactar a PROMOS
- ? **Gamificación**: Saben cuánto les falta para el siguiente nivel
- ? **Notificaciones proactivas**: Reciben alertas cuando alcanzan hitos importantes
- ? **Historial completo**: Acceden a bonos de períodos anteriores

**Para PROMOS:**
- ? **Reducción 70% de tiempo administrativo**: Elimina cálculos y validaciones manuales
- ? **Precisión 100%**: Elimina errores humanos en cálculos
- ? **Auditoría completa**: Historial inmutable de cada cálculo para resolver reclamaciones
- ? **Control total**: Aprobaciones configurables para ingresos manuales
- ? **Reportería instantánea**: Exporta datos en Excel/PDF
- ? **Competitividad**: Ofrece experiencia automatizada similar a competencia

---

## 2.2 FUNCIONALIDADES DE ALTO NIVEL

A continuación se presentan las funcionalidades agrupadas por módulo y actor del sistema:

### 2.2.1 Módulo de Administración (Usuario PROMOS - Aldebaran.Web)

Este módulo permite al personal de PROMOS configurar los elementos clave del sistema de bonificación.

#### 2.2.1.1 Gestión de Períodos (Definición de Periodicidad)

**Descripción:**  
Crear y administrar las definiciones de periodicidad (templates) que establecen la configuración base temporal para calcular bonificaciones. Cada período es una plantilla reutilizable que define la duración en días.

**Funcionalidades:**
- Crear nueva definición de período especificando:
  - **Nombre único** (ej: "Quincena PROMOS", "Mes PROMOS")
  - **Tipo**: Mensual / Quincenal / Semanal / Diario / Custom
  - **Duración en días** (unidad de medida base):
    - Mensual: 30 días
    - Quincenal: 15 días
    - Semanal: 7 días
    - Diario: 1 día
    - Custom: N días (configurable)
  - **Descripción** (opcional)
  - **Estado**: Activo / Inactivo
- Modificar período (solo si no tiene instancias activas/cerradas)
- Ver listado de períodos configurados
- Ver cuántas **Instancias de Período Activas** se han generado
- **Generar Instancia de Período Activa** manualmente (especificando fecha inicio)
- Ver estado de instancias: Abierto / En Curso / Cerrado

**Conceptos Clave:**
- **Período (Template)**: Definición de periodicidad reutilizable (ej: "Quincena PROMOS" = 15 días)
- **Instancia de Período**: Período activo con fechas específicas (ej: "QUI-2026-01" del 01/01/2026 al 15/01/2026)
- La **Fecha Fin** de una instancia se calcula automáticamente: `Fecha Inicio + Duración`

**Restricciones:**
- No puede editar duración de un período que ya tiene instancias cerradas
- No puede crear dos períodos con el mismo nombre
- No puede eliminar período que tiene instancias generadas
- No puede ingresar duración en días ? 0

**[PLACEHOLDER: Mockup de pantalla "Gestión de Períodos - Listado de Templates"]**

**[PLACEHOLDER: Mockup de pantalla "Gestión de Períodos - Crear/Editar Template"]**

**[PLACEHOLDER: Mockup de pantalla "Gestión de Períodos - Generar Instancia Activa"]**

---

#### 2.2.1.2 Gestión de Tipos de Bono

**Descripción:**  
Definir los tipos de bonificación disponibles especificando en qué insumo se basa cada uno (Facturación, Pedido o Entregado) y asociarlos a un Período (template de periodicidad).

**Funcionalidades:**
- Crear nuevo tipo de bono:
  - Nombre único (ej: "Bono por Facturación")
  - Descripción (opcional)
  - **Base del Bono** (fuente de datos que genera el bono):
    - Facturación: Usa datos de TOTUS (valor facturado sin impuestos)
    - Pedido: Usa órdenes de Aldebaran (cantidad pedida × precio)
    - Entregado: Usa entregas de Aldebaran (cantidad entregada × precio)
  - **Período al cual aplica** (referencia a template de periodicidad de CU1)
    - Ejemplo: B1 "Bono por Facturación" ? asociado a P1 "Quincena PROMOS"
  - Estado: Activo / Inactivo
- Modificar tipo de bono (solo antes de tener vigencias activas)
- Ver listado de tipos disponibles
- Consultar cuántas vigencias usan cada tipo
- Ver vigencia actualmente ACTIVA para este tipo

**Restricciones:**
- No se puede eliminar un tipo si ya tiene bonos calculados
- No se puede cambiar "Base del Bono" de un tipo que tiene vigencias activas
- No se pueden tener dos tipos con mismo nombre asociados al mismo período

**[PLACEHOLDER: Mockup de pantalla "Gestión de Tipos de Bono"]**

---

#### 2.2.1.3 Gestión de Vigencias

**Descripción:**  
Configurar vigencias (rangos de valores de compra con porcentajes de bono asociados) para cada Tipo de Bonificación, con opción de parametrización por artículos/referencias específicos. **Solo UNA vigencia puede estar ACTIVA por Tipo de Bono** en un momento dado.

**Funcionalidades:**
- Crear nueva vigencia:
  - Nombre único (ej: "V3 - Bono Facturación Marzo 2026")
  - **Tipo de Bono asociado** (referencia a CU2)
    - Ejemplo: Vigencia B1V3 ? asociada a Tipo B1 "Bono por Facturación"
  - **Fecha de Activación** (desde cuándo aplica)
    - Al activarse, desactiva automáticamente la vigencia anterior del mismo Tipo de Bono
  - **Rangos de Valores** (Relación 1:N - Entidad hija de Vigencia):
    - **UNA vigencia tiene MÚLTIPLES rangos de valores**
    - Cada rango es un registro independiente con:
      - **Valor Mínimo** (numérico)
      - **Valor Máximo** (numérico)
      - **Porcentaje de Bono** (%)
    - Ejemplo: Una vigencia puede tener 3 rangos:
      - Rango 1: Mín=$1M, Máx=$5M, Bono=5%
      - Rango 2: Mín=$5M, Máx=$10M, Bono=6%
      - Rango 3: Mín=$10M, Máx=?, Bono=7%
  - **OPCIONAL**: Restricción por artículos/referencias:
    - Sin restricción (DEFAULT - aplica a TODOS los artículos)
    - Artículos específicos (TODAS sus referencias)
    - Artículos + Referencias específicas (combinación personalizada)
  - Moneda del bono (COP, USD, etc.)
  - Monto máximo de bono (tope configurable)
- Modificar vigencia (solo antes de que comience a usarse)
- Ver listado de vigencias (activas e históricas) por Tipo de Bono
- Copiar vigencia anterior como plantilla

**Conceptos Clave:**
- **Vigencia ACTIVA**: La más reciente en estado ACTIVO para un Tipo de Bono
- **Rangos de Valores (1:N)**: Cada vigencia tiene múltiples rangos (entidad hija). Cada rango define: Valor Mínimo, Valor Máximo, Porcentaje de Bono
- **Activación Automática**: Al activar una nueva vigencia, la anterior del mismo Tipo de Bono pasa a INACTIVA

**Restricciones:**
- **SOLO UNA vigencia ACTIVA por Tipo de Bono** en un momento dado
- No se puede editar una vigencia que ya está en uso
- No se puede crear vigencia con fecha de activación en el pasado
- Si se parametriza por artículos, debe validarse que existan en el sistema

**[PLACEHOLDER: Mockup de pantalla "Gestión de Vigencias - Crear/Editar"]**

**[PLACEHOLDER: Mockup de pantalla "Gestión de Vigencias - Tramos de Valor"]**

---

### 2.2.2 Módulo de Consulta de Bonificación (Distribuidor - Sitio Público)

Este módulo permite a los distribuidores acceder de forma segura a su información de bonificación sin necesidad de contactar a PROMOS.

#### 2.2.2.1 Autenticación por OTP (One Time Password)

**Descripción:**  
Proceso de autenticación seguro para que distribuidores accedan al sitio público desde la Página Promocional.

**Flujo de Usuario:**
1. Distribuidor hace clic en botón "Ver mi bonificación" en Página Promocional
2. Sistema redirecciona a Sitio Público Aldebaran
3. Distribuidor ingresa su número de documento (cédula)
4. Sistema valida que:
   - El documento existe en Aldebaran
   - Es tipo "DISTRIBUIDOR" (no otro tipo de cliente)
   - Tiene Email o Celular configurados
5. Sistema genera código OTP de 6 dígitos
6. Sistema envía OTP por SMS o Email según preferencia del distribuidor
7. Distribuidor ingresa código OTP recibido
8. Sistema valida OTP (máximo 3 intentos)
9. Si es válido, sistema crea sesión JWT válida por 8 horas
10. Distribuidor accede a su información de bonificación

**Reglas de Negocio:**
- OTP válido por 10 minutos (configurable)
- Máximo 3 intentos fallidos (después requiere solicitar nuevo OTP)
- No se puede reutilizar un OTP ya usado
- Token JWT expira después de 8 horas (configurable)
- Sistema registra todos los intentos de autenticación (exitosos/fallidos)

**Restricciones:**
- No se puede autenticar si el distribuidor no tiene Email ni Celular configurados
- OTP no se guarda en texto plano (encriptado)

**NOTA IMPORTANTE - Consideraciones de Costo y Proveedores:**

```
???????????????????????????????????????????????????????????????????
? OTP POR EMAIL (Recomendado - Sin Costos Adicionales):          ?
???????????????????????????????????????????????????????????????????
? ? Proceso 100% local de PROMOS                                 ?
? ? NO requiere suscripción mensual                              ?
? ? NO requiere integración con terceros                         ?
? ? Sin cargos por envío                                         ?
? ? Usa infraestructura de email existente de PROMOS             ?
?                                                                 ?
? Configuración:                                                  ?
? ?? SMTP de PROMOS (ya existente)                                ?
?                                                                 ?
???????????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????????
? OTP POR SMS (Opcional - Costos Adicionales):                    ?
???????????????????????????????????????????????????????????????????
? ??  REQUIERE suscripción mensual con proveedor externo          ?
? ??  Cargos por cada SMS enviado                                 ?
? ??  Integración con API de tercero (ej: Masivian)               ?
?                                                                 ?
? Opciones de Proveedor:                                          ?
? ?? Usar proveedor SMS existente de PROMOS (si ya existe)        ?
? ?? Contratar nuevo proveedor (ej: Masivian, Twilio, etc.)       ?
?                                                                 ?
? Costos Estimados (referencia):                                  ?
? ?? Suscripción mensual: Variable según proveedor                ?
? ?? Costo por SMS: Variable (ej: $50-$150 COP/SMS)               ?
? ?? Volumen estimado: Depende de cantidad de distribuidores      ?
?                                                                 ?
???????????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????????
? RECOMENDACIÓN PARA PROMOS:                                      ?
???????????????????????????????????????????????????????????????????
?                                                                 ?
? 1. FASE INICIAL (MVP):                                          ?
?    ?? Implementar SOLO OTP por Email                            ?
?    ?? Sin costos adicionales                                    ?
?    ?? Validar adopción de distribuidores                        ?
?                                                                 ?
? 2. FASE POSTERIOR (Opcional):                                   ?
?    ?? Si distribuidores requieren SMS                           ?
?    ?? Evaluar proveedor existente de PROMOS                     ?
?    ?? Calcular ROI vs beneficio                                 ?
?                                                                 ?
? 3. CONFIGURACIÓN FLEXIBLE:                                      ?
?    ?? Sistema debe soportar ambas opciones                      ?
?    ?? Activación de SMS bajo demanda                            ?
?    ?? Prioridad: Email (default), SMS (opcional)                ?
?                                                                 ?
???????????????????????????????????????????????????????????????????
```

**[PLACEHOLDER: Mockup de pantalla "Login - Ingreso de Documento"]**

**[PLACEHOLDER: Mockup de pantalla "Login - Ingreso de OTP"]**

---

#### 2.2.2.2 Consulta de Bonificación - Período Actual

**Descripción:**  
Consulta en tiempo real (menos de 500ms) del bono acumulado durante el período activo, con gamificación y notificaciones automáticas.

**Flujo de Usuario:**
1. Distribuidor (autenticado) accede al dashboard de bonificación
2. Sistema calcula dinámicamente:
   - Bono por Facturación (consulta TOTUS en tiempo real)
   - Bono por Pedido (suma órdenes del período)
   - Bono por Entregado (suma entregas confirmadas del período)
   - Gamificación: Cuánto falta para siguiente nivel
3. Sistema muestra:
   - Bono total acumulado
   - Desglose por tipo de bono
   - Período actual y días transcurridos
   - Gamificación visual (barra de progreso, porcentaje completado)
   - Monto faltante para siguiente nivel
4. Distribuidor puede:
   - Consultar múltiples veces (cada consulta recalcula en tiempo real)
   - **Ver detalles de cada tipo de bono**:
     - **Facturación**: Solo totales consolidados (limitación de TOTUS)
     - **Pedido**: Desglose completo por orden y artículo
     - **Entregado**: Desglose completo por entrega y artículo
   - Configurar preferencias de notificación
   - Descargar comprobante/resumen (PDF)

**Información Mostrada:**
- **Bono por Facturación** (Solo totales consolidados):
  - Base: Valor total facturado sin impuestos
  - Ajustes: NC anterior, Fletes, Descuentos (totalizados)
  - OC Especiales: Monto aprobado (si aplica)
  - Porcentaje aplicado (según vigencia)
  - Valor calculado del bono
  - ?? **NOTA**: TOTUS solo retorna totales agregados, NO desglose detallado
- **Bono por Pedido** (Con desglose detallado):
  - Base: Valor total de órdenes pedidas
  - **Desglose disponible**:
    - Por cada orden: Número, Fecha, Artículos, Cantidades, Precios
    - Por artículo/referencia: Cantidad pedida × Precio
  - Porcentaje aplicado (según vigencia)
  - Valor calculado del bono
- **Bono por Entregado** (Con desglose detallado):
  - Base: Valor total de entregas confirmadas
  - **Desglose disponible**:
    - Por cada entrega: Número, Fecha, Artículos, Cantidades, Precios
    - Por artículo/referencia: Cantidad entregada × Precio
  - Porcentaje aplicado (según vigencia)
  - Valor calculado del bono
- **Gamificación**:
  - Nivel actual (tramo de bonificación)
  - Próximo nivel (tramo siguiente)
  - Porcentaje completado hacia próximo nivel
  - Monto faltante para alcanzar próximo nivel

**Notificaciones Automáticas Integradas:**

**1. Notificación: Alcanzó Nuevo Nivel**
- **Cuándo:** Se dispara automáticamente cuando distribuidor sube de tramo en cualquier tipo de bono
- **Canales:** SMS + Email (según preferencias del distribuidor)
- **Contenido:** "¡Felicidades! Alcanzaste un nuevo nivel de bonificación. Tu bono por [Tipo] es ahora [%]% sobre [base]. ¡Sigue adelante!"
- **Frecuencia:** Máximo 1 notificación por distribuidor por tipo de bono en 24 horas

**2. Notificación: Está Cerca del Siguiente Nivel**
- **Cuándo:** Job diario (default: 6 AM) verifica si acumulado ? X% del siguiente tramo
- **Canales:** SMS + Email (según preferencias del distribuidor)
- **Contenido:** "¡Casi lo logras! Estás a poco de alcanzar el siguiente nivel de bonificación en [Tipo de Bono]. Necesitas $[Monto Faltante] más. ¡Adelante!"
- **Configuración Admin:** Umbral % (50-95%, default 80%), Frecuencia, Horario, Activo/Inactivo
- **Frecuencia:** Máximo 1 notificación en 24 horas (anti-spam)

**3. Recordatorio Periódico: Progreso de Bonificación**
- **Cuándo:** Job semanal (default: Lunes 8 AM) o configurada por Admin
- **Canales:** SMS (breve) + Email (detallado, según preferencias)
- **Contenido Email:** Resumen completo de 3 bonos con acumulado, tramo, % completado, falta para siguiente
- **Configuración Admin:** Frecuencia (Diaria/Semanal/Bisemanal/Mensual), Día/Hora, Activo/Inactivo
- **Preferencias Distribuidor:** Puede desuscribirse, elegir canal preferido, establecer horario

**Reglas de Negocio:**
- Cálculo es dinámico (se ejecuta cada consulta, NO precalculado)
- SLA: 500ms máximo (incluida consulta a TOTUS)
- **Cambios en CUALQUIER fuente se reflejan en próxima consulta**:
  - Nueva factura en TOTUS ? Actualiza bono por Facturación
  - Nueva orden en Aldebaran ? Actualiza bono por Pedido
  - Nueva entrega confirmada en Aldebaran ? Actualiza bono por Entregado
  - Nueva OC Especial aprobada ? Actualiza bono por Facturación
- Distribuidor SOLO ve su información (aislamiento total)
- No hay cache de resultados de bonos (siempre recalcula)

**Restricciones:**
- Página de solo lectura (sin ingreso de datos)
- No puede ver información de otros distribuidores
- Token debe estar válido (no expirado)
- Notificaciones respetan siempre preferencias del distribuidor

**[PLACEHOLDER: Mockup de pantalla "Dashboard Bonificación - Período Actual"]**

**[PLACEHOLDER: Mockup de pantalla "Gamificación - Progreso hacia Siguiente Nivel"]**

---

#### 2.2.2.3 Consulta de Histórico de Bonos - Períodos Anteriores

**Descripción:**  
Consulta de bonos finales congelados (inmutables) de períodos cerrados anteriores, con estado de aplicación de NC.

**Flujo de Usuario:**
1. Distribuidor (autenticado) selecciona "Ver histórico"
2. Sistema muestra dropdown con últimos N períodos cerrados (default: 12)
3. Distribuidor selecciona período anterior
4. Sistema retorna (sin cálculos - datos congelados):
   - Bono final asignado (inmutable)
   - Desglose por tipo de bono
   - Estado de aplicación de NC: Aplicada / Pendiente / Rechazada
   - Fecha en que se aplicó la NC (si aplica)
   - Referencia/ID de la NC
   - OC Especiales incluidas en ese período (si aplica)
5. Distribuidor puede:
   - Navegar entre períodos anteriores
   - Descargar comprobante/resumen (PDF)
   - Ver estado de cada bono

**Información Mostrada:**
- **Período**: Nombre, Fecha inicio/fin, Estado: Cerrado
- **Bono Total**: Valor congelado (no recalcula)
- **Desglose**: Por Facturación, Pedido, Entregado
- **Aplicación NC**:
  - Estado: Aplicada (?) / Pendiente (?) / Rechazada (?)
  - Fecha de aplicación
  - Número de referencia de NC en TOTUS

**Reglas de Negocio:**
- Bonos mostrados son INMUTABLES (congelados al cierre del período)
- Solo puede ver últimos N períodos cerrados (N configurable por Admin)
- No puede ver períodos activos (en curso) - solo cerrados

**Restricciones:**
- Página de solo lectura (sin ingreso de datos)
- No puede ver información de otros distribuidores
- No puede acceder a datos administrativos (Aldebaran.Web)

**[PLACEHOLDER: Mockup de pantalla "Histórico de Bonos - Listado de Períodos"]**

**[PLACEHOLDER: Mockup de pantalla "Histórico de Bonos - Detalle de Período Cerrado"]**

---

### 2.2.3 Módulo de Integraciones (Automático - Backend)

Este módulo gestiona las integraciones con sistemas externos y procesos automáticos sin intervención humana.

#### 2.2.3.1 Obtener Facturación de TOTUS

**Descripción:**  
Consulta directa a la base de datos TOTUS (local en PROMOS) mediante procedimiento almacenado para obtener valor facturado real de cada distribuidor.

**NOTA TÉCNICA:** La consulta NO es a un sistema externo via API, sino a la **base de datos TOTUS local** mediante **Stored Procedure**. Esto garantiza latencia mínima (< 50ms típicamente).

**?? NOTA CRÍTICA - DEPENDENCIA EXTERNA:**

```
???????????????????????????????????????????????????????????????????
? EL SP DE TOTUS ES DESARROLLADO POR OTRA FÁBRICA DE SOFTWARE    ?
???????????????????????????????????????????????????????????????????
?                                                                 ?
? ? ALDEBARAN (Este proyecto) NO desarrolla el SP:               ?
?    ?? Solo CONSUME el SP ya existente                          ?
?    ?? NO puede modificar la lógica del SP                      ?
?    ?? NO puede optimizar el SP                                 ?
?    ?? Depende 100% de la otra fábrica                          ?
?                                                                 ?
? ? OTRA FÁBRICA (Externa) es responsable de:                    ?
?    ?? Desarrollo del SP                                        ?
?    ?? Optimización y performance                               ?
?    ?? Garantizar SLA < 50ms                                    ?
?    ?? Mantenimiento y soporte                                  ?
?                                                                 ?
? ?? COORDINACIÓN REQUERIDA:                                      ?
?    ?? Especificar contrato del SP (parámetros/retorno)        ?
?    ?? Acordar SLA de respuesta                                 ?
?    ?? Definir manejo de errores                                ?
?    ?? Establecer canal de soporte                              ?
?                                                                 ?
???????????????????????????????????????????????????????????????????
```

**Flujo Técnico:**
1. Sistema ejecuta SP desarrollado por OTRA FÁBRICA en BD TOTUS (local)
2. Parámetros enviados:
   - Tipo documento: "FAC" (Factura)
   - Número documento: Cédula del distribuidor
   - Fecha inicio: Primer día del período
   - Fecha fin: Último día del período o hoy
   - Lista de artículos (OPCIONAL - si vigencia está parametrizada)
   - Mapa de referencias por artículo (OPCIONAL)
3. TOTUS retorna:
   - ValorTotalFacturadoSinImpuestos (obligatorio)
   - TotalNotasCredito (obligatorio)
   - TotalFletes (opcional)
   - TotalDescuentos (opcional)
4. ?? **IMPORTANTE - SIN CACHE**: Sistema **NO cachea** el resultado para garantizar información 100% actualizada en tiempo real
5. Sistema registra auditoría de cada consulta

**Reglas de Negocio:**
- SLA: 500ms máximo (consulta a BD TOTUS típicamente < 50ms)
- **Sin cache**: Cada consulta ejecuta SP directamente en BD TOTUS local
- Si BD TOTUS no responde ? Fallback a último valor conocido + Alerta a Admin + Banner de advertencia al distribuidor
- Si vigencia está parametrizada por artículos ? Filtra facturación solo de esos artículos
- Valores negativos no se permiten (validación)

**Manejo de Errores:**
- Si BD TOTUS no disponible: Usa último valor conocido (fallback) + Alerta inmediata a Admin + Banner de advertencia al distribuidor
- Si timeout de SP: Reintentos configurables (default: 3 intentos, 500ms espera entre intentos)
- Si error de SQL: Registra excepción completa + Alerta a Admin
- Registra todos los fallos en log de auditoría
- Muestra al distribuidor: "?? Información de facturación desactualizada. Última actualización: [fecha/hora]"

**Restricciones:**
- Solo lectura (no modifica valores en TOTUS)
- **Sin cache de resultados** (siempre consulta en tiempo real)
- Fallback solo en caso de error de TOTUS (con advertencia visible al distribuidor)

**JUSTIFICACIÓN TÉCNICA - Sin Cache:**

```
DECISIÓN: NO usar cache de consultas TOTUS

RAZONES:
1. Consultas de distribuidores NO son constantes (patrón esporádico)
   ?? No todos consultan a la misma hora
   ?? No todos consultan todos los días
   ?? Patrón de uso es impredecible

2. Promesa de información en tiempo real es CRÍTICA
   ?? Cache rompe la promesa de "información actualizada"
   ?? Distribuidores confían en los datos mostrados
   ?? Nueva factura debe reflejarse inmediatamente

3. BD TOTUS (local) debe manejar su carga operativa normal
   ?? Consultas de bonificación son parte de operación estándar
   ?? SP debe estar optimizado con índices apropiados
   ?? Connection pooling de ADO.NET maneja concurrencia eficientemente

4. Connection pooling y optimizaciones de BD son suficientes
   ?? Reutilización eficiente de conexiones a BD TOTUS (ADO.NET pool)
   ?? Timeouts configurables (3 reintentos, 500ms c/u)
   ?? SLA de 500ms es FÁCILMENTE alcanzable (consulta BD < 50ms típicamente)
```

**VALIDACIÓN REQUERIDA CON PROMOS Y OTRA FÁBRICA:**

**Con OTRA FÁBRICA (Responsable del SP):**
- ? Especificar contrato del SP:
  - Nombre del SP: `dbo.sp_ObtenerFacturacionDistribuidor`
  - Parámetros de entrada: @NumeroDocumento, @FechaInicio, @FechaFin, @ListaArticulos (opcional)
  - Campos de retorno: ValorFacturadoSinImpuestos, TotalNotasCredito, TotalFletes, TotalDescuentos
- ? Acordar SLA de respuesta: < 50ms (95% del tiempo)
- ? Confirmar que SP está optimizado (índices, estadísticas)
- ? Definir manejo de errores (códigos de error, mensajes)
- ? Establecer canal de soporte (Slack, Email, Tickets)
- ? Definir proceso de cambios al SP (versionamiento)

**Con PROMOS (Cliente):**
- ? Validar que connection string tiene pooling habilitado (Min Pool Size, Max Pool Size)
- ? Confirmar que BD TOTUS puede manejar N consultas concurrentes
- ? Definir plan de contingencia si BD TOTUS no está disponible
- ? Establecer proceso de escalamiento si SP no cumple SLA

---

#### 2.2.3.2 Cargar Lista de Precios (Diario Automático)

**Descripción:**  
Descarga automática diaria de la lista de precios desde Página Promocional para calcular bonos correctamente.

**Flujo Técnico:**
1. Job programado (default: 6 AM diariamente)
2. Sistema descarga archivo de precios desde:
   - URL/SFTP/API de Página Promocional (configurable)
   - Credenciales configurables
3. Valida estructura y datos:
   - Estructura: Referencia, Precio Unitario, Descuento Distribuidor
   - Validación: Precios > 0, Descuentos 0-100%
4. Reemplaza tabla PreciosDistribuidor (actual)
5. Copia histórico en PreciosDistribuidorHistorico (con fecha)
6. Ejecuta limpieza automática (borra precios más antiguos que período de retención - default: 120 días)
7. Registra auditoría completa
8. Envía alerta si descarga falla

**Reglas de Negocio:**
- Horario configurable (default: 6 AM)
- Política de reintentos: 3 intentos, 5 minutos espera entre intentos
- Política de retención: 120 días (configurable)
- Si descarga falla ? Usa precios del día anterior (fallback)

**Manejo de Errores:**
- Si descarga falla después de 3 intentos ? Alerta a Admin + Usa precios anteriores
- No puede dejar el sistema sin precios
- No acepta archivos con estructura incorrecta

**Restricciones:**
- No puede ejecutar limpieza de precios activos (en uso)
- No puede eliminar precios de períodos no cerrados
- No puede sobreescribir precios durante horas de operación críticas

---

#### 2.2.3.3 Cierre Automático de Instancia de Período

**Descripción:**  
Proceso automático que se ejecuta el último día de cada **Instancia de Período Activa** para congelar cálculos y generar recomendaciones de NC.

**Flujo Técnico:**
1. Job programado (último día de la instancia, hora configurable - default: 23:59:59)
2. Sistema verifica que es el último día de la instancia de período activa
3. Para cada distribuidor con actividad en la instancia de período:
   - Calcula bono FINAL de la instancia completa (no dinámico)
   - Almacena FOTO congelada en HistorialBono (estado: CALCULADO)
   - Genera recomendación de NC (estado: RECOMENDADA)
4. Marca **Instancia de Período** como CERRADO
5. Publica evento "InstanciaPeriodoCerrado" (RabbitMQ)
6. Notifica a Usuario PROMOS (Email configurable)
7. Registra auditoría completa

**Información Generada:**
- **FOTO Congelada**: Bono final inmutable de cada distribuidor
- **Recomendación de NC**: Valor sugerido para aplicar en TOTUS
- **Estado de la Instancia de Período**: CERRADO (no permite modificaciones)

**Reglas de Negocio:**
- Horario configurable (default: 23:59:59 del último día)
- FOTO es inmutable post-cierre (no se puede recalcular)
- NO aplica NC automáticamente en TOTUS (solo recomienda)
- Usuario PROMOS es responsable de aplicar NC en TOTUS

**Restricciones:**
- No puede cerrar instancia de período que ya está cerrada
- No puede modificar datos después del cierre (inmutabilidad)

---

### 2.2.4 Módulo de Operaciones Manuales (Usuario PROMOS - Aldebaran.Web)

Este módulo permite al personal de PROMOS realizar operaciones manuales críticas.

#### 2.2.4.1 Consultar Bono Actual Dinámico (Admin)

**Descripción:**  
Consulta del bono actual de cualquier distribuidor en período activo para preparar recomendaciones de NC.

**Flujo de Usuario:**
1. Usuario PROMOS ingresa a Aldebaran.Web (autenticado)
2. Busca distribuidor por documento o nombre
3. Selecciona período (actual o anterior)
4. Sistema muestra:
   - Bono por Facturación (desglosado)
   - Bono por Pedido (desglosado)
   - Bono por Entregado (desglosado)
   - Total bonificación
   - Vigencia aplicada
   - Tramos usados
   - NC anterior descontada
   - Precios usados
   - Historial de cambios durante el período
   - Auditoría completa (quién, qué, cuándo)
5. Usuario puede:
   - Expandir detalles de cada bono
   - Ver historial de cálculos del período
   - Generar recomendación de NC
   - Exportar información (Excel/PDF)

**Reglas de Negocio:**
- Solo acceso a Usuario PROMOS (autenticado internamente)
- Datos mostrados son cálculos internos (no vinculantes hasta aplicar en TOTUS)

**Restricciones:**
- Solo lectura (no modifica valores calculados)
- No puede acceder a distribuidores de otros segmentos (si hay restricción de rol)

**[PLACEHOLDER: Mockup de pantalla "Consulta Admin - Selección de Distribuidor"]**

**[PLACEHOLDER: Mockup de pantalla "Consulta Admin - Detalle de Bonos"]**

---

#### 2.2.4.2 Ingreso Manual de Órdenes de Compra Especiales

**Descripción:**  
Permite ingresar manualmente órdenes de compra especiales (OC) que se sumarán al bono por facturación, en modalidad unitaria o masiva (CSV).

**Modalidad UNITARIO:**

**Flujo de Usuario:**
1. Usuario PROMOS selecciona "Ingresar OC Especial - Unitario"
2. Completa formulario:
   - Distribuidor (búsqueda/selección)
   - Período aplicable
   - Monto de la OC especial
   - Descripción/Motivo
   - Fecha de registro
3. Sistema valida datos ingresados
4. Usuario confirma
5. Sistema registra OC con estado: PENDIENTE
6. Sistema envía para aprobación (si configurado)
7. Una vez APROBADA, se suma al bono por facturación

**Modalidad MASIVO (CSV):**

**Flujo de Usuario:**
1. Usuario PROMOS selecciona "Carga Masiva OC Especiales"
2. Descarga plantilla CSV
3. Completa plantilla con múltiples distribuidores:
   - Documento distribuidor, Período, Monto OC, Descripción
4. Carga archivo CSV
5. Sistema valida estructura y datos:
   - Distribuidores existen
   - Períodos válidos
   - Montos numéricos positivos
6. Muestra resumen de registros válidos/inválidos
7. Usuario confirma carga
8. Sistema registra todas las OC con estado: PENDIENTE
9. Sistema envía para aprobación (si configurado)

**Reglas de Negocio:**
- Solo OC con estado APROBADA se suman al bono
- Si OC está PENDIENTE o RECHAZADA, NO se incluye en cálculo
- Sistema registra auditoría: quién ingresó, cuándo, monto, estado

**Restricciones:**
- No puede ingresar montos negativos o cero
- No puede modificar OC después de aprobada

**[PLACEHOLDER: Mockup de pantalla "Ingreso OC Especial - Unitario"]**

**[PLACEHOLDER: Mockup de pantalla "Carga Masiva OC - Resumen de Validación"]**

---

#### 2.2.4.3 Reconciliación Manual de Nota de Crédito

**Descripción:**  
Permite registrar el valor REAL de la NC que se aplicó en TOTUS vs la NC calculada por el sistema, para usar ese valor en cálculos del siguiente período.

**Modalidad UNITARIO:**

**Flujo de Usuario:**
1. Usuario PROMOS selecciona "Reconciliación NC - Unitario"
2. Selecciona:
   - Distribuidor
   - Período a conciliar (período anterior cerrado)
3. Sistema muestra:
   - NC Calculada (del cierre automático)
   - Campo para ingresar NC Real (lo que TOTUS aplicó)
   - Fecha de confirmación en TOTUS (opcional)
   - Motivo si hay discrepancia (opcional)
4. Usuario ingresa NC Real
5. Sistema compara NC Calculada vs NC Real:
   - Si hay diferencia > umbral (ej: 2%) ? Alerta
6. Usuario confirma
7. Sistema registra:
   - HistorialBono.BonoAplicado = NC Real
   - HistorialBono.Estado = CONCILIADO o CONCILIADO CON DISCREPANCIA
   - HistorialBono.Diferencia = NC Real - NC Calculada
8. Próximos cálculos usan NC Real (no la calculada)

**Modalidad MASIVO (CSV):**

**Flujo de Usuario:**
1. Usuario PROMOS selecciona "Reconciliación NC - Masivo"
2. Descarga plantilla CSV
3. Completa plantilla con múltiples distribuidores:
   - Documento distribuidor, Período, NC Real, Fecha confirmación, Motivo discrepancia
4. Carga archivo CSV
5. Sistema valida estructura y datos
6. Muestra resumen de registros válidos/inválidos
7. Usuario confirma carga
8. Sistema procesa reconciliaciones en lote

**Reglas de Negocio:**
- Solo puede conciliar períodos cerrados
- NC Real debe ser numérica y positiva
- Si diferencia > umbral (configurable) ? Sistema alerta
- Cambio de estado es irreversible (PENDIENTE ? CONCILIADO)

**Restricciones:**
- No puede modificar valores de la FOTO congelada
- No puede conciliar período no cerrado
- No puede ingresar valores negativos o cero

**[PLACEHOLDER: Mockup de pantalla "Reconciliación NC - Unitario"]**

**[PLACEHOLDER: Mockup de pantalla "Reconciliación NC - Masivo - Resumen"]**

---

### 2.2.5 Módulo de Reportería (Usuario PROMOS - Aldebaran.Web)

Este módulo genera reportes de auditoría, análisis y exportación de datos.

#### 2.2.5.1 Reporte: Bonos Calculados vs Bonos Aplicados

**Descripción:**  
Muestra comparación entre bonos calculados por el sistema y bonos realmente aplicados en TOTUS.

**Información Incluida:**
- Período seleccionado
- Listado de distribuidores con:
  - Documento / Nombre
  - Bono Calculado (del cierre automático)
  - Bono Aplicado (de reconciliación)
  - Diferencia (Aplicado - Calculado)
  - % Diferencia
  - Estado: Sin Discrepancia / Con Discrepancia
- Totales generales
- Filtros: Por período, por rango de diferencia, por estado

**Acciones:**
- Exportar a Excel/PDF
- Ver detalle de cada distribuidor
- Filtrar por diferentes criterios

**[PLACEHOLDER: Mockup de pantalla "Reporte Bonos Calculados vs Aplicados"]**

---

#### 2.2.5.2 Reporte: Distribuidores que Consultaron Bonos

**Descripción:**  
Auditoría de consultas realizadas por distribuidores en el sitio público.

**Información Incluida:**
- Período seleccionado
- Listado con:
  - Documento / Nombre distribuidor
  - Fecha y hora de consulta
  - Bono mostrado en esa consulta (por tipo)
  - Desde dónde consultó (IP/Dispositivo)
  - Tiempo de sesión
- Totales: Cantidad de consultas por distribuidor, por día
- Filtros: Por período, por fecha, por rango de bonos, por distribuidor

**Acciones:**
- Exportar a Excel/PDF
- Ver histórico completo de consultas de un distribuidor

**[PLACEHOLDER: Mockup de pantalla "Reporte Distribuidores que Consultaron"]**

---

#### 2.2.5.3 Reporte: Discrepancias de NC (Calculada vs Real)

**Descripción:**  
Muestra todas las diferencias encontradas entre NC calculada y NC real aplicada.

**Información Incluida:**
- Período seleccionado
- Listado con:
  - Documento / Nombre distribuidor
  - NC Calculada
  - NC Real
  - Diferencia (Real - Calculada)
  - % Diferencia
  - Motivo de discrepancia (ingresado por usuario)
  - Estado: Resuelta / Pendiente
  - Fecha de reconciliación
- Totales y estadísticas
- Filtros: Por período, por rango de diferencia, por estado

**Acciones:**
- Exportar a Excel/PDF
- Marcar discrepancia como Resuelta
- Agregar notas internas

**[PLACEHOLDER: Mockup de pantalla "Reporte Discrepancias de NC"]**

---

#### 2.2.5.4 Reporte: Auditoría de Acciones del Usuario PROMOS

**Descripción:**  
Historial completo de todas las acciones realizadas por usuarios PROMOS en el sistema.

**Información Incluida:**
- Usuario PROMOS
- Fecha y hora
- Acción realizada:
  - Creación de período/tipo/vigencia
  - Ingreso manual de OC Especial
  - Reconciliación de NC
  - Aprobación/Rechazo de ingresos manuales
  - Modificaciones en configuración
- Resultado: Exitoso / Fallido
- Detalles adicionales (valores ingresados, cambios realizados)
- Filtros: Por usuario, por fecha, por tipo de acción

**Acciones:**
- Exportar a Excel/PDF
- Ver detalles completos de cada acción

**[PLACEHOLDER: Mockup de pantalla "Reporte Auditoría Usuario PROMOS"]**

---

#### 2.2.5.5 Reporte: Precios y Vigencias Usados en Período

**Descripción:**  
Detalle de qué lista de precios y qué vigencias se usaron en los cálculos de cada período.

**Información Incluida:**
- Período seleccionado
- Vigencias activas en ese período:
  - Nombre vigencia
  - Tipo de bono
  - Tramos configurados
  - Fecha de inicio
- Lista de precios usada:
  - Fecha de carga
  - Cantidad de referencias
  - Rango de precios (mín/máx)
- Filtros: Por período, por tipo de bono

**Acciones:**
- Exportar a Excel/PDF
- Descargar lista de precios completa del período

**[PLACEHOLDER: Mockup de pantalla "Reporte Precios y Vigencias Usados"]**

---

#### 2.2.5.6 Reporte: Ingresos Manuales Aplicados

**Descripción:**  
Auditoría de todos los ingresos manuales realizados (OC Especiales + Reconciliaciones).

**Información Incluida:**
- Período seleccionado
- Listado con:
  - Tipo: OC Especial / Reconciliación NC
  - Documento / Nombre distribuidor
  - Monto ingresado
  - Usuario PROMOS que ingresó
  - Fecha de ingreso
  - Estado: Pendiente / Aprobada / Rechazada
  - Motivo/Descripción
- Totales por tipo
- Filtros: Por período, por tipo, por estado, por usuario

**Acciones:**
- Exportar a Excel/PDF
- Ver detalle completo de cada ingreso manual

**[PLACEHOLDER: Mockup de pantalla "Reporte Ingresos Manuales Aplicados"]**

---

### 2.2.6 Módulo de Resolución de Reclamaciones (Usuario PROMOS - Aldebaran.Web)

#### 2.2.6.1 Resolver Reclamación de Distribuidor

**Descripción:**  
Acceso completo al historial de bonos mostrados al distribuidor durante un período para investigar y resolver reclamaciones.

**Escenarios de Reclamación:**
1. Distribuidor reclama que vio diferentes valores en diferentes consultas
2. Distribuidor reclama que el bono final no corresponde con sus cálculos
3. Distribuidor reclama cambios no justificados entre consultas

**Flujo de Usuario:**
1. Usuario PROMOS ingresa a "Resolver Reclamación"
2. Busca distribuidor y selecciona período
3. Sistema muestra:
   - **Historial de Consultas del Distribuidor**:
     - Fecha y hora de cada consulta
     - Bono mostrado en cada consulta (desglosado)
     - Acumulado de insumos en ese momento (Facturación, Pedidos, Entregas, OC)
   - **FOTO Final del Período**:
     - Bono calculado al cierre (inmutable)
     - Desglose completo
   - **Análisis de Cambios**:
     - Qué cambió entre cada par de consultas
     - Por qué cambió el bono
     - Identificar ingresos manuales que afectaron cálculo
   - **Auditoría de Ingresos Manuales**:
     - OC Especiales ingresadas (quién, cuándo, monto, estado)
     - Reconciliaciones de NC realizadas
   - **Detalles del Cálculo**:
     - Vigencia aplicada
     - Tramos usados
     - NC anterior descontada
     - Precios usados
4. Usuario puede:
   - Expandir cada consulta para ver detalles
   - Ver cambios entre consultas y causas
   - Generar reporte de investigación
   - Exportar auditoría completa (PDF/Excel)
   - Crear nota interna con conclusiones

**Reglas de Negocio:**
- Solo lectura (no modifica datos históricos)
- No puede editar valores congelados en HistorialBono
- Si descubre errores en cálculo ? Requiere apertura de ticket a soporte técnico

**Restricciones:**
- No puede recalcular períodos ya cerrados
- No puede eliminar o modificar auditoría

**[PLACEHOLDER: Mockup de pantalla "Resolver Reclamación - Historial de Consultas"]**

**[PLACEHOLDER: Mockup de pantalla "Resolver Reclamación - Análisis de Cambios"]**

---

## 2.3 CONSIDERACIONES TÉCNICAS Y RESTRICCIONES

### 2.3.1 Seguridad

**Autenticación:**
- OTP de un solo uso (6 dígitos, válido 10 minutos - configurable)
- Token JWT válido por 8 horas (configurable)
- Máximo 3 intentos fallidos de OTP (requiere nuevo OTP)
- OTP y token en tránsito encriptados (HTTPS)
- Datos sensibles en BD encriptados

**Autorización:**
- Distribuidor solo ve su información (aislamiento total)
- Usuario PROMOS accede solo según permisos de rol
- Administrador tiene permisos completos

**Auditoría:**
- Logs de acceso (quién, cuándo, desde dónde)
- Logs de OTP enviado/validado
- Logs de todas las acciones críticas (cálculos, reconciliaciones, ingresos manuales)

### 2.3.2 Rendimiento

**SLA Consultas:**
- Consulta de bono período actual: 500ms máximo (incluida consulta TOTUS)
- Consulta de histórico: 200ms máximo (solo lectura de BD)

**SLA Procesos Automáticos:**
- Carga de precios: 5 minutos máximo
- Cierre de período: 5 minutos máximo por período

**Disponibilidad:**
- 99% de uptime (objetivo)
- Ventanas de mantenimiento programadas (notificadas con 48h anticipación)

### 2.3.3 Escalabilidad

**Capacidad:**
- Soporta miles de distribuidores concurrentes
- Soporta múltiples períodos simultáneos
- Soporta múltiples tipos de bonos por período

**Optimizaciones:**
- **Sin cache de consultas TOTUS** (garantiza información actualizada en tiempo real)
- Indexación de BD para consultas rápidas
- Paginación de reportes con grandes volúmenes de datos
- Connection pooling para consultas a TOTUS

### 2.3.4 Auditoría e Inmutabilidad

**Post-Cierre:**
- FOTO de bonos es inmutable (no se puede recalcular)
- Historial de consultas de distribuidores es inmutable
- Auditoría de acciones no se puede eliminar

**Trazabilidad:**
- Cada cálculo registra: Vigencia usada, Precios usados, NC anterior, Fecha/Hora
- Cada reconciliación registra: Quién, Qué, Cuándo, Motivo discrepancia
- Cada ingreso manual registra: Usuario, Fecha, Monto, Estado, Aprobaciones

---

## 2.4 GLOSARIO DE TÉRMINOS

### 2.4.1 Términos de Negocio

**Bonificación / Bono:**  
Incentivo económico que PROMOS otorga a distribuidores basado en su desempeño de compras en un período determinado.

**Distribuidor:**  
Cliente de PROMOS que compra productos para revenderlos. Beneficiario de las bonificaciones.

**Nota de Crédito (NC):**  
Valor del bono aplicado como descuento en TOTUS para el siguiente período. Representa una disminución en la deuda del distribuidor.

**Período:**  
**Definición de periodicidad** (template reutilizable) que establece la duración en días para calcular bonificaciones. Ejemplos: "Quincena PROMOS" = 15 días, "Mes PROMOS" = 30 días. NO tiene fechas específicas.

**Instancia de Período:**  
Ejecución específica de un Período (template) con fechas concretas. Se genera manual o automáticamente especificando una fecha inicio, y la fecha fin se calcula como: `Fecha Inicio + Duración del Template`. Ejemplo: "QUI-2026-01" del 01/01/2026 al 15/01/2026.

**Tipo de Bono:**
Modalidad de bonificación asociada a un Período (template) que define la **base del bono** (fuente de datos que lo genera): Facturación (TOTUS), Pedido (Aldebaran), Entregado (Aldebaran). Ejemplo: B1 "Bono por Facturación" con base en datos de TOTUS, asociado a P1 "Quincena PROMOS".

**Vigencia:**  
Configuración asociada a un Tipo de Bono que define rangos de valores de compra con porcentajes de bono. Solo UNA vigencia puede estar ACTIVA por Tipo de Bono. Al activar una nueva, la anterior pasa a INACTIVA automáticamente. Ejemplo: B1V3 activa desde 2026-03-01.

**Rango de Valores:**  
Entidad hija de Vigencia (relación 1:N) que define mínimo, máximo y porcentaje de bono. Cada rango es un registro independiente con: Valor Mínimo (numérico), Valor Máximo (numérico), Porcentaje de Bono (%). Una vigencia puede tener múltiples rangos. Ejemplo: Rango 1 con Mín=$1M, Máx=$5M, Bono=5%.

**Gamificación:**
Visualización de cuánto falta para alcanzar el siguiente nivel/tramo de bonificación. Motiva al distribuidor a comprar más.

**OC Especial (Orden de Compra Especial):**  
Monto ingresado manualmente por PROMOS que se suma al bono por facturación. Requiere aprobación.

**Reconciliación:**  
Proceso de comparar NC calculada vs NC realmente aplicada en TOTUS, e ingresar el valor real para usar en próximos cálculos.

**FOTO:**  
Registro inmutable del bono calculado al cierre del período. No se puede recalcular después del cierre.

### 2.4.2 Términos Técnicos

**OTP (One Time Password):**  
Código de un solo uso enviado por SMS/Email para autenticar al distribuidor de forma segura.

**JWT (JSON Web Token):**  
Token de sesión válido por 8 horas que permite al distribuidor acceder al sitio público sin re-autenticarse.

**SLA (Service Level Agreement):**  
Acuerdo de nivel de servicio. Define tiempos máximos de respuesta (ej: 500ms).

**Fallback:**  
Mecanismo de respaldo cuando una integración falla (ej: si TOTUS no responde, usa valor cacheado anterior).

**Job (Scheduled Job):**  
Proceso automático programado que se ejecuta en horarios específicos sin intervención humana.

**Caché:**  
Almacenamiento temporal de datos para mejorar rendimiento (ej: cachear consulta TOTUS del mismo día).

---

## 2.5 RESPONSABILIDADES Y FLUJOS DE INTEGRACIÓN

### 2.5.1 Responsabilidades de ALDEBARAN (Este Sistema)

? **CALCULA:** Bonos recomendados al cierre de cada período  
? **REGISTRA:** FOTO inmutable en HistorialBono  
? **SUGIERE:** Valor de NC a aplicar en TOTUS  
? **NOTIFICA:** Usuario PROMOS para revisión  
? **AUDITÁ:** Registra cada acción de usuarios y sistema  
? **CONSULTA:** Valor facturado desde TOTUS en tiempo real  
? **DESCARGA:** Lista de precios desde Página Promocional  

? **NO APLICA DIRECTAMENTE:** NC en TOTUS (solo recomienda)  
? **NO AFECTA:** Datos en TOTUS directamente  

### 2.5.2 Responsabilidades de USUARIO PROMOS

? **REVISA:** Recomendación de NC en Aldebaran.Web  
? **APLICA MANUALMENTE:** NC en TOTUS (es responsable del valor real)  
? **REGISTRA:** Valor NC real que aplicó en TOTUS (reconciliación)  
? **INGRESA:** OC Especiales y aprobaciones  
? **RESUELVE:** Reclamaciones de distribuidores  

? **NO CALCULA:** Bonos (el sistema lo hace)  
? **NO MODIFICA:** TOTUS directamente desde Aldebaran  
? **NO EDITA:** Valores congelados en HistorialBono  

### 2.5.3 Responsabilidades de TOTUS (Sistema Externo)

? **SUMINISTRA:** Valor facturado (verdad única de facturación)  
? **RETORNA:** ValorFacturado, NotasCredito, Fletes, Descuentos  
? **RECIBE:** Recomendación de NC de Aldebaran (sugerencia)  
? **APLICA:** NC en siguiente período (responsabilidad Usuario PROMOS)  
? **REGISTRA:** NC real aplicada en su BD  

? **NO CALCULA:** Bonos (Aldebaran lo hace)  
? **NO INTERFIERE:** Con cálculos de Aldebaran  

### 2.5.4 Responsabilidades de PÁGINA PROMOCIONAL (Sistema Externo)

? **MANTIENE:** Lista de precios actualizada (diariamente)  
? **PUBLICA:** Lista de precios en formato especificado (Excel)  
? **OFRECE:** Link/Botón "Ver mi bonificación" (redirección)  
? **REDIRECCIONA:** A Sitio Público Aldebaran  

? **NO CALCULA:** Bonos  
? **NO AUTENTICA:** Distribuidores

---

## 2.6 MOCKUPS Y WIREFRAMES

**INSTRUCCIONES PARA AGREGAR MOCKUPS:**

A continuación se listan los mockups/wireframes que deben agregarse al documento. Puedes crearlos con herramientas como Figma, Balsamiq, Excalidraw, o cualquier otra.

### 2.6.1 Pantallas de Administración (Aldebaran.Web)

- [ ] **Mockup 1**: Gestión de Períodos - Listado
- [ ] **Mockup 2**: Gestión de Períodos - Crear/Editar
- [ ] **Mockup 3**: Gestión de Tipos de Bono - Listado
- [ ] **Mockup 4**: Gestión de Tipos de Bono - Crear/Editar
- [ ] **Mockup 5**: Gestión de Vigencias - Listado
- [ ] **Mockup 6**: Gestión de Vigencias - Crear/Editar (Tramos)
- [ ] **Mockup 7**: Gestión de Vigencias - Parametrización por Artículos

### 2.6.2 Pantallas de Distribuidor (Sitio Público)

- [ ] **Mockup 8**: Login - Ingreso de Documento
- [ ] **Mockup 9**: Login - Ingreso de OTP
- [ ] **Mockup 10**: Dashboard Bonificación - Período Actual
- [ ] **Mockup 11**: Gamificación - Progreso hacia Siguiente Nivel
- [ ] **Mockup 12**: Histórico de Bonos - Listado de Períodos
- [ ] **Mockup 13**: Histórico de Bonos - Detalle de Período Cerrado
- [ ] **Mockup 14**: Configuración de Preferencias de Notificación

### 2.6.3 Pantallas de Operaciones Manuales (Aldebaran.Web)

- [ ] **Mockup 15**: Consulta Admin - Selección de Distribuidor
- [ ] **Mockup 16**: Consulta Admin - Detalle de Bonos
- [ ] **Mockup 17**: Ingreso OC Especial - Unitario
- [ ] **Mockup 18**: Carga Masiva OC - Resumen de Validación
- [ ] **Mockup 19**: Reconciliación NC - Unitario
- [ ] **Mockup 20**: Reconciliación NC - Masivo - Resumen

### 2.6.4 Pantallas de Reportería (Aldebaran.Web)

- [ ] **Mockup 21**: Reporte Bonos Calculados vs Aplicados
- [ ] **Mockup 22**: Reporte Distribuidores que Consultaron
- [ ] **Mockup 23**: Reporte Discrepancias de NC
- [ ] **Mockup 24**: Reporte Auditoría Usuario PROMOS
- [ ] **Mockup 25**: Reporte Precios y Vigencias Usados
- [ ] **Mockup 26**: Reporte Ingresos Manuales Aplicados

### 2.6.5 Pantallas de Resolución de Reclamaciones (Aldebaran.Web)

- [ ] **Mockup 27**: Resolver Reclamación - Historial de Consultas
- [ ] **Mockup 28**: Resolver Reclamación - Análisis de Cambios

---

## 2.7 PRÓXIMOS PASOS

### 2.7.1 Validación con Cliente

1. **Revisión de Propuesta Funcional** con stakeholders de PROMOS
2. **Validación de Mockups** (una vez agregados)
3. **Ajustes según Feedback** del cliente

### 2.7.2 Documentación Técnica

1. **Diseño de Base de Datos** (Modelo ER completo)
2. **Especificación de APIs** (Endpoints, Request/Response)
3. **Arquitectura del Sistema** (Componentes, Integraciones)
4. **Plan de Pruebas** (Casos de prueba por funcionalidad)

### 2.7.3 Estimación de Esfuerzo

Una vez aprobada la propuesta funcional, se procederá a:

1. **Estimar tiempo de desarrollo** por funcionalidad
2. **Estimar costo** del proyecto
3. **Definir cronograma** de implementación por fases
4. **Asignar recursos** (desarrolladores, testers, etc.)

---

## 2.8 APROBACIONES

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| **Product Owner (PROMOS)** | | | |
| **Gerente de Proyecto** | | | |
| **Arquitecto de Software** | | | |
| **Líder de Desarrollo** | | | |

---

**FIN DE LA PROPUESTA FUNCIONAL**

**Documento generado por:** GitHub Copilot  
**Fecha:** Mayo 2026  
**Versión:** 1.0