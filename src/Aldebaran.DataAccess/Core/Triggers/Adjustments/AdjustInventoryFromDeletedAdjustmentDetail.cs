using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Extensions;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Adjustments
{
    public class AdjustInventoryFromDeletedAdjustmentDetail : InventoryManagementBase, IBeforeSaveTrigger<AdjustmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromDeletedAdjustmentDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<AdjustmentDetail> context, CancellationToken cancellationToken)
        {
            if (_context.Events.Get("ChangeWarehousesTransfer", true) == true)
                return;

            if (context.ChangeType != ChangeType.Deleted)
                return;

            var adjustmentType = await _context.AdjustmentTypes.FindAsync(new object[] { context.Entity.Adjustment.AdjustmentTypeId }, cancellationToken) ?? throw new ArgumentNullException($"Tipo de ajuste con id {context.Entity.Adjustment.AdjustmentTypeId} no encontrado");

            await UpdateInventoryQuantityAsync(context.Entity.ReferenceId, context.Entity.Quantity, (adjustmentType.Operator * -1), cancellationToken);
            await UpdateWarehouseReferenceQuantityAsync(context.Entity.WarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, (adjustmentType.Operator * -1), cancellationToken);
        }
    }
}
