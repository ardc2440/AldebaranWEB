﻿using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IProviderRepository
    {
        Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default);
        Task<bool> ExistsByCode(string code, CancellationToken ct = default);
        Task<bool> ExistsByName(string name, CancellationToken ct = default);
        Task<(IEnumerable<Provider>, int)> GetAsync(int? skip = null, int? top = null, CancellationToken ct = default);
        Task<(IEnumerable<Provider>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<Provider?> FindAsync(int providerId, CancellationToken ct = default);
        Task AddAsync(Provider provider, CancellationToken ct = default);
        Task UpdateAsync(int providerId, Provider provider, CancellationToken ct = default);
        Task DeleteAsync(int providerId, CancellationToken ct = default);

    }
}
