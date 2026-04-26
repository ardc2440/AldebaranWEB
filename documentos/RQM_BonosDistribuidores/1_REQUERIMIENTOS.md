# 1. REQUERIMIENTOS FUNCIONALES - Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS | **Estado**: REQUERIMIENTOS DEFINIDOS | **Fecha**: 2026

---

## 1.1 Descripción General

### Problemática de Negocio

**Situación Actual (Sin Sistema):**
- Los Distribuidores deben calcular manualmente sus bonificaciones
- Deben validar el cálculo con personal de PROMOS (proceso manual y lento)
- Falta transparencia en cómo se calculan los bonos
- Los Distribuidores prefieren comprar con competencia que tiene proceso automatizado
- PROMOS invierte tiempo en validar cálculos manuales (no agrega valor)
- No hay visibilidad sobre qué falta para acceder al siguiente nivel de bonificación
- Dificultad para resolver reclamaciones (no hay auditoría del cálculo)

### Objetivo
Automatizar el cálculo de bonificaciones para distribuidores en la empresa PROMOS con tres modalidades:
- **Bonificación por Facturación**: Incentivo basado en valor total facturado en período (TOTUS)
- **Bonificación por Pedido**: Incentivo basado en valor total pedido en período (Cantidad pedida × Precio)
- **Bonificación por Entregado**: Incentivo basado en valor total entregado en período (Cantidad entregada × Precio)

### Propuesta de Valor

**Para Distribuidores (Acceso en Sitio Público - Consulta de Bonificación):**
- ? Acceso desde Página Promocional (clic en botón/link)
- ? Autenticación segura por OTP (SMS/Email) con lifetime configurable
- ? Consultar bono acumulado del período actual en tiempo real (SLA 500ms)
- ? Ver qué falta para acceder al siguiente nivel de bonificación (gamificación)
- ? Acceso seguro a información solo de su distribuidor (sin ver datos de competidores)
- ? Página informativa (solo lectura): Sin ingreso de datos adicionales
- ? Transparencia total: Resumen claro de todos sus bonos aplicables
- ? Sin necesidad de contactar a PROMOS para solicitar información

**Para PROMOS (Acceso en Aldebaran.Web - Admin):**
- ? Acceso rápido al valor final de bonificación para cada período
- ? Generar recomendación de Nota Crédito para aplicar en TOTUS
- ? Historial completo y auditable de cada cálculo (soporte para reclamaciones)
- ? Validación automática: NC calculada vs NC realmente aplicada (reconciliación)
- ? Reducción de tiempo administrativo: de manual a automático
- ? Precisión 100%: elimina errores de cálculo manual

### Modelo de Negocio

```
CLIENTE DISTRIBUIDOR (en Aldebaran):
  ?? Registro: Documento (Cédula), Nombre, Email(s), Celular
  ?? Órdenes: Pedidos de artículos (cantidad pedida + precio)
  ?? Entregas: Salidas de almacén (cantidad realmente entregada, puede ser parcial)
  ?? Facturación: Registrada en TOTUS (verdad única, se factura lo entregado)
  ?? Contacto: Email/SMS para OTP

PÁGINA PROMOCIONAL (Tercero):
  ?? Suministra: Lista de precios + Descuentos diarios
  ?? Aloja: Link/Botón "Ver mi bonificación" 
  ?? Redirecciona: A Sitio Público Aldebaran

SITIO PÚBLICO ALDEBARAN (Consulta de Bonificación):
  ?? Autenticación: OTP vía SMS/Email (lifetime configurable)
  ?? Validación: Documento distribuidor (cédula) contra Cliente en Aldebaran
  ?? Consulta: Bonos acumulados período actual (SLA 500ms)
  ?? Presentación: Informe informativo (solo lectura)
  ?? Acceso: Aislado - distribuidor solo ve SU información

ALDEBARAN (Backend/Motor de Cálculo):
  ?? Base de Datos: 
  ?  ?? Clientes (Distribuidores) - con Email, Celular
  ?  ?? Órdenes (Pedidos del distribuidor: cantidad pedida, precio)
  ?  ?? Entregas (Salidas de almacén: cantidad entregada, estado confirmado)
  ?  ?? Períodos, TiposBono, Vigencias
  ?  ?? HistorialBono (auditoría)
  ?
  ?? Carga diaria: Precios desde Página Promocional
  ?? Obtiene: Valor facturado desde TOTUS (por período - facturación real)
  ?? Obtiene: Valor pedido de órdenes + precios (cantidad pedida)
  ?? Obtiene: Valor entregado de entregas confirmadas (cantidad realmente entregada)
  ?? Calcula: Bonos (recomendación) basado en insumos
  ?? Registra: Historial y FOTO del cierre (inmutable post-cierre)
  ?? Genera: Recomendación de nota crédito para TOTUS
  ?? Reconcilia: Valor calculado vs valor real aplicado

TOTUS (Tercero - Sistema de Facturación):
  ?? Suministra: Valor facturado (verdad única de facturación)
  ?? Parámetros: Documento (cédula), Tipo Doc (FAC), Fecha inicio/fin
  ?? Retorna: ValorFacturado, NotasCredito, Fletes, Descuentos
  ?? Aplica: Las notas crédito en siguiente período
  ?? Retorna: Valor real aplicado (para reconciliación)
  ?? Integración: BD local TOTUS en servidor PROMOS (Read-only)

USUARIO PROMOS (Aldebaran.Web - Interno):
  ?? Acceso: Restringido con autenticación interna PROMOS
  ?? Funciones:
  ?  ?? Admin: Gestiona Períodos, Tipos, Vigencias
  ?  ?? Consulta: Bonos finales por distribuidor (por período)
  ?  ?? Prepara: Recomendación de NC para aplicar en TOTUS
  ?  ?? Aplica: Bonos recomendados en TOTUS (responsable valor real)
  ?  ?? Resuelve: Reclamaciones (acceso a historial completo)
  ?  ?? Genera: Reportes, exportaciones, auditoría
  ?? Responsabilidad: Usuario es quien finalmente aplica NC en TOTUS

ADMINISTRADOR (Aldebaran.Web - Interno):
  ?? Acceso: Todas las funciones de USUARIO PROMOS +
  ?? Configura: Integraciones (TOTUS, Página Promocional)
  ?? Configura: Horarios de carga, reintentos, timeouts
  ?? Ve: Logs de seguridad (accesos distribuidores)
  ?? Gestiona: Usuarios, roles, permisos

PROCESO AUTOMÁTICO (Scheduled Jobs):
  ?? Carga: Precios diariamente (horario configurable)
  ?? Obtiene: Facturación de TOTUS (por período)
  ?? Obtiene: Órdenes y entregas (continuamente)
  ?? Cierre: Automático último día del período
  ?? Reconciliación: Automática primer día período N+1
  ?? Limpieza: Datos antiguos según política de retención
```

---

## 1.2 Actores del Sistema

**EXTERNOS (Público):**
- **CLIENTE DISTRIBUIDOR**: Beneficiario de bonificaciones. Accede vía Página Promocional ? Sitio Público (con OTP)
- **PÁGINA PROMOCIONAL (Tercero)**: Suministra precios diarios + Ofrece link/botón de acceso a consulta de bonos
- **TOTUS (Tercero)**: Sistema de facturación. Suministra valor facturado, aplica notas crédito

**INTERNOS (PROMOS - Aldebaran):**
- **USUARIO PROMOS**: Persona que consulta bonos finales, prepara recomendación de NC, aplica bonos en TOTUS
- **ADMINISTRADOR**: Gestiona Períodos, Tipos, Vigencias, Integraciones, Logs de seguridad. Todas funciones de Usuario PROMOS +
- **PROCESO AUTOMÁTICO**: Carga precios, cierre período, reconciliación. Sin intervención humana

**SISTEMAS:**
- **SITIO PÚBLICO ALDEBARAN**: Portal de consulta de bonificación (autenticación OTP, solo lectura)
- **ALDEBARAN.WEB**: Backend - Motor de cálculo, gestión administrativa, integraciones

---

## 1.2.1 Matriz de Acceso

| Actor | Plataforma | Funcionalidad |
|-------|---------|---|
| **DISTRIBUIDOR (No Autenticado)** | Página Promocional | Clic en botón/link "Ver mi bonificación" |
| **DISTRIBUIDOR (No Autenticado)** | Sitio Público Aldebaran | Ingresa número documento |
| | | Recibe OTP por SMS/Email (lifetime configurable) |
| | | Ingresa OTP (máx 3 intentos) |
| **DISTRIBUIDOR (Autenticado)** | Sitio Público Aldebaran | Consultar bonos acumulados (RF6) |
| | | Ver gamificación (falta para siguiente nivel) |
| | | Ver seguridad: solo su información (RF5) |
| | | Cierre de sesión |
| **USUARIO PROMOS** | Aldebaran.Web | Crear/gestionar Períodos (RF1) |
| | | Crear/gestionar Tipos de Bono (RF2) |
| | | Crear/gestionar Vigencias (RF3) |
| | | Consultar bonos finales (RF7) |
| | | Resolver reclamaciones (Historial auditable - RF8) |
| | | Generar recomendación NC para TOTUS |
| **ADMINISTRADOR** | Aldebaran.Web | Todas las funcionalidades de Usuario PROMOS |
| | | Configurar integraciones (TOTUS, Página Promocional) |
| | | Ver logs de seguridad (Accesos distribuidores) |
| **PROCESO AUTOMÁTICO** | Aldebaran.Web | Cargar precios (RF10) |
| | | Obtener facturación TOTUS (RF11) |
| | | Obtener pedidos (RF12) |
| | | Obtener entregas (RF13) |
| | | Cierre de período (CU8) |
| | | Reconciliación NC (CU9) |

---

## 1.3 Casos de Uso (10 Total)

### CU1: Crear Período
Admin define período (Mensual/Quincenal/Semanal/Custom): Nombre, Día inicio, Duración (días)

### CU2: Crear Tipo de Bono
Admin define Tipo con Afectación (Facturación/Pedido/Entregado) y Período

### CU3: Crear Vigencia
Admin define vigencia: rangos valor, porcentaje, fecha inicio
Lógica: Vigencia más reciente (fecha ? hoy, Activo) se aplica automáticamente

### CU4: Cargar Lista de Precios (Automático Diario)
Proceso automático: Descarga ? Procesa ? Carga desde página promocional
Almacenamiento: PreciosDistribuidor (actual) + PreciosDistribuidorHistorico (4 meses)

### CU5: Autenticar Distribuidor (OTP - Seguridad)
**Ubicación:** Sitio Público Aldebaran
**Acceso:** Desde Página Promocional (clic en botón/link)
**Actor:** Distribuidor
**Flujo:**
1. Distribuidor en Página Promocional hace clic en "Ver mi bonificación"
2. Redirecciona a Sitio Público Aldebaran - Página de autenticación
3. Distribuidor ingresa número de documento (cédula)
4. Sistema valida documento en Aldebaran (existe cliente tipo Distribuidor)
5. Obtiene email(s) y celular configurados del distribuidor
6. Genera código OTP de un solo uso (6 dígitos, lifetime configurable - default 10 minutos)
7. Envía OTP por SMS (celular) y/o Email (según configuración del distribuidor)
8. Distribuidor ingresa código OTP
9. Sistema valida código
10. Si válido: Crea sesión segura + Token de acceso (lifetime configurable - default 8 horas)
11. Si inválido o expirado: Rechaza y permite reintentar (máx 3 intentos)
12. Si válido: Redirecciona a página de consulta de bonificación (solo lectura)

**Seguridad:**
- OTP válido tiempo configurable (default 10 minutos)
- Máximo 3 intentos fallidos (luego requiere solicitar nuevo OTP)
- Session token válido tiempo configurable (default 8 horas)
- Cada solicitud incluye token (validación en cada consulta)
- Logs de acceso: quién, cuándo, desde dónde

### CU6: Consultar Bonificación (Página Informativa) - DISTRIBUIDOR
**Ubicación:** Sitio Público Aldebaran
**Acceso:** Solo con autenticación OTP válida
**Actor:** Distribuidor (autenticado)
**Tipo de Página:** Informativa - Solo lectura (NO hay ingreso de datos)

**Flujo de Cálculo (Dinámico en el momento de la consulta)**:
```
Cuando el distribuidor ingresa a consultar su bonificación:

1. Sistema obtiene:
   - Período actual (ej: 1 al 15 del mes)
   - Fecha/Hora actual (ej: 11 de mes, 14:35 hrs)

2. Define rango de cálculo:
   - FechaInicio: Primer día del período (ej: 1 del mes)
   - FechaFin: Hoy a las 23:59:59 (ej: 11 del mes a 23:59:59)
   - (Nota: Busca acumulado desde inicio hasta FIN DEL DÍA ACTUAL, no hasta la hora actual)

3. Para VALOR PEDIDO: Suma todas las órdenes del distribuidor
   - Entre FechaInicio y FechaFin
   - Usando precios históricos del día de cada pedido
   - Valor acumulado hasta HOY (fin del día actual)

4. Para VALOR ENTREGADO: Suma todas las entregas confirmadas
   - Entre FechaInicio y FechaFin
   - Usando precios congelados del pedido original
   - Entregas confirmadas hasta HOY (fin del día actual)

5. Para VALOR FACTURADO: Consulta TOTUS
   - Parámetros: Documento distribuidor, Período (FechaInicio a FechaFin)
   - Retorna: Facturación acumulada hasta HOY
   - (Nota: TOTUS puede tener lag - se consulta en tiempo real)

6. Calcula cada bono aplicable:
   - Obtiene insumo (Pedido/Entregado/Facturado) con acumulado hasta HOY
   - Descuenta NC período anterior
   - Busca vigencia más reciente (activa)
   - Busca tramo correspondiente
   - Aplica porcentaje
   - Calcula gamificación (falta para siguiente nivel)

7. Retorna resultado (SLA: 500ms)
   - Bonos calculados dinámicamente
   - Reflejan estado ACTUAL del período
```

