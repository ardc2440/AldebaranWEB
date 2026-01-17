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
        protected bool IsErrorVisible;
        protected List<string> ValidationErrors = new();

        protected async Task FormSubmit()
        {
            if (IsSubmitInProgress) return;

            try
            {
                IsErrorVisible = false;
                ValidationErrors.Clear();

                IsSubmitInProgress = true;
                await InventoryAutomationConnectionService.CreateAsync(Connection);
                DialogService.Close(true);
            }
            catch (InvalidOperationException ex)
            {
                // Regla de unicidad rota
                Logger.LogWarning(ex, nameof(FormSubmit));
                ValidationErrors.Add(ex.Message);
                IsErrorVisible = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                ValidationErrors.Add("Se produjo un error al crear la conexión.");
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
