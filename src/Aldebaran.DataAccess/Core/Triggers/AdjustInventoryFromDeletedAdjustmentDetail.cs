using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers
{
    public class AdjustInventoryFromDeletedAdjustmentDetail : IBeforeSaveTrigger<AdjustmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromDeletedAdjustmentDetail(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<AdjustmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                if (!_context.ChangeAdjustmentType)
                {

                    var adjustmentType = await _context.AdjustmentTypes.FindAsync(new object[] { context.Entity.Adjustment.AdjustmentTypeId }, cancellationToken) ?? throw new ArgumentNullException($"Tipo de ajuste con id {context.Entity.Adjustment.AdjustmentTypeId} no encontrado");

                    var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { context.Entity.ReferenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {context.Entity.ReferenceId} no encontrada");

                    var warehouseReferenceEntity = await _context.ReferencesWarehouses.FindAsync(new object[] { context.Entity.ReferenceId, context.Entity.WarehouseId }, cancellationToken) ?? throw new ArgumentNullException($"Bodega con id {context.Entity.WarehouseId} y referencia {context.Entity.ReferenceId} no encontrada");

                    inventoryEntity.InventoryQuantity += (context.Entity.Quantity * adjustmentType.Operator * -1);

                    warehouseReferenceEntity.Quantity += (context.Entity.Quantity * adjustmentType.Operator * -1);
                }
            }
        }
    }
}
