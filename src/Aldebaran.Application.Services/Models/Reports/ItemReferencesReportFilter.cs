namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de Artículos y Referencias.</summary>
    public class ItemReferencesReportFilter
    {
        /// <summary>Ids de línea. Se usa solo si no hay artículos ni referencias.</summary>
        public IEnumerable<short> LineIds { get; set; } = Enumerable.Empty<short>();
        /// <summary>Ids de artículo. Se usa solo si no hay referencias.</summary>
        public IEnumerable<int> ItemIds { get; set; } = Enumerable.Empty<int>();
        /// <summary>Ids de referencia. Nivel más específico: prevalece sobre artículos y líneas.</summary>
        public IEnumerable<int> ReferenceIds { get; set; } = Enumerable.Empty<int>();

        // Switches de Artículo (true = aplica el filtro, false = trae todo)
        public bool OnlyActiveItems { get; set; }
        public bool OnlyCatalogVisible { get; set; }
        public bool OnlyDomesticProduct { get; set; }
        public bool OnlySpecialImport { get; set; }
        public bool OnlySaleOff { get; set; }

        // Switches de Referencia (true = aplica el filtro, false = trae todo)
        public bool OnlyActiveReferences { get; set; }
        public bool OnlySoldOut { get; set; }
        public bool OnlyWithAlarmMinimumQuantity { get; set; }
        public bool OnlyWithMinimumLocalWarehouseQuantity { get; set; }
    }
}
