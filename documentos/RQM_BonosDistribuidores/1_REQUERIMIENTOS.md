# 1. REQUERIMIENTOS FUNCIONALES - Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS | **Estado**: REQUERIMIENTOS DEFINIDOS | **Fecha**: 2026  
**Versión**: 1.4

---

## 1.1 📚 GLOSARIO - Términos Clave del Documento

### 1.1.1 🎭 ¿Qué es un CASO DE USO (CU)?

Un **Caso de Uso** describe **UN FLUJO COMPLETO DE NEGOCIO** desde la perspectiva del usuario/actor.

- **Enfoque:** ¿Qué hace el ACTOR? ¿Cuál es el escenario de negocio?
- **Ejemplo:** CU7 = "Consultar Bonificación - Período Actual"
  - Un distribuidor autenticado CONSULTA su bono dinámicamente
  - Es UN proceso completo: se autentica → consulta → ve bonos → cierra sesión

**Total en este proyecto: 13 Casos de Uso (CU0 a CU12)**

**NOTA**: CU0 es un Caso de Uso **Prerequisito Transversal** - debe ejecutarse primero para que todos los demás casos de uso funcionen correctamente

### 1.1.2 ⚙️ ¿Qué es un REQUISITO FUNCIONAL (RF)?

Un **Requisito Funcional** describe **UNA CAPACIDAD ESPECÍFICA** que el sistema DEBE tener.

- **Enfoque:** ¿Qué DEBE HACER el SISTEMA? ¿Cuál es la funcionalidad concreta?
- **Ejemplo:** RF6 = "Consultar Bonificación - Período Actual"
  - El sistema DEBE calcular bono dinámicamente en 500ms
  - El sistema DEBE retornar desglose por tipo de bono
  - El sistema DEBE mostrar gamificación

**Total en este proyecto: 34 Requisitos Funcionales (RF1 a RF34)**

### 1.1.3 📊 Relación CU ↔ RF (Matriz de Trazabilidad)

```
UN CASO DE USO (flujo) = MÚLTIPLES REQUISITOS FUNCIONALES (capacidades)

Ejemplo:
┌─────────────────────────────────────────────────────────────┐
│ CU7: Consultar Bonificación - Período Actual                │
│ (El DISTRIBUIDOR consulta su bono en tiempo real)           │
├─────────────────────────────────────────────────────────────┤
│ Soportado por VARIOS RF:                                    │
│ ├─ RF6: Consultar Bonificación - Período Actual             │
│ ├─ RF8: Registrar Historial (auditoría de consultas)        │
│ ├─ RF9: Gamificación (mostrar falta para siguiente nivel)   │
│ ├─ RF11: Capturar Valor Facturado (TOTUS)                   │
│ ├─ RF12: Capturar Valor Pedido                              │
│ ├─ RF4: Autenticar Distribuidor (OTP Email)                 │
│ └─ RF29: Notificaciones Gamificación (Email)                │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 1.1.4 📋 Cómo leer este documento

1. **Secciones 1.1 - 1.4**: Contexto general, actores, matriz de acceso
2. **Sección 1.3**: CASOS DE USO (CU1 a CU12) - Flujos de negocio completos
3. **Sección 1.4**: REQUISITOS FUNCIONALES (RF1 a RF26) - Capacidades específicas del sistema
4. **Secciones 1.5+**: Detalles técnicos, insumos, responsabilidades

---

## 1.2 📊 MATRIZ DE TRAZABILIDAD CU ↔ RF (Relaciones Completas)

### 1.2.1 Leyenda

- 🟢 **Verde**: RF crítico para ese CU (funciona completamente si se implementa)
- 🟡 **Amarillo**: RF complementario (mejora pero no bloquea el CU)
- 🔵 **Azul**: RF de auditoría/soporte (trazabilidad)

### 1.2.2 Matriz Completa

| CU | Descripción | RF Críticos 🟢 | RF Complementarios 🟡 | RF Auditoría 🔵 |
|---|---|---|---|---|
| **CU0** | Configurar Tipo de Cliente - Distribuidor + Email de Bonificación | RF32, RF33 | - | RF8 |
| **CU1** | Crear Período | RF1 | RF2, RF3 | RF8 |
| **CU2** | Crear Tipo de Bono | RF2 | RF1, RF3 | RF8 |
| **CU3** | Crear Vigencia | RF3, RF11 | RF1, RF2, RF12 | RF8, RF24 |
| **CU4** | Obtener Facturación (TOTUS) | RF11, RF14 | RF10 | RF8 |
| **CU5** | Cargar Precios | RF10 | RF12 | RF8, RF24 |
| **CU6** | Autenticar Distribuidor | RF4, RF5 | - | RF8 |
| **CU7** | Consultar Bonificación (Actual) | RF6, RF11, RF12, RF9, RF29 | RF4, RF5, RF10, RF14 | RF8, RF21 |
| **CU8** | Consultar Histórico (Anterior) | RF28, RF14, RF15 | RF5 | RF8, RF21 |
| **CU9** | Consultar Bono (Admin) | RF7, RF11, RF12 | RF1, RF2, RF3, RF14 | RF8, RF23, RF24 |
| **CU10** | Cierre de Período (Automático) | RF7, RF11, RF12, RF14, RF15 | RF10, RF16-A, RF16-B | RF8, RF23 |
| **CU11** | Reconciliación NC (Manual) | RF15, RF17, RF18, RF19 | RF14 | RF8, RF22, RF23, RF25 |
| **CU12** | Resolver Reclamación (Soporte) | RF8, RF21, RF22, RF23, RF24 | RF7 | RF20, RF25, RF26 |

---

### 1.3 Vista por Categoría de RF

#### 1.3.1 🔧 **ADMINISTRACIÓN** (RF1-RF3, RF32-RF34)

```
RF34 (Gestionar Descuento General de Distribuidor)
├─ CU0 ━━━━ Configurar descuento general aplicable a TODOS los distribuidores (entrada principal)
├─ CU7 ━━━ Usa descuento general en cálculo dinámico de Bono por Pedido
├─ CU10 ━━ Usa descuento general en cierre automático de período
├─ CU12 ━━ Admin ve descuento usado en análisis de reclamaciones
└─ CU9 ━━━ Admin consulta descuento general configurado

RF33 (Gestionar Email de Bonificación para Distribuidores)
├─ CU0 ━━━━ Validar/configurar Email de Bonificación en distribuidores (entrada principal)
├─ CU6 ━━━ Usar Email de Bonificación para enviar OTP
├─ CU7 ━━━ Usar Email de Bonificación para notificaciones de gamificación
└─ CU9 ━━━ Admin ve Email de Bonificación de cada distribuidor

RF32 (Marcar/Identificar Customers como Distribuidores)
├─ CU0 ━━━━ Marcar clientes tipo DISTRIBUIDOR (entrada principal)
├─ CU6 ━━━ Validar que documento sea tipo DISTRIBUIDOR en OTP
├─ CU7 ━━━ Filtra solo distribuidores en consultas de bonificación
└─ CU9 ━━━ Admin solo consulta bonos de clientes tipo DISTRIBUIDOR

RF1 (Gestionar Períodos)
├─ CU1 ━━━━ Crear período (entrada principal)
├─ CU9 ━━━ Consultar bonos por período
└─ CU12 ━ Resolver reclamaciones (filtro por período)

RF2 (Gestionar Tipos de Bono)
├─ CU2 ━━━━ Crear tipo (entrada principal)
├─ CU3 ━━━ Asociar con vigencia
└─ CU9 ━━ Mostrar tipo usado en cálculos

RF3 (Gestionar Vigencias)
├─ CU3 ━━━━ Crear vigencia (entrada principal)
├─ CU4 ━━━ Parámetros de facturación (si filtrada)
├─ CU5 ━━━ Parámetros de precios
└─ CU24 ━ Auditoría de vigencias usadas
```

#### 1.3.2 🔐 **SEGURIDAD** (RF4, RF5, RF20)

```
RF4 (Autenticar por OTP - Email Interno)
├─ CU6 ━━━━ OTP Generation & Validation (entrada principal)
└─ CU7 ━━━ Acceso al bono (requiere autenticación)

RF5 (Validar Seguridad - Aislamiento)
├─ CU6 ━━━ OTP valida que sea distribuidor
├─ CU7 ━━ Solo ve su bono (no de otros)
└─ CU8 ━━ Solo ve su histórico

RF20 (Gestión de Aprobaciones)
├─ CU11 ━━━━ Aprueba/Rechaza OC Especiales (entrada principal)
├─ CU11 ━━━ Aprueba/Rechaza reconciliaciones
└─ CU9 ━━━ Consulta estados pendientes de aprobación
```

#### 1.3.3 📊 **CONSULTAS** (RF6, RF7, RF28)

```
RF6 (Consultar Bono - Período Actual)
├─ CU7 ━━━━ Calcula dinámicamente en tiempo real
└─ CU12 ━━ Muestra qué vio distribuidor en CU7

RF7 (Consultar Bono - Admin)
├─ CU9 ━━━━ Admin consulta bono calculado (entrada principal)
└─ CU10 ━━ Genera recomendación de NC al cierre

RF28 (Consultar Histórico - Períodos Anteriores)
├─ CU8 ━━━━ Retorna bonos congelados (entrada principal)
└─ CU12 ━━ Muestra qué vio distribuidor en CU8
```

#### 1.3.4 📈 **HISTORIAL & AUDITORÍA** (RF8, RF9)

```
RF8 (Registrar Historial de Bonos)
├─ TODOS CU ━━ Auditoría de CADA acción (transversal)
│  ├─ CU1 - Creación período
│  ├─ CU2 - Creación tipo bono
│  ├─ CU3 - Creación vigencia
│  ├─ CU4 - Consulta TOTUS
│  ├─ CU5 - Carga precios
│  ├─ CU6 - OTP attempt
│  ├─ CU7 - Consulta bono
│  ├─ CU8 - Consulta histórico
│  ├─ CU9 - Consulta admin
│  ├─ CU10 - Cierre período
│  ├─ CU11 - Reconciliación
│  └─ CU12 - Investigación reclamación
└─ CU12 ━━━━ Resolver reclamaciones (acceso principal)

RF9 (Gamificación)
├─ CU7 ━━━━ Muestra falta para siguiente nivel (entrada principal)
└─ CU12 ━━ Incluida en resolución de reclamaciones
```

#### 1.3.5 🔗 **INTEGRACIÓN DE DATOS** (RF10-RF15)

```
RF10 (Cargar Precios)
├─ CU5 ━━━━ Carga automática diaria de PRECIOS UNITARIOS (entrada principal)
├─ CU7 ━━━ Usa precios históricos en cálculos de bonos
├─ CU9 ━━━ Admin ve precios usados en cálculos
├─ RF34 ━ Usa DESCUENTO GENERAL de RF34 para cálculos
└─ CU24 ━ Auditoría de precios

RF11 (Capturar Facturación TOTUS)
├─ CU4 ━━━━ Obtiene de TOTUS (entrada principal)
├─ CU7 ━━━ Calcula Bono por Facturación
├─ CU9 ━━━ Admin ve valor facturado
└─ CU10 ━━ Cierre calcula con facturación final

RF12 (Capturar Valor Pedido)
├─ CU7 ━━━ Calcula Bono por Pedido (entrada principal)
├─ CU9 ━━━ Admin ve valor pedido
└─ CU10 ━━ Cierre calcula con pedidos acumulados

RF14 (Gestionar NC Período Anterior)
├─ CU4 ━━━ Obtiene NC de TOTUS
├─ CU7 ━━━ Descuenta en cálculo dinámico
├─ CU9 ━━━ Admin ve NC descontada
└─ CU10 ━━ Cierre calcula con NC anterior

