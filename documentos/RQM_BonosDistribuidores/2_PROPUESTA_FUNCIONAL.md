# 2. PROPUESTA FUNCIONAL - Sistema de Bonificación de Distribuidores

**Identificador**: RQM_BonosDistribuidores_052026  
**Cliente**: PROMOS  
**Estado**: PROPUESTA FUNCIONAL  
**Fecha**: Mayo 2026  
**Versión**: 1.4

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

El sistema calculará **dos tipos de bonos independientes** para cada distribuidor:

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

**Bono Total** = Bonificación por Facturación + Bonificación por Pedido

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

### 2.2.0 Módulo de Configuración de Clientes (Prerequisito - Usuario PROMOS - Aldebaran.Web)

**IMPORTANTE:** Este módulo es un **PREREQUISITO transversal** que debe implementarse primero. Sin esta funcionalidad, los distribuidores no pueden ser identificados en el sistema y por lo tanto no podrán acceder al Sistema de Bonificación.

#### 2.2.0.1 Gestión de Tipo de Cliente - Distribuidor

**Descripción:**  
Marcar o identificar qué Customers en Aldebaran son de tipo "DISTRIBUIDOR" para que puedan acceder al Sistema de Bonificación. Esta es una propiedad de cada Cliente (Customer) que diferencia entre:
- **DISTRIBUIDOR**: Cliente que vende/resuelve (beneficiario de bonificaciones)
- **OTRO TIPO**: Cliente no distribuidor (ej: mayorista, revendedor diferenciado, etc.)

**Funcionalidades:**
- Visualizar listado de clientes (Customers) en Aldebaran.Web
- Marcar/Identificar un Customer como tipo **DISTRIBUIDOR**:
  - Campo checkbox o selector: "Es Distribuidor" (Sí/No)
  - O campo de tipo: "Clasificación Cliente" con opciones (Distribuidor, No Distribuidor, Otro)
- Modificar clasificación de un Customer (cambiar de DISTRIBUIDOR a NO DISTRIBUIDOR o viceversa)
  - ⚠️ Restricción: No puede cambiar clasificación si tiene períodos activos con bonificaciones calculadas
- Ver cuántos clientes están marcados como DISTRIBUIDOR
- Ver cuántos DISTRIBUIDORES tienen sesiones activas / han consultado bonos
- Ver cuántos DISTRIBUIDORES tienen períodos cerrados (histórico)

**Validaciones:**
- Un Customer NO puede ser marcado como DISTRIBUIDOR si:
  - No tiene Email ni Celular configurados (necesarios para OTP)
  - Ya está clasificado de otra forma y tiene datos asociados que lo contradicen
- Un Customer SI puede ser marcado como DISTRIBUIDOR si:
  - Ya tiene datos en Aldebaran (órdenes, entregas)
  - Tiene Email y/o Celular configurados (para OTP)

**Impacto Transversal:**

```
┌─────────────────────────────────────────────────────────────────┐
│ RF32: Marcar/Identificar Customers como Distribuidores         │
│ (CU0 - Prerequisito)                                            │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────────────┼─────────────────────┐
        ↓                     ↓                     ↓
   CU6 (Autenticación)   CU7 (Consulta)        CU9 (Admin)
   Valida que el        Filtra solo            Solo ve
   documento sea        distribuidores         clientes
   tipo DISTRIBUIDOR                           tipo DISTRIBUIDOR
```

**Flujo de Usuario (Admin PROMOS):**
1. Usuario PROMOS ingresa a Aldebaran.Web
2. Accede a "Gestión de Clientes" → "Clasificación"
3. Ve listado de Customers con campo "Es Distribuidor"
4. Marca aquellos que deben tener acceso al Sistema de Bonificación
5. Sistema registra el cambio (auditoría: quién, cuándo, antes/después)
6. Si el Customer es marcado como DISTRIBUIDOR:
   - Pasa a estar disponible para acceso OTP en Sitio Público
   - Puede consultar sus bonos si está autenticado
   - Aparece en filtros de Admin para bonificación

**Reglas de Negocio:**
- Un Customer puede estar marcado o no marcado como DISTRIBUIDOR
- Un Customer NO puede tener dos clasificaciones simultáneas
- Solo DISTRIBUIDORES pueden autenticarse vía OTP (CU6)
- Solo DISTRIBUIDORES pueden consultar bonos (CU7/CU8)
- Admin solo ve DISTRIBUIDORES en reportes de bonificación (CU9)
- Cambio de clasificación es reversible (se puede desmarcar)

**Restricciones:**
- No puede desmarcar un Customer como DISTRIBUIDOR si tiene períodos activos
- No puede desmarcar si tiene sesiones activas de consulta de bonos
- Cambio debe registrarse en auditoría

**Auditoría:**
- Quién marcó/desmarcó: Usuario PROMOS
- Cuándo: Timestamp del cambio
- Antes/Después: Estado anterior vs nuevo

---

#### 2.2.0.2 Gestión de Email de Bonificación para Distribuidores

**Descripción:**  
Configurar un Email especializado para Bonificación (distinto del Email general del Customer) para que reciba exclusivamente:
- Códigos OTP para autenticación en el Sistema de Bonificación (CU6)
- Notificaciones de gamificación (alcanzó nivel, está cerca del siguiente, recordatorios) (CU7)

