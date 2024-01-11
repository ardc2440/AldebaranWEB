namespace Aldebaran.DataAccess.Core.Triggers
{
    public class WarehouseReferenceManagementBase
    {
        private readonly AldebaranDbContext _context;

        public WarehouseReferenceManagementBase(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task UpdateWarehouseReferenceQuantityAsync(short warehouseId, int referenceId, int quantity, int operatorInOut, CancellationToken cancellationToken)
        {
            var warehouseReferenceEntity = await _context.ReferencesWarehouses.FindAsync(new object[] { referenceId, warehouseId }, cancellationToken) ?? throw new ArgumentNullException($"Bodega con id {warehouseId} y referencia {referenceId} no encontrada");
            warehouseReferenceEntity.Quantity += (quantity * operatorInOut);
        }
    }
}
