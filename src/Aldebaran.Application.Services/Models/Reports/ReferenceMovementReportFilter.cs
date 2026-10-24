namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de movimientos de artículos (referencias).</summary>
    public class ReferenceMovementReportFilter
    {
        /// <summary>Ids de referencia separados por coma; vacío = según las condiciones de activo.</summary>
        public string? ReferenceIds { get; set; }
        public DateTime? InitialMovementDate { get; set; }
        public DateTime? FinalMovementDate { get; set; }
        /// <summary>Artículo activo: true = Sí, false = No, null = Todos.</summary>
        public bool? IsItemActive { get; set; }
        /// <summary>Referencia activa: true = Sí, false = No, null = Todos.</summary>
        public bool? IsReferenceActive { get; set; }
        /// <summary>Precio bomba: true = Sí, false = No, null = Todos.</summary>
        public bool? BombPrice { get; set; }
    }
}
