using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Reservations
{
    public class AdjustmentInventoryFromReservationToOrder : InventoryManagementBase, IBeforeSaveTrigger<CustomerReservation>
    {
        private readonly AldebaranDbContext _context;

        public AdjustmentInventoryFromReservationToOrder(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerReservation> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

                if (statusOrder == 2)
                {
                    var detailChanges = context.Entity.GetType()
                     .GetProperties()
                     .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                     .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

                    if ((short)(detailChanges.oldValue ?? 0) != (short)(detailChanges.newValue ?? 0))
                    {
                        var indicatorInOut = -1;

                        foreach (var item in context.Entity.CustomerReservationDetails)
                            await UpdateReservedQuantity(item.ReferenceId, item.ReservedQuantity, indicatorInOut, cancellationToken);
                    }
                }
            }
        }
    }
}
