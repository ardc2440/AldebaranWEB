# ?? ÍNDICE DE REQUERIMIENTOS - Aldebaran.Web

> **Estructura de Documentación por Requerimientos (RQM)**

Este archivo proporciona un índice central de todos los requerimientos documentados en el proyecto Aldebaran.Web.

---

## ?? Requerimientos Documentados

### 1?? RQM_BonosDistribuidores (RQM_BonosDistribuidores_052026)

**Estado**: ?? En Definición  
**Rama**: `RQM_BonosDistribuidores_052026`  
**Carpeta**: `/RQM_BonosDistribuidores/`  
**Descripción**: Gestión de bonos para distribuidores

**Documentos**:
- ?? [Requerimientos](./RQM_BonosDistribuidores/1_REQUERIMIENTOS.md)
- ??? [Arquitectura](./RQM_BonosDistribuidores/2_ARQUITECTURA.md)
- ?? [Entidades y Modelos](./RQM_BonosDistribuidores/3_ENTIDADES_Y_MODELOS.md)
- ?? [Servicios y APIs](./RQM_BonosDistribuidores/4_SERVICIOS_Y_APIS.md)
- ?? [Integraciones Terceros](./RQM_BonosDistribuidores/5_INTEGRACIONES_TERCEROS.md)
- ??? [Segunda Base de Datos](./RQM_BonosDistribuidores/6_SEGUNDA_BASE_DATOS.md)
- ?? [Estimación de Esfuerzo](./RQM_BonosDistribuidores/7_ESTIMACION_ESFUERZO.md)
- ?? [Plan de Implementación](./RQM_BonosDistribuidores/8_PLAN_IMPLEMENTACION.md)
- ?? [Cambios de Código](./RQM_BonosDistribuidores/9_CAMBIOS_CODIGO.md)
- ?? [Migraciones BD](./RQM_BonosDistribuidores/10_MIGRACIONES_BD.md)
- ?? [Configuración y Despliegue](./RQM_BonosDistribuidores/11_CONFIGURACION_DESPLIEGUE.md)
- ?? [Estado del Proyecto](./RQM_BonosDistribuidores/STATUS.md)

**Última Actualización**: [Pendiente]  
**Responsable**: [Usuario]

---

## ?? Estructura Esperada

Para cada nuevo requerimiento, se debe:

1. ? Crear rama: `RQM_<NombreRequerimiento>_<YYMMDD>`
2. ? Crear carpeta: `/RQM_<NombreRequerimiento>/`
3. ? Crear 11 documentos base (template disponible)
4. ? Completar documentos en orden
5. ? Validar antes de iniciar código
6. ? Documentar cambios en archivo correspondiente

---

## ?? Convención de Nombres

### Ramas
```
RQM_BonosDistribuidores_052026        ? Formato correcto
RQM_<Nombre>_<YYMMDD>                 ? Template
```

### Carpetas
```
RQM_BonosDistribuidores/              ? Formato correcto
RQM_<Nombre>/                         ? Template
```

### Documentos Internos
```
1_REQUERIMIENTOS.md
2_ARQUITECTURA.md
3_ENTIDADES_Y_MODELOS.md
4_SERVICIOS_Y_APIS.md
5_INTEGRACIONES_TERCEROS.md
6_SEGUNDA_BASE_DATOS.md
7_ESTIMACION_ESFUERZO.md
8_PLAN_IMPLEMENTACION.md
9_CAMBIOS_CODIGO.md
10_MIGRACIONES_BD.md
11_CONFIGURACION_DESPLIEGUE.md
README.md
STATUS.md
```

---

## ?? Estados Posibles

```
?? En Definición     ? Recolectando requerimientos
?? En Diseño         ? Definiendo arquitectura
?? Diseño Aprobado   ? Listo para desarrollo
?? En Desarrollo     ? Código en construcción
? Completado        ? Feature en producción
?? Archivado         ? Feature descontinuada
```

---

## ?? Checklist para Nuevo Requerimiento

```bash
# 1. Crear rama
git checkout -b RQM_<Nombre>_$(date +%y%m%d)

# 2. Crear carpeta
mkdir RQM_<Nombre>

# 3. Copiar template de documentos
cp RQM_BonosDistribuidores/* RQM_<Nombre>/

# 4. Actualizar README.md con referencia a nuevo RQM
# (Editarlo en este archivo)

# 5. Empezar a completar documentos
# Comenzar con 1_REQUERIMIENTOS.md

# 6. Commit inicial
git add RQM_<Nombre>/
git commit -m "chore: add documentation template for RQM_<Nombre>"

# 7. Push
git push origin RQM_<Nombre>_$(date +%y%m%d)
```

---

## ?? Próximos Requerimientos (Por Agregar)

```
[ ] RQM_<Nombre2>_YYMMDD
[ ] RQM_<Nombre3>_YYMMDD
[ ] RQM_<Nombre4>_YYMMDD
```

---

## ?? Soporte

**¿Cómo crear documentación para un nuevo requerimiento?**

1. Revisa estructura de `RQM_BonosDistribuidores/`
2. Copia template completo
3. Sigue convención de nombres
4. Completa documentos en orden
5. Valida antes de iniciar código

**¿Dónde están los documentos?**

```
Raíz del proyecto:
??? RQM_BonosDistribuidores/        ? Requerimiento 1
??? RQM_<Nombre2>/                  ? Requerimiento 2 (próximo)
??? RQM_<Nombre3>/                  ? Requerimiento 3 (próximo)
??? REQUIREMENTS_INDEX.md            ? Este archivo
```

---

**Última actualización**: 2024  
**Rama**: `RQM_BonosDistribuidores_052026`  
**Estado**: En Construcción
