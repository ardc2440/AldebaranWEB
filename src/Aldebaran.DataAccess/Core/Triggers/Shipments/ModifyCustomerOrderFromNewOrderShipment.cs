using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class ModifyCustomerOrderFromNewOrderShipment : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderShipment>
    {
        private readonly AldebaranDbContext _context;

        public ModifyCustomerOrderFromNewOrderShipment(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderShipment> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Added)
                return;

            var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "P", cancellationToken);
            var statusInProcessCustomerOrder = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 3, cancellationToken);

            var customerOrder = await _context.CustomerOrders.FirstOrDefaultAsync(i => i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken);

            if (customerOrder!.StatusDocumentTypeId == statusInProcessCustomerOrder!.StatusDocumentTypeId)
                return;
            customerOrder!.StatusDocumentTypeId = statusInProcessCustomerOrder!.StatusDocumentTypeId;
        }
    }
}
