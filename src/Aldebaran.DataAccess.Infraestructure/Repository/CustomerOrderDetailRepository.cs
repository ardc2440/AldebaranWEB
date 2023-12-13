namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderDetailRepository : ICustomerOrderDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