RF15 (Reconciliación NC)
├─ CU8 ━━━ Usa NC Real para histórico
├─ CU11 ━━ Ingresa manualmente NC Real (entrada principal)
└─ CU7 ━━━ Proximos cálculos usan NC Real
```

#### 1.3.6 👤 **USUARIO PROMOS - OPERACIONES** (RF16-RF20)

```
RF16 (Ingreso Manual OC Especiales - Unitario)
├─ CU11 ━━━━ Ingresa una OC especial (entrada principal)
└─ CU7 ━━━━ Se suma al bono si está aprobada

RF17 (Carga Masiva OC Especiales - CSV)
├─ CU11 ━━━━ Carga múltiples OC especiales (entrada principal)
└─ CU7 ━━━━ Se suman al bono si están aprobadas

RF18 (Aplicación Manual NC en TOTUS)
├─ CU11 ━━━━ Aplicar NC en TOTUS con confirmación (entrada principal)
└─ CU10 ━━━ Cierre genera recomendación de NC

RF19 (Reconciliación Manual NC)
├─ CU11 ━━━━ Ingresa NC Real Unitaria o Masiva (entrada principal)
├─ CU7 ━━━ Proximos cálculos usan NC Real
└─ CU8 ━━━ Bonos históricos usan NC Real reconciliada

RF20 (Gestión de Aprobaciones)
├─ CU11 ━━━━ Aprueba/Rechaza ingresos manuales (entrada principal)
├─ CU7 ━━━ Si aprueba OC, se suma al bono
└─ CU10 ━━ Cierre solo calcula con OC aprobadas
```

#### 1.3.7 📋 **REPORTERÍA** (RF21-RF27)

```
RF21 (Bonos Calculados vs Aplicados)
├─ CU12 ━━━━ Resuelve reclamaciones (entrada principal)
├─ CU10 ━━━ Genera datos para reporte post-cierre
└─ CU11 ━━━ Muestra discrepancias de NC

RF22 (Distribuidores que Consultaron Bonos)
├─ CU12 ━━━━ Investiga reclamaciones (entrada principal)
├─ CU7 ━━━ Registra cada consulta
└─ CU8 ━━━ Registra cada consulta de histórico

RF23 (Discrepancias de NC)
├─ CU12 ━━━━ Resuelve discrepancias (entrada principal)
├─ CU11 ━━━ Identifica NC calculada ≠ NC real
└─ CU8 ━━━ Usa NC real en próximos períodos

RF24 (Auditoría de Acciones Usuario PROMOS)
├─ CU12 ━━━━ Investiga decisiones del usuario (entrada principal)
├─ CU11 ━━━ Auditoría de aprobaciones
├─ CU10 ━━━ Auditoría de cierre automático
└─ CU9 ━━━ Auditoría de cálculos realizados

RF25 (Precios y Vigencias Usados)
├─ CU12 ━━━━ Resuelve reclamaciones "¿por qué ese precio?" (entrada principal)
├─ CU7 ━━━ Documenta qué precios/vigencias se usaron
├─ CU10 ━━ Documenta configuración de cierre
└─ CU3 ━━━ Vigencia afecta precio usado

RF26 (Ingresos Manuales Aplicados)
├─ CU12 ━━━━ Auditoría de decisiones manuales (entrada principal)
├─ CU11 ━━━ Documenta OC Especiales + Reconciliaciones
└─ CU20 ━━ Auditoría de aprobaciones

RF27 (Exportación Reportes)
├─ CU12 ━━━━ Exporta para investigación reclamación (entrada principal)
├─ RF21-RF26 Aplica a TODOS los reportes
└─ Formatos: Excel + PDF
```

#### 1.3.8 🔔 **NOTIFICACIONES - GAMIFICACIÓN** (RF29-RF31)

```
RF29 (Notificación: Alcanzó Nuevo Nivel)
├─ CU7 ━━━━ Envío automático Email al alcanzar nivel (entrada principal)
├─ Evento: Distribuidor sube de tramo (ej: $1M-$5M → $5M-$10M)
├─ Canal: Email (configurable por distribuidor)
└─ Auditoría: Registra envío exitoso/fallido + timestamp

RF30 (Notificación: Cerca del Siguiente Nivel)
├─ CU7 ━━━━ Envío automático Email si está a X% del siguiente nivel
├─ Umbral: Configurable por Admin (default: 80%)
├─ Canal: Email (configurable por distribuidor)
├─ Frecuencia: Máximo 1 notificación por día (evitar spam)
└─ Auditoría: Registra envío exitoso/fallido + umbral usado

RF31 (Recordatorio Periódico: Progreso de Bonificación)
├─ CU7 ━━━━ Envío automático Email con resumen de progreso
├─ Frecuencia: Configurable por Admin (daily/weekly/monthly, default: weekly)
├─ Contenido: Bono actual, bono alcanzado, falta para siguiente nivel
├─ Canal: Email (configurable por distribuidor)
├─ Auditoría: Registra envío exitoso/fallido + contenido enviado
└─ Preferencias: Distribuidor puede desuscribirse de recordatorios
```

---

### 1.4 Resumen de Cobertura

| Categoría | Cantidad | CU Afectados | RF Críticos |
|---|---|---|---|
| **Administración** | 3 RF | CU1, CU2, CU3, CU9, CU12 | RF1, RF2, RF3 |
| **Seguridad** | 3 RF | CU6, CU7, CU8, CU9, CU11 | RF4, RF5, RF20 |
| **Consultas** | 3 RF | CU7, CU8, CU9, CU12 | RF6, RF7, RF28 |
| **Auditoría** | 2 RF | TODOS (transversal) | RF8, RF9 |
| **Integración** | 6 RF | CU4, CU5, CU7, CU9, CU10, CU11 | RF10-RF15 |
| **Operaciones Usuario** | 5 RF | CU11 | RF16-RF20 |
| **Reportería** | 7 RF | CU12 (principal) | RF21-RF27 |
| **Notificaciones** | 3 RF | CU7 (principal) | RF29-RF31 |
| **TOTAL** | **32 RF** | **12 CU** | **Todos relacionados** |

---

### 1.5 Flujo de Dependencias Críticas

```
┌─────────────────────────────────────────────────────────────────┐
│                    ORDEN DE IMPLEMENTACIÓN                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│ FASE 1: BASES ADMINISTRATIVAS                                   │
│ CU1 → RF1 (Crear Período)                                       │
│ CU2 → RF2 (Crear Tipo de Bono)                                  │
│ CU3 → RF3 (Crear Vigencia)                                      │
│   ↓                                                             │
│ FASE 2: INTEGRACIONES EXTERNAS                                  │
│ CU4 → RF11 (Obtener Facturación TOTUS) ✓ Requerido para CU7     │
│ CU5 → RF10 (Cargar Precios) ✓ Requerido para CU7                │
│   ↓                                                             │
│ FASE 3: SEGURIDAD & ACCESO DISTRIBUIDOR                         │
│ CU6 → RF4 + RF5 (OTP + Seguridad) ✓ Requerido para CU7/CU8      │
│   ↓                                                             │
│ FASE 4: CONSULTAS PÚBLICAS                                      │
│ CU7 → RF6 + RF11 + RF12 + RF13 + RF9 (Bono Actual)              │
│ CU8 → RF6-B + RF14 + RF15 (Histórico)                           │
│   ↓                                                             │
│ FASE 5: CONSULTAS ADMINISTRATIVAS                               │
│ CU9 → RF7 (Bono Admin)                                          │ 
│   ↓                                                             │
│ FASE 6: AUTOMATIZACIÓN & CIERRE                                 │
│ CU10 → RF7 + RF11-15 (Cierre Período)                           │
│   ↓                                                             │
│ FASE 7: OPERACIONES MANUALES                                    │
│ CU11 → RF15 + RF16-18 + RF19 (Reconciliación + OC + Aprob)      │
│   ↓                                                             │
│ FASE 8: REPORTERÍA & INVESTIGACIÓN                              │
│ CU12 → RF20-26 (Resolver Reclamaciones)                         │
│                                                                 │
│ ⏱️ RF8 (Auditoría) es TRANSVERSAL a TODOS los CU                │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```
## 1.6 Descripción General

### 1.6.1 Problemática de Negocio

**Situación Actual (Sin Sistema):**
- Los Distribuidores deben calcular manualmente sus bonificaciones
- Deben validar el cálculo con personal de PROMOS (proceso manual y lento)
- Falta transparencia en cómo se calculan los bonos
- Los Distribuidores prefieren comprar con competencia que tiene proceso automatizado
- PROMOS invierte tiempo en validar cálculos manuales (no agrega valor)
- No hay visibilidad sobre qué falta para acceder al siguiente nivel de bonificación
- Dificultad para resolver reclamaciones (no hay auditoría del cálculo)

### 1.6.2 Objetivo
Automatizar el cálculo de bonificaciones para distribuidores en la empresa PROMOS con dos modalidades:
- **Bonificación por Facturación**: Incentivo basado en valor total facturado en período (TOTUS)
- **Bonificación por Pedido**: Incentivo basado en valor total pedido en período (Cantidad pedida × Precio)

### 1.6.3 Propuesta de Valor

**Para Distribuidores (Acceso en Sitio Público - Consulta de Bonificación):**
- ✅ Acceso desde Página Promocional (clic en botón/link)
- ✅ Autenticación segura por OTP (SMS/Email) con lifetime configurable
- ✅ Consultar bono acumulado del período actual en tiempo real (SLA 500ms)
- ✅ Ver qué falta para acceder al siguiente nivel de bonificación (gamificación)
- ✅ Acceso seguro a información solo de su distribuidor (sin ver datos de competidores)
- ✅ Página informativa (solo lectura): Sin ingreso de datos adicionales
- ✅ Transparencia total: Resumen claro de todos sus bonos aplicables
- ✅ Sin necesidad de contactar a PROMOS para solicitar información

**Para PROMOS (Acceso en Aldebaran.Web - Admin):**
- ✅ Acceso rápido al valor final de bonificación para cada período
- ✅ Generar recomendación de Nota Crédito para aplicar en TOTUS
- ✅ Historial completo y auditable de cada cálculo (soporte para reclamaciones)
- ✅ Validación automática: NC calculada vs NC realmente aplicada (reconciliación)
- ✅ Reducción de tiempo administrativo: de manual a automático
- ✅ Precisión 100%: elimina errores de cálculo manual

### 1.6.4 Modelo de Negocio

```
CLIENTE DISTRIBUIDOR (en Aldebaran):
  ├─ Registro: Documento (Cédula), Nombre, Email(s), Celular
  ├─ Órdenes: Pedidos de artículos (cantidad pedida + precio)
  ├─ Entregas: Salidas de almacén (cantidad realmente entregada, puede ser parcial)
  ├─ Facturación: Registrada en TOTUS (verdad única, se factura lo entregado)
  └─ Contacto: Email/SMS para OTP

PÁGINA PROMOCIONAL (Tercero):
  ├─ Suministra: Lista de precios + Descuentos diarios
  ├─ Aloja: Link/Botón "Ver mi bonificación" 
  └─ Redirecciona: A Sitio Público Aldebaran

SITIO PÚBLICO ALDEBARAN (Consulta de Bonificación):
  ├─ Autenticación: OTP vía SMS/Email (lifetime configurable)
  ├─ Validación: Documento distribuidor (cédula) contra Cliente en Aldebaran
  ├─ Consulta: Bonos acumulados período actual (SLA 500ms)
  ├─ Presentación: Informe informativo (solo lectura)
  └─ Acceso: Aislado - distribuidor solo ve SU información

