using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aldebaran.DataAccess.Core.Triggers.Adjustments
{
    public class AdjustInventoryFromAdjustmentTypeChanged : IBeforeSaveTrigger<Adjustment>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromAdjustmentTypeChanged(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<Adjustment> context, CancellationToken cancellationToken)
        {
            _context.ChangeAdjustmentType = false;

            if (context.ChangeType == ChangeType.Modified)
            {
                var detailChanges = context.Entity.GetType()
                    .GetProperties()
                    .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                    .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("AdjustmentTypeId"));

                if ((short)(detailChanges.oldValue ?? 0) != (short)(detailChanges.newValue ?? 0))
                {

                    _context.ChangeAdjustmentType = true;

                    var detail = context.Entity.AdjustmentDetails;

                    var adj = await _context.AdjustmentTypes.FindAsync(new object[] { detailChanges.oldValue! }, cancellationToken);

                    var oldIndicatorInOut = adj!.Operator;

                    adj = await _context.AdjustmentTypes.FindAsync(new object[] { detailChanges.newValue! }, cancellationToken);

                    var newIndicatorInOut = adj!.Operator;

                    foreach (var kvp in detail)
                    {
                        var entry = _context.Entry(kvp);

                        if (entry == null)
                            continue;

                        if (entry.State == EntityState.Added)
                            continue;

                        PropertyEntry<AdjustmentDetail, short> warehouse = _context.Entry(kvp).Property(e => e.WarehouseId);
                        PropertyEntry<AdjustmentDetail, int> reference = _context.Entry(kvp).Property(e => e.ReferenceId);
                        PropertyEntry<AdjustmentDetail, int> quantity = _context.Entry(kvp).Property(e => e.Quantity);

                        await UpdateInventoryValue(reference.OriginalValue, quantity.OriginalValue, oldIndicatorInOut * -1, cancellationToken);
                        await UpdateWarehouseReferenceValue(warehouse.OriginalValue, reference.OriginalValue, quantity.OriginalValue, oldIndicatorInOut * -1, cancellationToken);

                        if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
                        {
                            await UpdateInventoryValue(reference.CurrentValue, quantity.CurrentValue, newIndicatorInOut, cancellationToken);
                            await UpdateWarehouseReferenceValue(warehouse.CurrentValue, reference.CurrentValue, quantity.CurrentValue, newIndicatorInOut, cancellationToken);
                        }
                    }
                }
            }
        }

        internal async Task UpdateInventoryValue(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.InventoryQuantity = inventoryEntity.InventoryQuantity + quantity * operatorInOut;
        }

        internal async Task UpdateWarehouseReferenceValue(short warehouseId, int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var warehouseReferenceEntity = await _context.ReferencesWarehouses.FindAsync(new object[] { referenceId, warehouseId }, cancellationToken) ?? throw new ArgumentNullException($"Bodega con id {warehouseId} y referencia {referenceId} no encontrada");
            warehouseReferenceEntity.Quantity = warehouseReferenceEntity.Quantity + quantity * operatorInOut;
        }
    }
}
