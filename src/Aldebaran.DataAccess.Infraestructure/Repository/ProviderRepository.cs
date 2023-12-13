namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly AldebaranDbContext _context;
        public ProviderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
