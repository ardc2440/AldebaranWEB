namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerReservationRepository : ICustomerReservationRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerReservationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