ALDEBARAN (Backend/Motor de Cálculo):
  ├─ Base de Datos: 
  │  ├─ Clientes (Distribuidores) - con Email, Celular
  │  ├─ Órdenes (Pedidos del distribuidor: cantidad pedida, precio)
  │  ├─ Entregas (Salidas de almacén: cantidad entregada, estado confirmado)
  │  ├─ Períodos, TiposBono, Vigencias
  │  └─ HistorialBono (auditoría)
  │
  ├─ Carga diaria: Precios desde Página Promocional
  ├─ Obtiene: Valor facturado desde TOTUS (por período - facturación real)
  ├─ Obtiene: Valor pedido de órdenes + precios (cantidad pedida)
  ├─ Obtiene: Valor entregado de entregas confirmadas (cantidad realmente entregada)
  ├─ Calcula: Bonos (recomendación) basado en insumos
  ├─ Registra: Historial y FOTO del cierre (inmutable post-cierre)
  ├─ Genera: Recomendación de nota crédito para TOTUS
  └─ Reconcilia: Valor calculado vs valor real aplicado

TOTUS (Tercero - Sistema de Facturación):
  ├─ Suministra: Valor facturado (verdad única de facturación)
  ├─ Parámetros: Documento (cédula), Tipo Doc (FAC), Fecha inicio/fin
  ├─ Retorna: ValorFacturado, NotasCredito, Fletes, Descuentos
  ├─ Aplica: Las notas crédito en siguiente período
  ├─ Retorna: Valor real aplicado (para reconciliación)
  └─ Integración: BD local TOTUS en servidor PROMOS (Read-only)

USUARIO PROMOS (Aldebaran.Web - Interno):
  ├─ Acceso: Restringido con autenticación interna PROMOS
  ├─ Funciones:
  │  ├─ Admin: Gestiona Períodos, Tipos, Vigencias
  │  ├─ Consulta: Bonos finales por distribuidor (por período)
  │  ├─ Prepara: Recomendación de NC para aplicar en TOTUS
  │  ├─ Aplica: Bonos recomendados en TOTUS (responsable valor real)
  │  ├─ Resuelve: Reclamaciones (acceso a historial completo)
  │  └─ Genera: Reportes, exportaciones, auditoría
  └─ Responsabilidad: Usuario es quien finalmente aplica NC en TOTUS

ADMINISTRADOR (Aldebaran.Web - Interno):
  ├─ Acceso: Todas las funciones de USUARIO PROMOS +
  ├─ Configura: Integraciones (TOTUS, Página Promocional)
  ├─ Configura: Horarios de carga, reintentos, timeouts
  ├─ Ve: Logs de seguridad (accesos distribuidores)
  └─ Gestiona: Usuarios, roles, permisos

PROCESO AUTOMÁTICO (Scheduled Jobs):
  ├─ Carga: Precios diariamente (horario configurable)
  ├─ Obtiene: Facturación de TOTUS (por período)
  ├─ Obtiene: Órdenes y entregas (continuamente)
  ├─ Cierre: Automático último día del período
  ├─ Conciliación: Manual (Usuario PROMOS la ejecuta en CU10)
  └─ Limpieza: Datos antiguos según política de retención
```

---

## 1.7 Actores del Sistema y Conrol de Acceso

### 1.7.1 Actores del Sistema

**EXTERNOS (Público):**
- **CLIENTE DISTRIBUIDOR**: Beneficiario de bonificaciones. Accede vía Página Promocional → Sitio Público (con OTP)
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

### 1.7.2 Matriz de Acceso

| Actor | Plataforma | Funcionalidad |
|-------|---------|---|
| **DISTRIBUIDOR (No Autenticado)** | Página Promocional | Clic en botón/link "Ver mi bonificación" |
| **DISTRIBUIDOR (No Autenticado)** | Sitio Público Aldebaran | Ingresa número documento |
| | | Recibe OTP por Email (lifetime configurable) |
| | | Ingresa OTP (máx 3 intentos) |
| **DISTRIBUIDOR (Autenticado)** | Sitio Público Aldebaran | Consultar bonos PERÍODO ACTUAL (RF6 - dinámico) |
| | | Consultar bonos PERÍODOS ANTERIORES (RF6 - histórico) |
| | | Ver estado aplicación NC en períodos anteriores (RF6) |
| | | Ver gamificación (falta para siguiente nivel) |
| | | Ver seguridad: solo su información (RF5) |
| | | Descargar comprobante/resumen período (RF6) |
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
| **PROCESO AUTOMÁTICO** | Aldebaran.Web | Obtener facturación TOTUS (CU4) |
| | | Cargar precios (CU5) |
| | | Cierre de período (CU10) |
| **USUARIO PROMOS** | Aldebaran.Web | Conciliación Manual de NC (CU11 - 2 modalidades) |

---

## 1.8 Casos de Uso (12 Total)

### 1.8.1 CU1: Crear Período (Definición de Periodicidad)

**Ubicación:** Aldebaran.Web  
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Administrador  
**Objetivo:** Definir una plantilla de periodicidad (template) que establece la configuración base para calcular bonificaciones

**Problemas que resuelve:**
- Sistema necesita conocer la **unidad de medida temporal** para organizar bonificaciones
- Permite crear **definiciones reutilizables** de periodicidad (Quincenal = 15 días, Mensual = 30 días, etc.)
- Cada período (template) se usa para generar **Instancias de Período Activas** con fechas específicas

**Información que debe poder ingresar:**
- **Nombre del período** (texto identificador único, ej: "Quincena PROMOS", "Mes PROMOS")
- **Tipo de período** (Mensual / Quincenal / Semanal / Diario / Custom)
- **Duración en días** (unidad de medida base):
  - Mensual: 30 días
  - Quincenal: 15 días
  - Semanal: 7 días
  - Diario: 1 día
  - Custom: N días (configurable)
- **Descripción** (opcional, ej: "Período quincenal para distribuidores")
- **Estado** (Activo / Inactivo)

**Acciones que puede realizar:**
- Crear nueva definición de período (template)
- Modificar período (solo si no tiene instancias activas/cerradas)
- Ver listado de períodos configurados
- Activar/Desactivar período
- Ver cuántas **Instancias de Período** se han generado con este template
- Generar manualmente una **Instancia de Período Activa** (con fecha inicio específica)

**Restricciones:**
- No puede editar duración de un período que ya tiene instancias cerradas
- No puede eliminar período que tiene instancias generadas
- No puede crear dos períodos con el mismo nombre
- No puede ingresar duración en días ≤ 0

---

**NOTA IMPORTANTE - Conceptos Clave:**

```
┌─────────────────────────────────────────────────────────────────┐
│ PERÍODO (Template/Definición):                                  │
│ └─ Es una PLANTILLA reutilizable                                │
│ └─ Define la DURACIÓN en días (unidad de medida)                │
│ └─ NO tiene fechas específicas                                  │
│ └─ Ejemplo: "Quincena PROMOS" = 15 días                         │
│                                                                 │
│ INSTANCIA DE PERÍODO (Período Activo):                          │
│ └─ Es una EJECUCIÓN específica del template                     │
│ └─ Tiene fecha inicio y fecha fin CALCULADA                     │
│ └─ Se genera manualmente o automáticamente                      │
│ └─ Ejemplo: "QUI-2026-01" del 01/01/2026 al 15/01/2026          │
│                                                                 │
│ RELACIÓN:                                                       │
│ └─ 1 Período (Template) → N Instancias de Período               │
│ └─ Fecha Fin = Fecha Inicio + Duración (del template)           │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

### 1.8.2 CU2: Crear Tipo de Bono

**Ubicación:** Aldebaran.Web
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Administrador  
**Objetivo:** Definir un tipo de bono especificando en qué insumo se basa (Facturación, Pedido o Entregado) y asociarlo a un Período (template de periodicidad)

**Problemas que resuelve:**
- Sistema debe saber qué tipos de bonificación están disponibles
- Permite estructurar bonos por diferentes criterios de incentivo
- Cada tipo de bono afecta diferentes comportamientos del distribuidor
- Asocia el tipo de bono a una definición de periodicidad (Quincenal, Mensual, etc.)

**Información que debe poder ingresar:**
- Nombre del tipo de bono (texto único, ej: "Bono por Facturación")
- Descripción (opcional, ej: "Incentivo basado en valor facturado")
- **Base del Bono** (qué fuente de datos genera el bono):
  - Facturación: Usa datos de TOTUS (valor facturado sin impuestos)
  - Pedido: Usa órdenes de Aldebaran (cantidad pedida × precio)
  - Entregado: Usa entregas de Aldebaran (cantidad entregada × precio)
- **Período al cual aplica** (referencia a CU1 - template de periodicidad)
  - Ejemplo: B1 "Bono por Facturación" → asociado a P1 "Quincena PROMOS"
- Estado (Activo / Inactivo)

**Acciones que puede realizar:**
- Crear nuevo tipo de bono
- Modificar tipo de bono (antes de usar en vigencia)
- Ver listado de tipos disponibles
- Activar/Desactivar tipo de bono
- Consultar cuántas vigencias usan este tipo
- Ver historial de bonos calculados por tipo

**Restricciones:**
- No puede eliminar tipo si ya tiene bonos calculados
- No puede cambiar "Base del Bono" si ya tiene vigencias activas
- No puede cambiar período de un tipo que tiene vigencias activas
- No puede tener dos tipos con mismo nombre asociados al mismo período
- No puede crear tipo sin definir Base del Bono

---

### 1.8.3 CU3: Crear Vigencia (CON PARAMETRIZACIÓN GRANULAR POR ARTÍCULO/REFERENCIA)

**Ubicación:** Aldebaran.Web
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Administrador  
**Objetivo:** Definir una vigencia (configuración de rangos de valores de compra con porcentajes de bono asociados) para un Tipo de Bono, con opción de restricción por artículos/referencias

**Problemas que resuelve:**
- Sistema necesita saber qué porcentaje de bono aplica según el acumulado del distribuidor
- Permite cambiar incentivos según necesidades de negocio (nuevas estrategias comerciales)
- Permite focalizar bonos en artículos específicos sin modificar toda la estructura
- Solo UNA vigencia puede estar ACTIVA por Tipo de Bono en un momento dado

**Información que debe poder ingresar:**
- Nombre de vigencia (texto único, ej: "V3 - Bono Facturación Marzo 2026")
- **Tipo de Bono al cual aplica** (referencia a CU2)
  - Ejemplo: Vigencia B1V3 → asociada a Tipo de Bono B1 "Bono por Facturación"
- **Fecha de Activación** (desde cuándo aplica esta vigencia)
  - Al activarse, desactiva automáticamente la vigencia anterior
- Estado (Activo / Inactivo)
- **Rangos de Valores** (Relación 1:N - Entidad hija de Vigencia):
  - **UNA vigencia tiene MÚLTIPLES rangos de valores**
  - Cada rango es un registro independiente con:
    - **Valor Mínimo** (numérico, ej: 1000000)
    - **Valor Máximo** (numérico, ej: 5000000)
    - **Porcentaje de Bono** (%, ej: 5.0)
  - Ejemplo de rangos para una vigencia:
    - Rango 1: Mín=$1M, Máx=$5M, Bono=5%
    - Rango 2: Mín=$5M, Máx=$10M, Bono=6%
    - Rango 3: Mín=$10M, Máx=∞, Bono=7%
- Opción: Restricción por artículos/referencias específicos (OPCIONAL):
  - Sin restricción (aplica a TODOS los artículos - DEFAULT)
  - Artículos específicos (TODAS sus referencias)
  - Artículos + Referencias específicas (combinación personalizada)
- Moneda del bono (COP, USD, etc.)
- Monto máximo de bono (tope configurable)

**Acciones que puede realizar:**
- Crear nueva vigencia
- Modificar vigencia (antes de que comience a usarse)
- Ver listado de vigencias activas e históricos
- Activar/Desactivar vigencia
- Especificar qué artículos/referencias incluir (si aplica)
- Ver qué distribuidores se benefician de esta vigencia
- Consultar histórico de cambios en vigencias
- Copiar vigencia anterior como template

**Restricciones:**
- **SOLO UNA vigencia ACTIVA por Tipo de Bono** (al activar una nueva, la anterior pasa a INACTIVA automáticamente)
- No puede editar vigencia que ya está en uso (debe crear nueva)
- No puede cambiar Tipo de Bono de una vigencia existente
- No puede crear vigencia con fecha de activación en el pasado
- Si parametriza por artículos, debe validar que artículos existan en sistema
- No puede eliminar vigencia que tiene bonos ya calculados

