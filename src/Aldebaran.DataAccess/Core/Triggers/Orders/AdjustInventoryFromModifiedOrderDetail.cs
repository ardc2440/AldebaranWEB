using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Reservations
{
    public class AdjustInventoryFromModifiedOrderDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromModifiedOrderDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var detailChanges = context.Entity.GetType()
                    .GetProperties()
                    .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                    .Where(x => x.newValue != x.oldValue);

                var reference = detailChanges.FirstOrDefault(x => x.name.Equals("ReferenceId"));
                var quantity = detailChanges.FirstOrDefault(x => x.name.Equals("RequestedQuantity"));

                await UpdateOrderedQuantityAsync((int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), -1, cancellationToken);
                await UpdateOrderedQuantityAsync((int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), 1, cancellationToken);
            }
        }
    }
}
