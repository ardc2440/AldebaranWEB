using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class Forwarders : ComponentBase
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

        protected IEnumerable<Models.AldebaranDb.Forwarder> forwarders;
        protected RadzenDataGrid<Models.AldebaranDb.Forwarder> grid0;
        protected string search = "";
        protected DialogResult dialogResult { get; set; }
        protected Models.AldebaranDb.Forwarder forwarder;
        protected RadzenDataGrid<Models.AldebaranDb.ForwarderAgent> ForwarderAgentsDataGrid;
        protected RadzenDataGrid<Models.AldebaranDb.ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethodDataGrid;
        protected Models.AldebaranDb.ShipmentForwarderAgentMethod shipmentMethod;
        protected Models.AldebaranDb.ForwarderAgent forwarderAgent;

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);
            forwarders = await AldebaranDbService.GetForwarders(new Query { Filter = $@"i => i.FORWARDER_NAME.Contains(@0) || i.PHONE1.Contains(@0) || i.PHONE2.Contains(@0) || i.FAX.Contains(@0) || i.FORWARDER_ADDRESS.Contains(@0) || i.MAIL1.Contains(@0) || i.MAIL2.Contains(@0)", FilterParameters = new object[] { search }, Expand = "City.Department.Country" });
        }
        protected override async Task OnInitializedAsync()
        {
            forwarders = await AldebaranDbService.GetForwarders(new Query { Filter = $@"i => i.FORWARDER_NAME.Contains(@0) || i.PHONE1.Contains(@0) || i.PHONE2.Contains(@0) || i.FAX.Contains(@0) || i.FORWARDER_ADDRESS.Contains(@0) || i.MAIL1.Contains(@0) || i.MAIL2.Contains(@0)", FilterParameters = new object[] { search }, Expand = "City.Department.Country" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddForwarder>("Nueva transportadora");
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Transportadora creada correctamente." };
            }
            await grid0.Reload();
        }

        protected async Task EditRow(Models.AldebaranDb.Forwarder args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditForwarder>("Actualizar transportadora", new Dictionary<string, object> { { "FORWARDER_ID", args.FORWARDER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Transportadora actualizada correctamente." };
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Forwarder forwarder)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar esta transportadora?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteForwarder(forwarder.FORWARDER_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Transportadora eliminada correctamente." };
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
                    Detail = $"No se ha podido eliminar la transportadora"
                });
            }
        }

        protected async Task GetChildData(Models.AldebaranDb.Forwarder args)
        {
            forwarder = args;
            var ForwarderAgentsResult = await AldebaranDbService.GetForwarderAgents(new Query { Filter = $@"i => i.FORWARDER_ID == {args.FORWARDER_ID}", Expand = "City.Department.Country,Forwarder" });
            if (ForwarderAgentsResult != null)
            {
                args.ForwarderAgents = ForwarderAgentsResult.ToList();
            }
        }

        protected async Task ForwarderAgentsAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Forwarder data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddForwarderAgent>("Nuevo agente", new Dictionary<string, object> { { "FORWARDER_ID", data.FORWARDER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Agente creado correctamente." };
            }
            await GetChildData(data);
            await ForwarderAgentsDataGrid.Reload();
        }

        protected async Task EditChildRow(Models.AldebaranDb.ForwarderAgent args, Models.AldebaranDb.Forwarder data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditForwarderAgent>("Actualizar agente", new Dictionary<string, object> { { "FORWARDER_AGENT_ID", args.FORWARDER_AGENT_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Agente actualizado correctamente." };
            }
            await GetChildData(data);
            await ForwarderAgentsDataGrid.Reload();
        }

        protected async Task ForwarderAgentsDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.ForwarderAgent forwarderAgent)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este agente?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteForwarderAgent(forwarderAgent.FORWARDER_AGENT_ID);
                    await GetChildData(forwarder);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Agente eliminado correctamente." };
                        await ForwarderAgentsDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el agente"
                });
            }
        }

        protected async Task GetShipmentData(Models.AldebaranDb.ForwarderAgent args)
        {
            forwarderAgent = args;
            var ShipmentForwarderAgentMethodsResult = await AldebaranDbService.GetShipmentForwarderAgentMethods(new Query { Filter = $@"i => i.FORWARDER_AGENT_ID == {args.FORWARDER_AGENT_ID}", Expand = "ShipmentMethod" });
            if (ShipmentForwarderAgentMethodsResult != null)
            {
                forwarderAgent.ShipmentForwarderAgentMethods = ShipmentForwarderAgentMethodsResult.ToList();
            }
        }
        protected async Task ShippingAddButtonClick(MouseEventArgs args, Models.AldebaranDb.ForwarderAgent data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddShipmentForwarderAgentMethod>("Nuevo método de envío", new Dictionary<string, object> { { "FORWARDER_AGENT_ID", data.FORWARDER_AGENT_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Método de envío creado correctamente." };
            }
            await GetShipmentData(data);
            await ForwarderAgentsDataGrid.Reload();
        }
        protected async Task ShippingDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.ShipmentForwarderAgentMethod shipmentForwarderAgent)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar este método de envío?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteShipmentForwarderAgentMethod(shipmentForwarderAgent.SHIPMENT_FORWARDER_AGENT_METHOD_ID);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Método de envío eliminado correctamente." };
                        await ShipmentForwarderAgentMethodDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el método de envío"
                });
            }
        }
    }
}