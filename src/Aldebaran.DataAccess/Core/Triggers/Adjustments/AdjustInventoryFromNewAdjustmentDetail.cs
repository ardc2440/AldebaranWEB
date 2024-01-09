using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Adjustments
{
    public class AdjustInventoryFromNewAdjustmentDetail : InventoryManagementBase, IBeforeSaveTrigger<AdjustmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromNewAdjustmentDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<AdjustmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                var adjustmentType = await _context.AdjustmentTypes.FindAsync(new object[] { context.Entity.Adjustment.AdjustmentTypeId }, cancellationToken) ?? throw new ArgumentNullException($"Tipo de ajuste con id {context.Entity.Adjustment.AdjustmentTypeId} no encontrado");

                await UpdateInventoryQuantity(context.Entity.ReferenceId, context.Entity.Quantity, adjustmentType.Operator, cancellationToken);
                await UpdateWarehouseReferenceQuantity(context.Entity.WarehouseId, context.Entity.ReferenceId, context.Entity.Quantity, adjustmentType.Operator, cancellationToken);
            }
        }
    }
}