Este campo **Email de Bonificación** es independiente del Email general del Customer, permitiendo que los distribuidores:
- Usen un email corporativo general para asuntos comerciales
- Usen un email específico (ej: bonificacion@empresa.com) para recibir notificaciones de bonificación

**Funcionalidades:**
- Visualizar Email de Bonificación de cada Cliente DISTRIBUIDOR
- Ingresar/Configurar Email de Bonificación:
  - Campo: Email para Bonificación (distinto del Email general)
  - Validación: Formato de email válido
  - Validación: Email no debe estar vacío si el Cliente es DISTRIBUIDOR
  - Validación: Puede ser igual o diferente al Email general
- Modificar Email de Bonificación:
  - Permitir cambiar en cualquier momento (no vinculado a períodos)
  - Registrar cambio en auditoría (quién, cuándo, antes/después)
- Validar que Email de Bonificación sea accesible:
  - Realizar prueba de envío de OTP para validar email (opcional pero recomendado)
  - Registrar si email es válido/inválido
  - Alertar si no se puede enviar a Email de Bonificación configurado

**Validaciones:**
- Un Customer DISTRIBUIDOR DEBE tener Email de Bonificación configurado
- Email debe ser formato válido (xxx@yyy.zzz)
- Email NO puede estar vacío para DISTRIBUIDORES
- Email puede ser igual al Email general o diferente
- No se puede marcar como DISTRIBUIDOR sin Email de Bonificación configurado
- No se puede dejar sin Email de Bonificación si ya es DISTRIBUIDOR

**Impacto Transversal:**

```
┌─────────────────────────────────────────────────────────────────┐
│ RF33: Gestionar Email de Bonificación para Distribuidores       │
│ (CU0 - Parte de Prerequisito)                                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────────────┼─────────────────────┐
        ↓                     ↓                     ↓
   CU6 (Autenticación)   CU7 (Consulta)      CU29-31 (Notificaciones)
   Envía OTP al Email   Valida Email         Envía notificaciones
   de Bonificación      de Bonificación      al Email de Bonificación
```

**Flujo de Usuario (Admin PROMOS):**
1. Usuario PROMOS ingresa a Aldebaran.Web
2. Accede a "Gestión de Clientes" → "Clasificación"
3. Busca un Cliente que es DISTRIBUIDOR
4. Ve campo "Email de Bonificación"
5. Ingresa o modifica Email de Bonificación (distinto del Email general)
6. Sistema valida formato de email
7. (Opcional) Sistema envía OTP de prueba para confirmar entregabilidad
8. Sistema registra cambio en auditoría
9. Sistema permite guardar solo si Email es válido

**Reglas de Negocio:**
- Email de Bonificación es OBLIGATORIO para DISTRIBUIDORES
- Email de Bonificación es INDEPENDIENTE del Email general del Customer
- Un distribuidor puede tener múltiples emails, pero **solo UNO** de Bonificación
- Cambio de Email de Bonificación es reversible
- Cambios son auditados completamente
- Validación es inmediata (no se permite guardar sin Email válido)
- Si hay error al enviar OTP, el sistema alerta pero permite guardar (email puede estar en correo no deseado)

**Restricciones:**
- No puede dejar Email de Bonificación vacío para DISTRIBUIDORES
- No puede marcar como DISTRIBUIDOR sin configurar Email de Bonificación
- Email debe ser formato válido (validación regex)
- No se permite caracteres especiales inválidos
- Máxima longitud: 254 caracteres (estándar RFC 5321)

**Auditoría:**
- Quién cambió Email: Usuario PROMOS
- Cuándo: Timestamp del cambio
- Antes/Después: Email anterior vs nuevo Email de Bonificación
- Motivo (opcional): Por qué se cambió
- Resultado: Si se logró enviar OTP de prueba o no

**Relación con CU6 (Autenticación OTP):**
```
CU6: Autenticación por OTP
↓
Sistema valida que distribuidor tenga Email de Bonificación
↓
Sistema envía OTP AL EMAIL DE BONIFICACIÓN (no al email general)
↓
Distribuidor recibe OTP en su email especializado
↓
Distribuidor ingresa OTP y accede al sistema
```

**Relación con CU7 (Notificaciones de Gamificación):**
```
CU7: Consulta de Bonificación + Gamificación
↓
Se alcanza nuevo nivel de bono
↓
Sistema dispara notificación
↓
Sistema envía EMAIL AL EMAIL DE BONIFICACIÓN (no al email general)
↓
Distribuidor recibe notificación en su email especializado
```

---

#### 2.2.0.3 Gestión de Vigencia de Descuento por Total de Pedido

**Descripción:**  
Configurar vigencias de descuento (rangos de totales de pedido con descuentos asociados) que se aplican a TODOS los distribuidores de forma uniforme. Este descuento es **independiente de cualquier descuento que pudiera existir en Página Promocional** y se utiliza exclusivamente en el cálculo del **Bono por Pedido**. 

