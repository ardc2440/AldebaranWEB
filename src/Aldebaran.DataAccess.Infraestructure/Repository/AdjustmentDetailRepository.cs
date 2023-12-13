namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentDetailRepository : IAdjustmentDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
