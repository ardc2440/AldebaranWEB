namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProcessSatelliteRepository : IProcessSatelliteRepository
    {
        private readonly AldebaranDbContext _context;
        public ProcessSatelliteRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
