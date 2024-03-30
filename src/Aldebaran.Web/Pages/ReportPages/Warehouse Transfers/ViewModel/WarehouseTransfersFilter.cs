using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;
using System.Security.Cryptography.Xml;
namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel
{
    public class WarehouseTransfersFilter
    {
        public short? TargetWarehouseId { get; set; }
        public Warehouse TargetWarehouse { get; set; }
        public short? SourceWarehouseId { get; set; }
        public Warehouse SourceWarehouse { get; set; }
        public DateRange AdjustmentDate { get; set; } = new();
        public string NationalizationNumber { get; set; }
        public StatusDocumentType Status { get; set; }
        public int? StatusId { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
    }
}
