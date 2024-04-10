using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class Forwarders : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<Forwarders> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        public IForwarderService ForwarderService { get; set; }

        [Inject]
        public IForwarderAgentService ForwarderAgentService { get; set; }

        [Inject]
        public IShipmentForwarderAgentMethodService ShipmentForwarderAgentMethodService { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.Forwarder> ForwardersList;
        protected RadzenDataGrid<ServiceModel.Forwarder> ForwardersDataGrid;
        protected ServiceModel.Forwarder Forwarder;
        protected RadzenDataGrid<ServiceModel.ForwarderAgent> ForwarderAgentsDataGrid;
        protected ServiceModel.ForwarderAgent ForwarderAgent;
        protected RadzenDataGrid<ServiceModel.ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethodDataGrid;
        protected ServiceModel.ShipmentForwarderAgentMethod ShipmentMethod;
        protected string search = "";
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetForwardersAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetForwardersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            ForwardersList = string.IsNullOrEmpty(searchKey) ? await ForwarderService.GetAsync(ct) : await ForwarderService.GetAsync(searchKey, ct);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await ForwardersDataGrid.GoToPage(0);
            await GetForwardersAsync(search);
        }
        protected async Task AddForwarder(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddForwarder>("Nueva transportadora");
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Transportadora",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Transportadora creada correctamente."
                });
            }
            await GetForwardersAsync();
            await ForwardersDataGrid.Reload();
        }
        protected async Task EditForwarder(ServiceModel.Forwarder args)
        {
            var result = await DialogService.OpenAsync<EditForwarder>("Actualizar transportadora", new Dictionary<string, object> { { "FORWARDER_ID", args.ForwarderId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Transportadora",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Transportadora actualizada correctamente."
                });
            }
            await GetForwardersAsync();
            await ForwardersDataGrid.Reload();
        }
        protected async Task DeleteForwarder(MouseEventArgs args, ServiceModel.Forwarder forwarder)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar esta transportadora?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ForwarderService.DeleteAsync(forwarder.ForwarderId);
                    await GetForwardersAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Transportadora",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Transportadora eliminada correctamente."
                    });
                    await ForwardersDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteForwarder));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar la transportadora."
                });
            }
        }

        protected async Task GetForwarderAgents(ServiceModel.Forwarder args)
        {
            Forwarder = args;
            await Task.Yield();
            var forwarderAgentsResult = await ForwarderAgentService.GetByForwarderIdAsync(args.ForwarderId);
            args.ForwarderAgents = forwarderAgentsResult.ToList();
        }
        protected async Task AddForwarderAgent(MouseEventArgs args, ServiceModel.Forwarder data)
        {
            var result = await DialogService.OpenAsync<AddForwarderAgent>("Nuevo agente", new Dictionary<string, object> { { "FORWARDER_ID", data.ForwarderId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Agente",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Agente creado correctamente."
                });
            }
            await GetForwarderAgents(data);
            await ForwarderAgentsDataGrid.Reload();
        }
        protected async Task EditForwarderAgent(ServiceModel.ForwarderAgent args, ServiceModel.Forwarder data)
        {
            var result = await DialogService.OpenAsync<EditForwarderAgent>("Actualizar agente", new Dictionary<string, object> { { "FORWARDER_AGENT_ID", args.ForwarderAgentId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Agente",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Agente actualizado correctamente."
                });
            }
            await GetForwarderAgents(data);
            await ForwarderAgentsDataGrid.Reload();
        }
        protected async Task DeleteForwarderAgent(MouseEventArgs args, ServiceModel.ForwarderAgent forwarderAgent)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este agente?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ForwarderAgentService.DeleteAsync(forwarderAgent.ForwarderAgentId);
                    await GetForwarderAgents(Forwarder);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Agente",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Agente eliminado correctamente."
                    });
                    await ForwarderAgentsDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteForwarderAgent));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el agente."
                });
            }
        }

        protected async Task GetShipmentForwarderAgentMethods(ServiceModel.ForwarderAgent args)
        {
            ForwarderAgent = args;
            await Task.Yield();
            var shipmentForwarderAgentMethodsResult = await ShipmentForwarderAgentMethodService.GetByForwarderAgentIdAsync(args.ForwarderAgentId);
            ForwarderAgent.ShipmentForwarderAgentMethods = shipmentForwarderAgentMethodsResult.ToList();
        }
        protected async Task AddShipmentForwarderAgentMethod(MouseEventArgs args, ServiceModel.ForwarderAgent data)
        {
            var result = await DialogService.OpenAsync<AddShipmentForwarderAgentMethod>("Nuevo método de envío", new Dictionary<string, object> { { "FORWARDER_AGENT_ID", data.ForwarderAgentId } });
            if (result == true)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Método de envío",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Método de envío creado correctamente."
                });
            }
            await GetShipmentForwarderAgentMethods(data);
            await ForwarderAgentsDataGrid.Reload();
        }
        protected async Task DeleteShipmentForwarderAgentMethod(MouseEventArgs args, ServiceModel.ShipmentForwarderAgentMethod shipmentForwarderAgent)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este método de envío?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
                {
                    await ShipmentForwarderAgentMethodService.DeleteAsync(shipmentForwarderAgent.ShipmentForwarderAgentMethodId);
                    await GetShipmentForwarderAgentMethods(ForwarderAgent);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Método de envío",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Método de envío eliminado correctamente."
                    });
                    await ForwarderAgentsDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DeleteShipmentForwarderAgentMethod));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el método de envío."
                });
            }
        }
        #endregion
    }
}