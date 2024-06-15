namespace Aldebaran.DataAccess.Core.Triggers
{
    public class InventoryManagementBase : CustomerOrderDetailManagementBase
    {
        private readonly AldebaranDbContext _context;

        public InventoryManagementBase(AldebaranDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task UpdateInventoryQuantityAsync(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.InventoryQuantity += (quantity * operatorInOut);
            inventoryEntity.IsSoldOut = inventoryEntity.InventoryQuantity <= 0;
        }

        public async Task UpdateReservedQuantityAsync(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.ReservedQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateWorkInProcessQuantityAsync(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.WorkInProcessQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateOrderedQuantityAsync(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.OrderedQuantity += (quantity * operatorInOut);
        }
    }
}
