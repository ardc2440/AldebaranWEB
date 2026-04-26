# 1. REQUERIMIENTOS FUNCIONALES - Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS | **Estado**: REQUERIMIENTOS DEFINIDOS | **Fecha**: 2024

---

## 1.1 Descripción General

### Objetivo
Sistema de gestión de bonificaciones para distribuidores PROMOS con dos modalidades:
- **Bonificación por Facturación**: Incentivo basado en valor total facturado en período
- **Bonificación por Pedido**: Incentivo basado en valor total pedido en período

### Modelo de Negocio

```
PÁGINA PROMOCIONAL (Tercero):
  Suministra: Lista de precios + Descuentos distribuidores (diariamente)

ALDEBARAN:
  Carga diaria: Precios desde página (automático)
  Obtiene: Valor facturado desde TOTUS (por período)
  Obtiene: Valor pedido de órdenes + precios
  Calcula: Bonos (recomendación) basado en insumos
  Registra: Historial y FOTO del cierre
  Consulta: Valores dinámicamente (SLA 500ms)
  Genera: Recomendación de nota crédito para TOTUS
  Reconcilia: Valor calculado vs valor real aplicado

TOTUS (Tercero):
  Suministra: Valor facturado (verdad única de facturación)
  Aplica: Las notas crédito en siguiente período
  Retorna: Valor real aplicado (para reconciliación)
  Integración: BD local TOTUS en servidor PROMOS

USUARIO PROMOS:
  Aplica: Bonos recomendados en TOTUS (responsable valor real)
```

---

## 1.2 Actores del Sistema

- ADMINISTRADOR (Aldebaran.Web): Gestiona Períodos, Tipos, Vigencias
- DISTRIBUIDOR: Beneficiario de bonificaciones
- PÁGINA PROMOCIONAL (Tercero): Suministra precios diarios
- TOTUS (Tercero): Suministra facturación, aplica notas crédito
- USUARIO PROMOS: Aplica bonos en TOTUS
- PROCESO AUTOMÁTICO: Carga precios, cierre período, reconciliación

---

## 1.3 Casos de Uso (7 Total)

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

### CU5: Consultar Bono Actual (Dinámico)
Consulta bono acumulado del período actual (SLA: 500ms)

### CU6: Cierre de Período (Automático)
Al último día ejecuta: Calcula bono final, Registra FOTO, Genera Nota Crédito

### CU7: Reconciliación de Nota Crédito (Automático)
Al inicio período N+1: Obtiene NC REAL de TOTUS, Actualiza historial

---

## 1.4 Requisitos Funcionales (11 - TODOS ALTA PRIORIDAD)

| RF | Descripción |
|----|---|
| RF1 | Gestionar Períodos |
| RF2 | Gestionar Tipos de Bono |
| RF3 | Gestionar Vigencias |
| RF4 | Consultar Bono Actual Dinámico |
| RF5 | Registrar Historial de Bonos |
| RF6 | Cargar Lista Precios Distribuidores |
| RF7 | Capturar Valor Facturado (TOTUS) |
| RF8 | Capturar Valor Pedido (Aldebaran + Precios) |
| RF9 | Capturar Valor Entregado (Aldebaran) |
| RF10 | Gestionar Nota Crédito Período Anterior |
| RF11 | Reconciliación Nota Crédito (TOTUS) |

---

## 1.5 Requisitos No Funcionales

| Requisito | Especificación |
|-----------|---|
| Disponibilidad | 99 porciento |
| Rendimiento | Consulta: 500ms, Cierre: 5min |
| Seguridad | Solo Admin, Auditoría, Historial inmutable |
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

