using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Models.ViewModels
{
    public class EmployeeViewModel : ServiceModel.Employee
    {
        public ApplicationUser ApplicationUser { get; set; }
    }
}
