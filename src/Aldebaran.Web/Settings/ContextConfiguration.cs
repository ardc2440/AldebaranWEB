using Aldebaran.DataAccess;
using Aldebaran.Web.Models;
using Microsoft.Extensions.Options;

namespace Aldebaran.Web.Settings
{
    public class ContextConfiguration : IContextConfiguration
    {
        private readonly AppSettings _settings;
        protected ILogger<ContextConfiguration> _logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContextConfiguration(IOptions<AppSettings> settings, ILogger<ContextConfiguration> logger, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(AppSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<ContextConfiguration>));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(IHttpContextAccessor));

        }
        public bool TrackEnabled => _settings.TrackEnabled;

        public string TrackUser
        {
            get
            {
                try
                {
                    if (_httpContextAccessor == null)
                        throw new Exception("El acceso al contexto HTTP esta vacio");
                    if (_httpContextAccessor.HttpContext == null)
                        throw new Exception("El contexto HTTP esta vacio");

                    var user = _httpContextAccessor.HttpContext?.User;

                    if (user == null)
                        throw new Exception("El usuario esta vacio");

                    if (!user.Identity.IsAuthenticated)
                        throw new Exception("El usuario no esta autenticado");

                    if (user.Identity == null)
                        throw new Exception("La identidad del usuario esta vacia");

                    if (user.Identity.Name == string.Empty)
                        throw new Exception("El nombre del usuario esta vacio");

                    return user.Identity.Name;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ContextConfiguration.TrackUser");
                    return "Unknown";
                }
            }
        }
    }
}
