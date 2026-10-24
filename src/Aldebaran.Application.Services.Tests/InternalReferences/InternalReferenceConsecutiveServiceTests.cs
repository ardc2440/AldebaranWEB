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
        }

        private static InternalReferenceConsecutiveEntry Row(string code, int n) =>
            new() { Code = code, Consecutive = n, InternalReference = $"{code}-{n:000}" };

        [Fact]
        public async Task GenerateAsync_SendsNormalizedCodeAndQuantity_AndMapsResult()
        {
            var repository = new FakeRepository();
            repository.Rows.AddRange(new[] { Row("MU", 26), Row("MU", 27) });
            var service = new InternalReferenceConsecutiveService(repository);

            var result = await service.GenerateAsync(" mu ", 2);

            Assert.Equal("MU", repository.LastCode);
            Assert.Equal(2, repository.LastQuantity);
            Assert.Equal(new[] { "MU-026", "MU-027" }, result.Select(r => r.InternalReference));
            Assert.Equal(new[] { 26, 27 }, result.Select(r => r.Consecutive));
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
        [InlineData("M U", 1)]
        [InlineData("ABCDEFG", 1)]
        [InlineData("MU", 0)]
        public async Task GenerateAsync_InvalidInput_ThrowsAndDoesNotCallRepository(string code, int quantity)
        {
            var repository = new FakeRepository();
            var service = new InternalReferenceConsecutiveService(repository);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GenerateAsync(code, quantity));
            Assert.Equal(0, repository.Calls);
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
