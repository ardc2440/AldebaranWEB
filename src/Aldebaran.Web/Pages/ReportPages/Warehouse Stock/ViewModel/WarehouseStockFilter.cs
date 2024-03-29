using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.ViewModel
{
    public class WarehouseStockFilter
    {
        public Warehouse Warehouse { get; set; } 
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
    }
}