**Cada rango puede definir descuento como PORCENTAJE (%)** o como **VALOR FIJO ($)**, permitiendo flexibilidad en estrategias comerciales. Por ejemplo: descuentos porcentuales para volúmenes bajos/medios y descuentos fijos para volúmenes altos.

**Contexto:**
- La Vigencia de Descuento por Total de Pedido es un parámetro de negocio configurado en **Aldebaran.Web** (NO en Página Promocional)
- Se aplica de forma uniforme a todos los distribuidores
- Afecta el cálculo del Bono por Pedido en la fórmula: `Cantidad × Precio × (1 - Descuento aplicable)`
- Es diferente del Email de Bonificación (RF33) y de la clasificación como DISTRIBUIDOR (RF32)
- **SOLO UNA vigencia de Descuento puede estar ACTIVA** en un momento dado

**Estructura Idéntica a Vigencias de Bono (RF3):**

```
┌────────────────────────────────────────────────────┐
│ Vigencia de Descuento (Template)                   │
│ Nombre: "V2 - Desc. Pedido Marzo 2026"             │
│ Fecha Activación: 01/03/2026                       │
│ Estado: ACTIVO                                     │
├────────────────────────────────────────────────────┤
│ Rangos de Total de Pedido (1:N)                    │
│                                                    │
│ Rango 1:                                           │
│  Total Mín: $1M    Total Máx: $5M                 │
│  Tipo: Porcentaje  Valor: 2%                      │
│                                                    │
│ Rango 2:                                           │
│  Total Mín: $5M    Total Máx: $10M                │
│  Tipo: Valor Fijo  Valor: 250.000                 │
│                                                    │
│ Rango 3:                                           │
│  Total Mín: $10M   Total Máx: ∞                   │
│  Tipo: Porcentaje  Valor: 5%                      │
└────────────────────────────────────────────────────┘
```

**Funcionalidades:**
- Crear nueva vigencia de descuento:
  - Nombre único (ej: "V2 - Descuento Pedido Marzo 2026")
  - **Fecha de Activación** (desde cuándo aplica)
    - Al activarse, desactiva automáticamente la vigencia anterior
  - **Rangos de Total de Pedido** (Relación 1:N - Entidad hija):
    - **UNA vigencia tiene MÚLTIPLES rangos**
    - Cada rango es un registro independiente con:
      - **Total Mínimo** (numérico - límite inferior del rango)
      - **Total Máximo** (numérico - límite superior del rango, o vacío para "en adelante")
      - **Tipo de Descuento**: Porcentaje (%) OR Valor Fijo ($)
      - **Valor de Descuento**: Número (interpretado según Tipo de Descuento)
    - Ejemplo 1 (Mixto - % y Fijo):
      - Rango 1: Total $1M-$5M, Tipo=%, Descuento=2%
      - Rango 2: Total $5M-$10M, Tipo=Fijo, Descuento=250.000
      - Rango 3: Total $10M+, Tipo=%, Descuento=5%
    - Ejemplo 2 (Solo Porcentajes):
      - Rango 1: Total $1M-$5M, Tipo=%, Descuento=1%
      - Rango 2: Total $5M-$10M, Tipo=%, Descuento=3%
      - Rango 3: Total $10M+, Tipo=%, Descuento=5%
    - Ejemplo 3 (Solo Valores Fijos):
      - Rango 1: Total $1M-$5M, Tipo=Fijo, Descuento=100.000
      - Rango 2: Total $5M-$10M, Tipo=Fijo, Descuento=300.000
      - Rango 3: Total $10M+, Tipo=Fijo, Descuento=500.000
- Modificar vigencia (solo antes de que comience a usarse)
- Ver listado de vigencias (activas e históricas)
- Copiar vigencia anterior como plantilla
- Ver cuál es la vigencia ACTIVA actualmente

**Conceptos Clave:**
- **Vigencia ACTIVA**: La más reciente en estado ACTIVO para Descuento por Total de Pedido
- **Rangos de Total de Pedido (1:N)**: Cada vigencia tiene múltiples rangos (entidad hija). Cada rango define: Total Mínimo, Total Máximo, Tipo de Descuento (% o Fijo), Valor de Descuento
- **Tipo de Descuento**: 
  - **Porcentaje (%)**: Descuento = Total Pedido × Porcentaje / 100
  - **Valor Fijo ($)**: Descuento = Valor Fijo (mismo monto independiente del total del pedido)
- **Activación Automática**: Al activar una nueva vigencia, la anterior pasa a INACTIVA automáticamente
- **SOLO UNA vigencia ACTIVA**: Solo existe una vigencia activa en el sistema en un momento dado

**Validaciones:**
- Descuento debe ser numérico (0-100% para porcentajes, 0-∞ para valores fijos)
- No puede haber descuento negativo
- No puede haber porcentaje > 100%
- **Los rangos NO pueden traslaparse**: Límites discretos sin sobreposición (igual que Vigencias de Bono)
- SOLO una vigencia de descuento activa en el sistema
- Cambios afectan a nuevos cálculos desde ese momento
- Tipo de Valor debe ser coherente: Cada rango define si es % o Fijo (no se mezclan en el mismo rango, pero sí entre rangos)

**Impacto Transversal:**

