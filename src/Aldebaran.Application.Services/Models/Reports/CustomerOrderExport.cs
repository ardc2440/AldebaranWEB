using System.ComponentModel;

namespace Aldebaran.Application.Services.Models.Reports
{
    public class CustomerOrderExport
    {
        [DisplayName("Pedido")]
        public string OrderNumber { get; set; }
        [DisplayName("Cliente")]
        public string CustomerName { get; set; }
        [DisplayName("Nit")]
        public string IdentityNumber { get; set; }
        [DisplayName("Telefono")]
        public string Phone { get; set; }
        [DisplayName("Ciudad")]
        public string CityName { get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Nombre Item")]
        public string ItemName { get; set; }
        [DisplayName("Código Item")]
        public string ItemCode { get; set; }
        [DisplayName("Nombre Referencia")]
        public string ReferenceName { get; set; }
        [DisplayName("Código Referencia")]
        public string ReferenceCode { get; set; }
        [DisplayName("Cant Pedido")]
        public int Amount { get; set; }
        [DisplayName("Cant. Enviada")]
        public int DeliveredAmount { get; set; }
        [DisplayName("Cant. Pendiente")]
        public int PendingAmount { get; set; }
        [DisplayName("Cant. Procesada")]
        public int ProcessedAmount { get; set; }
        [DisplayName("Fecha Estimada Entrega")]
        public DateTime EstimatedDeliveryDate { get; set; }
        [DisplayName("Nombre del cliente")]
        public string? InternalNotes { get; set; }
        [DisplayName("Observaciones Cliente")]
        public string? CustomerNotes { get; set; }
        [DisplayName("Remision")]
        public string? DeliveriNote { get; set; }
        [DisplayName("Fecha Envio")]
        public DateTime? ShippingDate { get; set; }
        [DisplayName("Observaciones Envio")]
        public string? ShippingNotes { get; set; }
        [DisplayName("Estado")]
        public string Status { get; set; }
    }
}
