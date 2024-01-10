using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustInventoryFromNewOrderInProcessDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderInProcessDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromNewOrderInProcessDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderInProcessDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                await UpdateWorkInProcessQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
                await UpdateInventoryQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, -1, cancellationToken);
                await UpdateOrderedQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, -1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.WarehouseId, context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.ProcessedQuantity, -1, cancellationToken);
            }
        }
    }
}
