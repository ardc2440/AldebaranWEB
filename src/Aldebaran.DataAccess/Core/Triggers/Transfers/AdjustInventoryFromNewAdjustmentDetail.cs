using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AdjustInventoryFromNewWarehouseTransferDetail : InventoryManagementBase, IBeforeSaveTrigger<WarehouseTransferDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromNewWarehouseTransferDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<WarehouseTransferDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                var warehouseTransfer = await _context.WarehouseTransfers.FirstOrDefaultAsync(i => i.WarehouseTransferId == context.Entity.WarehouseTransferId, cancellationToken);

                await UpdateWarehouseReferenceQuantityAsync(warehouseTransfer!.OrigenWarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, -1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(warehouseTransfer!.DestinationWarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, 1, cancellationToken);
            }
        }
    }
}
