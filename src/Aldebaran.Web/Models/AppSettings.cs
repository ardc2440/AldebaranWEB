namespace Aldebaran.Web.Models
{
    public class AppSettings
    {
        public int[] RefreshIntervalOptionsMinutes { get; set; }
        public TimeSpan SlidingExpirationCache { get; set; }
        public int DefaultIntervalOption { get; set; }
        public int PurchaseOrderWhiteFlag { get; set; }
        public int PurchaseOrderYellowFlag { get; set; }
        public int PurchaseOrderRedFlag { get; set; }
        public bool TrackEnabled { get; set; }
        public int VariationMonthNumber { get; set; }
        public string ImageRepositoryPath { get; set; }
        public int ProcessSatelliteId { get; set; }
    }
}