**Información Mostrada:**
```
RESUMEN PERSONAL DE BONIFICACIÓN
??????????????????????????????????????????
Distribuidor: [Nombre]
Documento: [Cédula]
Período Actual: [1 al 15 del mes] - Hoy: [11 del mes]
Acumulado desde inicio del período hasta HOY: 11 días

BONIFICACIONES APLICABLES AL DISTRIBUIDOR
??????????????????????????????????????????

Bono 1: Bonificación por Facturación
  Valor Facturado Acumulado (1 al 11): $XX,XXX,XXX
  Menos: NC Período Anterior: $X,XXX,XXX
  Valor Neto: $XX,XXX,XXX
  Tramo Aplicable: $XM - $YM
  Porcentaje: XX%
  ? BONO FACTURACIÓN (ACTUAL): $XX,XXX,XXX

  Falta para siguiente nivel:
  [Barra visual] XX% hacia siguiente tramo
  Si facturas $X,XXX,XXX más ? Accedes a XX% (XX,XXX más de bono)

Bono 2: Bonificación por Pedido
  Valor Pedido Acumulado (1 al 11): $XX,XXX,XXX
  Tramo Aplicable: $XM - $YM
  Porcentaje: XX%
  ? BONO PEDIDO (ACTUAL): $XX,XXX,XXX

  Falta para siguiente nivel:
  [Barra visual] XX% hacia siguiente tramo
  Si pides $X,XXX,XXX más ? Accedes a XX% (XX,XXX más de bono)

Bono 3: Bonificación por Entregado
  Valor Entregado Acumulado (1 al 11): $XX,XXX,XXX
  Tramo Aplicable: $XM - $YM
  Porcentaje: XX%
  ? BONO ENTREGADO (ACTUAL): $XX,XXX,XXX

  Falta para siguiente nivel:
  [Barra visual] XX% hacia siguiente tramo
  Si entregas $X,XXX,XXX más ? Accedes a XX% (XX,XXX más de bono)

??????????????????????????????????????????
TOTAL BONIFICACIÓN (ACTUAL): $XX,XXX,XXX
(Cálculo dinámico - refleja acumulado a la fecha)
Se aplicará como Nota Crédito al CIERRE del período
??????????????????????????????????????????

[Botón: Cerrar Sesión]
```

**Validaciones:**
- Token válido y no expirado
- Distribuidor solo ve su propia información (por documento/cédula)
- No puede acceder a información de otro distribuidor
- Página de solo lectura: Sin campos de entrada, sin acciones
- Cálculo dinámico: Se ejecuta cada vez que consulta (no está precalculado)

### CU7: Consultar Bono Actual (Dinámico) - PROMOS
**Ubicación:** Aldebaran.Web
**Acceso:** Admin - Autenticación interna PROMOS
**Actor:** Usuario PROMOS
Consulta bono final para cada distribuidor (preparar recomendación de NC para TOTUS)
Información: Bono calculado, Historial de cálculo, Auditoría completa

### CU8: Cierre de Período (Automático)
**Cuándo:** Último día del período, a hora configurada (ej: 23:59:59)

**IMPORTANTE - RESPONSABILIDADES CLARAS**:
```
????????????????????????????????????????????????????????????????
? ALDEBARAN (Este Sistema):                                    ?
? ?? CALCULA: Bono recomendado al cierre                       ?
? ?? REGISTRA: FOTO en HistorialBono (inmutable)               ?
? ?? SUGIERE: Valor de NC a aplicar en TOTUS                  ?
? ?? NOTIFICA: Usuario PROMOS para revisión                   ?
?                                                              ?
? ? NO APLICA DIRECTAMENTE en TOTUS                          ?
? ? NO AFECTA datos en TOTUS                                 ?
?                                                              ?
????????????????????????????????????????????????????????????????
? USUARIO PROMOS (Humano - Responsable):                       ?
? ?? CONSULTA: Recomendación de NC en Aldebaran.Web           ?
? ?? REVISA: Montos y detalles (validación manual si desea)   ?
? ?? APLICA: La NC en TOTUS (responsable del valor real)      ?
? ?? CONFIRMA: Que la NC se registró en TOTUS                 ?
?                                                              ?
????????????????????????????????????????????????????????????????
? TOTUS (Sistema Tercero - Verdad Única):                      ?
? ?? RECIBE: Recomendación de NC (sugerencia de Aldebaran)    ?
? ?? APLICA: La NC en el siguiente período (Usuario PROMOS)   ?
? ?? REGISTRA: NC real aplicada en su BD                       ?
? ?? RETORNA: Valor real aplicado (para reconciliación)       ?
?                                                              ?
????????????????????????????????????????????????????????????????
```

**Proceso Detallado:**

```
1. Sistema Aldebaran identifica: Hoy es el último día del período actual
   Ej: Período 1-15 del mes, hoy es 15

2. Para CADA distribuidor que tuvo actividad en el período:

   a) Calcula bono RECOMENDADO del período (1 al 15 completo):
      - Valor Pedido: SUM(todas órdenes del 1 al 15)
      - Valor Entregado: SUM(todas entregas del 1 al 15)
      - Valor Facturado: TOTUS(1 al 15)
      - Aplica descuentos, vigencias, tramos
      - RESULTADO: Bono Recomendado = $X,XXX,XXX
      - ?? ESTE ES UN CÁLCULO, NO ES FINAL AÚN

   b) Registra FOTO (congelada en Aldebaran):
      - Almacena en HistorialBono
      - Estado: CALCULADO (no APLICADO)
      - Datos: Distribuidor, Período, Bono Calculado, Detalles
      - Datos: Precio usados ese día, Vigencia, Tramo aplicado
      - Esta FOTO NO CAMBIA (auditoría del cálculo)

   c) Genera RECOMENDACIÓN de Nota Crédito:
      - Distribuidor: ID
      - Valor: Bono Calculado (SUGERENCIA)
      - Para aplicar en: Siguiente período (RECOMENDACIÓN)
      - Estado: RECOMENDADA (en espera)
      - ?? PENDIENTE que Usuario PROMOS la aplique en TOTUS

3. Marca período como CERRADO en Aldebaran:
   - Estado: CERRADO
   - Ya no permite recalcular bonos
   - HistorialBono es INMUTABLE (auditoría)
   - ? PERO LA NC AÚN NO ESTÁ APLICADA EN TOTUS

4. Publica evento (RabbitMQ):
   - Event: "PeriodoCerrado"
   - Datos: PeriodoId, Cantidad distribuidores, Timestamp
   - Consumer: NotificationProcessor
     ?? Notifica a Usuario PROMOS: 
        "Revisar X recomendaciones de NC para aplicar en TOTUS"

5. Usuario PROMOS (Responsable - Paso Manual):
   - Accede a Aldebaran.Web
   - Consulta lista de NCs recomendadas
   - Revisa cada monto (validación manual si desea)
   - Abre TOTUS (sistema tercero)
   - APLICA MANUALMENTE cada NC en TOTUS
   - Confirma: Que la NC fue registrada en TOTUS
   - ?? USUARIO es el responsable del valor real en TOTUS

6. TOTUS (Tercero - Verdad Única):
   - Recibe y registra la NC (aplicada por Usuario PROMOS)
   - Almacena: NC real aplicada
   - Desconecta del período siguiente
   - Ejemplo: NC de Enero se aplica en Febrero
   - Sistema retiene: Registro de NC real aplicada

7. Primer día período N+1 - Reconciliación Automática (CU9):
   - Aldebaran consulta TOTUS
   - Pregunta: "¿Qué NC realmente se aplicó en período anterior?"
   - TOTUS retorna: NC real aplicada
   - Aldebaran compara:
      ?? NC Calculada: $X,XXX,XXX (en HistorialBono)
      ?? NC Real: $Y,XXX,XXX (retornada por TOTUS)
      ?? Si diferencia: Registra, Alerta, Audita
   - Actualiza HistorialBono con ValorReal
   - Próximos cálculos usan ValorReal (no el calculado)
```

**DIFERENCIAS CLAVE - DEBE QUEDAR CRISTALINO**:

```
??????????????????????????????????????????????????????????????
? BONO CALCULADO (Aldebaran - Día 15)                        ?
??????????????????????????????????????????????????????????????
? • Es un CÁLCULO basado en datos disponibles               ?
? • Es una RECOMENDACIÓN para aplicar en TOTUS             ?
? • Es SUGERENTE, no es definitivo                         ?
? • Se almacena en HistorialBono (auditoría)               ?
? • Se congela (NO CAMBIA aunque precios cambien)          ?
? • Se usa para notificar Usuario PROMOS                   ?
? • ? NO se aplica automáticamente en TOTUS               ?
? • ? NO afecta directamente TOTUS                        ?
??????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????
? NC REAL (TOTUS - Aplicada por Usuario PROMOS)             ?
??????????????????????????????????????????????????????????????
? • Es lo que REALMENTE se aplicó en TOTUS                 ?
? • Puede ser ? del bono calculado (si Usuario decide)     ?
? • Se registra en BD de TOTUS (tercero)                   ?
? • Es la VERDAD ÚNICA para el siguiente período            ?
? • Se recupera mediante reconciliación (CU9)              ?
? • Se usa para calcular bonos futuros (descuento NC)      ?
? • ? ES lo que realmente afecta al distribuidor         ?
??????????????????????????????????????????????????????????????
```

**FLUJO TEMPORAL COMPLETO - EJEMPLO REAL**:

```
PERÍODO 1: 1 AL 15 DE ENERO
???????????????????????????????????????????????????????????????

DÍA 15 (23:59:59) - CIERRE:
  ?? Aldebaran CALCULA bono: $1,000,000 (recomendado)
  ?? Almacena FOTO en HistorialBono
  ?? Estado: CALCULADO
  ?? Notifica a Usuario PROMOS: "1 NC recomendada: $1M"

DÍA 16 - USUARIO PROMOS:
  ?? Accede Aldebaran.Web
  ?? Ve recomendación: $1,000,000
  ?? Revisa: Datos, vigencia, tramo (validación)
  ?? Abre TOTUS (sistema tercero)
  ?? Aplica NC de $1,000,000 en TOTUS (manual)
  ?? Confirma en TOTUS: NC registrada
  ?? ?? Usuario es responsable del valor real

PERÍODO 2: 1 AL 15 DE FEBRERO
???????????????????????????????????????????????????????????????

DÍA 1 (00:00:00) - RECONCILIACIÓN AUTOMÁTICA:
  ?? Aldebaran consulta TOTUS:
     "¿Qué NC se aplicó en período anterior (Enero)?"
  ?? TOTUS retorna: $1,000,000 (la que aplicó Usuario)
  ?? Aldebaran compara:
     Calculada: $1,000,000
     Real:      $1,000,000
     ? Match (sin diferencia)
  ?? Actualiza HistorialBono: ValorReal = $1,000,000
  ?? Estado: RECONCILIADO

DÍA 11 DE FEBRERO - DISTRIBUIDOR CONSULTA:
  ?? Aldebaran CALCULA bono dinámico (1 al 11 febrero)
  ?? Descuenta NC período anterior:
     Valor Facturado Feb: $50,000,000
     Menos NC Enero (real): -$1,000,000 (la que TOTUS aplicó)
     = $49,000,000 (base para bono)
  ?? Aplica vigencia/tramo: Bono = $2,450,000 (dinámico)

NOTA: Si Usuario PROMOS hubiera aplicado NC diferente:
  Ejemplo: $900,000 en lugar de $1,000,000
  ?? TOTUS retorna en reconciliación: $900,000
  ?? Aldebaran detecta diferencia
  ?? Registra: Discrepancia $100,000
  ?? Alerta admin
  ?? Próximos cálculos usan $900,000 (valor real)
```

**RESPONSABILIDADES BIEN DEFINIDAS**:

