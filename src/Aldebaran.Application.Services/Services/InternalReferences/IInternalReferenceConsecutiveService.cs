using Aldebaran.Application.Services.Models.InternalReferences;

namespace Aldebaran.Application.Services.InternalReferences
{
    public interface IInternalReferenceConsecutiveService
    {
        /// <summary>Consecutivos disponibles del código (los usados en artículos se eliminan antes de consultar).</summary>
        Task<IReadOnlyList<InternalReferenceConsecutive>> GetAvailableAsync(string code, CancellationToken ct = default);

        /// <summary>Completa hasta <paramref name="quantity"/> disponibles creando solo los que faltan; devuelve todos los disponibles.</summary>
        Task<IReadOnlyList<InternalReferenceConsecutive>> GenerateAsync(string code, int quantity, CancellationToken ct = default);

        /// <summary>Referencias de artículos con el código que no cumplen el formato de consecutivo (explican el siguiente número).</summary>
        Task<IReadOnlyList<InternalReferenceNonconforming>> GetNonconformingAsync(string code, CancellationToken ct = default);
    }
}
