namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentReasonRepository : IAdjustmentReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
