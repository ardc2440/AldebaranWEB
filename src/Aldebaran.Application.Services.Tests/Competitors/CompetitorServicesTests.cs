using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Aldebaran.DataAccess.Infraestructure.Repository.Competitors;
using Xunit;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Tests.Competitors
{
    /// <summary>Orquestación de los servicios del Catálogo de la competencia con repositorios en memoria (sin BD).</summary>
    public class CompetitorServicesTests
    {
        #region Repositorios en memoria
        private sealed class FakeCompanyRepository : ICompetitorCompanyRepository
        {
            public List<Entities.CompetitorCompany> Data { get; } = new();
            public bool ValidLocation { get; set; } = true;

            public Task<IReadOnlyList<Entities.CompetitorCompany>> GetAsync(bool includeInactive, string? searchKey, CancellationToken ct = default) =>
                Task.FromResult((IReadOnlyList<Entities.CompetitorCompany>)Data.Where(c => includeInactive || c.IsActive).ToList());
            public Task<Entities.CompetitorCompany?> FindAsync(int id, CancellationToken ct = default) =>
                Task.FromResult(Data.FirstOrDefault(c => c.CompetitorCompanyId == id));
            public Task<bool> ExistsActiveByNameAsync(string name, int? excludeId, CancellationToken ct = default) =>
                Task.FromResult(Data.Any(c => c.IsActive && string.Equals(c.CompanyName, name.Trim(), StringComparison.OrdinalIgnoreCase) && c.CompetitorCompanyId != excludeId));
            public Task<bool> IsValidLocationAsync(int? countryId, int? departmentId, int? cityId, CancellationToken ct = default) => Task.FromResult(ValidLocation);
            public Task<int> AddAsync(Entities.CompetitorCompany company, CancellationToken ct = default)
            {
                company.CompetitorCompanyId = Data.Count + 1; company.IsActive = true; Data.Add(company);
                return Task.FromResult(company.CompetitorCompanyId);
            }
            public Task UpdateAsync(int id, Entities.CompetitorCompany company, CancellationToken ct = default)
            {
                var e = Data.Single(c => c.CompetitorCompanyId == id); e.CompanyName = company.CompanyName; e.Website = company.Website;
                return Task.CompletedTask;
            }
            public Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
            {
                Data.Single(c => c.CompetitorCompanyId == id).IsActive = isActive; return Task.CompletedTask;
            }
        }

        private sealed class FakeHomologationRepository : ICompetitorHomologationRepository
        {
            public List<Entities.CompetitorHomologation> Data { get; } = new();
            public HashSet<int> ActiveCompanies { get; } = new() { 1 };
            public Dictionary<int, int> ReferenceItem { get; } = new() { [100] = 10, [200] = 20 };

            public Task<IReadOnlyList<Entities.CompetitorHomologation>> GetAsync(CompetitorHomologationQuery query, CancellationToken ct = default) =>
                Task.FromResult((IReadOnlyList<Entities.CompetitorHomologation>)Data.ToList());
            public Task<Entities.CompetitorHomologation?> FindAsync(int id, CancellationToken ct = default) =>
                Task.FromResult(Data.FirstOrDefault(h => h.CompetitorHomologationId == id));
            public Task<bool> ExistsActiveAsync(int companyId, int itemId, int? referenceId, string competitorReference, int? excludeId, CancellationToken ct = default) =>
                Task.FromResult(Data.Any(h => h.IsActive && h.CompetitorCompanyId == companyId && h.ItemId == itemId && h.ReferenceId == referenceId
                                              && h.CompetitorReference == competitorReference.Trim() && h.CompetitorHomologationId != excludeId));
            public Task<bool> IsActiveCompanyAsync(int companyId, CancellationToken ct = default) => Task.FromResult(ActiveCompanies.Contains(companyId));
            public Task<bool> ItemExistsAsync(int itemId, CancellationToken ct = default) => Task.FromResult(itemId is 10 or 20);
            public Task<bool> ReferenceBelongsToItemAsync(int referenceId, int itemId, CancellationToken ct = default) =>
                Task.FromResult(ReferenceItem.TryGetValue(referenceId, out var item) && item == itemId);
            public Task<int> AddAsync(Entities.CompetitorHomologation h, CancellationToken ct = default)
            {
                h.CompetitorHomologationId = Data.Count + 1; h.IsActive = true; Data.Add(h);
                return Task.FromResult(h.CompetitorHomologationId);
            }
            public Task UpdateAsync(int id, Entities.CompetitorHomologation h, CancellationToken ct = default)
            {
                var e = Data.Single(x => x.CompetitorHomologationId == id);
                e.CompetitorCompanyId = h.CompetitorCompanyId; e.ItemId = h.ItemId; e.ReferenceId = h.ReferenceId; e.CompetitorReference = h.CompetitorReference;
                return Task.CompletedTask;
            }
            public Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
            {
                Data.Single(x => x.CompetitorHomologationId == id).IsActive = isActive; return Task.CompletedTask;
            }
            public Task SetImageFileNameAsync(int id, string? imageFileName, CancellationToken ct = default) => Task.CompletedTask;
        }
        #endregion

        #region Compañías
        private static CompetitorCompanyInput Company(string name) => new() { CompanyName = name, Website = "www.c.com" };

        [Fact]
        public async Task Compania_DuplicadaActiva_Rechazada()
        {
            var service = new CompetitorCompanyService(new FakeCompanyRepository());
            await service.AddAsync(Company("Competidor"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(Company(" competidor ")));
        }

        [Fact]
        public async Task Compania_UbicacionInvalida_Rechazada()
        {
            var service = new CompetitorCompanyService(new FakeCompanyRepository { ValidLocation = false });
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(Company("A")));
        }

        [Fact]
        public async Task Compania_ReactivarConOtraActivaIgual_Rechazada_EditarLaInactivaSiSePermite()
        {
            var repo = new FakeCompanyRepository();
            var service = new CompetitorCompanyService(repo);
            var first = await service.AddAsync(Company("Competidor"));
            await service.InactivateAsync(first);
            await service.AddAsync(Company("Competidor"));

            await service.UpdateAsync(first, Company("Competidor")); // inactiva: no valida nombre
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ActivateAsync(first));
            Assert.False(repo.Data.Single(c => c.CompetitorCompanyId == first).IsActive);
        }
        #endregion

        #region Homologaciones
        private static CompetitorHomologationInput Homologation(int company = 1, int item = 10, int? reference = null, string competitorReference = "CMP-1") => new()
        {
            CompetitorCompanyId = company, ItemId = item, ReferenceId = reference,
            CompetitorReference = competitorReference, CompetitorProductName = "Producto", ProductUrl = "competidor.com/p"
        };

        [Fact]
        public async Task Homologacion_Valida_NormalizaEnlace()
        {
            var repo = new FakeHomologationRepository();
            await new CompetitorHomologationService(repo).AddAsync(Homologation());
            Assert.Equal("https://competidor.com/p", repo.Data.Single().ProductUrl);
        }

        [Fact]
        public async Task Homologacion_CompaniaInactiva_Rechazada() =>
            await Assert.ThrowsAsync<ArgumentException>(() => new CompetitorHomologationService(new FakeHomologationRepository()).AddAsync(Homologation(company: 2)));

        [Fact]
        public async Task Homologacion_ReferenciaDeOtroArticulo_Rechazada() =>
            await Assert.ThrowsAsync<ArgumentException>(() => new CompetitorHomologationService(new FakeHomologationRepository()).AddAsync(Homologation(item: 10, reference: 200)));

        [Fact]
        public async Task Homologacion_MismaReferenciaCompetidor_ANivelArticuloYReferencia_SonDistintas()
        {
            var repo = new FakeHomologationRepository();
            var service = new CompetitorHomologationService(repo);
            await service.AddAsync(Homologation());
            await service.AddAsync(Homologation(reference: 100));
            Assert.Equal(2, repo.Data.Count);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(Homologation(reference: 100)));
        }

        [Fact]
        public async Task Homologacion_EditarSinCambiarCompaniaInactiva_SePermite_CambiarAInactiva_No()
        {
            var repo = new FakeHomologationRepository();
            repo.ActiveCompanies.Add(3);
            var service = new CompetitorHomologationService(repo);
            var id = await service.AddAsync(Homologation(company: 3));
            repo.ActiveCompanies.Remove(3); // la compañía se inactiva después

            await service.UpdateAsync(id, Homologation(company: 3, competitorReference: "CMP-2"));
            Assert.Equal("CMP-2", repo.Data.Single().CompetitorReference);
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(id, Homologation(company: 2)));
        }

        [Fact]
        public async Task Homologacion_Reactivar_ConDuplicadoActivoOCompaniaInactiva_Rechazada()
        {
            var repo = new FakeHomologationRepository();
            var service = new CompetitorHomologationService(repo);
            var first = await service.AddAsync(Homologation());
            await service.InactivateAsync(first);
            var second = await service.AddAsync(Homologation());
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ActivateAsync(first));

            await service.InactivateAsync(second);
            repo.ActiveCompanies.Remove(1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ActivateAsync(first));

            repo.ActiveCompanies.Add(1);
            await service.ActivateAsync(first);
            Assert.True(repo.Data.Single(h => h.CompetitorHomologationId == first).IsActive);
        }
        #endregion
    }
}
