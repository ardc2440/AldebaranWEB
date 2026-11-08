using Radzen;

namespace Aldebaran.Web.Pages.CompetitorPages
{
    /// <summary>Abre la imagen ampliada de un producto de la competencia (pantalla de homologaciones y reporte).</summary>
    public static class CompetitorImageDialogExtensions
    {
        public static void OpenCompetitorImage(this DialogService dialogService, string imageUrl, string title) =>
            dialogService.Open<CompetitorImageDialog>("", new Dictionary<string, object>
            {
                { nameof(CompetitorImageDialog.Title), title },
                { nameof(CompetitorImageDialog.ImageUrl), imageUrl }
            }, new DialogOptions { CloseDialogOnOverlayClick = true });
    }
}
