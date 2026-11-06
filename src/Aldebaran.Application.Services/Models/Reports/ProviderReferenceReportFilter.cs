namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de Referencias del Proveedor.</summary>
    public class ProviderReferenceReportFilter
    {
        public int? ProviderId { get; set; }

        /// <summary>Ids de línea. Se usa solo si no hay artículos ni referencias.</summary>
        public IEnumerable<short> LineIds { get; set; } = Enumerable.Empty<short>();
        /// <summary>Ids de artículo. Se usa solo si no hay referencias.</summary>
        public IEnumerable<int> ItemIds { get; set; } = Enumerable.Empty<int>();
        /// <summary>Ids de referencia. Nivel más específico: prevalece sobre artículos y líneas.</summary>
        public IEnumerable<int> ReferenceIds { get; set; } = Enumerable.Empty<int>();

        // Condiciones de Artículo (true = Sí, false = No, null = Todos)
        public bool? IsItemActive { get; set; }
        public bool? IsCatalogVisible { get; set; }
        public bool? IsDomesticProduct { get; set; }
        public bool? IsSpecialImport { get; set; }
        public bool? IsSaleOff { get; set; }
        public bool? BombPrice { get; set; }

        // Condiciones de Referencia (true = Sí, false = No, null = Todos)
        public bool? IsReferenceActive { get; set; }
        public bool? IsSoldOut { get; set; }

        /// <summary>Opción "Mostrar inventario y bodegas" (Sí por defecto). No filtra registros, solo la presentación.</summary>
        public bool IncludeInventory { get; set; } = true;
    }
}
