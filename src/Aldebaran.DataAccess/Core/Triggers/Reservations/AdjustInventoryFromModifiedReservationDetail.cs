using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Reservations
{
    public class AdjustInventoryFromModifiedReservationDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerReservationDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromModifiedReservationDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerReservationDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var detailChanges = context.Entity.GetType()
                    .GetProperties()
                    .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                    .Where(x => x.newValue != x.oldValue);

                var reference = detailChanges.FirstOrDefault(x => x.name.Equals("ReferenceId"));
                var quantity = detailChanges.FirstOrDefault(x => x.name.Equals("ReservedQuantity"));

                await UpdateReservedQuantity((int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), -1, cancellationToken);
                await UpdateReservedQuantity((int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), 1, cancellationToken);
            }
        }
    }
}
