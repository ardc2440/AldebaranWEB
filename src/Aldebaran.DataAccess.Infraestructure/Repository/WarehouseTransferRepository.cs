using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseTransferRepository : IWarehouseTransferRepository
    {
        private readonly AldebaranDbContext _context;
        public WarehouseTransferRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CancelAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            var entity = await _context.WarehouseTransfers.FirstOrDefaultAsync(x => x.WarehouseTransferId == warehouseTransferId, ct) ?? throw new KeyNotFoundException($"Traslado con id {warehouseTransferId} no existe.");
            var documentType = await _context.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "B", ct);
            var statutsDocumentType = await _context.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 2, ct);
            entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<WarehouseTransfer?> FindAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            return await _context.WarehouseTransfers.AsNoTracking()
                .Include(i => i.OriginWarehouse)
                .Include(i => i.DestinationWarehouse)
                .Include(i => i.Employee)
                .Include(i => i.StatusDocumentType)
                .FirstOrDefaultAsync(i => i.WarehouseTransferId == warehouseTransferId, ct);
        }

        public async Task<IEnumerable<WarehouseTransfer>> GetAsync(CancellationToken ct = default)
        {
            return await _context.WarehouseTransfers.AsNoTracking()
                .Include(i => i.OriginWarehouse)
                .Include(i => i.DestinationWarehouse)
                .Include(i => i.Employee)
                .Include(i => i.StatusDocumentType)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<WarehouseTransfer>> GetAsync(string search, CancellationToken ct = default)
        {
            return await _context.WarehouseTransfers.AsNoTracking()
                .Include(i => i.OriginWarehouse)
                .Include(i => i.DestinationWarehouse)
                .Include(i => i.Employee)
                .Include(i => i.StatusDocumentType)
                .Where(i => i.Notes.Contains(search) ||
                          i.TransferDate.ToString().Contains(search) ||
                          i.Nationalization.Contains(search) ||
                          i.StatusDocumentType.StatusDocumentTypeName.Contains(search) ||
                          i.DestinationWarehouse.WarehouseName.Contains(search) ||
                          i.OriginWarehouse.WarehouseName.Contains(search) ||
                          i.Employee.FullName.Contains(search))
                .ToListAsync(ct);
        }

        public async Task<WarehouseTransfer?> AddAsync(WarehouseTransfer warehouseTransfer, CancellationToken ct = default)
        {
            var entity = new WarehouseTransfer
            {
                WarehouseTransferDetails = new List<WarehouseTransferDetail>(),
                CreationDate = DateTime.Now,
                TransferDate = warehouseTransfer.TransferDate,
                DestinationWarehouseId = warehouseTransfer.DestinationWarehouseId,
                OriginWarehouseId = warehouseTransfer.OriginWarehouseId,
                StatusDocumentTypeId = warehouseTransfer.StatusDocumentTypeId,
                EmployeeId = warehouseTransfer.EmployeeId,
                Nationalization = warehouseTransfer.Nationalization,
                Notes = warehouseTransfer.Notes
            };

            foreach (var item in warehouseTransfer.WarehouseTransferDetails)
                entity.WarehouseTransferDetails.Add(new WarehouseTransferDetail
                {
                    ReferenceId = item.ReferenceId,
                    Quantity = item.Quantity
                });

            try
            {
                await _context.WarehouseTransfers.AddAsync(entity, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }

            return entity;
        }
    }

}
