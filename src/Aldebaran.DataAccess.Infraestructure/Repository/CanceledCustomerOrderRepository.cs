namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CanceledCustomerOrderRepository : ICanceledCustomerOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public CanceledCustomerOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
