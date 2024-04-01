using Aldebaran.Application.Services;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class Providers
    {

        #region Injections
        [Inject]
        protected ILogger<Providers> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected IProviderReferenceService ProviderReferenceService { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.Provider> ProvidersList;
        protected LocalizedDataGrid<ServiceModel.Provider> ProvidersGrid;
        protected ServiceModel.Provider Provider;
        protected LocalizedDataGrid<ServiceModel.ProviderReference> ProviderReferencesDataGrid;
        protected string search = "";
        protected bool IsLoadingInProgress;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoadingInProgress = true;
                await GetProvidersAsync();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetProvidersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            ProvidersList = string.IsNullOrEmpty(searchKey) ? await ProviderService.GetAsync(ct) : await ProviderService.GetAsync(searchKey, ct);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ProvidersGrid.GoToPage(0);
            await GetProvidersAsync(search);
        }
        protected async Task AddProvider(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddProvider>("Nuevo proveedor", null);
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Proveedor",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Proveedor creado correctamente."
                });
            }
            await GetProvidersAsync();
            await ProvidersGrid.Reload();
        }
        protected async Task EditProvider(ServiceModel.Provider args)
        {
            var result = await DialogService.OpenAsync<EditProvider>("Actualizar proveedor", new Dictionary<string, object> { { "PROVIDER_ID", args.ProviderId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Proveedor",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Proveedor actualizado correctamente."
                });
            }
            await GetProvidersAsync();
            await ProvidersGrid.Reload();
        }
        protected async Task DeleteProvider(MouseEventArgs args, ServiceModel.Provider provider)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este proveedor?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ProviderService.DeleteAsync(provider.ProviderId);
                    await GetProvidersAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Proveedor",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Proveedor eliminado correctamente."
                    });
                    await ProvidersGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteProvider));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el proveedor."
                });
            }
        }

        void OnProviderReferenceRender(DataGridRenderEventArgs<ServiceModel.ProviderReference> args)
        {
            if (args.FirstRender)
            {
                args.Grid.Groups.Add(new GroupDescriptor() { Title = "Línea", Property = "ItemReference.Item.Line.LineName", SortOrder = SortOrder.Descending });
                args.Grid.Groups.Add(new GroupDescriptor() { Title = "Artículo", Property = "ItemReference.Item.ItemName", SortOrder = SortOrder.Descending });
                StateHasChanged();
            }
        }
        protected async Task GetProviderReferences(ServiceModel.Provider args)
        {
            Provider = args;
            try
            {
                IsLoadingInProgress = true;
                await Task.Yield();
                var providerReferencesResult = await ProviderReferenceService.GetByProviderIdAsync(args.ProviderId);
                args.ProviderReferences = providerReferencesResult.ToList();
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
        protected async Task AddProviderReference(MouseEventArgs args, ServiceModel.Provider data)
        {
            var result = await DialogService.OpenAsync<AddProviderReference>("Agregar referencia", new Dictionary<string, object> { { "PROVIDER_ID", data.ProviderId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Referencia",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Referencia agregada correctamente al proveedor."
                });
            }
            await GetProviderReferences(data);
            await ProviderReferencesDataGrid.Reload();
        }
        protected async Task DeleteProviderReference(MouseEventArgs args, ServiceModel.ProviderReference providerReference)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia del proveedor?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ProviderReferenceService.DeleteAsync(providerReference.ProviderId, providerReference.ReferenceId);
                    await GetProviderReferences(Provider);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Referencia",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Referencia eliminada correctamente al proveedor."
                    });
                    await ProviderReferencesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteProviderReference));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la referencia el proveedor."
                });
            }
        }
        #endregion
    }
}