# 1. REQUERIMIENTOS FUNCIONALES - Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS | **Estado**: ? REQUERIMIENTOS DEFINIDOS | **Fecha**: 2024

---

## 1.1 Descripción General

### Objetivo
Sistema de gestión de bonificaciones para distribuidores PROMOS:
- **Bonificación por Facturación**: Incentivo basado en valor total facturado/período
- **Bonificación por Pedido**: Incentivo basado en valor total pedido/período

### Modelo de Negocio - FLUJO INTEGRAL

```
PÁGINA PROMOCIONAL (Tercero):
  ?? Suministra: Lista de precios + Descuentos distribuidores (diariamente)

ALDEBARAN:
  ?? Carga diaria: Precios desde página (automático - RF6)
  ?? Obtiene: Valor facturado desde TOTUS (por período - RF7)
  ?? Obtiene: Valor pedido de órdenes + precios (RF8)
  ?? Calcula: Bonos (recomendación) basado en insumos
  ?? Registra: Historial y FOTO del cierre (RF5)
  ?? Consulta: Valores dinámicamente SLA 500ms (RF4)
  ?? Genera: Recomendación de nota crédito para TOTUS
  ?? Reconcilia: Valor calculado vs valor real aplicado (RF11)

TOTUS (Tercero):
  ?? Suministra: Valor facturado (verdad única de facturación)
  ?? Aplica: Las notas crédito en siguiente período
  ?? Retorna: Valor real aplicado (para reconciliación)
  ?? Integración: BD local TOTUS en servidor PROMOS

USUARIO PROMOS:
  ?? Aplica: Bonos recomendados en TOTUS (responsable valor real)
```

### Contexto
Flujo automático: PÁGINA ? ALDEBARAN (calcula) ? TOTUS (aplica) ? ALDEBARAN (reconcilia)

### Usuarios Finales
- Administrador: Configura períodos, tipos, vigencias
- Distribuidor: Beneficiario de bonificaciones
- Usuario PROMOS: Aplica bonos en TOTUS
- TOTUS: Sistema que aplica notas crédito

---

## 1.2 Actores del Sistema

```
ADMINISTRADOR (Aldebaran.Web): Gestiona Períodos, Tipos, Vigencias
DISTRIBUIDOR: Beneficiario de bonificaciones
PÁGINA PROMOCIONAL (Tercero): Suministra precios diarios
SISTEMA TOTUS (Tercero): Suministra facturación, aplica notas crédito
USUARIO PROMOS: Aplica bonos en TOTUS
PROCESO AUTOMÁTICO: Carga precios, cierre período, reconciliación
```

---

## 1.3 Casos de Uso (7 Total)

### CU1: Crear Período
Admin define período (Mensual/Quincenal/Semanal/Custom): Nombre, Día inicio, Duración (días)

### CU2: Crear Tipo de Bono
Admin define Tipo con Afectación (Facturación/Pedido/Entregado) y Período

### CU3: Crear Vigencia
Admin define vigencia: rangos valor, porcentaje, fecha inicio
- Lógica: Vigencia más reciente (fecha ? hoy, Activo) se aplica automáticamente
- Permite proyección: Crear vigencia con fecha futura

### CU4: Cargar Lista de Precios (Automático Diario)
Proceso automático: Descarga ? Procesa ? Carga desde página promocional
- Almacenamiento: PreciosDistribuidor (actual) + PreciosDistribuidorHistorico (4 meses)
- Auditoría y Alertas: Éxito/fallo del proceso

### CU5: Consultar Bono Actual (Dinámico)
Consulta bono acumulado del período actual
- Obtiene: Datos según Tipo Afectación (Facturación/Pedido/Entregado)
- Si Facturación: Descuenta Nota Crédito de período anterior
- Busca: Vigencia más reciente + rango que contiene valor
- Retorna: Valor Acumulado, Porcentaje, Vigencia, Tramo
- SLA: 500ms | No persiste, es DINÁMICO

### CU6: Cierre de Período (Automático)
Al último día del período ejecuta automáticamente
- Calcula: Bono final
- Registra: FOTO en Historial (inmutable)
  - Insumos detallados (Pedido o Facturación)
  - Vigencia aplicada, Porcentaje
