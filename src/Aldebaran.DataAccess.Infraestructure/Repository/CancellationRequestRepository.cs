
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CancellationRequestRepository : RepositoryBase<AldebaranDbContext>, ICancellationRequestRepository
    {
        public CancellationRequestRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(CancellationRequest cancellationRequest, Reason reason, CancellationToken ct = default)
        {
            var reasonEntity = CreateInstance(cancellationRequest.DocumentType.DocumentTypeCode, reason, cancellationRequest.DocumentNumber);

            reasonEntity.EmployeeId = reason.EmployeeId;

            var cancellationRequestEntity = new CancellationRequest
            {
                DocumentTypeId = cancellationRequest.DocumentType.DocumentTypeId,
                DocumentNumber = cancellationRequest.DocumentNumber,
                RequestEmployeeId = cancellationRequest.RequestEmployeeId,
                StatusDocumentTypeId = cancellationRequest.StatusDocumentTypeId
            };

            await ExecuteCommandAsync(async dbContext =>
            {
                switch (cancellationRequest.DocumentType.DocumentTypeCode)
                {
                    case "C":
                        dbContext.CanceledPurchaseOrders.Add(reasonEntity);
                        break;
                    case "E":
                        dbContext.CanceledCustomerOrders.Add(reasonEntity);
                        break;
                    case "F":
                        dbContext.ClosedCustomerOrders.Add(reasonEntity);
                        break;
                    default:
                        throw new NotImplementedException("El código de documento no está implementado.");
                }

                await dbContext.CancellationRequests.AddAsync(cancellationRequestEntity, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task<CancellationRequest> UpdateAsync(CancellationRequest cancellationRequest, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CancellationRequests.FirstOrDefaultAsync(x => x.RequestId == cancellationRequest.RequestId, ct) ?? throw new KeyNotFoundException($"Solicitud con id {cancellationRequest.RequestId} no existe.");
                entity.StatusDocumentTypeId = cancellationRequest.StatusDocumentTypeId;
                entity.ResponseDate = DateTime.Now;
                entity.ResponseEmployeeId = cancellationRequest.ResponseEmployeeId;
                entity.ResponseReason  = cancellationRequest.ResponseReason;
                                
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }

                return entity;
            }, ct);
        }

        public async Task<bool> ExistsAnyPendingRequestAsync(short documentTypeId, int documentNumber, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                    {
                        return await dbContext.CancellationRequests
                            .Include(i => i.StatusDocumentType)
                            .AsNoTracking()
                            .AnyAsync(a => a.StatusDocumentType.StatusOrder == 1
                                   && a.DocumentNumber == documentNumber
                                   && a.DocumentTypeId == documentTypeId, ct);
                    }, ct);
        }

        private static dynamic CreateInstance(string documentTypeCode, Reason reason, int documentNumber)
        {
            return documentTypeCode switch
            {
                "C" => new CanceledPurchaseOrder { PurchaseOrderId = documentNumber, CancellationReasonId = reason.ReasonId, CancellationDate = DateTime.Now },
                "E" => new CanceledCustomerOrder { CustomerOrderId = documentNumber, CancellationReasonId = reason.ReasonId, CancellationDate = DateTime.Now },
                "F" => new ClosedCustomerOrder { CustomerOrderId = documentNumber, CloseCustomerOrderReasonId = reason.ReasonId, CloseDate = DateTime.Now },
                _ => throw new NotImplementedException()
            };
        }

        public async Task<(IEnumerable<CancellationRequestModel>,int)> GetAllByStatusOrderAsync(int skip, int top, string? searchKey, bool getPendingRequest, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var pendingRequest = new SqlParameter("@GetPendingRequest", getPendingRequest);
                var searchParam = new SqlParameter("@SearchKey", searchKey ?? (object)DBNull.Value);
                var skipParam = new SqlParameter("@Skip", skip);
                var topParam = new SqlParameter("@Top", top);
                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int);
                totalCountParam.Direction = ParameterDirection.Output;

                return (await dbContext.Set<CancellationRequestModel>()
                        .FromSqlRaw($"EXEC SP_GET_CANCELLATION_REQUESTS " +
                        $"@GetPendingRequest, @SEARCHKEY, @Skip, @Top, @TotalCount OUT",
                        pendingRequest, searchParam, skipParam, topParam, totalCountParam).ToListAsync(ct), (int)totalCountParam.Value);
                
            }, ct);
        }
    
        public async Task<CancellationRequest?> FindAsync(int requestId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CancellationRequests
                    .Include(i => i.StatusDocumentType)
                    .Include(i => i.DocumentType)
                    .Include(i => i.RequestEmployee)
                    .Include(i => i.ResponseEmployee)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.RequestId == requestId, ct);
            }, ct);
        } 
    }
}
