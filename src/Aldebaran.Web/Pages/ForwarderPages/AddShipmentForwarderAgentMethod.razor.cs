using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class AddShipmentForwarderAgentMethod : ComponentBase
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IForwarderAgentService ForwarderAgentService { get; set; }
        [Inject]
        protected IShipmentForwarderAgentMethodService ShipmentForwarderAgentMethodService { get; set; }
        [Inject]
        protected IShipmentMethodService ShipmentMethodService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int FORWARDER_AGENT_ID { get; set; }
        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.ShipmentMethod> AvailableShipmentMethodsForSelection;
        protected ServiceModel.ForwarderAgent ForwarderAgent;
        protected ServiceModel.ShipmentForwarderAgentMethod ShipmentForwarderAgentMethod;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ForwarderAgent = await ForwarderAgentService.FindAsync(FORWARDER_AGENT_ID);
                var currentShipmentMethodsForAgent = await ShipmentForwarderAgentMethodService.GetByForwarderAgentIdAsync(FORWARDER_AGENT_ID);
                // Metodos de envio disponibles para seleccion. Metodos de envio excepto los ya seleccionados
                var shipmentMethods = await ShipmentMethodService.GetAsync();
                AvailableShipmentMethodsForSelection = shipmentMethods.Where(w => !currentShipmentMethodsForAgent.Any(x => x.ShipmentMethodId == w.ShipmentMethodId)).ToList();
                ShipmentForwarderAgentMethod = new ServiceModel.ShipmentForwarderAgentMethod
                {
                    ForwarderAgentId = FORWARDER_AGENT_ID
                };
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await ShipmentForwarderAgentMethodService.AddAsync(ShipmentForwarderAgentMethod);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}