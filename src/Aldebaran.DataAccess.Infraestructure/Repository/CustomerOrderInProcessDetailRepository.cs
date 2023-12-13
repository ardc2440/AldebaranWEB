namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderInProcessDetailRepository : ICustomerOrderInProcessDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderInProcessDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
