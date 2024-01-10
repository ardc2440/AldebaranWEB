using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Orders
{
    public class AdjustInventoryFromOrderClosed : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrder>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromOrderClosed(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrder> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Modified)
                return;

            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 5)
                return;

            var detailChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

            if ((short)(detailChanges.oldValue ?? 0) == (short)(detailChanges.newValue ?? 0))
                return;

            var indicatorInOut = -1;

            foreach (var item in context.Entity.CustomerOrderDetails)
            {
                var reversedQuantity = item.RequestedQuantity - item.ProcessedQuantity - item.DeliveredQuantity;

                await UpdateOrderedQuantityAsync(item.ReferenceId, reversedQuantity, indicatorInOut, cancellationToken);
            }
        }
    }
}
