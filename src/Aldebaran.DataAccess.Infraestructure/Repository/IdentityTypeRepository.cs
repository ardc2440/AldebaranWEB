namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class IdentityTypeRepository : IIdentityTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public IdentityTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
