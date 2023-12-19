using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationDetailRepository : ICustomerReservationDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerReservationDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerReservationDetail>> GetAsync(int customerReservationId, CancellationToken ct = default)
        {
            return await _context.CustomerReservationDetails.AsNoTracking()
                .Include(i => i.ItemReference.Item.Line)
                .Where(i => i.CustomerReservationId.Equals(customerReservationId))
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int customerReservationDetailId, CustomerReservationDetail customerReservationDetail, CancellationToken ct = default)
        {
            var entity = _context.CustomerReservationDetails.FirstOrDefault(i => i.CustomerReservationDetailId.Equals(customerReservationDetailId)) ?? throw new KeyNotFoundException($"Orden con id {customerReservationDetailId} no existe.");

            entity.Brand = customerReservationDetail.Brand;
            entity.ReferenceId = customerReservationDetail.ReferenceId;
            entity.ReservedQuantity = customerReservationDetail.ReservedQuantity;
            entity.SendToCustomerOrder = customerReservationDetail.SendToCustomerOrder;

            await _context.SaveChangesAsync(ct);

        }
    }

}
