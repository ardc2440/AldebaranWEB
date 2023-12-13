namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class VisualizedAlarmRepository : IVisualizedAlarmRepository
    {
        private readonly AldebaranDbContext _context;
        public VisualizedAlarmRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
