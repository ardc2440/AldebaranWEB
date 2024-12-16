namespace Aldebaran.DataAccess.Entities
{
    // ****************************************************************************************************
    // This is not a commercial licence, therefore only a few tables/views/stored procedures are generated.
    // ****************************************************************************************************
    public class ActivityType
    {
        public short ActivityTypeId { get; set; }
        public string? ActivityTypeName { get; set; }
        // Reverse navigation
        public ICollection<ActivityTypesArea> ActivityTypesAreas { get; set; }
        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }
        public ActivityType()
        {
            ActivityTypesAreas = new List<ActivityTypesArea>();
            CustomerOrderActivityDetails = new List<CustomerOrderActivityDetail>();
        }
    }
}
