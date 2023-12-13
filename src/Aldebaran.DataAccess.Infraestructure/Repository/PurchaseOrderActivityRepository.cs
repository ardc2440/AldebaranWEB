namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderActivityRepository : IPurchaseOrderActivityRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderActivityRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
