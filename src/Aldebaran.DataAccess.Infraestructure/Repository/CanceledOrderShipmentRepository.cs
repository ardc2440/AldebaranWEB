namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CanceledOrderShipmentRepository : ICanceledOrderShipmentRepository
    {
        private readonly AldebaranDbContext _context;
        public CanceledOrderShipmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
