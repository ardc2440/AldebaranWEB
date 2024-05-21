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
                    var user = _httpContextAccessor.HttpContext?.User;
                    if (user == null || !user.Identity.IsAuthenticated)
                        throw new Exception("No se ha podido obtener el usuario autenticado para la auditoria");
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
