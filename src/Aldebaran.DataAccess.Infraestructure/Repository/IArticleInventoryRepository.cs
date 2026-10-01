using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IArticleInventoryRepository
    {
        /// <summary>Inventario de las referencias activas del artículo al que pertenece <paramref name="referenceId"/>.</summary>
        Task<IEnumerable<ItemReferenceInventory>> GetByReferenceAsync(int referenceId, CancellationToken ct = default);
    }
}
