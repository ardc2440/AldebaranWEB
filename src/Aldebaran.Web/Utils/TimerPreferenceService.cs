using Aldebaran.Web.Models;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Aldebaran.Web.Utils
{
    public class TimerPreferenceService : ITimerPreferenceService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly SecurityService _security;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly AppSettings _settings;
        public List<DataTimer> Timers { get; private set; }

        public TimerPreferenceService(IMemoryCache cache, IOptions<AppSettings> settings, SecurityService securityService)
        {
            _memoryCache = cache ?? throw new ArgumentNullException(nameof(cache));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(IOptions<AppSettings>));
            _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = _settings.SlidingExpirationCache };
            _security = securityService ?? throw new ArgumentNullException(nameof(securityService));
            var refreshTimers = _settings.RefreshIntervalOptionsMinutes;
            Timers = refreshTimers.Select(minutes =>
            {
                double milliseconds = TimeSpan.FromMinutes(minutes).TotalMilliseconds;
                string description = TimeSpan.FromMinutes(minutes).Humanize();
                return new DataTimer { Milliseconds = milliseconds, Description = description };
            }).ToList();
        }

        public DataTimer GetTimerPreferences(string key)
        {
            var loggedUserCache = GetCacheKey(key);
            var min = Timers.MinBy(x => x.Milliseconds); // Por defecto sera el minimo tiempo posible de refresco
            if (!_memoryCache.TryGetValue(loggedUserCache, out DataTimer timer))
            {
                // No se encuentra en cache el tiempo por defecto
                _memoryCache.Set(loggedUserCache, min, _cacheEntryOptions);
                return min;
            }
            return timer ?? min;
        }
        public void UpdateTimerPreferences(string key, double timerMilliseconds)
        {
            var timer = Timers.SingleOrDefault(x => x.Milliseconds == timerMilliseconds) ?? throw new ArgumentNullException($"Timer not found for {timerMilliseconds} milliseconds");
            _memoryCache.Set(GetCacheKey(key), timer, _cacheEntryOptions);
        }

        string GetCacheKey(string key)
        {
            return $"{_security.User.Id}-{key}";
        }
    }
}