**CÁLCULO DEL BONO POR FACTURACIÓN - PASO A PASO**:
```
Paso 1: Obtiene de TOTUS
  ValorBruto = ValorTotalFacturadoSinImpuestos
  Ej: $100,000,000

Paso 2: Descuenta Nota Crédito del período ANTERIOR
  NC_PeríodoAnterior = Obtiene de Historial (reconciliada)
  Ej: $1,500,000
  ValorNeto = ValorBruto - NC_PeríodoAnterior
  Ej: $100,000,000 - $1,500,000 = $98,500,000

Paso 3: Busca Vigencia más reciente (fecha ? hoy, estado Activo)
  Vigencia del mes en curso

Paso 4: Busca en qué TRAMO cae ValorNeto
  Tramos configurados:
    Tramo 1: $10M - $20M = 58 porciento
    Tramo 2: $20M - $30M = 59 porciento
    Tramo 3: $30M - $100M = 60 porciento
    Tramo 4: >$100M = 61 porciento

  ValorNeto = $98,500,000 ? Cae en Tramo 3 (58-100M) ? 60 porciento

Paso 5: Aplica porcentaje
  Bono = ValorNeto * Porcentaje
  Bono = $98,500,000 * 0.60 = $59,100,000

Paso 6: Genera Nota Crédito para siguiente período
  Distribuidor: DIST-001
  Valor: $59,100,000
  Para aplicar en: Febrero
```

**ESTRATEGIA MOCK**:
```
appSettings:
  "ProcedimientoAlmacenadoTOTUS": "sp_ObtenerFacturacion"
  "UsarMockTOTUS": true (durante desarrollo)
  "ValorMockFacturado": 100000000
  "TotalMockNC": 1500000

Si UsarMockTOTUS = true:
  Retorna valores de prueba configurables
  Permite testing sin dependencia de TOTUS real
```

### 1.7.2 VALOR PEDIDO (De Órdenes Aldebaran + Lista de Precios Página)

**PROBLEMA**: Aldebaran NO tiene precios de artículos. Página Promocional sí.

**SOLUCIÓN**: Cargar archivo Excel diariamente desde página (mejor que servicio 1-a-1)

**FUENTES DE DATOS**:
```
Fuente 1: ÓRDENES EN ALDEBARAN (por período)
  - Referencia del artículo
  - Cantidad pedida
  - Fecha de la orden

Fuente 2: LISTA DE PRECIOS (cargada diariamente de Página)
  - Referencia
  - PrecioUnitario (precio base)
  - DescuentoDistribuidor (porcentaje)
```

**CÁLCULO DEL VALOR PEDIDO - PASO A PASO**:
```
Para cada orden del período:

  1. Busca en lista de precios por Referencia
     Ej: REF-001

  2. Obtiene Precio y Descuento
     PrecioUnitario = $20
     DescuentoDistribuidor = 10 porciento

  3. Calcula precio con descuento
     PrecioConDescuento = PrecioUnitario * (1 - DescuentoDistribuidor)
     PrecioConDescuento = $20 * (1 - 0.10) = $18

  4. Calcula valor de la orden
     Cantidad = 100 unidades
     ValorOrden = Cantidad * PrecioConDescuento
     ValorOrden = 100 * $18 = $1,800

  5. ACUMULA para el período completo
     ValorTotalPedido = SUM(todas las órdenes del período)
     Ej: $1,800,000
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

### 1.7.3 VALOR ENTREGADO (De Recibos Aldebaran)

**Fuente**: Entregas/Recibos confirmados en Aldebaran

**Cálculo**:
```
ValorTotalEntregado = SUM(Valor de todas entregas confirmadas en período)

Se usa para Tipo de Bono con Afectación = "Entregado"
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

## 1.8 Integraciones Terceros

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

## ? RESUMEN

INSUMOS NECESARIOS:
1. Valor Facturado (de TOTUS)
2. Valor Pedido (de órdenes + precios)
3. Valor Entregado (de entregas)
4. Nota Crédito Anterior (del historial)
5. Lista de Precios (diaria de página)

CÁLCULO DEL BONO:
1. Obtiene insumo según Tipo Afectación
2. Descuenta NC período anterior (si aplica)
3. Busca vigencia más reciente
4. Busca tramo que contiene el valor
5. Aplica porcentaje del tramo
6. Genera Nota Crédito

Estado: COMPLETO - Listo para 2_ARQUITECTURA.md
