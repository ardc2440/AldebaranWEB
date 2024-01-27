using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationRepository : ICustomerReservationRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerReservationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CustomerReservation> AddAsync(CustomerReservation customerReservation, CancellationToken ct = default)
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
                await _context.CustomerReservations.AddAsync(entity, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }

            return entity;
        }

        public async Task<IEnumerable<CustomerReservation>> GetAsync(CancellationToken ct = default)
        {
            return await _context.CustomerReservations.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<CustomerReservation>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.CustomerReservations.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .Where(i => i.Notes.Contains(searchKey) ||
                            i.ReservationDate.ToString().Contains(searchKey) ||
                            i.CreationDate.ToString().Contains(searchKey) ||
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
                            i.Customer.Email1.Contains(searchKey) ||
                            i.Customer.Email2.Contains(searchKey) ||
                            i.Customer.Email3.Contains(searchKey) ||
                            i.Customer.CustomerAddress.Contains(searchKey) ||
                            i.Customer.Phone1.Contains(searchKey) ||
                            i.Customer.Phone2.Contains(searchKey) ||
                            i.Customer.Fax.Contains(searchKey) ||
                            i.Customer.IdentityType.IdentityTypeCode.Contains(searchKey) ||
                            i.Customer.IdentityType.IdentityTypeName.Contains(searchKey) ||
                            i.Customer.IdentityNumber.Contains(searchKey))
                .ToListAsync(ct);
        }

        public async Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default)
        {
            return await _context.CustomerReservations.AsNoTracking()
                .Include(i => i.Customer.City.Department.Country)
                .Include(i => i.Customer.IdentityType)
                .Include(i => i.StatusDocumentType.DocumentType)
                .Include(i => i.Employee.IdentityType)
                .Include(i => i.CustomerReservationDetails)
                .FirstOrDefaultAsync(i => i.CustomerReservationId == customerReservationId, ct);
        }

        public async Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, CancellationToken ct = default)
        {
            var entity = await _context.CustomerReservations.Include(i => i.CustomerReservationDetails).FirstOrDefaultAsync(x => x.CustomerReservationId == customerReservationId, ct) ?? throw new KeyNotFoundException($"Reserva con id {customerReservationId} no existe.");
            entity.ExpirationDate = customerReservation.ExpirationDate;
            entity.ReservationDate = customerReservation.ReservationDate;
            entity.CustomerId = customerReservation.CustomerId;
            entity.Notes = customerReservation.Notes;
            entity.StatusDocumentTypeId = customerReservation.StatusDocumentTypeId;
            entity.CustomerOrderId = customerReservation.CustomerOrderId;

            var detailDeleted = new List<CustomerReservationDetail>();

            foreach (var item in entity.CustomerReservationDetails)
                if (!customerReservation.CustomerReservationDetails.Any(i => i.CustomerReservationDetailId == item.CustomerReservationDetailId))
                    _context.CustomerReservationDetails.Remove(item);

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

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, CancellationToken ct = default)
        {
            var entity = await _context.CustomerReservations.FirstOrDefaultAsync(x => x.CustomerReservationId == customerReservationId, ct) ?? throw new KeyNotFoundException($"Reserva con id {customerReservationId} no existe.");

            entity.StatusDocumentTypeId = canceledStatusDocumentId;

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }
}
