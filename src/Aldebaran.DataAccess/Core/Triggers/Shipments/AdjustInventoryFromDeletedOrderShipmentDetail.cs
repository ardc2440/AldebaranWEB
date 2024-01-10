using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustInventoryFromDeletedOrderShipmentDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderShipmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromDeletedOrderShipmentDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderShipmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
                await UpdateWorkInProcessQuantityAsync(context.Entity.CustomerOrderDetail.ReferenceId, context.Entity.DeliveredQuantity, 1, cancellationToken);
        }
    }
}
