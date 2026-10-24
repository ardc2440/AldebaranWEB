using Aldebaran.Application.Services.Models.InternalReferences;
using Aldebaran.DataAccess.Infraestructure.Repository;

namespace Aldebaran.Application.Services.InternalReferences
{
    public class InternalReferenceConsecutiveService : IInternalReferenceConsecutiveService
    {
        private readonly IInternalReferenceConsecutiveRepository _repository;

        public InternalReferenceConsecutiveService(IInternalReferenceConsecutiveRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInternalReferenceConsecutiveRepository));
        }

        public async Task<IReadOnlyList<InternalReferenceConsecutive>> GetAvailableAsync(string code, CancellationToken ct = default)
        {
            var normalized = ValidCode(code);
            return ToModels(await _repository.GetAvailableAsync(normalized, ct));
        }

        public async Task<IReadOnlyList<InternalReferenceConsecutive>> GenerateAsync(string code, int quantity, CancellationToken ct = default)
        {
            var normalized = ValidCode(code);
            var quantityError = InternalReferenceCodeRules.ValidateQuantity(quantity);
            if (quantityError != null)
                throw new ArgumentException(quantityError, nameof(quantity));
            return ToModels(await _repository.GenerateAsync(normalized, quantity, ct));
        }

        public async Task<IReadOnlyList<InternalReferenceNonconforming>> GetNonconformingAsync(string code, CancellationToken ct = default)
        {
            var normalized = ValidCode(code);
            var rows = await _repository.GetNonconformingAsync(normalized, ct);
            return rows.Select(r => new InternalReferenceNonconforming(r.InternalReference, r.ItemName, r.Reason)).ToList();
        }

        private static string ValidCode(string code)
        {
            var normalized = InternalReferenceCodeRules.Normalize(code);
            var error = InternalReferenceCodeRules.ValidateCode(normalized);
            return error == null ? normalized : throw new ArgumentException(error, nameof(code));
        }

        private static IReadOnlyList<InternalReferenceConsecutive> ToModels(IEnumerable<DataAccess.Entities.InternalReferenceConsecutiveEntry> rows) =>
            rows.Select(r => new InternalReferenceConsecutive(r.Code, r.Consecutive, r.InternalReference, r.IsNew)).ToList();
    }
}
