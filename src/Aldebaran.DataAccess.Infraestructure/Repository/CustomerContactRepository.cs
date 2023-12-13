namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerContactRepository : ICustomerContactRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerContactRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
