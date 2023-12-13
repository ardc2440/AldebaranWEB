namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderShipmentDetailRepository : ICustomerOrderShipmentDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderShipmentDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
