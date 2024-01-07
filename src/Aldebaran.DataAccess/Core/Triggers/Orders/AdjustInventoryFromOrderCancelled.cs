using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Reservations
{
    public class AdjustInventoryFromOrderCancelled : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrder>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromOrderCancelled(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrder> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

                if (statusOrder == 6)
                {
                    var detailChanges = context.Entity.GetType()
                     .GetProperties()
                     .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                     .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

                    if ((short)(detailChanges.oldValue ?? 0) != (short)(detailChanges.newValue ?? 0))
                    {
                        var indicatorInOut = -1;

                        foreach (var item in context.Entity.CustomerOrderDetails)
                            await UpdateOrderedQuantity(item.ReferenceId, item.RequestedQuantity, indicatorInOut, cancellationToken);
                    }
                }
            }
        }
    }
}
