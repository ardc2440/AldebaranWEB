using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<CustomerReservation>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservations.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .OrderBy(o => o.ReservationNumber)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<CustomerReservation>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservations.AsNoTracking()
                            .Include(i => i.Customer.City.Department.Country)
                            .Include(i => i.Customer.IdentityType)
                            .Include(i => i.StatusDocumentType.DocumentType)
                            .Include(i => i.Employee.IdentityType)
                            .Where(i => i.Notes.Contains(searchKey) ||
                                        dbContext.Format(i.CreationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        dbContext.Format(i.ExpirationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        dbContext.Format(i.ReservationDate, _SharedLocalizer["date:format"]).Contains(searchKey) ||
                                        i.ReservationNumber.Contains(searchKey) ||
                                        i.StatusDocumentType.StatusDocumentTypeName.Contains(searchKey) ||
                                        i.Employee.FullName.Contains(searchKey) ||
                                        i.Employee.DisplayName.Contains(searchKey) ||
                                        i.Employee.Area.AreaName.Contains(searchKey) ||
                                        i.Customer.City.Department.Country.CountryCode.Contains(searchKey) ||
                                        i.Customer.City.Department.Country.CountryName.Contains(searchKey) ||
                                        i.Customer.City.Department.DepartmentName.Contains(searchKey) ||
                                        i.Customer.City.CityName.Contains(searchKey) ||
                                        i.Customer.CustomerName.Contains(searchKey) ||
                                        i.Customer.CustomerAddress.Contains(searchKey) ||
                                        i.Customer.IdentityType.IdentityTypeCode.Contains(searchKey) ||
                                        i.Customer.IdentityType.IdentityTypeName.Contains(searchKey) ||
                                        i.Customer.IdentityNumber.Contains(searchKey))
                            .OrderBy(o => o.ReservationNumber)
                            .ToListAsync(ct);
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
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    if (reasonEntity != null)
                        dbContext.Entry(reasonEntity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.CustomerReservations.FirstOrDefaultAsync(x => x.CustomerReservationId == customerReservationId, ct) ?? throw new KeyNotFoundException($"Reserva con id {customerReservationId} no existe.");
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
                                where d.DocumentTypeCode.Equals("P") && a.DocumentId == entity.CustomerOrderId && a.IsActive == true
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
    }
}
