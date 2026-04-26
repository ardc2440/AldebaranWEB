# PLAN DE SIMPLIFICACIÓN - Convertir CU a REQUERIMIENTO Puro

**Estado:** En Progreso  
**Objetivo:** Convertir todos los CU de DISEÑO (CÓMO) a REQUERIMIENTO (QUÉ)  
**Referencia:** CU11 es el modelo a seguir

---

## ? COMPLETADOS

### CU1: Crear Período
**Status:** ? REFACTORIZADO
- Eliminados: detalles técnicos
- Agregados: Objetivo, Problemas, Información de Entrada, Acciones, Restricciones

### CU2: Crear Tipo de Bono
**Status:** ? REFACTORIZADO
- Eliminados: detalles técnicos
- Agregados: Objetivo, Problemas, Información de Entrada, Acciones, Restricciones

### CU3: Crear Vigencia
**Status:** ? REFACTORIZADO
- Eliminados: Ejemplos prácticos (4 CASOS DE USO - 140 líneas)
- Eliminados: Detalle de los "4 Niveles de Parametrización"
- Mantenido: Explicación conceptual concisa
- Agregados: Objetivo, Problemas, Información de Entrada, Acciones, Restricciones

---

## ?? EN PROGRESO

### CU4: Obtener Facturación de TOTUS
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ? Eliminar: "Flujo" (pasos 1-5 detalladísimo)
- ? Eliminar: "Integración con RF11" (es nota técnica)
- ? Agregar: Objetivo, Problemas, Información de Entrada, Acciones, Restricciones
- **Líneas a reemplazar:** 378-418 (paso a paso del flujo)

### CU5: Cargar Lista de Precios
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ? Eliminar: "Flujo" (pasos 1-6 detalladísimo)
- ? Eliminar: Sección "Reintentos" (es detalle técnico)
- ? Agregar: Información de Entrada clara
- ? Agregar: Acciones que puede realizar
- **Líneas a reemplazar:** 422-461 (todos los pasos del flujo)

### CU6: Autenticar Distribuidor
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ? Eliminar: "Flujo" de 12 pasos (descripción CÓMO)
- ? Eliminar: Sección "Seguridad" (duplicada en restricciones)
- ? Agregar: Objetivo claro
- ? Agregar: Acciones que puede realizar
- **Líneas a reemplazar:** 378-401 (todos los pasos del flujo)

### CU7: Consultar Bonificación
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ? Eliminar: "Flujo de Cálculo" (7 pasos pseudocódigo)
- ? Eliminar: Ejemplo de pantalla ASCII (DISEÑO)
- ? Eliminar: "Validaciones" sección (ir a restricciones)
- ? Agregar: Objetivo claro
- ? Agregar: Información que retorna
- ? Agregar: Acciones que puede realizar
- **Líneas a reemplazar:** 409-594 (~185 líneas de diseño)

### CU8: Consultar Bono Actual
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ?? MUY CORTO (solo 3 líneas)
- ? Expandir con: Objetivo, Problemas, Información, Acciones, Restricciones
- **Líneas a reemplazar:** 595-598

### CU9: Cierre de Período
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ? Eliminar: "Proceso Detallado" (pseudocódigo de 50+ líneas)
- ? Eliminar: "DIFERENCIAS CLAVE" (es nota de diseño)
- ? Agregar: Objetivo claro
- ? Agregar: Acciones que realiza automáticamente
- **Líneas a reemplazar:** 599-700 (aprox)

### CU10: Conciliación de Nota Crédito
**Status:** ? PENDIENTE
**Cambios Necesarios:**
- ? Eliminar: "Modalidades permitidas" (ejemplo práctico CÓMO)
- ? Eliminar: Ejemplo "Modalidad Unitario" (DISEÑO)
- ? Eliminar: Ejemplo "Modalidad Masivo" (DISEÑO)
- ? Eliminar: "Resultado Final" (es pseudocódigo)
- ? Eliminar: "Transición de Estados" (es arquitectura)
- ? Agregar: Objetivo QUÉ
- ? Agregar: DOS modalidades como INFORMACIÓN (no ejemplos)
- **Líneas a reemplazar:** 700-850 (aprox, 150+ líneas de diseño)

---

## ?? ESTRUCTURA ESTÁNDAR A APLICAR

```markdown
### CU[N]: [Nombre]

**Ubicación:** [Plataforma donde se ejecuta]
**Acceso:** [Quién puede hacerlo / Requisitos de acceso]
**Actor:** [Quién lo ejecuta]
**Objetivo:** [QUÉ necesita lograr - sin decir CÓMO]

**Problemas que resuelve:**
- Problema 1
- Problema 2
- Problema 3

**Información que necesita [ingresar/acceder]:**
- Campo 1
- Campo 2
- Campo 3 (OPCIONAL)

**Información que [retorna/genera]:**
- Dato 1
- Dato 2

**Acciones que puede realizar:**
- Acción 1
- Acción 2
- Acción 3

**Restricciones:**
- Restricción 1 (QUÉ NO puede hacer)
- Restricción 2
- Restricción 3
```

---

## ?? TIMELINE

**Total cambios necesarios:** ~400-500 líneas a reemplazar

| CU | Status | Líneas | Prioridad |
|----|--------|--------|-----------|
| CU1 | ? | 15 | ? |
| CU2 | ? | 12 | ? |
| CU3 | ? | 30 | ? |
| CU4 | ? | 40 | ?? |
| CU5 | ? | 40 | ?? |
| CU6 | ? | 30 | ?? |
| CU7 | ? | 185 | ?? |
| CU8 | ? | 20 | ?? |
| CU9 | ? | 100 | ?? |
| CU10 | ? | 150 | ?? |
| CU11 | ? | 60 | ? |

---

## ? BENEFICIO FINAL

**ANTES:** Documento con mezcla de:
- ? Requerimientos
- ? Diseño paso a paso
- ? Pseudocódigos
- ? Ejemplos de pantalla
- ? Arquitectura detallada

**DESPUÉS:** Documento con:
- ? Requerimientos PUROS (QUÉ, no CÓMO)
- ? Estructura consistente en todos los CU
- ? Listo para Propuesta Técnica (diseño detallado)
- ? Clear separation: Requerimiento vs Diseño vs Arquitectura

---

**Próximo paso:** Continuar con refactorización de CU4-CU10
