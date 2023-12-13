namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CanceledOrdersInProcessRepository : ICanceledOrdersInProcessRepository
    {
        private readonly AldebaranDbContext _context;
        public CanceledOrdersInProcessRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
