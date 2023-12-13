namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class LineRepository : ILineRepository
    {
        private readonly AldebaranDbContext _context;
        public LineRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
