using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    /// <summary>Casos de uso del inventario del artículo de una alarma de cantidades mínimas.</summary>
    public interface IArticleInventoryService
    {
        /// <summary>
        /// Inventario de las referencias activas del artículo al que pertenece <paramref name="referenceId"/>,
        /// con la referencia de la alarma marcada. Lista vacía si la referencia no existe o no es válida.
        /// </summary>
        Task<IReadOnlyList<ArticleReferenceInventory>> GetByReferenceAsync(int referenceId, CancellationToken ct = default);
    }
}
