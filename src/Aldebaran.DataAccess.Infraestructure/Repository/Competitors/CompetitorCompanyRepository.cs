using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Competitors
{
    public class CompetitorCompanyRepository : RepositoryBase<AldebaranDbContext>, ICompetitorCompanyRepository
    {
        public CompetitorCompanyRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IReadOnlyList<CompetitorCompany>> GetAsync(bool includeInactive, string? searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var query = WithLocation(dbContext.CompetitorCompanies.AsNoTracking());
                if (!includeInactive)
                    query = query.Where(c => c.IsActive);
                if (!string.IsNullOrWhiteSpace(searchKey))
                {
                    var key = searchKey.Trim();
                    query = query.Where(c => c.CompanyName.Contains(key) || c.Website.Contains(key)
                                          || (c.ContactPerson != null && c.ContactPerson.Contains(key))
                                          || (c.Phone != null && c.Phone.Contains(key)));
                }
                return (IReadOnlyList<CompetitorCompany>)await query.OrderBy(c => c.CompanyName).ToListAsync(ct);
            }, ct);
        }

        public async Task<CompetitorCompany?> FindAsync(int competitorCompanyId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                await WithLocation(dbContext.CompetitorCompanies.AsNoTracking())
                    .FirstOrDefaultAsync(c => c.CompetitorCompanyId == competitorCompanyId, ct), ct);
        }

        public async Task<bool> ExistsActiveByNameAsync(string companyName, int? excludeCompetitorCompanyId, CancellationToken ct = default)
        {
            var name = (companyName ?? string.Empty).Trim();
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.CompetitorCompanies.AsNoTracking()
                    .AnyAsync(c => c.IsActive && c.CompanyName == name
                                   && (excludeCompetitorCompanyId == null || c.CompetitorCompanyId != excludeCompetitorCompanyId), ct), ct);
        }

        public async Task<bool> IsValidLocationAsync(int? countryId, int? departmentId, int? cityId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                if (departmentId != null && !await dbContext.Departments.AsNoTracking().AnyAsync(d => d.DepartmentId == departmentId && d.CountryId == countryId, ct))
                    return false;
                if (cityId != null && !await dbContext.Cities.AsNoTracking().AnyAsync(c => c.CityId == cityId && c.DepartmentId == departmentId, ct))
                    return false;
                if (countryId != null && departmentId == null && !await dbContext.Countries.AsNoTracking().AnyAsync(c => c.CountryId == countryId, ct))
                    return false;
                return true;
            }, ct);
        }

        public async Task<int> AddAsync(CompetitorCompany company, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.CompetitorCompanies.AddAsync(company, ct);
                await SaveAsync(dbContext, company.CompanyName, ct);
                return company.CompetitorCompanyId;
            }, ct);
        }

        public async Task UpdateAsync(int competitorCompanyId, CompetitorCompany company, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await GetTrackedAsync(dbContext, competitorCompanyId, ct);
                entity.CompanyName = company.CompanyName;
                entity.Website = company.Website;
                entity.ContactPerson = company.ContactPerson;
                entity.Phone = company.Phone;
                entity.CountryId = company.CountryId;
                entity.DepartmentId = company.DepartmentId;
                entity.CityId = company.CityId;
                await SaveAsync(dbContext, entity.CompanyName, ct);
            }, ct);
        }

        public async Task SetActiveAsync(int competitorCompanyId, bool isActive, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await GetTrackedAsync(dbContext, competitorCompanyId, ct);
                entity.IsActive = isActive;
                await SaveAsync(dbContext, entity.CompanyName, ct);
            }, ct);
        }

        private static IQueryable<CompetitorCompany> WithLocation(IQueryable<CompetitorCompany> query) =>
            query.Include(c => c.Country).Include(c => c.Department).Include(c => c.City);

        private static async Task<CompetitorCompany> GetTrackedAsync(AldebaranDbContext dbContext, int competitorCompanyId, CancellationToken ct) =>
            await dbContext.CompetitorCompanies.FirstOrDefaultAsync(c => c.CompetitorCompanyId == competitorCompanyId, ct)
            ?? throw new KeyNotFoundException($"Compañía de la competencia con id {competitorCompanyId} no existe.");

        /// <summary>Guarda y traduce la violación del índice único (solo activos) a un mensaje para el usuario.</summary>
        private static async Task SaveAsync(AldebaranDbContext dbContext, string companyName, CancellationToken ct)
        {
            try
            {
                await dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new InvalidOperationException($"Ya existe una compañía de la competencia activa con el nombre \"{companyName}\".", ex);
            }
        }
    }
}
