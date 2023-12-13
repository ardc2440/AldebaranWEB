namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
