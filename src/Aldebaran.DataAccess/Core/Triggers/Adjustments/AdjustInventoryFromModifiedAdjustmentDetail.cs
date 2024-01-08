using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Adjustments
{
    public class AdjustInventoryFromModifiedAdjustmentDetail : InventoryManagementBase, IBeforeSaveTrigger<AdjustmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromModifiedAdjustmentDetail(AldebaranDbContext context) : base(context)
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

                    await UpdateInventoryQuantityAsync((int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), indicatorInOut * -1, cancellationToken);
                    await UpdateWarehouseReferenceQuantityAsync((short)(warehouse.oldValue ?? 0), (int)(reference.oldValue ?? 0), (int)(quantity.oldValue ?? 0), indicatorInOut * -1, cancellationToken);

                    await UpdateInventoryQuantityAsync((int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), indicatorInOut, cancellationToken);
                    await UpdateWarehouseReferenceQuantityAsync((short)(warehouse.newValue ?? 0), (int)(reference.newValue ?? 0), (int)(quantity.newValue ?? 0), indicatorInOut, cancellationToken);
                }
            }
        }
    }
}
