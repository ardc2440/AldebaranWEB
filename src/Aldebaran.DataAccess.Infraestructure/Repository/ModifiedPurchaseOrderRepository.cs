namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModifiedPurchaseOrderRepository : IModifiedPurchaseOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public ModifiedPurchaseOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
