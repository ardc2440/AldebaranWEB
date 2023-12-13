namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class UsersAlarmTypeRepository : IUsersAlarmTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public UsersAlarmTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
