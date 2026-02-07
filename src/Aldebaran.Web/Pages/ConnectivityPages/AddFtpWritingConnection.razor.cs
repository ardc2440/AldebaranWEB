using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public partial class AddFtpWritingConnection
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFtpWritingConnectionService FtpWritingConnectionService { get; set; }

        [Inject]
        protected ILogger<AddFtpWritingConnection> Logger { get; set; }

        protected FtpWritingConnection Connection = new();
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        protected async Task FormSubmit()
        {
            if (IsSubmitInProgress) return;

            try
            {
                IsSubmitInProgress = true;
                await FtpWritingConnectionService.CreateAsync(Connection);
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
