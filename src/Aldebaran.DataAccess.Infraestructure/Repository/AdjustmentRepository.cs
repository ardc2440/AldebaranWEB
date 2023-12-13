namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
