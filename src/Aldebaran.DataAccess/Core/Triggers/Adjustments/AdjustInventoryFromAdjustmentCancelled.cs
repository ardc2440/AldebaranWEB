using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.Adjustments
{
    public class AdjustInventoryFromAdjustmentCancelled : InventoryManagementBase, IBeforeSaveTrigger<Adjustment>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromAdjustmentCancelled(AldebaranDbContext context) : base(context)
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
                            await UpdateInventoryQuantity(item.ReferenceId, item.Quantity, indicatorInOut, cancellationToken);
                            await UpdateWarehouseReferenceQuantity(item.WarehouseId, item.ReferenceId, item.Quantity, indicatorInOut, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
