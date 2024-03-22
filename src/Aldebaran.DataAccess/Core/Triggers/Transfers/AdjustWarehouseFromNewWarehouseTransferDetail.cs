using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AdjustWarehouseFromNewWarehouseTransferDetail : InventoryManagementBase, IBeforeSaveTrigger<WarehouseTransferDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustWarehouseFromNewWarehouseTransferDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<WarehouseTransferDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {                
                var warehouseTransfer = _context.WarehouseTransfers.Local.FirstOrDefault();

                await UpdateWarehouseReferenceQuantityAsync(warehouseTransfer!.OriginWarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, -1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(warehouseTransfer!.DestinationWarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, 1, cancellationToken);
            }
        }
    }
}
