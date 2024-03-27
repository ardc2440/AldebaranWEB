using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel
{
    public class WarehouseTransfersFilter
    {
        public short? TargetWarehouseId { get; set; }
        public Warehouse TargetWarehouse { get; set; }
        public short? SourceWarehouseId { get; set; }
        public Warehouse SourceWarehouse { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string NationalizationNumber { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
    }
}
