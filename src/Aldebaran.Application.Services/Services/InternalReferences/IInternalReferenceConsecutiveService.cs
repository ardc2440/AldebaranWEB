using Aldebaran.Application.Services.Models.InternalReferences;

namespace Aldebaran.Application.Services.InternalReferences
{
    public interface IInternalReferenceConsecutiveService
    {
        /// <summary>Consecutivos generados para el código que aún no usa ninguna referencia interna de artículo.</summary>
        Task<IReadOnlyList<InternalReferenceConsecutive>> GetAvailableAsync(string code, CancellationToken ct = default);

        /// <summary>Genera la cantidad indicada de consecutivos seguidos y los devuelve.</summary>
        Task<IReadOnlyList<InternalReferenceConsecutive>> GenerateAsync(string code, int quantity, CancellationToken ct = default);
    }
}