**NOTA IMPORTANTE - Vigencia ACTIVA:**

```
┌─────────────────────────────────────────────────────────────────┐
│ TIPO DE BONO B1 (Bono por Facturación):                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│ Vigencias (Históricas):                                         │
│ ├─ B1V1: Activa desde 2026-01-01 → Estado: INACTIVA            │
│ ├─ B1V2: Activa desde 2026-02-01 → Estado: INACTIVA            │
│ └─ B1V3: Activa desde 2026-03-01 → Estado: ACTIVA ✓            │
│    ├─ Rango 1: $1M-$5M → 5% de bono                            │
│    ├─ Rango 2: $5M-$10M → 6% de bono                           │
│    └─ Rango 3: $10M+ → 7% de bono                              │
│                                                                 │
│ ➜ Cálculos usan VIGENCIA ACTIVA (B1V3)                         │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 1.8.4 CU4: Obtener Facturación de TOTUS (Integración a BD Local)

**Ubicación:** Backend Aldebaran (Motor de Cálculo)
**Cuándo:** Dinámicamente (cada vez que se calcula bono) + Al cierre del período  
**Actor:** PROCESO AUTOMÁTICO + MOTOR DE CÁLCULO  
**Objetivo:** Obtener valor facturado real desde BD TOTUS (local) mediante SP para usarlo en cálculo de bonos

**NOTA TÉCNICA:** La consulta NO es a un sistema externo via API, sino a la **base de datos TOTUS local** mediante **Stored Procedure**. Esto garantiza latencia mínima (< 50ms típicamente).

**⚠️ DEPENDENCIA EXTERNA CRÍTICA:** El SP de TOTUS (`sp_ObtenerFacturacionDistribuidor`) es **desarrollado y mantenido por OTRA FÁBRICA DE SOFTWARE**, NO por Aldebaran. Aldebaran solo CONSUME el SP.

**Problemas que resuelve:**
- Sistema necesita valor facturado como fuente de verdad (BD TOTUS es referencia única)
- Debe poder filtrar por artículos/referencias si la vigencia está parametrizada
- Debe manejar fallos de BD sin interrumpir operación
- Debe coordinarse con otra fábrica para asegurar SLA del SP

**Información que necesita:**
- Tipo documento: "FAC" (Factura)
- Número documento: Cédula del distribuidor
- Fecha inicio: Primer día del período
- Fecha fin: Último día del período o hoy
- Lista de artículos (OPCIONAL - si vigencia está parametrizada)
- Mapa de referencias por artículo (OPCIONAL - si vigencia está parametrizada)

**Información que retorna de BD TOTUS (via SP):**
- ValorTotalFacturadoSinImpuestos (obligatorio)
- TotalNotasCredito (obligatorio)
- TotalFletes (opcional)
- TotalDescuentos (opcional)

**Acciones que puede realizar:**
- Ejecutar SP **desarrollado por OTRA FÁBRICA** en BD TOTUS (local)
- Usar MOCK configurable si SP no está disponible (solo para desarrollo/testing)
- Filtrar facturación por artículos específicos (parámetros del SP)
- ⚠️ **IMPORTANTE - SIN CACHE**: **NO cachea** resultado para garantizar información 100% actualizada
- Registrar auditoría de cada consulta (timestamp, distribuidor, resultado)
- Usar fallback (último valor conocido) si BD TOTUS no responde + Alerta a Admin + Banner de advertencia
- **COORDINARSE** con otra fábrica si SP no cumple SLA o tiene errores

**Restricciones:**
- SLA máximo 500ms para cálculos dinámicos (consulta BD TOTUS típicamente < 50ms)
- No puede usar valores negativos
- No puede ignorar filtros de artículos/referencias
- No puede modificar valores en BD TOTUS (solo lectura - SELECT via SP)
- **Sin cache de resultados** (siempre ejecuta SP en tiempo real)
- Fallback solo en caso de error de BD TOTUS (con advertencia visible al distribuidor)
- Connection string debe tener pooling habilitado (ADO.NET)

**NOTA IMPORTANTE - Decisión de NO usar Cache:**

```
RAZÓN PRINCIPAL: Consultas NO son constantes, son ESPORÁDICAS

JUSTIFICACIÓN:
├─ Distribuidores consultan de forma impredecible (no hay patrón fijo)
├─ Promesa de información en tiempo real es CRÍTICA para el negocio
├─ BD TOTUS (local) maneja fácilmente carga concurrente con SP optimizado
├─ Connection pooling de ADO.NET + Timeouts optimizados son suficientes
└─ Latencia de BD local (< 50ms) hace cache innecesario

VENTAJAS DE CONSULTA A BD LOCAL:
├─ Sin dependencia de API/HTTP externa
├─ Latencia mínima (< 50ms típicamente)
├─ Connection pooling eficiente (ADO.NET)
└─ SLA de 500ms FÁCILMENTE alcanzable

VALIDACIÓN REQUERIDA:
├─ SP en BD TOTUS está optimizado (índices, estadísticas)
├─ Connection string tiene pooling habilitado
├─ BD TOTUS puede manejar N consultas concurrentes (típicamente no es problema)
└─ Latencia promedio del SP < 50ms
```

---

### 1.8.5 CU5: Cargar Lista de Precios (Automático Diario)

**Ubicación:** Backend Aldebaran (Scheduled Job)
**Cuándo:** Automático, horario configurable (default: 6 AM)  
**Actor:** PROCESO AUTOMÁTICO  
**Objetivo:** Descargar y cargar la lista de precios diaria desde Página Promocional

**Problemas que resuelve:**
- Sistema necesita precios actualizados diariamente para calcular bonos correctamente
- Los precios cambian constantemente y deben estar disponibles
- Si descarga falla, debe continuar operando sin interrupciones

**Información que necesita acceder:**
- URL/SFTP/API de Página Promocional (configurable)
- Credenciales de autenticación (configurable)
- Horario de descarga (configurable, default 6 AM)
- Política de reintentos (configurable, default 3 intentos)
- Espera entre reintentos (configurable, default 5 minutos)
- Política de retención histórico (configurable, default 120 días)

**Información que obtiene:**
- Archivo con estructura: Referencia, Precio Unitario, Descuento Distribuidor
- Validación: Precios > 0, Descuentos 0-100%

**Acciones que puede realizar:**
- Descargar archivo desde Página Promocional
- Validar estructura y datos
- Reemplazar tabla PreciosDistribuidor (actual)
- Guardar histórico en PreciosDistribuidorHistorico con fecha
- Ejecutar limpieza automática (borra precios más antiguos que período de retención)
- Registrar auditoría completa (quién, qué, cuándo, resultado)
- Enviar alertas si descarga falla
- Usar precios del día anterior si carga falla completamente

**Restricciones:**
- No puede dejar el sistema sin precios (fallback a precios anteriores)
- No puede ejecutar limpieza de precios activos (en uso)
- No puede eliminar precios de períodos no cerrados
- No puede permitir precios duplicados
- No puede aceptar archivos con estructura incorrecta
- No puede sobreescribir precios durante horas de operación críticas

---

### 1.8.6 CU6: Autenticar Distribuidor (OTP - Seguridad)

**Ubicación:** Sitio Público Aldebaran
**Acceso:** Desde Página Promocional (clic en botón/link)  
**Actor:** Distribuidor (cliente externo)  
**Objetivo:** Autenticar distribuidor de forma segura usando OTP (One Time Password) antes de mostrar bonificación

**Problemas que resuelve:**
- Distribuidores deben acceder de forma segura desde Página Promocional
- No hay autenticación corporativa disponible
- Deben asegurar que solo el distribuidor autorizado vea su información

**Información que necesita ingresar:**
- Número de documento (cédula)
- Código OTP recibido por Email

**Información que valida:**
- Documento existe en BD Aldebaran
- Es tipo "DISTRIBUIDOR" (no otro tipo de cliente)
- Tiene Email configurado

**Información que genera:**
- Código OTP de 6 dígitos único (generado internamente por Aldebaran)
- Token de sesión JWT

**Acciones que puede realizar:**
- Generar código OTP internamente (sin terceros)
- Enviar OTP por Email a dirección configurada
- Permitir reintentos (máx 3 intentos)
- Crear sesión con token JWT válido 8 horas
- Invalidar OTP anterior si distribuidor solicita otro
- Registrar intento de autenticación (exitoso/fallido)
- Rechazar acceso después de 3 intentos fallidos

**Restricciones:**
- OTP válido solo 10 minutos (configurable)
- Máximo 3 intentos fallidos
- No puede autenticar si no hay Email
- No puede guardar OTP en texto plano (encriptado)
- No puede permitir acceso sin validar OTP
- No puede reutilizar OTP ya usado
- Token expira después de 8 horas (configurable)

**NOTA IMPORTANTE - OTP Interno:**

```
┌─────────────────────────────────────────────────────────────────┐
│ OTP GENERADO Y VALIDADO INTERNAMENTE POR ALDEBARAN              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│ IMPLEMENTACIÓN:                                                 │
│ ✅ OTP de 6 dígitos generado internamente (sin terceros)        │
│ ✅ Validación interna de código OTP                             │
│ ✅ Envío por Email via SMTP local de PROMOS                     │
│ ✅ Sin suscripción mensual                                      │
│ ✅ Sin cargos adicionales                                       │
│                                                                 │
│ CONFIGURACIÓN:                                                  │
│ └─ Usar infraestructura de email existente de PROMOS (SMTP)     │
│ └─ OTP válido: 10 minutos (configurable)                        │
│ └─ Máximo 3 intentos fallidos                                   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

### 1.8.7 CU7: Consultar Bonificación - Período Actual (Distribuidor - Sitio Público)

**Ubicación:** Sitio Público Aldebaran
**Acceso:** Solo con autenticación OTP válida (CU6 completado)  
**Actor:** Distribuidor (autenticado)  
**Objetivo:** Consultar bonos acumulados en tiempo real durante el período actual, con notificaciones automáticas de gamificación

**Problemas que resuelve:**
- Distribuidor necesita ver su bono actualizado según lo que ha pedido/entregado/facturado en el período EN CURSO
- Debe conocer cuánto falta para acceder al siguiente nivel de bonificación (gamificación)
- Necesita transparencia total sin contactar a PROMOS
- Debe recibir notificaciones proactivas sobre logros alcanzados y recordatorios de progreso
- Página es solo lectura: sin ingreso de datos adicionales

**Información que necesita acceder:**
- Período actual (fechas inicio/fin)
- Órdenes del distribuidor del período actual (cantidad + precio)
- Entregas confirmadas del período actual (cantidad + precio)
- Facturación real desde TOTUS del período actual
- NC período anterior (del historial reconciliado)
- Vigencia activa más reciente
- Tramos configurados para cada tipo de bono
- Precios históricos del día de cada transacción
- Preferencias de notificación del distribuidor

**Información que calcula dinámicamente:**
- Bono por Facturación: Base = Facturado - NC_Período - NC_Anterior; Bono = Base × % Vigencia
- Bono por Pedido: Base = Suma órdenes; Bono = Base × % Vigencia
- Gamificación: Diferencia entre acumulado actual y siguiente tramo

**Acciones que puede realizar:**
- Ver bonos acumulados en tiempo real (SLA: 500ms)
- **Ver desglose por tipo de bono** con diferentes niveles de detalle:
  - **Facturación**: Solo totales consolidados (limitación de TOTUS - no desglose)
  - **Pedido**: Desglose completo por orden, artículo, cantidad y precio
