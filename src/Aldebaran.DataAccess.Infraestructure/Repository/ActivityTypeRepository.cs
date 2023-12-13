namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ActivityTypeRepository : IActivityTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public ActivityTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
