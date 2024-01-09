using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Reservations
{
    public class AdjustmentInventoryFromNewOrderDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustmentInventoryFromNewOrderDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
                await UpdateOrderedQuantity(context.Entity.ReferenceId, context.Entity.RequestedQuantity, 1, cancellationToken);
        }
    }
}
