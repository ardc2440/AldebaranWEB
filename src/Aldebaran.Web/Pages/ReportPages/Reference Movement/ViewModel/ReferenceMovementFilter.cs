using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;
namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel
{
    public class ReferenceMovementFilter
    {
        public DateRange MovementDate { get; set; } = new();
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
    }
}
