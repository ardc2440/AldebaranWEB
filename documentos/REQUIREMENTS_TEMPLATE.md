# ?? TEMPLATE DE DOCUMENTACIÓN PARA NUEVO REQUERIMIENTO

> Este archivo proporciona un template para crear la documentación de un nuevo requerimiento rápidamente.

---

## ?? Pasos para Crear Nuevo Requerimiento

### 1. Crear Rama
```bash
git checkout -b RQM_<NombreCorto>_$(date +%y%m%d)
# Ejemplo: git checkout -b RQM_GestionPedidos_250515
```

### 2. Crear Carpeta de Requerimiento
```bash
mkdir RQM_<NombreCorto>
cd RQM_<NombreCorto>
```

### 3. Copiar Template de Documentos
```bash
# Desde RQM_BonosDistribuidores, copiar:
cp ../RQM_BonosDistribuidores/1_REQUERIMIENTOS.md .
cp ../RQM_BonosDistribuidores/2_ARQUITECTURA.md .
cp ../RQM_BonosDistribuidores/3_ENTIDADES_Y_MODELOS.md .
cp ../RQM_BonosDistribuidores/4_SERVICIOS_Y_APIS.md .
cp ../RQM_BonosDistribuidores/5_INTEGRACIONES_TERCEROS.md .
cp ../RQM_BonosDistribuidores/6_SEGUNDA_BASE_DATOS.md .
cp ../RQM_BonosDistribuidores/7_ESTIMACION_ESFUERZO.md .
cp ../RQM_BonosDistribuidores/8_PLAN_IMPLEMENTACION.md .
cp ../RQM_BonosDistribuidores/9_CAMBIOS_CODIGO.md .
cp ../RQM_BonosDistribuidores/10_MIGRACIONES_BD.md .
cp ../RQM_BonosDistribuidores/11_CONFIGURACION_DESPLIEGUE.md .
cp ../RQM_BonosDistribuidores/README.md .
cp ../RQM_BonosDistribuidores/STATUS.md .
```

### 4. Actualizar README.md
Cambiar:
```markdown
# ?? DOCUMENTACIÓN - Requerimiento: Bonos Distribuidores

**Identificador**: `RQM_BonosDistribuidores_052026`
```

Por:
```markdown
# ?? DOCUMENTACIÓN - Requerimiento: <Nombre Completo>

**Identificador**: `RQM_<Nombre>_YYMMDD`
```

### 5. Actualizar STATUS.md
Cambiar datos:
- Rama de `RQM_BonosDistribuidores_052026` a `RQM_<Nombre>_YYMMDD`
- Descripción del requerimiento
- Estadísticas si aplica

### 6. Actualizar REQUIREMENTS_INDEX.md (Raíz)
Agregar nuevo requerimiento a la lista:
```markdown
### 2?? RQM_<Nombre> (RQM_<Nombre>_YYMMDD)

**Estado**: ?? En Definición  
**Rama**: `RQM_<Nombre>_YYMMDD`  
**Carpeta**: `/RQM_<Nombre>/`  
**Descripción**: [Descripción corta]

**Documentos**:
- ?? [Requerimientos](./RQM_<Nombre>/1_REQUERIMIENTOS.md)
- ...
```

### 7. Commit Inicial
```bash
git add RQM_<Nombre>/
git commit -m "chore: add documentation template for RQM_<Nombre>"
git push origin RQM_<Nombre>_YYMMDD
```

### 8. Comenzar Documentación
Completar documentos en orden:
1. ? 1_REQUERIMIENTOS.md
2. ? 2_ARQUITECTURA.md
3. ? 3_ENTIDADES_Y_MODELOS.md
4. ? 4_SERVICIOS_Y_APIS.md
5. ? 5_INTEGRACIONES_TERCEROS.md
6. ? 6_SEGUNDA_BASE_DATOS.md
7. ? 7_ESTIMACION_ESFUERZO.md
8. ? 8_PLAN_IMPLEMENTACION.md
9. ? 9_CAMBIOS_CODIGO.md
10. ? 10_MIGRACIONES_BD.md
11. ? 11_CONFIGURACION_DESPLIEGUE.md

---

## ?? Ejemplo Completo

### Crear RQM de "Gestión de Pedidos"

