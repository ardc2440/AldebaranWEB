namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
