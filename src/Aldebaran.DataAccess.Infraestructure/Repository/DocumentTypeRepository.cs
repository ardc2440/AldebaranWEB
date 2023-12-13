namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public DocumentTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
