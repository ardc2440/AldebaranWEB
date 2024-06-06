using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class ModifyCustomerOrderFromNewOrderInProcess : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrdersInProcess>
    {
        private readonly AldebaranDbContext _context;

        public ModifyCustomerOrderFromNewOrderInProcess(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrdersInProcess> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Added)
                return;

            /* obtain the statuses In process and Partially attended */
            var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "P", cancellationToken);
            var statusInProcess = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 2, cancellationToken);
            var statusPartiallyAttended = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 3, cancellationToken);

            var customerOrder = await _context.CustomerOrders.FirstOrDefaultAsync(i => i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken);

            /* if the customer order is In process or Partially attended state, Nothing is done, the current state is maintained*/
            if (customerOrder!.StatusDocumentTypeId == statusInProcess!.StatusDocumentTypeId ||
                customerOrder!.StatusDocumentTypeId == statusPartiallyAttended!.StatusDocumentTypeId)
                return;

            /* is assigned to the customer order, the status in process */
            customerOrder!.StatusDocumentTypeId = statusInProcess!.StatusDocumentTypeId;
        }
    }
}