```
??????????????????????????????????????????????????????????????????
? PÁGINA PROMOCIONAL (Tercero - Externo):                        ?
??????????????????????????????????????????????????????????????????
? ? Mantiene lista de precios actualizada (diariamente)          ?
? ? Publica lista de precios en formato especificado (Excel)     ?
? ? Asegura disponibilidad de descargas (sin downtime)           ?
? ? Ofrece link/botón "Ver mi bonificación" (redirección)        ?
? ? Redirecciona a Sitio Público Aldebaran (endpoint configurable)
? ? Asegura comunicación HTTPS segura                            ?
? ? NO calcula bonos                                             ?
? ? NO autentica distribuidores                                  ?
? ? NO accede a datos de TOTUS                                   ?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? ALDEBARAN - SCHEDULED JOBS (Automático):                       ?
??????????????????????????????????????????????????????????????????
? ? Descarga lista de precios (horario configurable)             ?
? ? Valida estructura y datos de precios                         ?
? ? Almacena precios en PreciosDistribuidor (actual)             ?
? ? Copia histórico en PreciosDistribuidorHistorico (con fecha)  ?
? ? Limpia precios antiguos (según política retención)           ?
? ? Notifica errores a administrador                             ?
? ? Fallback a precios anteriores si descarga falla              ?
? ? Realiza cierre de período (último día, hora configurable)    ?
? ? Calcula bonos recomendados al cierre                         ?
? ? Almacena FOTO en HistorialBono (inmutable post-cierre)       ?
? ? Genera recomendaciones de NC (estado RECOMENDADA)            ?
? ? Cierra período (estado CERRADO)                              ?
? ? Publica evento PeriodoCerrado (RabbitMQ)                     ?
? ? NO aplica NC en TOTUS                                        ?
? ? NO modifica datos de TOTUS                                   ?
? ? NO realiza reconciliación automática (requiere acción humana)?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? ALDEBARAN - MOTOR DE CÁLCULO (En línea):                       ?
??????????????????????????????????????????????????????????????????
? ? Obtiene período actual                                       ?
? ? Suma todas las órdenes (cantidad + precio histórico)         ?
? ? Suma todas las entregas confirmadas (cantidad + precio)      ?
? ? Consulta TOTUS: Valor facturado real (tiempo real)           ?
? ? Busca NC período anterior (de HistorialBono reconciliado)    ?
? ? Busca vigencia más reciente (activa)                         ?
? ? Busca tramo correspondiente                                  ?
? ? Aplica porcentaje del tramo                                  ?
? ? Calcula gamificación (falta para siguiente nivel)            ?
? ? Retorna bono dinámico (SLA: 500ms)                           ?
? ? Registra en auditoría: qué se consultó, cuándo              ?
? ? NO precalcula bonos                                          ?
? ? NO modifica TOTUS                                            ?
? ? NO aplica NC automáticamente                                 ?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? ALDEBARAN - CONFIGURACIÓN (Admin):                             ?
??????????????????????????????????????????????????????????????????
? ? Crea/modifica Períodos (nombre, inicio, duración)            ?
? ? Crea Tipos de Bono (afectación, estrategia precio)           ?
? ? Crea Vigencias (rangos, porcentaje, fecha inicio)            ?
? ? Configura horario carga precios                              ?
? ? Configura retención histórico de precios                     ?
? ? Configura horario y cadencia limpieza                        ?
? ? Configura horario cierre período                             ?
? ? Configura reintentos y timeouts                              ?
? ? Configura integración TOTUS (SP, parámetros)                 ?
? ? Configura integración Página Promocional (URL, auth)         ?
? ? Gestiona usuarios, roles, permisos                           ?
? ? Consulta logs de seguridad                                   ?
? ? NO modifica datos históricos (auditoría)                     ?
? ? NO interfiere con cierres ya realizados                      ?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? USUARIO PROMOS (Humano - Responsable Final):                   ?
??????????????????????????????????????????????????????????????????
?                                                                ?
? ?? CONSULTA Y REVISIÓN ??                                      ?
? ? Consulta recomendación de NC en Aldebaran.Web                ?
? ? Revisa montos calculados por el sistema                      ?
? ? Accede a historial completo de cálculo                       ?
? ? Ve detalles: Vigencia usada, Precios aplicados, NC anterior ?
?                                                                ?
? ?? INGRESO MANUAL DE DATOS (CRÍTICO) ??                        ?
? ? Puede ingresar manualmente Valor Facturado REAL de TOTUS     ?
?   (para casos donde TOTUS no retorna valor en tiempo real)     ?
? ? Registra la fecha en que se ingresó este valor               ?
? ? Sistema usa valor ingresado en cálculos futuros si aplica    ?
? ? Auditoría registra: quién, qué, cuándo ingresó              ?
?                                                                ?
? ?? APLICACIÓN EN TOTUS ??                                      ?
? ? Abre TOTUS (sistema tercero)                                 ?
? ? APLICA MANUALMENTE cada NC recomendada en TOTUS              ?
? ? Puede aplicar NC diferente a la recomendada (su decisión)    ?
? ? Registra en el sistema: valor NC real que aplicó en TOTUS    ?
? ? Confirma que NC fue registrada en TOTUS                      ?
?                                                                ?
? ?? RECONCILIACIÓN MANUAL (NO AUTOMÁTICA) ??                    ?
? ? Ingresa manualmente: Monto NC que TOTUS confirma aplicó      ?
? ? Sistema compara: NC calculada vs NC real ingresada           ?
? ? Si hay diferencia: Sistema alerta y registra discrepancia    ?
? ? Auditoría: Qué diferencia hubo, por qué, quién la registró   ?
? ? Próximos cálculos usan NC real ingresada (no la calculada)   ?
?                                                                ?
? ?? SOPORTE A DISTRIBUIDORES ??                                 ?
? ? Resuelve reclamaciones de distribuidores                     ?
? ? Accede a historial completo de cálculo de cada distribuidor  ?
? ? Ve exactamente qué bonos se le entregaron en consultas       ?
? ? Justifica los cálculos con datos congelados en HistorialBono ?
?                                                                ?
? ?? GENERACIÓN DE REPORTES ??                                   ?
? ? Reporte: Bonos calculados vs Bonos realmente aplicados       ?
?   - Por período y distribuidor                                 ?
?   - Muestra: Monto calculado + Monto aplicado + Diferencias    ?
?   - Detecta: NCs aplicadas parcialmente o no aplicadas         ?
?                                                                ?
? ? Reporte: Distribuidores que consultaron bonos                ?
?   - Listado con: Distribuidor, Fecha consulta, Hora, Bono     ?
?   - Muestra: Exactamente qué información se le mostró          ?
?   - Filtro: Por período, fecha, rango de bonos                 ?
?   - Auditoría: Quién consultó, desde dónde, cuándo             ?
?                                                                ?
? ? Reporte: Discrepancias de NC (calculada vs real)             ?
?   - Muestra todas las diferencias encontradas                  ?
?   - Detalles: Distribuidor, período, monto diferencia          ?
?   - Causa: Quién registró qué y cuándo                         ?
?   - Estado: Resuelta o pendiente                               ?
?                                                                ?
? ? Reporte: Auditoría de acciones del usuario PROMOS            ?
?   - Historial: Qué hizo, cuándo, resultado                     ?
?   - NCs aplicadas, valores ingresados, reconciliaciones        ?
?   - Modificaciones y decisiones tomadas                        ?
?                                                                ?
? ? Reporte: Precios usados en período                           ?
?   - Muestra: Qué lista de precios se usó en cada cálculo       ?
?   - Vigencia aplicada para cada tipo de bono                   ?
?   - Tramos usados en cálculos finales                          ?
?                                                                ?
? ? Reporte: Exportación de datos                                ?
?   - Excel/PDF de cualquier reporte                             ?
?   - Formato configurable según necesidad                       ?
?                                                                ?
? ?? RESTRICCIONES ??                                            ?
? ? NO calcula bonos (el sistema lo hace)                        ?
? ? NO modifica TOTUS directamente desde Aldebaran               ?
? ? NO interfiere con cierres automáticos ya realizados          ?
? ? NO puede editar valores congelados en HistorialBono          ?
? ? NO puede eliminar registros de auditoría                     ?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? TOTUS (Tercero - Verdad Única - Sistema Facturación):          ?
??????????????????????????????????????????????????????????????????
? ? Suministra Valor Facturado (via SP, parámetros configurables)?
? ? Retorna: ValorFacturado, NC, Fletes, Descuentos             ?
? ? Recibe recomendación de NC de Aldebaran (sugerencia)         ?
? ? Aplica NC en siguiente período (responsabilidad Usuario)     ?
? ? Registra NC real aplicada en su BD                           ?
? ? Retorna NC real aplicada (para reconciliación Aldebaran)     ?
? ? Es fuente de verdad para valor facturado                     ?
? ? Es fuente de verdad para NC realmente aplicadas              ?
? ? NO calcula bonos                                             ?
? ? NO interfiere con cálculos de Aldebaran                      ?
? ? NO aplica NC automáticamente (Usuario es responsable)        ?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? SITIO PÚBLICO ALDEBARAN (Portal - Distribuidor):               ?
??????????????????????????????????????????????????????????????????
? ? Autentica distribuidor (OTP por SMS/Email)                   ?
? ? Valida documento distribuidor contra BD Aldebaran            ?
? ? Genera OTP (6 dígitos, 10 min default, configurable)         ?
? ? Envía OTP por canal preferido (SMS o Email)                  ?
? ? Valida OTP ingresado (max 3 intentos)                        ?
? ? Genera Token JWT (8 horas default, configurable)             ?
? ? Consulta Motor de Cálculo (obtiene bono dinámico)            ?
? ? Retorna bono dinámico (SLA: 500ms)                           ?
? ? Solo muestra información del distribuidor autenticado         ?
? ? Página solo lectura (sin entrada de datos)                   ?
? ? Gestiona gamificación (falta para siguiente nivel)            ?
? ? Registra logs de auditoría (acceso, consultas)               ?
? ? Invalida token al cierre de sesión                           ?
? ? NO calcula bonos (delega a Motor de Cálculo)                 ?
? ? NO aplica NC                                                 ?
? ? NO modifica datos en BD                                      ?
??????????????????????????????????????????????????????????????????

??????????????????????????????????????????????????????????????????
? DISTRIBUIDOR (Cliente - Externo):                              ?
??????????????????????????????????????????????????????????????????
? ? Haz clic en "Ver mi bonificación" (Página Promocional)       ?
? ? Ingresa documento (cédula) en Sitio Público                  ?
? ? Recibe OTP (SMS o Email)                                     ?
? ? Ingresa OTP (máx 3 intentos)                                 ?
? ? Consulta su bonificación acumulada                           ?
? ? Ve gamificación (falta para siguiente nivel)                 ?
? ? Cierra sesión                                                ?
? ? NO calcula bonos (sistema lo hace)                           ?
? ? NO accede a datos de otros distribuidores                    ?
? ? NO accede a Aldebaran.Web (admin interno)                    ?
??????????????????????????????????????????????????????????????????
```

**IMPORTANTE PARA PRÓXIMOS PERÍODOS**:

```
En cálculos futuros (período N+1, N+2, etc.):
  ?? Aldebaran SIEMPRE usa NC REAL (del historial reconciliado)
  ?? No usa NC CALCULADA (aunque esté en FOTO)
  ?? Esto asegura precisión basada en lo realmente aplicado
  ?? Ejemplo:
     Bono por Facturación Feb = Fact Feb - NC REAL Enero
     NC REAL Enero = lo que TOTUS registró
     (no lo que se calculó en Aldebaran)
```

### CU9: Reconciliación de Nota Crédito (Automático)
Al inicio período N+1: Obtiene NC REAL de TOTUS, Actualiza historial, Valida vs NC calculada

### CU10: Resolver Reclamación (Soporte)
**Acceso:** Aldebaran.Web (Admin)
**Actor:** Usuario PROMOS
Consulta historial completo de cálculo para responder reclamos de distribuidores
Información: Paso a paso del cálculo, Vigencia usada, Precios aplicados, NC anterior descontada

---

## 1.4 Requisitos Funcionales (26 - TODOS ALTA PRIORIDAD)

| RF | Descripción | Categoría |
|----|---|---|
| RF1 | Gestionar Períodos | Administración |
| RF2 | Gestionar Tipos de Bono | Administración |
| RF3 | Gestionar Vigencias | Administración |
| RF4 | **Autenticar Distribuidor (OTP - SMS/Email)** | Seguridad |
| RF5 | Validar Seguridad: Solo distribuidor ve su información | Seguridad |
| RF6 | Consultar Bonificación (Distribuidor - Sitio Público - Solo Lectura) | Consultas |
| RF7 | Consultar Bono Actual Dinámico (Admin - Aldebaran.Web) | Consultas |
| RF8 | Registrar Historial de Bonos (Auditoría completa) | Historial |
| RF9 | Gamificación: Mostrar falta para siguiente nivel | Historial |
| RF10 | Cargar Lista Precios Distribuidores | Integración |
| RF11 | Capturar Valor Facturado (TOTUS) | Integración |
| RF12 | Capturar Valor Pedido (Aldebaran + Precios) | Integración |
| RF13 | Capturar Valor Entregado (Aldebaran) | Integración |
| RF14 | Gestionar Nota Crédito Período Anterior | Integración |
| RF15 | Reconciliación Nota Crédito (TOTUS - Manual) | Integración |
| RF16-A | **Ingreso Manual de Órdenes de Compra Especiales (Unitario)** | Usuario PROMOS |
| RF16-B | **Carga Masiva de Órdenes de Compra Especiales (CSV)** | Usuario PROMOS |
| RF17 | Aplicación Manual de NC en TOTUS (con Confirmación) | Usuario PROMOS |
| RF18 | Reconciliación Manual de NC (Unitario + CSV Masivo) | Usuario PROMOS |
| RF19 | **Gestión de Aprobaciones para Ingresos Manuales** | Seguridad/Control |
| RF20 | Reporte: Bonos Calculados vs Bonos Aplicados por Período | Reportería |
| RF21 | Reporte: Distribuidores que Consultaron Bonos (Log) | Reportería |
| RF22 | Reporte: Discrepancias de NC (Calculada vs Real) | Reportería |
| RF23 | Reporte: Auditoría de Acciones del Usuario PROMOS | Reportería |
| RF24 | Reporte: Precios y Vigencias Usados en Período | Reportería |
| RF25 | Reporte: Ingresos Manuales Aplicados (OC + Reconciliaciones) | Reportería |
| RF26 | Exportación de Reportes (Excel/PDF) | Reportería |

---

## 1.5 Requisitos No Funcionales

| Requisito | Especificación |
|-----------|---|
| Disponibilidad | 99 porciento |
| Rendimiento | Consulta: 500ms, Cierre: 5min |
| **Seguridad - Autenticación** | OTP de un solo uso (6 dígitos, válido 10 minutos) |
| **Seguridad - Sesión** | Token de acceso válido 8 horas |
| **Seguridad - Acceso** | Cada distribuidor solo ve su información (no puede acceder a otro) |
| **Seguridad - Intentos** | Máximo 3 intentos fallidos de OTP (luego requiere nuevo OTP) |
| **Seguridad - Auditoría** | Logs de: Acceso (quién, cuándo, desde dónde), OTP enviado, OTP validado |
| **Seguridad - Encriptación** | OTP y token en tránsito encriptados (HTTPS), Datos sensibles en BD encriptados |
| Escalabilidad | Miles distribuidores |
| Integrabilidad | TOTUS + Página Promocional |
| Mantenibilidad | 100 porciento sin código |

---

## 1.6 Restricciones

- Bono se aplica como NOTA CRÉDITO en siguiente período
- Aldebaran: Calcula/recomienda, TOTUS: Aplica, Usuario: Responsable
- Historial inmutable después de cierre
- .NET 7, SQL Server, Blazor Server
- Precios: Carga diaria, se usa MÁS RECIENTE
- TOTUS es "verdad única" para valor facturado
- Vigencia: Nueva vigencia, NO edición (auditoría)
- **Seguridad:** Distribuidor accede desde Página Promocional ? Redirige a Sitio Público Aldebaran (OTP) ? Página informativa solo lectura. NO tiene acceso a Aldebaran.Web

---

## 1.6.1 SEGURIDAD - Autenticación Distribuidor (OTP)

### Flujo de Autenticación

