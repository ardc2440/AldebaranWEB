using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Item_References.Components
{
    /// <summary>
    /// Presenta un artículo del reporte (encabezado con sus datos) y la tabla de sus referencias.
    /// </summary>
    public partial class ItemReferencesReportItemTable
    {
        private const string OddRowStyle = "background: #f5f5f5;";

        [Inject]
        protected DialogService DialogService { get; set; }

        [Parameter, EditorRequired]
        public ItemReferencesReportItem Item { get; set; }

        protected static string ToYesNo(bool value) => value ? "Sí" : "No";

        protected static string ToStatus(bool isActive) => isActive ? "Activo" : "Inactivo";

        protected static string RowStyle(int index) => index % 2 == 0 ? OddRowStyle : string.Empty;

        protected Task ShowImageDialogAsync() =>
            DialogService.OpenAsync<ImageDialog>(string.Empty, new Dictionary<string, object>
            {
                { "ArticleName", $"[{Item.InternalReference}] {Item.ItemName}" }
            });
    }
}
