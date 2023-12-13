namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PackagingRepository : IPackagingRepository
    {
        private readonly AldebaranDbContext _context;
        public PackagingRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
