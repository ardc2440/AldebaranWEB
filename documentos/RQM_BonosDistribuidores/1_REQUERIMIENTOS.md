# 1. REQUERIMIENTOS FUNCIONALES - Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS | **Estado**: REQUERIMIENTOS DEFINIDOS | **Fecha**: 2026

---

## 1.1 📚 GLOSARIO - Términos Clave del Documento

### 1.1.1 🎭 ¿Qué es un CASO DE USO (CU)?

Un **Caso de Uso** describe **UN FLUJO COMPLETO DE NEGOCIO** desde la perspectiva del usuario/actor.

- **Enfoque:** ¿Qué hace el ACTOR? ¿Cuál es el escenario de negocio?
- **Ejemplo:** CU7 = "Consultar Bonificación - Período Actual"
  - Un distribuidor autenticado CONSULTA su bono dinámicamente
  - Es UN proceso completo: se autentica → consulta → ve bonos → cierra sesión

**Total en este proyecto: 12 Casos de Uso (CU1 a CU12)**

### 1.1.2 ⚙️ ¿Qué es un REQUISITO FUNCIONAL (RF)?

Un **Requisito Funcional** describe **UNA CAPACIDAD ESPECÍFICA** que el sistema DEBE tener.

- **Enfoque:** ¿Qué DEBE HACER el SISTEMA? ¿Cuál es la funcionalidad concreta?
- **Ejemplo:** RF6 = "Consultar Bonificación - Período Actual"
  - El sistema DEBE calcular bono dinámicamente en 500ms
  - El sistema DEBE retornar desglose por tipo de bono
  - El sistema DEBE mostrar gamificación

**Total en este proyecto: 26 Requisitos Funcionales (RF1 a RF26)**

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
│ ├─ RF13: Capturar Valor Entregado                           │
│ └─ RF4: Autenticar Distribuidor (OTP)                       │
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
| **CU1** | Crear Período | RF1 | RF2, RF3 | RF8 |
| **CU2** | Crear Tipo de Bono | RF2 | RF1, RF3 | RF8 |
| **CU3** | Crear Vigencia | RF3, RF11 | RF1, RF2, RF12, RF13 | RF8, RF24 |
| **CU4** | Obtener Facturación (TOTUS) | RF11, RF14 | RF10 | RF8 |
| **CU5** | Cargar Precios | RF10 | RF12, RF13 | RF8, RF24 |
| **CU6** | Autenticar Distribuidor | RF4, RF5 | - | RF8 |
| **CU7** | Consultar Bonificación (Actual) | RF6, RF11, RF12, RF13, RF9 | RF4, RF5, RF10, RF14 | RF8, RF21 |
| **CU8** | Consultar Histórico (Anterior) | RF28, RF14, RF15 | RF5 | RF8, RF21 |
| **CU9** | Consultar Bono (Admin) | RF7, RF11, RF12, RF13 | RF1, RF2, RF3, RF14 | RF8, RF23, RF24 |
| **CU10** | Cierre de Período (Automático) | RF7, RF11, RF12, RF13, RF14, RF15 | RF10, RF16-A, RF16-B | RF8, RF23 |
| **CU11** | Reconciliación NC (Manual) | RF15, RF17, RF18, RF19 | RF14 | RF8, RF22, RF23, RF25 |
| **CU12** | Resolver Reclamación (Soporte) | RF8, RF21, RF22, RF23, RF24 | RF7 | RF20, RF25, RF26 |

---

### 1.3 Vista por Categoría de RF

#### 1.3.1 🔧 **ADMINISTRACIÓN** (RF1-RF3)

