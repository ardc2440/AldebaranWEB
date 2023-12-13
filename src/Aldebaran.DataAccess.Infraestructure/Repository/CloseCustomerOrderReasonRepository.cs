namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CloseCustomerOrderReasonRepository : ICloseCustomerOrderReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public CloseCustomerOrderReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

}
