namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CancellationReasonRepository : ICancellationReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public CancellationReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
