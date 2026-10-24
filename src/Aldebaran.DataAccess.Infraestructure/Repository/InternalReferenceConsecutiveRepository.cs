using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class InternalReferenceConsecutiveRepository : RepositoryBase<AldebaranDbContext>, IInternalReferenceConsecutiveRepository
    {
        public InternalReferenceConsecutiveRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<IEnumerable<InternalReferenceConsecutiveEntry>> GetAvailableAsync(string code, CancellationToken ct = default) =>
            QueryAsync(new StoredProcedureCommand("dbo.SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES")
                .VarChar("@Code", code, 6), ct);

        /// <summary>El SP genera los consecutivos en una transacción con bloqueo por código y devuelve los generados.</summary>
        public Task<IEnumerable<InternalReferenceConsecutiveEntry>> GenerateAsync(string code, int quantity, CancellationToken ct = default) =>
            QueryAsync(new StoredProcedureCommand("dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES")
                .VarChar("@Code", code, 6)
                .Int("@Quantity", quantity), ct);

        private async Task<IEnumerable<InternalReferenceConsecutiveEntry>> QueryAsync(StoredProcedureCommand command, CancellationToken ct) =>
            await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<InternalReferenceConsecutiveEntry>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct), ct);
    }
}
