namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AldebaranDbContext _context;
        public EmployeeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
