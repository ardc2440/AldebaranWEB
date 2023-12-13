namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemReferenceRepository : IItemReferenceRepository
    {
        private readonly AldebaranDbContext _context;
        public ItemReferenceRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
