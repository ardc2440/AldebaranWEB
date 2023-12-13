namespace Aldebaran.DataAccess.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CountryId { get; set; }
        // Reverse navigation
        public ICollection<City> Cities { get; set; }
        public Country Country { get; set; }
        public Department()
        {
            Cities = new List<City>();
        }
    }
}