```bash
# 1. Crear rama
git checkout -b RQM_GestionPedidos_250515

# 2. Crear carpeta
mkdir RQM_GestionPedidos

# 3. Copiar template
cp RQM_BonosDistribuidores/* RQM_GestionPedidos/

# 4. Actualizar README.md
# [Editar: cambiar "Bonos" por "Gestión de Pedidos"]

# 5. Actualizar STATUS.md
# [Editar: cambiar identificador, rama, descripción]

# 6. Commit
cd RQM_GestionPedidos
git add -A
git commit -m "chore: add documentation template for RQM_GestionPedidos"
git push origin RQM_GestionPedidos_250515

# 7. Comenzar a llenar 1_REQUERIMIENTOS.md
```

---

## ?? Convenciones de Nombres

### Rama
```
RQM_<NombreCorto>_<YYMMDD>

Ejemplos:
- RQM_BonosDistribuidores_052026
- RQM_GestionPedidos_250515
- RQM_ReportesAvanzados_250601
```

### Carpeta
```
RQM_<NombreCorto>/

Ejemplos:
- RQM_BonosDistribuidores/
- RQM_GestionPedidos/
- RQM_ReportesAvanzados/
```

### Nombre Legible
```
"Requerimiento: <Nombre Completo>"

Ejemplos:
- "Requerimiento: Bonos Distribuidores"
- "Requerimiento: Gestión de Pedidos"
- "Requerimiento: Reportes Avanzados"
```

---

## ?? Checklist Antes de Iniciar Código

Para cada requerimiento, antes de escribir una línea de código:

```
Documentación:
  [ ] 1_REQUERIMIENTOS.md - 100% completo
  [ ] 2_ARQUITECTURA.md - 100% aprobado
  [ ] 3_ENTIDADES_Y_MODELOS.md - 100% detallado
  [ ] 4_SERVICIOS_Y_APIS.md - 100% definido
  [ ] 5_INTEGRACIONES_TERCEROS.md - 100% claro (si aplica)
  [ ] 6_SEGUNDA_BASE_DATOS.md - 100% listo (si aplica)
  [ ] 7_ESTIMACION_ESFUERZO.md - 100% calculado
  [ ] 8_PLAN_IMPLEMENTACION.md - 100% confirmado
  [ ] 9_CAMBIOS_CODIGO.md - 100% detallado
  [ ] 10_MIGRACIONES_BD.md - 100% especificado
  [ ] 11_CONFIGURACION_DESPLIEGUE.md - 100% listo

Aprobaciones:
  [ ] Requerimientos aprobados por Product Owner
  [ ] Arquitectura aprobada por Arquitecto
  [ ] Estimación aprobada por Manager
  [ ] Plan confirmado con equipo
  [ ] Especificación técnica validada

Ambiente:
  [ ] Ambiente DEV configurado
  [ ] Segunda BD (si aplica) creada
  [ ] APIs de terceros (si aplica) accesibles
  [ ] Equipo capacitado
  [ ] Herramientas instaladas

Entonces SÍ: ? INICIAR CÓDIGO
```

---

## ?? Estructura Final por Requerimiento

```
src/
??? RQM_BonosDistribuidores/
?   ??? README.md
?   ??? 1_REQUERIMIENTOS.md
?   ??? 2_ARQUITECTURA.md
?   ??? 3_ENTIDADES_Y_MODELOS.md
?   ??? 4_SERVICIOS_Y_APIS.md
?   ??? 5_INTEGRACIONES_TERCEROS.md
?   ??? 6_SEGUNDA_BASE_DATOS.md
?   ??? 7_ESTIMACION_ESFUERZO.md
?   ??? 8_PLAN_IMPLEMENTACION.md
?   ??? 9_CAMBIOS_CODIGO.md
?   ??? 10_MIGRACIONES_BD.md
?   ??? 11_CONFIGURACION_DESPLIEGUE.md
?   ??? STATUS.md
?
??? RQM_GestionPedidos/        ? Próximo requerimiento
?   ??? README.md
?   ??? 1_REQUERIMIENTOS.md
?   ??? [... resto de docs]
?
??? RQM_ReportesAvanzados/     ? Futuro requerimiento
?   ??? [... estructura igual]
?
??? [Proyectos C# existentes...]
?
??? REQUIREMENTS_INDEX.md        ? Índice maestro
```

---

## ?? Beneficios de Esta Estructura

? **Organización**: Cada requerimiento separado
? **Historial**: Se mantiene todo versionado
? **Paralelo**: Múltiples RQM en simultáneo
? **Reutilizable**: Template para nuevos RQM
? **Escalable**: Crece sin conflictos
? **Trazable**: Git log limpio por RQM
? **Profesional**: Documentación centralizada
? **Eficiente**: No hay redundancias

---

**Última actualización**: 2024  
**Tipo**: Template para Nuevos Requerimientos  
**Estado**: Listo para Usar
