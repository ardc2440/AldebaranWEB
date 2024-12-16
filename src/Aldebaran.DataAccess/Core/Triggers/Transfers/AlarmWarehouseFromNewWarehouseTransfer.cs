using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.Transfers
{
    public class AlarmWarehouseFromNewWarehouseTransfer : WarehouseAlarmManagementBase, IAfterSaveTrigger<WarehouseTransfer>
    {
        private readonly AldebaranDbContext _context;

        public AlarmWarehouseFromNewWarehouseTransfer(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AfterSave(ITriggerContext<WarehouseTransfer> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Added)
                return;

            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 1)
                return;

            /* Obtener la bodega local */
            var localWarehouse = _context.Warehouses.AsNoTracking().FirstOrDefault(f => f.WarehouseCode == 1);

            if (context.Entity.DestinationWarehouseId != localWarehouse!.WarehouseId)
                return;

            /* Validar si en la orden hay algun detalle cuyo destino sea la bodega local y la cantidad recibida sea > 0 */
            await AddWarehouseAlarmAsync("B", context.Entity.WarehouseTransferId, context.Entity.WarehouseTransferDetails, detail => detail.ReferenceId, cancellationToken);
        }
    }
}
