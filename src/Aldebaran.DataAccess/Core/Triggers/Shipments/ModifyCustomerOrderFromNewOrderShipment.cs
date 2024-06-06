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
            var statusPartiallyAttended = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 3, cancellationToken);
            var statusFullyAttended = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 4, cancellationToken);

            var customerOrder = await _context.CustomerOrders.FirstOrDefaultAsync(i => i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken);

            if (customerOrder!.StatusDocumentTypeId == statusPartiallyAttended!.StatusDocumentTypeId || 
                customerOrder!.StatusDocumentTypeId == statusFullyAttended!.StatusDocumentTypeId)
                return;

            customerOrder!.StatusDocumentTypeId = statusPartiallyAttended!.StatusDocumentTypeId;
        }
    }
}