```
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
RF4 (Autenticar por OTP)
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
├─ CU5 ━━━━ Carga automática diaria (entrada principal)
├─ CU7 ━━━ Usa precios históricos en cálculos
├─ CU9 ━━━ Admin ve precios usados
└─ CU24 ━━ Auditoría de precios

RF11 (Capturar Facturación TOTUS)
├─ CU4 ━━━━ Obtiene de TOTUS (entrada principal)
├─ CU7 ━━━ Calcula Bono por Facturación
├─ CU9 ━━━ Admin ve valor facturado
└─ CU10 ━━ Cierre calcula con facturación final

RF12 (Capturar Valor Pedido)
├─ CU7 ━━━ Calcula Bono por Pedido (entrada principal)
├─ CU9 ━━━ Admin ve valor pedido
└─ CU10 ━━ Cierre calcula con pedidos acumulados

RF13 (Capturar Valor Entregado)
├─ CU7 ━━━ Calcula Bono por Entregado (entrada principal)
├─ CU9 ━━━ Admin ve valor entregado
└─ CU10 ━━ Cierre calcula con entregas confirmadas

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
| **TOTAL** | **29 RF** | **12 CU** | **Todos relacionados** |

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
Automatizar el cálculo de bonificaciones para distribuidores en la empresa PROMOS con tres modalidades:
- **Bonificación por Facturación**: Incentivo basado en valor total facturado en período (TOTUS)
- **Bonificación por Pedido**: Incentivo basado en valor total pedido en período (Cantidad pedida × Precio)
- **Bonificación por Entregado**: Incentivo basado en valor total entregado en período (Cantidad entregada × Precio)

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
| | | Recibe OTP por SMS/Email (lifetime configurable) |
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

### 1.8.1 CU1: Crear Período

**Ubicación:** Aldebaran.Web  
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Administrador  
**Objetivo:** Definir un nuevo período (ventana de tiempo) para calcular bonificaciones

**Problemas que resuelve:**
- Sistema necesita conocer cuándo inicia y termina cada período de bonificación
- Permite organizar bonos por períodos (Enero 1-15, Enero 16-31, etc.)
- Cada período tiene su propia configuración de vigencias y cálculos

**Información que debe poder ingresar:**
- Nombre del período (texto identificador único, ej: "Enero 2026 - Quincena 1")
- Tipo de período (Mensual / Quincenal / Semanal / Custom)
- Día de inicio (primer día del período)
- Duración en días (cuántos días tiene el período)
- Fecha inicio (DD/MM/YYYY)
- Fecha fin (DD/MM/YYYY)
- Estado (Activo / Inactivo)

**Acciones que puede realizar:**
- Crear nuevo período
- Modificar período (antes de que comience a ser usado)
- Ver listado de períodos activos e históricos
- Activar/Desactivar período
- Consultar fechas exactas de cada período
- Ver cuántos distribuidores se benefician de este período

**Restricciones:**
- No puede editar período que ya está cerrado
- No puede crear período con fecha de inicio en el pasado
- No puede crear período que se superponga con otro existente
- No puede eliminar período que tiene bonos ya calculados
- No puede cambiar duración de período activo

---

### 1.8.2 CU2: Crear Tipo de Bono

**Ubicación:** Aldebaran.Web
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Administrador  
**Objetivo:** Definir un tipo de bono especificando en qué insumo se basa (Facturación, Pedido o Entregado)

**Problemas que resuelve:**
- Sistema debe saber qué tipos de bonificación están disponibles
- Permite estructurar bonos por diferentes criterios de incentivo
- Cada tipo de bono afecta diferentes comportamientos del distribuidor

**Información que debe poder ingresar:**
- Nombre del tipo de bono (texto único, ej: "Bono por Facturación")
- Descripción (opcional, ej: "Incentivo basado en valor facturado")
- Afectación (qué insumo usa: Facturación / Pedido / Entregado)
- Período al cual aplica (referencia a CU1)
- Estado (Activo / Inactivo)
- Orden de aplicación (si hay múltiples, cuál se aplica primero)

**Acciones que puede realizar:**
- Crear nuevo tipo de bono
- Modificar tipo de bono (antes de usar en vigencia)
- Ver listado de tipos disponibles
- Activar/Desactivar tipo de bono
- Consultar cuántas vigencias usan este tipo
- Ver historial de bonos calculados por tipo

**Restricciones:**
- No puede eliminar tipo si ya tiene bonos calculados
- No puede cambiar "Afectación" si ya tiene bonos activos
- No puede cambiar período de un tipo activo
- No puede tener dos tipos con mismo nombre en mismo período
- No puede crear tipo sin asignar Afectación

---

### 1.8.3 CU3: Crear Vigencia (AMPLIADO CON PARAMETRIZACIÓN GRANULAR POR ARTÍCULO/REFERENCIA)

**Ubicación:** Aldebaran.Web
**Acceso:** Admin - Autenticación interna PROMOS  
**Actor:** Administrador  
**Objetivo:** Definir una vigencia (configuración de rango de valor y porcentaje de aplicación del bono) para calcular bonos, con opción de restricción por artículos/referencias

**Problemas que resuelve:**
- Sistema necesita saber qué porcentaje de bono aplica según el acumulado del distribuidor
- Permite cambiar incentivos según necesidades de negocio (stock, lanzamientos, etc.)
- Permite focalizar bonos en artículos específicos sin modificar toda la estructura

**Información que debe poder ingresar:**
- Nombre de vigencia (texto único, ej: "Bono Enero 2026 - Facturación")
- Período al cual aplica (referencia a CU1)
- Tipo de bono (referencia a CU2)
- Fecha inicio vigencia (cuándo comienza a usarse)
- Estado (Activo / Inactivo)
- Tramos de valor con porcentajes (ej: "$1M-$5M = 5%", "$5M-$10M = 6%")
- Opción: Restricción por artículos/referencias específicos (OPCIONAL):
  - Sin restricción (aplica a TODOS los artículos - DEFAULT)
  - Artículos específicos (TODAS sus referencias)
  - Artículos + Referencias específicas (combinación personalizada)
  - Todas referencias de un artículo (WILDCARD)
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
- No puede editar vigencia que ya está en uso (debe crear nueva)
- No puede cambiar Tipo de Bono de vigencia activa
- No puede cambiar Período de vigencia activa
- No puede crear vigencia con fecha inicio en el pasado
- No puede tener dos vigencias simultáneas para mismo Tipo de Bono en mismo Período (excepto con parametrización diferente)
- Si parametriza por artículos, debe validar que artículos existan en sistema
- No puede eliminar vigencia que tiene bonos ya calculados

### 1.8.4 CU4: Obtener Facturación de TOTUS (Integración)

**Ubicación:** Backend Aldebaran (Motor de Cálculo)
**Cuándo:** Dinámicamente (cada vez que se calcula bono) + Al cierre del período  
**Actor:** PROCESO AUTOMÁTICO + MOTOR DE CÁLCULO  
**Objetivo:** Obtener valor facturado real desde el sistema TOTUS para usarlo en cálculo de bonos

**Problemas que resuelve:**
- Sistema necesita valor facturado como fuente de verdad (TOTUS es referencia única)
- Debe poder filtrar por artículos/referencias si la vigencia está parametrizada
- Debe manejar fallos de conectividad sin interrumpir operación

**Información que necesita:**
- Tipo documento: "FAC" (Factura)
- Número documento: Cédula del distribuidor
- Fecha inicio: Primer día del período
- Fecha fin: Último día del período o hoy
- Lista de artículos (OPCIONAL - si vigencia está parametrizada)
- Mapa de referencias por artículo (OPCIONAL - si vigencia está parametrizada)

**Información que retorna de TOTUS:**
- ValorTotalFacturadoSinImpuestos (obligatorio)
- TotalNotasCredito (obligatorio)
- TotalFletes (opcional)
- TotalDescuentos (opcional)

**Acciones que puede realizar:**
- Consultar SP configurable en TOTUS
- Usar MOCK configurable si SP no está disponible
- Filtrar facturación por artículos específicos (si aplica)
- Cachear resultado para consultas posteriores (mismo día)
- Registrar auditoría de cada consulta
- Usar fallback (valor anterior) si TOTUS no responde

**Restricciones:**
- SLA máximo 500ms (incluida consulta TOTUS) para cálculos dinámicos
- No puede usar valores negativos
- No puede ignorar filtros de artículos/referencias
- No puede modificar valores en TOTUS (solo lectura)
- No puede almacenar datos sensibles en caché

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
- Código OTP recibido por SMS o Email

**Información que valida:**
- Documento existe en BD Aldebaran
- Es tipo "DISTRIBUIDOR" (no otro tipo de cliente)
- Tiene Email o Celular configurados

**Información que genera:**
- Código OTP de 6 dígitos único
- Token de sesión JWT

**Acciones que puede realizar:**
- Enviar OTP por SMS al celular del distribuidor
- Enviar OTP por Email a direcciones configuradas
- Permitir reintentos (máx 3 intentos)
- Crear sesión con token JWT válido 8 horas
- Invalidar OTP anterior si distribuidor solicita otro
- Registrar intento de autenticación (exitoso/fallido)
- Rechazar acceso después de 3 intentos fallidos

**Restricciones:**
- OTP válido solo 10 minutos (configurable)
- Máximo 3 intentos fallidos
- No puede autenticar si no hay Email ni Celular
- No puede guardar OTP en texto plano
- No puede permitir acceso sin validar OTP
- No puede reutilizar OTP ya usado
- Token expira después de 8 horas (configurable)

---

### 1.8.7 CU7: Consultar Bonificación - Período Actual (Distribuidor - Sitio Público)

**Ubicación:** Sitio Público Aldebaran
**Acceso:** Solo con autenticación OTP válida (CU6 completado)  
**Actor:** Distribuidor (autenticado)  
**Objetivo:** Consultar bonos acumulados en tiempo real durante el período actual

**Problemas que resuelve:**
- Distribuidor necesita ver su bono actualizado según lo que ha pedido/entregado/facturado en el período EN CURSO
- Debe conocer cuánto falta para acceder al siguiente nivel de bonificación (gamificación)
- Necesita transparencia total sin contactar a PROMOS
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

**Información que calcula dinámicamente:**
- Bono por Facturación: Base = Facturado - NC_Período - NC_Anterior; Bono = Base × % Vigencia
- Bono por Pedido: Base = Suma órdenes; Bono = Base × % Vigencia
- Bono por Entregado: Base = Suma entregas confirmadas; Bono = Base × % Vigencia
- Gamificación: Diferencia entre acumulado actual y siguiente tramo

**Acciones que puede realizar:**
- Ver bonos acumulados en tiempo real (SLA: 500ms)
- Ver desglose por tipo de bono (Facturación, Pedido, Entregado)
- Ver gamificación (falta para siguiente nivel)
- Ver período actual y días transcurridos
- Consultar múltiples veces (cada consulta recalcula)
- Cerrar sesión

**Restricciones:**
- Página solo lectura (sin ingreso de datos)
- No puede ver información de otro distribuidor
- Token debe estar válido (no expirado)
- Cálculo es dinámico (se ejecuta cada consulta, no precalculado)
- Cambios en pedidos/entregas se reflejan en próxima consulta
- No puede acceder a datos administrativos (Aldebaran.Web)
- No puede ver períodos anteriores (ver CU8 para eso)

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
- Bono por Entregado (desglosado)
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
**Cuándo:** Último día del período, a hora configurada (ej: 23:59:59)  
**Actor:** PROCESO AUTOMÁTICO  
**Objetivo:** Cerrar período y calcular bonos finales recomendados (FOTO congelada)

**Problemas que resuelve:**
- Necesario congelar cálculos al final del período para auditoría
- Genera recomendación de NC para que Usuario PROMOS la aplique en TOTUS
- Garantiza inmutabilidad de datos post-cierre para cumplimiento normativo

**Información que necesita:**
- Período actual (verificar que es el último día)
- Todos los distribuidores con actividad en el período
- Órdenes, entregas y facturación acumulada
- Vigencias activas
- Precios del período

**Información que genera:**
- FOTO congelada en HistorialBono (inmutable post-cierre)
- Bono Recomendado (cálculo final del período)
- Recomendación de NC para aplicar en siguiente período
- Evento de cierre (para notificación)
- Estado del período: CERRADO

**Acciones que realiza automáticamente:**
- Calcula bono RECOMENDADO del período (período completo, no dinámico)
- Almacena FOTO en HistorialBono (estado CALCULADO)
- Genera recomendación de NC con estado RECOMENDADA
- Marca período como CERRADO
- Publica evento de cierre (RabbitMQ)
- Notifica Usuario PROMOS (configuración pendiente)
- Registra auditoría completa

**Restricciones:**
- No puede modificar datos después del cierre (inmutabilidad)
- No aplica NC automáticamente en TOTUS (solo recomienda)
- No puede cerrar período que ya está cerrado
- Usuario PROMOS es responsable de aplicar NC en TOTUS
- No puede recalcular período cerrado (la FOTO es definitiva)

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
   - Bono mostrado en cada consulta (desglosado por tipo: Facturación, Pedido, Entregado)
   - Acumulado de insumos en cada momento (Facturación, Pedidos, Entregas, OC Especiales)

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
│ ✓ Crea Tipos de Bono (afectación, estrategia precio)           │
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
| RF4 | **Autenticar Distribuidor (OTP - SMS/Email)** | Seguridad |
| RF5 | Validar Seguridad: Solo distribuidor ve su información | Seguridad |
| RF6 | Consultar Bonificación - Período Actual (Distribuidor - CU7) | Consultas |
| RF7 | Consultar Bono Actual (Admin - Aldebaran.Web - CU9) | Consultas |
| RF8 | Registrar Historial de Bonos (Auditoría completa) | Historial |
| RF9 | Gamificación: Mostrar falta para siguiente nivel | Historial |
| RF10 | Cargar Lista Precios Distribuidores | Integración |
| RF11 | **Capturar Valor Facturado (TOTUS) - CON PARAMETRIZACIÓN OPCIONAL POR ARTÍCULO/REFERENCIA** | Integración |
| RF12 | Capturar Valor Pedido (Aldebaran + Precios) | Integración |
| RF13 | Capturar Valor Entregado (Aldebaran) | Integración |
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

#### Conceptos Clave

- **Fuente**: Órdenes creadas en Aldebaran
- **Base de Cálculo**: Cantidad PEDIDA × Precio histórico del día del pedido
- **Precio**: Se congela el día que se crea el pedido (NO cambia después)
- **Descuentos**: NO aplican descuentos por NC (eso es solo para Facturación)
- **Insumos Adicionales**: NINGUNO (OC Especiales NO aplican aquí)
- **Dinámico**: Se RECALCULA CADA VEZ que el distribuidor consulta

#### Fórmula

```
Para CADA orden del período:

  1. Obtiene de Orden: Referencia + Cantidad + FechaPedido
     Ej: REF-001, 100 unidades, Día 5 del período

  2. Busca en HISTÓRICO DE PRECIOS el precio vigente ESE DÍA
     Referencia = REF-001
     FechaCarga ≤ FechaPedido
     Selecciona el precio MÁS RECIENTE antes de esa fecha

  3. Obtiene Precio y Descuento de ese día
     PrecioUnitario = $20 (del día 5)
     DescuentoDistribuidor = 10%

  4. Calcula precio con descuento
     PrecioConDescuento = PrecioUnitario * (1 - DescuentoDistribuidor)
     PrecioConDescuento = $20 * (1 - 0.10) = $18

  5. Calcula valor de la orden
     Cantidad = 100 unidades
     ValorOrden = Cantidad * PrecioConDescuento
     ValorOrden = 100 * $18 = $1,800

  6. ACUMULA para el período completo
     ValorTotalPedido = SUM(todas las órdenes del período)
     Ej: $14,400,000

  7. Busca Vigencia más reciente (fecha ≤ hoy, estado Activo)
     Vigencia del período actual

  8. Busca el RANGO DE VALOR en la Vigencia que contiene ValorTotalPedido
     La Vigencia tiene rangos de valor configurables:
       Rango 1: $1M - $5M → Porcentaje de Bono: 5%
       Rango 2: $5M - $10M → Porcentaje de Bono: 6%
       Rango 3: $10M - $20M → Porcentaje de Bono: 7%
       Rango 4: >$20M → Porcentaje de Bono: 8%

     ValorTotalPedido = $14,400,000 → Cae en Rango 3 ($10M-$20M) → Porcentaje = 7%

  9. Aplica el porcentaje del rango
     Bono = ValorTotalPedido × Porcentaje
     Bono = $14,400,000 × 0.07 = $1,008,000

  10. RESULTADO (DINÁMICO - CAMBIA CON CADA CONSULTA)
      Distribuidor DIST-001 acumula: $1,008,000 como Bono por Pedido
      (Este valor CAMBIA si ingresa más órdenes antes del cierre)
