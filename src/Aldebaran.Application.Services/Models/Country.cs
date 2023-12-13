namespace Aldebaran.Application.Services.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        // Reverse navigation
        public ICollection<Department> Departments { get; set; }
        public Country()
        {
            Departments = new List<Department>();
        }

    }
}
