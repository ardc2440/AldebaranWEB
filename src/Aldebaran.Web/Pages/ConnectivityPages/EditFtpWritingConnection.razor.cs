using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ConnectivityPages
{
    public partial class EditFtpWritingConnection
    {
        [Parameter]
        public int FTP_WRITING_CONNECTION_ID { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFtpWritingConnectionService FtpWritingConnectionService { get; set; }

        [Inject]
        protected ILogger<EditFtpWritingConnection> Logger { get; set; }

        protected FtpWritingConnection Connection;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Connection = await FtpWritingConnectionService.GetByIdAsync(FTP_WRITING_CONNECTION_ID);
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

        protected void FormSubmit()
        {
            if (IsSubmitInProgress) return;

            try
            {
                IsSubmitInProgress = true;
                FtpWritingConnectionService.UpdateAsync(Connection);
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
