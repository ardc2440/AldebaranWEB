namespace Aldebaran.Application.Services.Models
{
    public class Area
    {
        public short AreaId { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }
        // Reverse navigation
        public ICollection<ActivityTypesArea> ActivityTypesAreas { get; set; }
        public ICollection<CustomerOrderActivity> CustomerOrderActivities { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<ItemsArea> ItemsAreas { get; set; }
        public Area()
        {
            ActivityTypesAreas = new List<ActivityTypesArea>();
            CustomerOrderActivities = new List<CustomerOrderActivity>();
            Employees = new List<Employee>();
            ItemsAreas = new List<ItemsArea>();
        }

    }
}
