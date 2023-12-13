namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ReferencesWarehouseRepository : IReferencesWarehouseRepository
    {
        private readonly AldebaranDbContext _context;
        public ReferencesWarehouseRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