- Ver gamificación (falta para siguiente nivel)
- Ver período actual y días transcurridos
- Consultar múltiples veces (cada consulta recalcula)
- Configurar preferencias de notificación:
  - Elegir canales (SMS, Email, ambos)
  - Decidir si recibir notificaciones al alcanzar nuevo nivel 
  - Decidir si recibir alertas cuando está cerca del siguiente nivel 
  - Decidir si recibir recordatorios periódicos de progreso 
- Cerrar sesión

**Notificaciones Automáticas Integradas:**

**Notificación: Alcanzó Nuevo Nivel**
- **Cuándo:** Se dispara automáticamente cuando distribuidor sube de tramo en cualquier tipo de bono
- **Canal:** Email
- **Contenido:** "¡Felicidades! Alcanzaste un nuevo nivel de bonificación. Tu bono por [Tipo] es ahora [%]% sobre [base]. ¡Sigue adelante!"
- **Restricción:** Máximo 1 notificación por distribuidor por tipo de bono en 24 horas
- **Auditoría:** Registra envío exitoso/fallido + timestamp

**Notificación: Está Cerca del Siguiente Nivel**
- **Cuándo:** Job diario verificación (default: 6 AM UTC) si acumulado ≥ X% del siguiente tramo
- **Canal:** Email
- **Contenido:** "¡Casi lo logras! Estás a poco de alcanzar el siguiente nivel de bonificación en [Tipo de Bono]. Necesitas $[Monto Faltante] más. ¡Adelante!"
- **Configuración Admin:** Umbral % (50-95%, default 80%), Frecuencia, Horario, Activo/Inactivo
- **Restricción:** Máximo 1 notificación en 24 horas, anti-spam habilitado
- **Auditoría:** Registra envío exitoso/fallido + umbral usado

**Recordatorio Periódico: Progreso de Bonificación**
- **Cuándo:** Job semanal (default: Lunes 8 AM UTC) o configurada por Admin
- **Canal:** Email
- **Contenido Email:** Resumen completo de 2 bonos (Facturación, Pedido) con acumulado, tramo, % completado, falta para siguiente
- **Configuración Admin:** Frecuencia (Diaria/Semanal/Bisemanal/Mensual), Día/Hora, Activo/Inactivo
- **Preferencias Distribuidor:** Puede desuscribirse, establecer horario
- **Restricción:** Solo si período está activo, distribuidor no está desuscrito, tiene Email
- **Auditoría:** Registra envío exitoso/fallido + contenido enviado

**Restricciones:**
- Página solo lectura (sin ingreso de datos)
- No puede ver información de otro distribuidor
- Token debe estar válido (no expirado)
- Cálculo es dinámico (se ejecuta cada consulta, no precalculado)
- **Cambios en CUALQUIER fuente se reflejan en próxima consulta**:
  - Nueva factura en TOTUS → Actualiza bono por Facturación
  - Nueva orden en Aldebaran → Actualiza bono por Pedido
  - Nueva entrega confirmada en Aldebaran → Actualiza bono por Entregado
  - Nueva OC Especial aprobada → Actualiza bono por Facturación
- No puede acceder a datos administrativos (Aldebaran.Web)
- No puede ver períodos anteriores (ver CU8 para eso)
- Notificaciones respetan siempre preferencias del distribuidor (desuscripciones, canales, horarios)

---

### 1.8.8 CU8: Consultar Histórico de Bonos - Períodos Anteriores (Distribuidor - Sitio Público)

**Ubicación:** Sitio Público Aldebaran
**Acceso:** Solo con autenticación OTP válida (CU6 completado)  
**Actor:** Distribuidor (autenticado)  
**Objetivo:** Consultar bonos finales congelados de períodos cerrados anteriores

**Problemas que resuelve:**
- Distribuidor necesita ver qué bonificación recibió en períodos anteriores (histórico)
- Debe saber si el bono anterior ya fue aplicado como NC o sigue en espera (transparencia de aplicación)
- Necesita acceder al histórico de todos sus bonos cerrados
- Página es solo lectura: sin ingreso de datos adicionales

**Información que necesita acceder:**
- Lista de períodos cerrados disponibles (últimos N períodos)
- Bono final CONGELADO de cada período anterior (inmutable)
- Estado de aplicación: "Aplicado como NC" vs "Pendiente de aplicación" vs "Rechazado"
- Fecha en que se aplicó la NC (si aplica)
- Desglose del bono por tipo (Facturación, Pedido, Entregado)
- OC Especiales incluidas en ese período (si aplica)

**Información que RETORNA (sin cálculos - solo lectura):**
- Bono Final asignado (congelado, inmutable)
- Estado: "Definitivo" o "En proceso"
- Aplicación: Estado de la NC (Aplicada, Pendiente, Rechazada)
- Referencia: Número/ID de la NC si fue aplicada

**Acciones que puede realizar:**
- Seleccionar período anterior a consultar (dropdown de períodos cerrados)
- Ver bono final asignado en ese período (congelado)
- Ver estado de aplicación de la NC
- Ver desglose del bono por tipo
- Ver fecha de aplicación (si aplica)
- Navegar entre períodos anteriores
- Descargar comprobante/resumen del período (PDF)
- Consultar múltiples veces (cada consulta retorna lo congelado)
- Cerrar sesión

**Restricciones:**
- Página solo lectura (sin ingreso de datos)
- No puede ver información de otro distribuidor
- Token debe estar válido (no expirado)
- Bonos mostrados son INMUTABLES (congelados al cierre del período)
- Solo puede ver últimos N períodos cerrados (N = configurable por Admin, default = 12)
- No puede ver períodos activos (en curso) - solo cerrados
- No puede acceder a datos administrativos (Aldebaran.Web)

---

### 1.8.9 CU9: Consultar Bono Actual Dinámico - PROMOS

**Ubicación:** Aldebaran.Web
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Usuario PROMOS  
**Objetivo:** Consultar bono actual de un distribuidor en período activo para preparar recomendación de NC

**Problemas que resuelve:**
- Usuario PROMOS necesita ver el bono actual calculado por el sistema
- Debe preparar recomendación de NC antes de aplicarla en TOTUS
- Necesita acceder a historial completo y auditoría de cada cálculo

**Información que necesita acceder:**
- Distribuidor (búsqueda/selección)
- Período actual o anterior (selección)
- Bono calculado (valor final)
- Detalles del cálculo (insumos usados)
- Historial de cálculo (cambios en el período)
- Auditoría completa (quién, qué, cuándo)

**Información que retorna:**
- Bono por Facturación (desglosado)
- Bono por Pedido (desglosado)
- Total bonificación
- Vigencia aplicada
- Tramos usados
- NC anterior descontada
- Precios usados
- Historial de cambios durante el período
- Auditoría de todas las acciones

**Acciones que puede realizar:**
- Buscar distribuidor por documento/nombre
- Seleccionar período (actual o anterior)
- Ver bonos desglosados por tipo
- Expandir detalles de cada bono
- Ver historial de cálculos del período
- Generar recomendación de NC
- Exportar información para preparar aplicación en TOTUS
- Ver auditoría completa

**Restricciones:**
- Solo acceso a Usuario PROMOS (autenticado internamente)
- No puede modificar valores calculados (solo lectura)
- No puede acceder a distribuidores de otros segmentos (si hay restricción de rol)
- Datos mostrados son cálculos internos (no son vinculantes hasta aplicar en TOTUS)

---

### 1.8.10 CU10: Cierre de Período (Automático)

**Ubicación:** Backend Aldebaran (Scheduled Job)
**Cuándo:** Último día de la **Instancia de Período Activa**, a hora configurada (ej: 23:59:59)  
**Actor:** PROCESO AUTOMÁTICO  
**Objetivo:** Cerrar la **Instancia de Período Activa** y calcular bonos finales recomendados (FOTO congelada)

**Problemas que resuelve:**
- Necesario congelar cálculos al final de la instancia de período para auditoría
- Genera recomendación de NC para que Usuario PROMOS la aplique en TOTUS
- Garantiza inmutabilidad de datos post-cierre para cumplimiento normativo

**Información que necesita:**
- **Instancia de Período Activa** (verificar que es el último día según duración del template)
- Todos los distribuidores con actividad en la instancia de período
- Órdenes, entregas y facturación acumulada
- Vigencias activas
- Precios del período

**Información que genera:**
- FOTO congelada en HistorialBono (inmutable post-cierre)
- Bono Recomendado (cálculo final de la instancia de período)
- Recomendación de NC para aplicar en siguiente instancia de período
- Evento de cierre (para notificación)
- Estado de la **Instancia de Período**: CERRADO

**Acciones que realiza automáticamente:**
- Calcula bono RECOMENDADO de la instancia de período (completo, no dinámico)
- Almacena FOTO en HistorialBono (estado CALCULADO)
- Genera recomendación de NC con estado RECOMENDADA
- Marca **Instancia de Período** como CERRADO
- Publica evento de cierre (RabbitMQ)
- Notifica Usuario PROMOS (configuración pendiente)
- Registra auditoría completa

**Restricciones:**
- No puede modificar datos después del cierre (inmutabilidad)
- No aplica NC automáticamente en TOTUS (solo recomienda)
- No puede cerrar instancia de período que ya está cerrada
- Usuario PROMOS es responsable de aplicar NC en TOTUS
- No puede recalcular instancia de período cerrada (la FOTO es definitiva)

