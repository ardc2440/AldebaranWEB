using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Shipments
{
    public class AdjustCustomerOrderDetailFromModifiedOrderShipmentDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderShipmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustCustomerOrderDetailFromModifiedOrderShipmentDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderShipmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var detailChanges = context.Entity.GetType()
                    .GetProperties()
                    .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                    .Where(x => x.newValue != x.oldValue);

                var customerOrderDetail = detailChanges.FirstOrDefault(x => x.name.Equals("CustomerOrderDetailId"));
                var quantity = detailChanges.FirstOrDefault(x => x.name.Equals("DeliveredQuantity"));

                await UpdateProcessedQuantityAsync((int)(customerOrderDetail.oldValue ?? 0), (int)(quantity.oldValue ?? 0), 1, cancellationToken);
                await UpdateProcessedQuantityAsync((int)(customerOrderDetail.newValue ?? 0), (int)(quantity.newValue ?? 0), -1, cancellationToken);

                await UpdateDeliveredQuantityAsync((int)(customerOrderDetail.oldValue ?? 0), (int)(quantity.oldValue ?? 0), -1, cancellationToken);
                await UpdateDeliveredQuantityAsync((int)(customerOrderDetail.newValue ?? 0), (int)(quantity.newValue ?? 0), 1, cancellationToken);
            }
        }
    }
}
