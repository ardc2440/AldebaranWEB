# 02 – Definición funcional: Catálogo de la Competencia (Homologaciones)

Fecha: 2026-10-29 · Estado: definida con Andrés; pendiente propuesta/aprobación del cliente.

## Necesidad
PROMOS homologa artículos/referencias propios con productos de compañías de la competencia (hoy 3, pueden crecer). Homologar = la competencia también los vende o produce. Hoy se lleva a mano; se quiere administrar en Aldebarán y consultarlo en un reporte. **Solo informativo**: no participa en ninguna operación.

## Menú
- **Administración → Competencia** (subgrupo, como "Log del sistema"): "Compañías de la competencia" y "Homologaciones con la competencia".
- **Reportes → Catálogos → "Homologaciones con la competencia"**.
- No se crea agrupador principal nuevo (2 opciones no lo justifican; se puede subir después).

## Compañías de la competencia
- Obligatorios: Nombre, Sitio web.
- Opcionales: Contacto, Teléfono, País, Departamento/Estado, Ciudad.
- Cascada: Departamento solo si hay País; Ciudad solo si hay Departamento. Ninguno es obligatorio. Se usan las tablas existentes `countries`, `departments`, `cities`.

## Homologaciones
- Nivel abierto: **Artículo** o **Artículo + Referencia** (Referencia opcional), según convenga al usuario.
- Obligatorios: Compañía, Artículo, Referencia del competidor, Nombre del producto del competidor.
- Opcionales: Referencia propia, Enlace, Precio de la competencia, Imagen.
- Imagen: archivo en carpeta configurada (mismo enfoque que `ImageRepositoryPath`), no en BD.

## Reporte
- Filtro: selector jerárquico Línea → Artículo → Referencia **limitado a lo homologado** (no se aplican los switches normales).
- Filtros de la competencia: Compañía(s), Referencia y Nombre del producto del competidor.
- Presentación: Compañía → Línea → Artículo → (Referencia) → productos homologados (referencia, nombre, precio, enlace, imagen en miniatura).
- Imprimir, PDF y Excel (Excel sin imagen).

## Seguridad
- Un solo rol nuevo **"Catálogo de la competencia"** para ambas opciones de administración (quien homologa crea las compañías).
- Reporte: roles Administrador y Consulta de reportes, como los demás.

## Supuestos a confirmar
- Compañías y homologaciones se **inactivan**, no se eliminan físicamente (eliminar solo si no tiene homologaciones).
- No se valida unicidad de la referencia del competidor entre compañías; sí que no se repita la misma homologación (compañía + artículo/referencia + referencia del competidor).
- La moneda del precio es pesos colombianos.
