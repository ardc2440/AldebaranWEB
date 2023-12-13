namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CustomerOrderShipmentRepository : ICustomerOrderShipmentRepository
    {
        private readonly AldebaranDbContext _context;
        public CustomerOrderShipmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
