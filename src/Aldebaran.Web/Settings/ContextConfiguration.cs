using Aldebaran.DataAccess;
using Aldebaran.Web.Models;
using Microsoft.Extensions.Options;

namespace Aldebaran.Web.Settings
{
    public class ContextConfiguration : IContextConfiguration
    {
        private readonly AppSettings _settings;
        //private readonly SecurityService Security;
        //protected IEmployeeService EmployeeService;
        protected IServiceProvider _serviceProvider;
        /// <summary>
        /// </summary>
        /// <param name="settings">Configuracion de la auditoria</param>
        /// <exception cref="ArgumentNullException">Cuando alguna inyeccion ha fallado</exception>
        //public ContextConfiguration(IOptions<AppSettings> settings, SecurityService security, IEmployeeService employeeService)
        //{
        //    _settings = settings?.Value ?? throw new ArgumentNullException(nameof(AppSettings));
        //    Security = security ?? throw new ArgumentNullException(nameof(SecurityService));
        //    EmployeeService = employeeService ?? throw new ArgumentNullException(nameof(EmployeeService));
        //}
        //public ContextConfiguration(IOptions<AppSettings> settings, SecurityService security)
        //{
        //    _settings = settings?.Value ?? throw new ArgumentNullException(nameof(AppSettings));
        //    Security = security ?? throw new ArgumentNullException(nameof(SecurityService));
        //}
        public ContextConfiguration(IOptions<AppSettings> settings, IServiceProvider serviceProvider)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(AppSettings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
        }
        public bool TrackEnabled => _settings.TrackEnabled;

        public string TrackUser
        {
            get
            {
                try
                {
                    //using (var scope = _serviceProvider.CreateScope())
                    //{
                    //    var securityService = scope.ServiceProvider.GetRequiredService<SecurityService>();
                    //    var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();
                    //    var employee = employeeService.FindByLoginUserIdAsync(securityService.User.Id).GetAwaiter().GetResult();
                    //    return employee.FullName;
                    //}
                    return "JLING";
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
