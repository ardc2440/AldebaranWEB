# Criterios de desarrollo – Aldebaran.Web (aplican a todo requerimiento)

Definidos por Andrés Ricardo Díaz Cárdenas. Se aplican siempre, sin necesidad de repetirlos al iniciar un requerimiento.

## Forma de trabajo
1. Primero se valida, se identifica y se arma el plan de trabajo; se aprueba, luego se crea la rama y se implementa.
2. Tareas de unidad mínima comprobable: implementar → probar → corregir → OK → siguiente. Nunca un desarrollo completo de una vez.
3. Regresión acumulada: la prueba de cada tarea repite las pruebas de todas las anteriores. Si algo se daña, la causa es la última tarea.
4. Preguntar antes de explorar el código a ciegas.

## Arquitectura y código
5. Clean Architecture: la UI no conoce la capa de datos; todo pasa por Application.Services.
6. Application.Services = casos de uso, no copias de la data. Sus Models se definen según el caso de uso que atienden.
7. SOLID / responsabilidad única: métodos cortos con tareas específicas y orquestadores que los coordinan. Nada de métodos de cientos de líneas.
8. No modificar componentes compartidos (ej. MultiReferencePicker); reutilizarlos sin tocarlos para no re-probar toda la aplicación.
9. SP con parámetros (`SqlParameter`), nunca SQL armado por concatenación. Si un desarrollo toca una pantalla/repositorio que concatena SQL, se corrige (filtro tipado + `StoredProcedureCommand`).
10. Codificación estándar: **UTF-8 sin BOM, CRLF** (código .cs, .razor, .sql, configuración y texto).
    - Archivos nuevos: siempre en el estándar.
    - Archivos existentes que se modifican en un desarrollo: se convierten al estándar (de UTF-8 con BOM se quita el BOM; de ISO-8859/Latin-1 se convierte a UTF-8 verificando que tildes y eñes se vean bien en pantalla y mensajes).
    - Archivos antiguos que el desarrollo no toca: se dejan como están (convertirlos masivamente exigiría re-probar toda la presentación).
    - Decisión de Andrés del 23-oct-2026 (RQM Complemento Consulta de Logs).
11. Nunca entregar un desarrollo con errores: un error preexistente dentro del alcance se corrige en ese mismo desarrollo.

## Interfaz
12. Filtros condicionales Sí / No / Todos: **casilla de tres estados** (`RadzenCheckBox TriState="true" TValue="bool?"`, valor inicial null): **raya = Todos** (valor por defecto), **chulo = Sí**, **vacía = No**. Mismo estilo que las demás casillas del formulario. Si un desarrollo toca una pantalla que no cumple el criterio, se ajusta en lo posible. (Decisión de Andrés del 23-oct-2026.)

## Documentación
13. Manuales y documentos entregables en Word, con la línea visual de las propuestas.
