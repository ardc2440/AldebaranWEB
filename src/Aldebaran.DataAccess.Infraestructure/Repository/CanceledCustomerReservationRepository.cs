namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CanceledCustomerReservationRepository : ICanceledCustomerReservationRepository
    {
        private readonly AldebaranDbContext _context;
        public CanceledCustomerReservationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