**NOTA - RESPONSABILIDADES CLARAS**:
```
┌──────────────────────────────────────────────────────────────┐
│ ALDEBARAN (Este Sistema):                                    │
│ └─ CALCULA: Bono recomendado al cierre                       │
│ └─ REGISTRA: FOTO en HistorialBono (inmutable)               │
│ └─ SUGIERE: Valor de NC a aplicar en TOTUS                   │
│ └─ NOTIFICA: Usuario PROMOS para revisión                    │
│                                                              │
│ ❌ NO APLICA DIRECTAMENTE en TOTUS                           │
│ ❌ NO AFECTA datos en TOTUS                                  │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ USUARIO PROMOS (Humano - Responsable):                       │
│ └─ APLICA: La NC en TOTUS (responsable del valor real)       │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ TOTUS (Sistema Tercero - Verdad Única):                      │
│ └─ RECIBE: Recomendación de NC                               │
│ └─ APLICA: La NC en el siguiente período                     │
│ └─ REGISTRA: NC real aplicada en su BD                       │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---
### 1.8.11 CU11: Conciliación Manual de Nota Crédito (Manual)

**Ubicación:** Aldebaran.Web (Ingreso manual de datos)
**Cuándo:** Usuario PROMOS la ejecuta manualmente, después del cierre del período N y antes de final del período N+1  
**Actor:** USUARIO PROMOS  
**Objetivo:** Registrar el valor REAL de la NC que se aplicó en TOTUS vs la NC calculada en CU9 (FOTO), para usar ese valor en cálculos del siguiente período

**Problemas que resuelve:**
- Sistema necesita saber cuánto fue REALMENTE aplicado en TOTUS (puede ser diferente al calculado)
- Distribuidores deben recibir bono basado en NC REAL de TOTUS, no en calculada
- Auditoría: Detectar discrepancias entre calculado y aplicado

**Modalidades soportadas:**
- **UNITARIO**: Ingreso manual distribuidor por distribuidor
- **MASIVO**: Carga de archivo CSV con múltiples distribuidores

**Información que necesita ingresar:**
- Distribuidor (selección)
- Período a conciliar (período anterior cerrado)
- NC Calculada (mostrada automáticamente del cierre de CU9)
- NC Real (ingresada manualmente: lo que TOTUS realmente aplicó)
- Fecha de confirmación en TOTUS (opcional)
- Motivo si hay discrepancia (opcional - texto libre)

**Información que valida:**
- Distribuidor existe
- Período está cerrado (ya se ejecutó CU9)
- NC Calculada existe en HistorialBono
- NC Real es numérica y positiva
- Fechas son válidas

**Información que registra:**
- HistorialBono.BonoAplicado = Valor real ingresado
- HistorialBono.Estado = CONCILIADO o CONCILIADO CON DISCREPANCIA
- HistorialBono.Diferencia = NC Real - NC Calculada
- Auditoría completa: quién, qué, cuándo, modalidad

**Acciones que puede realizar:**
- **Ingreso Unitario**: Formulario con campos para ingresar NC Real de un distribuidor
- **Carga Masiva (CSV)**: Descargar plantilla, completar con múltiples distribuidores, cargar
- Validar estructura del archivo CSV antes de aplicar
- Mostrar resumen de registros válidos/inválidos antes de confirmar
- Comparar NC Calculada vs Real (detecta discrepancias)
- Generar alertas si diferencia supera umbral configurable (ej: 2%)
- Registrar motivo de discrepancia en auditoría

**Restricciones:**
- No puede modificar valores de CU9 (FOTO congelada)
- No puede conciliar período no cerrado
- No puede ingresar valores negativos o cero
- No puede eliminar conciliación (solo marcar como rechazada)
- Cambio de estado es irreversible (PENDIENTE → CONCILIADO)

---

### 1.8.12 CU12: Resolver Reclamación (Soporte)

**Ubicación:** Aldebaran.Web
**Acceso:** Usuario PROMOS (Admin o rol superior)
**Actor:** Usuario PROMOS
**Objetivo:** Acceder a información completa del histórico de bonos mostrados al distribuidor durante un período para investigar y resolver reclamaciones

**Problemas que resuelve:**
- Distribuidor reclama que vio diferentes valores de bonos en diferentes consultas durante el período
- Distribuidor reclama que el bono final no corresponde con sus cálculos
- Distribuidor reclama cambios no justificados entre consultas (ej: OC ingresadas después de que él consultó)

**Información que debe poder consultar:**

1. **Historial de Consultas del Distribuidor**
   - Fecha y hora de cada consulta realizada en el período
   - Bono mostrado en cada consulta (desglosado por tipo: Facturación, Pedido)
   - Acumulado de insumos en cada momento (Facturación, Pedidos, OC Especiales)

2. **Foto Final del Período**
   - Bono calculado al cierre del período (inmutable)
   - Desglose completo: cada tipo de bono + OC Especiales incluidas

3. **Análisis de Cambios**
   - Qué cambió entre cada par de consultas (facturación, pedidos, entregas ingresados)
   - Por qué cambió el bono de una consulta a otra
   - Identificar si hubo ingresos manuales (OC Especiales) que afectaron el cálculo

4. **Auditoría de Ingresos Manuales**
   - OC Especiales ingresadas durante el período (quién, cuándo, monto, estado)
   - Reconciliaciones de NC realizadas (si aplica)
   - Aprobaciones de ingresos manuales

5. **Detalles del Cálculo**
   - Vigencia aplicada en el período
   - Tramos usados para cada bono
   - NC período anterior descontada
   - Precios usados en el cálculo

**Acciones que puede realizar:**
- Buscar distribuidor y período
- Ver histórico de consultas del distribuidor
- Expandir cada consulta para ver detalles del acumulado en ese momento
- Ver cambios entre consultas y sus causas
- Ver auditoría de ingresos manuales realizados
- Generar reporte de investigación para documentar la reclamación
- Exportar auditoría completa (PDF/Excel) para responder al distribuidor
- Crear nota interna con conclusiones

**Restricciones:**
- Solo lectura: No puede modificar datos históricos
- No puede editar valores congelados en HistorialBono
- No puede recalcular períodos ya cerrados
- No puede eliminar o modificar auditoría
- Si descubre errores en cálculo, requiere apertura de ticket a soporte técnico

---

## 1.9 Responsabilidades Bien Definidas (APLICABLES A TODO EL SISTEMA)

```
┌────────────────────────────────────────────────────────────────┐
│ ALDEBARAN - NOTIFICADOR (Scheduled Jobs + Event-Driven):       │
├────────────────────────────────────────────────────────────────┤
│ ✓ Monitorea cambios de tramo en bonos del distribuidor          │
│ ✓ Envía SMS/Email cuando alcanza nuevo nivel (RF29)             │
│ ✓ Envía SMS/Email cuando está cerca del siguiente nivel (RF30)  │
│ ✓ Envía resumen periódico con progreso (RF31 - semanal)         │
│ ✓ Respeta preferencias de canales del distribuidor              │
│ ✓ Respeta desuscripciones (recordatorios)                       │
│ ✓ Registra auditoría: envíos, fallos, canales usados            │
│ ✓ Maneja reintentos si envío falla                              │
│ ✓ Cachea datos para evitar consultas excesivas                  │
│ ✓ Limpia datos de notificaciones antiguas (política retención)  │
│ ✗ NO modifica datos de distribuidores                           │
│ ✗ NO interfiere con cálculo de bonos                            │
│ ✗ NO modifica TOTUS directamente                                │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ PÁGINA PROMOCIONAL (Tercero - Externo):                        │
├────────────────────────────────────────────────────────────────┤
│ ✓ Mantiene lista de precios actualizada (diariamente)          │
│ ✓ Publica lista de precios en formato especificado (Excel)     │
│ ✓ Asegura disponibilidad de descargas (sin downtime)           │
│ ✓ Ofrece link/botón "Ver mi bonificación" (redirección)        │
│ ✓ Redirecciona a Sitio Público Aldebaran (endpoint configurable)
│ ✓ Asegura comunicación HTTPS segura                            │
│ ✗ NO calcula bonos                                             │
│ ✗ NO autentica distribuidores                                  │
│ ✗ NO accede a datos de TOTUS                                   │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ ALDEBARAN - SCHEDULED JOBS (Automático):                       │
├────────────────────────────────────────────────────────────────┤
│ ✓ Descarga lista de precios (horario configurable)             │
│ ✓ Valida estructura y datos de precios                         │
│ ✓ Almacena precios en PreciosDistribuidor (actual)             │
│ ✓ Copia histórico en PreciosDistribuidorHistorico (con fecha)  │
│ ✓ Limpia precios antiguos (según política retención)           │
│ ✓ Notifica errores a administrador                             │
│ ✓ Fallback a precios anteriores si descarga falla              │
│ ✓ Realiza cierre de período (último día, hora configurable)    │
│ ✓ Calcula bonos recomendados al cierre                         │
│ ✓ Almacena FOTO en HistorialBono (inmutable post-cierre)       │
│ ✓ Genera recomendaciones de NC (estado RECOMENDADA)            │
│ ✓ Cierra período (estado CERRADO)                              │
│ ✓ Publica evento PeriodoCerrado (RabbitMQ)                     │
│ ✓ NO realiza conciliación (es manual - CU10)                   │
│ ✗ NO aplica NC en TOTUS                                        │
│ ✗ NO modifica datos de TOTUS                                   │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ ALDEBARAN - MOTOR DE CÁLCULO (En línea):                       │
├────────────────────────────────────────────────────────────────┤
│ ✓ Obtiene período actual                                       │
│ ✓ Suma todas las órdenes (cantidad + precio histórico)         │
│ ✓ Suma todas las entregas confirmadas (cantidad + precio)      │
│ ✓ Consulta TOTUS: Valor facturado real (tiempo real)           │
│ ✓ Busca NC período anterior (de HistorialBono reconciliado)    │
│ ✓ Busca vigencia más reciente (activa)                         │
│ ✓ Busca tramo correspondiente                                  │
│ ✓ Aplica porcentaje del tramo                                  │
│ ✓ Calcula gamificación (falta para siguiente nivel)            │
│ ✓ Retorna bono dinámico (SLA: 500ms)                           │
│ ✓ Registra en auditoría: qué se consultó, cuándo              │
│ ✗ NO precalcula bonos                                          │
│ ✗ NO modifica TOTUS                                            │
│ ✗ NO aplica NC automáticamente                                 │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ ALDEBARAN - CONFIGURACIÓN (Admin):                             │
├────────────────────────────────────────────────────────────────┤
│ ✓ Crea/modifica Períodos (nombre, inicio, duración)            │
│ ✓ Crea Tipos de Bono (base del bono, período asociado)         │
│ ✓ Crea Vigencias (rangos, porcentaje, fecha inicio)            │
│ ✓ Configura horario carga precios                              │
│ ✓ Configura retención histórico de precios                     │
│ ✓ Configura horario y cadencia limpieza                        │
│ ✓ Configura horario cierre período                             │
│ ✓ Configura reintentos y timeouts                              │
│ ✓ Configura integración TOTUS (SP, parámetros)                 │
│ ✓ Configura integración Página Promocional (URL, auth)         │
│ ✓ Gestiona usuarios, roles, permisos                           │
│ ✓ Consulta logs de seguridad                                   │
│ ✗ NO modifica datos históricos (auditoría)                     │
│ ✗ NO interfiere con cierres ya realizados                      │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ USUARIO PROMOS (Humano - Responsable Final):                   │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│ ══ CONSULTA Y REVISIÓN ══                                      │
│ ✓ Consulta recomendación de NC en Aldebaran.Web                │
│ ✓ Revisa montos calculados por el sistema                      │
│ ✓ Accede a historial completo de cálculo                       │
│ ✓ Ve detalles: Vigencia usada, Precios aplicados, NC anterior │
│                                                                │
│ ══ INGRESO MANUAL DE DATOS (CRÍTICO) ══                        │
│ ✓ Puede ingresar manualmente Valor Facturado REAL de TOTUS     │
│   (para casos donde TOTUS no retorna valor en tiempo real)     │
│ ✓ Registra la fecha en que se ingresó este valor               │
│ ✓ Sistema usa valor ingresado en cálculos futuros si aplica    │
│ ✓ Auditoría registra: quién, qué, cuándo ingresó              │
│                                                                │
│ ══ APLICACIÓN EN TOTUS ══                                      │
│ ✓ Abre TOTUS (sistema tercero)                                 │
│ ✓ APLICA MANUALMENTE cada NC recomendada en TOTUS              │
│ ✓ Puede aplicar NC diferente a la recomendada (su decisión)    │
│ ✓ Registra en el sistema: valor NC real que aplicó en TOTUS    │
│ ✓ Confirma que NC fue registrada en TOTUS                      │
│                                                                │
│ ══ RECONCILIACIÓN MANUAL (NO AUTOMÁTICA) ══                    │
│ ✓ Ingresa manualmente: Monto NC que TOTUS confirma aplicó      │
│ ✓ Sistema compara: NC calculada vs NC real ingresada           │
│ ✓ Si hay diferencia: Sistema alerta y registra discrepancia    │
│ ✓ Auditoría: Qué diferencia hubo, por qué, quién la registró   │
│ ✓ Próximos cálculos usan NC real ingresada (no la calculada)   │
│                                                                │
│ ══ SOPORTE A DISTRIBUIDORES ══                                 │
│ ✓ Resuelve reclamaciones de distribuidores                     │
│ ✓ Accede a historial completo de cálculo de cada distribuidor  │
│ ✓ Ve exactamente qué bonos se le entregaron en consultas       │
│ ✓ Justifica los cálculos con datos congelados en HistorialBono │
│                                                                │
│ ══ GENERACIÓN DE REPORTES ══                                   │
│ ✓ Reporte: Bonos calculados vs Bonos realmente aplicados       │
│   - Por período y distribuidor                                 │
│   - Muestra: Monto calculado + Monto aplicado + Diferencias    │
│   - Detecta: NCs aplicadas parcialmente o no aplicadas         │
│                                                                │
│ ✓ Reporte: Distribuidores que consultaron bonos                │
│   - Listado con: Distribuidor, Fecha consulta, Hora, Bono     │
│   - Muestra: Exactamente qué información se le mostró          │
│   - Filtro: Por período, fecha, rango de bonos                 │
│   - Auditoría: Quién consultó, desde dónde, cuándo             │
│                                                                │
│ ✓ Reporte: Discrepancias de NC (calculada vs real)             │
│   - Muestra todas las diferencias encontradas                  │
│   - Detalles: Distribuidor, período, monto diferencia          │
│   - Causa: Quién registró qué y cuándo                         │
│   - Estado: Resuelta o pendiente                               │
│                                                                │
│ ✓ Reporte: Auditoría de acciones del usuario PROMOS            │
│   - Historial: Qué hizo, cuándo, resultado                     │
│   - NCs aplicadas, valores ingresados, reconciliaciones        │
│   - Modificaciones y decisiones tomadas                        │
│                                                                │
│ ✓ Reporte: Precios usados en período                           │
│   - Muestra: Qué lista de precios se usó en cada cálculo       │
│   - Vigencia aplicada para cada tipo de bono                   │
│   - Tramos usados en cálculos finales                          │
│                                                                │
│ ✓ Reporte: Exportación de datos                                │
│   - Excel/PDF de cualquier reporte                             │
│   - Formato configurable según necesidad                       │
│                                                                │
│ ══ RESTRICCIONES ══                                            │
│ ✗ NO calcula bonos (el sistema lo hace)                        │
│ ✗ NO modifica TOTUS directamente desde Aldebaran               │
│ ✗ NO interfiere con cierres automáticos ya realizados          │
│ ✗ NO puede editar valores congelados en HistorialBono          │
│ ✗ NO puede eliminar registros de auditoría                     │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ TOTUS (Tercero - Verdad Única - Sistema Facturación):          │
├────────────────────────────────────────────────────────────────┤
│ ✓ Suministra Valor Facturado (via SP, parámetros configurables)│
│ ✓ Retorna: ValorFacturado, NC, Fletes, Descuentos             │
│ ✓ Recibe recomendación de NC de Aldebaran (sugerencia)         │
│ ✓ Aplica NC en siguiente período (responsabilidad Usuario)     │
│ ✓ Registra NC real aplicada en su BD                           │
│ ✓ Retorna NC real aplicada (para reconciliación Aldebaran)     │
│ ✓ Es fuente de verdad para valor facturado                     │
│ ✓ Es fuente de verdad para NC realmente aplicadas              │
│ ✗ NO calcula bonos                                             │
│ ✗ NO interfiere con cálculos de Aldebaran                      │
│ ✗ NO aplica NC automáticamente (Usuario es responsable)        │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ SITIO PÚBLICO ALDEBARAN (Portal - Distribuidor):               │
├────────────────────────────────────────────────────────────────┤
│ ✓ Autentica distribuidor (OTP por SMS/Email)                   │
│ ✓ Valida documento distribuidor contra BD Aldebaran            │
│ ✓ Genera OTP (6 dígitos, 10 min default, configurable)         │
│ ✓ Envía OTP por canal preferido (SMS o Email)                  │
│ ✓ Valida OTP ingresado (max 3 intentos)                        │
│ ✓ Genera Token JWT (8 horas default, configurable)             │
│ ✓ Consulta Motor de Cálculo (obtiene bono dinámico)            │
│ ✓ Retorna bono dinámico (SLA: 500ms)                           │
│ ✓ Solo muestra información del distribuidor autenticado         │
│ ✓ Página solo lectura (sin entrada de datos)                   │
│ ✓ Gestiona gamificación (falta para siguiente nivel)            │
│ ✓ Registra logs de auditoría (acceso, consultas)               │
│ ✓ Invalida token al cierre de sesión                           │
│ ✗ NO calcula bonos (delega a Motor de Cálculo)                 │
│ ✗ NO aplica NC                                                 │
│ ✗ NO modifica datos en BD                                      │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│ DISTRIBUIDOR (Cliente - Externo):                              │
├────────────────────────────────────────────────────────────────┤
│ ✓ Haz clic en "Ver mi bonificación" (Página Promocional)       │
│ ✓ Ingresa documento (cédula) en Sitio Público                  │
│ ✓ Recibe OTP (SMS o Email)                                     │
│ ✓ Ingresa OTP (máx 3 intentos)                                 │
│ ✓ Consulta su bonificación acumulada                           │
│ ✓ Ve gamificación (falta para siguiente nivel)                 │
│ ✓ Cierra sesión                                                │
│ ✗ NO calcula bonos (sistema lo hace)                           │
│ ✗ NO accede a datos de otros distribuidores                    │
│ ✗ NO accede a Aldebaran.Web (admin interno)                    │
└────────────────────────────────────────────────────────────────┘
````````

