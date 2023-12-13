namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AdjustmentTypeRepository : IAdjustmentTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public AdjustmentTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
