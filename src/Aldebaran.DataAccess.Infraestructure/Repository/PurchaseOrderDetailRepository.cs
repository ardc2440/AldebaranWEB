namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderDetailRepository : IPurchaseOrderDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
