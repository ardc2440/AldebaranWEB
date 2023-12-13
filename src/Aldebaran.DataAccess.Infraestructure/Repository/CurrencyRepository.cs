namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AldebaranDbContext _context;
        public CurrencyRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
