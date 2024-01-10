using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Shipments
{
    public class AdjustCustomerOrderDetailFromDeletedOrderShipmentDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderShipmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustCustomerOrderDetailFromDeletedOrderShipmentDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderShipmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                await UpdateProcessedQuantityAsync(context.Entity.CustomerOrderDetailId, context.Entity.DeliveredQuantity, 1, cancellationToken);
                await UpdateDeliveredQuantityAsync(context.Entity.CustomerOrderDetailId, context.Entity.DeliveredQuantity, -1, cancellationToken);
            }
        }
    }
}
