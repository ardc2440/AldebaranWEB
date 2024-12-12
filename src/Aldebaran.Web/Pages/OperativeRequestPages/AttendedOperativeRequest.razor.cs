using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.OperativeRequestPages
{
    public partial class AttendedOperativeRequest
    {
        #region Injections

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ICancellationRequestService CancellationRequestService { get; set; }

        #endregion

        #region Variables

        protected bool isLoadingInProgress;
        protected string search = "";
        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        protected LocalizedDataGrid<CancellationRequestModel> cancellationRequestGrid;
        protected IEnumerable<CancellationRequestModel> cancellationRequests;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await Task.Yield();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
            await GetAttendedOperationalRequestAsync();
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await cancellationRequestGrid.GoToPage(0);
            await GetAttendedOperationalRequestAsync();
        }

        protected async Task GetAttendedOperationalRequestAsync() 
        {
            await Task.Yield();
            (cancellationRequests, count)  = await CancellationRequestService.GetAllByStatusOrderAsync(skip, top, search, await GetStatusFilter(), false);
        }

        public async Task<string> GetStatusFilter()
        {
            var conditions = new List<string>();

            // Validaciones
            if (Security.IsInRole("Administrador", "Consulta de solicitudes de cancelación de ordenes de compra"))
            {
                conditions.Add("C"); // Agregar condición para "C"
            }

            if (Security.IsInRole("Administrador", "Consulta de solicitudes de cancelación de pedidos"))
            {
                conditions.Add("E"); // Agregar condición para "E"
            }

            if (Security.IsInRole("Administrador", "Consulta de solicitudes de cierre de pedidos"))
            {
                conditions.Add("F"); // Agregar condición para "F"
            }

            // Generar filtro SQL dinámico
            if (conditions.Any())
            {
                return string.Join(",", conditions);
            }

            return "X";
        }

        #endregion
    }
}
