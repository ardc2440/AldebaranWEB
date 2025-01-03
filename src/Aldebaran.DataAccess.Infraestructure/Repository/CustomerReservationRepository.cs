using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationRepository : RepositoryBase<AldebaranDbContext>, ICustomerReservationRepository
    {
        private readonly ISharedStringLocalizer _SharedLocalizer;
        public CustomerReservationRepository(IServiceProvider serviceProvider, ISharedStringLocalizer sharedLocalizer) : base(serviceProvider)
        {
            _SharedLocalizer = sharedLocalizer ?? throw new ArgumentNullException(nameof(ISharedStringLocalizer));
        }

        public async Task<CustomerReservation> AddAsync(CustomerReservation customerReservation, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                var entity = new CustomerReservation
                {
                    EmployeeId = customerReservation.EmployeeId,
                    ReservationDate = customerReservation.ReservationDate,
                    ExpirationDate = customerReservation.ExpirationDate,
                    Notes = customerReservation.Notes,
                    StatusDocumentTypeId = customerReservation.StatusDocumentTypeId,
                    CustomerId = customerReservation.CustomerId,
                    CustomerReservationDetails = new List<CustomerReservationDetail>()
                };

                foreach (var item in customerReservation.CustomerReservationDetails)
                {
                    entity.CustomerReservationDetails.Add(new CustomerReservationDetail
                    {
                        Brand = item.Brand,
                        ReferenceId = item.ReferenceId,
                        ReservedQuantity = item.ReservedQuantity,
                        SendToCustomerOrder = item.SendToCustomerOrder
                    });
                }

                try
                {
                    await dbContext.CustomerReservations.AddAsync(entity, ct);
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

        public async Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.CustomerReservations.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .OrderByDescending(o => o.ReservationNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.CustomerReservations.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .Where(i => i.Notes.Contains(searchKey) ||
                                        dbContext.Format(i.ExpirationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        dbContext.Format(i.ReservationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        i.ReservationNumber.Contains(searchKey) ||
                                        i.StatusDocumentType.StatusDocumentTypeName.Contains(searchKey) ||
                                        i.Customer.CustomerName.Contains(searchKey))
                            .OrderByDescending(o => o.ReservationNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservations.AsNoTracking()
               .Include(i => i.Customer.City.Department.Country)
               .Include(i => i.Customer.IdentityType)
               .Include(i => i.StatusDocumentType.DocumentType)
               .Include(i => i.Employee.IdentityType)
               .Include(i => i.CustomerReservationDetails)
               .FirstOrDefaultAsync(i => i.CustomerReservationId == customerReservationId, ct);
            }, ct);
        }

        public async Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, Reason? reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerReservations.Include(i => i.CustomerReservationDetails).FirstOrDefaultAsync(x => x.CustomerReservationId == customerReservationId, ct) ?? throw new KeyNotFoundException($"Reserva con id {customerReservationId} no existe.");
                entity.ExpirationDate = customerReservation.ExpirationDate;
                entity.ReservationDate = customerReservation.ReservationDate;
                entity.CustomerId = customerReservation.CustomerId;
                entity.Notes = customerReservation.Notes;
                entity.StatusDocumentTypeId = customerReservation.StatusDocumentTypeId;
                entity.CustomerOrderId = customerReservation.CustomerOrderId;

                var detailDeleted = new List<CustomerReservationDetail>();

                foreach (var item in entity.CustomerReservationDetails)
                    if (!customerReservation.CustomerReservationDetails.Any(i => i.CustomerReservationDetailId == item.CustomerReservationDetailId))
                        dbContext.CustomerReservationDetails.Remove(item);

                foreach (var item in customerReservation.CustomerReservationDetails)
                {
                    if (item.CustomerReservationDetailId > 0)
                    {
                        var detail = entity.CustomerReservationDetails.FirstOrDefault(i => i.CustomerReservationDetailId == item.CustomerReservationDetailId);

                        if (detail != null)
                        {
                            detail.Brand = item.Brand;
                            detail.ReferenceId = item.ReferenceId;
                            detail.ReservedQuantity = item.ReservedQuantity;
                            detail.SendToCustomerOrder = item.SendToCustomerOrder;
                        }
                        continue;
                    }

                    entity.CustomerReservationDetails.Add(new CustomerReservationDetail()
                    {
                        Brand = item.Brand,
                        CustomerReservationId = item.CustomerReservationDetailId,
                        ReferenceId = item.ReferenceId,
                        ReservedQuantity = item.ReservedQuantity,
                        SendToCustomerOrder = item.SendToCustomerOrder
                    });
                }

                ModifiedCustomerReservation? reasonEntity = null;
                if (reason != null)
                {
                    reasonEntity = new ModifiedCustomerReservation
                    {
                        CustomerReservationId = customerReservationId,
                        ModificationReasonId = reason.ReasonId,
                        EmployeeId = reason.EmployeeId,
                        ModificationDate = reason.Date
                    };
                    dbContext.ModifiedCustomerReservations.Add(reasonEntity);
                }

                var alarms = new List<Alarm>();
                var statusDocument = await dbContext.StatusDocumentTypes.AsNoTracking().FirstAsync(f => f.StatusDocumentTypeId == customerReservation.StatusDocumentTypeId);

                if (statusDocument.StatusOrder == 2)
                {
                    alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("R") && a.DocumentId == entity.CustomerReservationId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                    foreach (var alarm in alarms) alarm.IsActive = false;
                }

                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    if (reasonEntity != null)
                        dbContext.Entry(reasonEntity).State = EntityState.Unchanged;

                    if (statusDocument.StatusOrder == 2)
                        foreach (var alarm in alarms)
                        {
                            dbContext.Entry(alarm).State = EntityState.Unchanged;
                        }
                    throw;
                }
            }, ct);
        }

        public async Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerReservations.Include(i=>i.CustomerReservationDetails).FirstOrDefaultAsync(x => x.CustomerReservationId == customerReservationId, ct) ?? throw new KeyNotFoundException($"Reserva con id {customerReservationId} no existe.");
                entity.StatusDocumentTypeId = canceledStatusDocumentId;

                var reasonEntity = new CanceledCustomerReservation
                {
                    CustomerReservationId = customerReservationId,
                    CancellationReasonId = reason.ReasonId,
                    EmployeeId = reason.EmployeeId,
                    CancellationDate = reason.Date
                };

                var alarms = await (from a in dbContext.Alarms
                                    join b in dbContext.AlarmMessages on a.AlarmMessageId equals b.AlarmMessageId
                                    join c in dbContext.AlarmTypes on b.AlarmTypeId equals c.AlarmTypeId
                                    join d in dbContext.DocumentTypes on c.DocumentTypeId equals d.DocumentTypeId
                                    where d.DocumentTypeCode.Equals("R") && a.DocumentId == entity.CustomerReservationId && a.IsActive == true
                                    select (Alarm)a).ToListAsync();

                foreach (var alarm in alarms) alarm.IsActive = false;

                try
                {
                    dbContext.CanceledCustomerReservations.Add(reasonEntity);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    foreach (var alarm in alarms)
                    {
                        dbContext.Entry(alarm).State = EntityState.Unchanged;
                    }

                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<(IEnumerable<CustomerReservation> customerReservations, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var query = dbContext.CustomerReservations.AsNoTracking()
                        .Include(i => i.Customer.City.Department.Country)
                        .Include(i => i.Customer.IdentityType)
                        .Include(i => i.StatusDocumentType.DocumentType)
                        .Include(i => i.Employee.IdentityType)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }
                var count = query.Count();
                var data = await query.Skip(skip).Take(take).ToListAsync(ct);
                return (data, count);
            }, ct);
        }

        /* Logs */
        public async Task<(IEnumerable<ModifiedCustomerReservation>, int count)> GetCustomerReservationModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.ModifiedCustomerReservations.AsNoTracking()
                            .Include(i => i.CustomerReservation.Customer)
                            .Include(i => i.CustomerReservation.Employee)
                            .Include(i => i.Employee)
                            .Include(i => i.ModificationReason)
                            .Where(i => (i.CustomerReservation.ReservationNumber.Contains(searchKey) ||
                                         i.CustomerReservation.Customer.CustomerName.Contains(searchKey) ||
                                         i.CustomerReservation.Customer.IdentityNumber.Contains(searchKey) ||
                                         i.Employee.FullName.Contains(searchKey) ||
                                         i.ModificationReason.ModificationReasonName.Contains(searchKey) ||
                                         dbContext.Format(i.ModificationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.CustomerReservation.ReservationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.CustomerReservation.ExpirationDate, _SharedLocalizer["date:format"]).Contains(searchKey))
                                         || searchKey.IsNullOrEmpty())
                            .OrderByDescending(o => o.CustomerReservation.ReservationNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<CanceledCustomerReservation>, int count)> GetCustomerReservationCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.CanceledCustomerReservations.AsNoTracking()
                            .Include(i => i.CustomerReservation.Customer)
                            .Include(i => i.CustomerReservation.Employee)
                            .Include(i => i.Employee)
                            .Include(i => i.CancellationReason)
                            .Where(i => (i.CustomerReservation.ReservationNumber.Contains(searchKey) ||
                                         i.CustomerReservation.Customer.CustomerName.Contains(searchKey) ||
                                         i.CustomerReservation.Customer.IdentityNumber.Contains(searchKey) ||
                                         i.Employee.FullName.Contains(searchKey) ||
                                         i.CancellationReason.CancellationReasonName.Contains(searchKey) ||
                                         dbContext.Format(i.CancellationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.CustomerReservation.ReservationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                         dbContext.Format(i.CustomerReservation.ExpirationDate, _SharedLocalizer["date:format"]).Contains(searchKey))
                                         || searchKey.IsNullOrEmpty())
                            .OrderByDescending(o => o.CustomerReservation.ReservationNumber);

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }              
    }
}
