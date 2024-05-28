using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationDetailRepository : RepositoryBase<AldebaranDbContext>, ICustomerReservationDetailRepository
    {
        public CustomerReservationDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<CustomerReservationDetail>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.CustomerReservationDetails.AsNoTracking()
                            .Include(i => i.ItemReference.Item.Line)
                            .Where(i => i.CustomerReservationId == customerReservationId)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int customerReservationDetailId, CustomerReservationDetail customerReservationDetail, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = dbContext.CustomerReservationDetails.FirstOrDefault(i => i.CustomerReservationDetailId == customerReservationDetailId) ?? throw new KeyNotFoundException($"Detalle de Reserva con id {customerReservationDetailId} no existe.");

                entity.Brand = customerReservationDetail.Brand;
                entity.ReferenceId = customerReservationDetail.ReferenceId;
                entity.ReservedQuantity = customerReservationDetail.ReservedQuantity;
                entity.SendToCustomerOrder = customerReservationDetail.SendToCustomerOrder;

                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
    }

}