## 1.10.1 Funcionales 

| RF | Descripción | Categoría |
|----|---|---|
| RF1 | Gestionar Períodos | Administración |
| RF2 | Gestionar Tipos de Bono | Administración |
| RF3 | Gestionar Vigencias | Administración |
| RF4 | **Autenticar Distribuidor (OTP - Email Interno)** | Seguridad |
| RF5 | Validar Seguridad: Solo distribuidor ve su información | Seguridad |
| RF6 | Consultar Bonificación - Período Actual (Distribuidor - CU7) | Consultas |
| RF7 | Consultar Bono Actual (Admin - Aldebaran.Web - CU9) | Consultas |
| RF8 | Registrar Historial de Bonos (Auditoría completa) | Historial |
| RF9 | Gamificación: Mostrar falta para siguiente nivel | Historial |
| RF10 | Cargar Lista Precios Distribuidores | Integración |
| RF11 | **Capturar Valor Facturado (TOTUS) - CON PARAMETRIZACIÓN OPCIONAL POR ARTÍCULO/REFERENCIA** | Integración |
| RF12 | Capturar Valor Pedido (Aldebaran + Precios) | Integración |
| RF14 | Gestionar Nota Crédito Período Anterior | Integración |
| RF15 | Reconciliación Nota Crédito (TOTUS - Manual) | Integración |
| RF16 | **Ingreso Manual de Órdenes de Compra Especiales (Unitario)** | Usuario PROMOS |
| RF17 | **Carga Masiva de Órdenes de Compra Especiales (CSV)** | Usuario PROMOS |
| RF18 | Aplicación Manual de NC en TOTUS (con Confirmación) | Usuario PROMOS |
| RF19 | Reconciliación Manual de NC (Unitario + CSV Masivo) | Usuario PROMOS |
| RF20 | **Gestión de Aprobaciones para Ingresos Manuales** | Seguridad/Control |
| RF21 | Reporte: Bonos Calculados vs Bonos Aplicados por Período | Reportería |
| RF22 | Reporte: Distribuidores que Consultaron Bonos (Log) | Reportería |
| RF23 | Reporte: Discrepancias de NC (Calculada vs Real) | Reportería |
| RF24 | Reporte: Auditoría de Acciones del Usuario PROMOS | Reportería |
| RF25 | Reporte: Precios y Vigencias Usados en Período | Reportería |
| RF26 | Reporte: Ingresos Manuales Aplicados (OC + Reconciliaciones) | Reportería |
| RF27 | Exportación de Reportes (Excel/PDF) | Reportería |
| RF28 | Consultar Histórico de Bonos - Períodos Anteriores (Distribuidor - CU8) | Consultas |
| RF29 | **Notificación: Alcanzó Nuevo Nivel de Bonificación (Email)** | Notificaciones |
| RF30 | **Notificación: Cerca del Siguiente Nivel (Email - Umbral Configurable)** | Notificaciones |
| RF31 | **Recordatorio Periódico: Progreso de Bonificación (Email)** | Notificaciones |

---

## 1.10.2 No Funcionales

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

## 1.11 MOTOR DE CÁLCULO DE BONIFICACIONES

### 1.11.1 Estructura General de Bonificación

El sistema calcula **3 TIPOS DE BONO INDEPENDIENTES** basados en **2 FUENTES** de información:

```
┌──────────────────────────────────────────────────────────────┐
│                    2 FUENTES DE BONIFICACIÓN                 │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│ FUENTE 1: FACTURACIÓN (de TOTUS)                             │
│ └─ Tipo 1: BONO POR FACTURACIÓN                              │
│    Base: Valor facturado en TOTUS                            │
│    Ejemplo: Si facturó $100M → Bono = $100M × 6% = $6M       │
│                                                              │
│ FUENTE 2: ÓRDENES DE ALDEBARAN (Pedidos + Entregas)          │
│ ├─ Tipo 2: BONO POR PEDIDO                                   │
│ │  Base: Valor total de ÓRDENES PEDIDAS                      │
│ │  Ejemplo: Si pidió por $50M → Bono = $50M × 5% = $2.5M     │
│ │                                                            │
│ └─ Tipo 3: BONO POR ENTREGADO                                │
│    Base: Valor total de lo EFECTIVAMENTE ENTREGADO           │
│    Ejemplo: Si entregó $45M → Bono = $45M × 4% = $1.8M       │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### 1.11.2 Diferencia Clave: Pedido vs Entregado

| Aspecto | Bono por Pedido | Bono por Entregado |
|---------|---|---|
| **Base** | Lo que pidió (cantidad pedida × precio) | Lo que salió del almacén (cantidad entregada × precio) |
| **Timing** | Se calcula al crear la orden | Se calcula cuando se confirma la entrega |
| **Incentivo** | Anima a pedir más | Anima a que confirme lo que pidió |
| **Riesgo** | Puede pedir pero no recibir | Solo cuenta lo recibido |
| **Precio Usado** | Precio del día del pedido (congelado) | Precio del día del pedido (congelado) |
| **Ejemplo** | Pide 100 unidades → Cuenta 100 × $20 | Entrega 80 unidades → Cuenta 80 × $20 |

---

### 1.11.3 BONO POR FACTURACIÓN (TOTUS)

El sistema deberá calcular el valor del Bono por Facturación para cada distribuidor 
en un período determinado. Para ello, tomará como base la facturación sin impuestos 
y aplicará los ajustes definidos por el negocio, incluyendo notas crédito, fletes, 
descuentos, el valor del bono del período anterior y las órdenes de compra especiales 
del período.

#### Fórmula de cálculo

El cálculo del Bono por Facturación deberá seguir la regla siguiente. Para mayor 
trazabilidad y auditoría, se definen variables intermedias:

**Paso 1: Calcular diferencia de Notas Crédito**
```
F = D - E

Donde:
  D = TotalNotasCredito (período actual, de TOTUS)
  E = NC correspondiente al Bono del Período Anterior (valor reconciliado de HistorialBono)
  F = Diferencia neta de Notas Crédito a descontar
```

**Paso 2: Calcular Base para Bono (Total Facturado)**
```
J = C - F - G - H - I

Donde:
  C = Facturación Sin Impuestos 
  F = diferencia calculada en Paso 1
  G = TotalFletes 
  H = TotalDescuentos 
  I = Órdenes de Compra Especiales 
  J = Base para Bono 
```

**Paso 3: Calcular Bono Aplicando Vigencia**
```
L = J * K

Donde:
  K = Porcentaje del rango de valores del periodo aplicable (ej: 6% → 0.06)
  L = Bono por Facturación (resultado final)
