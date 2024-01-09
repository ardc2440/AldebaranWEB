using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustInventoryFromOrderInProcessCancelled : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrdersInProcess>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromOrderInProcessCancelled(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrdersInProcess> context, CancellationToken cancellationToken)
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

            foreach (var item in context.Entity.CustomerOrderInProcessDetails)
            {
                var reference = await _context.CustomerOrderDetails.FindAsync(new object[] { item.CustomerOrderDetailId }, cancellationToken);

                await UpdateWorkInProcessQuantityAsync(reference!.ReferenceId, item.ProcessedQuantity, -1, cancellationToken);
                await UpdateInventoryQuantityAsync(reference!.ReferenceId, item.ProcessedQuantity, 1, cancellationToken);
                await UpdateOrderedQuantityAsync(reference!.ReferenceId, item.ProcessedQuantity, 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(item.WarehouseId, reference!.ReferenceId, item.ProcessedQuantity, 1, cancellationToken);
            }
        }
    }
}