```
┌─────────────────────────────────────────────────────────────────┐
│ RF34: Gestionar Vigencia de Descuento por Total de Pedido       │
│ (CU0 - Parte de Configuración)                                  │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────────────┼─────────────────────┐
        ↓                     ↓                     ↓
   CU7 (Consulta)       CU10 (Cierre)        CU9 (Admin)
   Usa en Bono         Usa en cálculo       Ve vigencia
   por Pedido          FOTO congelada       ACTIVA
```

**Flujo de Usuario (Admin PROMOS):**
1. Usuario PROMOS ingresa a Aldebaran.Web
2. Accede a "Configuración" → "Vigencia de Descuento por Pedido"
3. Ve vigencia ACTIVA actual (ej: "V1 - Desc. Pedido Feb 2026")
4. Puede:
   - Ver rangos de la vigencia activa
   - Crear nueva vigencia para próximo período
   - Activar vigencia futura (desactiva la anterior automáticamente)
   - Copiar vigencia anterior como plantilla
   - Ver historial de vigencias anteriores
5. Sistema registra cambios en auditoría

**Reglas de Negocio:**
- Vigencia de Descuento es ÚNICA y GLOBAL para todos los distribuidores
- Descuento se aplica en el cálculo del Bono por Pedido (se resta del valor total antes de aplicar porcentaje de bono)
- Cambio de vigencia NO recalcula períodos ya cerrados (FOTO inmutable)
- Cambio de vigencia SI afecta a cálculos dinámicos de período activo (CU7)
- Cambio de vigencia SI afecta a nuevos cierres de período (CU10)
- Auditoría completa de cambios: Quién activó, cuándo, vigencia anterior vs nueva

**Restricciones:**
- Descuento porcentaje debe estar entre 0% y 100%
- Descuento fijo debe ser >= 0
- SOLO una vigencia activa en el sistema
- Cambios son reversibles (desactivar y reactivar vigencia anterior si es necesario)
- No puede cambiar tipo de vigencia (siempre es descuento por total de pedido)

**Auditoría:**
- Quién activó vigencia: Usuario PROMOS
- Cuándo: Timestamp del cambio
- Vigencia anterior: Cuál era la activa antes
- Vigencia nueva: Cuál es la nueva activa
- Efecto: Cálculos futuros usan nuevo descuento
- Motivo (opcional): Por qué se cambió

**Relación con RF10 (Cargar Precios):**
```
RF10: Cargar Lista de Precios desde Página Promocional
↓
Obtiene SOLO: Referencia, Precio Unitario
Obtiene NO: Descuento Distribuidor (no está en Página Promocional)
↓
RF34: Vigencia de Descuento por Total de Pedido (configurado en Aldebaran.Web)
↓
Cálculo Bono por Pedido = Cantidad × Precio × (1 - Descuento Aplicable RF34)
```

**Relación con CU7 (Cálculo Dinámico de Bono por Pedido):**
```
CU7: Consulta Bonificación - Período Actual
↓
Total Pedidos = ∑(Cantidad pedida × Precio del día)
↓
Descuento Aplicable = Buscar en Vigencia ACTIVA RF34 según Total Pedidos
                    = Si Total $5M: buscar rango que contiene $5M
                    = Usar Tipo y Valor del rango encontrado
↓
Base para Bono = Total Pedidos - Descuento Aplicable
                (O: Total Pedidos × (1 - Descuento) si es %)
↓
Bono por Pedido = Aplica vigencia de bono según Base
↓
Utiliza Vigencia de Descuento configurada en RF34
```

---

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
Definir los tipos de bonificación disponibles especificando en qué insumo se basa cada uno (Facturación o Pedido) y asociarlos a un Período (template de periodicidad).

**Funcionalidades:**
- Crear nuevo tipo de bono:
  - Nombre único (ej: "Bono por Facturación")
  - Descripción (opcional)
  - **Base del Bono** (fuente de datos que genera el bono):
    - Facturación: Usa datos de TOTUS (valor facturado sin impuestos)
    - Pedido: Usa órdenes de Aldebaran (cantidad pedida × precio)
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
Configurar vigencias (rangos de valores de compra con bonificaciones asociadas) para cada Tipo de Bonificación, con opción de parametrización por artículos/referencias específicos. **Solo UNA vigencia puede estar ACTIVA por Tipo de Bono** en un momento dado. Cada rango puede definir bonificación como **PORCENTAJE (%)** o como **VALOR FIJO ($)**.

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
      - **Tipo de Bono**: Porcentaje (%) OR Valor Fijo ($)
      - **Valor de Bono**: Número (interpretado según Tipo de Bono)
    - Ejemplo 1 (Mixto - % y Fijo):
      - Rango 1: Mín=$1M, Máx=$5M, Tipo=%, Bono=5%
      - Rango 2: Mín=$5M, Máx=$10M, Tipo=Fijo, Bono=500.000
      - Rango 3: Mín=$10M, Máx=?, Tipo=%, Bono=7%
    - Ejemplo 2 (Solo Porcentajes):
      - Rango 1: Mín=$1M, Máx=$5M, Tipo=%, Bono=5%
      - Rango 2: Mín=$5M, Máx=$10M, Tipo=%, Bono=6%
      - Rango 3: Mín=$10M, Máx=?, Tipo=%, Bono=7%
  - **OPCIONAL**: Restricción por artículos/referencias:
    - Sin restricción (DEFAULT - aplica a TODOS los artículos)
    - Artículos específicos (TODAS sus referencias)
    - Artículos + Referencias específicas (combinación personalizada)
  - Moneda del bono (COP, USD, etc.)
  - Monto máximo de bono (tope configurable)
