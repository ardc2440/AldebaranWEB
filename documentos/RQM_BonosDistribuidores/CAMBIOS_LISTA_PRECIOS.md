# Resumen de Cambios — Integración de Lista de Precios Promocional

**Fecha:** 2026-05-20  
**Versión:** 1.2  
**Archivos modificados:**
- `3_TAREAS_DESARROLLO.md` — Agregado módulo 2.2.4 con TAREA-070 a TAREA-078
- `4_Estimacion_Tareas.md` — Actualizado con 9 tareas adicionales y nuevos totales

---

## Cambios Aplicados

### 1. Archivo `3_TAREAS_DESARROLLO.md`

**Agregada nueva subsección dentro del módulo 2.2:**
- **Subsección 2.2.4 — Lista de Precios Promocional** (9 tareas)
- Parte del **Módulo de Operaciones de Bonificación**, no módulo independiente

**Tareas creadas:**
- **TAREA-070** ??? — Tablas `PromotionalPriceLists` + `PromotionalPriceListItems` (13 columnas A-M)
- **TAREA-071** ?? — Entidades EF + Configurations + Models + Mappings
- **TAREA-072** ?? — Repositorio y servicio con `LoadDayListAsync` y `GetItemPriceAsync`
- **TAREA-073** ?? — Servicio de descarga HTTP con 2 estrategias (directa/autenticada) + parseo Excel
- **TAREA-074** ?? — Worker automático `PriceListFetchWorker` (hora configurable, notificación email)
- **TAREA-075** ?? — Servicio de parseo manual (contingencia)
- **TAREA-076** ??? — Página `PromotionalPriceLists.razor` + carga manual
- **TAREA-077** ??? — Notificación en Dashboard si no hay lista activa
- **TAREA-078** ??? — Agregar "Lista de Precios" al menú Configuración

---

### 2. Archivo `4_Estimacion_Tareas.md`

**Sección 2.2.4 agregada con tabla de tareas:**
- 9 tareas, **todas REQUERIDAS** (??)
- **Subtotal:** 46 horas
- **Prioridad:** Crítico — sin lista de precios no hay cálculo de Bono por Facturación
- **Ubicación:** Dentro del Módulo 2.2 (Operaciones de Bonificación)

**Totales actualizados:**

| Métrica | Antes | Después | ? |
|---------|-------|---------|---|
| **Total Tareas** | 67 | **76** | +9 |
| **Total Horas** | 367 | **413** | +46 |
| **Tareas REQUERIDAS** | 43 (267 h) | **52 (320 h)** | +9 (+46 h) |
| **MVP Mínimo** | 267 h | **320 h** | +46 h |
| **Alcance Completo** | 367 h | **413 h** | +46 h |

**Distribución actualizada:**

| Módulo | Tareas | Horas | % |
|--------|--------|-------|---|
| 2.1.1 – Clientes Distribuidores | 10 | 34 | 8.2% |
| 2.1.3 – Períodos de Bonificación | 9 | 63 | 15.3% |
| 2.1.4 – Tipos de Bono | 6 | 32 | 7.7% |
| 2.1.5 – Vigencias de Bonificación | 9 | 57 | 13.8% |
| 2.1.6 – Vigencias de Descuentos | 9 | 41 | 9.9% |
| 2.2.1 – OC Especiales (Manual) | 9 | 35 | 8.5% |
| 2.2.2 – OC Especiales (Masivo) | 4 | 27 | 6.5% |
| 2.2.3 – Conciliación de NC | 11 | 78 | 18.9% |
| **2.2.4 – Lista de Precios Promocional** | **9** | **46** | **11.1%** |
| **TOTAL** | **76** | **413** | **100%** |

---

## Detalles Técnicos Clave

### Doble Estrategia de Descarga (TAREA-073/074)

**Razón:** Previsión ante cambios en política de seguridad del proveedor.

**Configuración en `appsettings.json`:**
```json
{
  "PriceListFetchOptions": {
    "Url": "https://www.catalogospromocionales.com/distribuidores/referenciasexcel",
    "UseAuthentication": false,
    "LoginUrl": "https://www.catalogospromocionales.com/distribuidores/login",
    "Username": "",
    "Password": "",
    "CronExpression": "0 0 6 * * *",
    "NotificationRecipients": ["bonificacion@promos.com"]
  }
}
```

**Escenarios:**
1. **`UseAuthentication = false`** ? Descarga directa sin login (política actual)
2. **`UseAuthentication = true`** ? Login previo con usuario/contraseña de distribuidor

### Estructura de Datos (TAREA-070/071)

**Tabla `PromotionalPriceLists`:**
- `LIST_DATE` (única por día con STATUS ACTIVE)
- `STATUS` (ACTIVE | HISTORICAL)
- `SOURCE` (AUTOMATIC | MANUAL)
- `LOADED_BY` (NULL si automático, userId si manual)

**Tabla `PromotionalPriceListItems` (13 columnas):**
- Columna A: `ITEM_CODE` (Referencia)
- Columna B: `ITEM_NAME` (NombreProducto)
- Columna C: `FEATURES` (Caracteristicas)
- Columnas D-M: `PRICE1_DESC`, `PRICE1`, `PRICE2_DESC`, `PRICE2`, ... `PRICE5_DESC`, `PRICE5`

**Lógica de `GetItemPriceAsync`:**
Retorna el **primer precio > 0** en orden: `Price1`, `Price2`, `Price3`, `Price4`, `Price5`

### Contingencia Manual (TAREA-075/076)

**Casos de uso:**
1. Fallo del worker automático
2. Ajustes dentro del día a la lista publicada
3. Problemas de conectividad en la hora programada

**Flujo:**
1. Usuario con rol sube archivo `.xlsx`
2. Sistema parsea y muestra preview de N artículos
3. Confirmación: "Reemplazará la lista activa de hoy. La anterior pasará a HISTÓRICO."
4. Registro con `SOURCE = MANUAL` + `LOADED_BY = userId` + `NOTES` obligatoria

### Alertas y Notificaciones (TAREA-077)

**Escenarios de notificación:**

| Escenario | Acción | Destinatario |
|-----------|--------|--------------|
| Worker descarga exitosa | Email: "Lista cargada: N artículos" | Admin bonificación |
| Worker falla | Email: "Error en descarga. Se usa lista del día X" | Admin bonificación |
| Dashboard sin lista hoy | Alert persistente en UI + link a carga manual | Admin con permisos |
| Sin lista en sistema | Alert crítico: "Cálculo bloqueado" | Admin con permisos |

---

## Impacto en el Proyecto

**Sin este módulo:**
- ? No es posible calcular el Bono por Facturación
- ? No hay fuente de precios del día para valorizar transacciones
- ? Proceso de bonificación queda incompleto

**Con este módulo:**
- ? Descarga automática diaria de precios (resiliente con reintentos)
- ? Contingencia manual ante fallos o ajustes intra-día
- ? Alertas proactivas si no hay lista activa
- ? Historial completo de listas por fecha (auditoría)
- ? Doble estrategia de descarga previene bloqueos futuros

---

## Próximos Pasos

1. **Revisión técnica** de la doble estrategia de autenticación (simular escenario con login)
2. **Validación funcional** del flujo de contingencia manual con PROMOS
3. **Configuración inicial** del worker (hora exacta según disponibilidad del proveedor)
4. **Definición de destinatarios** de notificaciones email
5. **Pruebas de integración** con proceso de cálculo de bonificación (cuando esté disponible)

---

**Estado:** ? Documentación completa  
**Próxima etapa:** Inicio de desarrollo (TAREA-001 en adelante)
