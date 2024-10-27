using Aldebaran.Application.Services;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.AreaPages
{
    public partial class Areas : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<Areas> Logger { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IAreaService AreaService { get; set; }

        [Inject]
        protected IItemAreaService ItemAreaService { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.Area> AreasList;
        protected LocalizedDataGrid<ServiceModel.Area> AreasDataGrid;
        protected ServiceModel.Area Area;
        protected LocalizedDataGrid<ServiceModel.ItemsArea> ItemsAreasDataGrid;
        protected string search = "";
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetAreasAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);
        async Task GetAreasAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            AreasList = string.IsNullOrEmpty(searchKey) ? await AreaService.GetAsync(ct) : await AreaService.GetAsync(searchKey, ct);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await AreasDataGrid.GoToPage(0);
            await GetAreasAsync(search);
        }
        protected async Task GetAreaItems(ServiceModel.Area args)
        {
            Area = args;
                await Task.Yield();
                var itemsAreasResult = await ItemAreaService.GetByAreaIdAsync(Area.AreaId);
                args.ItemsAreas = itemsAreasResult.ToList();            
        }
        protected async Task AddItemArea(MouseEventArgs args, ServiceModel.Area data)
        {
            var result = await DialogService.OpenAsync<AddItemsArea>("Agregar artículo", new Dictionary<string, object> { { "AREA_ID", data.AreaId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Artículo",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Artículo agregado correctamente al área."
                });
            }
            await GetAreaItems(data);
            await ItemsAreasDataGrid.Reload();
        }
        protected async Task DeleteItemArea(MouseEventArgs args, ServiceModel.ItemsArea itemsArea)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este artículo del área?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ItemAreaService.DeleteAsync(itemsArea.AreaId, itemsArea.ItemId);
                    await GetAreaItems(Area);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Artículo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Artículo eliminado del área correctamente."
                    });
                    await ItemsAreasDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteItemArea));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el artículo del área \n {ex.Message}"
                });
            }
        }
        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}