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
- Ver listado de vigencias (activas e históricas) por Tipo de Bono
- Copiar vigencia anterior como plantilla

**Conceptos Clave:**
- **Vigencia ACTIVA**: La más reciente en estado ACTIVO para un Tipo de Bono
- **Rangos de Valores (1:N)**: Cada vigencia tiene múltiples rangos (entidad hija). Cada rango define: Valor Mínimo, Valor Máximo, Tipo de Valor (% o Fijo), Valor de Bono
- **Tipo de Valor**: 
  - **Porcentaje (%)**: Bono = Valor Total × Porcentaje / 100
  - **Valor Fijo ($)**: Bono = Valor Fijo (mismo monto independiente del valor total)
- **Activación Automática**: Al activar una nueva vigencia, la anterior del mismo Tipo de Bono pasa a INACTIVA

**Restricciones:**
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
  - **Descuento aplicable** (según Vigencia RF34 - % o Fijo):
    - Si Total Pedido cae en rango con Tipo=%, Descuento = Total × %
    - Si Total Pedido cae en rango con Tipo=Fijo, Descuento = Valor Fijo
  - **Base para Bono**: Total Pedidos - Descuento Aplicable (o restado como porcentaje)
  - **Porcentaje de Bono** (según vigencia RF3): Se aplica a Base después de descuento
  - Valor calculado del bono
  - ?? **NOTA**: Descuento puede ser % o Fijo según vigencia activa
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
Descarga automática diaria de la lista de precios unitarios por referencia desde Página Promocional para calcular bonos correctamente.

**Flujo Técnico:**
1. Job programado (default: 6 AM diariamente)
2. Sistema descarga archivo de precios desde:
   - URL/SFTP/API de Página Promocional (configurable)
   - Credenciales configurables
3. Valida estructura y datos:
   - Estructura: Referencia, Precio Unitario
   - **NOTA**: NO incluye Descuento Distribuidor (no existe en Página Promocional)
   - Validación: Precios > 0
4. Reemplaza tabla PreciosDistribuidor (actual)
5. Copia histórico en PreciosDistribuidorHistorico (con fecha)
6. Ejecuta limpieza automática (borra precios más antiguos que período de retención - default: 120 días)
7. Registra auditoría completa
8. Envía alerta si descarga falla

**Consideración Importante - Descuento Distribuidor:**