- Modificar vigencia (solo antes de que comience a usarse)
- Ver listado de vigencias (activas e históricas)
- Copiar vigencia anterior como plantilla

**Conceptos Clave:**
- **Vigencia ACTIVA**: La más reciente en estado ACTIVO para un Tipo de Bono
- **Rangos de Valores (1:N)**: Cada vigencia tiene múltiples rangos (entidad hija). Cada rango define: Valor Mínimo, Valor Máximo, Tipo de Valor (% o Fijo), Valor de Bono
- **Tipo de Valor**: 
  - **Porcentaje (%)**: Bono = Valor Total × Porcentaje / 100
  - **Valor Fijo ($)**: Bono = Valor Fijo (mismo monto independiente del valor total)
- **Activación Automática**: Al activar una nueva vigencia, la anterior del mismo Tipo de Bono pasa a INACTIVA automáticamente

**Validaciones:**
- **SOLO UNA vigencia ACTIVA por Tipo de Bono** en un momento dado
- **Los rangos de una vigencia NO pueden traslaparse (sobreposición)**: Cada rango debe tener límites discretos sin sobreposición. Ejemplo correcto:
  - Rango 1: $1M a $5M
  - Rango 2: $5M a $10M (comienza donde termina el anterior)
  - Rango 3: $10M en adelante
  - ❌ INCORRECTO: Rango 1: $1M-$5M, Rango 2: $4M-$10M (tralapan en $4M-$5M)
- No se puede editar una vigencia que ya está en uso
- No se puede crear vigencia con fecha de activación en el pasado
- Si se parametriza por artículos, debe validarse que existan en el sistema
- **Tipo de Valor debe ser coherente**: Cada rango define si es % o Fijo (no se mezclan en el mismo rango, pero sí entre rangos)

**[PLACEHOLDER: Mockup de pantalla "Gestión de Vigencias - Crear/Editar"]**

**[PLACEHOLDER: Mockup de pantalla "Gestión de Vigencias - Tramos de Valor"]**

---

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
   - Es tipo "DISTRIBUIDOR" (no otro tipo de cliente) - **RF32**
   - **Tiene Email de Bonificación configurado** - **RF33**
5. Sistema genera código OTP de 6 dígitos
6. **Sistema envía OTP AL EMAIL DE BONIFICACIÓN (no al email general)** - **RF33**
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
- **OTP se envía SIEMPRE al Email de Bonificación** (no al email general del Customer)

**Restricciones:**
- No se puede autenticar si el distribuidor no tiene Email de Bonificación configurado - **RF33**
- No se puede autenticar si el distribuidor no es tipo DISTRIBUIDOR - **RF32**
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
  - **Descuento aplicable** (según Vigencia RF34 - % o Fijo):
    - Si Total Pedido cae en rango con Tipo=%, Descuento = Total × %
    - Si Total Pedido cae en rango con Tipo=Fijo, Descuento = Valor Fijo
  - **Base para Bono**: Total Pedidos - Descuento Aplicable (o restado como porcentaje)
  - **Porcentaje de Bono** (según vigencia RF3): Se aplica a Base después de descuento
  - Valor calculado del bono
  - ?? **NOTA**: Descuento puede ser % o Fijo según vigencia activa
- **Gamificación**:
  - Nivel actual (tramo de bonificación)
  - Próximo nivel (tramo siguiente)
  - Porcentaje completado hacia próximo nivel
  - Monto faltante para alcanzar próximo nivel

**Notificaciones Automáticas Integradas:**

**1. Notificación: Alcanzó Nuevo Nivel**
- **Cuándo:** Se dispara automáticamente cuando distribuidor sube de tramo en cualquier tipo de bono
- **Canales:** Email al Email de Bonificación (RF33) + SMS según preferencias del distribuidor
- **Contenido:** "¡Felicidades! Alcanzaste un nuevo nivel de bonificación. Tu bono por [Tipo] es ahora [%]% sobre [base]. ¡Sigue adelante!"
- **Frecuencia:** Máximo 1 notificación por distribuidor por tipo de bono en 24 horas

**2. Notificación: Está Cerca del Siguiente Nivel**
- **Cuándo:** Job diario (default: 6 AM) verifica si acumulado ≥ X% del siguiente tramo
- **Canales:** Email al Email de Bonificación (RF33) + SMS según preferencias del distribuidor
- **Contenido:** "¡Casi lo logras! Estás a poco de alcanzar el siguiente nivel de bonificación en [Tipo de Bono]. Necesitas $[Monto Faltante] más. ¡Adelante!"
- **Configuración Admin:** Umbral % (50-95%, default 80%), Frecuencia, Horario, Activo/Inactivo
- **Frecuencia:** Máximo 1 notificación en 24 horas (anti-spam)