```

#### Ejemplo Temporal - Período 1 al 15

```
DÍA 1: Distribuidor consulta
  Rango: 1 al 1
  Pedidos acumulados: $1M
  Bono calculado: $50K

DÍA 5: Distribuidor consulta nuevamente
  Rango: 1 al 5
  Pedidos acumulados: $8M (sumó 4 días más)
  Bono calculado: $500K (CAMBIÓ porque hay más pedidos)

DÍA 11: Distribuidor consulta
  Rango: 1 al 11
  Pedidos acumulados: $14M (sumó 6 días más)
  Bono calculado: $1M (CAMBIÓ nuevamente)

DÍA 15 (CIERRE): Sistema calcula FINAL
  Rango: 1 al 15 (período completo)
  Pedidos acumulados: $18M
  Bono FINAL: $1.3M
  Este valor se CONGELA en HistorialBono (inmutable post-cierre)
```

---

### 1.11.5 BONO POR ENTREGADO (Aldebaran)

#### Conceptos Clave

- **Fuente**: Entregas confirmadas en Aldebaran
- **Base de Cálculo**: Cantidad EFECTIVAMENTE ENTREGADA × Precio histórico del pedido original
- **Precio**: Se usa el precio CONGELADO del día que se creó el pedido (NO el actual)
- **Descuentos**: NO aplican descuentos por NC (eso es solo para Facturación)
- **Entregas Parciales**: Se acumulan cada entrega con su propio precio congelado
- **Insumos Adicionales**: NINGUNO (OC Especiales NO aplican aquí)
- **Dinámico**: Se RECALCULA CADA VEZ que el distribuidor consulta

#### Proceso Real de PROMOS

```
1. Distribuidor crea PEDIDO (Orden de Compra)
   ├─ Registra: Artículo, Cantidad pedida
   ├─ Sistema obtiene: Precio histórico del pedido (del día)
   ├─ Se CONGELA ese precio
   └─ Estado: PEDIDO CREADO

