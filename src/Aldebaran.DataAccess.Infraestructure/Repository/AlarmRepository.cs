namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmRepository : IAlarmRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
