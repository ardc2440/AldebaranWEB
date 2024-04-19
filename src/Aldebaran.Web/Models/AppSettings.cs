namespace Aldebaran.Web.Models
{
    public class AppSettings
    {
        public int[] RefreshIntervalOptionsMinutes { get; set; }
        public TimeSpan SlidingExpirationCache { get; set; }
    }
}