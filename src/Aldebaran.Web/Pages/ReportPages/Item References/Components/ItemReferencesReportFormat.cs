namespace Aldebaran.Web.Pages.ReportPages.Item_References.Components
{
    /// <summary>Formatos de presentación del reporte, compartidos por la pantalla y el documento (imprimir/PDF).</summary>
    public static class ItemReferencesReportFormat
    {
        public const string Title = "Artículos y referencias";

        public static string ToYesNo(bool value) => value ? "Sí" : "No";

        public static string ToStatus(bool isActive) => isActive ? "Activo" : "Inactivo";

        public static string ToNumber(int value) => value.ToString("N0");

        /// <summary>Encabezados de la tabla de referencias (mismo orden en pantalla y documento).</summary>
        public static IReadOnlyList<(string Text, string Width)> ReferenceColumns { get; } = new[]
        {
            ("Nombre referencia", "16%"),
            ("Código interno", "12%"),
            ("Nombre Referencia para proveedor", "16%"),
            ("Código Referencia para proveedor", "12%"),
            ("Estado", "8%"),
            ("Agotada", "8%"),
            ("Cantidad mínima general", "9%"),
            ("Cantidad mínima en Bodega local", "10%"),
            ("Variación en orden de compra", "9%")
        };
    }
}
