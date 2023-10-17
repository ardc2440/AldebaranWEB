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

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class Forwarders
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

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Forwarder> forwarders;

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.Forwarder> grid0;

        protected string search = "";

        protected DialogResult dialogResult { get; set; }
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
            var result = await DialogService.OpenAsync<AddForwarder>("Nueva transportadora", null);
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Transportadora creada correctamente." };
            }
            await grid0.Reload();
        }

        protected async Task EditRow(Aldebaran.Web.Models.AldebaranDb.Forwarder args)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditForwarder>("Actualizar transportadora", new Dictionary<string, object> { { "FORWARDER_ID", args.FORWARDER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Transportadora actualizada correctamente." };
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder)
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

        protected Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder;
        protected async Task GetChildData(Aldebaran.Web.Models.AldebaranDb.Forwarder args)
        {
            forwarder = args;
            var ForwarderAgentsResult = await AldebaranDbService.GetForwarderAgents(new Query { Filter = $@"i => i.FORWARDER_ID == {args.FORWARDER_ID}", Expand = "City.Department.Country,Forwarder" });
            if (ForwarderAgentsResult != null)
            {
                args.ForwarderAgents = ForwarderAgentsResult.ToList();
            }
        }

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.ForwarderAgent> ForwarderAgentsDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task ForwarderAgentsAddButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.Forwarder data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddForwarderAgent>("Nuevo agente", new Dictionary<string, object> { { "CITY_ID", data.CITY_ID }, { "FORWARDER_ID", data.FORWARDER_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Agente creado correctamente." };
            }
            await GetChildData(data);
            await ForwarderAgentsDataGrid.Reload();
        }

        protected async Task EditChildRow(Aldebaran.Web.Models.AldebaranDb.ForwarderAgent args, Aldebaran.Web.Models.AldebaranDb.Forwarder data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditForwarderAgent>("Actualizar agente", new Dictionary<string, object> { { "CITY_ID", args.CITY_ID }, { "FORWARDER_ID", args.FORWARDER_ID }, { "FORWARDER_AGENT_ID", args.FORWARDER_AGENT_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Agente actualizado correctamente." };
            }
            await GetChildData(data);
            await ForwarderAgentsDataGrid.Reload();
        }

        protected async Task ForwarderAgentsDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.ForwarderAgent forwarderAgent)
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea eliminar este agente?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteForwarderAgent(forwarderAgent.FORWARDER_AGENT_ID);

                    await GetChildData(forwarder);

                    if (deleteResult != null)
                    {
                        await ForwarderAgentsDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el agente"
                });
            }
        }
    }
}