```
PASO 1: INGRESO DE DOCUMENTO
???????????????????????????????????????
? Distribuidor en Página Promocional  ?
???????????????????????????????????????
? 1. Ingresa: Número documento (cédula)
? 2. Sistema valida que existe en Aldebaran
? 3. Sistema obtiene: Email(s) + Celular
? 4. Registra intento: timestamp, IP, documento
???????????????????????????????????????

PASO 2: GENERACIÓN Y ENVÍO OTP
???????????????????????????????????????
? Generación de OTP (One Time Password)
???????????????????????????????????????
? 1. Genera código aleatorio: 6 dígitos
? 2. Válido por: 10 minutos (configurable)
? 3. Almacena en BD: OTP + timestamp + documento
? 4. Busca canal preferido (SMS o Email):
?    - Si tiene celular: Envía SMS
?    - Si NO tiene celular: Envía Email
?    - Si tiene ambos: Envía SMS + Email (usuario elige)
? 5. Registra: qué OTP, cuándo, por qué canal
???????????????????????????????????????

PASO 3: VALIDACIÓN OTP
???????????????????????????????????????
? Distribuidor recibe OTP y lo ingresa
???????????????????????????????????????
? 1. Sistema recibe código ingresado
? 2. Valida:
?    ? OTP existe en BD
?    ? OTP no expirado (? 10 minutos)
?    ? OTP no fue usado ya
?    ? Intentos < 3
? 3. Si válido:
?    - Marca OTP como USADO
?    - Genera Token JWT (8 horas de validez)
?    - Crea Sesión: documento + token + timestamp
?    - Registra: OTP validado correctamente
? 4. Si inválido:
?    - Incrementa contador de intentos
?    - Si intentos ? 3: Bloquea, debe solicitar nuevo OTP
?    - Registra: Intento fallido
???????????????????????????????????????

PASO 4: ACCESO CON TOKEN
???????????????????????????????????????
? Distribuidor ahora accede a bono
???????????????????????????????????????
? 1. Cada solicitud incluye Token en header
? 2. Sistema valida:
?    ? Token existe
?    ? Token no expirado (< 8 horas)
?    ? Token pertenece al documento autenticado
? 3. Si válido: Retorna bono del distribuidor
? 4. Si inválido: Rechaza (401 Unauthorized)
? 5. Registra: Qué información consultó, cuándo
???????????????????????????????????????

PASO 5: CIERRE DE SESIÓN
???????????????????????????????????????
? Distribuidor cierra sesión
???????????????????????????????????????
? 1. Token se invalida manualmente
? 2. O automáticamente después de 8 horas
? 3. Registra: Cierre de sesión, timestamp
???????????????????????????????????????
```

### Matriz de Validaciones

| Validación | Cuándo | Acción si FALLA | Registro |
|-----------|--------|---|---|
| Documento existe en Aldebaran | Ingresa documento | Rechaza acceso | Intento con doc inválido |
| Es tipo "DISTRIBUIDOR" | Valida documento | Rechaza acceso | Acceso denegado: no es distribuidor |
| OTP válido (6 dígitos) | Ingresa OTP | Rechaza | Intento OTP inválido |
| OTP no expirado | Ingresa OTP | Rechaza | OTP expirado |
| OTP no usado | Ingresa OTP | Rechaza | OTP ya usado |
| Intentos < 3 | Falla validación | Bloquea, requiere nuevo OTP | Bloqueado por 3 intentos |
| Token no expirado | Realiza consulta | Rechaza (401) | Sesión expirada |
| Token pertenece a documento | Realiza consulta | Rechaza | Intento acceso con token inválido |
| Distribuidor = documento del token | Consulta bono | Solo retorna su bono | Acceso aislado |

### Configuración de Contacto (Aldebaran)

**Campos requeridos en Cliente/Distribuidor:**
- Documento (Cédula)
- Email (obligatorio) - puede ser múltiple separado por comas
- Celular (opcional, pero si existe se prefiere SMS)
- Preferencia de contacto (SMS, Email, Ambos)

**Si NO tiene Email ni Celular:**
- No puede autenticarse
- Sistema rechaza: "No hay forma de enviar OTP"

### Logs de Auditoría (Seguridad)

**Se registra SIEMPRE:**
```
Tabla: AuditoriaSeguridadDistribuidor
Campos: 
  - Timestamp
  - Tipo evento (Intento Login, OTP Generado, OTP Validado, Consulta Bono, Sesión Expirada)
  - Documento distribuidor
  - IP origen
  - Resultado (Éxito/Fallo + motivo)
  - Detalles adicionales
  - Usuario que realizó (si aplica)
```

### Casos de Seguridad Especiales

**Caso 1: Distribuidor olvida OTP**
- Debe solicitar nuevo OTP (aparece opción)
- OTP anterior se invalida
- Se genera nuevo código
- Se envía nuevamente

**Caso 2: Distribuidor ingresa mal OTP 3 veces**
- Se bloquea temporalmente
- Debe solicitar nuevo OTP desde cero
- Se registra: Intento de fuerza bruta

**Caso 3: OTP expira mientras distribuidor lo ingresa**
- Sistema rechaza: "OTP expirado"
- Debe solicitar nuevo

**Caso 4: Token expira durante consulta**
- Sistema retorna 401 (No autorizado)
- Distribuidor debe re-autenticarse

**Caso 5: Distribuidor intenta acceder con Token de otro**
- Sistema rechaza (token no pertenece a su documento)
- Se registra como intento de acceso no autorizado

---

## 1.7 INSUMOS - OBTENCIÓN DE VALORES PARA CÁLCULO

### 1.7.1 VALOR FACTURADO (De TOTUS via Procedimiento Almacenado)

**Ubicación**: BD TOTUS en servidor local PROMOS (sin problemas conectividad)

**Procedimiento Almacenado**:
- Nombre: A definir (parametrizable en appSettings, ej: sp_ObtenerFacturacion)
- STATUS: NO EXISTE AÚN en TOTUS (en construcción)
- Uso: MOCK parametrizable mientras se construye

**ENTRADA (Parámetros que ALDEBARAN ENVÍA)**:
```
- TipoDocumento: "FAC" (Factura)
- NumeroDocumento: "1234567890" (Cédula del distribuidor)
- FechaInicio: "2024-01-01" (Primer día del período)
- FechaFin: "2024-01-31" (Último día del período)
```

**SALIDA (Lo que TOTUS RETORNA)**:
```
- ValorTotalFacturadoSinImpuestos: decimal (CRÍTICO para bono)
- TotalNotasCredito: decimal (Descuentos acumulados)
- TotalFletes: decimal (Fletes del período)
- TotalDescuentos: decimal (Descuentos comerciales)
```

**COMPONENTES DEL BONO POR FACTURACIÓN**:

El Bono por Facturación se calcula con 4 insumos:

```
????????????????????????????????????????????????????????????????
? INSUMO 1: VALOR FACTURADO (De TOTUS)                         ?
????????????????????????????????????????????????????????????????
? Origen: Procedimiento Almacenado en TOTUS                    ?
? Valor: ValorTotalFacturadoSinImpuestos                       ?
? Ejemplo: $100,000,000                                        ?
????????????????????????????????????????????????????????????????

????????????????????????????????????????????????????????????????
? INSUMO 2: NOTAS CRÉDITO DEL PERÍODO (De TOTUS)               ?
????????????????????????????????????????????????????????????????
? Origen: Procedimiento Almacenado en TOTUS                    ?
? Valor: TotalNotasCredito                                     ?
? Uso: Se descuentan del valor facturado bruto                 ?
? Ejemplo: $5,000,000                                          ?
? Cálculo: ValorFacturado - NotasCreditoDelPeriodo             ?
????????????????????????????????????????????????????????????????

????????????????????????????????????????????????????????????????
? INSUMO 3: NOTA CRÉDITO DEL PERÍODO ANTERIOR                  ?
????????????????????????????????????????????????????????????????
? Origen: Historial de Bonos (reconciliado)                    ?
? Valor: NC Real que se aplicó en TOTUS período anterior       ?
? Uso: Se descuenta del valor neto para no doble contar        ?
? Ejemplo: $1,500,000                                          ?
? Cálculo: (Fact - NC_Período) - NC_PeríodoAnterior            ?
????????????????????????????????????????????????????????????????

????????????????????????????????????????????????????????????????
? INSUMO 4: ÓRDENES DE COMPRA ESPECIALES (Ingreso Manual)      ?
????????????????????????????????????????????????????????????????
? Origen: Usuario PROMOS (ingresa manualmente)                 ?
? Descripción: Órdenes que NO vienen de Aldebaran ni de TOTUS  ?
? Razón: PROMOS conoce el valor total (es el único insumo)     ?
? Valor: Valor total de la OC Especial (NO por artículo)       ?
? Uso: Se SUMA al valor facturado (es insumo adicional)        ?
? Ejemplo: $2,000,000                                          ?
? Nota: Requiere aprobación si excede límite configurado       ?
? Estado: Puede ser PENDIENTE APROBACIÓN o APROBADA            ?
????????????????????????????????????????????????????????????????
```

**FÓRMULA DEL BONO POR FACTURACIÓN**:

```
Paso 1: Obtiene de TOTUS
  ValorFacturadoBruto = ValorTotalFacturadoSinImpuestos
  Ej: $100,000,000

Paso 2: Descuenta Notas Crédito del PERÍODO ACTUAL (de TOTUS)
  NotasCredito_PeriodoActual = Obtenido de TOTUS
  Ej: $5,000,000
  ValorFacturadoNeto = ValorFacturadoBruto - NotasCredito_PeriodoActual
  Ej: $100,000,000 - $5,000,000 = $95,000,000

Paso 3: Descuenta Nota Crédito del período ANTERIOR
  NC_PeríodoAnterior = Obtiene de Historial (reconciliada - valor REAL)
  Ej: $1,500,000
  ValorFacturadoFinal = ValorFacturadoNeto - NC_PeríodoAnterior
  Ej: $95,000,000 - $1,500,000 = $93,500,000

Paso 4: SUMA Órdenes de Compra Especiales (SOLO APROBADAS)
  OCEspeciales_Aprobadas = SUM(OC Especiales con status APROBADA)
  Ej: $2,000,000
  ValorBaseParaBono = ValorFacturadoFinal + OCEspeciales_Aprobadas
  Ej: $93,500,000 + $2,000,000 = $95,500,000

Paso 5: Busca Vigencia más reciente (fecha ? hoy, estado Activo)
  Vigencia del período en curso

Paso 6: Busca en qué TRAMO cae ValorBaseParaBono
  Tramos configurados:
    Tramo 1: $10M - $20M = 58%
    Tramo 2: $20M - $30M = 59%
    Tramo 3: $30M - $100M = 60%
    Tramo 4: >$100M = 61%

  ValorBaseParaBono = $95,500,000 ? Cae en Tramo 3 (30M-100M) ? 60%

Paso 7: Aplica porcentaje
  Bono = ValorBaseParaBono * Porcentaje
  Bono = $95,500,000 * 0.60 = $57,300,000

Paso 8: Genera Nota Crédito para siguiente período
  Distribuidor: DIST-001
  Valor: $57,300,000
  Para aplicar en: Siguiente período
```

**EJEMPLO COMPLETO - CON OC ESPECIALES**:

```
PERÍODO: 1 AL 15 DE ENERO
???????????????????????????????????????????????????????????????????

DISTRIBUIDOR: DIST-001

INSUMO 1: Valor Facturado (de TOTUS)
  Valor Bruto: $100,000,000

INSUMO 2: Notas Crédito del Período (de TOTUS)
  Total NC: $5,000,000
  Valor Neto: $95,000,000

INSUMO 3: NC Período Anterior (de Historial)
  NC_Enero (ya reconciliada): $1,500,000
  Valor después descuento: $93,500,000

INSUMO 4: Órdenes de Compra Especiales (Ingreso Manual USUARIO PROMOS)
  OC Especial 1: $1,000,000 ? Status: APROBADA ?
  OC Especial 2: $500,000 ? Status: APROBADA ?
  OC Especial 3: $1,000,000 ? Status: PENDIENTE APROBACIÓN ? (no cuenta)
  Total OC Aprobadas: $1,500,000

CÁLCULO FINAL:
  Base = $93,500,000 + $1,500,000 = $95,000,000
  Vigencia: 60%
  Bono = $95,000,000 × 0.60 = $57,000,000
```

**DIFERENCIAS CON OTROS BONOS**:

```
????????????????????????????????????????????????????????????????
? BONO POR FACTURACIÓN (CON OC ESPECIALES)                     ?
????????????????????????????????????????????????????????????????
? Base: Facturado + OC Especiales                              ?
? OC Especiales: SÍ, aplican (ingreso manual)                  ?
? Requiere aprobación: SÍ (si excede límite)                   ?
? Responsable ingreso: Usuario PROMOS                          ?
????????????????????????????????????????????????????????????????

????????????????????????????????????????????????????????????????
? BONO POR PEDIDO                                              ?
????????????????????????????????????????????????????????????????
? Base: Órdenes de Aldebaran + Precios Históricos              ?
? OC Especiales: NO, NO aplican                                ?
? Requiere aprobación: NO                                      ?
? Responsable: Sistema (cálculo automático)                    ?
????????????????????????????????????????????????????????????????

????????????????????????????????????????????????????????????????
? BONO POR ENTREGADO                                           ?
????????????????????????????????????????????????????????????????
? Base: Entregas Confirmadas + Precios Históricos              ?
? OC Especiales: NO, NO aplican                                ?
? Requiere aprobación: NO                                      ?
? Responsable: Sistema (cálculo automático)                    ?
????????????????????????????????????????????????????????????????
```

**ESTRATEGIA MOCK**:
```
appSettings:
  "ProcedimientoAlmacenadoTOTUS": "sp_ObtenerFacturacion"
  "UsarMockTOTUS": true (durante desarrollo)
  "ValorMockFacturado": 100000000
  "TotalMockNC": 5000000
  "TotalMockNCAnterior": 1500000

Si UsarMockTOTUS = true:
  Retorna valores de prueba configurables
  Permite testing sin dependencia de TOTUS real
```

### 1.7.2 VALOR PEDIDO (De Órdenes Aldebaran + Histórico de Precios)

