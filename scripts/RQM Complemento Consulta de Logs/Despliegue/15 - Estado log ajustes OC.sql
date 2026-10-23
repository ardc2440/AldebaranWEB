/* =====================================================================================
   RQM Complemento Consulta de Logs - T18 (deuda 4): estado en el log de aprobación de ajustes de OC
   -------------------------------------------------------------------------------------
   La app grababa en PURCHASE_ORDER_ADJUSTMENT_LOG, al APROBAR un ajuste, el estado que traía
   la pantalla ("Ajuste en aprobación") en lugar del estado nuevo ("Confirmada").
   Corrección de código: PurchaseOrderRepository.ConfirmAsync. Este script corrige el histórico.
   - Masivo, sin llaves fijas: toda fila del log con estado "Ajuste en aprobación" (orden 4) es
     una aprobación -> pasa a "Confirmada" (orden 2). Las devoluciones (Pendiente) no se tocan.
   - Respaldo previo en RQM_LOGS_PO_ADJUSTMENT_STATUS_BACKUP para la reversa.
   Re-ejecutable. Reversa: Reversa\15 - Reversa estado log ajustes OC.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID('dbo.RQM_LOGS_PO_ADJUSTMENT_STATUS_BACKUP') IS NULL
    CREATE TABLE dbo.RQM_LOGS_PO_ADJUSTMENT_STATUS_BACKUP (
        PURCHASE_ORDER_ADJUSTMENT_LOG_ID INT NOT NULL PRIMARY KEY,
        PREVIOUS_STATUS_DOCUMENT_TYPE_ID SMALLINT NOT NULL,
        BACKUP_DATE DATETIME NOT NULL DEFAULT (GETDATE()));
GO

DECLARE @InApproval SMALLINT, @Confirmed SMALLINT;
SELECT @InApproval = MAX(CASE WHEN s.STATUS_ORDER = 4 THEN s.STATUS_DOCUMENT_TYPE_ID END),
       @Confirmed  = MAX(CASE WHEN s.STATUS_ORDER = 2 THEN s.STATUS_DOCUMENT_TYPE_ID END)
  FROM dbo.status_document_types s
  JOIN dbo.document_types d ON d.DOCUMENT_TYPE_ID = s.DOCUMENT_TYPE_ID
 WHERE d.DOCUMENT_TYPE_CODE = 'O';

IF @InApproval IS NULL OR @Confirmed IS NULL
    THROW 50010, N'No se encontraron los estados de OC "Ajuste en aprobación" / "Confirmada".', 1;

BEGIN TRAN;

    INSERT INTO dbo.RQM_LOGS_PO_ADJUSTMENT_STATUS_BACKUP (PURCHASE_ORDER_ADJUSTMENT_LOG_ID, PREVIOUS_STATUS_DOCUMENT_TYPE_ID)
    SELECT l.PURCHASE_ORDER_ADJUSTMENT_LOG_ID, l.NEW_STATUS_DOCUMENT_TYPE_ID
      FROM dbo.PURCHASE_ORDER_ADJUSTMENT_LOG l
     WHERE l.NEW_STATUS_DOCUMENT_TYPE_ID = @InApproval
       AND NOT EXISTS (SELECT 1 FROM dbo.RQM_LOGS_PO_ADJUSTMENT_STATUS_BACKUP b
                        WHERE b.PURCHASE_ORDER_ADJUSTMENT_LOG_ID = l.PURCHASE_ORDER_ADJUSTMENT_LOG_ID);

    UPDATE dbo.PURCHASE_ORDER_ADJUSTMENT_LOG
       SET NEW_STATUS_DOCUMENT_TYPE_ID = @Confirmed
     WHERE NEW_STATUS_DOCUMENT_TYPE_ID = @InApproval;

    PRINT CONCAT('Aprobaciones corregidas: ', @@ROWCOUNT);

COMMIT;
GO
