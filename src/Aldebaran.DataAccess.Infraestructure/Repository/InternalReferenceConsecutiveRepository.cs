using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class InternalReferenceConsecutiveRepository : RepositoryBase<AldebaranDbContext>, IInternalReferenceConsecutiveRepository
    {
        /// <summary>Largo del parámetro @Code de los SP (mayor que el código válido para que el SP valide y no se trunque).</summary>
        private const int CodeParameterLength = 10;

        /// <summary>Rango de THROW de reglas de negocio de los SP de consecutivos (código, cantidad, tope 9999).</summary>
        private const int FirstRuleError = 50100, LastRuleError = 50199;

        public InternalReferenceConsecutiveRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<IEnumerable<InternalReferenceConsecutiveEntry>> GetAvailableAsync(string code, CancellationToken ct = default) =>
            QueryAsync(new StoredProcedureCommand("dbo.SP_GET_AVAILABLE_INTERNAL_REFERENCE_CONSECUTIVES")
                .VarChar("@Code", code, CodeParameterLength), ct);

        /// <summary>El SP genera los consecutivos en una transacción con bloqueo por código y devuelve los generados.</summary>
        public Task<IEnumerable<InternalReferenceConsecutiveEntry>> GenerateAsync(string code, int quantity, CancellationToken ct = default) =>
            QueryAsync(new StoredProcedureCommand("dbo.SP_GENERATE_INTERNAL_REFERENCE_CONSECUTIVES")
                .VarChar("@Code", code, CodeParameterLength)
                .Int("@Quantity", quantity), ct);

        /// <summary>Referencias de artículos del código que no se toman como consecutivo, con el motivo.</summary>
        public async Task<IEnumerable<InternalReferenceNonconformingEntry>> GetNonconformingAsync(string code, CancellationToken ct = default)
        {
            var command = new StoredProcedureCommand("dbo.SP_GET_INTERNAL_REFERENCE_NONCONFORMING").VarChar("@Code", code, CodeParameterLength);
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.Set<InternalReferenceNonconformingEntry>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct), ct);
        }

        /// <summary>Las reglas que rechaza el SP (THROW 50100-50199) llegan como InvalidOperationException con el mensaje para el usuario.</summary>
        private async Task<IEnumerable<InternalReferenceConsecutiveEntry>> QueryAsync(StoredProcedureCommand command, CancellationToken ct)
        {
            try
            {
                return await ExecuteQueryAsync(async dbContext =>
                    await dbContext.Set<InternalReferenceConsecutiveEntry>().FromSqlRaw(command.Sql, command.Parameters).AsNoTracking().ToListAsync(ct), ct);
            }
            catch (SqlException ex) when (ex.Number >= FirstRuleError && ex.Number <= LastRuleError)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}