**ARQUITECTURA DE PRECIOS**:
```
Aldebaran NO almacena precios de artículos en tabla Órdenes/Pedidos
Los precios vienen del histórico de precios cargado desde Página Promocional

FLUJO:
  1. Página Promocional envía lista de precios (diariamente)
  2. Se carga en Aldebaran: Tabla PreciosDistribuidor (actual)
  3. Se guarda histórico en: Tabla PreciosDistribuidorHistorico (4 meses)
  4. Al crear un PEDIDO:
     ?? Solo se registra: Referencia + Cantidad
     ?? El precio se obtiene DEL HISTÓRICO DE PRECIOS (MÁS RECIENTE)
     ?? Ese precio se CONGELA en el pedido para auditoría/futuras entregas
  5. Para cálculo de bonos:
     ?? Se usan precios del histórico (el que estaba activo ese día)
     ?? Garantiza precisión: incluso si precios cambian después
```

**FUENTES DE DATOS**:
```
Fuente 1: ÓRDENES EN ALDEBARAN (por período)
  - Referencia del artículo
  - Cantidad pedida
  - Fecha de la orden
  - (NO tiene precio - viene del histórico)

Fuente 2: HISTÓRICO DE PRECIOS (cargado desde Página Promocional)
  - Tabla: PreciosDistribuidorHistorico
  - Estructura: Referencia, PrecioUnitario, DescuentoDistribuidor, FechaCarga
  - Contiene: Histórico configurable (ADMINISTRADOR define período de retención)
  - Se usa para recuperar el precio vigente EN EL DÍA DEL PEDIDO

CONFIGURACIÓN DE RETENCIÓN DE PRECIOS (Política de Administrador):
  - parámetro appSettings: "HistorialPreciosRetentionDays" (ej: 120 días = 4 meses)
  - Administrador decide cuánto tiempo mantener el histórico
  - Limpieza automática: Scheduled Job elimina precios más antiguos
  - Propósito: Balance entre auditoría (más días = mejor) y espacio BD (menos días = menor)
```

**CÁLCULO DEL VALOR PEDIDO - PASO A PASO**:
```
Para cada orden del período:

  1. Obtiene de Orden: Referencia + Cantidad + FechaPedido
     Ej: REF-001, 100 unidades, Día 5 del período

  2. Busca en HISTÓRICO DE PRECIOS el precio vigente ESE DÍA
     Referencia = REF-001
     FechaCarga <= FechaPedido
     Selecciona el precio MÁS RECIENTE antes de esa fecha

  3. Obtiene Precio y Descuento de ese día
     PrecioUnitario = $20 (del día 5)
     DescuentoDistribuidor = 10 porciento

  4. Calcula precio con descuento
     PrecioConDescuento = PrecioUnitario * (1 - DescuentoDistribuidor)
     PrecioConDescuento = $20 * (1 - 0.10) = $18

  5. Calcula valor de la orden
     Cantidad = 100 unidades
     ValorOrden = Cantidad * PrecioConDescuento
     ValorOrden = 100 * $18 = $1,800

  6. ACUMULA para el período completo
     ValorTotalPedido = SUM(todas las órdenes del período)
     Ej: $1,800,000

CRÍTICO: Se usa el precio del HISTÓRICO del día del pedido, NO el precio actual
```

**ESTRATEGIA DE PRECIOS APLICABLES - CONFIGURABLE POR TIPO DE BONO**:

```
Los precios pueden VARIAR DIARIAMENTE desde Página Promocional.
Por esto, hay 4 OPCIONES de cómo aplicar el precio al calcular bonos:

???????????????????????????????????????????????????????????????????????
? OPCIÓN 1: PRECIO A FECHA DEL PEDIDO (MÁS COMÚN)                   ?
???????????????????????????????????????????????????????????????????????
? • Se usa: Precio vigente el DÍA que se creó el pedido              ?
? • Se congela: No cambia aunque el precio varíe después             ?
? • Ventaja: Refleja precio exacto que distribuidor vio             ?
? • Ventaja: Inmune a cambios posteriores de precio                 ?
? • Auditoría: Fácil trazar qué precio se usó                       ?
? • Uso recomendado: BONO POR PEDIDO (ambos usan fecha del pedido)  ?
?                     BONO POR ENTREGADO (precio original congelado) ?
?                                                                      ?
? Ejemplo:                                                             ?
?   Pedido Día 5: 100 unidades, Precio $20                           ?
?   Día 8 Entrega: Precio actual es $18 (pero usa $20 del día 5)     ?
?   Bono: 100 × $20 = $2,000 (NO 100 × $18)                          ?
???????????????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????????????
? OPCIÓN 2: PRECIO A FECHA DEL DÍA (HOY - MÁS ACTUAL)               ?
???????????????????????????????????????????????????????????????????????
? • Se usa: Precio vigente HOY (más reciente cargado)               ?
? • Se actualiza: Refleja cambios de precio en tiempo real           ?
? • Ventaja: Siempre usa precio más actual                          ?
? • Desventaja: Bono puede cambiar por variación de precio          ?
? • Auditoría: Compleja (qué precio se usó?)                        ?
? • Uso: Consultas dinámicas que necesitan precio más actualizado   ?
?                                                                      ?
? Ejemplo:                                                             ?
?   Pedido Día 5: 100 unidades (precio era $20 ese día)             ?
?   Consulta Día 8: Precio actual es $18                            ?
?   Bono: 100 × $18 = $1,800 (cambió porque precio bajó)            ?
???????????????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????????????
? OPCIÓN 3: PRECIO A FECHA DE ENTREGA                               ?
???????????????????????????????????????????????????????????????????????
? • Se usa: Precio vigente el DÍA de la entrega (cuando salió almacén)
? • Diferente: Al precio del pedido si pasaron N días               ?
? • Ventaja: Refleja precio vigente cuando se despachó              ?
? • Desventaja: Distinto al precio que distribuidor vio             ?
? • Auditoría: Moderada (necesita buscar precio de ese día)         ?
? • Uso: BONO POR ENTREGADO (alternativa a fecha pedido)            ?
?                                                                      ?
? Ejemplo:                                                             ?
?   Pedido Día 5: 100 unidades, Precio $20                          ?
?   Entrega Día 8: Precio ese día es $18                            ?
?   Bono: 100 × $18 = $1,800 (NO 100 × $20)                         ?
???????????????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????????????
? OPCIÓN 4: PRECIO AL PROMEDIO DEL PERÍODO                          ?
???????????????????????????????????????????????????????????????????????
? • Se usa: Promedio de precios de TODOS los días del período       ?
? • Cálculo: SUM(precios todos los días) / Cantidad de días          ?
? • Ventaja: Normaliza variaciones diarias (más justo)              ?
? • Ventaja: Mismo precio para todas las cantidades del período      ?
? • Desventaja: Más complejo de entender                            ?
? • Auditoría: Difícil trazar "por qué ese precio"                  ?
? • Uso: Cuando se quiere estabilizar precio variable               ?
?                                                                      ?
? Ejemplo:                                                             ?
?   Período 1-15 días:                                               ?
?   Precios cargados: Día 1=$20, Día 2=$20, Día 3=$21, ... Día 15=$18
?   Promedio: ($20+$20+$21+...+$18) / 15 = $19.33                    ?
?   Bono: 100 × $19.33 = $1,933 (MISMO para todo el período)        ?
???????????????????????????????????????????????????????????????????????
```

**CONFIGURACIÓN DE ESTRATEGIA DE PRECIOS (Por Administrador)**:

```
El Administrador debe poder definir para cada Tipo de Bono:

BONO POR PEDIDO
?? Afectación: Basado en cantidad PEDIDA
?? Estrategia de Precio (seleccionar una opción):
?  ?? Precio del día en que se hizo el pedido (RECOMENDADO)
?  ?? Precio de hoy (más reciente)
?  ?? Precio del día en que se entregó
?  ?? Precio promedio del período
?? Se aplica automáticamente en todos los cálculos

BONO POR ENTREGADO
?? Afectación: Basado en cantidad EFECTIVAMENTE ENTREGADA
?? Estrategia de Precio (seleccionar una opción):
?  ?? Precio del día en que se hizo el pedido (RECOMENDADO)
?  ?? Precio de hoy (más reciente)
?  ?? Precio del día en que se entregó
?  ?? Precio promedio del período
?? Se aplica automáticamente en todos los cálculos

BONO POR FACTURACIÓN
?? Afectación: Basado en valor FACTURADO (de TOTUS)
?? Estrategia de Precio: No aplica (facturación viene de TOTUS)

REQUISITOS FUNCIONALES:
? Administrador elige estrategia para cada tipo de bono
? La estrategia se aplica automáticamente sin intervención
? Sistema registra en auditoría cuál estrategia se usó
? Sistema documenta el precio que se aplicó en cada cálculo
? Cambios en estrategia solo afectan cálculos futuros
```

**IMPACTO DE CADA ESTRATEGIA DE PRECIOS**:

```
Ejemplo: 100 unidades pedidas el Día 5 a $20, entregadas el Día 8 con precio actual $18

Estrategia 1: Precio día del pedido
?? Bono = 100 × $20 (precio del día 5) = $2,000

Estrategia 2: Precio de hoy
?? Bono = 100 × $18 (precio actual) = $1,800

Estrategia 3: Precio día entrega
?? Bono = 100 × $18 (precio del día 8) = $1,800

Estrategia 4: Precio promedio período
?? Bono = 100 × $19.33 (promedio de todos los días) = $1,933

CADA ESTRATEGIA GENERA DIFERENTES RESULTADOS
```

**POLÍTICA DE RETENCIÓN DE HISTÓRICO DE PRECIOS - CONFIGURABLE**:

```
El Administrador debe poder definir:

1. CUÁNTO TIEMPO MANTENER EL HISTÓRICO DE PRECIOS
   - Opciones: 60 días, 90 días, 120 días (default), 180 días, 365 días
   - El valor se configura según necesidad del negocio
   - Ejemplos de uso:
     * 60 días: Para distribuidores con ciclos cortos
     * 120 días: Balance recomendado (auditar 4 períodos)
     * 365 días: Para requisitos legales/compliance rigurosos

2. CUÁNDO EJECUTAR LA LIMPIEZA DE PRECIOS ANTIGUOS
   - Horario configurable (ej: 2 AM, 4 AM, etc.)
   - Se recomienda horario de bajo uso para no afectar operaciones
   - Default: 2 AM

3. CON QUÉ CADENCIA EJECUTAR LA LIMPIEZA
   - Frecuencia configurable: Diariamente, Semanalmente, Mensualmente
   - Default: Diariamente (más seguro)

4. HORARIO DE CARGA AUTOMÁTICA DE PRECIOS
   - Horario configurable (ej: 6 AM)
   - Reintentos configurables si falla (ej: máximo 3 intentos)
   - Espera entre reintentos configurable (ej: 5 minutos)
   - Si falla: Sistema continúa usando precios del día anterior (NO interrumpe)
   - Notificación automática a administrador si hay error

PROTECCIONES GARANTIZADAS:
   ? Nunca borra precios del período actual (en uso)
   ? Nunca borra precios de períodos no cerrados
   ? Registra auditoría: Qué se borró, cuándo, por qué
   ? Mantiene backup antes de borrar (recuperación disponible)
```

**IMPACTO EN EL NEGOCIO**:

```
? Administrador tiene CONTROL TOTAL sobre retención de datos
   • Ajusta según auditoría/compliance requerida
   • Optimize espacio/recursos según necesidad
   • Configura sin intervención técnica

? Continuidad operativa GARANTIZADA
   • Horario configurable no interfiere con operaciones
   • Reintentos automáticos si descarga falla
   • Fallover a precios anteriores (sin interrupciones)

? Seguridad y auditoría INTEGRADA
   • Datos críticos nunca se pierden
   • Todo lo borrado queda registrado
   • Recuperación disponible si es necesario
```

**EJEMPLO COMPLETO - PERÍODO 15 DÍAS**:
```
Orden 1: REF-001, Cant: 100, Precio: $20, Desc: 10% ? $1,800
Orden 2: REF-002, Cant: 50, Precio: $100, Desc: 15% ? $4,250
Orden 3: REF-001, Cant: 200, Precio: $20, Desc: 10% ? $3,600
Orden 4: REF-003, Cant: 1000, Precio: $5, Desc: 5% ? $4,750

Total período = $1,800 + $4,250 + $3,600 + $4,750 = $14,400
```

**CÁLCULO DEL BONO POR PEDIDO**:
```
Paso 1: Calcula acumulado del período
  ValorTotalPedido = $14,400,000

Paso 2: Busca Vigencia más reciente (fecha ? hoy)
  Vigencia del período actual

Paso 3: Busca TRAMO que contiene ValorTotalPedido
  Tramos (configurables):
    Tramo 1: $1M - $5M = 5 porciento
    Tramo 2: $5M - $10M = 6 porciento
    Tramo 3: $10M - $20M = 7 porciento
    Tramo 4: >$20M = 8 porciento

  ValorTotalPedido = $14,400,000 ? Tramo 3 ? 7 porciento

Paso 4: Aplica porcentaje
  Bono = ValorTotalPedido * Porcentaje
  Bono = $14,400,000 * 0.07 = $1,008,000

Paso 5: Genera Nota Crédito para siguiente período
  Distribuidor: DIST-001
  Valor: $1,008,000
  Para aplicar en: Siguiente período
```

**DÓNDE SE USA LISTA DE PRECIOS**:
```
Tabla: PreciosDistribuidor (se reemplaza diariamente)
  Referencia, PrecioUnitario, DescuentoDistribuidor, FechaCarga

Tabla: PreciosDistribuidorHistorico (histórico 4 meses)
  Mismos campos + información de auditoría

Propósito:
  Auditoría: Qué precios se usaron en cada cálculo
  Reclamos: Si distribuidor reclama, ver qué lista se usó
```

**PROCESO AUTOMÁTICO DE CARGA (RF6)**:
```
Diariamente (horario a definir):
  1. Conecta a Página Promocional (protocolo a definir)
  2. Descarga archivo Excel
  3. Procesa:
     - Valida estructura (debe tener 3 columnas)
     - Valida datos (precios > 0, descuentos 0-100%)
  4. Carga a BD:
     - Borra PreciosDistribuidor anterior
     - Inserta nuevos precios
     - Copia a PreciosDistribuidorHistorico (con fecha)
  5. Auditoría: Registra fecha, cantidad registros, resultado
  6. Alertas:
     - Si éxito: Log "Precios cargados OK"
     - Si fallo: Alerta admin, conserva precios anteriores

Si falla durante período:
  Usa precios del día anterior
  Calcula bonos con esos precios
  NO interrumpe la operación
```

