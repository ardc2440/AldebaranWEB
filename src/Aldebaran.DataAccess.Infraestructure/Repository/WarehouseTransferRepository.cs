using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseTransferRepository : RepositoryBase<AldebaranDbContext>, IWarehouseTransferRepository
    {
        public WarehouseTransferRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task CancelAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.WarehouseTransfers.FirstOrDefaultAsync(x => x.WarehouseTransferId == warehouseTransferId, ct) ?? throw new KeyNotFoundException($"Traslado con id {warehouseTransferId} no existe.");
                var documentType = await dbContext.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "B", ct);
                var statutsDocumentType = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 2, ct);
                entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<WarehouseTransfer?> FindAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.WarehouseTransfers.AsNoTracking()
                   .Include(i => i.OriginWarehouse)
                   .Include(i => i.DestinationWarehouse)
                   .Include(i => i.Employee)
                   .Include(i => i.StatusDocumentType)
                   .FirstOrDefaultAsync(i => i.WarehouseTransferId == warehouseTransferId, ct);
            }, ct);
        }

        public async Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.WarehouseTransfers.AsNoTracking()
                    .Include(i => i.OriginWarehouse)
                    .Include(i => i.DestinationWarehouse)
                    .Include(i => i.Employee)
                    .Include(i => i.StatusDocumentType)
                    .OrderByDescending(o => o.WarehouseTransferId);

                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, string search, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.WarehouseTransfers.AsNoTracking()
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
                            .OrderByDescending(o => o.WarehouseTransferId);

                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<WarehouseTransfer?> AddAsync(WarehouseTransfer warehouseTransfer, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var localWarehouse = dbContext.Warehouses.AsNoTracking().FirstOrDefault(f => f.WarehouseCode == 1);

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
                    await dbContext.WarehouseTransfers.AddAsync(entity, ct);
                    await dbContext.SaveChangesAsync(ct);

                    if (entity.DestinationWarehouseId == localWarehouse?.WarehouseId)
                        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.SP_AUTOMATIC_CUSTOMER_ORDER_IN_PROCESS_GENERATION @DocumentType = 'B', @DocumentId = {entity.WarehouseTransferId}", ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }

                return entity;
            }, ct);
        }
    }
}
