namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class StatusDocumentTypeRepository : IStatusDocumentTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public StatusDocumentTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
