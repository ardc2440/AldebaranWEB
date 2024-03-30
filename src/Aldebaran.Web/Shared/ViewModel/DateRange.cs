namespace Aldebaran.Web.Shared.ViewModel
{
    public class DateRange : ICloneable
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
