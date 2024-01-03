using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers
{
    public class AdjustInventoryFromAdjustmentCancelled : IBeforeSaveTrigger<Adjustment>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromAdjustmentCancelled(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<Adjustment> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified)
            {
                var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

                if (statusOrder == 2)
                {
                    var detailChanges = context.Entity.GetType()
                     .GetProperties()
                     .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
                     .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

                    if ((short)(detailChanges.oldValue ?? 0) != (short)(detailChanges.newValue ?? 0))
                    {
                        var indicatorInOut = (await _context.AdjustmentTypes.FindAsync(new object[] { context.Entity.AdjustmentTypeId }, cancellationToken))!.Operator * -1;

                        foreach (var item in context.Entity.AdjustmentDetails)
                        {
                            await UpdateInventoryValue(item.ReferenceId, item.Quantity, indicatorInOut, cancellationToken);
                            await UpdateWarehouseReferenceValue(item.WarehouseId, item.ReferenceId, item.Quantity, indicatorInOut, cancellationToken);
                        }
                    }
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