- Genera: Información Nota Crédito con ID: BONO_RQM_[Período]_[Distribuidor]

### CU7: Reconciliación de Nota Crédito (Automático)
Al inicio período N+1, obtiene valor REAL de NC que fue aplicada en período N
- Consulta: BD TOTUS
- Busca: NC con identificador BONO_RQM_[Período N]_[Distribuidor]
- Obtiene: Valor Real aplicado
- Actualiza: Historial con Valor Real, Diferencia, Porcentaje Diferencia
- Alerta: Si Diferencia mayor que X%
- CRÍTICO: Evita doble conteo de notas crédito en cálculos siguientes

---

## 1.4 Requisitos Funcionales (11 - TODOS ALTA PRIORIDAD)

| RF | Descripción |
|----|---|
| RF1 | Gestionar Períodos: Crear, editar, activar/desactivar |
| RF2 | Gestionar Tipos de Bono: Crear, editar, activar/desactivar |
| RF3 | Gestionar Vigencias: Crear, editar, activar. Lógica: más reciente se aplica |
| RF4 | Consultar Bono Actual Dinámico: SLA 500ms, No persiste |
| RF5 | Registrar Historial de Bonos: FOTO del cierre, Inmutable |
| RF6 | Cargar Lista Precios: Descarga automática diaria desde página |
| RF7 | Capturar Valor Facturado: De Procedimiento TOTUS (Mock parametrizable) |
| RF8 | Capturar Valor Pedido: De órdenes Aldebaran + precios actuales |
| RF9 | Capturar Valor Entregado: De entregas Aldebaran confirmadas |
| RF10 | Gestionar Nota Crédito Anterior: Para no contar dos veces en cálculos |
| RF11 | Reconciliación Nota Crédito: Obtener REAL de TOTUS y actualizar historial |

---

## 1.5 Requisitos No Funcionales

| Requisito | Especificación |
|-----------|---|
| Disponibilidad | 99% |
| Rendimiento | Consulta bono: 500ms, Cierre período: 5min |
| Seguridad | Solo Admin modifica, Auditoría completa, Historial inmutable |
| Escalabilidad | Miles distribuidores, Histórico 5+ años |
| Integrabilidad | TOTUS + Página Promocional integradas |
| Mantenibilidad | 100% sin código, Parametrizable |

---

## 1.6 Restricciones

- Bono se aplica como NOTA CRÉDITO en siguiente período (no directo)
- Aldebaran: Calcula/recomienda. TOTUS: Aplica. Usuario: Responsable
- Historial inmutable después de cierre
- .NET 7, SQL Server, Blazor Server
- Precios: Carga diaria, se usa lista MÁS RECIENTE
- TOTUS es "verdad única" para valor facturado
- Vigencia: Nueva vigencia, NO edición de anterior (auditoría)

---

## 1.7 Base de Datos TOTUS (Integración Local)

```
Ubicación: BD TOTUS en servidor local PROMOS
Procedimiento: A definir (parametrizable en appSettings)
Entrada: Tipo Doc, Número Doc (Cédula), Fecha Inicio, Fecha Fin
Salida: Valor Facturado SIN Impuestos, NC, Fletes, Descuentos
Status: Procedimiento NO existe aún, usar MOCK parametrizable
```

---

## 1.8 Integraciones Terceros

### TOTUS (Sistema Facturación Externa)

DATOS RECIBIMOS:
- Valor Facturado por distribuidor/período
- Total Notas Crédito (descuentos previos)
- Total Fletes, Total Descuentos

DATOS ENVIAMOS:
- Información de Nota Crédito generada por bono
- Distribuidor, Valor recomendado, Período aplicación
- Identificador único: BONO_RQM_[Período]_[Distribuidor]

RECONCILIACIÓN:
- Obtener Nota Crédito REAL que se aplicó en período anterior
- PENDIENTE DEFINIR: Cómo identificar en TOTUS nuestra NC

### Página Promocional (Precios)

