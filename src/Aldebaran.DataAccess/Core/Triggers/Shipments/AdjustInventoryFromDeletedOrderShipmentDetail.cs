using Aldebaran.DataAccess.Entities;
using EntityFrameworkCore.Triggered;

namespace Aldebaran.DataAccess.Core.Triggers.OrderInProcesses
{
    public class AdjustInventoryFromDeletedOrderShipmentDetail : InventoryManagementBase, IBeforeSaveTrigger<CustomerOrderShipmentDetail>
    {
        private readonly AldebaranDbContext _context;

        public AdjustInventoryFromDeletedOrderShipmentDetail(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeforeSave(ITriggerContext<CustomerOrderShipmentDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                /* La navegación CustomerOrderDetail no viene cargada al eliminar un detalle desde la modificación del despacho:
                   se obtiene el detalle del pedido por su llave (igual que el trigger de modificación) */
                var customerOrderDetail = await _context.CustomerOrderDetails.FindAsync(new object[] { context.Entity.CustomerOrderDetailId }, cancellationToken)
                    ?? throw new KeyNotFoundException($"Detalle de pedido con id {context.Entity.CustomerOrderDetailId} no existe.");

                await UpdateWorkInProcessQuantityAsync(customerOrderDetail.ReferenceId, context.Entity.DeliveredQuantity, 1, cancellationToken);
            }
        }
    }
}
