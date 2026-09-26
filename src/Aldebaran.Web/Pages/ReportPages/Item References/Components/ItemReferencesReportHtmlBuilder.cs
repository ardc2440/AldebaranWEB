using Aldebaran.Application.Services.Models.Reports;
using System.Net;
using System.Text;

namespace Aldebaran.Web.Pages.ReportPages.Item_References.Components
{
    /// <summary>
    /// Genera el documento HTML completo del reporte (todas las líneas, artículos y referencias)
    /// a partir del resultado del caso de uso, sin depender de lo que la pantalla tenga pintado (paginado).
    /// Usa las mismas clases CSS que la pantalla para que el documento se vea igual.
    /// Nota (DT5): replica la estructura del markup Razor porque en .NET 7 no existe HtmlRenderer.
    /// </summary>
    public static class ItemReferencesReportHtmlBuilder
    {
        private const string OddRowStyle = "background: #f5f5f5;";
        /// <summary>Imprimir abre en horizontal (9 columnas), igual que el PDF. Solo afecta a este documento.</summary>
        private const string LandscapePageStyle = " @page { size: landscape; }";
        private const string AllowPageBreakStyle = "page-break-inside:auto !important; break-inside:auto !important;";

        public static string Build(ItemReferencesReportResult report, ItemReferencesReportFilterSummary summary, string css, string generatedAt)
        {
            var html = new StringBuilder();
            html.Append("<html><head><meta charset=\"utf-8\"><style>").Append(css ?? string.Empty).Append(LandscapePageStyle).Append("</style></head><body>");
            html.Append("<div class=\"report\">");
            AppendHeader(html, summary, generatedAt);
            AppendBody(html, report ?? ItemReferencesReportResult.Empty);
            html.Append("</div></body></html>");
            return html.ToString();
        }

        private static void AppendHeader(StringBuilder html, ItemReferencesReportFilterSummary summary, string generatedAt)
        {
            html.Append("<table class=\"mt-1\"><tr><td width=\"50%\">")
                .Append("<h2 class=\"text-left mb-0\">").Append(E(ItemReferencesReportFormat.Title)).Append("</h2>")
                .Append("<span class=\"mb-1\">").Append(E(generatedAt)).Append("</span>")
                .Append("</td><td width=\"50%\">");
            AppendFilters(html, summary);
            html.Append("</td></tr></table>");
        }

        private static void AppendFilters(StringBuilder html, ItemReferencesReportFilterSummary summary)
        {
            if (summary?.HasAny != true)
                return;

            html.Append("<div class=\"filters\"><div class=\"title\">Filtros:</div>");
            foreach (var (caption, values) in summary.Groups())
            {
                html.Append("<div class=\"mt-1\"><span class=\"component-label\"><b>").Append(E(caption)).Append(": </b></span>");
                foreach (var value in values)
                    html.Append("<div>").Append(E(value)).Append("</div>");
                html.Append("</div>");
            }
            html.Append("</div>");
        }

        private static void AppendBody(StringBuilder html, ItemReferencesReportResult report)
        {
            if (!report.HasData)
            {
                html.Append("<div class=\"no-data-container\"><h4>No hay datos para los filtros seleccionados.</h4></div>");
                return;
            }

            // La tabla contenedora sí puede partirse entre páginas (print.css evita partir "table.edged");
            // cada artículo conserva su tabla sin partir.
            html.Append("<table class=\"edged mt-1\" style=\"").Append(AllowPageBreakStyle).Append("\"><tbody>");
            foreach (var line in report.Lines)
                AppendLine(html, line);
            html.Append("</tbody></table>");
        }

        private static void AppendLine(StringBuilder html, ItemReferencesReportLine line)
        {
            html.Append("<tr><td width=\"100%\" style=\"padding:0 !important\"><p class=\"title\">").Append(E(line.LineName)).Append("</p></td></tr>");
            foreach (var item in line.Items)
            {
                html.Append("<tr><td width=\"100%\" class=\"m-0 p-0\">");
                AppendItem(html, item);
                html.Append("</td></tr>");
            }
        }

        private static void AppendItem(StringBuilder html, ItemReferencesReportItem item)
        {
            html.Append("<div class=\"p-2\"><table class=\"edged m-0\"><thead>");
            AppendItemHeader(html, item);
            AppendReferenceColumns(html);
            html.Append("</thead><tbody>");
            for (int i = 0; i < item.References.Count; i++)
                AppendReference(html, item.References[i], i);
            html.Append("</tbody></table></div>");
        }

        private static void AppendItemHeader(StringBuilder html, ItemReferencesReportItem item)
        {
            html.Append("<tr><td colspan=\"").Append(ItemReferencesReportFormat.ReferenceColumns.Count).Append("\" width=\"100%\"><div style=\"padding-bottom:0.5rem !important\">")
                .Append("<h3 class=\"text-left m-0\">").Append(E(item.ItemName)).Append("</h3>")
                .Append("<p class=\"m-0\">")
                .Append(Field("Referencia interna", item.InternalReference)).Append(Separator)
                .Append(Field("Nombre para el proveedor", item.ProviderItemName)).Append(Separator)
                .Append(Field("Referencia Artículo para proveedor", item.ProviderReference))
                .Append("</p><p class=\"m-0\">")
                .Append(Field("Estado", ItemReferencesReportFormat.ToStatus(item.IsActive))).Append(Separator)
                .Append(Field("Visible en página", ItemReferencesReportFormat.ToYesNo(item.IsCatalogVisible))).Append(Separator)
                .Append(Field("Producto nacional", ItemReferencesReportFormat.ToYesNo(item.IsDomesticProduct))).Append(Separator)
                .Append(Field("Importación especial", ItemReferencesReportFormat.ToYesNo(item.IsSpecialImport))).Append(Separator)
                .Append(Field("Promoción", ItemReferencesReportFormat.ToYesNo(item.IsSaleOff)))
                .Append("</p></div></td></tr>");
        }

        private static void AppendReferenceColumns(StringBuilder html)
        {
            html.Append("<tr>");
            foreach (var (text, width) in ItemReferencesReportFormat.ReferenceColumns)
                html.Append("<th width=\"").Append(width).Append("\"><b>").Append(E(text)).Append("</b></th>");
            html.Append("</tr>");
        }

        private static void AppendReference(StringBuilder html, ItemReferencesReportReference reference, int index)
        {
            html.Append("<tr style=\"").Append(index % 2 == 0 ? OddRowStyle : string.Empty).Append("\">")
                .Append(Cell(reference.ReferenceName))
                .Append(Cell(reference.ReferenceCode))
                .Append(Cell(reference.ProviderReferenceName))
                .Append(Cell(reference.ProviderReferenceCode))
                .Append(Cell(ItemReferencesReportFormat.ToStatus(reference.IsActive)))
                .Append(Cell(ItemReferencesReportFormat.ToYesNo(reference.IsSoldOut)))
                .Append(Cell(ItemReferencesReportFormat.ToNumber(reference.AlarmMinimumQuantity)))
                .Append(Cell(ItemReferencesReportFormat.ToNumber(reference.MinimumLocalWarehouseQuantity)))
                .Append(Cell(ItemReferencesReportFormat.ToNumber(reference.PurchaseOrderVariation)))
                .Append("</tr>");
        }

        private const string Separator = " &nbsp;|&nbsp; ";

        private static string Field(string label, string value) => $"{E(label)}: <b>{E(value)}</b>";

        private static string Cell(string value) => $"<td>{E(value)}</td>";

        private static string E(string value) => WebUtility.HtmlEncode(value ?? string.Empty);
    }
}