DATOS RECIBIMOS:
- Archivo Excel con lista de precios
- Estructura: Referencia, Precio Unitario, Descuento Distribuidor
- Formato: Header + datos planos (sin jerarquías)
- Frecuencia: Descarga diaria automática

PENDIENTE DEFINIR:
- Protocolo descarga (URL, SFTP, API, etc.)
- Autenticación requerida
- Horario de descarga
- Qué hacer si falla

---

## 1.9 Expectativas de Negocio

IMPACTO:
- Bonificación transparente y real-time
- Control total sin código
- Información clara para aplicar en TOTUS

BENEFICIOS:
- Incentivación objetiva
- Aumento volumen facturado
- Retención distribuidores
- Flexibilidad sin código
- Auditoría completa

MÉTRICAS ÉXITO:
- 100% precisión en cálculos
- Consulta 500ms
- Disponibilidad 99%
- Cero errores doble conteo NC

STAKEHOLDERS:
- Admin PROMOS, Distribuidores, Gerencia, TOTUS, Finanzas, Aldebaran

---

## 1.10 PENDIENTES POR DEFINIR EN FASE DISEÑO

### CRÍTICO 1: Identificación de Nota Crédito en TOTUS (RF11)

NECESIDAD: Identificar en TOTUS cuál fue NC REAL que se aplicó para período anterior

PREGUNTAS:
1. Cómo se distingue NC de Bono vs otras NC
2. Hay procedimiento almacenado para consultar NC
3. Hay identificador único (ID) por NC
4. Búsqueda exacta o con tolerancia

IMPACTO SI NO SE DEFINE: RF11 no funciona, Cálculo N+1 será INCORRECTO

### CRÍTICO 2: Descarga de Lista de Precios (RF6)

NECESIDAD: Proceso automático descarga precios de página promocional diariamente

PREGUNTAS:
1. Cómo está disponible el archivo (URL, SFTP, API, etc.)
2. Horario de disponibilidad
3. Autenticación requerida
4. Qué hacer si falla la descarga

IMPACTO SI NO SE DEFINE: RF6 no funciona, Bono por Pedido no se calcula

---

## ? RESUMEN INSUMOS NECESARIOS

PARAMETRIZACIONES:
- Períodos: Crear, editar, activar
- Tipos de Bono: Crear, editar, activar
- Vigencias: Crear, editar, activar (con fecha)

DATOS OPERACIONALES:
- Valor Facturado (de TOTUS)
- Valor Pedido (de órdenes + precios)
- Valor Entregado (de entregas)
- Nota Crédito Período Anterior (del Historial reconciliado)
- Lista de Precios (cargada diariamente)

REGISTRO HISTÓRICO:
- Historial de Bonos (FOTO del cierre - inmutable)
- Histórico de Precios (últimos 4 meses)
- Reconciliación de Bonos (calculado vs real)
- Auditoría completa

---

## ?? ENTIDADES/TABLAS BASE

```
PERÍODO: ID, Nombre, DiaInicio, DuracionDias, Estado
TIPO_AFECTACION: ID, Nombre (Facturación | Pedido | Entregado)
TIPO_BONO: ID, Nombre, TipoAfectacionID, PeriodoID, Estado
VIGENCIA_BONO: ID, TipoBonoID, ValorMin, ValorMax, Porcentaje, FechaInicio, Estado
PRECIOS_DISTRIBUIDOR: ID, Referencia, Precio, Descuento, FechaCarga, Activo
PRECIOS_DISTRIBUIDOR_HISTORICO: ID, Referencia, Precio, Descuento, FechaCarga (últimos 4 meses)
HISTORIAL_BONO: ID, Distribuidor, TipoBono, Periodo, ValorCalculado, ValorReal, Vigencia, Porcentaje, Insumos, Estado
RECONCILIACION_BONOS: ID, Distribuidor, TipoBono, Periodo, ValorCalculado, ValorReal, Diferencia
CONFIGURACION_SISTEMA: ClaveParametro, Valor, Tipo
```

---

**Estado**: ? COMPLETO
**Próximo**: 2_ARQUITECTURA.md
