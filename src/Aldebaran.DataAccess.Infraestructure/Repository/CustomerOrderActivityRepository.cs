namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderActivityRepository : ICustomerOrderActivityRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderActivityRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