**3. Recordatorio Periódico: Progreso de Bonificación**
- **Cuándo:** Job semanal (default: Lunes 8 AM) o configurada por Admin
- **Canales:** Email al Email de Bonificación (RF33) - Resumen detallado + SMS breve según preferencias
- **Contenido Email:** Resumen completo de 3 bonos con acumulado, tramo, % completado, falta para siguiente
- **Configuración Admin:** Frecuencia (Diaria/Semanal/Bisemanal/Mensual), Día/Hora, Activo/Inactivo
- **Preferencias Distribuidor:** Puede desuscribirse, elegir canal preferido, establecer horario

**Reglas de Negocio:**
- Cálculo es dinámico (se ejecuta cada consulta, NO precalculado)
- SLA: 500ms máximo (incluida consulta a TOTUS)
- **Cambios en CUALQUIER fuente se reflejan en próxima consulta**:
  - Nueva factura en TOTUS ? Actualiza bono por Facturación
  - Nueva orden en Aldebaran ? Actualiza bono por Pedido
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
- **Desglose**: Por Facturación, Pedido
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
- ✅ Especificar contrato del SP:
  - Nombre del SP: `dbo.sp_ObtenerFacturacionDistribuidor`
  - Parámetros de entrada: Documento, TipoDoc, FechaInicio, FechaFin, ListaArticulos (opcional)
  - Retorno: ValorFacturado, NC, Fletes, Descuentos
- ✅ Acordar SLA: < 50ms (típico consulta BD local)
- ✅ Definir manejo de errores: Timeout, excepción SQL, BD no disponible
- ✅ Establecer canal de soporte: Para incidentes relacionados con el SP

**Con PROMOS:**
- ✅ Validar que BD TOTUS pueda manejar consultas concurrentes
- ✅ Confirmar que connection pooling está habilitado
- ✅ Verificar que SP está optimizado con índices apropiados

---

### 2.2.4 Módulo de Gestión de Exclusiones (Usuario PROMOS - Aldebaran.Web)

Este módulo permite al personal de PROMOS gestionar pedidos especiales que deben excluirse del cálculo de bonificación.

#### 2.2.4.1 Gestión de Exclusión de Pedidos de Bonificación

**Descripción:**  
Permitir marcar pedidos específicos como "excluidos de bonificación" para casos excepcionales donde ciertos pedidos no deben generar bonificación al distribuidor (ej: pedidos internos, pruebas, devoluciones, promociones especiales sin bono, etc.).

**Contexto de Negocio:**
Existen situaciones especiales donde PROMOS necesita crear pedidos que NO deben generar bonificación para el distribuidor:
- Pedidos de prueba o demo
- Pedidos internos de PROMOS
- Devoluciones o ajustes
- Promociones especiales que ya tienen otro tipo de beneficio
- Correcciones administrativas

Sin esta funcionalidad, todos los pedidos automáticamente generan bonificación, lo cual no es deseable en estos casos excepcionales.

**Funcionalidades:**

**1. Durante la Creación del Pedido (Cualquier usuario con permisos):**
- Campo adicional en formulario de creación de pedido:
  - **Checkbox**: "Pedido Especial (excluir de bonificación)"
  - **Default**: SIN MARCAR (incluido en bonificación)
  - **Ubicación**: Sección "Configuración Adicional" o similar
  - **Tooltip**: "Marque esta opción solo para pedidos que NO deben generar bonificación (ej: pedidos internos, pruebas, devoluciones)"
- Una vez creado el pedido:
  - El flag se **congela** (no editable por usuarios normales)
  - Solo usuarios con rol **ADMIN_EXCLUSION** pueden modificarlo posteriormente
- Se registra en auditoría:
  - ID Pedido
  - Valor inicial del flag (incluido/excluido)
  - Usuario que creó el pedido
  - Fecha/hora de creación

