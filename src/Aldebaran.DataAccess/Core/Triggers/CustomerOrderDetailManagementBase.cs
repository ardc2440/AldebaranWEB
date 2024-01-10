namespace Aldebaran.DataAccess.Core.Triggers
{
    public class CustomerOrderDetailManagementBase
    {
        private readonly AldebaranDbContext _context;

        public CustomerOrderDetailManagementBase(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task UpdateProcessedQuantityAsync(int customerOrderdetailId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var customerorderDetailEntity = await _context.CustomerOrderDetails.FindAsync(new object[] { customerOrderdetailId }, cancellationToken) ?? throw new ArgumentNullException($"Detalle de Pedido con id {customerOrderdetailId} no encontrado");
            customerorderDetailEntity.ProcessedQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateDeliveredQuantityAsync(int customerOrderdetailId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var customerorderDetailEntity = await _context.CustomerOrderDetails.FindAsync(new object[] { customerOrderdetailId }, cancellationToken) ?? throw new ArgumentNullException($"Detalle de Pedido con id {customerOrderdetailId} no encontrado");
            customerorderDetailEntity.DeliveredQuantity += (quantity * operatorInOut);
        }
    }
}
