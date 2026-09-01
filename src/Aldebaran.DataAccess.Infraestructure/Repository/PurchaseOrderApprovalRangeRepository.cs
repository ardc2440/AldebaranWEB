using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderApprovalRangeRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderApprovalRangeRepository
    {
        public PurchaseOrderApprovalRangeRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task AddAsync(PurchaseOrderApprovalRange purchaseOrderApprovalRange, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
                {
                    try
                    {
                        await dbContext.PurchaseOrderApprovalRanges
                            .AddAsync(purchaseOrderApprovalRange, ct);

                        await dbContext.SaveChangesAsync(ct);
                    }
                    catch
                    {
                        dbContext.Entry(purchaseOrderApprovalRange)
                            .State = EntityState.Unchanged;
                        throw;
                    }
                }, ct);
        }

        public async Task<bool> ExistsOverlapAsync(int requestedQuantityFrom, int requestedQuantityTo, int? purchaseOrderApprovalRangeId = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                {
                    var query = dbContext.PurchaseOrderApprovalRanges
                        .AsNoTracking()
                        .Where(x => x.IsActive);

                    if (purchaseOrderApprovalRangeId.HasValue)
                        query = query.Where(x => x.PurchaseOrderApprovalRangeId != purchaseOrderApprovalRangeId.Value);

                    return await query.AnyAsync(x => requestedQuantityFrom <= x.RequestedQuantityTo &&
                                                     requestedQuantityTo >= x.RequestedQuantityFrom, ct);
                }, ct);
        }

        public async Task<PurchaseOrderApprovalRange?> FindAsync(int purchaseOrderApprovalRangeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                {
                    return await dbContext.PurchaseOrderApprovalRanges
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.PurchaseOrderApprovalRangeId == purchaseOrderApprovalRangeId, ct);
                }, ct);
        }

        public async Task<IEnumerable<PurchaseOrderApprovalRange>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                {
                    return await dbContext.PurchaseOrderApprovalRanges
                        .AsNoTracking()
                        .OrderBy(x => x.RequestedQuantityFrom)
                        .ToListAsync(ct);
                }, ct);
        }

        public async Task<PurchaseOrderApprovalRange?> GetByRequestedQuantityAsync(int requestedQuantity, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                {
                    return await dbContext.PurchaseOrderApprovalRanges
                        .AsNoTracking()
                        .Where(x => x.IsActive)
                        .FirstOrDefaultAsync(x => requestedQuantity >= x.RequestedQuantityFrom &&
                                                  requestedQuantity <= x.RequestedQuantityTo, ct);
                }, ct);
        }

        public async Task UpdateAsync(int purchaseOrderApprovalRangeId, PurchaseOrderApprovalRange purchaseOrderApprovalRange, string changeReason, int employeeId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
                {
                    var entity = await dbContext.PurchaseOrderApprovalRanges
                                    .FirstOrDefaultAsync(x => x.PurchaseOrderApprovalRangeId == purchaseOrderApprovalRangeId, ct) ?? throw new KeyNotFoundException($"Rango de aprobación con id {purchaseOrderApprovalRangeId} no existe.");

                    await AddLogAsync(dbContext, entity, changeReason, employeeId, ct);

                    entity.RequestedQuantityFrom = purchaseOrderApprovalRange.RequestedQuantityFrom;
                    entity.RequestedQuantityTo = purchaseOrderApprovalRange.RequestedQuantityTo;
                    entity.AllowedDifferencePercent = purchaseOrderApprovalRange.AllowedDifferencePercent;
                    entity.IsActive = purchaseOrderApprovalRange.IsActive;

                    try
                    {
                        await dbContext.SaveChangesAsync(ct);
                    }
                    catch
                    {
                        dbContext.Entry(entity)
                            .State = EntityState.Unchanged;

                        throw;
                    }
                }, ct);
        }

        private static async Task AddLogAsync(AldebaranDbContext dbContext, PurchaseOrderApprovalRange entity, string changeReason, int employeeId, CancellationToken ct)
        {
            var log = new PurchaseOrderApprovalRangeLog
            {
                PurchaseOrderApprovalRangeId = entity.PurchaseOrderApprovalRangeId,
                PreviousRequestedQuantityFrom = entity.RequestedQuantityFrom,
                PreviousRequestedQuantityTo = entity.RequestedQuantityTo,
                PreviousAllowedDifferencePercent = entity.AllowedDifferencePercent,
                PreviousIsActive = entity.IsActive,
                ChangeReason = changeReason,
                ChangedByEmployeeId = employeeId
            };

            await dbContext.PurchaseOrderApprovalRangeLogs.AddAsync(log, ct);
        }
    }
}
