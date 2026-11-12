/* =====================================================================================
   Control de Cambios 001 - Reversa Tarea 2
   Deja sin efecto el cargue de homologaciones de la competencia.
   - Inactiva (IS_ACTIVE=0) las homologaciones de las 3 companias cargadas.
   - Inactiva las 3 companias (no se eliminan, por si hay auditoria/Track).
   Para BORRADO fisico (solo si se confirma que no hay datos capturados a mano encima),
   descomentar el bloque DELETE del final.
   ===================================================================================== */
SET NOCOUNT ON;
UPDATE h SET IS_ACTIVE = 0
  FROM dbo.COMPETITOR_HOMOLOGATIONS h
  JOIN dbo.COMPETITOR_COMPANIES c ON c.COMPETITOR_COMPANY_ID = h.COMPETITOR_COMPANY_ID
 WHERE c.COMPANY_NAME IN ('Marpico','StockSur','Promoopcion') AND h.IS_ACTIVE = 1;
PRINT CONCAT('Homologaciones inactivadas: ', @@ROWCOUNT);
UPDATE dbo.COMPETITOR_COMPANIES SET IS_ACTIVE = 0 WHERE COMPANY_NAME IN ('Marpico','StockSur','Promoopcion') AND IS_ACTIVE = 1;
PRINT CONCAT('Companias inactivadas: ', @@ROWCOUNT);

/* -- BORRADO FISICO (opcional, irreversible):
DELETE h FROM dbo.COMPETITOR_HOMOLOGATIONS h JOIN dbo.COMPETITOR_COMPANIES c ON c.COMPETITOR_COMPANY_ID=h.COMPETITOR_COMPANY_ID
 WHERE c.COMPANY_NAME IN ('Marpico','StockSur','Promoopcion');
DELETE FROM dbo.COMPETITOR_COMPANIES WHERE COMPANY_NAME IN ('Marpico','StockSur','Promoopcion');
*/
