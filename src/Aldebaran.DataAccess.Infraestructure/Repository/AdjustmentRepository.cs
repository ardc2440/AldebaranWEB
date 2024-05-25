using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentRepository : RepositoryBase<AldebaranDbContext>, IAdjustmentRepository
    {
        private readonly ISharedStringLocalizer _SharedLocalizer;
        public AdjustmentRepository(IServiceProvider serviceProvider, ISharedStringLocalizer sharedLocalizer) : base(serviceProvider)
        {
            _SharedLocalizer = sharedLocalizer ?? throw new ArgumentNullException(nameof(ISharedStringLocalizer));
        }

        public async Task<Adjustment> AddAsync(Adjustment adjustment, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = new Adjustment
                {
                    AdjustmentDate = adjustment.AdjustmentDate,
                    AdjustmentReasonId = adjustment.AdjustmentReasonId,
                    AdjustmentTypeId = adjustment.AdjustmentTypeId,
                    CreationDate = adjustment.CreationDate,
                    EmployeeId = adjustment.EmployeeId,
                    Notes = adjustment.Notes,
                    StatusDocumentTypeId = adjustment.StatusDocumentTypeId,
                    AdjustmentDetails = new List<AdjustmentDetail>()
                };

                foreach (var item in adjustment.AdjustmentDetails)
                    entity.AdjustmentDetails.Add(new AdjustmentDetail
                    {
                        Quantity = item.Quantity,
                        ReferenceId = item.ReferenceId,
                        WarehouseId = item.WarehouseId
                    });

                try
                {
                    await dbContext.Adjustments.AddAsync(entity, ct);
                    dbContext.Events.AddOrUpdate("ChangeAdjustmentType", false);
                    await dbContext.SaveChangesAsync(ct);
                    return entity;
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task CancelAsync(int adjustmentId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Adjustments.FirstOrDefaultAsync(x => x.AdjustmentId == adjustmentId, ct) ?? throw new KeyNotFoundException($"Ajuste con id {adjustmentId} no existe.");
                var documentType = await dbContext.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "A", ct);
                var statutsDocumentType = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 2, ct);
                entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;
                try
                {
                    dbContext.Events.AddOrUpdate("ChangeAdjustmentType", false);
                    await dbContext.SaveChangesAsync(ct);

                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .FirstOrDefaultAsync(w => w.AdjustmentId == adjustmentId, ct);
            }, ct);
        }

        public async Task<IEnumerable<Adjustment>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .OrderBy(o=>o.AdjustmentId)
                .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<Adjustment>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee.IdentityType)
                .Include(i => i.Employee.Area)
                .Where(i => i.StatusDocumentType.StatusDocumentTypeName.Contains(searchKey) ||
                          i.AdjustmentId.ToString().Contains(searchKey) ||
                          dbContext.Format(i.AdjustmentDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                          dbContext.Format(i.CreationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                          i.AdjustmentType.AdjustmentTypeName.Contains(searchKey) ||
                          i.AdjustmentReason.AdjustmentReasonName.Contains(searchKey) ||
                          i.Employee.FullName.Contains(searchKey) ||
                          i.AdjustmentReason.AdjustmentReasonNotes.Contains(searchKey) ||
                          i.Notes.Contains(searchKey))
                .OrderBy(o => o.AdjustmentId)
                .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Adjustments
                .Include(i => i.AdjustmentDetails)
                .FirstOrDefaultAsync(x => x.AdjustmentId == adjustmentId, ct) ?? throw new KeyNotFoundException($"Ajuste con id {adjustmentId} no existe.");

                entity.AdjustmentDate = adjustment.AdjustmentDate;
                entity.AdjustmentTypeId = adjustment.AdjustmentTypeId;
                entity.AdjustmentReasonId = adjustment.AdjustmentReasonId;
                entity.EmployeeId = adjustment.EmployeeId;
                entity.Notes = adjustment.Notes;
                entity.CreationDate = adjustment.CreationDate;
                entity.StatusDocumentTypeId = adjustment.StatusDocumentTypeId;

                foreach (var item in entity.AdjustmentDetails)
                {
                    if (!adjustment.AdjustmentDetails.Any(x => x.AdjustmentDetailId == item.AdjustmentDetailId))
                        dbContext.AdjustmentDetails.Remove(item);
                }

                foreach (var item in adjustment.AdjustmentDetails)
                {
                    if (item.AdjustmentDetailId > 0)
                    {
                        var entityDetail = await dbContext.AdjustmentDetails.FirstOrDefaultAsync(i => i.AdjustmentDetailId == item.AdjustmentDetailId, ct) ?? throw new KeyNotFoundException($"Detalle de Ajuste con id {item.AdjustmentDetailId} no existe.");

                        entityDetail.Quantity = item.Quantity;
                        entityDetail.ReferenceId = item.ReferenceId;
                        entityDetail.WarehouseId = item.WarehouseId;
                        continue;
                    }

                    entity.AdjustmentDetails.Add(new AdjustmentDetail
                    {
                        Quantity = item.Quantity,
                        ReferenceId = item.ReferenceId,
                        WarehouseId = item.WarehouseId,
                        AdjustmentId = item.AdjustmentId
                    });
                }

                try
                {
                    dbContext.Events.AddOrUpdate("ChangeAdjustmentType", false);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }
    }

}