2. PROMOS prepara en almacén (puede tomar 1, 3, 8 o N días)
   └─ Separa mercancía, empaca

3. PROMOS realiza ENTREGA/SALIDA
   ├─ Usuario ingresa a Aldebaran
   ├─ Marca: Cantidad entregada (puede ser parcial o total)
   ├─ Registra: Cantidad que realmente salió del almacén
   ├─ Genera: Guía de remisión/Documento de salida
   ├─ Usa: Precio histórico del pedido (el que se registró hace N días)
   └─ Estado: ENTREGA CONFIRMADA

4. Valuación de lo entregado (CRÍTICO)
   └─ Cantidad entregada × Precio unitario del MOMENTO DEL PEDIDO
   └─ NO se usa el precio actual de hoy
   └─ Se usa el precio que existía cuando se creó el pedido
```

#### Fórmula

```
Para CADA entrega confirmada en el período:

  1. Obtiene: Referencia + Cantidad ENTREGADA + Precio CONGELADO del pedido
     Ej: REF-001, 80 unidades entregadas, $20 (del día del pedido)

  2. Calcula valor con descuento
     PrecioConDescuento = $20 * (1 - 0.10) = $18

  3. Calcula valor de la entrega
     Cantidad Entregada = 80 unidades
     ValorEntrega = 80 * $18 = $1,440

  4. ACUMULA todas las entregas del período
     ValorTotalEntregado = SUM(todas las entregas confirmadas)
     Ej: $6,600,000

  5. Busca Vigencia más reciente (fecha ≤ hoy, estado Activo)
     Vigencia del período actual

  6. Busca el RANGO DE VALOR en la Vigencia que contiene ValorTotalEntregado
     La Vigencia tiene rangos de valor configurables:
       Rango 1: $1M - $5M → Porcentaje de Bono: 4%
       Rango 2: $5M - $10M → Porcentaje de Bono: 5%
       Rango 3: >$10M → Porcentaje de Bono: 6%

     ValorTotalEntregado = $6,600,000 → Cae en Rango 2 ($5M-$10M) → Porcentaje = 5%

  7. Aplica el porcentaje del rango
     Bono = ValorTotalEntregado × Porcentaje
     Bono = $6,600,000 × 0.05 = $330,000

  8. RESULTADO (DINÁMICO - CAMBIA CON CADA NUEVA ENTREGA)
     Distribuidor DIST-001 acumula: $330,000 como Bono por Entregado
     (Este valor CAMBIA si se confirman más entregas antes del cierre)
