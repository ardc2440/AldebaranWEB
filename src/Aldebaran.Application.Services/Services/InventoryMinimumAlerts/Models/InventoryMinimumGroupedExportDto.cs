using Aldebaran.Infrastructure.Common.Attributes;
using System.ComponentModel;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts.Models
{
    /// <summary>
    /// Fila del Excel del correo de cantidades mínimas, agrupada por artículo:
    /// padre = alarma (con imagen), hijos = referencias activas del artículo (sin imagen).
    /// Mismas columnas y fórmulas del diálogo "Inventario del artículo" del Tablero.
    /// </summary>
    public class InventoryMinimumGroupedExportDto
    {
        [DisplayName("Artículo / Referencia")]
        public required string Description { get; set; }

        [DisplayName("Imagen")]
        [ExcelImage(BadgeProperty = nameof(ImageBadge))]
        public string? ImagePath { get; set; }

        /// <summary>Marca sobre la imagen (Precio Bomba); no es columna.</summary>
        [ExcelIgnore]
        public string? ImageBadge { get; set; }

        [DisplayName("En alarma")]
        public string? InAlarm { get; set; }

        [DisplayName("Cantidad mínima")]
        public int? MinimumQuantity { get; set; }

        [DisplayName("Bodega Local")]
        public int? LocalWarehouse { get; set; }

        [DisplayName("Zona Franca")]
        public int? FreeZone { get; set; }

        [DisplayName("Stock Físico")]
        public int? PhysicalStock { get; set; }

        [DisplayName("En tránsito")]
        public int? InTransit { get; set; }

        [DisplayName("Comprometido")]
        public int? Committed { get; set; }

        [DisplayName("Disponible")]
        public int? Available { get; set; }

        // Última OC Confirmada de la referencia: solo en filas hijas (la fila padre repite la referencia de la alarma).
        [DisplayName("Proveedor última compra")]
        public string? LastPurchaseProvider { get; set; }

        [DisplayName("Fecha OC")]
        public DateTime? LastPurchaseOrderDate { get; set; }

        [DisplayName("Fecha real de recibo")]
        public DateTime? LastPurchaseReceiptDate { get; set; }

        [DisplayName("Cantidad solicitada")]
        public int? LastPurchaseRequestedQuantity { get; set; }

        [DisplayName("Cantidad recibida")]
        public int? LastPurchaseReceivedQuantity { get; set; }

        /// <summary>Referencias del artículo (no es columna: el generador solo exporta tipos simples).</summary>
        public IReadOnlyList<InventoryMinimumGroupedExportDto> Children { get; set; } = Array.Empty<InventoryMinimumGroupedExportDto>();
    }
}
