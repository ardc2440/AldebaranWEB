using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Adjustment adjustment, CancellationToken ct = default)
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
                    WarehouseId = item.WarehouseId,
                    AdjustmentId = item.AdjustmentId
                });

            await _context.Adjustments.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int adjustmentId, CancellationToken ct = default)
        {
            var entity = await _context.Adjustments.FirstOrDefaultAsync(x => x.AdjustmentId == adjustmentId, ct) ?? throw new KeyNotFoundException($"Ajuste con id {adjustmentId} no existe.");
            _context.Adjustments.Remove(entity);
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
                .Where(i => i.StatusDocumentType.StatusDocumentTypeName.Equals(searchKey) ||
                          i.Employee.FullName.Equals(searchKey) ||
                          i.Employee.DisplayName.Equals(searchKey) ||
                          i.Employee.IdentityNumber.Equals(searchKey) ||
                          i.Employee.IdentityType.IdentityTypeName.Equals(searchKey) ||
                          i.Employee.IdentityType.IdentityTypeCode.Equals(searchKey) ||
                          i.Employee.Area.AreaName.Equals(searchKey) ||
                          i.Employee.Area.AreaCode.Equals(searchKey) ||
                          i.Employee.Area.Description.Equals(searchKey) ||
                          i.StatusDocumentType.StatusDocumentTypeName.Equals(searchKey) ||
                          i.StatusDocumentType.Notes.Equals(searchKey) ||
                          i.AdjustmentReason.AdjustmentReasonName.Equals(searchKey) ||
                          i.AdjustmentReason.AdjustmentReasonNotes.Equals(searchKey) ||
                          i.AdjustmentType.AdjustmentTypeName.Equals(searchKey) ||
                          i.Notes.Equals(searchKey) ||
                          i.AdjustmentDate.ToString().Equals(searchKey) ||
                          i.CreationDate.ToString().Equals(searchKey))
                .Include(i => i.StatusDocumentType)
                .Include(i => i.AdjustmentReason)
                .Include(i => i.AdjustmentType)
                .Include(i => i.Employee.IdentityType)
                .Include(i => i.Employee.Area)
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

            foreach (var item in adjustment.AdjustmentDetails)
            {
                if (item.AdjustmentDetailId > 0)
                {
                    var entityDetail = await _context.AdjustmentDetails.FirstOrDefaultAsync(i => i.AdjustmentDetailId == item.AdjustmentDetailId) ?? throw new KeyNotFoundException($"Dtalle de Ajuste con id {item.AdjustmentDetailId} no existe.");

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

            await _context.SaveChangesAsync(ct);
        }
    }

}
