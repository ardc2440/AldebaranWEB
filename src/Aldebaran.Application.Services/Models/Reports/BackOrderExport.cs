using System.ComponentModel;

namespace Aldebaran.Application.Services.Models.Reports
{
    public class BackOrderExport
    {
        [DisplayName("Pedido")]
        public string OrderNumber { get; set; } = null!;
        [DisplayName("Pedido especial")]
        public string SpecialOrder { get; set; } = null!;
        [DisplayName("Cliente")]
        public string CustomerName { get; set; } = null!;
        [DisplayName("Nit")]
        public string IdentityNumber { get; set; } = null!;
        [DisplayName("Telefono")]
        public string Phone { get; set; } = null!;
        [DisplayName("Ciudad")]
        public string CityName { get; set; } = null!;
        [DisplayName("Fecha Pedido")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Nombre Item")]
        public string ItemName { get; set; } = null!;
        [DisplayName("Código Item")]
        public string ItemCode { get; set; } = null!;
        [DisplayName("Nombre Referencia")]
        public string ReferenceName { get; set; } = null!;
        [DisplayName("Código Referencia")]
        public string ReferenceCode { get; set; } = null!;
        [DisplayName("Cant. Pendiente")]
        public int PendingAmount { get; set; }
        [DisplayName("Fecha Estimada Entrega")]
        public DateTime EstimatedDeliveryDate { get; set; }
        [DisplayName("Observaciones Internas")]
        public string? InternalNotes { get; set; }
        [DisplayName("Observaciones Cliente")]
        public string? CustomerNotes { get; set; }
        [DisplayName("Estado")]
        public string Status { get; set; } = null!;
    }
}
