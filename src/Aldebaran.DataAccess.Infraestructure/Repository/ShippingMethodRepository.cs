namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShippingMethodRepository : IShippingMethodRepository
    {
        private readonly AldebaranDbContext _context;
        public ShippingMethodRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
