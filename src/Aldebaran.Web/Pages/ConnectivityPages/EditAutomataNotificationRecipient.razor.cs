using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Aldebaran.Application.Services.Services;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public partial class EditAutomataNotificationRecipient
    {
        [Parameter]
        public ConnectivityType? ParentType { get; set; }
        [Parameter]
        public int Id { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IAutomataNotificationRecipientService AutomataNotificationRecipientService { get; set; }

        [Inject]
        protected ILogger<EditAutomataNotificationRecipient> Logger { get; set; }

        protected AutomataNotificationRecipient Recipient = null!;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected IReadOnlyList<SelectItem> NotificationTypes
        {
            get
            {
                var all = new List<SelectItem>
                {
                    new SelectItem { Text = "Conectividad caída", Value = "CONNECTIVITY_DOWN" },
                    new SelectItem { Text = "Conectividad recuperada", Value = "CONNECTIVITY_RECOVERED" },
                    new SelectItem { Text = "Error de negocio", Value = "BUSINESS_ERROR" }
                };

                if (ParentType == ConnectivityType.FtpWriting)
                {
                    return all.Where(w => w.Value == "CONNECTIVITY_DOWN").ToList();
                }

                return all;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Recipient = await AutomataNotificationRecipientService.GetByIdAsync(Id);
                if (ParentType == ConnectivityType.FtpWriting && string.IsNullOrEmpty(Recipient.NotificationType))
                {
                    Recipient.NotificationType = "CONNECTIVITY_DOWN";
                }
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
                await AutomataNotificationRecipientService.UpdateAsync(Recipient);
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
