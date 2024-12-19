using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Models.ViewModels
{
    public class LocalWarehouseAlarm
    {
        public ServiceModel.LocalWarehouseAlarm WarehouseAlarm { get; set; }
        public ServiceModel.PurchaseOrder PurchaseOrder { get; set; }
        public ServiceModel.WarehouseTransfer WarehouseTransfer { get; set; }
        public ICollection<ServiceModel.CustomerOrder> CustomerOrders {  get; set; } = new List<ServiceModel.CustomerOrder>();
        public ICollection<AlarmReference> AlarmReferences { get; set; } = new List<AlarmReference>();
        public ICollection<AlarmOrder> AlarmCustomerOrders { get; set; } = new List<AlarmOrder>();        
    }

    public class AlarmOrder
    {
        public int AlarmOrderId { get; set; }
        public ICollection<AlarmOrderDetail> Details { get; set; } = new List<AlarmOrderDetail>();
    }

    public class AlarmOrderDetail
    {
        public int ReferenceId { get; set; }
        public int PendingQuantity { get; set; }
    }

    public class AlarmReference
    {
        public int ReferenceId { get; set; }
    }
}
