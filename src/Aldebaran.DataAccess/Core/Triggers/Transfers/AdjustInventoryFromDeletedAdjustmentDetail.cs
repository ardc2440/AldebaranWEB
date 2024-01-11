using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Extensions;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AdjustInventoryFromDeletedWarehouseTransferDetail : InventoryManagementBase, IBeforeSaveTrigger<WarehouseTransferDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromDeletedWarehouseTransferDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<WarehouseTransferDetail> context, CancellationToken cancellationToken)
        {
            if (_context.Events.Get("ChangeWarehousesTransfer", true) == true)
                return;

            if (context.ChangeType != ChangeType.Deleted)
                return;

            var warehouseTransfer = await _context.WarehouseTransfers.FirstOrDefaultAsync(i => i.WarehouseTransferId == context.Entity.WarehouseTransferId, cancellationToken);

            await UpdateWarehouseReferenceQuantityAsync(warehouseTransfer!.OrigenWarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, 1, cancellationToken);
            await UpdateWarehouseReferenceQuantityAsync(warehouseTransfer!.DestinationWarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, -1, cancellationToken);
        }
    }
}
