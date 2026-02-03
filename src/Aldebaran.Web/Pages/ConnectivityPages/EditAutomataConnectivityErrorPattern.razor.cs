using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Aldebaran.Application.Services.Services;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public partial class EditAutomataConnectivityErrorPattern
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IAutomataConnectivityErrorPatternService AutomataConnectivityErrorPatternService { get; set; }

        [Inject]
        protected ILogger<EditAutomataConnectivityErrorPattern> Logger { get; set; }

        protected AutomataConnectivityErrorPattern Pattern = null!;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected IReadOnlyList<SelectItem> Targets { get; } = new List<SelectItem>
        {
            new SelectItem { Text = "Destino", Value = "D" },
            new SelectItem { Text = "Origen", Value = "O" },
            new SelectItem { Text = "Ambos", Value = "B" }
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Pattern = await AutomataConnectivityErrorPatternService.GetByIdAsync(Id);
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
                IsSubmitInProgress = true;
                await AutomataConnectivityErrorPatternService.UpdateAsync(Pattern);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected void CancelButtonClick() => DialogService.Close(false);
    }
}
