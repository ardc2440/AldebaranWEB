namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ActivityTypesAreaRepository : IActivityTypesAreaRepository
    {
        private readonly AldebaranDbContext _context;
        public ActivityTypesAreaRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
