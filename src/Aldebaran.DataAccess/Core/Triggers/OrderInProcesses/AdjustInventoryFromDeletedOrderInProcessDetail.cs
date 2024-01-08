using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustInventoryFromDeletedOrderInProcessDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderInProcessDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromDeletedOrderInProcessDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderInProcessDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                await UpdateWorkInProcessQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, -1, cancellationToken);
                await UpdateInventoryQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
                await UpdateOrderedQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.WarehouseId, context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
            }
        }
    }
}
