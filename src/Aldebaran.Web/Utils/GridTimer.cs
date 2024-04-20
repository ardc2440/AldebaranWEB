namespace Aldebaran.Web.Utils
{
    public class GridTimer : IDisposable
    {
        public string Key { get; }
        public System.Timers.Timer Timer { get; set; }
        public double SelectedTimer { get; set; }
        public bool IsLoading { get; set; } = false;
        public DateTime LastUpdate { get; set; }
        public GridTimer(string key)
        {
            Key = key;
            LastUpdate = DateTime.Now;
        }

        public async Task<GridTimer> InitializeTimer(DataTimer dataTimer, System.Timers.ElapsedEventHandler elapsedHandler)
        {
            SelectedTimer = dataTimer.Milliseconds;
            Timer = new System.Timers.Timer(dataTimer.Milliseconds);
            Timer.Elapsed += elapsedHandler;
            Timer.AutoReset = true;
            Timer.Enabled = true;
            return this;
        }
        public void UpdateTimerInterval(double interval)
        {
            Timer.Stop();
            Timer.Interval = interval;
            Timer.Start();
        }
        public void Dispose()
        {
            Timer?.Stop();
        }
    }
}