/* Pruebas de SP_GET_COMPETITOR_HOMOLOGATION_REPORT (T13). Crea datos dentro de una transacción y hace ROLLBACK:
   no deja cambios. Requiere en desarrollo las homologaciones de prueba de ARIA (CMP-T11, nivel artículo y Amarillo)
   de la compañía 2 activa y la compañía 1 inactiva (ver bitácora). Resultado 2026-11-07: 13/13 OK; 2026-11-08 ("contiene" con CHARINDEX): 16/16 OK. */
SET NOCOUNT ON;
CREATE TABLE #r (CompetitorCompanyId INT, CompanyName VARCHAR(100), Website VARCHAR(250), IsCompanyActive BIT, LineId SMALLINT, LineName VARCHAR(100), ItemId INT, InternalReference VARCHAR(20), ItemName VARCHAR(200), ReferenceId INT, ReferenceCode VARCHAR(20), ReferenceName VARCHAR(100), CompetitorHomologationId INT, CompetitorReference VARCHAR(50), CompetitorProductName VARCHAR(150), ProductUrl VARCHAR(500), CompetitorPrice DECIMAL(18,2), ImageFileName VARCHAR(100), IsActive BIT);
DECLARE @o TABLE (Caso VARCHAR(70), Esperado INT, Obtenido INT);
BEGIN TRAN;
DECLARE @B INT; INSERT COMPETITOR_COMPANIES(COMPANY_NAME,WEBSITE) VALUES('PRUEBA T13 B','b.com'); SET @B=SCOPE_IDENTITY();
DECLARE @aria INT = (SELECT ITEM_ID FROM items WHERE INTERNAL_REFERENCE='ARIA'), @amarillo INT, @azul INT, @otro INT, @otroRef INT, @ln SMALLINT;
SELECT @amarillo = REFERENCE_ID FROM item_references WHERE ITEM_ID=@aria AND REFERENCE_CODE='01';
SELECT @azul = REFERENCE_ID FROM item_references WHERE ITEM_ID=@aria AND REFERENCE_CODE='03';
SELECT TOP 1 @otro = i.ITEM_ID, @otroRef = r.REFERENCE_ID FROM items i JOIN item_references r ON r.ITEM_ID=i.ITEM_ID WHERE i.ITEM_ID<>@aria ORDER BY i.ITEM_ID;
SELECT @ln = LINE_ID FROM items WHERE ITEM_ID=@aria;
INSERT COMPETITOR_HOMOLOGATIONS(COMPETITOR_COMPANY_ID,ITEM_ID,REFERENCE_ID,COMPETITOR_REFERENCE,COMPETITOR_PRODUCT_NAME) VALUES
 (@B,@aria,@azul,'B-AZUL','Lapicero azul B'), (@B,@otro,NULL,'B-OTRO','Otro producto B'), (1,@aria,NULL,'INACT-CO','Compañía inactiva');
INSERT COMPETITOR_HOMOLOGATIONS(COMPETITOR_COMPANY_ID,ITEM_ID,REFERENCE_ID,COMPETITOR_REFERENCE,COMPETITOR_PRODUCT_NAME,IS_ACTIVE) VALUES (2,@otro,@otroRef,'H-INACT','Homologación inactiva',0);
DECLARE @s NVARCHAR(400), @n VARCHAR(70), @e INT, @sql NVARCHAR(MAX);
DECLARE @t TABLE (n VARCHAR(70), s NVARCHAR(400), e INT);
INSERT @t VALUES
('P01 sin filtros (solo activas de compañías activas)', N'', 4),
('P02 incluye inactivas', N'@IncludeInactive=1', 6),
('P03 por línea', N'@LineIds='''+CAST(@ln AS VARCHAR)+'''', 4),
('P04 por artículo ARIA', N'@ItemIds='''+CAST(@aria AS VARCHAR)+'''', 3),
('P05 por ref. Amarillo (+ nivel artículo)', N'@ReferenceIds='''+CAST(@amarillo AS VARCHAR)+'''', 2),
('P06 por ref. Azul (+ nivel artículo)', N'@ReferenceIds='''+CAST(@azul AS VARCHAR)+'''', 2),
('P07 artículo + ref: prevalece referencia', N'@ItemIds='''+CAST(@aria AS VARCHAR)+''', @ReferenceIds='''+CAST(@amarillo AS VARCHAR)+'''', 2),
('P08 por compañía B', N'@CompetitorCompanyIds='''+CAST(@B AS VARCHAR)+'''', 2),
('P09 compañías 2 y B', N'@CompetitorCompanyIds=''2,'+CAST(@B AS VARCHAR)+'''', 4),
('P10 referencia competidor contiene "t11"', N'@CompetitorReference=''t11''', 2),
('P11 nombre producto contiene "amarillo"', N'@CompetitorProductName=''amarillo''', 1),
('P12 lista inválida no rompe', N'@ItemIds=''x,'+CAST(@aria AS VARCHAR)+',,''', 3),
('P13 compañía B + artículo ARIA', N'@CompetitorCompanyIds='''+CAST(@B AS VARCHAR)+''', @ItemIds='''+CAST(@aria AS VARCHAR)+'''', 1),
('P14 "%" es literal, no comodín', N'@CompetitorReference=''%''', 0),
('P15 "_" es literal, no comodín', N'@CompetitorReference=''CMP_T11''', 0),
('P16 inyección en texto no altera la consulta', N'@CompetitorProductName=''x'''' OR 1=1 --''', 0);
DECLARE k CURSOR LOCAL FAST_FORWARD FOR SELECT n,s,e FROM @t; OPEN k; FETCH k INTO @n,@s,@e;
WHILE @@FETCH_STATUS=0 BEGIN
  SET @sql = N'INSERT #r EXEC dbo.SP_GET_COMPETITOR_HOMOLOGATION_REPORT '+@s; EXEC(@sql);
  INSERT @o SELECT @n,@e,COUNT(*) FROM #r; TRUNCATE TABLE #r; FETCH k INTO @n,@s,@e; END
CLOSE k; DEALLOCATE k;
ROLLBACK;
SELECT Caso, Esperado, Obtenido, CASE WHEN Esperado=Obtenido THEN 'OK' ELSE 'FALLA' END R FROM @o;
