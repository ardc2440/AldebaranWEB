using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Reservations
{
    public class AdjustmentInventoryFromNewReservationDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerReservationDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustmentInventoryFromNewReservationDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerReservationDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
                await UpdateReservedQuantity(context.Entity.ReferenceId, context.Entity.ReservedQuantity, 1, cancellationToken);
        }
    }
}
