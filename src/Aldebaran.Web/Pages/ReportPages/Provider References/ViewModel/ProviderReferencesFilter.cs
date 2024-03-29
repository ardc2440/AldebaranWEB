using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel
{
    public class ProviderReferencesFilter
    {
        public int? ProviderId { get; set; }
        public Provider Provider { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
    }
}
