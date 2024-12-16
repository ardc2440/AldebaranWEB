namespace Aldebaran.DataAccess.Entities
{
    public class ActivityTypesArea
    {
        public short ActivityTypeId { get; set; }
        public short AreaId { get; set; }
        public ActivityType? ActivityType { get; set; }
        public Area? Area { get; set; }
    }
}
