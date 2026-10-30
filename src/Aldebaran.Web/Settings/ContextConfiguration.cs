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

        /// <summary>
        /// Usuario registrado en el Track.
        /// Sin usuario autenticado (callbacks del notificador, enlace del correo de mínimos, procesos en segundo plano)
        /// el cambio lo hace el sistema: se registra <see cref="SystemUser"/> sin escribir un error en el log.
        /// </summary>
        public string TrackUser
        {
            get
            {
                try
                {
                    var identity = _httpContextAccessor.HttpContext?.User?.Identity;

                    if (identity == null || !identity.IsAuthenticated || string.IsNullOrWhiteSpace(identity.Name))
                        return SystemUser;

                    return identity.Name;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ContextConfiguration.TrackUser");
                    return "Unknown";
                }
            }
        }

        public const string SystemUser = "Sistema";
    }
}
