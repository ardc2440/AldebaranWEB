using Aldebaran.Application.Services.Models.Competitors;
using Aldebaran.DataAccess.Infraestructure.Repository.Competitors;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>
    /// Orquesta los casos de uso de homologaciones: normaliza y valida (reglas puras), verifica compañía activa,
    /// artículo, pertenencia de la referencia y unicidad entre activas (BD), y delega la persistencia al repositorio.
    /// </summary>
    public class CompetitorHomologationService : ICompetitorHomologationService
    {
        private readonly ICompetitorHomologationRepository _repository;
        private readonly ICompetitorImageStorage? _imageStorage;

        public CompetitorHomologationService(ICompetitorHomologationRepository repository, ICompetitorImageStorage? imageStorage = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _imageStorage = imageStorage;
        }

        public async Task<IReadOnlyList<CompetitorHomologation>> GetAsync(CompetitorHomologationFilter filter, CancellationToken ct = default)
        {
            filter ??= new CompetitorHomologationFilter();
            var query = new CompetitorHomologationQuery
            {
                CompetitorCompanyId = filter.CompetitorCompanyId,
                ItemId = filter.ItemId,
                IncludeInactive = filter.IncludeInactive,
                SearchKey = filter.SearchKey
            };
            return (await _repository.GetAsync(query, ct)).Select(ToModel).ToList();
        }

        public async Task<CompetitorHomologation?> FindAsync(int competitorHomologationId, CancellationToken ct = default)
        {
            var entity = await _repository.FindAsync(competitorHomologationId, ct);
            return entity == null ? null : ToModel(entity);
        }

        public async Task<int> AddAsync(CompetitorHomologationInput input, CancellationToken ct = default)
        {
            var data = await ValidateAsync(input, requireActiveCompany: true, checkDuplicate: true, excludeId: null, ct);
            return await _repository.AddAsync(ToEntity(data), ct);
        }

        public async Task UpdateAsync(int competitorHomologationId, CompetitorHomologationInput input, CancellationToken ct = default)
        {
            var current = await GetExistingAsync(competitorHomologationId, ct);
            // Si cambia la compañía, la nueva debe estar activa. Una homologación inactiva puede coincidir con una activa;
            // la unicidad se valida al reactivarla.
            var data = await ValidateAsync(input,
                requireActiveCompany: input?.CompetitorCompanyId != current.CompetitorCompanyId,
                checkDuplicate: current.IsActive, excludeId: competitorHomologationId, ct);
            await _repository.UpdateAsync(competitorHomologationId, ToEntity(data), ct);
        }

        public async Task InactivateAsync(int competitorHomologationId, CancellationToken ct = default)
        {
            await GetExistingAsync(competitorHomologationId, ct);
            await _repository.SetActiveAsync(competitorHomologationId, false, ct);
        }

        public async Task ActivateAsync(int competitorHomologationId, CancellationToken ct = default)
        {
            var current = await GetExistingAsync(competitorHomologationId, ct);
            if (current.IsActive)
                return;
            if (!await _repository.IsActiveCompanyAsync(current.CompetitorCompanyId, ct))
                throw new InvalidOperationException("No se puede reactivar: la compañía de la competencia está inactiva.");
            if (await _repository.ExistsActiveAsync(current.CompetitorCompanyId, current.ItemId, current.ReferenceId, current.CompetitorReference, competitorHomologationId, ct))
                throw new InvalidOperationException(CompetitorHomologationRepository.DuplicateMessage);
            await _repository.SetActiveAsync(competitorHomologationId, true, ct);
        }

        #region Imagen
        public async Task SetImageAsync(int competitorHomologationId, Stream content, string fileName, long size, CancellationToken ct = default)
        {
            var error = CompetitorImageRules.Validate(fileName, size);
            if (error != null)
                throw new ArgumentException(error);
            var storage = _imageStorage ?? throw new InvalidOperationException("El almacenamiento de imágenes no está configurado.");
            // Se toma el nombre anterior antes de actualizar: no depender de que la entidad leída no cambie.
            var previousFileName = (await GetExistingAsync(competitorHomologationId, ct)).ImageFileName;

            var newFileName = await storage.SaveAsync(competitorHomologationId, content, CompetitorImageRules.NormalizeExtension(fileName)!, ct);
            try
            {
                await _repository.SetImageFileNameAsync(competitorHomologationId, newFileName, ct);
            }
            catch
            {
                storage.Delete(newFileName); // no dejar archivos huérfanos si no se pudo registrar
                throw;
            }
            if (!string.Equals(previousFileName, newFileName, StringComparison.OrdinalIgnoreCase))
                storage.Delete(previousFileName);
        }

        public async Task RemoveImageAsync(int competitorHomologationId, CancellationToken ct = default)
        {
            var previousFileName = (await GetExistingAsync(competitorHomologationId, ct)).ImageFileName;
            if (previousFileName == null)
                return;
            await _repository.SetImageFileNameAsync(competitorHomologationId, null, ct);
            _imageStorage?.Delete(previousFileName);
        }
        #endregion

        #region Validación
        private async Task<CompetitorHomologationInput> ValidateAsync(CompetitorHomologationInput input, bool requireActiveCompany, bool checkDuplicate, int? excludeId, CancellationToken ct)
        {
            var data = CompetitorHomologationRules.Normalize(input);
            var errors = CompetitorHomologationRules.Validate(data);
            if (errors.Count > 0)
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            if (requireActiveCompany && !await _repository.IsActiveCompanyAsync(data.CompetitorCompanyId, ct))
                throw new ArgumentException("La compañía de la competencia no existe o está inactiva.");
            if (!await _repository.ItemExistsAsync(data.ItemId, ct))
                throw new ArgumentException("El artículo no existe.");
            if (data.ReferenceId != null && !await _repository.ReferenceBelongsToItemAsync(data.ReferenceId.Value, data.ItemId, ct))
                throw new ArgumentException("La referencia no pertenece al artículo seleccionado.");
            if (checkDuplicate && await _repository.ExistsActiveAsync(data.CompetitorCompanyId, data.ItemId, data.ReferenceId, data.CompetitorReference, excludeId, ct))
                throw new InvalidOperationException(CompetitorHomologationRepository.DuplicateMessage);
            return data;
        }

        private async Task<Entities.CompetitorHomologation> GetExistingAsync(int competitorHomologationId, CancellationToken ct) =>
            await _repository.FindAsync(competitorHomologationId, ct)
            ?? throw new KeyNotFoundException($"Homologación con id {competitorHomologationId} no existe.");
        #endregion

        #region Mapeo
        private static CompetitorHomologation ToModel(Entities.CompetitorHomologation e) =>
            new(e.CompetitorHomologationId, e.CompetitorCompanyId, e.CompetitorCompany.CompanyName, e.CompetitorCompany.IsActive,
                e.Item.Line?.LineName ?? string.Empty, e.ItemId, e.Item.InternalReference, e.Item.ItemName,
                e.ReferenceId, e.ItemReference?.ReferenceCode, e.ItemReference?.ReferenceName,
                e.CompetitorReference, e.CompetitorProductName, e.ProductUrl, e.CompetitorPrice, e.ImageFileName,
                e.IsActive, e.CreationDate);

        private static Entities.CompetitorHomologation ToEntity(CompetitorHomologationInput data) => new()
        {
            CompetitorCompanyId = data.CompetitorCompanyId,
            ItemId = data.ItemId,
            ReferenceId = data.ReferenceId,
            CompetitorReference = data.CompetitorReference,
            CompetitorProductName = data.CompetitorProductName,
            ProductUrl = data.ProductUrl,
            CompetitorPrice = data.CompetitorPrice
        };
        #endregion
    }
}
