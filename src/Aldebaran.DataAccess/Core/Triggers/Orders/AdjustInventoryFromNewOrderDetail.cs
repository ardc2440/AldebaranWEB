using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Orders
{
    public class AdjustInventoryFromNewOrderDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromNewOrderDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
                await UpdateOrderedQuantityAsync(context.Entity.ReferenceId, context.Entity.RequestedQuantity, 1, cancellationToken);
        }
    }
}
