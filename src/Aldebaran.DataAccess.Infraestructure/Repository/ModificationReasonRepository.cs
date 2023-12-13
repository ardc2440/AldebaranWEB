namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModificationReasonRepository : IModificationReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public ModificationReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