### 1.7.3 VALOR ENTREGADO (De Entregas/Salidas de Aldebaran)

**Fuente**: Entregas/Salidas de almacén confirmadas en Aldebaran

**Proceso Real de PROMOS**:
```
1. Distribuidor crea PEDIDO (Orden de Compra)
   ?? Registra: Artículo, Cantidad pedida, Precio unitario (del día)
   ?? Almacena: Precio histórico del pedido (se congela en ese momento)
   ?? Estado: PEDIDO CREADO

2. PROMOS prepara en almacén (puede tomar 1, 3, 8 o N días)
   ?? Separa mercancía
   ?? Empaca

3. PROMOS realiza ENTREGA/SALIDA
   ?? Usuario de PROMOS ingresa a Aldebaran
   ?? Marca en el Pedido: Cantidad entregada (puede ser parcial o total)
   ?? Registra: Cantidad que realmente salió del almacén
   ?? Genera: Guía de remisión/Documento de salida
   ?? Usa: Precio histórico del pedido (el que se registró hace N días)
   ?? Estado: ENTREGA CONFIRMADA (cantidad entregada registrada)

4. Valuación de lo entregado (CRÍTICO)
   ?? Cantidad entregada × Precio unitario del MOMENTO DEL PEDIDO
   ?? NO se usa el precio actual de hoy
   ?? Se usa el precio que existía cuando se creó el pedido
```

**Cálculo**:
```
Para cada Pedido con Entrega confirmada en el período:
  Valor = Cantidad entregada × Precio unitario DEL PEDIDO (histórico)

ValorTotalEntregado = SUM(Valor de todas entregas confirmadas en período)

EJEMPLO TEMPORAL:
  Día 1 (Creación del Pedido):
    Pedido 1: 100 unidades solicitadas
    Precio en BD el día 1: $20 c/u (se guarda este precio)
    Estado: PEDIDO CREADO

  Día 8 (Entrega):
    Precio actual en BD el día 8: $18 c/u (pero NO se usa este)
    Usuario confirma entrega: 80 unidades entregadas
    Valor a contar: 80 × $20 (precio del día 1) = $1,600
    Estado: ENTREGA CONFIRMADA

  Pedido 2:
    Día 2: 50 unidades pedidas a $100 c/u
    Día 9: 50 unidades entregadas (entrega completa)
    Valor: 50 × $100 (precio del día 2) = $5,000

  Total período = $1,600 + $5,000 = $6,600

Se usa para Tipo de Bono con Afectación = "Entregado"

IMPORTANTE - REGLAS CRÍTICAS:
  • Solo se cuenta LO EFECTIVAMENTE ENTREGADO (cantidad confirmada en Aldebaran)
  • NO se cuenta lo que fue PEDIDO pero no entregado
  • El precio aplicado es el HISTÓRICO del pedido (congelado en el momento del pedido)
  • NO se usa el precio actual del día de la entrega
  • Si hay entregas parciales en múltiples ocasiones, se acumula cada parte con su precio original
  • Esto asegura que el bono refleja el VALOR REAL DEL PEDIDO, no variaciones de precio posteriores
```

**Diferencia con Bonificación por Pedido**:
```
BONO POR PEDIDO:
  ?? Usa: Cantidad PEDIDA (aunque no se haya entregado)
  ?? Usa: Precio del día del pedido
  ?? Incentiva: Que el distribuidor pida más
  ?? Se calcula: Apenas se crea el pedido

BONO POR ENTREGADO:
  ?? Usa: Cantidad EFECTIVAMENTE ENTREGADA (confirmada en Aldebaran)
  ?? Usa: Precio del día del pedido (congelado, no actual)
  ?? Incentiva: Que el distribuidor reciba/confirme lo que pidió
  ?? Se calcula: Cuando se confirma la entrega
```

**ALMACENAMIENTO DE PRECIO HISTÓRICO**:
```
Tabla: PreciosDistribuidorHistorico
  - Referencia del artículo
  - PrecioUnitario (precio base ese día)
  - DescuentoDistribuidor (descuento ese día)
  - FechaCarga (cuándo se cargó este precio)
  - Se retiene: 4 meses de histórico (para auditoría)

Cuando se crea un PEDIDO:
  - Se busca precio vigente en PreciosDistribuidorHistorico (más reciente antes de esa fecha)
  - Se congela ese precio (referencia para futuras entregas)
  - Se almacena para auditoría (qué precio se usó ese día)

Cuando se confirma ENTREGA:
  - Se usa el precio congelado del pedido (el que se buscó el día de creación)
  - NO se busca precio actual (aunque haya cambiado)
  - Garantiza que bono refleja VALOR REAL del pedido original

El histórico de precios es la CLAVE para precisión auditada
```

### 1.7.4 NOTA CRÉDITO DEL PERÍODO ANTERIOR

**Obtención**:
```
De: Tabla HistorialBono del período anterior
Estado = "Aplicado" (reconciliado)
Valor: ValorReal (no el calculado, sino el que TOTUS realmente aplicó)

Uso en cálculo Bono por Facturación:
  ValorNetoBono = ValorFacturado - NC_PeríodoAnterior
```

**CRÍTICO PARA PRECISIÓN**:
```
Sin descontar NC anterior = DOBLE CONTEO de bonos
Ejemplo del desastre:
  Enero: Facturado $100M ? Bono $60M (NC Enero)
  Febrero: Facturado $95M
    SI NO descuenta NC Enero: Calcularía bono sobre $95M (INCORRECTO)
    SI descuenta NC Enero: Calcula sobre $95M - $60M = $35M (CORRECTO)
```

---

## 1.10 NUEVOS REQUISITOS FUNCIONALES - Usuario PROMOS (Complemento a RF1-RF15)

### Descripción General

El sistema permite al **Usuario PROMOS** ingresar manualmente datos que impactan el cálculo de bonos:

1. **Órdenes de Compra Especiales (OC Especiales)** ? Se SUMAN solo al Bono por Facturación
   - Órdenes que PROMOS conoce pero NO están en Aldebaran/TOTUS
   - Ejemplos: Ajustes comerciales, reembolsos, pedidos especiales

2. **Reconciliación de Notas Crédito** ? Confirma NC real aplicada en TOTUS
   - Usuario ingresa lo que TOTUS realmente aplicó
   - Sistema compara con lo calculado y detecta discrepancias

Todos soportan:
- ? Ingreso UNITARIO (uno a uno)
- ? Ingreso MASIVO (CSV)
- ? Aprobaciones configurables por límite
- ? Auditoría completa e inmutable

### ¿Dónde aplican las OC Especiales?

```
? BONO POR FACTURACIÓN (SOLO)
   Base = Facturado - NC_Período - NC_Anterior + OC_Especiales_Aprobadas
   Bono = Base × % Vigencia

? BONO POR PEDIDO (NO)
? BONO POR ENTREGADO (NO)
```

### Ejemplo de Impacto Financiero

```
Período: 1-15 Enero | Distribuidor: DIST-001 | Vigencia: 60%

SIN OC Especiales:
  Base: $100M - $5M - $1.5M = $93.5M
  Bono: $56.1M

CON OC Especiales ($2M Aprobadas):
  Base: $100M - $5M - $1.5M + $2M = $95.5M
  Bono: $57.3M
  Diferencia: +$1.2M
```

| RF | Descripción | Tipo |
|----|---|---|
| RF16-A | **Ingreso Manual de OC Especiales (Unitario)** | Manual |
| RF16-B | **Carga Masiva de OC Especiales (CSV)** | Masivo |
| RF17 | **Gestión de Aprobaciones para Ingresos Manuales** | Control |
| RF18 | Reconciliación Manual de NC (Unitario + CSV) | Manual |
| RF19 | Reporte: Bonos Calculados vs Bonos Aplicados | Reportería |
| RF20 | Reporte: Distribuidores que Consultaron Bonos | Reportería |
| RF21 | Reporte: Discrepancias de NC | Reportería |
| RF22 | Reporte: Discrepancias de NC (Calculada vs Real) |
| RF23 | Reporte: Auditoría de Acciones del Usuario PROMOS |
| RF24 | Reporte: Precios y Vigencias Usados en Período |
| RF25 | Reporte: Ingresos Manuales Aplicados (OC Especiales + Reconciliaciones) |
| RF26 | Exportación de Reportes (Excel/PDF) |

---

### RF16-A: Ingreso Manual de Órdenes de Compra Especiales (Unitario)

**Justificación:**
- Órdenes de Compra que NO vienen de Aldebaran ni de TOTUS
- PROMOS conoce el valor total (es el único insumo disponible)
- SOLO aplican al Bono por Facturación (no a Pedido ni Entregado)
- Usuario PROMOS ingresa valor total, no por artículo
- Requiere aprobación si excede límite configurado

**Funcionalidad:**
```
Usuario PROMOS debe poder:

1. Acceder a: "Órdenes de Compra Especiales"
2. Opción: "Agregar OC Especial Manual"
3. Seleccionar: Distribuidor + Período
4. Ingresar:
   - Número OC (identificador único)
   - Valor Total (campo numérico - monto completo)
   - Descripción (campo texto - opcional)
   - Fecha OC (fecha cuando se generó la OC)
5. Sistema valida:
   - Distribuidor existe
   - Período existe
   - Valor > 0
   - Fecha válida y dentro del período
6. Sistema registra:
   - Valor ingresado
   - Quién lo ingresó
   - Cuándo se ingresó
   - Estado: PENDIENTE APROBACIÓN (si excede límite)
   - Estado: APROBADA (si está dentro del límite automático)
   - Auditoría completa

7. ESTE VALOR se SUMA al Bono por Facturación:
   Base = Facturado - NC_Período - NC_Anterior + OC_Especiales_Aprobadas
   Bono = Base × % Vigencia

8. El bono dinámico del distribuidor incluye automáticamente
   este valor EN LA PRÓXIMA CONSULTA (si está APROBADO)
```

**Restricción Importante:**
```
? OC Especiales NO aplican a:
  - Bono por Pedido
  - Bono por Entregado

? OC Especiales SOLO aplican a:
  - Bono por Facturación
```

---

### RF16-B: Carga Masiva de Órdenes de Compra Especiales (CSV)

**Justificación:**
- Permite ingreso rápido de múltiples OC Especiales
- Evita ingreso manual uno a uno (muy dispendioso)
- Validación de estructura y datos
- Posibilidad de corrección antes de aplicar

**Funcionalidad:**
```
Usuario PROMOS debe poder:

1. Acceder a: "Órdenes de Compra Especiales"
2. Opción: "Cargar OC Especiales (CSV Masivo)"
3. Descarga plantilla CSV con estructura:
   ??????????????????????????????????????????????????????????
   Distribuidor | Período | NumeroOC | ValorTotal | Descripción | Fecha
   DIST-001     | ENE2026 | OC-1001  | 1000000    | Especial    | 2026-01-10
   DIST-001     | ENE2026 | OC-1002  | 500000     | Especial    | 2026-01-12
   DIST-002     | ENE2026 | OC-2001  | 2000000    | Especial    | 2026-01-15
   ??????????????????????????????????????????????????????????

4. Usuario completa el archivo CSV
5. Usuario carga archivo
6. Sistema valida:
   - Estructura correcta (columnas requeridas)
   - Distribuidor existe
   - Período existe
   - Valores > 0
   - Fechas válidas y dentro del período
   - NumeroOC único (no duplicado en BD)

7. Muestra RESUMEN antes de aplicar:
   ?? Total registros en archivo: XXX
   ?? Registros válidos: YYY
   ?? Registros con error: ZZZ
   ?? Valor total a agregar: $AAAA
   ?? Registros que requieren aprobación: BBB
   ?? Registros aprobados automáticamente: CCC
   ?? Botones: [Confirmar] [Revisar Errores] [Cancelar]

8. Si hay errores:
   - Descarga reporte de errores (CSV)
   - Usuario corrige en el archivo
   - Recarga

9. Si confirma:
   - Carga todos los registros válidos
   - Estado: PENDIENTE APROBACIÓN (si excede límite)
   - Estado: APROBADA (si automático)
   - Notificación a aprobador (si hay pendientes)
   - Registra auditoría: quién, qué, cuándo, cuántos, detalles

10. Los valores se SUMAN al Bono por Facturación
    (solo los que están APROBADOS)
```

---

### RF17: Aplicación Manual de NC en TOTUS (con Confirmación)

**Justificación:**
- Usuario PROMOS es responsable de aplicar NC en TOTUS
- Sistema debe registrar qué NC se aplicó realmente
- Auditoría debe dejar constancia de la acción

**Funcionalidad:**
```
Usuario PROMOS debe poder:

1. Acceder a: "NCs Pendientes de Aplicar"
2. Ver lista de NCs recomendadas (estado RECOMENDADA)
3. Por cada NC:
   - Ver: Distribuidor, Monto recomendado, Período aplicación
   - Decidir: Aplicar como está, Aplicar monto diferente, o Rechazar
4. Si decide aplicar:
   - Abre TOTUS manualmente
   - Aplica NC en TOTUS
   - Regresa a Aldebaran
5. Ingresa en el sistema:
   - Monto que realmente aplicó en TOTUS
   - Estado: "APLICADA" (con confirmación)
6. Sistema registra:
   - NC calculada vs NC aplicada
   - Si hay diferencia: Alerta y registra discrepancia
   - Auditoría: quién, qué, cuándo, cuánto
```

---

### RF18: Reconciliación Manual de NC (Unitario + CSV Masivo)

**Justificación:**
- NO es automática (requiere confirmación del Usuario PROMOS)
- Usuario confirma cuándo terminó de aplicar todas las NCs en TOTUS
- Permite auditar discrepancias entre calculado y aplicado
- Debe permitir ingreso masivo (CSV) para velocidad

