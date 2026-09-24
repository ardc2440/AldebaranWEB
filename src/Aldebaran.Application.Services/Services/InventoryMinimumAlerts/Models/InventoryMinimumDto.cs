using Aldebaran.Infrastructure.Common.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts.Models
{
    public class InventoryMinimumDto
    {
        public int AlarmId { get; set; }

        public int ReferenceId { get; set; }

        public required string ArticleName { get; set; }

        public string? ImagePath { get; set; }

        public int MinimumQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public int InTransitQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int OrderedQuantity { get; set; }
    }

    public class InventoryMinimumExportDto
    {
        [DisplayName("Artículo")]
        public required string ArticleName { get; set; }

        [DisplayName("Imagen")]
        [ExcelImage]
        public string? ImagePath { get; set; }

        [DisplayName("Cantidad mínima")]
        public int MinimumQuantity { get; set; }

        [DisplayName("B. Local + Zona Franca")]
        public int TotalStock { get; set; }

        [DisplayName("En tránsito")]
        public int InTransitQuantity { get; set; }

        [DisplayName("Reservado")]
        public int ReservedQuantity { get; set; }

        [DisplayName("Pedido")]
        public int OrderedQuantity { get; set; }

        [DisplayName("Disponible")]
        public int AvailableQuantity { get; set; }
    }
}