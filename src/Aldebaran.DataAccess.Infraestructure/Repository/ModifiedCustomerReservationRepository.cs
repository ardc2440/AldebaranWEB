namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModifiedCustomerReservationRepository : IModifiedCustomerReservationRepository
    {
        private readonly AldebaranDbContext _context;
        public ModifiedCustomerReservationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
