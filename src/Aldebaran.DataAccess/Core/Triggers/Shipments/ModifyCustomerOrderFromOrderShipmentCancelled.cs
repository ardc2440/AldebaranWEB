using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class ModifyCustomerOrderFromOrderShipmentCancelled : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderShipment>
    {
        private readonly AldebaranDbContext _context;

        public ModifyCustomerOrderFromOrderShipmentCancelled(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderShipment> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Modified)
                return;

            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 2)
                return;

            var detailChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

            if ((short)(detailChanges.oldValue ?? 0) == (short)(detailChanges.newValue ?? 0))
                return;

            var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "D", cancellationToken);
            var statusExcutedCustomerOrderInProcess = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 1, cancellationToken);

            if (_context.CustomerOrderShipments.Any(i => i.StatusDocumentTypeId == statusExcutedCustomerOrderInProcess!.StatusDocumentTypeId && i.CustomerOrderShipmentId != context.Entity.CustomerOrderShipmentId))
                return;

            documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "P", cancellationToken);
            var statusInProcessCustomerOrder = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && i.StatusOrder == 2, cancellationToken);

            var customerOrder = await _context.CustomerOrders.FirstOrDefaultAsync(i => i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken);

            if (customerOrder!.StatusDocumentTypeId == statusInProcessCustomerOrder!.StatusDocumentTypeId)
                return;

            customerOrder!.StatusDocumentTypeId = statusInProcessCustomerOrder!.StatusDocumentTypeId;
        }
    }
}
