namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModifiedCustomerOrderRepository : IModifiedCustomerOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public ModifiedCustomerOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
