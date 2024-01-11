using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AdjustInventoryFromWarehouseTransferCancelled : InventoryManagementBase, IBeforeSaveTrigger<WarehouseTransfer>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromWarehouseTransferCancelled(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<WarehouseTransfer> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Modified)
                return;

            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 2)
                return;

            var detailChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

            if ((short)(detailChanges.oldValue ?? 0) == (short)(detailChanges.newValue ?? 0))
                return;

            foreach (var item in context.Entity.WarehouseTransferDetails)
            {
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.OrigenWarehouseId, item.ReferenceId, item.Quantity, 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(context.Entity.DestinationWarehouseId, item.ReferenceId, item.Quantity, -1, cancellationToken);
            }
        }
    }
}
