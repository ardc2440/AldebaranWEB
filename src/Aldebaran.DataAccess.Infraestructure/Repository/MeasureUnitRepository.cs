namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class MeasureUnitRepository : IMeasureUnitRepository
    {
        private readonly AldebaranDbContext _context;
        public MeasureUnitRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
