using Aldebaran.Application.Services.InternalReferences;
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Xunit;

namespace Aldebaran.Application.Services.Tests.InternalReferences
{
    public class InternalReferenceConsecutiveServiceTests
    {
        private sealed class FakeRepository : IInternalReferenceConsecutiveRepository
        {
            public string? LastCode { get; private set; }
            public int? LastQuantity { get; private set; }
            public int Calls { get; private set; }
            public List<InternalReferenceConsecutiveEntry> Rows { get; } = new();

            public Task<IEnumerable<InternalReferenceConsecutiveEntry>> GetAvailableAsync(string code, CancellationToken ct = default)
            {
                Calls++; LastCode = code;
                return Task.FromResult<IEnumerable<InternalReferenceConsecutiveEntry>>(Rows);
            }

            public Task<IEnumerable<InternalReferenceConsecutiveEntry>> GenerateAsync(string code, int quantity, CancellationToken ct = default)
            {
                Calls++; LastCode = code; LastQuantity = quantity;
                return Task.FromResult<IEnumerable<InternalReferenceConsecutiveEntry>>(Rows);
            }

            public List<InternalReferenceNonconformingEntry> Nonconforming { get; } = new();

            public Task<IEnumerable<InternalReferenceNonconformingEntry>> GetNonconformingAsync(string code, CancellationToken ct = default)
            {
                Calls++; LastCode = code;
                return Task.FromResult<IEnumerable<InternalReferenceNonconformingEntry>>(Nonconforming);
            }
        }

        private static InternalReferenceConsecutiveEntry Row(string code, int n, bool isNew = false) =>
            new() { Code = code, Consecutive = n, InternalReference = $"{code}-{n:000}", IsNew = isNew };

        [Fact]
        public async Task GenerateAsync_SendsNormalizedCodeAndRequiredQuantity_AndMapsAvailableWithNewFlag()
        {
            // Disponibles 26 y 27; se piden 5 => la BD crea 28, 29 y 30 y devuelve los 5 disponibles.
            var repository = new FakeRepository();
            repository.Rows.AddRange(new[] { Row("MU", 26), Row("MU", 27), Row("MU", 28, true), Row("MU", 29, true), Row("MU", 30, true) });
            var service = new InternalReferenceConsecutiveService(repository);

            var result = await service.GenerateAsync(" mu ", 5);

            Assert.Equal("MU", repository.LastCode);
            Assert.Equal(5, repository.LastQuantity);
            Assert.Equal(5, result.Count);
            Assert.Equal(new[] { "MU-028", "MU-029", "MU-030" }, result.Where(r => r.IsNew).Select(r => r.InternalReference));
            Assert.Equal(new[] { 26, 27 }, result.Where(r => !r.IsNew).Select(r => r.Consecutive));
        }

        [Fact]
        public async Task GetAvailableAsync_SendsNormalizedCode()
        {
            var repository = new FakeRepository();
            repository.Rows.Add(Row("MUX", 1));
            var service = new InternalReferenceConsecutiveService(repository);

            var result = await service.GetAvailableAsync("mux");

            Assert.Equal("MUX", repository.LastCode);
            Assert.Single(result);
            Assert.Equal("MUX-001", result[0].InternalReference);
        }

        [Theory]
        [InlineData("", 1)]
        [InlineData("M-U", 1)]
        [InlineData("ABCDEF", 1)]
        [InlineData("MU", 0)]
        [InlineData("MU", 1001)]
        public async Task GenerateAsync_InvalidInput_ThrowsAndDoesNotCallRepository(string code, int quantity)
        {
            var repository = new FakeRepository();
            var service = new InternalReferenceConsecutiveService(repository);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GenerateAsync(code, quantity));
            Assert.Equal(0, repository.Calls);
        }

        [Fact]
        public async Task GetNonconformingAsync_SendsNormalizedCode_AndMapsReason()
        {
            var repository = new FakeRepository();
            repository.Nonconforming.Add(new InternalReferenceNonconformingEntry { InternalReference = "MU-XL", ItemName = "Mug", Reason = "Después del guion no hay un número (sigue \"X\")." });
            var service = new InternalReferenceConsecutiveService(repository);

            var result = await service.GetNonconformingAsync("mu");

            Assert.Equal("MU", repository.LastCode);
            Assert.Single(result);
            Assert.Equal("MU-XL", result[0].InternalReference);
            Assert.Contains("\"X\"", result[0].Reason);
        }

        [Fact]
        public async Task GetAvailableAsync_EmptyCode_Throws()
        {
            var repository = new FakeRepository();
            var service = new InternalReferenceConsecutiveService(repository);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetAvailableAsync("  "));
            Assert.Equal(0, repository.Calls);
        }
    }
}
