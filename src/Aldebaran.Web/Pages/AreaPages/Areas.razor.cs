using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.AreaPages
{
    public partial class Areas : ComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.AldebaranDb.Area> areas;
        protected RadzenDataGrid<Models.AldebaranDb.Area> grid0;
        protected RadzenDataGrid<Models.AldebaranDb.ItemsArea> ItemsAreasDataGrid;
        protected Models.AldebaranDb.Area area;
        protected string search = "";
        protected DialogResult dialogResult { get; set; }
        protected bool isLoadingInProgress;

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);
            areas = await AldebaranDbService.GetAreas(new Query { Filter = $@"i => i.AREA_CODE.Contains(@0) || i.AREA_NAME.Contains(@0) || i.DESCRIPTION.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();
                areas = await AldebaranDbService.GetAreas(new Query { Filter = $@"i => i.AREA_CODE.Contains(@0) || i.AREA_NAME.Contains(@0) || i.DESCRIPTION.Contains(@0)", FilterParameters = new object[] { search } });
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Area area)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar esta área?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteArea(area.AREA_ID);
                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el área"
                });
            }
        }

        protected async Task GetChildData(Models.AldebaranDb.Area args)
        {
            area = args;
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                var ItemsAreasResult = await AldebaranDbService.GetItemsAreas(new Query { Filter = $@"i => i.AREA_ID == {args.AREA_ID}", Expand = "Area,Item.MeasureUnit,Item.Currency,Item.MeasureUnit1,Item.Line" });
                if (ItemsAreasResult != null)
                {
                    args.ItemsAreas = ItemsAreasResult.ToList();
                }
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task ItemsAreasAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Area data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddItemsArea>("Agregar artículo", new Dictionary<string, object> { { "AREA_ID", data.AREA_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Artículo agregado correctamente al área." };
            }
            await GetChildData(data);
            await ItemsAreasDataGrid.Reload();
        }

        protected async Task ItemsAreasDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.ItemsArea itemsArea)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar esta artículo del área?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItemsArea(itemsArea.ITEM_ID, itemsArea.AREA_ID);
                    await GetChildData(area);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Artículo eliminado del área correctamente." };
                        await ItemsAreasDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el artículo del área"
                });
            }
        }
    }
}