using Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.ViewModel
{
    public class MinimumWarehouseStockFilter
    {
        public short? WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
        public int? UmbralMaximoExistencias { get; set; } // Campo obligatorio - Umbral máximo de existencias
        
        // Filtros con multiselección usando strings de IDs separados por comas
        public string ItemIds { get; set; } = string.Empty; // "1,2,3"
        public string ReferenceIds { get; set; } = string.Empty; // "1,2,3"
        
        // Filtros de texto libre para campos descriptivos del proveedor
        public string ProviderItemName { get; set; } = string.Empty; // Artículo del proveedor
        public string ProviderReference { get; set; } = string.Empty; // Referencia interna del proveedor
        public string ProviderReferenceName { get; set; } = string.Empty; // Referencia del proveedor
        public string ProviderReferenceCode { get; set; } = string.Empty; // Código de referencia del proveedor
        
        // Filtros de checkbox de 3 estados para Artículos (null = ambos, true = sí, false = no)
        public bool? ItemIsActive { get; set; } = null; // Activo
        public bool? ItemIsDomesticProduct { get; set; } = null; // Producto nacional
        public bool? ItemIsSpecialImport { get; set; } = null; // Importado especial
        public bool? ItemIsSaleOff { get; set; } = null; // En oferta
        public bool? ItemIsCatalogVisible { get; set; } = null; // Visible en catálogo
        
        // Filtros de checkbox de 3 estados para Referencias (null = ambos, true = sí, false = no)
        public bool? ReferenceIsActive { get; set; } = null; // Activa
        public bool? ReferenceHasAlarmMinimumQuantity { get; set; } = null; // Alarma de cantidad mínima
        public bool? ReferenceIsSoldOut { get; set; } = null; // Agotado
        
        // Listas para mostrar los nombres seleccionados en la UI
        public List<Item> SelectedItems { get; set; } = new List<Item>();
        public List<ItemReference> SelectedReferences { get; set; } = new List<ItemReference>();
    }
}