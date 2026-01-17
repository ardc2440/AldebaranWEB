using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public partial class AddInventoryAutomationConnection
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IInventoryAutomationConnectionService InventoryAutomationConnectionService { get; set; }

        [Inject]
        protected ILogger<AddInventoryAutomationConnection> Logger { get; set; }

        protected InventoryAutomationConnection Connection = new();
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        protected async Task FormSubmit()
        {
            if (IsSubmitInProgress) return;

            try
            {
                IsSubmitInProgress = true;
                await InventoryAutomationConnectionService.CreateAsync(Connection);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                // Manejo de error similar a otras pantallas si se requiere
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected void CancelButtonClick() => DialogService.Close(false);
    }
}
