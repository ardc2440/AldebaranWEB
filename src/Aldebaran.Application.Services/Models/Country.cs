namespace Aldebaran.Application.Services.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        // Reverse navigation
        public ICollection<Department> Departments { get; set; }
        public Country()
        {
            Departments = new List<Department>();
        }

    }
}
