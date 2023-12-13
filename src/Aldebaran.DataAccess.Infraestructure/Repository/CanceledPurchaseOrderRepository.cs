namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CanceledPurchaseOrderRepository : ICanceledPurchaseOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public CanceledPurchaseOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
