using System.ComponentModel;

namespace Aldebaran.Application.FileWritingService.Workers.Inventory.Models
{
    public class InventoryExcelViewModel
    {
        // Linea
        [DisplayName("Codigo de la línea")]
        public short LineId { get; set; }
        [DisplayName("Línea")]
        public string LineName { get; set; }

        // Articulo
        [DisplayName("Código del artículo")]
        public int ItemId { get; set; }
        [DisplayName("Artículo")]
        public string ItemName { get; set; }
        [DisplayName("Referencia interna")]
        public string InternalReference { get; set; }

        // Referencia
        [DisplayName("Nombre de la referencia")]
        public string ReferenceName { get; set; }
        [DisplayName("Disponible")]
        public int AvailableAmount { get; set; }
        [DisplayName("Zona franca")]
        public int FreeZone { get; set; }

        // Ordenes
        [DisplayName("Código de la orden")]
        public int? PurchaseOrderId { get; set; }
        [DisplayName("Código de la referencia en tránsico")]
        public int ReferenceId { get; set; }
        [DisplayName("Fecha de la orden")]
        public DateTime? OrderDate { get; set; }
        [DisplayName("Bodega")]
        public string? Warehouse { get; set; }
        [DisplayName("Total")]
        public int? Total { get; set; }

        //Actividades
        [DisplayName("Fecha de la actividad")]
        public DateTime? ActivityDate { get; set; }
        [DisplayName("Descripción de la actividad")]
        public string? Description { get; set; }
    }
}
