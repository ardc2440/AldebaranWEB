using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

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
                var customerDetail = await _context.CustomerOrderDetails.FirstOrDefaultAsync(i => i.CustomerOrderDetailId == context.Entity.CustomerOrderDetailId, cancellationToken);

                await UpdateWorkInProcessQuantityAsync(customerDetail!.ReferenceId, context.Entity.ProcessedQuantity, -1, cancellationToken);
                await UpdateInventoryQuantityAsync(customerDetail!.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
                await UpdateOrderedQuantityAsync(customerDetail!.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.WarehouseId, customerDetail!.ReferenceId, context.Entity.ProcessedQuantity, 1, cancellationToken);
            }
        }
    }
}
