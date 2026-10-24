using Aldebaran.DataAccess.Entities.Reports;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public class AutomaticPurchaseOrderAssigmentReportRepository : RepositoryBase<AldebaranDbContext>, IAutomaticPurchaseOrderAssigmentReportRepository
    {
        public AutomaticPurchaseOrderAssigmentReportRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(AutomaticAssigmentReportFilter filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var command = BuildAssigmentCommand(filter);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<AutomaticCustomerOrderAssigmentReport>().FromSqlRaw(command.Sql, command.Parameters).ToListAsync(ct), ct);
        }

        public async Task<IEnumerable<AutomaticPendingCustomerOrderInProcessReport>> GetAutomaticPendingCustomerOrderInProcessReportDataAsync(AutomaticPendingCustomerOrderInProcessReportFilter filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var command = BuildPendingCommand(filter);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<AutomaticPendingCustomerOrderInProcessReport>().FromSqlRaw(command.Sql, command.Parameters).ToListAsync(ct), ct);
        }

        /// <summary>Las fechas se envían sin hora; Pedido especial Sí/No/Todos (null no se envía = Todos).</summary>
        private static StoredProcedureCommand BuildAssigmentCommand(AutomaticAssigmentReportFilter filter) =>
            new StoredProcedureCommand("SP_AUTOMATIC_CUSTOMER_ORDER_ASSIGMENT_REPORT")
                .VarChar("@DocumentType", filter.DocumentType, 1)
                .VarChar("@DocumentNumber", filter.DocumentNumber, 10)
                .Int("@ProviderId", filter.ProviderId)
                .VarChar("@ProformaNumber", filter.ProformaNumber, 50)
                .VarChar("@ImportNumber", filter.ImportNumber, 20)
                .Date("@ReceiptDateFrom", filter.ReceiptDateFrom)
                .Date("@ReceiptDateTo", filter.ReceiptDateTo)
                .Date("@ConfirmedDateFrom", filter.ConfirmedDateFrom)
                .Date("@ConfirmedDateTo", filter.ConfirmedDateTo)
                .VarChar("@CustomerOrderNumber", filter.CustomerOrderNumber, 10)
                .Int("@CustomerId", filter.CustomerId)
                .Date("@OrderDateFrom", filter.OrderDateFrom)
                .Date("@OrderDateTo", filter.OrderDateTo)
                .Date("@EstimatedDeliveryDateFrom", filter.EstimatedDeliveryDateFrom)
                .Date("@EstimatedDeliveryDateTo", filter.EstimatedDeliveryDateTo)
                .Int("@StatusDocumentTypeId", filter.StatusDocumentTypeId)
                .VarChar("@ReferenceIds", filter.ReferenceIds)
                .Bit("@IsSpecialOrder", filter.IsSpecialOrder);

        private static StoredProcedureCommand BuildPendingCommand(AutomaticPendingCustomerOrderInProcessReportFilter filter) =>
            new StoredProcedureCommand("SP_GET_AUTOMATIC_CUSTOMER_ORDER_INPROCESS_PENDING_REPORT")
                .VarChar("@OrderNumber", filter.OrderNumber, 10)
                .Date("@OrderDateFrom", filter.OrderDateFrom)
                .Date("@OrderDateTo", filter.OrderDateTo)
                .Date("@ProcessDateFrom", filter.ProcessDateFrom)
                .Date("@ProcessDateTo", filter.ProcessDateTo)
                .Int("@CustomerId", filter.CustomerId)
                .Int("@StatusDocumentTypeId", filter.StatusDocumentTypeId)
                .VarChar("@ReferenceIds", filter.ReferenceIds)
                .Bit("@IsSpecialOrder", filter.IsSpecialOrder);
    }
}
