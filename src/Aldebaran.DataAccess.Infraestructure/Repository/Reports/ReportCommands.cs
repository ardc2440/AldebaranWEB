namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    /// <summary>
    /// Llamadas parametrizadas (SqlParameter) a los SP de reportes que antes recibían los filtros concatenados en el texto SQL.
    /// Los valores sin dato no se envían y el SP aplica su valor por defecto, igual que cuando el filtro no se incluía.
    /// </summary>
    public static class ReportCommands
    {
        /// <summary>SP_GET_CUSTOMER_RESERVATION_REPORT.</summary>
        public static StoredProcedureCommand CustomerReservation(DataAccess.Entities.Reports.CustomerReservationReportFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return new StoredProcedureCommand("dbo.SP_GET_CUSTOMER_RESERVATION_REPORT")
                .VarChar("@ReservationNumber", filter.ReservationNumber, 10)
                .Date("@CreationDateFrom", filter.CreationDateFrom)
                .Date("@CreationDateTo", filter.CreationDateTo)
                .Date("@ReservationDateFrom", filter.ReservationDateFrom)
                .Date("@ReservationDateTo", filter.ReservationDateTo)
                .Int("@StatusDocumentTypeId", filter.StatusDocumentTypeId)
                .Int("@CustomerId", filter.CustomerId)
                .VarChar("@ReferenceIds", filter.ReferenceIds);
        }

        /// <summary>SP_GET_INVENTORY_ADJUSTMENT_REPORT.</summary>
        public static StoredProcedureCommand InventoryAdjustment(DataAccess.Entities.Reports.InventoryAdjustmentReportFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return new StoredProcedureCommand("dbo.SP_GET_INVENTORY_ADJUSTMENT_REPORT")
                .Int("@AdjustmentId", filter.AdjustmentId)
                .Date("@AdjustmentDateFrom", filter.AdjustmentDateFrom)
                .Date("@AdjustmentDateTo", filter.AdjustmentDateTo)
                .Date("@CreationDateFrom", filter.CreationDateFrom)
                .Date("@CreationDateTo", filter.CreationDateTo)
                .Int("@AdjustmentTypeId", filter.AdjustmentTypeId)
                .Int("@AdjustmentReasonId", filter.AdjustmentReasonId)
                .Int("@EmployeeId", filter.EmployeeId)
                .VarChar("@ReferenceIds", filter.ReferenceIds);
        }

        /// <summary>SP_GET_PURCHASE_ORDER_REPORT.</summary>
        public static StoredProcedureCommand PurchaseOrder(DataAccess.Entities.Reports.PurchaseOrderReportFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return new StoredProcedureCommand("dbo.SP_GET_PURCHASE_ORDER_REPORT")
                .VarChar("@OrderNumber", filter.OrderNumber, 10)
                .Date("@CreationDateFrom", filter.CreationDateFrom)
                .Date("@CreationDateTo", filter.CreationDateTo)
                .Date("@RequestDateFrom", filter.RequestDateFrom)
                .Date("@RequestDateTo", filter.RequestDateTo)
                .Date("@ExpectedReceiptDateFrom", filter.ExpectedReceiptDateFrom)
                .Date("@ExpectedReceiptDateTo", filter.ExpectedReceiptDateTo)
                .VarChar("@ReferenceIds", filter.ReferenceIds)
                .VarChar("@ImportNumber", filter.ImportNumber, 30)
                .VarChar("@EmbarkationPort", filter.EmbarkationPort, 30)
                .VarChar("@ProformaNumber", filter.ProformaNumber, 30)
                .Int("@ProviderId", filter.ProviderId)
                .Int("@ForwarderId", filter.ForwarderId)
                .Int("@ForwarderAgentId", filter.ForwarderAgentId)
                .Int("@ShipmentMethodId", filter.ShipmentMethodId)
                .Int("@WarehouseId", filter.WarehouseId)
                .Int("@StatusDocumentTypeId", filter.StatusDocumentTypeId)
                .Bit("@RequestedQuantityDifference", filter.RequestedQuantityDifference ? true : null);
        }

        /// <summary>SP_GET_WAREHOUSE_STOCK_REPORT.</summary>
        public static StoredProcedureCommand WarehouseStock(DataAccess.Entities.Reports.WarehouseStockReportFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return new StoredProcedureCommand("dbo.SP_GET_WAREHOUSE_STOCK_REPORT")
                .Int("@WarehouseId", filter.WarehouseId)
                .VarChar("@ReferenceIds", filter.ReferenceIds);
        }

        /// <summary>SP_GET_WAREHOUSE_TRANSFER_REPORT.</summary>
        public static StoredProcedureCommand WarehouseTransfer(DataAccess.Entities.Reports.WarehouseTransferReportFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return new StoredProcedureCommand("dbo.SP_GET_WAREHOUSE_TRANSFER_REPORT")
                .Int("@TargetWarehouseId", filter.TargetWarehouseId)
                .Int("@SourceWarehouseId", filter.SourceWarehouseId)
                .Date("@AdjustmentDateFrom", filter.AdjustmentDateFrom)
                .Date("@AdjustmentDateTo", filter.AdjustmentDateTo)
                .VarChar("@NationalizationNumber", filter.NationalizationNumber, 30)
                .Int("@StatusId", filter.StatusId)
                .VarChar("@ReferenceIds", filter.ReferenceIds);
        }

        /// <summary>SP que solo reciben @ReferenceIds (Inventario, Inventario en proceso, Zona franca vs. disponible). Siempre se envía; vacío = todas.</summary>
        public static StoredProcedureCommand ByReferenceIds(string procedure, string? referenceIds) =>
            new StoredProcedureCommand(procedure).VarCharOrEmpty("@ReferenceIds", referenceIds);
    }
}
