namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ClosedCustomerOrderRepository : IClosedCustomerOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public ClosedCustomerOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
