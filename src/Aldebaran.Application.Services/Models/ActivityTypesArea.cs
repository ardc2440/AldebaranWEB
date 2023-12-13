namespace Aldebaran.Application.Services.Models
{
    public class ActivityTypesArea
    {
        public short ActivityTypeId { get; set; }
        public short AreaId { get; set; }
        public required ActivityType ActivityType { get; set; }
        public required Area Area { get; set; }

    }
}
