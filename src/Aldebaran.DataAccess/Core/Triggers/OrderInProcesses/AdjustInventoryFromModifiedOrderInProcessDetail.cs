using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustInventoryFromModifiedOrderInProcessDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderInProcessDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromModifiedOrderInProcessDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderInProcessDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var detailChanges = context.Entity.GetType()
                    .GetProperties()
                    .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                    .Where(x => x.newValue != x.oldValue);

                var customerOrderDetail = detailChanges.FirstOrDefault(x => x.name.Equals("CustomerOrderDetailId"));
                var quantity = detailChanges.FirstOrDefault(x => x.name.Equals("ProcessedQuantity"));

                var oldReference = await _context.CustomerOrderDetails.FindAsync(new object[] { (int)(customerOrderDetail.oldValue ?? 0) }, cancellationToken);
                await UpdateWorkInProcessQuantityAsync(oldReference!.ReferenceId, (int)(quantity.oldValue ?? 0), -1, cancellationToken);
                await UpdateInventoryQuantityAsync(oldReference!.ReferenceId, (int)(quantity.oldValue ?? 0), 1, cancellationToken);
                await UpdateOrderedQuantityAsync(oldReference!.ReferenceId, (int)(quantity.oldValue ?? 0), 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.WarehouseId, oldReference!.ReferenceId, (int)(quantity.oldValue ?? 0), 1, cancellationToken);

                var newReference = await _context.CustomerOrderDetails.FindAsync(new object[] { (int)(customerOrderDetail.newValue ?? 0) }, cancellationToken);
                await UpdateWorkInProcessQuantityAsync(newReference!.ReferenceId, (int)(quantity.newValue ?? 0), 1, cancellationToken);
                await UpdateInventoryQuantityAsync(newReference!.ReferenceId, (int)(quantity.newValue ?? 0), -1, cancellationToken);
                await UpdateOrderedQuantityAsync(newReference!.ReferenceId, (int)(quantity.newValue ?? 0), -1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.WarehouseId, newReference!.ReferenceId, (int)(quantity.newValue ?? 0), -1, cancellationToken);
            }
        }
    }
}
