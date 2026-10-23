using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Radzen;

namespace Aldebaran.Web.Shared
{
    /// <summary>
    /// Diálogo "Inventario del artículo" de la alarma de cantidades mínimas.
    /// Solo presenta: el caso de uso lo resuelve <see cref="IArticleInventoryService"/>.
    /// </summary>
    public partial class ArticleInventoryDialog
    {
        private const string AlarmRowStyle = "font-weight: bold; background-color: #fff3cd;";
        protected const string BombBadgeStyle = "font-size: 1rem; padding: 0.35rem 0.75rem; transform: rotate(-12deg); box-shadow: 0 2px 6px rgba(0,0,0,.35);";

        #region Injections
        [Inject]
        protected IArticleInventoryService ArticleInventoryService { get; set; }

        [Inject]
        protected ILogger<ArticleInventoryDialog> Logger { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IOptions<AppSettings> Settings { get; set; }
        #endregion

        #region Parameters
        /// <summary>Referencia de la alarma: ubica el artículo y se resalta en la grilla.</summary>
        [Parameter]
        public int ReferenceId { get; set; }

        /// <summary>Nombre de la alarma con formato "[referencia interna] artículo - referencia" (lo usa ImageDialog).</summary>
        [Parameter]
        public string ArticleName { get; set; } = string.Empty;

        /// <summary>Artículo con precio bomba: muestra la marca sobre la imagen.</summary>
        [Parameter]
        public bool IsBombPrice { get; set; }
        #endregion

        #region Variables
        protected IReadOnlyList<ArticleReferenceInventory> references = Array.Empty<ArticleReferenceInventory>();
        protected bool isLoading;
        protected bool loadError;
        protected bool hasImage;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            hasImage = ArticleImageLocator.Exists(ArticleName, Settings.Value.ImageRepositoryPath);
            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            isLoading = true;
            loadError = false;
            try
            {
                references = await ArticleInventoryService.GetByReferenceAsync(ReferenceId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "No se pudo consultar el inventario del artículo de la referencia {ReferenceId}", ReferenceId);
                references = Array.Empty<ArticleReferenceInventory>();
                loadError = true;
            }
            finally
            {
                isLoading = false;
            }
        }

        /// <summary>Resalta la referencia de la alarma (Radzen pinta el fondo en las celdas, no en la fila).</summary>
        protected void OnCellRender(DataGridCellRenderEventArgs<ArticleReferenceInventory> args)
        {
            if (args.Data.IsAlarmReference)
                args.Attributes["style"] = AlarmRowStyle;
        }

        protected static string FormatQuantity(int value) => value.ToString("N0");

        protected static string FormatQuantity(int? value) => value?.ToString("N0") ?? string.Empty;

        protected static string FormatDate(DateTime? value) => value?.ToString("dd/MM/yyyy") ?? string.Empty;

        /// <summary>Muestra al pasar el cursor sobre el proveedor los datos de la última OC Confirmada.</summary>
        protected void ShowLastPurchaseTooltip(ElementReference element, ArticleReferenceInventory row) =>
            TooltipService.Open(element, _ => LastPurchaseTooltip(row), new TooltipOptions { Position = TooltipPosition.Left, Duration = 8000 });

        protected static string AvailableStyle(int available) =>
            available <= 0 ? "color: red; font-weight: bold;" : "color: green; font-weight: bold;";
    }
}
