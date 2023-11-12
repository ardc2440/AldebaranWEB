using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class Providers
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

        protected IEnumerable<Models.AldebaranDb.Provider> providers;
        protected RadzenDataGrid<Models.AldebaranDb.Provider> grid0;
        protected string search = "";
        protected Models.AldebaranDb.Provider provider;
        protected RadzenDataGrid<Models.AldebaranDb.ProviderReference> ProviderReferencesDataGrid;
        protected DialogResult dialogResult { get; set; }

        protected bool isLoadingInProgress;

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);
            providers = await AldebaranDbService.GetProviders(new Query { Filter = $@"i => i.IDENTITY_NUMBER.Contains(@0) || i.PROVIDER_CODE.Contains(@0) || i.PROVIDER_NAME.Contains(@0) || i.PROVIDER_ADDRESS.Contains(@0) || i.PHONE.Contains(@0) || i.FAX.Contains(@0) || i.EMAIL.Contains(@0) || i.CONTACT_PERSON.Contains(@0)", FilterParameters = new object[] { search }, Expand = "City.Department.Country,IdentityType" });
        }
        void OnRender(DataGridRenderEventArgs<Models.AldebaranDb.ProviderReference> args)
        {
            if (args.FirstRender)
            {
                args.Grid.Groups.Add(new GroupDescriptor() { Title = "Línea", Property = "ItemReference.Item.Line.LINE_NAME", SortOrder = SortOrder.Descending });
                args.Grid.Groups.Add(new GroupDescriptor() { Title = "Artículo", Property = "ItemReference.Item.ITEM_NAME", SortOrder = SortOrder.Descending });
                StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                providers = await AldebaranDbService.GetProviders(new Query { Filter = $@"i => i.IDENTITY_NUMBER.Contains(@0) || i.PROVIDER_CODE.Contains(@0) || i.PROVIDER_NAME.Contains(@0) || i.PROVIDER_ADDRESS.Contains(@0) || i.PHONE.Contains(@0) || i.FAX.Contains(@0) || i.EMAIL.Contains(@0) || i.CONTACT_PERSON.Contains(@0)", FilterParameters = new object[] { search }, Expand = "City.Department.Country,IdentityType" });
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddProvider>("Nuevo proveedor", null);
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Proveedor creado correctamente." };
            }
            await grid0.Reload();
        }

        protected async Task EditRow(Models.AldebaranDb.Provider args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditProvider>("Actualizar proveedor", new Dictionary<string, object> { { "PROVIDER_ID", args.PROVIDER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Proveedor actualizado correctamente." };
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Provider provider)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este proveedor?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteProvider(provider.PROVIDER_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Proveedor eliminado correctamente." };
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
                    Detail = $"No se ha podido eliminar el proveedor"
                });
            }
        }

        protected async Task GetChildData(Models.AldebaranDb.Provider args)
        {
            provider = args;
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();
                var ProviderReferencesResult = await AldebaranDbService.GetProviderReferences(new Query { Filter = $@"i => i.PROVIDER_ID == {args.PROVIDER_ID}", Expand = "Provider,ItemReference.Item.Line" });
                if (ProviderReferencesResult != null)
                {
                    args.ProviderReferences = ProviderReferencesResult.ToList();
                }
            }
            finally { isLoadingInProgress = false; }
        }

        protected async Task ProviderReferencesAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Provider data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddProviderReference>("Agregar referencia", new Dictionary<string, object> { { "PROVIDER_ID", data.PROVIDER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Referencia agregada correctamente al proveedor." };
            }
            await GetChildData(data);
            await ProviderReferencesDataGrid.Reload();
        }

        protected async Task ProviderReferencesDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.ProviderReference providerReference)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia del proveedor?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteProviderReference(providerReference.REFERENCE_ID, providerReference.PROVIDER_ID);
                    await GetChildData(provider);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Referencia eliminada del proveedor correctamente." };
                        await ProviderReferencesDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la referencia el proveedor"
                });
            }
        }
    }
}