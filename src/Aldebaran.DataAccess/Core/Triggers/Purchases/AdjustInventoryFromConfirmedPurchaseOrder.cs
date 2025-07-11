﻿using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Core.Triggers.Purchases
{
    public class AdjustInventoryFromConfirmedPurchaseOrder : WarehouseAlarmManagementBase, IBeforeSaveTrigger<PurchaseOrder>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromConfirmedPurchaseOrder(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<PurchaseOrder> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType != ChangeType.Modified)
                return;

            var statusOrder = (await _context.StatusDocumentTypes.FindAsync(new object[] { context.Entity.StatusDocumentTypeId }, cancellationToken))!.StatusOrder;

            if (statusOrder != 2)
                return;

            var detailChanges = context.Entity.GetType()
             .GetProperties()
             .Select(property => (name: property.Name, oldValue: property.GetValue(context.UnmodifiedEntity), newValue: property.GetValue(context.Entity)))
             .FirstOrDefault(x => x.newValue != x.oldValue && x.name.Equals("StatusDocumentTypeId"));

            if ((short)(detailChanges.oldValue ?? 0) == (short)(detailChanges.newValue ?? 0))
                return;

            foreach (var item in context.Entity.PurchaseOrderDetails)
            {
                await UpdateInventoryQuantityAsync(item.ReferenceId, item.ReceivedQuantity ?? 0, 1, cancellationToken);
                await UpdateWarehouseReferenceQuantityAsync(item.WarehouseId, item.ReferenceId, item.ReceivedQuantity ?? 0, 1, cancellationToken);                
            }

            /* Obtener la bodega local */
            var localWarehouse = _context.Warehouses.AsNoTracking().FirstOrDefault(f => f.WarehouseCode == 1);

            var purchaseOrderDetails = context.Entity.PurchaseOrderDetails.Where(a => a.WarehouseId == localWarehouse!.WarehouseId && a.ReceivedQuantity > 0);
            
            /* Validar si en la orden hay algun detalle cuyo destino sea la bodega local y la cantidad recibida sea > 0 */
            if (purchaseOrderDetails.Any())
            {   
                await AddWarehouseAlarmAsync("O", context.Entity.PurchaseOrderId, purchaseOrderDetails, detail => detail.ReferenceId, cancellationToken);
            }
        }
    }
}