```
┌─────────────────────────────────────────────────────────────────┐
│ ACLARACIÓN: Descuento Distribuidor NO viene de Página Promocional
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│ Página Promocional suministra:                                   │
│ ✓ Referencia (código de producto)                               │
│ ✓ Precio Unitario (precio base del día)                         │
│ ✗ Descuento Distribuidor (NO ESTÁ)                              │
│                                                                  │
│ Aldebaran.Web (RF34) suministra:                                │
│ ✓ Descuento General de Distribuidor (parámetro global)          │
│   - Configurado por Admin PROMOS                                 │
│   - Se aplica a TODOS los distribuidores                        │
│   - Usado en cálculo del Bono por Pedido                        │
│                                                                  │
│ FÓRMULA DE CÁLCULO (Bono por Pedido):                            │
│ Bono = ∑(Cantidad × Precio RF10 × (1 - Descuento RF34))         │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Reglas de Negocio:**
- Horario configurable (default: 6 AM)
- Política de reintentos: 3 intentos, 5 minutos espera entre intentos
- Política de retención: 120 días (configurable)
- Si descarga falla ? Usa precios del día anterior (fallback)
- **Descuento General (RF34) se configura separadamente en Aldebaran.Web**

**Manejo de Errores:**
- Si descarga falla después de 3 intentos ? Alerta a Admin + Usa precios anteriores
- No puede dejar el sistema sin precios
- No acepta archivos con estructura incorrecta (falta Referencia o Precio)

**Restricciones:**
- No puede ejecutar limpieza de precios activos (en uso)
- No puede eliminar precios de períodos no cerrados
- No puede sobreescribir precios durante horas de operación críticas
- **No obtiene Descuento Distribuidor** (configurado en RF34)

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

**Período (Template/Definición de Periodicidad):**  
Definición de periodicidad **reutilizable** que establece la duración en días para calcular bonificaciones. Es una plantilla que NO tiene fechas específicas. Ejemplos: "Quincena PROMOS" = 15 días, "Mes PROMOS" = 30 días. Se diferencia de **Instancia de Período** que sí tiene fechas concretas.

**Instancia de Período:**  
Ejecución **específica** de un Período (template) con fechas concretas. Se genera manual o automáticamente especificando una fecha inicio, y la fecha fin se calcula automáticamente como: `Fecha Inicio + Duración del Template`. Ejemplo: "QUI-2026-01" del 01/01/2026 al 15/01/2026 es una instancia de la plantilla "Quincena PROMOS".

**Tipo de Bono:**
Modalidad de bonificación **asociada a un Período** que define la **base del bono** (fuente de datos que lo genera):
- **Facturación**: Usa datos de TOTUS (valor facturado sin impuestos)
- **Pedido**: Usa órdenes de Aldebaran (cantidad pedida × precio)
- **Entregado**: Usa entregas de Aldebaran (cantidad entregada × precio)

Ejemplo: B1 "Bono por Facturación" con base en datos de TOTUS, asociado a P1 "Quincena PROMOS".

**Vigencia (Bono):**  
Configuración **asociada a un Tipo de Bono** que define **rangos de valores de compra con bonificaciones asociadas**. Solo UNA vigencia puede estar ACTIVA por Tipo de Bono. Al activar una nueva, la anterior pasa a INACTIVA automáticamente. Estructura: 1 Vigencia → N Rangos de Valores (relación 1:N). Ejemplo: B1V3 "Vigencia Bono Facturación Marzo" activa desde 2026-03-01.

**Tipo de Valor (Bono o Descuento):**  
Define cómo se calcula un valor en un rango: **Porcentaje (%)** o **Valor Fijo ($)**. 
- **Porcentaje**: Cálculo = Base × Porcentaje / 100 (ej: $5M × 5% = $250K)
- **Valor Fijo**: Cálculo = Valor constante (ej: siempre $500.000 sin importar la base)

Un mismo rango solo puede tener UN tipo (no se mezclan % y Fijo en el mismo rango), pero **diferentes rangos de la misma vigencia SÍ pueden tener tipos distintos** (Rango 1 = %, Rango 2 = Fijo, etc.).

**Rango de Valores:**  
Entidad hija de una Vigencia (relación 1:N) que especifica:
- **Valor Mínimo** y **Valor Máximo**: Límites del rango (ej: $1M a $5M)
- **Tipo de Valor**: Porcentaje (%) o Valor Fijo ($)
- **Valor de Bono**: Número interpretado según el Tipo de Valor

Una vigencia contiene múltiples rangos, cada uno discreto sin traslaparse. Ejemplo: Vigencia con 3 rangos:
- Rango 1: Mín=$1M, Máx=$5M, Tipo=%, Bono=5%
- Rango 2: Mín=$5M, Máx=$10M, Tipo=Fijo, Bono=500.000
- Rango 3: Mín=$10M, Máx=∞, Tipo=%, Bono=7%

**Vigencia de Descuento por Total de Pedido (RF34):**  
Configuración **independiente y paralela a Vigencias de Bono** que define **rangos de totales de pedido con descuentos asociados**. Estructura idéntica a Vigencias de Bono:
- 1 Vigencia Descuento → N Rangos de Total de Pedido (relación 1:N)
- Cada rango: Total Mín, Total Máx, Tipo de Descuento (% o Fijo), Valor de Descuento
- Solo UNA vigencia de descuento ACTIVA en el sistema

Ejemplo: Vigencia "V2 - Desc. Pedido Marzo 2026" con rangos:
- Rango 1: Total $1M-$5M, Tipo=%, Desc=2%
- Rango 2: Total $5M-$10M, Tipo=Fijo, Desc=250.000
- Rango 3: Total $10M+, Tipo=%, Desc=5%

Se aplica en el cálculo: `Base para Bono = Total Pedidos - Descuento Aplicable`

**Vigencia Activa:**  
La vigencia más reciente en estado ACTIVO para un **Tipo de Bono específico** (en RF3) o para **Descuento por Total de Pedido** (en RF34). Solo puede haber UNA vigencia activa por concepto. Al activar una nueva vigencia, la anterior pasa automáticamente a INACTIVA.

**Gamificación:**
Visualización interactiva que muestra al distribuidor **cuánto falta para alcanzar el siguiente nivel/tramo** de bonificación. Incluye:
- Nivel actual (tramo donde está)
- Próximo nivel (tramo siguiente)
- Porcentaje completado hacia próximo nivel (barra visual)
- Monto faltante en valores monetarios

Propósito: Motivar al distribuidor a comprar más volumen para subir de tramo.

**OC Especial (Orden de Compra Especial):**  
Monto ingresado **manualmente por PROMOS** que se suma al **bono por facturación**. No procede de TOTUS sino de decisiones comerciales de PROMOS. Requiere aprobación antes de incluirse en cálculos.

**Reconciliación:**  
Proceso de **comparar** NC calculada (del cierre automático) **vs** NC realmente aplicada en TOTUS (responsabilidad Usuario PROMOS), e ingresar el valor real para usar en próximos cálculos. Ejemplo: Sistema calcula $1M pero usuario aplicó $950K en TOTUS → Se registra el valor real ($950K) para que próximos períodos lo usen como referencia.

**FOTO (Registro Congelado):**  
Registro **inmutable** del bono calculado al **cierre del período**. Contiene el bono final de cada distribuidor para ese período. No se puede recalcular después del cierre. Representa la "fotografía" tomada en ese momento, garantizando auditoría completa e inmutabilidad de datos históricos.

### 2.4.2 Términos Técnicos

**OTP (One Time Password):**  
Código de un solo uso (típicamente 6 dígitos) **enviado por SMS o Email** para autenticar al distribuidor de forma segura. Válido por tiempo limitado (default: 10 minutos). No se puede reutilizar.

**JWT (JSON Web Token):**  
Token de sesión que permite al distribuidor acceder al sitio público **sin re-autenticar** después del OTP. Válido por tiempo configurado (default: 8 horas). Encriptado y enviado en HTTPS.

**SLA (Service Level Agreement):**  
Acuerdo de nivel de servicio que define tiempos **máximos de respuesta** garantizados para operaciones críticas. Ejemplo: "Consulta de bono período actual: 500ms máximo incluyendo consulta a TOTUS".

**Fallback (Mecanismo de Respaldo):**  
Plan alterno cuando una integración **falla o se queda sin respuesta**. Ejemplo: Si TOTUS no responde, sistema usa el **último valor conocido** (precalculado) y muestra alerta al distribuidor indicando que datos pueden estar desactualizados.

**Job / Scheduled Job (Trabajo Programado):**  
Proceso automático que se ejecuta **en horarios específicos** sin intervención humana. Ejemplos en sistema:
- Job diario: Cargar lista de precios (6 AM)
- Job semanal: Enviar recordatorios de progreso (Lunes 8 AM)
- Job periódico: Cierre automático de período (último día a 23:59:59)

**Caché / Almacenamiento Temporal:**  
Almacenamiento **temporal de datos** para mejorar rendimiento. **NOTA IMPORTANTE en este sistema**: Consultas a TOTUS NO se cachean (cada consulta es en tiempo real) para garantizar información actualizada 100%. Sin embargo, otros datos como lista de precios SÍ pueden cachearse por períodos específicos.

**Relación 1:N (Relación Padre-Hijo):**  
Estructura de BD donde 1 registro padre puede tener **múltiples registros hijos** asociados. Ejemplo en este sistema:
- 1 Vigencia → N Rangos de Valores
- 1 Período (Template) → N Instancias de Período
- 1 Tipo de Bono → N Vigencias de Bono

La relación es **cascada**: Si se elimina el padre, se pueden eliminar los hijos (con restricciones de negocio).

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