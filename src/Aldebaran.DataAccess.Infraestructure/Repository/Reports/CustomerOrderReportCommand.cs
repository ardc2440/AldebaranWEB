using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    /// <summary>Llamada parametrizada a los SP de reportes de pedidos que comparten los mismos filtros.</summary>
    internal static class CustomerOrderReportCommand
    {
        /// <summary>Las fechas se envían sin hora (igual que antes, 'yyyyMMdd'); Pedido especial Sí/No/Todos (null no se envía = Todos).</summary>
        public static StoredProcedureCommand Build(string procedure, CustomerOrderReportFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return new StoredProcedureCommand(procedure)
                .VarChar("@OrderNumber", filter.OrderNumber, 10)
                .Date("@CreationDateFrom", filter.CreationDateFrom)
                .Date("@CreationDateTo", filter.CreationDateTo)
                .Date("@OrderDateFrom", filter.OrderDateFrom)
                .Date("@OrderDateTo", filter.OrderDateTo)
                .Date("@EstimatedDeliveryDateFrom", filter.EstimatedDeliveryDateFrom)
                .Date("@EstimatedDeliveryDateTo", filter.EstimatedDeliveryDateTo)
                .Int("@StatusDocumentTypeId", filter.StatusDocumentTypeId)
                .Int("@CustomerId", filter.CustomerId)
                .VarChar("@ReferenceIds", filter.ReferenceIds)
                .Bit("@IsSpecialOrder", filter.IsSpecialOrder);
        }
    }
}