**Funcionalidad:**
```
PARTE 1: UNITARIO (Manual)
?????????????????????????????????

Usuario PROMOS debe poder:

1. Acceder a: "Reconciliación de NC"
2. Seleccionar: Período ANTERIOR (cerrado)
3. Para CADA distribuidor:
   - Muestra: NC Calculada en Aldebaran
   - Muestra: NC Aplicada (si la ingresó en RF17)
   - Campo editable: Monto NC REAL que TOTUS aplicó
   - Campo: Fecha en que se confirmó en TOTUS

4. Usuario ingresa valor real
5. Sistema compara:
   ?? Si coincide: Status "RECONCILIADO" ?
   ?? Si NO coincide: Status "DISCREPANCIA" ??
   ?  ?? Muestra: Calculada vs Real vs Diferencia
   ?? Si hay aprobación pendiente: Status "REQUIERE APROBACIÓN"

6. Usuario puede:
   ?? Guardar cambio individual
   ?? Pasar al siguiente distribuidor
   ?? O cargar masivamente (CSV)

PARTE 2: MASIVO (CSV)
?????????????????????????????????

1. Opción: "Cargar Reconciliación (CSV Masivo)"
2. Descarga plantilla CSV:
   ??????????????????????????????????????????????????????????????
   Distribuidor | Período | NCCalculada | NCReal | Fecha | Motivo
   DIST-001     | ENE2026 | 1000000     | 950000 | 2026-02-01 | Parcial
   DIST-002     | ENE2026 | 500000      | 500000 | 2026-02-02 | OK
   DIST-003     | ENE2026 | 2000000     | 2000000 | 2026-02-03 | OK
   ??????????????????????????????????????????????????????????????

3. Usuario completa el archivo CSV
4. Usuario carga archivo
5. Sistema valida:
   - Estructura correcta
   - Distribuidor existe
   - Período existe
   - Valores numéricos válidos

6. Muestra resumen:
   ?? Total registros: XXX
   ?? Registros válidos: YYY
   ?? Registros con error: ZZZ
   ?? Reconciliados (coinciden): AAA
   ?? Discrepancias (no coinciden): BBB
   ?? Botones: [Confirmar] [Revisar Errores] [Cancelar]

7. Si hay discrepancias:
   - Muestra TODAS las diferencias lado a lado
   - Permite editar valores en la pantalla
   - O descargar reporte, corregir en CSV y recargar

8. Si confirma:
   - Carga todos los registros
   - Actualiza estado: RECONCILIADO (coincide)
   - Mantiene: DISCREPANCIA (no coincide, requiere investigación)
   - Registra auditoría completa: quién, qué, cuándo, diferencias

9. Próximos cálculos de bonos usan NC REAL (reconciliada)
   NO la calculada
```

---

### RF19: Gestión de Aprobaciones para Ingresos Manuales

**Justificación:**
- Ciertos ingresos requieren autorización (control financiero)
- Límites configurables por rol/usuario
- Auditoría de quién aprobó qué y por qué
- Afecta directamente el cálculo de bonos (dinero)

**Funcionalidad:**
```
FLUJO DE APROBACIÓN:
????????????????????

USUARIO PROMOS (rol básico):
  ?? Ingresa OC Especiales (manual o CSV)
  ?? Ingresa Reconciliación NC (manual o CSV)
  ?? Montos menores a $XXX: Se aprueban automáticamente
  ?? Montos mayores a $XXX: REQUIEREN APROBACIÓN
  ?? Estado: PENDIENTE APROBACIÓN

APROBADOR (Admin o rol superior):
  ?? Accede a: "Aprobaciones Pendientes"
  ?? Ve lista de:
  ?  ?? OC Especiales pendientes (con monto total)
  ?  ?? Reconciliaciones con discrepancias (con diferencia)
  ?  ?? Ingresos manuales por revisar
  ?? Por cada registro:
  ?  ?? Ve: Distribuidor, Período, Monto, Quién lo ingresó
  ?  ?? Puede: Aprobar / Rechazar / Pedir más datos
  ?  ?? Comenta: Motivo de rechazo o aprobación
  ?? Acción genera auditoría:
     ?? Quién aprobó
     ?? Cuándo
     ?? Motivo
     ?? Resultado

LÍMITE DE APROBACIÓN (Configurable por Admin):
  ?? OC Especiales < $XXX: Aprobación automática
  ?? OC Especiales >= $XXX: Requiere aprobación humana
  ?? Reconciliación con diferencia < $XXX: Automática
  ?? Reconciliación con diferencia >= $XXX: Requiere aprobación
  ?? Admin configura estos límites en ADMINISTRACIÓN

IMPACTO EN CÁLCULO:
  ?? Si APROBADO: Se incluye en próximos cálculos de bono ?
  ?? Si RECHAZADO: No se incluye, se notifica a Usuario
  ?? Si PENDIENTE: Se EXCLUYE del cálculo hasta aprobación
  ?? Auditoría registra: quién, qué, cuándo, decisión, motivo

RESTRICCIONES:
  ? Usuario básico NO puede aprobar sus propios ingresos
  ? NO puede cambiar estado aprobado a rechazado (solo Admin)
  ? NO puede eliminar registros (solo marcar como rechazados)
```

---

### RF20: Reporte - Bonos Calculados vs Bonos Aplicados por Período

**Justificación:**
- Usuario PROMOS necesita visibilidad de qué se calculó vs qué se aplicó
- Detecta NCs aplicadas parcialmente o no aplicadas
- Auditoría de cumplimiento

**Funcionalidad:**
```
Reporte debe mostrar:

RESUMEN POR PERÍODO
?? Período: [1 al 15 de Enero]
?? Total Bonos Calculados: $X,XXX,XXX
?? Total Bonos Aplicados: $Y,XXX,XXX
?? Diferencia: $D,XXX,XXX (si diferencia > 0: ALERTA)
?? OC Especiales Aprobadas (incluidas en cálculo): $E,XXX,XXX
?? % Cumplimiento: ZZ%

DETALLE POR DISTRIBUIDOR
?? Distribuidor: DIST-001
?? Bono Facturación Calculado: $1,000,000
?? OC Especiales Incluidas: $200,000
?? Bono Facturación Aplicado: $950,000 ?? DIFERENCIA $250K
?? Bono Pedido Calculado: $500,000
?? Bono Pedido Aplicado: $500,000 ?
?? Bono Entregado Calculado: $250,000
?? Bono Entregado Aplicado: $0 ?? NO APLICADO
?? Causa: [Campo para justificación si aplica]

FILTROS Y OPCIONES
?? Período: [Dropdown]
?? Distribuidor: [Búsqueda]
?? Estado: Solo diferencias / Todos
?? Incluir OC Especiales: Sí/No
?? Exportar: Excel/PDF
```

---

### RF21: Reporte - Distribuidores que Consultaron Bonos (Log de Consultas)

**Justificación:**
- Auditoría: Quién consultó qué y cuándo
- Detecta distribuidores inactivos o muy activos
- Soporte: Qué información exacta se le mostró

**Funcionalidad:**
```
Reporte debe mostrar:

LISTADO DE CONSULTAS
?? Distribuidor: [Nombre]
?? Documento: [Cédula]
?? Fecha Consulta: [DD/MM/YYYY]
?? Hora Consulta: [HH:MM]
?? Bono Mostrado: $X,XXX,XXX
?? Incluye OC Especiales: Sí/No
?? Bono por Facturación: $A,XXX,XXX
?? Bono por Pedido: $B,XXX,XXX
?? Bono por Entregado: $C,XXX,XXX
?? IP Origen: [IP]
?? Período Consultado: [1 al 15 de Enero]
?? Estado: Exitosa / Error

FILTROS Y OPCIONES
?? Período: [Dropdown]
?? Rango Fechas: [Desde] a [Hasta]
?? Distribuidor: [Búsqueda]
?? Estado Consulta: Exitosa / Fallida / Todas
?? Rango de Bonos: [Desde] a [Hasta]
?? Mostrar solo con OC Especiales: Sí/No
?? Exportar: Excel/PDF

ESTADÍSTICAS
?? Total Consultas: XXX
?? Distribuidores Únicos: YYY
?? Consultas Exitosas: ZZ%
?? Consultas con OC Especiales: AA%
?? Promedio Bonos Consultados: $X,XXX,XXX
```

---

### RF22: Reporte - Discrepancias de NC (Calculada vs Real)

**Justificación:**
- Detecta problemas de aplicación de NCs
- Ayuda a reconciliar diferencias
- Auditoría de precisión

**Funcionalidad:**
```
Reporte debe mostrar:

DISCREPANCIAS ENCONTRADAS
?? Distribuidor: DIST-001
?? Período: Enero
?? NC Calculada: $1,000,000
?? NC Real (Ingresada): $950,000
?? Diferencia: -$50,000 (12% menos)
?? Causa Probable: [Según usuario/auditoría]
?? Fecha Detección: [DD/MM/YYYY]
?? Usuario que Ingresó: [Nombre]
?? Estado: Investigando / Resuelta / Justificada
?? Impacto en Próximo Período: Se usa $950K (real) en cálculos

FILTROS Y OPCIONES
?? Período: [Dropdown]
?? Tipo Discrepancia: Mayor / Menor / Todas
?? Rango Diferencia: [Desde] a [Hasta]
?? Estado: Todas / Investigando / Resueltas
?? Distribuidor: [Búsqueda]
?? Exportar: Excel/PDF

ANÁLISIS
?? Total Discrepancias: XXX
?? Discrepancias Mayores: YYY
?? Valor Total Diferencia: $Z,XXX,XXX
?? % Casos con Discrepancia: AA%
```

---

### RF23: Reporte - Auditoría de Acciones del Usuario PROMOS

**Justificación:**
- Rastrear todas las acciones del usuario
- Cumplimiento y responsabilidad
- Investigación de problemas

**Funcionalidad:**
```
Reporte debe mostrar:

HISTORIAL DE ACCIONES
?? Timestamp: [DD/MM/YYYY HH:MM:SS]
?? Acción: Ingresó OC Especial / Ingresó Reconciliación / Aprobó / Rechazó
?? Tipo Ingreso: Manual / Masivo (CSV)
?? Detalles:
?  ?? Distribuidor: [Si aplica]
?  ?? Período: [Si aplica]
?  ?? Monto: [Si aplica]
?  ?? Cantidad registros: [Si es masivo]
?  ?? Valor Anterior: [Si modificó]
?  ?? Valor Nuevo: [Si modificó]
?  ?? Motivo/Observación: [Si agregó nota]
?? Estado: Exitosa / Error
?? Resultado: [Si aplica]
?? Usuario que Realizó: [Nombre]

FILTROS Y OPCIONES
?? Rango Fechas: [Desde] a [Hasta]
?? Usuario PROMOS: [Si hay múltiples]
?? Tipo Acción: Todas / Ingreso / Aprobación / Reconciliación
?? Tipo Ingreso: Manual / Masivo / Todos
?? Período: [Dropdown]
?? Distribuidor: [Búsqueda]
?? Estado: Todas / Exitosas / Errores
?? Exportar: Excel/PDF

ESTADÍSTICAS
?? Total Acciones: XXX
?? Acciones por Usuario: [Desglose]
?? Acciones Exitosas: ZZ%
?? Acciones con Error: AA%
?? Ingresos Masivos (CSV): BB registros
?? Últimas Acciones: [Resumen]
```

---

### RF24: Reporte - Precios y Vigencias Usados en Período

**Justificación:**
- Auditoría de qué precios se usaron en cálculos
- Soporte a reclamaciones ("¿Por qué ese precio?")
- Validación de exactitud

**Funcionalidad:**
```
Reporte debe mostrar:

CONFIGURACIÓN USADA EN PERÍODO
?? Período: [1 al 15 de Enero]
?? Vigencia Aplicada: [Nombre]
?? Fecha Vigencia: [DD/MM/YYYY]
?? Estrategia Precios:
?  ?? Bono por Facturación: N/A + OC Especiales (Sí/No)
?  ?? Bono por Pedido: Precio día del pedido
?  ?? Bono por Entregado: Precio día del pedido
?? Tramos Configurados:
?  ?? Tramo 1: $1M - $5M = 5%
?  ?? Tramo 2: $5M - $10M = 6%
?  ?? Tramo 3: $10M - $20M = 7%
?  ?? Tramo 4: >$20M = 8%
?? Fecha Configuración: [Cuándo se configuró]

PRECIOS USADOS POR DISTRIBUIDOR
?? Distribuidor: DIST-001
?? Orden 1: REF-001, Cantidad 100, Precio $20 (Día 5)
?? Orden 2: REF-002, Cantidad 50, Precio $100 (Día 5)
?? Orden 3: REF-001, Cantidad 200, Precio $20 (Día 8)
?? Promedio Precios Usados: $X.XX
?? Fuente: [Qué lista de precios se usó]

LISTA DE PRECIOS USADA
?? Fecha Carga: [DD/MM/YYYY]
?? Cantidad de Artículos: XXX
?? Rango de Precios: $Y a $Z
?? Última Actualización: [Si hubo cambios]
?? Validación: OK / Advertencia

FILTROS Y OPCIONES
?? Período: [Dropdown]
?? Distribuidor: [Búsqueda]
?? Tipo Bono: Todos / Facturación / Pedido / Entregado
?? Exportar: Excel/PDF
```

---

### RF25: Reporte - Ingresos Manuales Aplicados (OC Especiales + Reconciliaciones)

**Justificación:**
- Visibilidad de todos los ingresos manuales realizados
- Auditoría de controles aplicados
- Historial de aprobaciones y cambios

