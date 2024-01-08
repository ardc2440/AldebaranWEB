using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aldebaran.DataAccess.Core.Triggers.Adjustments
{
    public class AdjustInventoryFromAdjustmentTypeChanged : InventoryManagementBase, IBeforeSaveTrigger<Adjustment>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromAdjustmentTypeChanged(AldebaranDbContext context) : base(context)
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
                    var oldIndicatorInOut = (await _context.AdjustmentTypes.FindAsync(new object[] { detailChanges.oldValue! }, cancellationToken))!.Operator;
                    var newIndicatorInOut = (await _context.AdjustmentTypes.FindAsync(new object[] { detailChanges.newValue! }, cancellationToken))!.Operator;

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

                        await UpdateInventoryQuantityAsync(reference.OriginalValue, quantity.OriginalValue, oldIndicatorInOut * -1, cancellationToken);
                        await UpdateWarehouseReferenceQuantityAsync(warehouse.OriginalValue, reference.OriginalValue, quantity.OriginalValue, oldIndicatorInOut * -1, cancellationToken);

                        if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
                        {
                            await UpdateInventoryQuantityAsync(reference.CurrentValue, quantity.CurrentValue, newIndicatorInOut, cancellationToken);
                            await UpdateWarehouseReferenceQuantityAsync(warehouse.CurrentValue, reference.CurrentValue, quantity.CurrentValue, newIndicatorInOut, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
