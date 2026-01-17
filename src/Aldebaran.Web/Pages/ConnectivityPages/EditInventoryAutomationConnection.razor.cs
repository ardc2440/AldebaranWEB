using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public partial class EditInventoryAutomationConnection
    {
        [Parameter]
        public int INVENTORY_AUTOMATION_CONNECTION_ID { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IInventoryAutomationConnectionService InventoryAutomationConnectionService { get; set; }

        [Inject]
        protected ILogger<EditInventoryAutomationConnection> Logger { get; set; }

        protected InventoryAutomationConnection Connection;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        protected List<string> ValidationErrors = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Connection = await InventoryAutomationConnectionService.GetByIdAsync(INVENTORY_AUTOMATION_CONNECTION_ID);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        protected async Task FormSubmit()
        {
            if (IsSubmitInProgress) return;

            try
            {
                IsErrorVisible = false;
                ValidationErrors.Clear();

                IsSubmitInProgress = true;
                await InventoryAutomationConnectionService.UpdateAsync(Connection);
                DialogService.Close(true);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarning(ex, nameof(FormSubmit));
                ValidationErrors.Add(ex.Message);
                IsErrorVisible = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                ValidationErrors.Add("Se produjo un error al actualizar la conexión.");
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected void CancelButtonClick() => DialogService.Close(false);
    }
}
