namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationDetailRepository : ICustomerReservationDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerReservationDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
