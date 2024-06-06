using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class ModifyCustomerOrderFromOrderInProcessCancelled : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrdersInProcess>
    {
        private readonly AldebaranDbContext _context;

        public ModifyCustomerOrderFromOrderInProcessCancelled(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrdersInProcess> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Modified)
                return;

            /* If new CustomerOrderInprocess's State isn't Cancelled, the process ends and does nothing */
            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 2)
                return;

            /* Get the old and new state value */
            var detailChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));


            /* If the state value doesn't change, the process ends and does nothing*/
            if ((short)(detailChanges.oldValue ?? 0) == (short)(detailChanges.newValue ?? 0))
                return;
                        
            var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "P", cancellationToken);
            var statusDoumentPartialAttended = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && 
                                                                                                           i.StatusOrder == 4, cancellationToken);
            /* If the customer order is in Partial Attended state, the process ends and does nothing*/
            if (await _context.CustomerOrders.AnyAsync(i => i.StatusDocumentTypeId == statusDoumentPartialAttended!.StatusDocumentTypeId &&
                                                            i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken))
                return;

            documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "T", cancellationToken);
            var statusExcuted = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && 
                                                                                            i.StatusOrder == 1, cancellationToken);

            /* if exists CustomerOrderInProcess in Executed State for this CustomerOrder, the process ends and does nothing */
            if (await _context.CustomerOrdersInProcesses.AnyAsync(i => i.StatusDocumentTypeId == statusExcuted!.StatusDocumentTypeId && 
                                                                       i.CustomerOrderInProcessId != context.Entity.CustomerOrderInProcessId && 
                                                                       i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken))
                return;
                        
            documentType = await _context.DocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeCode == "P", cancellationToken);
            var statusPendingCustomerOrder = await _context.StatusDocumentTypes.FirstOrDefaultAsync(i => i.DocumentTypeId == documentType!.DocumentTypeId && 
                                                                                                         i.StatusOrder == 1, cancellationToken);

            var customerOrder = await _context.CustomerOrders.FirstOrDefaultAsync(i => i.CustomerOrderId == context.Entity.CustomerOrderId, cancellationToken);

            /* If the CustomerOrder's state Is Pending, the process ends and does nothing*/
            if (customerOrder!.StatusDocumentTypeId == statusPendingCustomerOrder!.StatusDocumentTypeId)
                return;

            /* Change the CustomerOrder's State to Pending*/
            customerOrder!.StatusDocumentTypeId = statusPendingCustomerOrder!.StatusDocumentTypeId;
        }
    }
}
