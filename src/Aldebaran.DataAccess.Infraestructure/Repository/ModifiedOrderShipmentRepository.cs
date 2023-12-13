namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModifiedOrderShipmentRepository : IModifiedOrderShipmentRepository
    {
        private readonly AldebaranDbContext _context;
        public ModifiedOrderShipmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
