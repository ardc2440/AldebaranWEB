
using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                dynamic? entityToRemove = null;
                var entity = await dbContext.CancellationRequests.FirstOrDefaultAsync(x => x.RequestId == cancellationRequest.RequestId, ct) ?? throw new KeyNotFoundException($"Solicitud con id {cancellationRequest.RequestId} no existe.");
                entity.StatusDocumentTypeId = cancellationRequest.StatusDocumentTypeId;
                entity.ResponseDate = DateTime.Now;
                entity.ResponseEmployeeId = cancellationRequest.ResponseEmployeeId;
                entity.ResponseReason = cancellationRequest.ResponseReason;

                if (cancellationRequest.StatusDocumentType.StatusOrder == 3)
                {
                    entityToRemove = await GetInstance(cancellationRequest.DocumentType.DocumentTypeCode, cancellationRequest.DocumentNumber, false, ct) ?? throw new NotImplementedException($"No se encontreo el documento con número {cancellationRequest.DocumentNumber}");

                    if (dbContext.Entry(entityToRemove).State == EntityState.Detached)
                    {
                        dbContext.Attach(entityToRemove);  // Adjunta la entidad al contexto
                    }

                    switch (cancellationRequest.DocumentType.DocumentTypeCode)
                    {
                        case "C":
                            dbContext.CanceledPurchaseOrders.Remove(entityToRemove);
                            break;
                        case "E":
                            dbContext.CanceledCustomerOrders.Remove(entityToRemove);
                            break;
                        case "F":
                            dbContext.ClosedCustomerOrders.Remove(entityToRemove);
                            break;
                        default:
                            throw new NotImplementedException("El código de documento no está implementado.");
                    }

                    if (dbContext.Entry(entityToRemove).State != EntityState.Deleted)  // Debería ser EntityState.Modified
                        dbContext.Entry(entityToRemove).State = EntityState.Deleted;
                }

                if (cancellationRequest.StatusDocumentType.StatusOrder == 2)
                {
                    entityToRemove = await GetInstance(cancellationRequest.DocumentType.DocumentTypeCode, cancellationRequest.DocumentNumber, true, ct) ?? throw new NotImplementedException($"No se encontreo el documento con número {cancellationRequest.DocumentNumber}");
                    
                    if (dbContext.Entry(entityToRemove).State == EntityState.Detached)
                    {
                        dbContext.Attach(entityToRemove);  // Adjunta la entidad al contexto
                    }

                    switch (cancellationRequest.DocumentType.DocumentTypeCode)
                    {
                        case "C":
                            var documentStatus = 
                            ((PurchaseOrder)entityToRemove).StatusDocumentTypeId = (await dbContext.StatusDocumentTypes
                                                                                        .Include(i=>i.DocumentType)
                                                                                        .FirstOrDefaultAsync(f => f.DocumentType.DocumentTypeCode.Equals("O") && f.StatusOrder == 3))
                                                                                        !.StatusDocumentTypeId;
                            break;
                        case "E":
                            ((CustomerOrder)entityToRemove).StatusDocumentTypeId = (await dbContext.StatusDocumentTypes
                                                                                        .Include(i => i.DocumentType)
                                                                                        .FirstOrDefaultAsync(f => f.DocumentType.DocumentTypeCode.Equals("P") && f.StatusOrder == 6))
                                                                                        !.StatusDocumentTypeId; 
                            break;
                        case "F":
                            ((CustomerOrder)entityToRemove).StatusDocumentTypeId = (await dbContext.StatusDocumentTypes
                                                                                        .Include(i => i.DocumentType)
                                                                                        .FirstOrDefaultAsync(f => f.DocumentType.DocumentTypeCode.Equals("P") && f.StatusOrder == 5))
                                                                                        !.StatusDocumentTypeId; 
                            break;
                        default:
                            throw new NotImplementedException("El código de documento no está implementado.");
                    }
                    if (dbContext.Entry(entityToRemove).State != EntityState.Modified)  // Debería ser EntityState.Modified
                        dbContext.Entry(entityToRemove).State = EntityState.Modified;
                }
                try
                {   
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    if (entityToRemove != null)
                        dbContext.Entry(entityToRemove).State = EntityState.Unchanged;
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

        private async Task<dynamic> GetInstance(string documentTypeCode, int documentNumber, bool originalDocument, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                dynamic result = documentTypeCode switch
                {
                    "C" => !originalDocument ? await dbContext.CanceledPurchaseOrders.FirstOrDefaultAsync(f => f.PurchaseOrderId == documentNumber, ct) ?? throw new KeyNotFoundException($"Motivo de cancelación de orden de compra con id {documentNumber} no existe.") :
                                                                         await dbContext.PurchaseOrders.Include(i => i.PurchaseOrderDetails).FirstOrDefaultAsync(f => f.PurchaseOrderId == documentNumber, ct) ?? throw new KeyNotFoundException($"Orden de compra con id {documentNumber} no existe."),
                    "E" => !originalDocument ? await dbContext.CanceledCustomerOrders.FirstOrDefaultAsync(f => f.CustomerOrderId == documentNumber, ct) ?? throw new KeyNotFoundException($"Motivo de cancelación de pedido con id {documentNumber} no existe.") :
                                                                         await dbContext.CustomerOrders.Include(i => i.CustomerOrderDetails).FirstOrDefaultAsync(f => f.CustomerOrderId == documentNumber, ct) ?? throw new KeyNotFoundException($"Pedido con id {documentNumber} no existe."),
                    "F" => !originalDocument ? await dbContext.ClosedCustomerOrders.FirstOrDefaultAsync(f => f.CustomerOrderId == documentNumber, ct) ?? throw new KeyNotFoundException($"motivo de cierre de pedido con id {documentNumber} no existe.") :
                                                                         await dbContext.CustomerOrders.Include(i => i.CustomerOrderDetails).FirstOrDefaultAsync(f => f.CustomerOrderId == documentNumber, ct) ?? throw new KeyNotFoundException($"Pedido con id {documentNumber} no existe."),
                    _ => throw new ArgumentException($"El código de documento '{documentTypeCode}' no está implementado o es inválido.", nameof(documentTypeCode)),
                };
                return result ?? throw new InvalidOperationException($"No se encontró el documento con el código '{documentTypeCode}' y el número de documento '{documentNumber}'.");
            }, ct);
        }

        private static dynamic CreateInstance(string documentTypeCode, Reason reason, int documentNumber)
        {
            return documentTypeCode switch
            {
                "C" => new CanceledPurchaseOrder { PurchaseOrderId = documentNumber, CancellationReasonId = reason.ReasonId, CancellationDate = DateTime.Now },
                "E" => new CanceledCustomerOrder { CustomerOrderId = documentNumber, CancellationReasonId = reason.ReasonId, CancellationDate = DateTime.Now },
                "F" => new ClosedCustomerOrder { CustomerOrderId = documentNumber, CloseCustomerOrderReasonId = reason.ReasonId, CloseDate = DateTime.Now },
                _ => throw new NotImplementedException("El código de documento no está implementado.")
            };
        }

        public async Task<(IEnumerable<CancellationRequestModel>, int)> GetAllByStatusOrderAsync(int skip, int top, string? searchKey, string type, bool getPendingRequest, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var pendingRequest = new SqlParameter("@GetPendingRequest", getPendingRequest);
                var searchParam = new SqlParameter("@SearchKey", searchKey ?? (object)DBNull.Value);
                var skipParam = new SqlParameter("@Skip", skip);
                var topParam = new SqlParameter("@Top", top);
                var typeParam = new SqlParameter("@Type", type);
                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                return (await dbContext.Set<CancellationRequestModel>()
                        .FromSqlRaw($"EXEC SP_GET_CANCELLATION_REQUESTS " +
                        $"@GetPendingRequest, @SEARCHKEY, @Skip, @Top, @Type, @TotalCount OUT",
                        pendingRequest, searchParam, skipParam, topParam, typeParam, totalCountParam).ToListAsync(ct), (int)totalCountParam.Value);

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
