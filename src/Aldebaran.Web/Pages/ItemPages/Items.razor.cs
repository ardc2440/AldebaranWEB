using Aldebaran.Application.Services;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
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
        protected TooltipService TooltipService { get; set; }

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
        protected bool isLoadingInProgress;

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events

        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip.Value;
            top = args.Top.Value;
            await GetItemsAsync(search);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetItemsAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (ItemsList, count) = string.IsNullOrEmpty(searchKey) ? await ItemService.GetAsync(skip, top, ct) : await ItemService.GetAsync(skip, top, searchKey, ct);
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ItemsDataGrid.GoToPage(0);
            await GetItemsAsync(search);
        }
        
        protected async Task AddItem(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddItem>("Nuevo artículo");
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Artículo",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Artículo creado correctamente."
                });
            }
            await GetItemsAsync(search);
            await ItemsDataGrid.Reload();
        }
        protected async Task EditItem(ServiceModel.Item item)
        {
            var result = await DialogService.OpenAsync<EditItem>("Actualizar artículo", new Dictionary<string, object> { { "ITEM_ID", item.ItemId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Artículo",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Artículo actualizado correctamente."
                });
            }
            await GetItemsAsync(search);
            await ItemsDataGrid.Reload();
        }
        protected async Task DeleteItem(MouseEventArgs args, ServiceModel.Item item)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este artículo?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ItemService.DeleteAsync(item.ItemId);
                    await GetItemsAsync(search);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Artículo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Artículo eliminado correctamente."
                    });
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
            await Task.Yield();
            var ItemReferencesResult = await ItemReferenceService.GetByItemIdAsync(args.ItemId);
            args.ItemReferences = ItemReferencesResult.ToList();
        }
        protected async Task AddItemReference(ServiceModel.Item data)
        {
            var result = await DialogService.OpenAsync<AddItemReference>("Nueva referencia", new Dictionary<string, object> { { "ITEM_ID", data.ItemId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Referencia",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Referencia creada correctamente."
                });
            }
            await GetItemReferences(data);
            await ItemReferencesDataGrid.Reload();
        }
        protected async Task EditItemReference(ServiceModel.ItemReference args, ServiceModel.Item data)
        {
            var result = await DialogService.OpenAsync<EditItemReference>("Actualizar referencia", new Dictionary<string, object> { { "REFERENCE_ID", args.ReferenceId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Referencia",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Referencia actualizada correctamente."
                });
            }
            await GetItemReferences(data);
            await ItemReferencesDataGrid.Reload();
        }
        protected async Task DeleteItemReference(MouseEventArgs args, ServiceModel.ItemReference itemReference)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ItemReferenceService.DeleteAsync(itemReference.ReferenceId);
                    await GetItemReferences(Item);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Referencia",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Referencia eliminada correctamente."
                    });
                    await ItemReferencesDataGrid.Reload();
                }
            }
            catch (DbUpdateException ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Existen movimientos para esta referencia y no se puede eliminar."
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteItemReference));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la referencia."
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