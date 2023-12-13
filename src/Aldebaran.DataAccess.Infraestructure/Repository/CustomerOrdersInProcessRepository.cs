namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrdersInProcessRepository : ICustomerOrdersInProcessRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrdersInProcessRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
