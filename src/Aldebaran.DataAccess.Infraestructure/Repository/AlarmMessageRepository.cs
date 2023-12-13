namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmMessageRepository : IAlarmMessageRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmMessageRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
