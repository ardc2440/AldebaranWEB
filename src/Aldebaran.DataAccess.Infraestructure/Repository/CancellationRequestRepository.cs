
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;

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
    }
}
