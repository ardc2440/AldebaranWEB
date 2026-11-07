using Aldebaran.Application.Services.Models.Competitors;
using Aldebaran.DataAccess.Infraestructure.Repository.Competitors;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>
    /// Orquesta los casos de uso de compañías de la competencia: normaliza y valida (reglas puras),
    /// verifica ubicación y nombre único entre activas (BD) y delega la persistencia al repositorio.
    /// </summary>
    public class CompetitorCompanyService : ICompetitorCompanyService
    {
        private readonly ICompetitorCompanyRepository _repository;

        public CompetitorCompanyService(ICompetitorCompanyRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<CompetitorCompany>> GetAsync(bool includeInactive = false, string? searchKey = null, CancellationToken ct = default) =>
            (await _repository.GetAsync(includeInactive, searchKey, ct)).Select(ToModel).ToList();

        public async Task<CompetitorCompany?> FindAsync(int competitorCompanyId, CancellationToken ct = default)
        {
            var entity = await _repository.FindAsync(competitorCompanyId, ct);
            return entity == null ? null : ToModel(entity);
        }

        public async Task<int> AddAsync(CompetitorCompanyInput input, CancellationToken ct = default)
        {
            var data = await ValidateAsync(input, checkName: true, excludeId: null, ct);
            return await _repository.AddAsync(ToEntity(data), ct);
        }

        public async Task UpdateAsync(int competitorCompanyId, CompetitorCompanyInput input, CancellationToken ct = default)
        {
            var current = await GetExistingAsync(competitorCompanyId, ct);
            // Una compañía inactiva puede compartir nombre con una activa; el nombre se valida al reactivarla.
            var data = await ValidateAsync(input, checkName: current.IsActive, excludeId: competitorCompanyId, ct);
            await _repository.UpdateAsync(competitorCompanyId, ToEntity(data), ct);
        }

        public async Task InactivateAsync(int competitorCompanyId, CancellationToken ct = default)
        {
            await GetExistingAsync(competitorCompanyId, ct);
            await _repository.SetActiveAsync(competitorCompanyId, false, ct);
        }

        public async Task ActivateAsync(int competitorCompanyId, CancellationToken ct = default)
        {
            var current = await GetExistingAsync(competitorCompanyId, ct);
            if (current.IsActive)
                return;
            await EnsureUniqueNameAsync(current.CompanyName, competitorCompanyId, ct);
            await _repository.SetActiveAsync(competitorCompanyId, true, ct);
        }

        #region Validación
        private async Task<CompetitorCompanyInput> ValidateAsync(CompetitorCompanyInput input, bool checkName, int? excludeId, CancellationToken ct)
        {
            var data = CompetitorCompanyRules.Normalize(input);
            var errors = CompetitorCompanyRules.Validate(data);
            if (errors.Count > 0)
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            if (!await _repository.IsValidLocationAsync(data.CountryId, data.DepartmentId, data.CityId, ct))
                throw new ArgumentException("La ubicación no es válida: el departamento/estado debe pertenecer al país y la ciudad al departamento/estado.");
            if (checkName)
                await EnsureUniqueNameAsync(data.CompanyName, excludeId, ct);
            return data;
        }

        private async Task EnsureUniqueNameAsync(string companyName, int? excludeId, CancellationToken ct)
        {
            if (await _repository.ExistsActiveByNameAsync(companyName, excludeId, ct))
                throw new InvalidOperationException($"Ya existe una compañía de la competencia activa con el nombre \"{companyName}\".");
        }

        private async Task<Entities.CompetitorCompany> GetExistingAsync(int competitorCompanyId, CancellationToken ct) =>
            await _repository.FindAsync(competitorCompanyId, ct)
            ?? throw new KeyNotFoundException($"Compañía de la competencia con id {competitorCompanyId} no existe.");
        #endregion

        #region Mapeo
        private static CompetitorCompany ToModel(Entities.CompetitorCompany e) =>
            new(e.CompetitorCompanyId, e.CompanyName, e.Website, e.ContactPerson, e.Phone,
                e.CountryId, e.Country?.CountryName, e.DepartmentId, e.Department?.DepartmentName, e.CityId, e.City?.CityName,
                e.IsActive, e.CreationDate);

        private static Entities.CompetitorCompany ToEntity(CompetitorCompanyInput data) => new()
        {
            CompanyName = data.CompanyName,
            Website = data.Website,
            ContactPerson = data.ContactPerson,
            Phone = data.Phone,
            CountryId = data.CountryId,
            DepartmentId = data.DepartmentId,
            CityId = data.CityId
        };
        #endregion
    }
}
