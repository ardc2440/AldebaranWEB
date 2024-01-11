using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Extensions;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AdjustWarehouseFromModifiedWarehouseTransferDetail : InventoryManagementBase, IBeforeSaveTrigger<WarehouseTransferDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustWarehouseFromModifiedWarehouseTransferDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<WarehouseTransferDetail> context, CancellationToken cancellationToken)
        {

            if (context.ChangeType != ChangeType.Modified)
                return;

            if (_context.Events.Get("ChangeWarehousesTransfer", true) == true)
                return;

            var detailChanges = context.Entity.GetType()
                .GetProperties()
                .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                .Where(x => x.newValue != x.oldValue);

            var warehouse = detailChanges.FirstOrDefault(x => x.name.Equals("WarehouseId"));
            var reference = detailChanges.FirstOrDefault(x => x.name.Equals("ReferenceId"));
            var quantity = detailChanges.FirstOrDefault(x => x.name.Equals("Quantity"));

            await UpdateWarehouseReferenceQuantityAsync((short)(warehouse.oldValue ?? 0), (int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), 1, cancellationToken);
            await UpdateWarehouseReferenceQuantityAsync((short)(warehouse.oldValue ?? 0), (int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), -1, cancellationToken);

            await UpdateWarehouseReferenceQuantityAsync((short)(warehouse.newValue ?? 0), (int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), -1, cancellationToken);
            await UpdateWarehouseReferenceQuantityAsync((short)(warehouse.newValue ?? 0), (int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), 1, cancellationToken);
        }
    }
}
