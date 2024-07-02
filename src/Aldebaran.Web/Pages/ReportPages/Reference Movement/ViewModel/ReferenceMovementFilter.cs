using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;
namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel
{
    public class ReferenceMovementFilter
    {
        public DateRange MovementDate { get; set; } = new();
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
        public bool LockReferenceSelection { get; set; } = false;
        public bool AllRequiredFieldsCompleted => MovementDate.StartDate != null && MovementDate.EndDate != null && ItemReferences.Any();
    }
}
