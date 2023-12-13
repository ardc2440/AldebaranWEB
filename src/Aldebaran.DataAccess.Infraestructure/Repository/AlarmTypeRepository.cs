namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmTypeRepository : IAlarmTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
