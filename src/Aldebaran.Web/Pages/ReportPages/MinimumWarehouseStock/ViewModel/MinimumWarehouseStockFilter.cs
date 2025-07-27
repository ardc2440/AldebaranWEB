using Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.ViewModel
{
    public class MinimumWarehouseStockFilter
    {
        public short? WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
        public bool OnlyBelowMinimum { get; set; } = true; // Filtro específico para este reporte
        public int? UmbralMaximoExistencias { get; set; } // Campo obligatorio - Umbral máximo de existencias
        
        // Nuevos filtros con multiselección usando strings de IDs separados por comas
        public string ProviderIds { get; set; } = string.Empty; // "1,2,3"
        public string ItemIds { get; set; } = string.Empty; // "1,2,3"
        public string ReferenceIds { get; set; } = string.Empty; // "1,2,3"
        
        // Listas para mostrar los nombres seleccionados en la UI
        public List<Provider> SelectedProviders { get; set; } = new List<Provider>();
        public List<Item> SelectedItems { get; set; } = new List<Item>();
        public List<ItemReference> SelectedReferences { get; set; } = new List<ItemReference>();
    }
}