**2. Edición Posterior del Flag (SOLO Rol "ADMIN_EXCLUSION"):**
- **Ubicación**: Módulo de Pedidos → Detalle de Pedido → Sección "Exclusión de Bonificación"
- **Visualización para usuarios normales**: Campo bloqueado (solo lectura) con valor actual
- **Edición para ADMIN_EXCLUSION**:
  - Puede cambiar el checkbox "Excluir de bonificación"
  - Al intentar cambiar, sistema muestra modal de confirmación:

    ```
    ┌──────────────────────────────────────────────────────────┐
    │ ⚠️ ADVERTENCIA: Cambio de Exclusión de Pedido            │
    ├──────────────────────────────────────────────────────────┤
    │                                                          │
    │ Pedido: PED-2026-00123                                   │
    │ Distribuidor: DIST-COLOMBIA-001                          │
    │ Valor Pedido: $5,000,000                                 │
    │                                                          │
    │ Estado Actual: INCLUIDO en bonificación                  │
    │ Cambio a: EXCLUIDO de bonificación                       │
    │                                                          │
    │ ⚠️ Este distribuidor YA consultó su bono en este período │
    │                                                          │
    │ IMPACTO EN BONIFICACIÓN:                                 │
    │ ├─ Bono Anterior: $6,500,000                             │
    │ └─ Bono Nuevo: $6,200,000                                │
    │                                                          │
    │ Diferencia: -$300,000 (disminuye)                        │
    │                                                          │
    │ Motivo del cambio (obligatorio):                         │
    │ [____________________________________________________]   │
    │                                                          │
    │ ✉️ Se notificará al distribuidor del cambio              │
    │                                                          │
    │ [ Cancelar ]  [ Confirmar Exclusión ]                    │
    └──────────────────────────────────────────────────────────┘
    ```

  - **Campo Motivo OBLIGATORIO** (10-500 caracteres):
    - "¿Por qué se está excluyendo/incluyendo este pedido?"
    - Ejemplos: "Pedido interno de PROMOS", "Corrección administrativa", "Devolución completa"
  - Sistema calcula IMPACTO en bono:
    - Recalcula bono SIN/CON el pedido (según cambio)
    - Muestra bono anterior vs bono nuevo
    - Muestra diferencia (positiva o negativa)
  - Sistema verifica si distribuidor ya consultó su bono:
    - Si SÍ → Muestra advertencia destacada
    - Si NO → Solo muestra impacto numérico
  - REQUIERE confirmación explícita del usuario
  - Al confirmar:
    - Sistema RECALCULA bonos del distribuidor para el período activo
    - Sistema ACTUALIZA historial de consultas (marca recálculo)
    - Si bono DISMINUYE → Sistema NOTIFICA al distribuidor vía email
    - Sistema REGISTRA auditoría completa

**3. Visualización en Listado de Pedidos:**
- Columna adicional: "Excluido de Bonificación"
  - ✅ = Excluido (NO genera bono)
  - ❌ = Incluido (SÍ genera bono)
- Filtro: "Mostrar solo pedidos excluidos" / "Mostrar solo incluidos" / "Todos"
- Color diferenciado (opcional): Pedidos excluidos en gris o con badge "SIN BONO"

**4. Auditoría Completa (Tabla: PedidosExclusionLog):**
Cada cambio en el flag de exclusión se registra en una tabla dedicada:

| Campo | Descripción |
|-------|-------------|
| **IDLog** | ID autoincrementable |
| **IDPedido** | Referencia al pedido modificado |
| **IDDistribuidor** | A quién pertenece el pedido |
| **EstadoAnterior** | INCLUIDO o EXCLUIDO (antes del cambio) |
| **EstadoNuevo** | INCLUIDO o EXCLUIDO (después del cambio) |
| **Motivo** | Texto ingresado por ADMIN_EXCLUSION (obligatorio) |
| **UsuarioAdmin** | Quién realizó el cambio |
| **FechaHora** | Timestamp del cambio |
| **IP** | Dirección IP del usuario |
| **UserAgent** | Navegador/dispositivo usado |
| **BonoAnterior** | Bono calculado antes del cambio |
| **BonoNuevo** | Bono calculado después del cambio |
| **Diferencia** | BonoNuevo - BonoAnterior |
| **DistribuidorNotificado** | SÍ/NO (si se envió email) |
| **FechaNotificacion** | Cuándo se notificó |
| **EstadoNotificacion** | ENVIADO / FALLIDO / NO_APLICA |

**Impacto en RF12 (Capturar Valor Pedido):**

La fórmula de cálculo de Bono por Pedido se actualiza para incluir el filtro de exclusión:

```
ANTES (sin exclusión):
A = Σ (Cantidad pedida × Precio del día del pedido)
    Para TODOS los pedidos del período

DESPUÉS (con exclusión):
A = Σ (Cantidad pedida × Precio del día del pedido)
    Para pedidos del período WHERE EstaExcluido = FALSE

RESULTADO:
Pedidos marcados como "Excluidos" NO se suman en el cálculo del bono
```

**Flujo de Usuario (Creación de Pedido - Usuario Normal):**
1. Usuario PROMOS crea nuevo pedido en Aldebaran.Web
2. Completa datos normales del pedido (distribuidor, artículos, cantidades, etc.)
3. En sección "Configuración Adicional":
   - Ve checkbox "Pedido Especial (excluir de bonificación)"
   - Default: SIN MARCAR
   - Si es un pedido especial: MARCA el checkbox
4. Guarda el pedido
5. Sistema registra el estado inicial en auditoría
6. **Flag queda congelado** (usuario normal no puede cambiarlo después)

**Flujo de Usuario (Edición Posterior - ADMIN_EXCLUSION):**
1. Usuario con rol ADMIN_EXCLUSION accede a detalle del pedido
2. Ve sección "Exclusión de Bonificación" con checkbox editable
3. Cambia el estado del checkbox
4. Sistema muestra modal de advertencia con:
   - Datos del pedido y distribuidor
   - Estado anterior vs nuevo
   - Impacto en bonificación (bono antes vs después)
   - Advertencia si distribuidor ya consultó su bono
   - Campo obligatorio "Motivo del cambio"
5. Usuario ingresa motivo y confirma
6. Sistema ejecuta:
   - Recalcula bonos del distribuidor
   - Registra auditoría completa en PedidosExclusionLog
   - Si bono disminuye → Envía email al distribuidor notificando el cambio
