using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Competitors
{
    public class CompetitorHomologationRepository : RepositoryBase<AldebaranDbContext>, ICompetitorHomologationRepository
    {
        public const string DuplicateMessage = "Ya existe una homologación activa igual (misma compañía, artículo, referencia y referencia del competidor).";

        public CompetitorHomologationRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IReadOnlyList<CompetitorHomologation>> GetAsync(CompetitorHomologationQuery query, CancellationToken ct = default)
        {
            query ??= new CompetitorHomologationQuery();
            return await ExecuteQueryAsync(async dbContext =>
            {
                var data = WithRelations(dbContext.CompetitorHomologations.AsNoTracking());
                if (!query.IncludeInactive)
                    data = data.Where(h => h.IsActive);
                if (query.CompetitorCompanyId != null)
                    data = data.Where(h => h.CompetitorCompanyId == query.CompetitorCompanyId);
                if (query.ItemId != null)
                    data = data.Where(h => h.ItemId == query.ItemId);
                if (!string.IsNullOrWhiteSpace(query.SearchKey))
                {
                    var key = query.SearchKey.Trim();
                    data = data.Where(h => h.CompetitorReference.Contains(key) || h.CompetitorProductName.Contains(key)
                                        || h.Item.InternalReference.Contains(key) || h.Item.ItemName.Contains(key));
                }
                return (IReadOnlyList<CompetitorHomologation>)await data
                    .OrderBy(h => h.CompetitorCompany.CompanyName).ThenBy(h => h.Item.ItemName)
                    .ThenBy(h => h.ItemReference == null ? string.Empty : h.ItemReference.ReferenceName).ThenBy(h => h.CompetitorReference)
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<CompetitorHomologation?> FindAsync(int competitorHomologationId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                await WithRelations(dbContext.CompetitorHomologations.AsNoTracking())
                    .FirstOrDefaultAsync(h => h.CompetitorHomologationId == competitorHomologationId, ct), ct);
        }

        public async Task<bool> ExistsActiveAsync(int competitorCompanyId, int itemId, int? referenceId, string competitorReference, int? excludeCompetitorHomologationId, CancellationToken ct = default)
        {
            var reference = (competitorReference ?? string.Empty).Trim();
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.CompetitorHomologations.AsNoTracking()
                    .AnyAsync(h => h.IsActive && h.CompetitorCompanyId == competitorCompanyId && h.ItemId == itemId
                                   && h.ReferenceId == referenceId && h.CompetitorReference == reference
                                   && (excludeCompetitorHomologationId == null || h.CompetitorHomologationId != excludeCompetitorHomologationId), ct), ct);
        }

        public async Task<bool> IsActiveCompanyAsync(int competitorCompanyId, CancellationToken ct = default) =>
            await ExecuteQueryAsync(async dbContext =>
                await dbContext.CompetitorCompanies.AsNoTracking().AnyAsync(c => c.CompetitorCompanyId == competitorCompanyId && c.IsActive, ct), ct);

        public async Task<bool> ItemExistsAsync(int itemId, CancellationToken ct = default) =>
            await ExecuteQueryAsync(async dbContext =>
                await dbContext.Items.AsNoTracking().AnyAsync(i => i.ItemId == itemId, ct), ct);

        public async Task<bool> ReferenceBelongsToItemAsync(int referenceId, int itemId, CancellationToken ct = default) =>
            await ExecuteQueryAsync(async dbContext =>
                await dbContext.ItemReferences.AsNoTracking().AnyAsync(r => r.ReferenceId == referenceId && r.ItemId == itemId, ct), ct);

        public async Task<int> AddAsync(CompetitorHomologation homologation, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.CompetitorHomologations.AddAsync(homologation, ct);
                await SaveAsync(dbContext, ct);
                return homologation.CompetitorHomologationId;
            }, ct);
        }

        public async Task UpdateAsync(int competitorHomologationId, CompetitorHomologation homologation, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await GetTrackedAsync(dbContext, competitorHomologationId, ct);
                entity.CompetitorCompanyId = homologation.CompetitorCompanyId;
                entity.ItemId = homologation.ItemId;
                entity.ReferenceId = homologation.ReferenceId;
                entity.CompetitorReference = homologation.CompetitorReference;
                entity.CompetitorProductName = homologation.CompetitorProductName;
                entity.ProductUrl = homologation.ProductUrl;
                entity.CompetitorPrice = homologation.CompetitorPrice;
                await SaveAsync(dbContext, ct);
            }, ct);
        }

        public async Task SetActiveAsync(int competitorHomologationId, bool isActive, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await GetTrackedAsync(dbContext, competitorHomologationId, ct);
                entity.IsActive = isActive;
                await SaveAsync(dbContext, ct);
            }, ct);
        }

        public async Task SetImageFileNameAsync(int competitorHomologationId, string? imageFileName, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await GetTrackedAsync(dbContext, competitorHomologationId, ct);
                entity.ImageFileName = imageFileName;
                await SaveAsync(dbContext, ct);
            }, ct);
        }

        private static IQueryable<CompetitorHomologation> WithRelations(IQueryable<CompetitorHomologation> query) =>
            query.Include(h => h.CompetitorCompany).Include(h => h.Item).ThenInclude(i => i.Line).Include(h => h.ItemReference);

        private static async Task<CompetitorHomologation> GetTrackedAsync(AldebaranDbContext dbContext, int competitorHomologationId, CancellationToken ct) =>
            await dbContext.CompetitorHomologations.FirstOrDefaultAsync(h => h.CompetitorHomologationId == competitorHomologationId, ct)
            ?? throw new KeyNotFoundException($"Homologación con id {competitorHomologationId} no existe.");

        /// <summary>Guarda y traduce la violación del índice único (solo activas) a un mensaje para el usuario.</summary>
        private static async Task SaveAsync(AldebaranDbContext dbContext, CancellationToken ct)
        {
            try
            {
                await dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new InvalidOperationException(DuplicateMessage, ex);
            }
        }
    }
}
