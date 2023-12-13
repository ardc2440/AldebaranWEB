namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AldebaranDbContext _context;
        public WarehouseRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
