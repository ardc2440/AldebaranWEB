namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderActivityDetailRepository : ICustomerOrderActivityDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderActivityDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
