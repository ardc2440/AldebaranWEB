namespace Aldebaran.DataAccess.Core.Triggers
{
    public class InventoryManagementBase
    {
        private readonly AldebaranDbContext _context;

        public InventoryManagementBase(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task UpdateInventoryQuantity(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.InventoryQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateReservedQuantity(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.ReservedQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateWorkInProcessQuantity(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.WorkInProcessQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateOrderedQuantity(int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var inventoryEntity = await _context.ItemReferences.FindAsync(new object[] { referenceId }, cancellationToken) ?? throw new ArgumentNullException($"Referencia con id {referenceId} no encontrada");
            inventoryEntity.OrderedQuantity += (quantity * operatorInOut);
        }

        public async Task UpdateWarehouseReferenceQuantity(short warehouseId, int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var warehouseReferenceEntity = await _context.ReferencesWarehouses.FindAsync(new object[] { referenceId, warehouseId }, cancellationToken) ?? throw new ArgumentNullException($"Bodega con id {warehouseId} y referencia {referenceId} no encontrada");
            warehouseReferenceEntity.Quantity += (quantity * operatorInOut);
        }
    }
}
