using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers
{
    public class AdjustInventoryFromModifiedAdjustmentDetail : IBeforeSaveTrigger<AdjustmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromModifiedAdjustmentDetail(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<AdjustmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                if (!_context.ChangeAdjustmentType)
                {
                    var indicatorInOut = (await _context.AdjustmentTypes.FindAsync(new object[] { context.Entity.Adjustment.AdjustmentTypeId }, cancellationToken) ?? throw new ArgumentNullException($"Tipo de ajuste con id {context.Entity.Adjustment.AdjustmentTypeId} no encontrado")).Operator;

                    var detailChanges = context.Entity.GetType()
                        .GetProperties()
                        .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                        .Where(x => x.newValue != x.oldValue);

                    var warehouse = detailChanges.FirstOrDefault(x => x.name.Equals("WarehouseId"));
                    var reference = detailChanges.FirstOrDefault(x => x.name.Equals("ReferenceId"));
                    var quantity = detailChanges.FirstOrDefault(x => x.name.Equals("Quantity"));

                    await UpdateInventoryValue((int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), indicatorInOut * -1, cancellationToken);
                    await UpdateWarehouseReferenceValue((short)(warehouse.oldValue ?? 0), (int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), indicatorInOut * -1, cancellationToken);

                    await UpdateInventoryValue((int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), indicatorInOut, cancellationToken);
                    await UpdateWarehouseReferenceValue((short)(warehouse.newValue ?? 0), (int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), indicatorInOut, cancellationToken);
                }
            }
        }

        internal async Task UpdateInventoryValue(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.InventoryQuantity = inventoryEntity.InventoryQuantity + (quantity * operatorInOut);
        }

        internal async Task UpdateWarehouseReferenceValue(short warehouseId, int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var warehouseReferenceEntity = await _context.ReferencesWarehouses.FindAsync(new object[] { referenceId, warehouseId }, cancellationToken) ?? throw new ArgumentNullException($"Bodega con id {warehouseId} y referencia {referenceId} no encontrada");
            warehouseReferenceEntity.Quantity = warehouseReferenceEntity.Quantity + (quantity * operatorInOut);
        }
    }
}
