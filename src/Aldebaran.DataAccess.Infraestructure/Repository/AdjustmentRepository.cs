using Aldebaran.DataAccess.Entities;
using Aldebaran.Infraestructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Aldebaran.Infraestructure.Common.Utils;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly AldebaranDbContext _context;
        private readonly ISharedStringLocalizer _SharedLocalizer;
        public AdjustmentRepository(ISharedStringLocalizer sharedLocalizer, AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _SharedLocalizer = sharedLocalizer;
        }

        public async Task<Adjustment> AddAsync(Adjustment adjustment, CancellationToken ct = default)
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
                await _context.Adjustments.AddAsync(entity, ct);
                _context.Events.AddOrUpdate("ChangeAdjustmentType", false);
                await _context.SaveChangesAsync(ct);
                return entity;
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task CancelAsync(int adjustmentId, CancellationToken ct = default)
        {
            var entity = await _context.Adjustments.FirstOrDefaultAsync(x => x.AdjustmentId == adjustmentId, ct) ?? throw new KeyNotFoundException($"Ajuste con id {adjustmentId} no existe.");
            var documentType = await _context.DocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeCode == "A", ct);
            var statutsDocumentType = await _context.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.DocumentTypeId == documentType.DocumentTypeId && f.StatusOrder == 2, ct);
            entity.StatusDocumentTypeId = statutsDocumentType.StatusDocumentTypeId;
            try
            {
                _context.Events.AddOrUpdate("ChangeAdjustmentType", false);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default)
        {
            return await _context.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .FirstOrDefaultAsync(w => w.AdjustmentId == adjustmentId, ct);
        }

        public async Task<IEnumerable<Adjustment>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee)
                .Include(i => i.AdjustmentDetails)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Adjustment>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Adjustments.AsNoTracking()
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee.IdentityType)
                .Include(i => i.Employee.Area)
                .Where(i => i.StatusDocumentType.StatusDocumentTypeName.Contains(searchKey) ||
                          i.AdjustmentId.ToString().Contains(searchKey) ||
                          _context.Format(i.AdjustmentDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                          _context.Format(i.CreationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                          i.AdjustmentType.AdjustmentTypeName.Contains(searchKey) ||
                          i.AdjustmentReason.AdjustmentReasonName.Contains(searchKey) ||
                          i.Employee.FullName.Contains(searchKey) ||
                          i.AdjustmentReason.AdjustmentReasonNotes.Contains(searchKey) ||
                          i.Notes.Contains(searchKey))
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default)
        {
            var entity = await _context.Adjustments
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
                    _context.AdjustmentDetails.Remove(item);
            }

            foreach (var item in adjustment.AdjustmentDetails)
            {
                if (item.AdjustmentDetailId > 0)
                {
                    var entityDetail = await _context.AdjustmentDetails.FirstOrDefaultAsync(i => i.AdjustmentDetailId == item.AdjustmentDetailId, ct) ?? throw new KeyNotFoundException($"Detalle de Ajuste con id {item.AdjustmentDetailId} no existe.");

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
                _context.Events.AddOrUpdate("ChangeAdjustmentType", false);
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }

}
