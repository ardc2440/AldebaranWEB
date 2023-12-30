using Aldebaran.Application.Services;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using System.Text.Encodings.Web;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class Items
    {
        #region Injections
        [Inject]
        protected ILogger<Items> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IItemService ItemService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IExportHelper ExportHelper { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.Item> ItemsList;
        protected ServiceModel.Item Item;
        protected LocalizedDataGrid<ServiceModel.Item> ItemsDataGrid;
        protected LocalizedDataGrid<ServiceModel.ItemReference> ItemReferencesDataGrid;
        protected string search = "";
        protected DialogResult DialogResult { get; set; }
        protected bool IsLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;
                await GetItemsAsync();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        async Task GetItemsAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            ItemsList = string.IsNullOrEmpty(searchKey) ? await ItemService.GetAsync(ct) : await ItemService.GetAsync(searchKey, ct);
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ItemsDataGrid.GoToPage(0);
            await GetItemsAsync(search);
        }

        void OnItemDataGridRender(DataGridRenderEventArgs<ServiceModel.Item> args)
        {
            if (!args.FirstRender)
                return;
            args.Grid.Groups.Add(new GroupDescriptor() { Title = "Línea", Property = "Line.LineName", SortOrder = SortOrder.Descending });
            StateHasChanged();
        }

        protected async Task AddItem(MouseEventArgs args)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<AddItem>("Nuevo artículo");
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Artículo creado correctamente." };
            }
            await GetItemsAsync();
            await ItemsDataGrid.Reload();
        }
        protected async Task EditItem(ServiceModel.Item item)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<EditItem>("Actualizar artículo", new Dictionary<string, object> { { "ITEM_ID", item.ItemId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Artículo actualizado correctamente." };
            }
            await GetItemsAsync();
            await ItemsDataGrid.Reload();
        }
        protected async Task DeleteItem(MouseEventArgs args, ServiceModel.Item item)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este artículo?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ItemService.DeleteAsync(item.ItemId);
                    await GetItemsAsync();
                    DialogResult = new DialogResult { Success = true, Message = "Artículo eliminado correctamente." };
                    await ItemsDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteItem));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el artículo"
                });
            }
        }
        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                NavigationManager.NavigateTo($"export/aldebarandb/items/csv(fileName='{UrlEncoder.Default.Encode("Artículos")}')", true);
            }

            if (args == null || args.Value == "xlsx")
            {
                NavigationManager.NavigateTo($"export/AldebaranDb/items/excel(fileName='{UrlEncoder.Default.Encode("Artículos")}')", true);
            }
        }

        protected async Task GetItemReferences(ServiceModel.Item args)
        {
            Item = args;
            try
            {
                IsLoadingInProgress = true;
                await Task.Yield();
                var ItemReferencesResult = await ItemReferenceService.GetByItemIdAsync(args.ItemId);
                args.ItemReferences = ItemReferencesResult.ToList();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        protected async Task AddItemReference(MouseEventArgs args, ServiceModel.Item data)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<AddItemReference>("Nueva referencia", new Dictionary<string, object> { { "ITEM_ID", data.ItemId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Referencia creada correctamente." };
            }
            await GetItemReferences(data);
            await ItemReferencesDataGrid.Reload();
        }
        protected async Task EditItemReference(ServiceModel.ItemReference args, ServiceModel.Item data)
        {
            DialogResult = null;
            var result = await DialogService.OpenAsync<EditItemReference>("Actualizar referencia", new Dictionary<string, object> { { "REFERENCE_ID", args.ReferenceId } });
            if (result == true)
            {
                DialogResult = new DialogResult { Success = true, Message = "Referencia actualizada correctamente." };
            }
            await GetItemReferences(data);
            await ItemReferencesDataGrid.Reload();
        }
        protected async Task DeleteItemReference(MouseEventArgs args, ServiceModel.ItemReference itemReference)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ItemReferenceService.DeleteAsync(itemReference.ReferenceId);
                    await GetItemReferences(Item);
                    DialogResult = new DialogResult { Success = true, Message = "Referencia eliminada correctamente." };
                    await ItemReferencesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteItemReference));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la referencia"
                });
            }
        }
        #endregion
    }
}