7. Sistema muestra confirmación: "Exclusión actualizada. Bonos recalculados. Distribuidor notificado (si aplica)."

**Notificación al Distribuidor (Solo si bono DISMINUYE):**

```
Asunto: Actualización en tu Bonificación - Período [Nombre]

Estimado distribuidor,

Te informamos que se ha realizado un ajuste en tu bonificación 
del período actual por el siguiente motivo:

Motivo: [Motivo ingresado por ADMIN_EXCLUSION]

Impacto en tu bonificación:
├─ Bono Anterior: $6,500,000
└─ Bono Actualizado: $6,200,000

Diferencia: -$300,000

Este ajuste se debe a: [Motivo]

Puedes consultar tu bonificación actualizada ingresando a:
[Link al Sitio Público]

Si tienes dudas, contacta a soporte de PROMOS.

Saludos,
Sistema de Bonificación PROMOS
```

**Reglas de Negocio:**
- Por defecto, TODOS los pedidos generan bonificación (flag = INCLUIDO)
- Flag solo puede cambiar:
  - Durante creación: Cualquier usuario con permisos de crear pedidos
  - Después de creación: SOLO rol ADMIN_EXCLUSION
- Cambio de flag SIEMPRE requiere motivo (obligatorio)
- Cambio de flag SIEMPRE recalcula bonos del período activo
- Distribuidor SOLO es notificado si su bono DISMINUYE (no si aumenta o no cambia)
- Cambios NO afectan períodos cerrados (FOTO inmutable)
- Cambios SÍ afectan cálculos dinámicos de período activo (CU7)
- Cambios SÍ afectan próximo cierre de período (CU10)

**Restricciones:**
- Flag NO editable por usuarios normales después de crear pedido
- SOLO rol ADMIN_EXCLUSION puede cambiar flag en pedidos existentes
- Campo Motivo es OBLIGATORIO (10-500 caracteres)
- No se puede cambiar flag sin ingresar motivo
- No se puede eliminar registro de auditoría (PedidosExclusionLog)
- Cambio requiere confirmación explícita (modal)
- Sistema DEBE recalcular bonos inmediatamente al cambiar flag
- Notificación solo si bono disminuye (evita spam innecesario)

**Validaciones:**
- Pedido debe existir en BD
- Usuario debe tener rol ADMIN_EXCLUSION para editar flag
- Motivo debe tener entre 10 y 500 caracteres
- Distribuidor del pedido debe existir y estar marcado como DISTRIBUIDOR
- Período del pedido debe estar activo (no cerrado)
- Email del distribuidor debe estar configurado (para notificación)

**Casos de Uso Prácticos:**

**Caso 1: Pedido Interno de PROMOS**
```
Situación: PROMOS crea pedido para pruebas internas
Acción: Al crear, marca checkbox "Excluir de bonificación"
Resultado: Pedido NO suma en bonificación
```

**Caso 2: Corrección Administrativa (Después de Crear)**
```
Situación: Se creó pedido normal, pero era una devolución
Acción: ADMIN_EXCLUSION cambia flag a "Excluido"
Motivo: "Devolución completa - No genera bono"
Resultado: Sistema recalcula bono (disminuye), notifica distribuidor
```

**Caso 3: Revertir Exclusión Incorrecta**
```
Situación: Pedido fue marcado como excluido por error
Acción: ADMIN_EXCLUSION cambia flag a "Incluido"
Motivo: "Corrección: Pedido válido, debe generar bono"
Resultado: Sistema recalcula bono (aumenta), NO notifica distribuidor
```

**Impacto Transversal:**

```
┌─────────────────────────────────────────────────────────────────┐
│ RF35: Gestionar Exclusión de Pedidos de Bonificación            │
│ (CU14 - Gestión de Excepciones)                                 │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────────────┼─────────────────────┐
        ↓                     ↓                     ↓
   RF12 (Cálculo Pedido)  CU7 (Consulta)      CU12 (Reclamación)
   Filtra pedidos        Bono recalculado     Auditoría completa
   EstaExcluido=FALSE    si se cambia flag    de cambios
```

**Relación con CU7 (Consulta de Bonificación):**
```
CU7: Distribuidor consulta su bono
↓
Sistema calcula Bono por Pedido
↓
RF12: Suma pedidos WHERE EstaExcluido = FALSE
↓
Sistema excluye automáticamente pedidos marcados
↓
Distribuidor ve bono SIN pedidos excluidos
```

**Relación con CU12 (Resolver Reclamación):**
```
Distribuidor reclama: "Mi bono bajó sin razón"
↓
CU12: Usuario PROMOS investiga
↓
Accede a PedidosExclusionLog
↓
Ve: Pedido PED-123 fue excluido el 15/03/2026
Motivo: "Devolución completa"
Usuario Admin: Juan Pérez
↓
Usuario PROMOS explica al distribuidor con evidencia auditable
```

**[PLACEHOLDER: Mockup de pantalla "Crear Pedido - Checkbox Exclusión"]**

**[PLACEHOLDER: Mockup de pantalla "Editar Pedido - Modal Advertencia Cambio Exclusión"]**

**[PLACEHOLDER: Mockup de pantalla "Listado Pedidos - Columna Excluido de Bonificación"]**

---