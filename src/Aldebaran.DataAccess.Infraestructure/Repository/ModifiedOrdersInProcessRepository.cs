namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModifiedOrdersInProcessRepository : IModifiedOrdersInProcessRepository
    {
        private readonly AldebaranDbContext _context;
        public ModifiedOrdersInProcessRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
