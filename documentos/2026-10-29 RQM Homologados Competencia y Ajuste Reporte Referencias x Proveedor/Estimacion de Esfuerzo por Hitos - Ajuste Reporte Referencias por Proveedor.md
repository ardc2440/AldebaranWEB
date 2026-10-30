# ESTIMACIÓN DE ESFUERZO POR HITOS – Ajuste al Reporte de Referencias del Proveedor

**Uso interno. No se entrega al cliente.** Insumo para calcular duración y costo de la propuesta.
**Fecha**: 2026-10-29 · **Base**: `01 - Definicion funcional - Ajuste Reporte Referencias por Proveedor.md`

## 1. Criterios
| Factor | Criterio |
|---|---|
| Perfil | Analista + Arquitecto + Desarrollador Senior .NET / Blazor / Radzen / SQL Server, conoce el codebase |
| Unidad | Horas efectivas · 8 h = 1 día hábil |
| Método | Tareas mínimas comprobables con regresión acumulada |
| Reutilización | `HierarchicalReferencePicker`, reglas de switches, `printHtml`, generación de Excel y patrón de servicio (árbol + filas planas) del Reporte de Artículos y Referencias; `FileBytesGeneratorService` para PDF |

## 2. Hitos
### H1 – Base de datos — 6 h
| # | Tarea | Horas |
|---|---|---|
| 1.1 | Ajuste `SP_GET_PROVIDER_REFERENCE_REPORT`: Líneas/Artículos/Referencias (nivel más específico), 7 switches, `TRY_CAST`, PK, `OPTION(RECOMPILE)`; idempotente | 4 |
| 1.2 | Script de pruebas del SP (casos por filtro y switch) | 2 |

### H2 – Application (caso de uso) — 8 h
| # | Tarea | Horas |
|---|---|---|
| 2.1 | Modelo de filtro + repositorio con parámetros | 2 |
| 2.2 | `GetReportAsync`: árbol Proveedor → Línea → Artículo → Referencia → Bodegas, agrupación O(n) | 3 |
| 2.3 | `GetExportAsync`: filas planas con/sin inventario | 2 |
| 2.4 | DI, mapeos, retiro de la lógica del `.razor.cs` | 1 |

### H3 – Filtros en pantalla — 6 h
| # | Tarea | Horas |
|---|---|---|
| 3.1 | Proveedor + `HierarchicalReferencePicker` | 2 |
| 3.2 | 7 switches de filtro + switch "Mostrar inventario y bodegas" (Sí por defecto); switches filtran el picker; validación | 3 |
| 3.3 | Resumen de filtros en pantalla y en el encabezado del documento | 1 |

### H4 – Visualización, Imprimir y PDF — 5 h
| # | Tarea | Horas |
|---|---|---|
| 4.1 | Render condicional con/sin inventario y bodegas | 2 |
| 4.2 | Imprimir y Guardar PDF en ambas modalidades | 2 |
| 4.3 | Ajustes de rendimiento con catálogo completo (12.383 relaciones proveedor-referencia) | 1 |

### H5 – Exportar a Excel — 4 h
| 5.1 | Excel con/sin inventario (columnas Bodega Local y Zona Franca), botón "Exportar → Excel" | 4 |
|---|---|---|

### H6 – Pruebas, documentación e instalación — 9 h
| # | Tarea | Horas |
|---|---|---|
| 6.1 | Prueba integral + regresión (Reporte de Artículos y Referencias, Inventario, otros usos del picker) | 3 |
| 6.2 | Estabilización / corrección de defectos | 2 |
| 6.3 | Manual funcional y técnico | 3 |
| 6.4 | Script y manual de despliegue | 1 |

## 3. Resumen
| Hito | Horas |
|---|---|
| H1 Base de datos | 6 |
| H2 Application | 8 |
| H3 Filtros | 6 |
| H4 Visualización / PDF | 5 |
| H5 Excel | 4 |
| H6 Pruebas, documentación, instalación | 9 |
| **Total** | **38 h ≈ 4,75 días** |

## 4. Traducción a la tabla de la propuesta (días hábiles)
| Fase | Días |
|---|---|
| Preparación de equipo y ambientes | 0,5 |
| Exploración del negocio | 0 |
| Desarrollo del sistema – (1) Iteración (H1–H5) | 4 |
| Documentación, pruebas integrales, estabilización y capacitación (H6.1–6.3) | 1 |
| Instalación | 0,5 |
| Acompañamiento en producción | 0,5 |
| **Total** | **6,5** |

Valor: calcular en la hoja de costos (4 roles, % de dedicación) y llevarlo a la tabla "Inversión y forma de pago" (40% / 60%).

---

# PARTE 2 – Catálogo de la Competencia (Homologaciones)
Base: `02 - Definicion funcional - Catalogo de la Competencia.md`

### H7 – Base de datos — 8 h
| # | Tarea | Horas |
|---|---|---|
| 7.1 | Tablas compañías y homologaciones (FK geografía nullables + CHECK de cascada, referencia opcional, índices/únicos), rol y menú | 4 |
| 7.2 | SP del reporte (jerarquía + filtros de competencia) y script de pruebas | 4 |

### H8 – Dominio y Application — 8 h
| 8.1 | Entidades, configuraciones EF, repositorios, servicios por caso de uso, validaciones | 8 |
|---|---|---|

### H9 – Compañías de la competencia — 8 h
| 9.1 | Listado, crear, editar, inactivar; cascada País → Departamento → Ciudad | 8 |
|---|---|---|

### H10 – Homologaciones — 14 h
| # | Tarea | Horas |
|---|---|---|
| 10.1 | Listado con filtros y crear/editar/inactivar a nivel Artículo o Artículo-Referencia | 8 |
| 10.2 | Carga, reemplazo y visualización de imagen (carpeta configurada, validación tipo/tamaño) | 6 |

### H11 – Reporte de homologaciones — 16 h
| # | Tarea | Horas |
|---|---|---|
| 11.1 | Selector jerárquico limitado a lo homologado (clase hija, sin modificar el picker base) + filtros de competencia | 5 |
| 11.2 | Caso de uso (árbol y filas planas), pantalla con miniaturas | 5 |
| 11.3 | Imprimir, PDF y Excel | 6 |

### H12 – Seguridad — 2 h
| 12.1 | Rol "Catálogo de la competencia" aplicado a ambas opciones; reporte con roles de reportes | 2 |
|---|---|---|

### H13 – Pruebas, documentación e instalación — 10 h
| 13.1 | Prueba integral + regresión, estabilización | 5 |
|---|---|---|
| 13.2 | Manual funcional, técnico y de despliegue | 5 |

**Total parte 2: 66 h ≈ 8,25 días**

## Consolidado propuesta (parte 1 + parte 2 = 104 h)
| Fase | Días |
|---|---|
| Preparación de equipo y ambientes | 0,5 |
| Exploración del negocio | 0 |
| Desarrollo del sistema – (2) Iteraciones (H1–H5, H7–H12) | 10,5 |
| Documentación, pruebas integrales, estabilización y capacitación | 2,5 |
| Instalación | 0,5 |
| Acompañamiento en producción | 0,5 |
| **Total** | **14,5** |

---

## Distribución de recursos (para la hoja de costos, mes 1)
| Rol | Horas | Dedicación |
|---|---|---|
| Senior Developer/Arquitecto (Javier) | 8 | 5% |
| Senior Developer (Andres) | 87 | 66% (ocupación del calendario: 14,5 de 22 días) |
| Analista de pruebas | 6 (+ prueba conjunta) | 5% |
| Analista funcional | 3 (+ revisión con cliente) | 3% |