```

#### Notas Importantes

1. **Ordenes de compra especiales (I)**: Solo se consideran OC Especiales 
   con estado `APROBADA`. Si una OC está `PENDIENTE` o `RECHAZADA`, NO se incluye.

2. **NC correspondiente al Bono del Periodo Anterior (E) **: Se obtiene de la conciliacion 
   del valor simulado por el sistema el último dia del periodo, con el valor cargado por 
   el usuario en Aldebaran, segun la aplicación final del bono en TOTUS.
 
3. **Valor Negativo (J < 0)**: Si el resultado de J es negativo (caso excepcional donde 
   ajustes superan facturación), el comportamiento por defecto será establecer J = 0, 
   resultando en Bono = 0. Se registrará en auditoría para investigación.

4. **Fuentes de Datos**: 
   - C, D, F, G, H provienen de TOTUS
   - E, I proviene de Aldebaran
   - Si TOTUS no responde, usar fallback definido en integraciones

#### Evaluación de aplicabilidad del bono vigente

Una vez obtenido J (Total Facturado), este deberá evaluarse contra los **rangos de valor** 
en las vigencias que se encuentren activas para los bonos de tipo Facturación. 

Cada vigencia tiene rangos de valor para el minimo y el maximo de facturación, y un porcentaje de bono asociado a cada rango. 

El sistema debe identificar en qué rango cae J y aplicar el porcentaje correspondiente para calcular L (Bono por Facturación).

**Ejemplo de configuración de Vigencia:**
- Bono Facturación Mensual Rango 1: $1M - $5M → Porcentaje de Bono: 5%
- Bono Facturación Mensual Rango 2: $5M - $10M → Porcentaje de Bono: 6%
- Bono Facturación Mensual Rango 3: $10M - $100M → Porcentaje de Bono: 6.5%
- Y así sucesivamente según configuración

**Búsqueda del Rango Aplicable:**
El sistema debe:
1. Evaluar en qué rango de valor cae J (Total Facturado)
2. Obtener el porcentaje K asociado a ese rango
3. Aplicar la fórmula L = J * K

**Ejemplo de Cálculo:**
- Si J = $3.5M → Cae en Rango 1 ($1M-$5M) → K = 5% → L = $3.5M × 0.05 = $175K
- Si J = $7M → Cae en Rango 2 ($5M-$10M) → K = 6% → L = $7M × 0.06 = $420K
- Si J = $25M → Cae en Rango 3 ($10M-$100M) → K = 6.5% → L = $25M × 0.065 = $1.625M

#### Resultado Final

El valor L resultante será el Bono por Facturación para el distribuidor en ese período, 
ya sea calculado dinámicamente en una consulta (CU7) o congelado al cierre del período (CU10).

---

### 1.11.4 BONO POR PEDIDO (Aldebaran)

El Bono por Pedido se calcula con base en los pedidos realizados por el distribuidor durante un período específico. 

Para este cálculo, se toma como insumo el Precio WEB Total del Período y el Descuento por Monto Total del Período, 
con el fin de determinar el valor final del bono que será aplicado como nota crédito.

El cálculo del bono deberá realizarse en las siguientes etapas:

#### 1. Cálculo del Precio WEB Total del Período
El sistema deberá sumar el valor de Precio WEB de todos los pedidos realizados por el distribuidor durante el período.

El Precio WEB de cada pedido corresponde a la sumatoria del producto entre el precio unitario de cada referencia, 
de acuerdo con la fecha del pedido, y la cantidad pedida.

#### 2. Cálculo del Descuento por Monto Total del Período
El sistema deberá sumar los valores de Descuento por Monto aplicados a cada pedido durante el mismo período.

Este descuento puede variar según el pedido o según la referencia, de acuerdo con la configuración definida para el bono.

#### 3. Cálculo del Bono Quincenal
Una vez obtenido el Precio WEB Total del Período, el sistema deberá identificar el rango de valor en el que dicho total 
1. cae, teniendo en cuenta las vigencias activas configuradas para el tipo de bono “Pedido”.

Sobre el valor identificado, se aplicará el porcentaje de bono correspondiente al rango aplicable, obteniendo así el Bono 
Quincenal.

#### 4. Cálculo del Valor Final del Bono por Pedido
Al Bono Quincenal calculado en el paso anterior, se le deberá restar el Descuento por Monto Total del Período.

El resultado de esta operación corresponderá al Valor Final del Bono por Pedido, el cual será aplicado como Nota Crédito.

### Fórmula de cálculo
Para mayor trazabilidad y auditoría, el cálculo puede expresarse de la siguiente manera:

#### Paso 1: Calcular el Precio WEB Total del Período

    A = Σ PrecioWEB de todos los pedidos del período

Donde:

    PrecioWEB de cada pedido = Σ (Precio unitario de cada referencia según fecha del pedido × cantidad pedida)
    A = Precio WEB Total del Período
#### Paso 2: Calcular el Descuento por Monto Total del Período

    B = Σ DescuentoPorMonto de todos los pedidos del período

Donde:

    B = Descuento por Monto Total del Período

#### Paso 3: Calcular el Bono Quincenal

    C = A * D

Donde:

    A = Precio WEB Total del Período
    D = Porcentaje del rango de valor aplicable según la vigencia activa del tipo de bono "Pedido"
    C = Bono Quincenal

#### Paso 4: Calcular el Valor Final del Bono por Pedido

    E = C - B

Donde:
  
    C = Bono Quincenal
    B = Descuento por Monto Total del Período
    E = Valor Final del Bono por Pedido

Ejemplo ilustrativo

Supongamos que durante un período, un distribuidor realizó varios pedidos que suman un Precio WEB Total de $100,000,000 (A) y que el Descuento por Monto Total del Período es de $5,000,000 (B).

```
A = 100,000,000
B = 5,000,000
D = 6% → 0.06

C = A × D
C = 100,000,000 × 0.06
C = 6,000,000

E = C - B
E = 6,000,000 - 5,000,000
E = 1,000,000

```
Resultado final: 

    El valor del Bono por Pedido sería 1,000,000, el cual se aplicará como Nota Crédito.

---

### 1.11.5 BONO POR ENTREGADO (Aldebaran)

El Bono por Entregado se calcula con base en los pedidos realizados por el distribuidor, que fueron entregados durante un período específico. 

Para este cálculo, se toma como insumo el Precio WEB Total del Período y el Descuento por Monto Total del Período, 
con el fin de determinar el valor final del bono que será aplicado como nota crédito.

El cálculo del bono deberá realizarse en las siguientes etapas:

#### 1. Cálculo del Precio WEB Total del Período
El sistema deberá sumar el valor de Precio WEB de todos los pedidos realizados por el distribuidor y que fueron entregados durante el período.

El Precio WEB de cada pedido corresponde a la sumatoria del producto entre el precio unitario de cada referencia, 
de acuerdo con la fecha del pedido, y la cantidad entregada de la referencia.

#### 2. Cálculo del Descuento por Monto Total del Período
El sistema deberá sumar los valores de Descuento por Monto aplicados a cada pedido durante el mismo período.

Este descuento puede variar según el pedido o según la referencia, de acuerdo con la configuración definida para el bono.

#### 3. Cálculo del Bono Quincenal
Una vez obtenido el Precio WEB Total del Período, el sistema deberá identificar el rango de valor en el que dicho total 
1. cae, teniendo en cuenta las vigencias activas configuradas para el tipo de bono “Entregado”.

Sobre el valor identificado, se aplicará el porcentaje de bono correspondiente al rango aplicable, obteniendo así el Bono 
Quincenal.

#### 4. Cálculo del Valor Final del Bono por Pedido
Al Bono Quincenal calculado en el paso anterior, se le deberá restar el Descuento por Monto Total del Período.

El resultado de esta operación corresponderá al Valor Final del Bono por Entregado, el cual será aplicado como Nota Crédito.

### Fórmula de cálculo
Para mayor trazabilidad y auditoría, el cálculo puede expresarse de la siguiente manera:

#### Paso 1: Calcular el Precio WEB Total del Período

    A = Σ PrecioWEB de todos los pedidos entregados del período

Donde:

    PrecioWEB de cada pedido = Σ (Precio unitario de cada referencia según fecha del pedido × cantidad Entregada)
    A = Precio WEB Total del Período
#### Paso 2: Calcular el Descuento por Monto Total del Período

    B = Σ DescuentoPorMonto de todos los pedidos entregados del período

Donde:

    B = Descuento por Monto Total del Período

#### Paso 3: Calcular el Bono Quincenal

    C = A * D

Donde:

    A = Precio WEB Total del Período
    D = Porcentaje del rango de valor aplicable según la vigencia activa del tipo de bono "Pedido"
    C = Bono Quincenal

#### Paso 4: Calcular el Valor Final del Bono por Pedido

    E = C - B

Donde:
  
    C = Bono Quincenal
    B = Descuento por Monto Total del Período
    E = Valor Final del Bono por Entregado

Ejemplo ilustrativo

Supongamos que durante un período, un distribuidor realizó varios pedidos de los cuales fueron entregadas algunas cantidades sumando un Precio WEB Total de $100,000,000 (A) y que el Descuento por Monto Total del Período es de $5,000,000 (B).

```
A = 100,000,000
B = 5,000,000
D = 6% → 0.06

C = A × D
C = 100,000,000 × 0.06
C = 6,000,000

E = C - B
E = 6,000,000 - 5,000,000
E = 1,000,000

```
Resultado final: 

    El valor del Bono por Pedido sería 1,000,000, el cual se aplicará como Nota Crédito.


---

### 1.11.5 Comparativa de los 2 Bonos

| Aspecto | Facturación | Pedido |
|---------|---|---|
| **Fuente** | TOTUS (tercero) | Aldebaran Órdenes |
| **Base** | Facturado - NC Período - NC Anterior + OC Esp. | Cantidad Pedida × Precio |
| **Dinámico** | NO (se congela al cierre) | SÍ (recalcula cada consulta) |
| **Descuentos NC** | SÍ (aplica NC) | NO |
| **OC Especiales** | SÍ (SOLO aquí) | NO |
| **Precio Usado** | N/A (de TOTUS) | Del día del pedido |
| **Incentiva** | Facturar más | Pedir más |
| **Ejemplo** | $100M facturado → $6M bono | $50M pedido → $2.5M bono |

---

### 1.11.6 Resumen: BONO TOTAL = Facturación + Pedido

```
┌──────────────────────────────────────────────────────────────┐
│ DISTRIBUIDOR DIST-001 - PERÍODO 1 AL 15 DE ENERO             │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│ ✓ BONO POR FACTURACIÓN:                                      │
│   Base: $95.5M (facturado menos NC + OC Especiales)          │
│   Porcentaje: 6% (según vigencia)                            │
│   BONO FACTURACIÓN: $5,730,000                               │
│                                                              │
│ ✓ BONO POR PEDIDO:                                           │
│   Base: $18M (total pedido acumulado)                        │
│   Porcentaje: 7% (según vigencia)                            │
│   BONO PEDIDO: $1,260,000                                    │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ ➜ BONO TOTAL = $5.73M + $1.26M = $6,990,000                 │
│                                                              │
│ ➜ Se genera una NOTA CRÉDITO de $6.99M                      │
│   Para aplicar en período siguiente en TOTUS                 │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```
---
## 1.12 Entregables 

### 1.12.1 ✅ Capacidades Entregables

| Funcionalidad | Descripción |
|---|---|
| **Consulta de Bonos** | Distribuidores ven bonos en tiempo real vía OTP (Sitio Público) |
| **Dos Tipos de Bonos** | Facturación (TOTUS) + Pedido (Aldebaran) |
| **Ingreso Manual** | OC Especiales + Reconciliación de NC (unitario o CSV masivo) |
| **Seguridad** | OTP + Aislamiento de datos + Auditoría completa |
| **Reportería** | 6 reportes + Exportación Excel/PDF |
| **Automatización** | Descarga de precios + Cierre de períodos + Consulta de Facturación|

### 1.12.2 📊 34 Requisitos Funcionales Definidos

- **6 de Administración** (RF1-RF3, RF32-RF34)
- **3 de Seguridad** (RF4-RF5, RF20)
- **3 de Consultas** (RF6-RF7, RF28)
- **2 de Historial** (RF8-RF9)
- **6 de Integración** (RF10-RF15)
- **5 de Operaciones Usuario** (RF16-RF20)
- **7 de Reportería** (RF21-RF27)
- **3 de Notificaciones** (RF29-RF31)

## 1.13 🎯 Diferenciales del Proyecto

✅ Transparencia: Distribuidores ven exactamente cómo se calcula su bono  
✅ Automatización: Elimina cálculos manuales y reduce tiempo 70%  
✅ Precisión: Auditoría completa, reconciliación automática de NC  
✅ Control: Aprobaciones configurables para ingresos manuales  
✅ Flexibilidad: Ingreso manual unitario y masivo (CSV)  
✅ Escalabilidad: Soporta múltiples bonos simultáneamente  
