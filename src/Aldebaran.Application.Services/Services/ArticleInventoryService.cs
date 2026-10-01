using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    /// <summary>
    /// Orquestador del caso de uso: valida la referencia, consulta el repositorio
    /// y entrega el modelo propio del caso de uso (no la entidad de datos).
    /// </summary>
    public class ArticleInventoryService : IArticleInventoryService
    {
        private readonly IArticleInventoryRepository _repository;

        public ArticleInventoryService(IArticleInventoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<ArticleReferenceInventory>> GetByReferenceAsync(int referenceId, CancellationToken ct = default)
        {
            if (referenceId <= 0)
                return Array.Empty<ArticleReferenceInventory>();

            var rows = await _repository.GetByReferenceAsync(referenceId, ct);
            return rows.Select(ToModel).ToList();
        }

        private static ArticleReferenceInventory ToModel(Entities.ItemReferenceInventory row) =>
            new(row.ReferenceId,
                row.ReferenceCode,
                row.ReferenceName,
                row.LocalWarehouse,
                row.FreeZone,
                row.PhysicalStock,
                row.InTransit,
                row.Committed,
                row.Available,
                row.IsAlarmReference);
    }
}