```

#### Ejemplo Temporal - Período 1 al 15

```
DÍA 1: Crea Pedido
  Pedido 1: 100 unidades, Precio $20 (congelado)

DÍA 5: Distribuidor consulta
  Entregas confirmadas: 0
  Bono por Entregado: $0

DÍA 8: Confirma ENTREGA PARCIAL
  Entrega 80 unidades de Pedido 1 a $20 = $1,600
  Distribuidor consulta: Bono por Entregado = $80K

DÍA 10: Confirma ENTREGA REST ANTE
  Entrega 20 unidades más de Pedido 1 a $20 = $400
  Distribuidor consulta: Bono por Entregado = $100K (CAMBIÓ)

DÍA 12: Crea y entrega Pedido 2
  Pedido 2: 50 unidades, Precio $100 (congelado)
  Entrega 50 unidades de Pedido 2 a $100 = $5,000
  Distribuidor consulta: Bono por Entregado = $300K (CAMBIÓ nuevamente)

DÍA 15 (CIERRE): Sistema calcula FINAL
  Total Entregado: $6,600
  Bono FINAL: $330K
  Este valor se CONGELA en HistorialBono
```

---

### 1.11.6 Comparativa de los 3 Bonos

| Aspecto | Facturación | Pedido | Entregado |
|---------|---|---|---|
| **Fuente** | TOTUS (tercero) | Aldebaran Órdenes | Aldebaran Entregas |
| **Base** | Facturado - NC Período - NC Anterior + OC Esp. | Cantidad Pedida × Precio | Cantidad Entregada × Precio |
| **Dinámico** | NO (se congela al cierre) | SÍ (recalcula cada consulta) | SÍ (recalcula cada consulta) |
| **Descuentos NC** | SÍ (aplica NC) | NO | NO |
| **OC Especiales** | SÍ (SOLO aquí) | NO | NO |
| **Precio Usado** | N/A (de TOTUS) | Del día del pedido | Del día del pedido |
| **Incentiva** | Facturar más | Pedir más | Entregar lo pedido |
| **Ejemplo** | $100M facturado → $6M bono | $50M pedido → $2.5M bono | $45M entregado → $1.8M bono |

---

### 1.11.7 Resumen: BONO TOTAL = Facturación + Pedido + Entregado

```
┌──────────────────────────────────────────────────────────────┐
│ DISTRIBUIDOR DIST-001 - PERÍODO 1 AL 15 DE ENERO             │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│ ✓ BONO POR FACTURACIÓN:                                      │
│   Base: $95.5M (facturado menos NC + OC Especiales)          │
│   Porcentaje: 60% (según vigencia)                           │
│   BONO FACTURACIÓN: $57,300,000                              │
│                                                              │
│ ✓ BONO POR PEDIDO:                                           │
│   Base: $18M (total pedido acumulado)                        │
│   Porcentaje: 7% (según vigencia)                            │
│   BONO PEDIDO: $1,260,000                                    │
│                                                              │
│ ✓ BONO POR ENTREGADO:                                        │
│   Base: $6.6M (total entregado acumulado)                    │
│   Porcentaje: 5% (según vigencia)                            │
│   BONO ENTREGADO: $330,000                                   │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ ➜ BONO TOTAL = $57.3M + $1.26M + $0.33M = $58,890,000       │
│                                                              │
│ ➜ Se genera una NOTA CRÉDITO de $58.89M                     │
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
| **Tres Tipos de Bonos** | Facturación (TOTUS) + Pedido (Aldebaran) + Entregado (Entregas) |
| **Ingreso Manual** | OC Especiales + Reconciliación de NC (unitario o CSV masivo) |
| **Seguridad** | OTP + Aislamiento de datos + Auditoría completa |
| **Reportería** | 6 reportes + Exportación Excel/PDF |
| **Automatización** | Descarga de precios + Cierre de períodos + Consulta de Facturación|

### 1.12.2 📊 28 Requisitos Funcionales Definidos

- **3 de Administración** 
- **2 de Seguridad** 
- **3 de Consultas** 
- **2 de Historial** 
- **6 de Integración** 
- **5 de Operaciones Usuario
- **7 de Reportería

## 1.13 🎯 Diferenciales del Proyecto

✅ Transparencia: Distribuidores ven exactamente cómo se calcula su bono  
✅ Automatización: Elimina cálculos manuales y reduce tiempo 70%  
✅ Precisión: Auditoría completa, reconciliación automática de NC  
✅ Control: Aprobaciones configurables para ingresos manuales  
✅ Flexibilidad: Ingreso manual unitario y masivo (CSV)  
✅ Escalabilidad: Soporta múltiples bonos simultáneamente  
