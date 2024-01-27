using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Extensions;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AdjustWarehouseFromWarehousesModified : InventoryManagementBase, IBeforeSaveTrigger<WarehouseTransfer>
    {
        private readonly AldebaranDbContext _context;

        public AdjustWarehouseFromWarehousesModified(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<WarehouseTransfer> context, CancellationToken cancellationToken)
        {
            _context.Events.AddOrUpdate("ChangeWarehousesTransfer", false);

            if (context.ChangeType != ChangeType.Modified)
                return;

            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 1)
                return;

            var originChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("OriginWarehouseId"));

            var destinationChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("DestinationWarehouseId"));

            if (((short)(originChanges.oldValue ?? 0) == (short)(originChanges.newValue ?? 0)) || ((short)(destinationChanges.oldValue ?? 0) == (short)(destinationChanges.newValue ?? 0)))
                return;

            _context.Events.AddOrUpdate("ChangeWarehousesTransfer", false);

            foreach (var item in context.Entity.WarehouseTransferDetails)
            {
                await UpdateWarehouseReferenceQuantityAsync((short)(originChanges.oldValue ?? 0), item.ReferenceId, item.Quantity, 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync((short)(originChanges.newValue ?? 0), item.ReferenceId, item.Quantity, -1, cancellationToken);

                await UpdateWarehouseReferenceQuantityAsync((short)(destinationChanges.oldValue ?? 0), item.ReferenceId, item.Quantity, -1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync((short)(destinationChanges.newValue ?? 0), item.ReferenceId, item.Quantity, 1, cancellationToken);
            }
        }
    }
}
