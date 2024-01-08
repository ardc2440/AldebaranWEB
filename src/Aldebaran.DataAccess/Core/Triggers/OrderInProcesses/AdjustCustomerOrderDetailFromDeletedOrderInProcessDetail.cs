using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustCustomerOrderDetailFromDeletedOrderInProcessDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderInProcessDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustCustomerOrderDetailFromDeletedOrderInProcessDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderInProcessDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
                await UpdateProcessedQuantityAsync(context.Entity.CustomerOrderDetailId, context.Entity.ProcessedQuantity, -1, cancellationToken);
        }
    }
}
