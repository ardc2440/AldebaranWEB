/* =====================================================================================
   RQM Complemento Consulta de Logs - T6: Pruebas de la consulta de Track
   Ejecutar después de "Despliegue\07 - Consulta Track.sql" y "08 - Busqueda Track por objetos.sql". Solo lectura. Revisar OK (1 = pasa).
   ===================================================================================== */
SET NOCOUNT ON;
DECLARE @r TABLE (PRUEBA VARCHAR(80), RESULTADO NVARCHAR(300), OK BIT);
DECLARE @t TABLE (ID BIGINT, ENTITY_NAME NVARCHAR(200), ENTITY_DISPLAY_NAME NVARCHAR(150), ENTITY_KEY NVARCHAR(4000), RECORD_DESCRIPTION NVARCHAR(4000), ACTION VARCHAR(10), MODIFIED_DATE DATETIME, MODIFIER_NAME NVARCHAR(100), TOTAL_ROWS INT);
DECLARE @d TABLE (ID BIGINT, ACTION VARCHAR(10), DATA_LOG NVARCHAR(MAX), PREVIOUS_ID BIGINT, PREVIOUS_DATA_LOG NVARCHAR(MAX));
DECLARE @Entity NVARCHAR(200) = N'ItemReference', @From DATE = DATEADD(MONTH, -3, CAST(GETDATE() AS DATE)), @To DATE = CAST(GETDATE() AS DATE);
DECLARE @total INT, @esperado INT, @id BIGINT, @prev BIGINT, @prevEsperado BIGINT, @key NVARCHAR(100);

/* K1: catálogo cubre todas las entidades presentes en log.track */
INSERT @r SELECT 'K1 Catálogo cubre las entidades trazadas', CONCAT(COUNT(*), N' sin nombre'),
       CASE WHEN COUNT(*) = 0 THEN 1 ELSE 0 END
  FROM (SELECT DISTINCT ENTITY_NAME FROM log.track WHERE MODIFIED_DATE >= DATEADD(YEAR, -1, GETDATE())) x
 WHERE NOT EXISTS (SELECT 1 FROM LOG_TRACK_ENTITY_NAMES n WHERE n.ENTITY_NAME = x.ENTITY_NAME);

/* K2: total = COUNT(*) directo */
INSERT @t EXEC SP_GET_TRACK_LOG @Entity, @From, @To, NULL, NULL, NULL, NULL, 0, 5;
SELECT @total = MAX(TOTAL_ROWS) FROM @t;
SELECT @esperado = COUNT(*) FROM log.track WHERE ENTITY_NAME = @Entity AND MODIFIED_DATE >= @From AND MODIFIED_DATE < DATEADD(DAY, 1, CAST(@To AS DATETIME));
INSERT @r SELECT 'K2 Total = COUNT(*)', CONCAT(@total, N' vs ', @esperado), CASE WHEN ISNULL(@total, 0) = @esperado THEN 1 ELSE 0 END;

/* K3: filtro por acción */
DELETE @t; INSERT @t EXEC SP_GET_TRACK_LOG @Entity, @From, @To, 'Added', NULL, NULL, NULL, 0, 500;
INSERT @r SELECT 'K3 Filtro por acción', CONCAT(COUNT(*), N' filas'), CASE WHEN NOT EXISTS (SELECT 1 FROM @t WHERE ACTION <> 'Added') THEN 1 ELSE 0 END FROM @t;

/* K4: filtro por entidad + objeto (referencia) */
SELECT TOP 1 @key = JSON_VALUE(CAST(ENTITY_KEY AS NVARCHAR(4000)), '$.ReferenceId') FROM log.track WHERE ENTITY_NAME = @Entity AND MODIFIED_DATE >= @From ORDER BY ID DESC;
DELETE @t; INSERT @t EXEC SP_GET_TRACK_LOG @Entity, @From, @To, NULL, NULL, 2, @key, 0, 500;
INSERT @r SELECT 'K4 Entidad + objeto', CONCAT(N'referencia ', @key, N': ', COUNT(*), N' filas'),
       CASE WHEN COUNT(*) > 0 AND NOT EXISTS (SELECT 1 FROM @t WHERE ENTITY_KEY NOT LIKE N'%"' + @key + N'"%' OR ENTITY_NAME <> @Entity) THEN 1 ELSE 0 END FROM @t;

/* K5: el detalle trae el movimiento anterior del mismo registro */
SELECT TOP 1 @id = ID FROM @t ORDER BY ID DESC;
SELECT TOP 1 @prevEsperado = x.ID FROM log.track x JOIN log.track c ON c.ID = @id
 WHERE x.ENTITY_NAME = c.ENTITY_NAME AND x.ENTITY_KEY_HASH = c.ENTITY_KEY_HASH AND x.ID < @id ORDER BY x.MODIFIED_DATE DESC, x.ID DESC;
INSERT @d EXEC SP_GET_TRACK_DETAIL @id;
SELECT @prev = PREVIOUS_ID FROM @d;
INSERT @r SELECT 'K5 Detalle con movimiento anterior', CONCAT(@id, N' → anterior ', ISNULL(CAST(@prev AS VARCHAR(20)), '-')),
       CASE WHEN ISNULL(@prev, -1) = ISNULL(@prevEsperado, -1) AND EXISTS (SELECT 1 FROM @d WHERE DATA_LOG IS NOT NULL) THEN 1 ELSE 0 END;

/* K6 / K7: validaciones */
BEGIN TRY
    EXEC SP_GET_TRACK_LOG NULL, @From, @To;
    INSERT @r VALUES ('K6 Entidad u objeto obligatorio (50001)', N'No lanzó error', 0);
END TRY
BEGIN CATCH
    INSERT @r VALUES ('K6 Entidad u objeto obligatorio (50001)', ERROR_MESSAGE(), CASE WHEN ERROR_NUMBER() = 50001 THEN 1 ELSE 0 END);
END CATCH
BEGIN TRY
    EXEC SP_GET_TRACK_LOG @Entity, @To, @From;
    INSERT @r VALUES ('K7 Rango inválido (50002)', N'No lanzó error', 0);
END TRY
BEGIN CATCH
    INSERT @r VALUES ('K7 Rango inválido (50002)', ERROR_MESSAGE(), CASE WHEN ERROR_NUMBER() = 50002 THEN 1 ELSE 0 END);
END CATCH

SELECT * FROM @r;
