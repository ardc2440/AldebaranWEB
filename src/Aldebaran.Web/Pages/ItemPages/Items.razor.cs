using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Aldebaran.Web.Models;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class Items
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

        protected IEnumerable<Models.AldebaranDb.Item> items;

        protected RadzenDataGrid<Models.AldebaranDb.Item> grid0;

        protected string search = "";
        protected DialogResult dialogResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            items = await AldebaranDbService.GetItems(new Query { Filter = $@"i => i.INTERNAL_REFERENCE.Contains(@0) || i.ITEM_NAME.Contains(@0) || i.PROVIDER_REFERENCE.Contains(@0) || i.PROVIDER_ITEM_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "MeasureUnit,Currency,MeasureUnit1,Line" });
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            items = await AldebaranDbService.GetItems(new Query { Filter = $@"i => i.INTERNAL_REFERENCE.Contains(@0) || i.ITEM_NAME.Contains(@0) || i.PROVIDER_REFERENCE.Contains(@0) || i.PROVIDER_ITEM_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "MeasureUnit,Currency,MeasureUnit1,Line" });
        }


        protected async Task AddButtonClick(MouseEventArgs args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddItem>("Nuevo artículo", null);
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Artículo creado correctamente." };
            }
            await grid0.Reload();
        }

        protected async Task EditRow(Models.AldebaranDb.Item item)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditItem>("Actualizar artículo", new Dictionary<string, object> { { "ITEM_ID", item.ITEM_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Artículo actualizado correctamente." };
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Item item)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este artículo?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItem(item.ITEM_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Artículo eliminado correctamente." };
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
                    Detail = $"No se ha podido eliminar el artículo"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await AldebaranDbService.ExportItemsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "MeasureUnit,Currency,MeasureUnit1,Line",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Items");
            }

            if (args == null || args.Value == "xlsx")
            {
                await AldebaranDbService.ExportItemsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "MeasureUnit,Currency,MeasureUnit1,Line",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Items");
            }
        }

        protected Models.AldebaranDb.Item item;
        protected async Task GetChildData(Models.AldebaranDb.Item args)
        {
            item = args;
            var ItemReferencesResult = await AldebaranDbService.GetItemReferences(new Query { Filter = $@"i => i.ITEM_ID == {args.ITEM_ID}", Expand = "Item" });
            if (ItemReferencesResult != null)
            {
                args.ItemReferences = ItemReferencesResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.ItemReference> ItemReferencesDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task ItemReferencesAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Item data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddItemReference>("Nueva referencia", new Dictionary<string, object> { { "ITEM_ID", data.ITEM_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Referencia creada correctamente." };
            }
            await GetChildData(data);
            await ItemReferencesDataGrid.Reload();
        }

        protected async Task ItemReferencesRowSelect(DataGridRowMouseEventArgs<Aldebaran.Web.Models.AldebaranDb.ItemReference> args, Aldebaran.Web.Models.AldebaranDb.Item data)
        {
            var dialogResult = await DialogService.OpenAsync<EditItemReference>("Edit ItemReferences", new Dictionary<string, object> { { "REFERENCE_ID", args.Data.REFERENCE_ID } });
            await GetChildData(data);
            await ItemReferencesDataGrid.Reload();
        }

        protected async Task EditChildRow(Models.AldebaranDb.ItemReference args, Models.AldebaranDb.Item data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditItemReference>("Actualizar referencia", new Dictionary<string, object> { { "REFERENCE_ID", args.REFERENCE_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Referencia actualizada correctamente." };
            }
            await GetChildData(data);
            await ItemReferencesDataGrid.Reload();
        }

        protected async Task ItemReferencesDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.ItemReference itemReference)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItemReference(itemReference.REFERENCE_ID);
                    await GetChildData(item);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Referencia eliminada correctamente." };
                        await ItemReferencesDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la referencia"
                });
            }
        }
    }
}