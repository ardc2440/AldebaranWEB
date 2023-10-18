using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Aldebaran.Web.Models
{
    public partial class ApplicationRole : IdentityRole
    {
        [JsonIgnore]
        public ICollection<ApplicationUser> Users { get; set; }

    }
}