**Funcionalidad:**
```
Reporte debe mostrar:

ÓRDENES DE COMPRA ESPECIALES INGRESADAS
?? Período: [1 al 15 de Enero]
?? Distribuidor: DIST-001
?? NumeroOC: OC-1001
?? Valor Total: $1,000,000
?? Descripción: [Especial]
?? Fecha OC: 2026-01-10
?? Tipo Ingreso: Manual / Masivo
?? Usuario que ingresó: [Nombre]
?? Estado: APROBADA / RECHAZADA / PENDIENTE APROBACIÓN
?? Usuario que aprobó: [Nombre] (si aplica)
?? Fecha aprobación: [DD/MM/YYYY] (si aplica)
?? Motivo aprobación: [Si hay]
?? Incluida en cálculo bono: Sí/No

RECONCILIACIONES DE NC INGRESADAS
?? Período: Enero (período reconciliado)
?? Distribuidor: DIST-001
?? NC Calculada: $1,000,000
?? NC Real Ingresada: $950,000
?? Diferencia: -$50,000
?? Fecha reconciliación: 2026-02-01
?? Usuario que ingresó: [Nombre]
?? Tipo Ingreso: Manual / Masivo
?? Estado: RECONCILIADO / DISCREPANCIA / PENDIENTE APROBACIÓN
?? Motivo: [Si hay]
?? Usada en próximos cálculos: Sí (valor real)

RESUMEN FINANCIERO
?? Total OC Especiales Aprobadas: $X,XXX,XXX
?? Total OC Especiales Rechazadas: $Y,XXX,XXX
?? Total OC Especiales Pendientes: $Z,XXX,XXX
?? Total Reconciliaciones con Discrepancia: $AAA,XXX,XXX
?? Valor Total Diferencias: $BBB,XXX,XXX
?? Impacto en bonos calculados: $CCC,XXX,XXX

FILTROS Y OPCIONES
?? Período: [Dropdown]
?? Tipo Ingreso: OC Especiales / Reconciliaciones / Ambas
?? Estado: Todas / Aprobadas / Rechazadas / Pendientes
?? Distribuidor: [Búsqueda]
?? Usuario: [Búsqueda]
?? Rango Fechas: [Desde] a [Hasta]
?? Exportar: Excel/PDF

ESTADÍSTICAS
?? Total Ingresos Manuales: XXX
?? Masivos (CSV): YYY
?? Unitarios: ZZZ
?? Aprobados: AA%
?? Rechazados: BB%
?? Pendientes: CC%
```

---

### RF26: Exportación de Reportes (Excel/PDF)

**Justificación:**
- Facilita auditoría externa
- Permite análisis adicional en Excel
- Distribución a otros departamentos

**Funcionalidad:**
```
Usuario PROMOS debe poder:

1. Después de cualquier reporte:
   - Botón: "Exportar como Excel"
   - Botón: "Exportar como PDF"

2. Excel:
   - Formato tabla: Con encabezados
   - Valores: Numéricos para cálculos
   - Fechas: Formato DD/MM/YYYY
   - Estilos: Encabezados destacados
   - Fórmulas: Habilitadas para análisis adicional

3. PDF:
   - Formato: Profesional / Imprimible
   - Incluye: Título, Período, Filtros aplicados
   - Incluye: Fecha/Hora generación + Usuario
   - Pie de página: Logo PROMOS + Confidencial
   - Resumen: Estadísticas principales

4. Validación:
   - Los datos exportados coinciden con pantalla
   - Incluye nota de: "Extraído de sistema Aldebaran"
   - Incluye: "Este reporte contiene información financiera confidencial"
```

---

### TOTUS (Sistema Facturación Externa)

DATOS RECIBIMOS de TOTUS:
- Valor Facturado por distribuidor/período (via SP)
- Total Notas Crédito
- Total Fletes, Total Descuentos

DATOS ENVIAMOS a TOTUS:
- Información de Nota Crédito generada por bono
- Distribuidor, Valor recomendado, Período aplicación

RECONCILIACIÓN:
- Obtener Nota Crédito REAL que se aplicó en período anterior
- PENDIENTE DEFINIR: Cómo identificar en TOTUS nuestra NC

### Página Promocional (Precios)

DATOS RECIBIMOS:
- Archivo Excel con lista de precios
- Estructura: Referencia, Precio Unitario, Descuento Distribuidor
- Descarga: Automática diaria

PENDIENTE DEFINIR:
- Protocolo descarga (URL, SFTP, API, etc.)
- Autenticación
- Horario
- Qué hacer si falla

---

## 1.9 Expectativas de Negocio

MÉTRICA DE ÉXITO:
- 100 porciento precisión en cálculos
- Consulta 500ms
- Disponibilidad 99 porciento
- Cero errores doble conteo NC

---

## RESUMEN

### CASOS DE USO PRINCIPALES (10 Total)

**Administración (CU1-CU3):**
- CU1: Crear Período
- CU2: Crear Tipo de Bono
- CU3: Crear Vigencia

**Seguridad (CU5):**
- CU5: Autenticar Distribuidor (OTP - SMS/Email)

**Operación (CU6-CU10):**
- CU6: Consultar Bono (Distribuidor - Página Promocional - Autenticado)
- CU7: Consultar Bono (Admin - Aldebaran.Web)
- CU8: Cierre de Período (Automático)
- CU10: Resolver Reclamación

**Integración de Datos (CU4):**
- CU4: Cargar Precios (Automático)

**Precisión (CU9):**
- CU9: Reconciliación NC (Manual - Usuario PROMOS)

### REQUISITOS FUNCIONALES (26 Total)

**Administración (RF1-RF3):**
- RF1: Gestionar Períodos
- RF2: Gestionar Tipos de Bono
- RF3: Gestionar Vigencias

**Seguridad (RF4-RF5, RF19):**
- RF4: Autenticar Distribuidor (OTP - SMS/Email)
- RF5: Validar Seguridad (Distribuidor solo ve su información)
- RF19: Gestión de Aprobaciones para Ingresos Manuales

**Consultas Dinámicas (RF6-RF7):**
- RF6: Consultar Bono (Distribuidor - Página Promocional)
- RF7: Consultar Bono (Admin - Aldebaran.Web)

**Historial y Gamificación (RF8-RF9):**
- RF8: Registrar Historial de Bonos (Auditoría completa)
- RF9: Gamificación (Mostrar falta para siguiente nivel)

**Integración de Datos (RF10-RF15):**
- RF10: Cargar Lista Precios Distribuidores
- RF11: Capturar Valor Facturado (TOTUS)
- RF12: Capturar Valor Pedido (Aldebaran + Precios)
- RF13: Capturar Valor Entregado (Aldebaran)
- RF14: Gestionar Nota Crédito Período Anterior
- RF15: Reconciliación Nota Crédito (TOTUS - Manual)

**Usuario PROMOS - Funciones Operacionales (RF16-RF18):**
- RF16-A: Ingreso Manual de OC Especiales (Unitario)
- RF16-B: Carga Masiva de OC Especiales (CSV)
- RF17: Aplicación Manual de NC en TOTUS (con Confirmación)
- RF18: Reconciliación Manual de NC (Unitario + CSV Masivo)

**Reportería (RF20-RF26):**
- RF20: Reporte - Bonos Calculados vs Bonos Aplicados por Período
- RF21: Reporte - Distribuidores que Consultaron Bonos (Log)
- RF22: Reporte - Discrepancias de NC (Calculada vs Real)
- RF23: Reporte - Auditoría de Acciones del Usuario PROMOS
- RF24: Reporte - Precios y Vigencias Usados en Período
- RF25: Reporte - Ingresos Manuales Aplicados (OC + Reconciliaciones)
- RF26: Exportación de Reportes (Excel/PDF)

### INSUMOS NECESARIOS
1. Valor Facturado (de TOTUS)
2. Valor Pedido (de órdenes + precios)
3. Valor Entregado (de entregas)
4. Nota Crédito Anterior (del historial)
5. Lista de Precios (diaria de página)
6. Información de contacto distribuidor: Email(s) + Celular

### CÁLCULO DEL BONO

**IMPORTANTE: Cálculo Dinámico (NO precalculado)**

El bono se calcula **en el momento de la consulta**, reflejando el acumulado desde el inicio del período hasta **HOY (fin del día actual)**.

```
Cuándo se calcula:
  ?? Cuando el distribuidor hace consulta en Sitio Público
  ?? NO se precalcula al crear pedido
  ?? NO se precalcula al confirmar entrega
  ?? Se calcula CADA VEZ que consulta

Rango de cálculo:
  ?? FechaInicio: Primer día del período actual
  ?? FechaFin: Hoy a las 23:59:59 (fin del día actual)
  ?? Ejemplo: Si período es 1-15 y hoy es 11, se cuenta del 1 al 11 (todo el día 11)

Pasos del cálculo:
  1. Autenticar distribuidor (OTP válido + Token activo)

  2. Obtener acumulado VALOR PEDIDO (1 al 11):
     - Suma todas órdenes del rango
     - Usa precios históricos del día de cada pedido
     - NO las facturadas, las PEDIDAS

  3. Obtener acumulado VALOR ENTREGADO (1 al 11):
     - Suma todas entregas confirmadas del rango
     - Usa precios congelados del pedido original
     - Solo lo EFECTIVAMENTE ENTREGADO

  4. Obtener acumulado VALOR FACTURADO (1 al 11):
     - Consulta TOTUS (tiempo real)
     - Parámetros: Documento distribuidor, período 1-11
     - Retorna facturación real del período

  5. Para cada Tipo de Bono activo:
     - Descuenta NC período anterior (si aplica)
     - Busca vigencia más reciente (fecha <= hoy, activo)
     - Busca tramo que contiene el valor
     - Aplica porcentaje del tramo
     - Calcula gamificación (falta para siguiente nivel)

  6. Retorna resultado (SLA: 500ms máximo)
     - Bonos calculados dinámicamente
     - Reflejan estado ACTUAL del período
     - Cambian cada vez que distribuidor consulta (si hay cambios en pedidos/entregas)
```

**Ejemplo Temporal - Período 1 al 15**:
```
DÍA 1: Distribuidor consulta
  Rango: 1 al 1 (fin del día 1)
  Pedido acumulado: $1M
  Bono calculado: $50K

DÍA 5: Distribuidor consulta nuevamente
  Rango: 1 al 5 (fin del día 5)
  Pedido acumulado: $8M (sumó 4 días más)
  Bono calculado: $500K (aumentó)

DÍA 11: Distribuidor consulta
  Rango: 1 al 11 (fin del día 11)
  Pedido acumulado: $14M (sumó 6 días más)
  Bono calculado: $1M (aumentó)

DÍA 15 (CIERRE): Sistema calcula bono FINAL
  Rango: 1 al 15 (fin del período)
  Pedido acumulado: $18M (período completo)
  Bono FINAL: $1.3M
  Este valor se congela en HistorialBono (inmutable post-cierre)
```

**Diferencia Clave**:
```
DURANTE el período (día 1 al 14):
  ?? Bono es DINÁMICO (cambia cada consulta)
  ?? Refleja lo acumulado HASTA HOY
  ?? Es indicativo/temporal

AL CIERRE del período (día 15):
  ?? Bono se calcula UNA VEZ más (período completo)
  ?? Se congela en HistorialBono (INMUTABLE)
  ?? Esto es el bono FINAL que se aplica en siguiente período
```

### BENEFICIARIOS

**Distribuidores:**
- Acceso desde Página Promocional (clic en botón/link "Ver mi bonificación")
- Redirección segura a Sitio Público Aldebaran (autenticación OTP)
- Autenticación por OTP (6 dígitos, lifetime configurable) - SMS/Email
- Página informativa solo lectura: Sin ingreso de datos adicionales
- Acceso aislado: solo ve su información, imposible ver otra
- Transparencia: Resumen claro de todos sus bonos aplicables
- Incentivo para aumentar compras (gamificación: falta para siguiente nivel)
- Sin necesidad de contactar a PROMOS

**PROMOS:**
- Automatización de cálculos (reduce tiempo administrativo)
- Historial auditable (soporte para reclamaciones)
- Validación automática (NC calculada vs real)
- Precisión 100% (elimina errores manuales)
- Auditoría completa de accesos de distribuidores

Estado: REFINADO CON SEGURIDAD - Listo para ajustes adicionales

---

## RESUMEN EJECUTIVO PARA EL CLIENTE

### ? Capacidades Entregables

| Funcionalidad | Descripción |
|---|---|
| **Consulta de Bonos** | Distribuidores ven bonos en tiempo real vía OTP (Sitio Público) |
| **Tres Tipos de Bonos** | Facturación (TOTUS) + Pedido (Aldebaran) + Entregado (Entregas) |
| **Ingreso Manual** | OC Especiales + Reconciliación de NC (unitario o CSV masivo) |
| **Aprobaciones** | Configurables por límite (automática o manual) |
| **Seguridad** | OTP + Aislamiento de datos + Auditoría completa |
| **Reportería** | 6 reportes + Exportación Excel/PDF |
| **Automatización** | Descarga de precios + Cierre de períodos |

### ?? 24 Requisitos Funcionales Definidos

- **3 de Administración** (Períodos, Tipos, Vigencias)
- **2 de Seguridad** (OTP, Aislamiento)
- **2 de Consultas** (Público + Admin)
- **2 de Historial** (Auditoría + Gamificación)
- **6 de Integración** (TOTUS, Precios, Órdenes, Entregas, NC, Ciclos)
- **3 de Ingresos Manuales** (OC Especiales + Reconciliación + Aprobaciones)
- **6 de Reportería** (Bonos vs Aplicados + Consultas + Discrepancias + Auditoría + Precios + Exportación)

### ?? Novedad: OC Especiales

**¿Qué son?**  
Órdenes de compra que PROMOS conoce pero no están capturadas en Aldebaran/TOTUS

**¿Dónde se aplican?**  
**SOLO** al Bono por Facturación (se SUMAN a la base)

**Ejemplo:**
```
Base sin OC:  $93.5M ? Bono: $56.1M
Base con OC:  $95.5M ? Bono: $57.3M
Incremento:            +$1.2M (+2.1%)
```

**Cómo funcionan:**
- Usuario PROMOS ingresa (unitario o CSV masivo)
- Sistema valida
- Si valor < límite: APROBADA automáticamente
- Si valor >= límite: Requiere aprobación del Admin
- Se suman al bono en próxima consulta

### ?? Diferenciales del Proyecto

? Transparencia: Distribuidores ven exactamente cómo se calcula su bono  
? Automatización: Elimina cálculos manuales y reduce tiempo 70%  
? Precisión: Auditoría completa, reconciliación automática de NC  
? Control: Aprobaciones configurables para ingresos manuales  
? Flexibilidad: Ingreso manual unitario y masivo (CSV)  
? Escalabilidad: Soporta múltiples bonos simultáneamente  

### ?? Estado del Documento

**Identificador:** RQM_BonosDistribuidores_052026  
**Cliente:** PROMOS  
**Estado:** ? REQUERIMIENTOS DEFINIDOS  
**Versión:** 1.0  
**Aprobado para:** Propuesta Técnica (siguiente fase)

---

**DOCUMENTO LISTO PARA PRESENTACIÓN AL CLIENTE**
