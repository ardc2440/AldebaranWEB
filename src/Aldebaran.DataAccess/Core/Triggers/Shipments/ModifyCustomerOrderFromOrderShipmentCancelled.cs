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

            /* If the new state is not canceled, it does nothing */
            if (statusOrder != 2)
                return;

            var detailChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

            /* If the state does not change, it does nothing */
            if ((short)(detailChanges.oldValue ?? 0) == (short)(detailChanges.newValue ?? 0))
                return;

            var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "D", cancellationToken);
            var statusExcuted = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId &&
                                                                                            i.StatusOrder == 1, cancellationToken);

            documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "P", cancellationToken);
            
            var statusPartiallyAttended = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId &&
                                                                                                      i.StatusOrder == 3, cancellationToken);

            var statusInProcess = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId &&
                                                                                              i.StatusOrder == 2, cancellationToken);
            var newStatus = statusInProcess;

            /* If there are more shipments and the status of the customer order is partially attended, it does nothing*/
            if (await _context.CustomerOrderShipments.AnyAsync(i => i.StatusDocumentTypeId == statusExcuted!.StatusDocumentTypeId &&
                                                                    i.CustomerOrderShipmentId != context.Entity.CustomerOrderShipmentId &&
                                                                    i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken))
                newStatus = statusPartiallyAttended;

            var customerOrder = await _context.CustomerOrders.FirstOrDefaultAsync(i => i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken);

            if (customerOrder!.StatusDocumentTypeId == newStatus!.StatusDocumentTypeId)
                return;

            /* change the state of tehe Customer Order to Partially Attended */
            customerOrder!.StatusDocumentTypeId = newStatus!.StatusDocumentTypeId;
        }
    }
}
