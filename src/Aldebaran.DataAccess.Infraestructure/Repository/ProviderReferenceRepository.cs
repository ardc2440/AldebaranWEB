namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderReferenceRepository : IProviderReferenceRepository
    {
        private readonly AldebaranDbContext _context;
        public ProviderReferenceRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
