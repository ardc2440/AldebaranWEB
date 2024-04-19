namespace Aldebaran.Web.Utils
{
    public interface ITimerPreferenceService
    {
        List<DataTimer> Timers { get; }
        public DataTimer GetTimerPreferences(string key);
        public void UpdateTimerPreferences(string key, double timerMilliseconds);
    }
}